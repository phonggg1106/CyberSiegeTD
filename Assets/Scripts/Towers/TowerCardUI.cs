using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AnutoTD.Data;
using AnutoTD.Core;
using AnutoTD.Towers;

namespace AnutoTD.UI
{
    [RequireComponent(typeof(Image))]
    public class TowerCardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Cấu hình tháp")]
        [SerializeField] private TowerDataSO towerData;
        [SerializeField] private GameObject towerPrefab;

        private Image cardImage;
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            cardImage = GetComponent<Image>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            EconomyManager.OnCreditsChanged += HandleCreditsChanged;
        }

        private void OnDisable()
        {
            EconomyManager.OnCreditsChanged -= HandleCreditsChanged;
        }

        private void Start()
        {
            if (EconomyManager.Instance != null)
            {
                HandleCreditsChanged(EconomyManager.Instance.CurrentCredits);
            }
        }

        // Tự động làm tối mờ cả thẻ nếu thiếu tiền
        private void HandleCreditsChanged(int currentCredits)
        {
            if (towerData == null) return;

            bool canAfford = currentCredits >= towerData.cost;

            // Nếu có CanvasGroup thì chỉnh độ mờ, không thì chỉnh màu ảnh tối đi
            if (canvasGroup != null)
            {
                canvasGroup.alpha = canAfford ? 1.0f : 0.45f;
            }
            else if (cardImage != null)
            {
                cardImage.color = canAfford ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.6f);
            }
        }

        // 1. BẮT ĐẦU KÉO
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (towerData == null || towerPrefab == null)
            {
                Debug.LogWarning($"Thẻ {gameObject.name} chưa gán TowerData hoặc TowerPrefab!");
                return;
            }

            if (TowerPlacementManager.Instance != null)
            {
                TowerPlacementManager.Instance.StartDragging(towerData, towerPrefab, eventData.position);
            }
        }

        // 2. ĐANG KÉO THEO CHUỘT
        public void OnDrag(PointerEventData eventData)
        {
            if (TowerPlacementManager.Instance != null)
            {
                TowerPlacementManager.Instance.OnDragging(eventData.position);
            }
        }

        // 3. THẢ CHUỘT RA
        public void OnEndDrag(PointerEventData eventData)
        {
            if (TowerPlacementManager.Instance != null)
            {
                TowerPlacementManager.Instance.EndDragging();
            }
        }
    }
}