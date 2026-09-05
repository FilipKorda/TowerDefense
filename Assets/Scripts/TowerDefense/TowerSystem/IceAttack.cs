using TowerDefense.EnemySystem;
using TowerDefense.TowerSystem;
using UnityEngine;

public class IceAttack : TowerAttackBehaviour
{
    [SerializeField] private IceProjectile projectilePrefab;
    [SerializeField] private Transform mortarOrigin;
    [SerializeField, Min(0.05f)] private float projectileFlightDuration = 0.8f;
    [SerializeField, Min(0f)] private float explosionRadius = 1.5f;
    [SerializeField, Range(0f, 1f)] private float slowMultiplier = 0.5f;
    [SerializeField, Min(0f)] private float slowDuration = 2f;
    [SerializeField] private LayerMask enemyMask = ~0;

    public override void Attack(TowerRuntime tower, EnemyRuntime target)
    {
        if (tower == null || tower.Definition == null || target == null || !target.IsAlive || projectilePrefab == null)
        {
            return;
        }

        Transform origin = mortarOrigin != null ? mortarOrigin : transform;

        IceProjectile projectile = Instantiate(projectilePrefab, origin.position, Quaternion.identity);
        projectile.Launch(
            origin.position,
            target.transform.position,
            projectileFlightDuration,
            tower.Damage,
            tower.Definition.DamageType,
            explosionRadius,
            slowMultiplier,
            slowDuration,
            enemyMask);
    }
}