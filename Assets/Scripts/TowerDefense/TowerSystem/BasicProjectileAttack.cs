using TowerDefense.EnemySystem;
using UnityEngine;

namespace TowerDefense.TowerSystem
{
    public class BasicProjectileAttack : TowerAttackBehaviour
    {
        [SerializeField] private TowerProjectile projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField, Min(0.1f)] private float projectileSpeed = 10f;

        public override void Attack(TowerRuntime tower, EnemyRuntime target)
        {
            if (tower == null || target == null || tower.Definition == null)
            {
                return;
            }

            Vector3 spawnPosition = firePoint != null ? firePoint.position : tower.transform.position + Vector3.up;

            if (projectilePrefab == null)
            {
                target.TakeDamage(tower.Damage, tower.Definition.DamageType);
                return;
            }

            TowerProjectile projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            projectile.Initialize(target, tower.Damage, tower.Definition.DamageType, projectileSpeed);
        }
    }
}
