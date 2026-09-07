using UnityEngine;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public class CodexController : MonoBehaviour
    {
        [Header("Enemy Codex")]
        [SerializeField] private CodexButton[] enemyCodexButtons;
        [SerializeField] private CodexInfoPanel enemyInfoPanel;

        [Header("Tower Codex")]
        [SerializeField] private TowerCodexButton[] towerCodexButtons;
        [SerializeField] private TowerInfoCodexPanel towerInfoCodexPanel;

        private void Awake()
        {
            SubscribeEnemyButtons();
            SubscribeTowerButtons();

            HideAllPanels();
        }

        private void OnDestroy()
        {
            UnsubscribeEnemyButtons();
            UnsubscribeTowerButtons();
        }

        private void SubscribeEnemyButtons()
        {
            if (enemyCodexButtons == null)
            {
                return;
            }

            foreach (CodexButton codexButton in enemyCodexButtons)
            {
                if (codexButton != null)
                {
                    codexButton.Clicked += HandleEnemyButtonClicked;
                }
            }
        }

        private void UnsubscribeEnemyButtons()
        {
            if (enemyCodexButtons == null)
            {
                return;
            }

            foreach (CodexButton codexButton in enemyCodexButtons)
            {
                if (codexButton != null)
                {
                    codexButton.Clicked -= HandleEnemyButtonClicked;
                }
            }
        }

        private void SubscribeTowerButtons()
        {
            if (towerCodexButtons == null)
            {
                return;
            }

            foreach (TowerCodexButton codexButton in towerCodexButtons)
            {
                if (codexButton != null)
                {
                    codexButton.Clicked += HandleTowerButtonClicked;
                }
            }
        }

        private void UnsubscribeTowerButtons()
        {
            if (towerCodexButtons == null)
            {
                return;
            }

            foreach (TowerCodexButton codexButton in towerCodexButtons)
            {
                if (codexButton != null)
                {
                    codexButton.Clicked -= HandleTowerButtonClicked;
                }
            }
        }

        private void HandleEnemyButtonClicked(CodexButton clickedButton)
        {
            HideAllPanels();

            if (enemyInfoPanel != null)
            {
                enemyInfoPanel.Show(clickedButton.Enemy);
            }
        }

        private void HandleTowerButtonClicked(TowerCodexButton clickedButton)
        {
            HideAllPanels();

            if (towerInfoCodexPanel != null)
            {
                towerInfoCodexPanel.Show(clickedButton.Tower);
            }
        }

        private void HideAllPanels()
        {
            if (enemyInfoPanel != null)
            {
                enemyInfoPanel.Hide();
            }

            if (towerInfoCodexPanel != null)
            {
                towerInfoCodexPanel.Hide();
            }
        }
    }
}