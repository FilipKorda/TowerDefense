using TowerDefense.EnemySystem;
using UnityEngine;

public class SlowEffect : MonoBehaviour
{
    private EnemyRuntime enemyRuntime;
    private float remainingDuration;
    private float appliedMultiplier = 1f;

    public float CurrentMoveSpeed { get; private set; }

    private void Awake()
    {
        enemyRuntime = GetComponent<EnemyRuntime>();
        RefreshCurrentSpeed();
    }

    public void Apply(float multiplier, float duration)
    {
        bool isStrongerSlow = multiplier <= appliedMultiplier;

        if (isStrongerSlow)
        {
            appliedMultiplier = multiplier;
        }

        remainingDuration = Mathf.Max(remainingDuration, duration);
        RefreshCurrentSpeed();
        PlayFrost();
    }

    private void Update()
    {
        remainingDuration -= Time.deltaTime;

        if (remainingDuration <= 0f)
        {
            Remove();
        }
    }

    private void OnDisable()
    {
        StopFrost();
    }

    private void RefreshCurrentSpeed()
    {
        float baseSpeed = enemyRuntime != null && enemyRuntime.Definition != null
            ? enemyRuntime.Definition.MoveSpeed
            : 0f;

        CurrentMoveSpeed = baseSpeed * appliedMultiplier;
    }

    private void PlayFrost()
    {
        ParticleSystem frostEffect = enemyRuntime != null ? enemyRuntime.FrostParticleEffect : null;

        if (frostEffect != null && !frostEffect.isPlaying)
        {
            frostEffect.Play(true);
        }
    }

    private void StopFrost()
    {
        ParticleSystem frostEffect = enemyRuntime != null ? enemyRuntime.FrostParticleEffect : null;

        if (frostEffect != null && frostEffect.isPlaying)
        {
            frostEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private void Remove()
    {
        Destroy(this);
    }
}