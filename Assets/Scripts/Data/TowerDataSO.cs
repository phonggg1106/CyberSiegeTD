using UnityEngine;

namespace AnutoTD.Data
{
    [CreateAssetMenu(fileName = "Tower_Canon_Data", menuName = "Anuto TD/Tower Data")]
    public class TowerDataSO : ScriptableObject
    {
        public TowerType towerType = TowerType.Canon;
        public string towerName = "Canon";

        [Header("Hình ảnh nòng súng")]
        public Sprite gunSprite;        // Chỉ cần ảnh nòng súng (canon)

        [Header("Chỉ số chiến đấu")]
        public int cost = 100;
        public float attackRange = 3.5f;
        public float fireRate = 1.2f;
        public float damage = 35f;
        public float projectileSpeed = 12f;

        [Header("Prefab Đạn")]
        public GameObject projectilePrefab;
    }
}