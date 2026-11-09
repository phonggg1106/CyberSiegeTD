using System.Collections.Generic;
using UnityEngine;
using AnutoTD.Data;

namespace AnutoTD.Map
{
    // Script hỗ trợ kéo thả vị trí trên Scene rồi lưu vào MapDataSO
    public class MapVisualHelper : MonoBehaviour
    {
        public MapDataSO targetMapData;
        public Transform plateauParent;
        public Transform waypointParent;

        [ContextMenu("1. Xuất tọa độ từ Scene vào MapDataSO")]
        public void SaveToMapData()
        {
            if (targetMapData == null)
            {
                Debug.LogError("Chưa gắn MapDataSO!");
                return;
            }

            // Lưu bệ
            targetMapData.plateauPositions = new List<Vector2>();
            if (plateauParent != null)
            {
                foreach (Transform t in plateauParent)
                {
                    targetMapData.plateauPositions.Add(new Vector2(t.position.x, t.position.y));
                }
            }

            // Lưu waypoints
            targetMapData.waypointPath = new List<Vector2>();
            if (waypointParent != null)
            {
                foreach (Transform t in waypointParent)
                {
                    targetMapData.waypointPath.Add(new Vector2(t.position.x, t.position.y));
                }
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(targetMapData);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
            Debug.Log($"Đã lưu thành công dữ liệu vào {targetMapData.name}!");
        }
    }
}