using System.Collections.Generic;
using UnityEngine;
using AnutoTD.Data;

namespace AnutoTD.Map
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }

        [Header("Tham chiếu Component")]
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private GameObject plateauPrefab;
        [SerializeField] private Transform plateauContainer;
        [SerializeField] private Transform waypointContainer;

        [Header("Dữ liệu Map đang chạy")]
        [SerializeField] private MapDataSO currentMapData;

        // Danh sách waypoint dạng Transform phục vụ cho Quái di chuyển
        public List<Transform> ActiveWaypoints { get; private set; } = new List<Transform>();
        public List<PlateauSlot> ActivePlateaus { get; private set; } = new List<PlateauSlot>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        // Nạp bản đồ từ ScriptableObject
        public void LoadMap(MapDataSO mapData)
        {
            currentMapData = mapData;
            ClearCurrentMap();

            if (mapData == null) return;

            // 1. Gán ảnh nền 16:9
            if (backgroundRenderer != null && mapData.backgroundSprite != null)
            {
                backgroundRenderer.sprite = mapData.backgroundSprite;
            }

            // 2. Tự động sinh các bệ đặt tháp
            if (mapData.plateauPositions != null && plateauPrefab != null)
            {
                foreach (Vector2 pos in mapData.plateauPositions)
                {
                    GameObject pObj = Instantiate(plateauPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity, plateauContainer);
                    PlateauSlot slot = pObj.GetComponent<PlateauSlot>();
                    if (slot != null) ActivePlateaus.Add(slot);
                }
            }

            // 3. Tự động tạo các mốc Waypoint cho đường đi
            if (mapData.waypointPath != null)
            {
                for (int i = 0; i < mapData.waypointPath.Count; i++)
                {
                    Vector2 wpPos = mapData.waypointPath[i];
                    GameObject wpObj = new GameObject($"Waypoint_{i}");
                    wpObj.transform.SetParent(waypointContainer);
                    wpObj.transform.position = new Vector3(wpPos.x, wpPos.y, 0);
                    ActiveWaypoints.Add(wpObj.transform);
                }
            }
        }

        private void ClearCurrentMap()
        {
            ActivePlateaus.Clear();
            ActiveWaypoints.Clear();

            if (plateauContainer != null)
            {
                foreach (Transform child in plateauContainer) Destroy(child.gameObject);
            }
            if (waypointContainer != null)
            {
                foreach (Transform child in waypointContainer) Destroy(child.gameObject);
            }
        }

        // Vẽ đường nối các waypoint trong Editor để dễ quan sát
        private void OnDrawGizmos()
        {
            if (currentMapData != null && currentMapData.waypointPath != null && currentMapData.waypointPath.Count > 1)
            {
                Gizmos.color = Color.yellow;
                for (int i = 0; i < currentMapData.waypointPath.Count - 1; i++)
                {
                    Vector3 p1 = currentMapData.waypointPath[i];
                    Vector3 p2 = currentMapData.waypointPath[i + 1];
                    Gizmos.DrawLine(p1, p2);
                    Gizmos.DrawSphere(p1, 0.15f);
                }
                Gizmos.DrawSphere(currentMapData.waypointPath[currentMapData.waypointPath.Count - 1], 0.2f);
            }
        }
    }
}