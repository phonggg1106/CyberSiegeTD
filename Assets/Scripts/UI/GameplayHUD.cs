using UnityEngine;
using TMPro;
using AnutoTD.Core;

namespace AnutoTD.UI
{
    public class GameplayHUD : MonoBehaviour
    {
        [Header("Tham chiếu Text TextMeshPro")]
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI creditsText;
        [SerializeField] private TextMeshProUGUI waveText;

        private void OnEnable()
        {
            // Đăng ký nhận sự kiện từ các Manager
            BaseHealth.OnHealthChanged += UpdateHealthDisplay;
            EconomyManager.OnCreditsChanged += UpdateCreditsDisplay;
            WaveSpawner.OnWaveChanged += UpdateWaveDisplay;
        }

        private void OnDisable()
        {
            // Hủy đăng ký khi tắt scene để tránh rò rỉ bộ nhớ
            BaseHealth.OnHealthChanged -= UpdateHealthDisplay;
            EconomyManager.OnCreditsChanged -= UpdateCreditsDisplay;
            WaveSpawner.OnWaveChanged -= UpdateWaveDisplay;
        }

        private void Start()
        {
            // Khởi tạo hiển thị ban đầu nếu các Manager đã sẵn sàng
            if (BaseHealth.Instance != null)
            {
                UpdateHealthDisplay(BaseHealth.Instance.CurrentHealth, BaseHealth.Instance.MaxHealth);
            }

            if (EconomyManager.Instance != null)
            {
                UpdateCreditsDisplay(EconomyManager.Instance.CurrentCredits);
            }

            if (WaveSpawner.Instance != null)
            {
                UpdateWaveDisplay(WaveSpawner.Instance.CurrentWaveIndex, WaveSpawner.Instance.TotalWaves);
            }
        }

        private void UpdateHealthDisplay(int currentHealth, int maxHealth)
        {
            if (healthText != null)
            {
                healthText.text = $"❤️ HP: {currentHealth} / {maxHealth}";
            }
        }

        private void UpdateCreditsDisplay(int credits)
        {
            if (creditsText != null)
            {
                creditsText.text = $" Credits: {credits}$";
            }
        }

        private void UpdateWaveDisplay(int currentWave, int totalWaves)
        {
            if (waveText != null)
            {
                waveText.text = $" Wave: {currentWave} / {totalWaves}";
            }
        }
    }
}