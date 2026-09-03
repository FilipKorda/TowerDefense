using System;
using System.Text;
using TMPro;
using TowerDefense.Combat;
using TowerDefense.EnemySystem;
using UnityEngine;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public class CodexInfoPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI maxHpText;
        [SerializeField] private TextMeshProUGUI moveSpeedText;
        [SerializeField] private TextMeshProUGUI moneyRewardText;
        [SerializeField] private TextMeshProUGUI movementTypeText;
        [SerializeField] private TextMeshProUGUI resistancesText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        public void Show(EnemyDefinition enemy)
        {
            if (enemy == null)
            {
                Hide();
                return;
            }

            descriptionText.text = enemy.EnemyDescription;
            nameText.text = enemy.DisplayName;
            maxHpText.text = $"Max HP: {enemy.MaxHp:0.#}";
            moveSpeedText.text = $"Move Speed: {enemy.MoveSpeed:0.#}";
            moneyRewardText.text = $"Money Reward: {enemy.MoneyReward}";
            movementTypeText.text = $"Movement: {enemy.MovementType}";
            resistancesText.text = BuildResistancesText(enemy);
            panelRoot.SetActive(true);
        }

        public void Hide()
        {
            panelRoot.SetActive(false);
        }

        private static string BuildResistancesText(EnemyDefinition enemy)
        {
            StringBuilder builder = new StringBuilder("Resistances:\n");
            bool hasAny = false;

            foreach (DamageType damageType in Enum.GetValues(typeof(DamageType)))
            {
                float resistance = enemy.GetResistance(damageType);

                if (resistance <= 0f)
                {
                    continue;
                }

                if (hasAny)
                {
                    builder.Append('\n');
                }

                builder.Append($"- {damageType}: {resistance * 100f:0}%");
                hasAny = true;
            }

            if (!hasAny)
            {
                builder.Append("- None");
            }

            return builder.ToString();
        }
    }
}