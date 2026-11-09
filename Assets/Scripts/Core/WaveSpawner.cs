using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnutoTD.Data;
using AnutoTD.Enemies;

namespace AnutoTD.Core
{
    public class WaveSpawner : MonoBehaviour
    {
        public static WaveSpawner Instance { get; private set; }

        [Header("Bản đồ hiện tại")]
        [SerializeField] private MapDataSO currentMapData;

        [Header("Danh sách Prefab và Data quái")]
        [SerializeField] private List<EnemyPrefabEntry> enemyPrefabs = new List<EnemyPrefabEntry>();

        [Header("Cấu hình test (Nếu chưa có StageConfig)")]
        [Tooltip("Số quái thả trong 1 wave test")]
        public int testEnemyCountPerWave = 5;
        [Tooltip("Khoảng cách giữa mỗi con")]
        public float testSpawnDelay = 1.2f;

        private List<WaveData> waveList = new List<WaveData>();
        private int currentWaveIndex = 0;
        private int enemiesAliveCount = 0;
        private bool isSpawning = false;

        public int CurrentWaveIndex => currentWaveIndex;
        public int TotalWaves => waveList.Count;

        public static event System.Action<int, int> OnWaveChanged;
        public static event System.Action OnAllWavesCleared;

        [System.Serializable]
        public class EnemyPrefabEntry
        {
            public EnemyType type;
            public GameObject prefab;
            public EnemyDataSO data;
        }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            // NẾU CHƯA CÓ WAVE DATA: Tự động tạo 3 wave mẫu để test ngay!
            if (waveList == null || waveList.Count == 0)
            {
                CreateDefaultTestWaves();
            }
        }

        private void OnEnable()
        {
            EnemyController.OnAnyEnemyRemoved += HandleEnemyRemoved;
        }

        private void OnDisable()
        {
            EnemyController.OnAnyEnemyRemoved -= HandleEnemyRemoved;
        }

        // Tạo 3 wave mẫu gồm quái Soldier để test phím Space
        private void CreateDefaultTestWaves()
        {
            waveList = new List<WaveData>();
            for (int w = 1; w <= 3; w++)
            {
                WaveData sampleWave = new WaveData();
                sampleWave.waveNumber = w;
                sampleWave.bonusCredits = 50 * w;

                int count = testEnemyCountPerWave + (w - 1) * 2;
                for (int i = 0; i < count; i++)
                {
                    EnemySpawnInfo info = new EnemySpawnInfo();
                    info.enemyType = EnemyType.Soldier;
                    info.delayBeforeSpawn = (i == 0) ? 0.2f : testSpawnDelay;
                    sampleWave.enemies.Add(info);
                }
                waveList.Add(sampleWave);
            }
            Debug.Log($"<color=green>[WaveSpawner]</color> Đã tự động tạo sẵn {waveList.Count} wave mẫu để test!");
        }

        public void SetWaves(List<WaveData> waves, MapDataSO mapData)
        {
            waveList = waves;
            currentMapData = mapData;
            currentWaveIndex = 0;
            enemiesAliveCount = 0;
        }

        public void SpawnNextWave()
        {
            if (isSpawning)
            {
                Debug.LogWarning("<color=yellow>[WaveSpawner]</color> Đang trong quá trình thả quái của wave hiện tại, vui lòng chờ!");
                return;
            }

            if (currentWaveIndex >= waveList.Count)
            {
                Debug.Log("<color=yellow>[WaveSpawner]</color> Đã hết toàn bộ các đợt wave của màn chơi!");
                return;
            }

            WaveData wave = waveList[currentWaveIndex];
            currentWaveIndex++;
            OnWaveChanged?.Invoke(currentWaveIndex, waveList.Count);
            Debug.Log($"<color=cyan>[WaveSpawner]</color> === BẮT ĐẦU WAVE {currentWaveIndex}/{waveList.Count} ({wave.enemies.Count} quái) ===");

            StartCoroutine(SpawnWaveCoroutine(wave));
        }

        private IEnumerator SpawnWaveCoroutine(WaveData wave)
        {
            isSpawning = true;

            for (int i = 0; i < wave.enemies.Count; i++)
            {
                EnemySpawnInfo spawnInfo = wave.enemies[i];
                if (spawnInfo.delayBeforeSpawn > 0)
                {
                    yield return new WaitForSeconds(spawnInfo.delayBeforeSpawn);
                }

                SpawnSingleEnemy(spawnInfo.enemyType, i + 1, wave.enemies.Count);
            }

            isSpawning = false;
            Debug.Log($"<color=cyan>[WaveSpawner]</color> Đã thả xong tất cả quái của Wave {currentWaveIndex}!");
        }

        private void SpawnSingleEnemy(EnemyType type, int enemyIndex, int totalInWave)
        {
            EnemyPrefabEntry entry = enemyPrefabs.Find(e => e.type == type);

            if (entry == null || entry.prefab == null || entry.data == null)
            {
                Debug.LogError($"<color=red>[WaveSpawner Lỗi]</color> Mục 'Enemy Prefabs' trên Inspector chưa kéo Prefab hoặc Data cho loại quái: {type}!");
                return;
            }

            if (currentMapData == null || currentMapData.waypointPath == null || currentMapData.waypointPath.Count == 0)
            {
                Debug.LogError("<color=red>[WaveSpawner Lỗi]</color> Ô 'Current Map Data' trên Inspector đang trống hoặc chưa có Waypoints!");
                return;
            }

            GameObject enemyObj = Instantiate(entry.prefab);
            EnemyController controller = enemyObj.GetComponent<EnemyController>();
            if (controller != null)
            {
                controller.Initialize(entry.data, currentMapData.waypointPath);
                Debug.Log($"<color=white>[WaveSpawner]</color> Ra quái {enemyIndex}/{totalInWave}: {entry.data.enemyName}");
            }

            enemiesAliveCount++;
        }

        private void HandleEnemyRemoved()
        {
            enemiesAliveCount--;
            enemiesAliveCount = Mathf.Max(0, enemiesAliveCount);
            Debug.Log($"[Quái trên sân] Còn lại: {enemiesAliveCount} con");

            if (currentWaveIndex >= waveList.Count && enemiesAliveCount <= 0 && !isSpawning)
            {
                Debug.Log("<color=green>[CHIẾN THẮNG!] Đã tiêu diệt sạch toàn bộ các đợt wave!</color>");
                OnAllWavesCleared?.Invoke();
            }
        }
    }
}