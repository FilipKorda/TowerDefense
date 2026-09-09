using System;
using System.Text;
using TMPro;
using TowerDefense.BuildSystem;
using TowerDefense.Combat;
using UnityEngine;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public class TowerInfoCodexPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI attackSpeedText;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private TextMeshProUGUI attackRangeText;
        [SerializeField] private TextMeshProUGUI damageTypeText;
        [SerializeField] private TextMeshProUGUI targetableTypesText;
        [SerializeField] private TextMeshProUGUI upgradeCostText;
        [SerializeField] private TextMeshProUGUI upgradeCostIncreaseText;
        [SerializeField] private TextMeshProUGUI maxUpgradeLevelText;
        [SerializeField] private TextMeshProUGUI sellRefundPercentText;

        public void Show(TowerDefinition tower)
        {
            if (tower == null)
            {
                Hide();
                return;
            }

            if (nameText != null)
            {
                nameText.text = tower.DisplayName;
            }

            if (descriptionText != null)
            {
                descriptionText.text = tower.TowerDescription;
            }

            if (costText != null)
            {
                costText.text = $"Cost: {tower.Cost}";
            }

            if (attackSpeedText != null)
            {
                attackSpeedText.text = $"Attack Speed: {tower.AttackSpeed:0.#}";
            }

            if (damageText != null)
            {
                damageText.text = $"Damage: {tower.Damage:0.#}";
            }

            if (attackRangeText != null)
            {
                attackRangeText.text = $"Attack Range: {tower.AttackRange:0.#}";
            }

            if (damageTypeText != null)
            {
                damageTypeText.text = $"Damage Type: {tower.DamageType}";
            }

            if (targetableTypesText != null)
            {
                targetableTypesText.text = $"Targets: {tower.TargetableTypes}";
            }

            if (upgradeCostText != null)
            {
                upgradeCostText.text = $"Upgrade Cost: {tower.UpgradeCost}";
            }

            if (upgradeCostIncreaseText != null)
            {
                upgradeCostIncreaseText.text = $"Upgrade Cost Increase: +{tower.UpgradeCostIncreasePerLevel} / level";
            }

            if (maxUpgradeLevelText != null)
            {
                maxUpgradeLevelText.text = $"Max Upgrade Level: {tower.MaxUpgradeLevel}";
            }

            if (sellRefundPercentText != null)
            {
                sellRefundPercentText.text = $"Sell Refund: {tower.SellRefundPercent * 100f:0}%";
            }

            if (panelRoot != null)
            {
                panelRoot.SetActive(true);
            }
        }

        public void Hide()
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(false);
            }
        }
    }
}