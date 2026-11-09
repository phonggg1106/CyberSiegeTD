using UnityEngine;
using AnutoTD.Data;
using AnutoTD.Enemies;

namespace AnutoTD
{
    public class TestSpawner : MonoBehaviour
    {
        public MapDataSO mapData;
        public GameObject enemyPrefab;
        public EnemyDataSO soldierData;

        [Header("Tự động thả quái mỗi X giây")]
        public float spawnInterval = 2.5f;
        private float timer = 0f;

        private void Start()
        {
            // Thả ngay 1 con khi bắt đầu
            SpawnEnemy();
        }

        private void Update()
        {
            // Tự động đếm giờ để thả thêm quái
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                timer = 0f;
                SpawnEnemy();
            }
        }

        public void SpawnEnemy()
        {
            if (mapData == null || enemyPrefab == null || soldierData == null)
            {
                return;
            }

            if (mapData.waypointPath == null || mapData.waypointPath.Count == 0)
            {
                Debug.LogError("MapData chưa có tọa độ đường đi!");
                return;
            }

            GameObject enemyObj = Instantiate(enemyPrefab);
            EnemyController controller = enemyObj.GetComponent<EnemyController>();
            if (controller != null)
            {
                controller.Initialize(soldierData, mapData.waypointPath);
            }
        }
    }
}