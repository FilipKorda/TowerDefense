using TowerDefense.EnemySystem;
using TowerDefense.TowerSystem;
using UnityEngine;

public class LaserAttack : TowerAttackBehaviour
{
    [SerializeField] private LineRenderer laserRenderer;
    [SerializeField] private Transform laserOrigin;

    private EnemyRuntime currentTarget;

    public override void Attack(TowerRuntime tower, EnemyRuntime target)
    {
        // Cała logika ataku laserowego odbywa się w Tick (ciągłe obrażenia w czasie)
    }

    public override void Tick(TowerRuntime tower, EnemyRuntime target, float deltaTime)
    {
        if (tower == null || tower.Definition == null)
        {
            StopLaser();
            return;
        }

        if (target == null || !target.IsAlive)
        {
            StopLaser();
            return;
        }

        currentTarget = target;

        DrawLaser(target.transform.position);
        target.TakeDamage(tower.Damage * deltaTime, tower.Definition.DamageType);
    }

    private void OnDisable()
    {
        StopLaser();
    }

    private void DrawLaser(Vector3 targetPosition)
    {
        if (laserRenderer == null)
        {
            return;
        }

        Transform origin = laserOrigin != null ? laserOrigin : transform;

        if (!laserRenderer.enabled)
        {
            laserRenderer.enabled = true;
        }

        laserRenderer.SetPosition(0, origin.position);
        laserRenderer.SetPosition(1, targetPosition);
    }

    private void StopLaser()
    {
        currentTarget = null;

        if (laserRenderer != null && laserRenderer.enabled)
        {
            laserRenderer.enabled = false;
        }
    }
}