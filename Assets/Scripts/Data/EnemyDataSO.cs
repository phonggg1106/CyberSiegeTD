using UnityEngine;

namespace AnutoTD.Data
{
    [CreateAssetMenu(fileName = "Enemy_Soldier", menuName = "Anuto TD/Enemy Data")]
    public class EnemyDataSO : ScriptableObject
    {
        public EnemyType enemyType;
        public string enemyName;
        public Sprite sprite;

        [Header("Chỉ số sinh tồn")]
        public float maxHealth = 100f;
        public float moveSpeed = 2.5f;
        public int rewardCredits = 15;      // Tiền thưởng khi diệt
        public int damageToBase = 1;         // Trừ máu nhà nếu lọt vào đích

        [Header("Đặc điểm kháng (Tùy chọn)")]
        public bool isFlying = false;        // Quái bay (không dính mìn)
        public float physicalArmor = 0f;     // Giảm sát thương súng đạn
    }
}