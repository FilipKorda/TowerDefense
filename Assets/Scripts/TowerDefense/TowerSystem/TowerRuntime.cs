using TowerDefense.BuildSystem;
using TowerDefense.EnemySystem;
using UnityEngine;

namespace TowerDefense.TowerSystem
{
    [DisallowMultipleComponent]
    public class TowerRuntime : MonoBehaviour
    {
        [SerializeField] private TowerDefinition definition;
        [SerializeField] private LayerMask enemyMask = ~0;
        [SerializeField] private Transform turretPivot;
        [SerializeField] private bool rotateToTarget = true;
        [SerializeField, Min(0.02f)] private float targetScanInterval = 0.15f;

        private readonly Collider[] targetBuffer = new Collider[64];
        private TowerAttackBehaviour attackBehaviour;
        private EnemyRuntime currentTarget;
        private float nextScanTime;
        private float nextAttackTime;

        private float currentDamage;
        private float currentAttackSpeed;
        private float currentAttackRange;
        private int upgradeLevel;
        private int totalInvestedMoney;

        public TowerDefinition Definition => definition;
        public EnemyRuntime CurrentTarget => currentTarget;
        public float Damage => currentDamage;
        public float AttackRange => currentAttackRange;
        public float AttackSpeed => currentAttackSpeed;
        public int UpgradeLevel => upgradeLevel;
        public bool CanUpgrade => definition != null && upgradeLevel < definition.MaxUpgradeLevel;
        public int UpgradeCost => definition != null
       ? Mathf.RoundToInt(GetBaseUpgradeCostForCurrentLevel() * tileUpgradeCostMultiplier)
       : 0;
        public int TotalInvestedMoney => totalInvestedMoney;

        private float tileUpgradeCostMultiplier = 1f;

        private TileBonusType appliedTileBonusType = TileBonusType.None;
        private float appliedTileBonusMultiplier = 1f;

        public TileBonusType AppliedTileBonusType => appliedTileBonusType;
        public float AppliedTileBonusMultiplier => appliedTileBonusMultiplier;

        private void Awake()
        {
            attackBehaviour = GetComponent<TowerAttackBehaviour>();

            if (attackBehaviour == null)
            {
                attackBehaviour = gameObject.AddComponent<BasicProjectileAttack>();
            }
        }

        private void Update()
        {
            if (definition == null)
            {
                return;
            }

            if (Time.time >= nextScanTime)
            {
                currentTarget = FindClosestEnemyInRange();
                nextScanTime = Time.time + targetScanInterval;
            }

            if (currentTarget == null || !currentTarget.IsAlive)
            {
                attackBehaviour.Tick(this, null, Time.deltaTime);
                return;
            }

            if (!IsTargetInRange(currentTarget))
            {
                currentTarget = null;
                attackBehaviour.Tick(this, null, Time.deltaTime);
                return;
            }

            RotateTowards(currentTarget.transform.position);
            attackBehaviour.Tick(this, currentTarget, Time.deltaTime);

            if (Time.time >= nextAttackTime)
            {
                attackBehaviour.Attack(this, currentTarget);
                nextAttackTime = Time.time + GetAttackCooldown();
            }
        }

        private int GetBaseUpgradeCostForCurrentLevel()
        {
            int levelsUpgraded = Mathf.Max(0, upgradeLevel - 1);
            return definition.UpgradeCost + definition.UpgradeCostIncreasePerLevel * levelsUpgraded;
        }
        public void Initialize(TowerDefinition towerDefinition)
        {
            definition = towerDefinition;
            ResetStatsFromDefinition();
            totalInvestedMoney = definition != null ? definition.Cost : 0;
        }

        public void ApplyTileBonus(TileBonusType bonusType, float bonusMultiplier)
        {
            appliedTileBonusType = bonusType;
            appliedTileBonusMultiplier = bonusMultiplier;

            switch (bonusType)
            {
                case TileBonusType.DamageBoost:
                    currentDamage *= bonusMultiplier;
                    break;

                case TileBonusType.RangeBoost:
                    currentAttackRange *= bonusMultiplier;
                    break;

                case TileBonusType.AttackSpeedBoost:
                    currentAttackSpeed *= bonusMultiplier;
                    break;

                case TileBonusType.LowUpgradeCost:
                    tileUpgradeCostMultiplier = 1f / Mathf.Max(0.01f, bonusMultiplier);
                    break;
            }
        }

        public bool TryUpgrade()
        {
            if (!CanUpgrade)
            {
                return false;
            }

            currentDamage *= definition.UpgradeDamageMultiplier;
            currentAttackSpeed *= definition.UpgradeAttackSpeedMultiplier;
            currentAttackRange *= definition.UpgradeAttackRangeMultiplier;
            upgradeLevel++;
            totalInvestedMoney += UpgradeCost; 
     
            return true;
        }

        public int GetSellRefund()
        {
            if (definition == null)
            {
                return 0;
            }

            return Mathf.RoundToInt(totalInvestedMoney * definition.SellRefundPercent);
        }

        private void ResetStatsFromDefinition()
        {
            currentDamage = definition != null ? definition.Damage : 0f;
            currentAttackSpeed = definition != null ? definition.AttackSpeed : 1f;
            currentAttackRange = definition != null ? definition.AttackRange : 0f;
            upgradeLevel = 1;
        }

        private EnemyRuntime FindClosestEnemyInRange()
        {
            int hitCount = Physics.OverlapSphereNonAlloc(
                transform.position,
                AttackRange,
                targetBuffer,
                enemyMask,
                QueryTriggerInteraction.Collide);

            EnemyRuntime closestEnemy = null;
            float closestDistanceSqr = float.MaxValue;

            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = targetBuffer[i];
                if (hit == null)
                {
                    continue;
                }

                EnemyRuntime enemy = hit.GetComponentInParent<EnemyRuntime>();
                if (enemy == null || !enemy.IsAlive || enemy.Definition == null)
                {
                    continue;
                }

                if (definition == null || !definition.CanTarget(enemy.Definition.MovementType))
                {
                    continue;
                }

                float distanceSqr = (enemy.transform.position - transform.position).sqrMagnitude;
                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestEnemy = enemy;
                }
            }

            return closestEnemy;
        }

        private bool IsTargetInRange(EnemyRuntime target)
        {
            float range = AttackRange;
            return (target.transform.position - transform.position).sqrMagnitude <= range * range;
        }

        private void RotateTowards(Vector3 targetPosition)
        {
            if (!rotateToTarget)
            {
                return;
            }

            Transform pivot = turretPivot;
            Vector3 direction = targetPosition - pivot.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.001f)
            {
                return;
            }

            pivot.rotation = Quaternion.LookRotation(-direction.normalized, Vector3.up);
        }

        private float GetAttackCooldown()
        {
            return 1f / Mathf.Max(0.01f, AttackSpeed);
        }

        private void OnDrawGizmosSelected()
        {
            float range = definition != null ? definition.AttackRange : 0f;
            if (range <= 0f)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}