using System.Collections.Generic;
using TMPro;
using TowerDefense.BuildSystem;
using TowerDefense.EnemySystem;
using TowerDefense.GameSystem;
using UnityEngine;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public class CodexNotificationBadge : MonoBehaviour
    {
        [SerializeField] private GameObject badgeRoot;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private List<EnemyDefinition> allEnemies = new List<EnemyDefinition>();
        [SerializeField] private List<TowerDefinition> allTowers = new List<TowerDefinition>();

        private void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            int unseenCount = 0;

            foreach (EnemyDefinition enemy in allEnemies)
            {
                if (enemy != null && CodexProgressStore.IsEnemyUnseenAndUnlocked(enemy))
                {
                    unseenCount++;
                }
            }

            foreach (TowerDefinition tower in allTowers)
            {
                if (tower != null && CodexProgressStore.IsTowerUnseenAndUnlocked(tower))
                {
                    unseenCount++;
                }
            }

            if (badgeRoot != null)
            {
                badgeRoot.SetActive(unseenCount > 0);
            }

            if (countText != null)
            {
                countText.text = unseenCount.ToString();
            }
        }
    }
}