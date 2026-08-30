using System.Collections.Generic;
using TowerDefense.EnemySystem;
using UnityEngine;

namespace TowerDefense.GameSystem
{
    [CreateAssetMenu(fileName = "Difficulty Definition", menuName = "Tower Defense/Difficulty Definition")]
    public class DifficultyDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string difficultyName = "Easy";
        [SerializeField, Min(0)] private int difficultyIndex = 0;
        [TextArea(2, 5)]
        [SerializeField] private string description = "A relaxed start for new players.";

        [Header("Level Config")]
        [SerializeField] private List<WaveDefinition> waves = new List<WaveDefinition>();
        [SerializeField, Min(1f)] private float playerHp = 20f;
        [SerializeField] private List<EnemyDefinition> enemyTypes = new List<EnemyDefinition>();

        [Header("Difficulty Scaling")]
        [SerializeField, Min(0f)] private float enemyHpBonus = 0f;

        public string DifficultyName => difficultyName;
        public int DifficultyIndex => difficultyIndex;
        public string Description => description;
        public IReadOnlyList<WaveDefinition> Waves => waves;
        public int WaveCount => waves.Count;
        public float PlayerHp => playerHp;
        public IReadOnlyList<EnemyDefinition> EnemyTypes => enemyTypes;
        public float EnemyHpBonus => enemyHpBonus;
    }
}