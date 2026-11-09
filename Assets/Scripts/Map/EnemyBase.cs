using UnityEngine;
using AnutoTD.Data;
using AnutoTD.Map;

namespace AnutoTD.Enemies
{
    [RequireComponent(typeof(PathFollower))]
    public class EnemyBase : MonoBehaviour
    {
        [Header("Cấu hình dữ liệu")]
        [SerializeField] private EnemyDataSO enemyData;

        [Header("Thanh máu trên đầu quái")]
        [SerializeField] private Transform healthBarFill; // Sprite thanh máu co giãn theo trục X

        private float currentHealth;
        private PathFollower pathFollower;
        private SpriteRenderer spriteRenderer;

        public EnemyDataSO Data => enemyData;
        public float CurrentHealth => currentHealth;
        public bool IsDead => currentHealth <= 0;

        // Sự kiện báo ra ngoài khi quái chết hoặc vào đích
        public static event System.Action<EnemyBase> OnAnyEnemyDied;
        public static event System.Action<int> OnAnyEnemyReachedGoal; // Truyền damageToBase

        private void Awake()
        {
            pathFollower = GetComponent<PathFollower>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            SetupEnemy(enemyData);
        }

        public void SetupEnemy(EnemyDataSO data)
        {
            enemyData = data;
            if (enemyData == null) return;

            currentHealth = enemyData.maxHealth;
            if (spriteRenderer != null && enemyData.sprite != null)
            {
                spriteRenderer.sprite = enemyData.sprite;
            }

            UpdateHealthBar();

            // Khởi động đường chạy
            if (MapManager.Instance != null && MapManager.Instance.ActiveWaypoints.Count > 0)
            {
                pathFollower.Initialize(MapManager.Instance.ActiveWaypoints, enemyData.moveSpeed);
            }

            pathFollower.OnReachGoal = HandleReachGoal;
        }

        // Nhận sát thương từ tháp
        public void TakeDamage(float damage)
        {
            if (IsDead) return;

            currentHealth -= damage;
            UpdateHealthBar();

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void UpdateHealthBar()
        {
            if (healthBarFill != null && enemyData != null)
            {
                float healthPercent = Mathf.Clamp01(currentHealth / enemyData.maxHealth);
                healthBarFill.localScale = new Vector3(healthPercent, 1f, 1f);
            }
        }

        private void Die()
        {
            OnAnyEnemyDied?.Invoke(this);
            // Sau này dùng Object Pooling, tạm thời hủy GameObject
            Destroy(gameObject);
        }

        private void HandleReachGoal()
        {
            OnAnyEnemyReachedGoal?.Invoke(enemyData.damageToBase);
            Destroy(gameObject);
        }
    }
}