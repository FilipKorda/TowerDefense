using System;
using System.Collections;
using TMPro;
using TowerDefense.PlayerSystem;
using TowerDefense.TowerSystem;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public class TowerUpgradePanel : MonoBehaviour
    {
        [SerializeField] private RectTransform panelRoot;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button sellButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI upgradeCostText;
        [SerializeField] private TextMeshProUGUI sellRefundText;
        [SerializeField] private TextMeshProUGUI upgradeButtonStatusText;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private TextMeshProUGUI attackSpeedText;
        [SerializeField] private TextMeshProUGUI attackRangeText;
        [SerializeField] private UpgradeInfoPanel upgradeInfoPanel;
        [SerializeField] private Color boostedStatColor = new Color(0.3f, 0.85f, 1f);
        [SerializeField] private Color normalStatColor = Color.white;

        public event Action Closed;
        public event Action<TowerRuntime> Sold;

        private TowerRuntime targetTower;
        private UpgradeButtonHover upgradeButtonHover;

        private void Awake()
        {
            if (upgradeButton != null)
            {
                upgradeButton.onClick.AddListener(HandleUpgradeClicked);

                upgradeButtonHover = upgradeButton.GetComponent<UpgradeButtonHover>();

                upgradeButtonHover.Highlighted += HandleUpgradeButtonHighlighted;
                upgradeButtonHover.Unhighlighted += HandleUpgradeButtonUnhighlighted;
            }

            if (sellButton != null)
            {
                sellButton.onClick.AddListener(HandleSellClicked);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(HandleCloseClicked);
            }

            Hide();
        }

        private void OnDestroy()
        {
            if (upgradeButton != null)
            {
                upgradeButton.onClick.RemoveListener(HandleUpgradeClicked);
            }

            if (upgradeButtonHover != null)
            {
                upgradeButtonHover.Highlighted -= HandleUpgradeButtonHighlighted;
                upgradeButtonHover.Unhighlighted -= HandleUpgradeButtonUnhighlighted;
            }

            if (sellButton != null)
            {
                sellButton.onClick.RemoveListener(HandleSellClicked);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(HandleCloseClicked);
            }
        }

        private void OnEnable()
        {
            StartCoroutine(SubscribeWhenReady());
        }

        private void OnDisable()
        {
            StopAllCoroutines();

            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.OnMoneyChanged -= HandleMoneyChanged;
            }
        }

        private IEnumerator SubscribeWhenReady()
        {
            while (PlayerStats.Instance == null)
            {
                yield return null;
            }

            PlayerStats.Instance.OnMoneyChanged += HandleMoneyChanged;
        }

        public void Show(TowerRuntime towerRuntime)
        {
            if (towerRuntime == null)
            {
                Hide();
                return;
            }

            targetTower = towerRuntime;
            upgradeButtonHover.SetTargetTower(targetTower);
            panelRoot.gameObject.SetActive(true);
            RefreshContent();
        }

        public void Hide()
        {
            targetTower = null;

            if (panelRoot != null)
            {
                panelRoot.gameObject.SetActive(false);
            }

            if (upgradeInfoPanel != null)
            {
                upgradeInfoPanel.Hide();
            }
        }

        private void HandleMoneyChanged(int currentMoney)
        {
            RefreshContent();
        }

        private void RefreshContent()
        {
            if (targetTower == null || targetTower.Definition == null)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = $"{targetTower.Definition.DisplayName} Lv. {targetTower.UpgradeLevel}";
            }

            bool canUpgrade = targetTower.CanUpgrade;
            int upgradeCost = targetTower.UpgradeCost;
            int currentMoney = PlayerStats.Instance != null ? PlayerStats.Instance.Money : 0;
            bool canAfford = currentMoney >= upgradeCost;

            if (upgradeCostText != null)
            {
                bool isDiscounted = targetTower.AppliedTileBonusType == TowerDefense.BuildSystem.TileBonusType.LowUpgradeCost;
                upgradeCostText.text = canUpgrade ? $"Upgrade cost: {upgradeCost}" : "Max poziom";
                upgradeCostText.color = canUpgrade && isDiscounted ? boostedStatColor : normalStatColor;
            }

            if (upgradeButton != null)
            {
                upgradeButton.interactable = canUpgrade && canAfford;
            }

            if (sellRefundText != null)
            {
                sellRefundText.text = $"Sell: {targetTower.GetSellRefund()}";
            }

            if (upgradeButtonStatusText != null)
            {
                if (!canUpgrade)
                {
                    upgradeButtonStatusText.text = "Max poziom";
                }
                else if (!canAfford)
                {
                    upgradeButtonStatusText.text = "Za malo pieniedzy";
                }
                else
                {
                    upgradeButtonStatusText.text = "Upgrade";
                }
            }

            RefreshStatTexts();
        }

        private void RefreshStatTexts()
        {
            TowerDefense.BuildSystem.TileBonusType bonusType = targetTower.AppliedTileBonusType;

            if (damageText != null)
            {
                bool isBoosted = bonusType == TowerDefense.BuildSystem.TileBonusType.DamageBoost;
                damageText.text = $"Damage: {FormatValue(targetTower.Damage)}";
                damageText.color = isBoosted ? boostedStatColor : normalStatColor;
            }

            if (attackSpeedText != null)
            {
                bool isBoosted = bonusType == TowerDefense.BuildSystem.TileBonusType.AttackSpeedBoost;
                attackSpeedText.text = $"Attack Speed: {FormatValue(targetTower.AttackSpeed)}";
                attackSpeedText.color = isBoosted ? boostedStatColor : normalStatColor;
            }

            if (attackRangeText != null)
            {
                bool isBoosted = bonusType == TowerDefense.BuildSystem.TileBonusType.RangeBoost;
                attackRangeText.text = $"Range: {FormatValue(targetTower.AttackRange)}";
                attackRangeText.color = isBoosted ? boostedStatColor : normalStatColor;
            }
        }

        private static string FormatValue(float value)
        {
            return value.ToString("0.#");
        }

        private void HandleUpgradeButtonHighlighted(TowerRuntime towerRuntime)
        {
            upgradeInfoPanel.Show(towerRuntime, panelRoot);
        }

        private void HandleUpgradeButtonUnhighlighted()
        {
            upgradeInfoPanel.Hide();
        }

        private void HandleUpgradeClicked()
        {
            if (targetTower == null || !targetTower.CanUpgrade)
            {
                return;
            }

            if (PlayerStats.Instance == null || !PlayerStats.Instance.TrySpendMoney(targetTower.UpgradeCost))
            {
                return;
            }

            targetTower.TryUpgrade();
            RefreshContent();
            RefreshUpgradeInfoPanel();
        }

        private void RefreshUpgradeInfoPanel()
        {
            if (upgradeInfoPanel == null || panelRoot == null)
            {
                return;
            }

            if (targetTower != null && targetTower.CanUpgrade)
            {
                upgradeInfoPanel.Show(targetTower, panelRoot);
            }
            else
            {
                upgradeInfoPanel.Hide();
            }
        }

        private void HandleSellClicked()
        {
            if (targetTower == null)
            {
                return;
            }

            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.AddMoney(targetTower.GetSellRefund());
            }

            Sold?.Invoke(targetTower);
            Hide();
        }

        private void HandleCloseClicked()
        {
            Closed?.Invoke();
            Hide();
        }
    }
}