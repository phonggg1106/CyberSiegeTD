using UnityEngine;
using AnutoTD.Enemies;

namespace AnutoTD.Core
{
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance { get; private set; }

        [SerializeField] private int startingCredits = 300;
        private int currentCredits;

        public int CurrentCredits => currentCredits;
        public static event System.Action<int> OnCreditsChanged;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void OnEnable()
        {
            EnemyController.OnAnyEnemyDied += HandleEnemyDied;
        }

        private void OnDisable()
        {
            EnemyController.OnAnyEnemyDied -= HandleEnemyDied;
        }

        public void SetStartingCredits(int amount)
        {
            startingCredits = amount;
            currentCredits = amount;
            OnCreditsChanged?.Invoke(currentCredits);
        }

        private void Start()
        {
            currentCredits = startingCredits;
            OnCreditsChanged?.Invoke(currentCredits);
        }

        private void HandleEnemyDied(EnemyController enemy)
        {
            if (enemy != null && enemy.Data != null)
            {
                AddCredits(enemy.Data.rewardCredits);
            }
        }

        public bool CanAfford(int cost) => currentCredits >= cost;

        public bool SpendCredits(int cost)
        {
            if (!CanAfford(cost)) return false;
            currentCredits -= cost;
            OnCreditsChanged?.Invoke(currentCredits);
            Debug.Log($"[Tiền] Trừ {cost}$, còn lại: {currentCredits}$");
            return true;
        }

        public void AddCredits(int amount)
        {
            currentCredits += amount;
            OnCreditsChanged?.Invoke(currentCredits);
            Debug.Log($"[Tiền] +{amount}$, tổng: {currentCredits}$");
        }
    }
}