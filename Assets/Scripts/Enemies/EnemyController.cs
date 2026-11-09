using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnutoTD.Data;

namespace AnutoTD.Enemies
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Dữ liệu quái")]
        [SerializeField] private EnemyDataSO enemyData;

        [Header("Góc bù trừ ảnh quái")]
        public float rotationOffset = 90f;

        [Header("Hiệu ứng khi chết (Kéo prefab nổ vào đây nếu có)")]
        [SerializeField] private GameObject deathEffectPrefab;

        private List<Vector2> waypoints = new List<Vector2>();
        private int currentWaypointIndex = 0;
        private float currentHealth;
        private float currentSpeed;
        private bool isMoving = false;
        private bool isDead = false;

        private SpriteRenderer spriteRenderer;

        public EnemyDataSO Data => enemyData;
        public float CurrentHealth => currentHealth;
        public bool IsDead => isDead;

        public static event System.Action<EnemyController> OnAnyEnemyDied;
        public static event System.Action<int> OnAnyEnemyReachedGoal;
        public static event System.Action OnAnyEnemyRemoved;

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void Initialize(EnemyDataSO data, List<Vector2> path)
        {
            enemyData = data;
            waypoints = path;
            currentWaypointIndex = 0;
            isDead = false;

            if (enemyData != null)
            {
                currentHealth = enemyData.maxHealth;
                currentSpeed = enemyData.moveSpeed;

                if (spriteRenderer != null && enemyData.sprite != null)
                {
                    spriteRenderer.sprite = enemyData.sprite;
                    spriteRenderer.color = Color.white;
                }
            }

            transform.localScale = Vector3.one;

            if (waypoints != null && waypoints.Count > 0)
            {
                transform.position = new Vector3(waypoints[0].x, waypoints[0].y, 0);
                isMoving = true;
            }
        }

        private void Update()
        {
            if (!isMoving || isDead || waypoints == null || currentWaypointIndex >= waypoints.Count) return;

            Vector3 targetPos = new Vector3(waypoints[currentWaypointIndex].x, waypoints[currentWaypointIndex].y, 0);
            Vector3 moveDir = (targetPos - transform.position).normalized;

            transform.position += moveDir * currentSpeed * Time.deltaTime;

            if (moveDir != Vector3.zero)
            {
                float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
            }

            if (Vector2.Distance(transform.position, targetPos) < 0.1f)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Count)
                {
                    ReachGoal();
                }
            }
        }

        // KHI NHẬN SÁT THƯƠNG
        public void TakeDamage(float damage)
        {
            if (isDead) return;

            float finalDamage = Mathf.Max(1f, damage - enemyData.physicalArmor);
            currentHealth -= finalDamage;

            // KÍCH HOẠT HIỆU ỨNG CHỚP ĐỎ & GIẬT NGƯỜI
            StopCoroutine("HitFlashCoroutine");
            StartCoroutine("HitFlashCoroutine");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private IEnumerator HitFlashCoroutine()
        {
            // Chớp đỏ rực và giật to nhẹ (1.15x)
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1f, 0.25f, 0.25f, 1f);
            }
            transform.localScale = Vector3.one * 1.15f;

            yield return new WaitForSeconds(0.08f);

            // Trả về màu cũ và kích thước bình thường
            if (spriteRenderer != null && !isDead)
            {
                spriteRenderer.color = Color.white;
            }
            transform.localScale = Vector3.one;
        }

        public void ApplySlow(float slowPercent, float duration)
        {
            if (isDead) return;
            StopCoroutine("SlowCoroutine");
            StartCoroutine(SlowCoroutine(slowPercent, duration));
        }

        private IEnumerator SlowCoroutine(float slowPercent, float duration)
        {
            currentSpeed = enemyData.moveSpeed * (1f - slowPercent);
            if (spriteRenderer != null) spriteRenderer.color = new Color(0.4f, 1f, 0.4f, 1f);

            yield return new WaitForSeconds(duration);

            currentSpeed = enemyData.moveSpeed;
            if (spriteRenderer != null && !isDead) spriteRenderer.color = Color.white;
        }

        private void Die()
        {
            if (isDead) return;
            isDead = true;
            isMoving = false;

            // Sinh hiệu ứng nổ khi chết nếu có gán prefab
            if (deathEffectPrefab != null)
            {
                Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            }

            OnAnyEnemyDied?.Invoke(this);
            OnAnyEnemyRemoved?.Invoke();
            Destroy(gameObject);
        }

        private void ReachGoal()
        {
            if (isDead) return;
            isDead = true;
            isMoving = false;

            OnAnyEnemyReachedGoal?.Invoke(enemyData.damageToBase);
            OnAnyEnemyRemoved?.Invoke();
            Destroy(gameObject);
        }
    }
}