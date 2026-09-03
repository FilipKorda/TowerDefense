using UnityEngine;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public class CodexPanelController : MonoBehaviour
    {
        [SerializeField] private CodexButton[] codexButtons;
        [SerializeField] private CodexInfoPanel codexInfoPanel;

        private void Awake()
        {
            if (codexButtons == null)
            {
                return;
            }

            foreach (CodexButton codexButton in codexButtons)
            {
                if (codexButton != null)
                {
                    codexButton.Clicked += HandleCodexButtonClicked;
                }
            }

            codexInfoPanel.Hide();
        }

        private void OnDestroy()
        {
            if (codexButtons == null)
            {
                return;
            }

            foreach (CodexButton codexButton in codexButtons)
            {
                if (codexButton != null)
                {
                    codexButton.Clicked -= HandleCodexButtonClicked;
                }
            }
        }

        private void HandleCodexButtonClicked(CodexButton clickedButton)
        {
            codexInfoPanel.Show(clickedButton.Enemy);
        }
    }
}