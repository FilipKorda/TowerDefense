using UnityEngine;

namespace TowerDefense.GameSystem
{
    public static class LevelProgressStore
    {
        private const string StarsKeyPrefix = "LevelStars_";
        private const string StarsPerDifficultyKeyPrefix = "LevelStarsDiff_";
        private const string DifficultyMaskKeyPrefix = "LevelDifficultyMask_";

        public static void SaveStars(LevelDefinition level, int difficultyIndex, int stars)
        {
            if (level == null || difficultyIndex < 0)
            {
                return;
            }

            string perDifficultyKey = StarsPerDifficultyKeyPrefix + level.LevelIndex + "_" + difficultyIndex;
            int existingPerDifficultyStars = PlayerPrefs.GetInt(perDifficultyKey, 0);

            if (stars > existingPerDifficultyStars)
            {
                PlayerPrefs.SetInt(perDifficultyKey, stars);
            }

            string overallKey = StarsKeyPrefix + level.LevelIndex;
            int existingOverallStars = PlayerPrefs.GetInt(overallKey, 0);

            if (stars > existingOverallStars)
            {
                PlayerPrefs.SetInt(overallKey, stars);
            }

            PlayerPrefs.Save();
        }

        public static int GetStars(LevelDefinition level)
        {
            if (level == null)
            {
                return 0;
            }

            return PlayerPrefs.GetInt(StarsKeyPrefix + level.LevelIndex, 0);
        }

        public static int GetStarsForDifficulty(LevelDefinition level, int difficultyIndex)
        {
            if (level == null || difficultyIndex < 0)
            {
                return 0;
            }

            return PlayerPrefs.GetInt(StarsPerDifficultyKeyPrefix + level.LevelIndex + "_" + difficultyIndex, 0);
        }

        public static void SaveCompletedDifficulty(LevelDefinition level, int difficultyIndex)
        {
            if (level == null || difficultyIndex < 0 || difficultyIndex > 30)
            {
                return;
            }

            int mask = GetCompletedDifficultyMask(level);
            int updatedMask = mask | (1 << difficultyIndex);

            if (updatedMask == mask)
            {
                return;
            }

            PlayerPrefs.SetInt(DifficultyMaskKeyPrefix + level.LevelIndex, updatedMask);
            PlayerPrefs.Save();
        }

        public static int GetCompletedDifficultyMask(LevelDefinition level)
        {
            if (level == null)
            {
                return 0;
            }

            return PlayerPrefs.GetInt(DifficultyMaskKeyPrefix + level.LevelIndex, 0);
        }

        public static bool IsDifficultyCompleted(LevelDefinition level, int difficultyIndex)
        {
            int mask = GetCompletedDifficultyMask(level);
            return (mask & (1 << difficultyIndex)) != 0;
        }
    }
}