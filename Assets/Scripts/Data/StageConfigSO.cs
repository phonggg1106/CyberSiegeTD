using System.Collections.Generic;
using UnityEngine;

namespace AnutoTD.Data
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public EnemyType enemyType;
        public float delayBeforeSpawn; // Thời gian chờ trước khi ra con này
    }

    [System.Serializable]
    public class WaveData
    {
        public int waveNumber;
        public int bonusCredits; // Thưởng khi qua wave
        public List<EnemySpawnInfo> enemies = new List<EnemySpawnInfo>();
    }

    [System.Serializable]
    public class EnemyPreviewInfo
    {
        public EnemyType enemyType;
        [TextArea(1, 2)]
        public string traitDescription; // Ví dụ: "Chạy rất nhanh, máu giấy"
    }

    [CreateAssetMenu(fileName = "Stage_01", menuName = "Anuto TD/Stage Config")]
    public class StageConfigSO : ScriptableObject
    {
        [Header("Thông tin cơ bản")]
        public int stageIndex;              // 1 đến 50
        [Range(0, 4)]
        public int mapIndex;                // 0: Outpost, 1: Industrial, 2: Wasteland, 3: Energy, 4: Core
        public int startingCredits = 300;   // Tiền cho trước
        public int baseHealth = 20;         // Máu nhà

        [Header("Giới hạn súng được dùng trong màn này")]
        public List<TowerType> allowedTowers = new List<TowerType>();

        [Header("Tình báo quái (Hiện ở bảng Briefing)")]
        public List<EnemyPreviewInfo> enemyPreviews = new List<EnemyPreviewInfo>();

        [Header("Các đợt Wave của màn")]
        public List<WaveData> waves = new List<WaveData>();
    }
}