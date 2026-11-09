using UnityEngine;
using AnutoTD.Data;
using AnutoTD.Combat;
using AnutoTD.Enemies;
using AnutoTD.Map;
using AnutoTD.Core;

namespace AnutoTD.Towers
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class TowerController : MonoBehaviour
    {
        [Header("Cấu hình tháp")]
        [SerializeField] private TowerDataSO towerData;

        [Header("Nòng súng & Điểm bắn")]
        [SerializeField] private SpriteRenderer gunRenderer;
        [SerializeField] private Transform firePoint;

        [Header("Góc bù trừ nòng súng")]
        public float rotationOffset = -90f;

        // Chỉ số có thể nâng cấp
        private int currentLevel = 1;
        private float currentDamage;
        private float currentRange;
        private float currentFireRate;
        private TargetStrategy currentStrategy = TargetStrategy.First;

        private PlateauSlot parentPlateau;
        private Transform currentTarget;
        private float fireCooldown = 0f;
        private LineRenderer selectionRangeCircle;

        public TowerDataSO Data => towerData;
        public int Level => currentLevel;
        public float Damage => currentDamage;
        public float Range => currentRange;
        public float FireRate => currentFireRate;
        public TargetStrategy Strategy => currentStrategy;

        public static event System.Action<TowerController> OnTowerSelected;
        public static event System.Action OnTowerDeselected;

        private void Awake()
        {
            if (gunRenderer == null) gunRenderer = GetComponentInChildren<SpriteRenderer>();

            if (firePoint == null)
            {
                if (gunRenderer != null) firePoint = gunRenderer.transform.Find("FirePoint");
                if (firePoint == null) firePoint = transform.Find("FirePoint");
            }

            // Đảm bảo Collider có Trigger để nhận diện click
            CircleCollider2D col = GetComponent<CircleCollider2D>();
            if (col != null) col.isTrigger = true;

            CreateRangeCircle();
        }

        private void Start()
        {
            if (towerData != null)
            {
                currentDamage = towerData.damage;
                currentRange = towerData.attackRange;
                currentFireRate = towerData.fireRate;

                if (gunRenderer != null && towerData.gunSprite != null)
                    gunRenderer.sprite = towerData.gunSprite;
            }
        }

        public void SetPlateau(PlateauSlot plateau)
        {
            parentPlateau = plateau;
        }

        private void Update()
        {
            if (towerData == null) return;

            if (currentTarget == null || !IsTargetInRange(currentTarget))
            {
                currentTarget = FindBestTarget();
            }

            if (currentTarget != null)
            {
                RotateGunTowards(currentTarget.position);

                fireCooldown -= Time.deltaTime;
                if (fireCooldown <= 0f)
                {
                    Shoot();
                    fireCooldown = 1f / currentFireRate;
                }
            }
        }

        private void RotateGunTowards(Vector3 targetPos)
        {
            if (gunRenderer == null) return;
            Vector3 dir = targetPos - gunRenderer.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            gunRenderer.transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
        }

        private void Shoot()
        {
            if (towerData.projectilePrefab == null) return;

            Vector3 spawnPos = (firePoint != null) ? firePoint.position : transform.position;
            GameObject projObj = Instantiate(towerData.projectilePrefab, spawnPos, Quaternion.identity);

            Projectile proj = projObj.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.Initialize(currentTarget, currentDamage, towerData.projectileSpeed);
            }
        }

        // CHỌN MỤC TIÊU THEO 5 CHIẾN THUẬT (First, Last, Closest, Strongest, Weakest)
        private Transform FindBestTarget()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, currentRange);
            Transform bestTarget = null;
            float bestValue = 0f;

            foreach (var hit in hits)
            {
                EnemyController enemy = hit.GetComponent<EnemyController>();
                if (enemy == null || enemy.IsDead) continue;

                float dist = Vector2.Distance(transform.position, enemy.transform.position);

                switch (currentStrategy)
                {
                    case TargetStrategy.Closest:
                        if (bestTarget == null || dist < bestValue)
                        {
                            bestValue = dist;
                            bestTarget = enemy.transform;
                        }
                        break;

                    case TargetStrategy.Strongest:
                        if (bestTarget == null || enemy.CurrentHealth > bestValue)
                        {
                            bestValue = enemy.CurrentHealth;
                            bestTarget = enemy.transform;
                        }
                        break;

                    case TargetStrategy.Weakest:
                        if (bestTarget == null || enemy.CurrentHealth < bestValue)
                        {
                            bestValue = enemy.CurrentHealth;
                            bestTarget = enemy.transform;
                        }
                        break;

                    case TargetStrategy.First:
                    default:
                        // Tạm thời ưu tiên con đầu tiên tìm thấy trong tầm
                        if (bestTarget == null) bestTarget = enemy.transform;
                        break;
                }
            }
            return bestTarget;
        }

        private bool IsTargetInRange(Transform target)
        {
            if (target == null) return false;
            EnemyController enemy = target.GetComponent<EnemyController>();
            if (enemy != null && enemy.IsDead) return false;
            return Vector2.Distance(transform.position, target.position) <= currentRange;
        }

        // ========== CÁC HÀM TƯƠNG TÁC (CLICK, ENHANCE, SELL, TARGET) ==========

        // Khi người chơi click chuột vào tháp
        private void OnMouseDown()
        {
            SelectThisTower();
        }

        public void SelectThisTower()
        {
            ShowRangeCircle(true);
            OnTowerSelected?.Invoke(this);
        }

        public void DeselectThisTower()
        {
            ShowRangeCircle(false);
            OnTowerDeselected?.Invoke();
        }

        // Cường hóa tăng chỉ số
        public bool Enhance()
        {
            int cost = GetEnhanceCost();
            if (EconomyManager.Instance != null && EconomyManager.Instance.SpendCredits(cost))
            {
                currentLevel++;
                currentDamage *= 1.25f; // Tăng 25% sát thương
                currentRange += 0.2f;   // Tăng nhẹ tầm bắn
                DrawRangeCircle(currentRange);
                Debug.Log($"<color=green>[Cường hóa]</color> {towerData.towerName} lên Lv.{currentLevel}!");
                return true;
            }
            return false;
        }

        public int GetEnhanceCost() => (int)(towerData.cost * 0.5f * currentLevel);

        // Bán tháp
        public void Sell()
        {
            int refund = (int)(towerData.cost * 0.7f);
            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.AddCredits(refund);
            }

            if (parentPlateau != null)
            {
                parentPlateau.ClearTower();
            }

            DeselectThisTower();
            Destroy(gameObject);
        }

        // Đổi chế độ bắn
        public void CycleStrategy()
        {
            currentStrategy = (TargetStrategy)(((int)currentStrategy + 1) % 4); // Chuyển vòng tròn 4 chế độ
            currentTarget = null;
        }

        // Vòng tròn tầm bắn khi được chọn
        private void CreateRangeCircle()
        {
            selectionRangeCircle = gameObject.AddComponent<LineRenderer>();
            selectionRangeCircle.useWorldSpace = false;
            selectionRangeCircle.loop = true;
            selectionRangeCircle.positionCount = 60;
            selectionRangeCircle.startWidth = 0.05f;
            selectionRangeCircle.endWidth = 0.05f;
            selectionRangeCircle.material = new Material(Shader.Find("Sprites/Default"));
            selectionRangeCircle.startColor = new Color(0f, 0.9f, 1f, 0.6f);
            selectionRangeCircle.endColor = new Color(0f, 0.9f, 1f, 0.6f);
            DrawRangeCircle(currentRange > 0 ? currentRange : 3.5f);
            selectionRangeCircle.enabled = false;
        }

        private void DrawRangeCircle(float radius)
        {
            float deltaTheta = (2f * Mathf.PI) / 60;
            float theta = 0f;
            for (int i = 0; i < 60; i++)
            {
                float x = radius * Mathf.Cos(theta);
                float y = radius * Mathf.Sin(theta);
                selectionRangeCircle.SetPosition(i, new Vector3(x, y, 0));
                theta += deltaTheta;
            }
        }

        public void ShowRangeCircle(bool show)
        {
            if (selectionRangeCircle != null) selectionRangeCircle.enabled = show;
        }
    }
}