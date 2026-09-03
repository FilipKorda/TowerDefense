using System;
using System.Collections.Generic;
using TowerDefense.Combat;
using UnityEngine;

namespace TowerDefense.EnemySystem
{
    public enum EnemyMovementType
    {
        Ground,
        Flying
    }

    [Serializable]
    public struct DamageResistance
    {
        public DamageType DamageType;
        [Range(0f, 1f)] public float ResistancePercent; 
    }

    [CreateAssetMenu(fileName = "Enemy Definition", menuName = "Tower Defense/Enemy Definition")]
    public class EnemyDefinition : ScriptableObject
    {
        [SerializeField] private string enemyDescription = "Some description of the current enemy.";
        [SerializeField] private string displayName = "Enemy";
        [SerializeField] private GameObject prefab;
        [SerializeField, Min(1f)] private float maxHp = 20f;
        [SerializeField, Min(0.01f)] private float moveSpeed = 2f;
        [SerializeField, Min(0)] private int moneyReward = 10;
        private int damageToPlayerBase = 1;

        [Header("Movement")]
        [SerializeField] private EnemyMovementType movementType = EnemyMovementType.Ground;
        [SerializeField, Min(0f)] private float heightAboveTheGround = 1f;

        [Header("Resistances")]
        [SerializeField] private List<DamageResistance> resistances = new List<DamageResistance>();

        public string DisplayName => displayName;
        public string EnemyDescription => enemyDescription;
        public GameObject Prefab => prefab;
        public float MaxHp => maxHp;
        public float MoveSpeed => moveSpeed;
        public int MoneyReward => moneyReward;
        public int DamageToPlayerBase => damageToPlayerBase;
        public EnemyMovementType MovementType => movementType;
        public float HeightAboveTheGround => heightAboveTheGround;

        public float GetResistance(DamageType damageType)
        {
            foreach (DamageResistance resistance in resistances)
            {
                if (resistance.DamageType == damageType)
                {
                    return resistance.ResistancePercent;
                }
            }

            return 0f;
        }
    }
}