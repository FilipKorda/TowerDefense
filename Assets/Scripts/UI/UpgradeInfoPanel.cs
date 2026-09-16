using TMPro;
using TowerDefense.BuildSystem;
using TowerDefense.TowerSystem;
using UnityEngine;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public class UpgradeInfoPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform panelRoot;
        [SerializeField] private TextMeshProUGUI upgradeInfo;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private TextMeshProUGUI attackSpeedText;
        [SerializeField] private TextMeshProUGUI attackRangeText;
        [SerializeField] private string arrowFormat = "<color=#FFA500>-></color>";
        [SerializeField] private float spacingFromAnchor = 10f;
        [SerializeField] private Color boostedStatColor = new Color(0.3f, 0.85f, 1f);
        [SerializeField] private Color normalStatColor = Color.white;

        private Canvas parentCanvas;

        private void Awake()
        {
            if (panelRoot == null)
            {
                panelRoot = transform as RectTransform;
            }

            parentCanvas = GetComponentInParent<Canvas>();

            Hide();
        }

        public void Show(TowerRuntime towerRuntime, RectTransform anchor)
        {
            if (towerRuntime == null || towerRuntime.Definition == null)
            {
                Hide();
                return;
            }

            if (!towerRuntime.CanUpgrade)
            {
                Hide();
                return;
            }

            TowerDefinition definition = towerRuntime.Definition;
            TileBonusType bonusType = towerRuntime.AppliedTileBonusType;

            float nextDamage = towerRuntime.Damage * definition.UpgradeDamageMultiplier;
            float nextAttackSpeed = towerRuntime.AttackSpeed * definition.UpgradeAttackSpeedMultiplier;
            float nextAttackRange = towerRuntime.AttackRange * definition.UpgradeAttackRangeMultiplier;

            if (upgradeInfo != null)
            {
                upgradeInfo.text = "Upgrade Info";
            }

            if (damageText != null)
            {
                bool isBoosted = bonusType == TileBonusType.DamageBoost;
                damageText.text = $"Damage: {FormatValue(towerRuntime.Damage)} {arrowFormat} {FormatValue(nextDamage)}";
                damageText.color = isBoosted ? boostedStatColor : normalStatColor;
            }

            if (attackSpeedText != null)
            {
                bool isBoosted = bonusType == TileBonusType.AttackSpeedBoost;
                attackSpeedText.text = $"Attack Speed: {FormatValue(towerRuntime.AttackSpeed)} {arrowFormat} {FormatValue(nextAttackSpeed)}";
                attackSpeedText.color = isBoosted ? boostedStatColor : normalStatColor;
            }

            if (attackRangeText != null)
            {
                bool isBoosted = bonusType == TileBonusType.RangeBoost;
                attackRangeText.text = $"Attack Range: {FormatValue(towerRuntime.AttackRange)} {arrowFormat} {FormatValue(nextAttackRange)}";
                attackRangeText.color = isBoosted ? boostedStatColor : normalStatColor;
            }

            if (panelRoot != null)
            {
                panelRoot.gameObject.SetActive(true);

                if (anchor != null)
                {
                    ScreenBoundsUtility.PositionNextToAnchor(panelRoot, anchor, parentCanvas, spacingFromAnchor);
                }
            }
        }

        private static string FormatValue(float value)
        {
            return value.ToString("0.#");
        }

        public void Hide()
        {
            if (panelRoot != null)
            {
                panelRoot.gameObject.SetActive(false);
            }
        }
    }
}