using System.Collections.Generic;
using UnityEngine;

namespace AnutoTD.Data
{
    [CreateAssetMenu(fileName = "Map_Outpost", menuName = "Anuto TD/Map Data")]
    public class MapDataSO : ScriptableObject
    {
        public string mapId;                     // campaign_outpost
        public Sprite backgroundSprite;          // Ảnh nền 16:9
        public List<Vector2> plateauPositions;  // Tọa độ các bệ đặt súng
        public List<Vector2> waypointPath;      // Tọa độ đường quái chạy
    }
}