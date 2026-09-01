using System;
using TowerDefense.Combat;
using TowerDefense.EnemySystem;
using UnityEngine;

namespace TowerDefense.BuildSystem
{
    [Flags]
    public enum TargetableEnemyTypes
    {
        None = 0,
        Ground = 1 << 0,  // 1
        Flying = 1 << 1,  // 2
        All = Ground | Flying  // 3
    }

    [CreateAssetMenu(fileName = "Tower Definition", menuName = "Tower Defense/Tower Definition")]
    public class TowerDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string displayName = "Tower";
        [SerializeField] private string towerDescription = "Tower";
        [SerializeField] private int cost = 50;
        [SerializeField] private GameObject prefab;

        [Header("Stats")]
        [SerializeField, Min(0.01f)] private float attackSpeed = 1f;
        [SerializeField, Min(0f)] private float damage = 10f;
        [SerializeField, Min(0f)] private float attackRange = 5f;
        [SerializeField] private DamageType damageType = DamageType.Physical;

        [Header("Targeting")]
        [SerializeField] private TargetableEnemyTypes targetableTypes;

        [Header("Upgrade")]
        [SerializeField, Min(0)] private int upgradeCost = 30;
        [SerializeField, Min(0)] private int upgradeCostIncreasePerLevel = 10;
        [SerializeField, Min(1)] private int maxUpgradeLevel = 2;
        [SerializeField, Range(1f, 3f)] private float upgradeDamageMultiplier = 1.25f;
        [SerializeField, Range(1f, 3f)] private float upgradeAttackSpeedMultiplier = 1.1f;
        [SerializeField, Range(1f, 3f)] private float upgradeAttackRangeMultiplier = 1.1f;

        [Header("Sell")]
        [SerializeField, Range(0f, 1f)] private float sellRefundPercent = 0.5f;

        [Header("Placement")]
        [SerializeField] private Vector3 buildOffset;

        [Header("UI")]
        [SerializeField] private Color buttonColor = new Color(0.2f, 0.55f, 0.95f);

        public string DisplayName => displayName;
        public int Cost => cost;
        public GameObject Prefab => prefab;
        public float AttackSpeed => attackSpeed;
        public float Damage => damage;
        public float AttackRange => attackRange;
        public string TowerDescription => towerDescription;
        public DamageType DamageType => damageType;
        public TargetableEnemyTypes TargetableTypes => targetableTypes & TargetableEnemyTypes.All;
        public int UpgradeCost => upgradeCost;
        public int UpgradeCostIncreasePerLevel => upgradeCostIncreasePerLevel;
        public int MaxUpgradeLevel => maxUpgradeLevel;
        public float UpgradeDamageMultiplier => upgradeDamageMultiplier;
        public float UpgradeAttackSpeedMultiplier => upgradeAttackSpeedMultiplier;
        public float UpgradeAttackRangeMultiplier => upgradeAttackRangeMultiplier;
        public Vector3 BuildOffset => buildOffset;
        public Color ButtonColor => buttonColor;
        public float SellRefundPercent => sellRefundPercent;

        public bool CanTarget(EnemyMovementType movementType)
        {
            TargetableEnemyTypes requiredFlag = movementType == EnemyMovementType.Flying
                ? TargetableEnemyTypes.Flying
                : TargetableEnemyTypes.Ground;

            return (TargetableTypes & requiredFlag) != 0;
        }
    }
}