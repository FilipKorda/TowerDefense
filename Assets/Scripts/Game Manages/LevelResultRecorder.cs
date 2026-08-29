using TowerDefense.EnemySystem;
using TowerDefense.PlayerSystem;
using UnityEngine;

namespace TowerDefense.GameSystem
{
    [DisallowMultipleComponent]
    public class LevelResultRecorder : MonoBehaviour
    {
        [SerializeField] private EnemySpawner enemySpawner;

        private void OnEnable()
        {
            if (enemySpawner != null)
            {
                enemySpawner.OnAllWavesCompleted += HandleLevelWon;
            }
        }

        private void OnDisable()
        {
            if (enemySpawner != null)
            {
                enemySpawner.OnAllWavesCompleted -= HandleLevelWon;
            }
        }

        private void HandleLevelWon()
        {

            int stars = StarRatingCalculator.CalculateStars(PlayerStats.Instance.CurrentHp, PlayerStats.Instance.MaxHp);

            LevelProgressStore.SaveStars(GameSession.SelectedLevel, GameSession.SelectedDifficulty.DifficultyIndex, stars);
            LevelProgressStore.SaveCompletedDifficulty(GameSession.SelectedLevel, GameSession.SelectedDifficulty.DifficultyIndex);
        }
    }
}