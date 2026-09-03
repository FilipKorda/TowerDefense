using TowerDefense.Combat;
using UnityEngine;

namespace TowerDefense.EnemySystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyRuntime))]
    public class BurningEffect : MonoBehaviour
    {
        private EnemyRuntime enemyRuntime;
        private ParticleSystem burnParticleEffect;
        private DamageType damageType;
        private float damagePerSecond;
        private float endTime;

        private void Awake()
        {
            enemyRuntime = GetComponent<EnemyRuntime>();
            burnParticleEffect = GetBurnParticleEffect();
        }

        private void Update()
        {
            if (enemyRuntime == null || !enemyRuntime.IsAlive)
            {
                StopBurnParticles(true);
                Destroy(this);
                return;
            }

            if (Time.time >= endTime)
            {
                StopBurnParticles(false);
                Destroy(this);
                return;
            }

            enemyRuntime.TakeDamage(damagePerSecond * Time.deltaTime, damageType);
        }

        public void Apply(float dps, DamageType burnDamageType, float duration)
        {
            damagePerSecond = Mathf.Max(damagePerSecond, dps);
            damageType = burnDamageType;
            endTime = Mathf.Max(endTime, Time.time + duration);
            PlayBurnParticles();
        }

        private void OnDisable()
        {
            StopBurnParticles(false);
        }

        private ParticleSystem GetBurnParticleEffect()
        {
            if (enemyRuntime != null && enemyRuntime.BurningParticleEffect != null)
            {
                return enemyRuntime.BurningParticleEffect;
            }

            return GetComponentInChildren<ParticleSystem>(true);
        }

        private void PlayBurnParticles()
        {
            if (burnParticleEffect == null)
            {
                burnParticleEffect = GetBurnParticleEffect();
            }

            if (burnParticleEffect != null && !burnParticleEffect.isPlaying)
            {
                burnParticleEffect.Play(true);
            }
        }

        private void StopBurnParticles(bool clear)
        {
            if (burnParticleEffect == null || !burnParticleEffect.isPlaying)
            {
                return;
            }

            ParticleSystemStopBehavior stopBehavior = clear
                ? ParticleSystemStopBehavior.StopEmittingAndClear
                : ParticleSystemStopBehavior.StopEmitting;

            burnParticleEffect.Stop(true, stopBehavior);
        }
    }
}
