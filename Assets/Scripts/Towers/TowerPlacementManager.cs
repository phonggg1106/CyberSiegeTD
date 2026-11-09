using UnityEngine;
using AnutoTD.Data;
using AnutoTD.Map;
using AnutoTD.Core;

namespace AnutoTD.Towers
{
    public class TowerPlacementManager : MonoBehaviour
    {
        public static TowerPlacementManager Instance { get; private set; }

        private GameObject ghostTowerObj;
        private SpriteRenderer ghostRenderer;
        private LineRenderer rangeCircleRenderer;

        private TowerDataSO currentTowerData;
        private GameObject currentTowerPrefab;
        private PlateauSlot currentHoveredPlateau;
        private bool isDragging = false;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            CreateGhostTower();
        }

        // Tự động tạo bóng ảo preview và vòng tròn tầm bắn bằng code
        private void CreateGhostTower()
        {
            ghostTowerObj = new GameObject("GhostTower_Preview");
            ghostRenderer = ghostTowerObj.AddComponent<SpriteRenderer>();
            ghostRenderer.sortingOrder = 20; // Luôn hiển thị nổi trên cùng

            // Tạo vòng tròn tầm bắn bằng LineRenderer
            rangeCircleRenderer = ghostTowerObj.AddComponent<LineRenderer>();
            rangeCircleRenderer.useWorldSpace = false;
            rangeCircleRenderer.loop = true;
            rangeCircleRenderer.positionCount = 60;
            rangeCircleRenderer.startWidth = 0.06f;
            rangeCircleRenderer.endWidth = 0.06f;
            rangeCircleRenderer.material = new Material(Shader.Find("Sprites/Default"));

            ghostTowerObj.SetActive(false);
        }

        // 1. BẮT ĐẦU KÉO TỪ THẺ SÚNG
        public void StartDragging(TowerDataSO data, GameObject towerPrefab, Vector2 screenPos)
        {
            if (data == null || towerPrefab == null) return;

            // Kiểm tra đủ tiền không
            if (EconomyManager.Instance != null && !EconomyManager.Instance.CanAfford(data.cost))
            {
                Debug.Log("<color=red>[Đặt tháp]</color> Không đủ tiền để mua tháp này!");
                return;
            }

            currentTowerData = data;
            currentTowerPrefab = towerPrefab;
            isDragging = true;

            // Đổi hình bóng ảo sang súng đang chọn
            ghostRenderer.sprite = data.gunSprite;
            ghostRenderer.color = new Color(1f, 1f, 1f, 0.65f); // Bán trong suốt

            // Vẽ vòng tròn theo bán kính tầm bắn của súng đó
            DrawRangeCircle(data.attackRange);

            ghostTowerObj.SetActive(true);
            UpdateGhostPosition(screenPos);
        }

        // 2. KHI ĐANG RÊ CHUỘT
        public void OnDragging(Vector2 screenPos)
        {
            if (!isDragging) return;
            UpdateGhostPosition(screenPos);
        }

        // 3. THẢ CHUỘT (CẮM THÁP)
        public void EndDragging()
        {
            if (!isDragging) return;
            isDragging = false;
            ghostTowerObj.SetActive(false);

            // Nếu đang nằm trên một bệ trống hợp lệ
            if (currentHoveredPlateau != null && !currentHoveredPlateau.isOccupied && currentTowerData != null)
            {
                // Kiểm tra và trừ tiền
                if (EconomyManager.Instance != null && EconomyManager.Instance.SpendCredits(currentTowerData.cost))
                {
                    // Cắm tháp thật lên bệ
                    GameObject realTower = Instantiate(currentTowerPrefab, currentHoveredPlateau.transform.position, Quaternion.identity);
                    currentHoveredPlateau.PlaceTower(realTower);
                    Debug.Log($"<color=green>[Thành công]</color> Đã đặt {currentTowerData.towerName} lên bệ!");
                }
            }

            currentHoveredPlateau = null;
            currentTowerData = null;
            currentTowerPrefab = null;
        }

        // Cập nhật vị trí bóng ảo và kiểm tra bệ
        private void UpdateGhostPosition(Vector2 screenPos)
        {
            if (Camera.main == null) return;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            worldPos.z = 0f;

            // Quét xem chuột có đang nằm trên PlateauSlot nào không
            Collider2D hit = Physics2D.OverlapPoint(worldPos);
            currentHoveredPlateau = null;

            if (hit != null)
            {
                PlateauSlot slot = hit.GetComponent<PlateauSlot>();
                if (slot != null && !slot.isOccupied)
                {
                    currentHoveredPlateau = slot;
                }
            }

            // Nếu nằm trên bệ trống: Hít vào tâm bệ và đổi vòng sang màu xanh lá
            if (currentHoveredPlateau != null)
            {
                ghostTowerObj.transform.position = currentHoveredPlateau.transform.position;
                SetRangeColor(new Color(0.2f, 1f, 0.3f, 0.85f)); // Xanh lá hợp lệ
            }
            else
            {
                // Không nằm trên bệ: Đi theo chuột và đổi vòng sang màu đỏ
                ghostTowerObj.transform.position = worldPos;
                SetRangeColor(new Color(1f, 0.25f, 0.25f, 0.7f)); // Đỏ chưa hợp lệ
            }
        }

        private void DrawRangeCircle(float radius)
        {
            float deltaTheta = (2f * Mathf.PI) / 60;
            float theta = 0f;

            for (int i = 0; i < 60; i++)
            {
                float x = radius * Mathf.Cos(theta);
                float y = radius * Mathf.Sin(theta);
                rangeCircleRenderer.SetPosition(i, new Vector3(x, y, 0));
                theta += deltaTheta;
            }
        }

        private void SetRangeColor(Color c)
        {
            rangeCircleRenderer.startColor = c;
            rangeCircleRenderer.endColor = c;
        }
    }
}