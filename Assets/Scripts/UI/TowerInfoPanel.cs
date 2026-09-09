using TMPro;
using TowerDefense.BuildSystem;
using UnityEngine;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public class TowerInfoPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform panelRoot;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI attackSpeedText;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private TextMeshProUGUI attackRangeText;
        [SerializeField] private float spacingFromAnchor = 10f;

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

        public void Show(TowerDefinition towerDefinition, RectTransform anchor)
        {
            if (towerDefinition == null)
            {
                Hide();
                return;
            }

            if (nameText != null)
            {
                nameText.text = towerDefinition.DisplayName;
            }

            if (descriptionText != null)
            {
                descriptionText.text = towerDefinition.TowerDescription;
            }

            if (attackSpeedText != null)
            {
                attackSpeedText.text = $"Attack Speed: {towerDefinition.AttackSpeed:0.#}";
            }

            if (damageText != null)
            {
                damageText.text = $"Damage: {towerDefinition.Damage:0.#}";
            }

            if (attackRangeText != null)
            {
                attackRangeText.text = $"Range: {towerDefinition.AttackRange:0.#}";
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

        public void Hide()
        {
            if (panelRoot != null)
            {
                panelRoot.gameObject.SetActive(false);
            }
        }
    }
}