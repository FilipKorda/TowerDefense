using TowerDefense.Combat;
using TowerDefense.EnemySystem;
using UnityEngine;

public class IceProjectile : MonoBehaviour
{
    [SerializeField] private float arcHeight = 3f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float flightDuration;
    private float elapsedTime;

    private float damage;
    private DamageType damageType;
    private float explosionRadius;
    private float slowMultiplier;
    private float slowDuration;
    private LayerMask enemyMask;

    private readonly Collider[] explosionBuffer = new Collider[32];

    public void Launch(
        Vector3 origin,
        Vector3 target,
        float duration,
        float damageAmount,
        DamageType type,
        float radius,
        float slowAmount,
        float slowTime,
        LayerMask mask)
    {
        startPosition = origin;
        targetPosition = target;
        flightDuration = Mathf.Max(0.01f, duration);
        elapsedTime = 0f;

        damage = damageAmount;
        damageType = type;
        explosionRadius = radius;
        slowMultiplier = slowAmount;
        slowDuration = slowTime;
        enemyMask = mask;

        transform.position = startPosition;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / flightDuration);

        Vector3 flatPosition = Vector3.Lerp(startPosition, targetPosition, t);
        float height = arcHeight * 4f * (t - t * t); // parabola: 0 na starcie i mecie, szczyt w połowie lotu
        transform.position = flatPosition + Vector3.up * height;

        if (t >= 1f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            explosionRadius,
            explosionBuffer,
            enemyMask,
            QueryTriggerInteraction.Collide);

        for (int i = 0; i < hitCount; i++)
        {
            Collider hit = explosionBuffer[i];
            if (hit == null)
            {
                continue;
            }

            EnemyRuntime enemy = hit.GetComponentInParent<EnemyRuntime>();
            if (enemy == null || !enemy.IsAlive)
            {
                continue;
            }

            enemy.TakeDamage(damage, damageType);
            ApplySlow(enemy);
        }

        Destroy(gameObject);
    }

    private void ApplySlow(EnemyRuntime enemy)
    {
        if (slowDuration <= 0f)
        {
            return;
        }

        SlowEffect slowEffect = enemy.GetComponent<SlowEffect>();
        if (slowEffect == null)
        {
            slowEffect = enemy.gameObject.AddComponent<SlowEffect>();
        }

        slowEffect.Apply(slowMultiplier, slowDuration);
    }
}