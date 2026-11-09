using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AnutoTD.Towers;

namespace AnutoTD.UI
{
    public class TowerInfoPanel : MonoBehaviour
    {
        [Header("Tham chiếu UI")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI statsText;
        [SerializeField] private TextMeshProUGUI strategyButtonText;
        [SerializeField] private TextMeshProUGUI enhanceButtonText;
        [SerializeField] private TextMeshProUGUI sellButtonText;

        private TowerController selectedTower;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        private void OnEnable()
        {
            TowerController.OnTowerSelected += OpenPanel;
            TowerController.OnTowerDeselected += ClosePanel;
        }

        private void OnDisable()
        {
            TowerController.OnTowerSelected -= OpenPanel;
            TowerController.OnTowerDeselected -= ClosePanel;
        }

        public void OpenPanel(TowerController tower)
        {
            selectedTower = tower;
            if (panelRoot != null) panelRoot.SetActive(true);
            UpdateUI();
        }

        public void ClosePanel()
        {
            if (selectedTower != null) selectedTower.ShowRangeCircle(false);
            selectedTower = null;
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        private void UpdateUI()
        {
            if (selectedTower == null) return;

            if (titleText != null)
                titleText.text = $"{selectedTower.Data.towerName} (Lv.{selectedTower.Level})";

            if (statsText != null)
                statsText.text = $"Sát thương: {selectedTower.Damage:F0}\nTầm bắn: {selectedTower.Range:F1}\nTốc bắn: {selectedTower.FireRate:F1}/s";

            if (strategyButtonText != null)
                strategyButtonText.text = $" Mục tiêu: {selectedTower.Strategy}";

            if (enhanceButtonText != null)
                enhanceButtonText.text = $" Cường hóa ({selectedTower.GetEnhanceCost()}$)";

            if (sellButtonText != null)
                sellButtonText.text = $"Bán (+{(int)(selectedTower.Data.cost * 0.7f)}$)";
        }

        // Sự kiện khi bấm nút Cường hóa
        public void OnClickEnhance()
        {
            if (selectedTower != null && selectedTower.Enhance())
            {
                UpdateUI();
            }
        }

        // Sự kiện khi bấm nút Bán
        public void OnClickSell()
        {
            if (selectedTower != null)
            {
                selectedTower.Sell();
                ClosePanel();
            }
        }

        // Sự kiện khi bấm nút Đổi chế độ ngắm
        public void OnClickCycleStrategy()
        {
            if (selectedTower != null)
            {
                selectedTower.CycleStrategy();
                UpdateUI();
            }
        }
    }
}