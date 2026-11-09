using UnityEngine;
using AnutoTD.Enemies;

namespace AnutoTD.Core
{
    public class BaseHealth : MonoBehaviour
    {
        public static BaseHealth Instance { get; private set; }

        [SerializeField] private int maxHealth = 20;
        private int currentHealth;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

        public static event System.Action<int, int> OnHealthChanged; // (current, max)
        public static event System.Action OnBaseDead;                // Máu nhà = 0

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void OnEnable()
        {
            EnemyController.OnAnyEnemyReachedGoal += HandleEnemyReachedGoal;
        }

        private void OnDisable()
        {
            EnemyController.OnAnyEnemyReachedGoal -= HandleEnemyReachedGoal;
        }

        private void Start()
        {
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void SetMaxHealth(int hp)
        {
            maxHealth = hp;
            currentHealth = hp;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        private void HandleEnemyReachedGoal(int damage)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            Debug.Log($"[Nhà chính] Bị trừ {damage} máu! Còn lại: {currentHealth}/{maxHealth}");

            if (currentHealth <= 0)
            {
                Debug.Log("<color=red>[GAME OVER] Nhà chính đã bị phá hủy!</color>");
                OnBaseDead?.Invoke();
            }
        }
    }
}