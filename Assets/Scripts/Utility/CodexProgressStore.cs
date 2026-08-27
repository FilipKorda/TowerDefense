using TowerDefense.BuildSystem;
using TowerDefense.EnemySystem;
using UnityEngine;

namespace TowerDefense.GameSystem
{
    public static class CodexProgressStore
    {
        private const string EnemyKeyPrefix = "CodexEnemyUnlocked_";
        private const string TowerKeyPrefix = "CodexTowerUnlocked_";
        private const string EnemySeenKeyPrefix = "CodexEnemySeen_";
        private const string TowerSeenKeyPrefix = "CodexTowerSeen_";

        public static void UnlockEnemy(EnemyDefinition enemy)
        {
            if (enemy == null || IsEnemyUnlocked(enemy))
            {
                return;
            }

            PlayerPrefs.SetInt(EnemyKeyPrefix + enemy.name, 1);
            PlayerPrefs.Save();
        }

        public static bool IsEnemyUnlocked(EnemyDefinition enemy)
        {
            if (enemy == null)
            {
                return false;
            }

            return PlayerPrefs.GetInt(EnemyKeyPrefix + enemy.name, 0) == 1;
        }

        public static void UnlockTower(TowerDefinition tower)
        {
            if (tower == null || IsTowerUnlocked(tower))
            {
                return;
            }

            PlayerPrefs.SetInt(TowerKeyPrefix + tower.name, 1);
            PlayerPrefs.Save();
        }

        public static bool IsTowerUnlocked(TowerDefinition tower)
        {
            if (tower == null)
            {
                return false;
            }

            return PlayerPrefs.GetInt(TowerKeyPrefix + tower.name, 0) == 1;
        }

        public static void MarkEnemySeen(EnemyDefinition enemy)
        {
            if (enemy == null)
            {
                return;
            }

            PlayerPrefs.SetInt(EnemySeenKeyPrefix + enemy.name, 1);
            PlayerPrefs.Save();
        }

        public static void MarkTowerSeen(TowerDefinition tower)
        {
            if (tower == null)
            {
                return;
            }

            PlayerPrefs.SetInt(TowerSeenKeyPrefix + tower.name, 1);
            PlayerPrefs.Save();
        }

        public static bool IsEnemyUnseenAndUnlocked(EnemyDefinition enemy)
        {
            return IsEnemyUnlocked(enemy) && PlayerPrefs.GetInt(EnemySeenKeyPrefix + enemy.name, 0) == 0;
        }

        public static bool IsTowerUnseenAndUnlocked(TowerDefinition tower)
        {
            return IsTowerUnlocked(tower) && PlayerPrefs.GetInt(TowerSeenKeyPrefix + tower.name, 0) == 0;
        }
    }
}