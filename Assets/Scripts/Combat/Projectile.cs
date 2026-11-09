using UnityEngine;
using AnutoTD.Enemies;

namespace AnutoTD.Combat
{
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [Header("Góc bù trừ nòng đạn")]
        public float rotationOffset = -90f;

        [Header("Hiệu ứng va chạm (Tùy chọn)")]
        [SerializeField] private GameObject hitEffectPrefab;

        [Header("Cơ chế Nổ Lan Diện Rộng (Mortar)")]
        public bool isAOE = false;
        public float aoeRadius = 1.5f;

        [Header("Cơ chế Làm Chậm (Glue Gun)")]
        public bool isGlue = false;
        [Range(0.1f, 0.9f)] public float slowPercent = 0.5f;
        public float slowDuration = 2.5f;

        private Transform targetEnemy;
        private float damage = 25f;
        private float speed = 12f;
        private Vector3 moveDirection = Vector3.right;
        private bool hasHit = false;

        public void Initialize(Transform target, float dmg, float spd)
        {
            targetEnemy = target;
            damage = dmg;
            speed = (spd > 0) ? spd : 12f;

            if (target != null)
            {
                moveDirection = (target.position - transform.position).normalized;
            }

            Destroy(gameObject, 4f);
        }

        private void Update()
        {
            if (hasHit) return;

            if (targetEnemy != null)
            {
                moveDirection = (targetEnemy.position - transform.position).normalized;

                if (Vector2.Distance(transform.position, targetEnemy.position) < 0.25f)
                {
                    HitTarget(targetEnemy.GetComponent<EnemyController>());
                    return;
                }
            }

            transform.position += moveDirection * speed * Time.deltaTime;

            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (hasHit) return;

            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                HitTarget(enemy);
            }
        }

        private void HitTarget(EnemyController directTarget)
        {
            if (hasHit) return;
            hasHit = true;

            // SINH HIỆU ỨNG TIA LỬA TẠI ĐIỂM VA CHẠM
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }

            if (isAOE)
            {
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, aoeRadius);
                foreach (var col in hitColliders)
                {
                    EnemyController enemy = col.GetComponent<EnemyController>();
                    if (enemy != null) enemy.TakeDamage(damage);
                }
            }
            else
            {
                if (directTarget != null)
                {
                    directTarget.TakeDamage(damage);
                    if (isGlue)
                    {
                        directTarget.ApplySlow(slowPercent, slowDuration);
                    }
                }
            }

            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            if (isAOE)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, aoeRadius);
            }
        }
    }
}