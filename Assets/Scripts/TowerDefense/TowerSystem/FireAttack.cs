using TowerDefense.Combat;
using TowerDefense.EnemySystem;
using TowerDefense.TowerSystem;
using UnityEngine;

public class FireAttack : TowerAttackBehaviour
{
    [SerializeField] private ParticleSystem flameEffect;
    [SerializeField] private Transform flameOrigin;
    [SerializeField, Min(0f)] private float burnDurationAfterAttack = 2f;

    private EnemyRuntime lastTarget;
    private float lastDamagePerSecond;
    private DamageType lastDamageType;

    public override void Attack(TowerRuntime tower, EnemyRuntime target)
    {
    }

    public override void Tick(TowerRuntime tower, EnemyRuntime target, float deltaTime)
    {
        if (tower == null || tower.Definition == null)
        {
            ApplyBurnToLastTarget();
            StopFlame();
            return;
        }

        if (target == null || !target.IsAlive)
        {
            ApplyBurnToLastTarget();
            StopFlame();
            return;
        }

        if (lastTarget != null && lastTarget != target)
        {
            ApplyBurnToLastTarget();
        }

        AimFlameAt(target.transform.position);
        PlayFlame();
        target.TakeDamage(tower.Damage * deltaTime, tower.Definition.DamageType);

        lastTarget = target;
        lastDamagePerSecond = tower.Damage;
        lastDamageType = tower.Definition.DamageType;
    }

    private void OnDisable()
    {
        ApplyBurnToLastTarget();
        StopFlame();
    }

    private void ApplyBurnToLastTarget()
    {
        if (lastTarget == null || !lastTarget.IsAlive || burnDurationAfterAttack <= 0f)
        {
            lastTarget = null;
            return;
        }

        BurningEffect burningEffect = lastTarget.GetComponent<BurningEffect>();
        if (burningEffect == null)
        {
            burningEffect = lastTarget.gameObject.AddComponent<BurningEffect>();
        }

        burningEffect.Apply(lastDamagePerSecond, lastDamageType, burnDurationAfterAttack);
        lastTarget = null;
    }

    private void PlayFlame()
    {
        if (flameEffect != null && !flameEffect.isPlaying)
        {
            flameEffect.Play(true);
        }
    }

    private void StopFlame()
    {
        if (flameEffect != null && flameEffect.isPlaying)
        {
            flameEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private void AimFlameAt(Vector3 targetPosition)
    {
        Transform origin = flameOrigin != null ? flameOrigin : transform;
        Vector3 direction = targetPosition - origin.position;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        origin.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
    }
}
