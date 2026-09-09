using UnityEngine;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public class TowerCodexPanelController : MonoBehaviour
    {
        [SerializeField] private TowerCodexButton[] towerCodexButtons;
        [SerializeField] private TowerInfoCodexPanel towerInfoCodexPanel;

        private void Awake()
        {
            if (towerCodexButtons == null)
            {
                return;
            }

            foreach (TowerCodexButton codexButton in towerCodexButtons)
            {
                if (codexButton != null)
                {
                    codexButton.Clicked += HandleTowerCodexButtonClicked;
                }
            }

            if (towerInfoCodexPanel != null)
            {
                towerInfoCodexPanel.Hide();
            }
        }

        private void OnDestroy()
        {
            if (towerCodexButtons == null)
            {
                return;
            }

            foreach (TowerCodexButton codexButton in towerCodexButtons)
            {
                if (codexButton != null)
                {
                    codexButton.Clicked -= HandleTowerCodexButtonClicked;
                }
            }
        }

        private void HandleTowerCodexButtonClicked(TowerCodexButton clickedButton)
        {
            if (towerInfoCodexPanel != null)
            {
                towerInfoCodexPanel.Show(clickedButton.Tower);
            }
        }
    }
}