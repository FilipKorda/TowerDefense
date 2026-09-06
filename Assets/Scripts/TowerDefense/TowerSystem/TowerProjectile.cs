using TowerDefense.Combat;
using TowerDefense.EnemySystem;
using UnityEngine;

namespace TowerDefense.TowerSystem
{
    public class TowerProjectile : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float hitDistance = 0.15f;
        [SerializeField, Min(0.1f)] private float lifeTime = 5f;

        private EnemyRuntime target;
        private DamageType damageType;
        private float damage;
        private float speed;
        private float destroyTime;

        public void Initialize(EnemyRuntime enemyTarget, float projectileDamage, DamageType projectileDamageType, float projectileSpeed)
        {
            target = enemyTarget;
            damage = projectileDamage;
            damageType = projectileDamageType;
            speed = projectileSpeed;
            destroyTime = Time.time + lifeTime;
        }

        private void Update()
        {
            if (Time.time >= destroyTime || target == null || !target.IsAlive)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 targetPosition = target.transform.position + Vector3.up * 0.5f;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            Vector3 direction = targetPosition - transform.position;
            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            }

            if (Vector3.Distance(transform.position, targetPosition) <= hitDistance)
            {
                target.TakeDamage(damage, damageType);
                Destroy(gameObject);
            }
        }
    }
}
