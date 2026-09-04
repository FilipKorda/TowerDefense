using System;
using TowerDefense.Combat;
using TowerDefense.PlayerSystem;
using UnityEngine;

namespace TowerDefense.EnemySystem
{
    [DisallowMultipleComponent]
    public class EnemyRuntime : MonoBehaviour, IDamageable
    {
        [SerializeField] private ParticleSystem burningParticleEffect;
        [SerializeField] private ParticleSystem frostParticleEffect;

        public EnemyDefinition Definition { get; private set; }
        public float CurrentHp { get; private set; }
        public DamageType LastDamageType { get; private set; }
        public ParticleSystem BurningParticleEffect => burningParticleEffect;
        public ParticleSystem FrostParticleEffect => frostParticleEffect;
        public bool IsAlive => CurrentHp > 0f;

        public event Action<float, float> OnHealthChanged;

        public static event Action<EnemyRuntime> AnyEnemyDestroyed;

        private float resolvedMaxHp;
        public float MaxHp => resolvedMaxHp;

        private void Awake()
        {
            if (burningParticleEffect != null && burningParticleEffect.isPlaying)
            {
                burningParticleEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            if (frostParticleEffect != null && frostParticleEffect.isPlaying)
            {
                frostParticleEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        private void OnDestroy()
        {
            AnyEnemyDestroyed?.Invoke(this);
        }

        public void Initialize(EnemyDefinition definition)
        {

            Definition = definition;

            float bonusHp = GameSystem.GameSession.SelectedDifficulty != null
                ? GameSystem.GameSession.SelectedDifficulty.EnemyHpBonus
                : 0f;

            resolvedMaxHp = definition.MaxHp + bonusHp;
            CurrentHp = resolvedMaxHp;

            OnHealthChanged?.Invoke(CurrentHp, resolvedMaxHp);
        }

        public void TakeDamage(float damage)
        {
            TakeDamage(damage, DamageType.Physical);
        }

        public void TakeDamage(float damage, DamageType damageType)
        {
            if (!IsAlive)
            {
                return;
            }

            float resistance = Definition != null ? Definition.GetResistance(damageType) : 0f;
            float finalDamage = damage * (1f - resistance);

            CurrentHp = Mathf.Max(0f, CurrentHp - finalDamage);
            LastDamageType = damageType;

            OnHealthChanged?.Invoke(CurrentHp, resolvedMaxHp);

            if (CurrentHp <= 0f)
            {
                if (Definition != null && PlayerStats.Instance != null)
                {
                    PlayerStats.Instance.AddMoney(Definition.MoneyReward);
                }

                Destroy(gameObject);
            }
        }
    }
}