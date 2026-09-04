using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.EnemySystem
{
    [System.Serializable]
    public class BurstDefinition
    {
        [SerializeField] private EnemyDefinition enemyDefinition;
        [SerializeField] private int enemyCount = 1;
        [SerializeField, Min(0.1f)] private float formationSpacing = 1f;

        public EnemyDefinition EnemyDefinition => enemyDefinition;
        public int EnemyCount => enemyCount;
        public float FormationSpacing => formationSpacing;
    }

    [CreateAssetMenu(fileName = "Wave Definition", menuName = "Tower Defense/Wave Definition")]
    public class WaveDefinition : ScriptableObject
    {
        [SerializeField] private List<BurstDefinition> bursts = new List<BurstDefinition>();
        [SerializeField, Min(0.05f)] private float timeBetweenBursts = 0.8f;

        public IReadOnlyList<BurstDefinition> Bursts => bursts;
        public float TimeBetweenBursts => timeBetweenBursts;
    }
}