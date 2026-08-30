using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public class MainMenuController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject playPanel;
        [SerializeField] private GameObject codexPanel;
        [SerializeField] private GameObject settingsPanel;

        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button codexButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button backButton;

        [Header("Back Button Position")]
        [SerializeField] private Vector2 backButtonDefaultPosition = new Vector2(0f, -275f);
        [SerializeField] private Vector2 backButtonPlayPanelPosition = new Vector2(-125f, -275f);

        private readonly Stack<GameObject> panelHistory = new Stack<GameObject>();
        private GameObject currentPanel;
        private RectTransform backButtonRect;

        private void Awake()
        {
            if (backButton != null)
            {
                backButtonRect = backButton.transform as RectTransform;
            }

            if (playButton != null)
            {
                playButton.onClick.AddListener(() => OpenPanel(playPanel));
            }

            if (codexButton != null)
            {
                codexButton.onClick.AddListener(() => OpenPanel(codexPanel));
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(() => OpenPanel(settingsPanel));
            }

            if (exitButton != null)
            {
                exitButton.onClick.AddListener(HandleExitClicked);
            }

            if (backButton != null)
            {
                backButton.onClick.AddListener(GoBack);
            }

            HideAllPanels();
            ShowPanelImmediate(mainPanel);
        }

        private void OnDestroy()
        {
            if (playButton != null)
            {
                playButton.onClick.RemoveListener(() => OpenPanel(playPanel));
            }

            if (codexButton != null)
            {
                codexButton.onClick.RemoveListener(() => OpenPanel(codexPanel));
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.RemoveListener(() => OpenPanel(settingsPanel));
            }

            if (exitButton != null)
            {
                exitButton.onClick.RemoveListener(HandleExitClicked);
            }

            if (backButton != null)
            {
                backButton.onClick.RemoveListener(GoBack);
            }
        }

        public void OpenPanel(GameObject panel)
        {
            if (panel == null || panel == currentPanel)
            {
                return;
            }

            if (currentPanel != null)
            {
                panelHistory.Push(currentPanel);
                currentPanel.SetActive(false);
            }

            currentPanel = panel;
            currentPanel.SetActive(true);

            RefreshBackButton();
        }

        public void GoBack()
        {
            if (panelHistory.Count == 0)
            {
                return;
            }

            if (currentPanel != null)
            {
                currentPanel.SetActive(false);
            }

            currentPanel = panelHistory.Pop();
            currentPanel.SetActive(true);

            RefreshBackButton();
        }

        private void ShowPanelImmediate(GameObject panel)
        {
            if (panel == null)
            {
                return;
            }

            currentPanel = panel;
            currentPanel.SetActive(true);

            RefreshBackButton();
        }

        private void RefreshBackButton()
        {
            if (backButton != null)
            {
                backButton.gameObject.SetActive(panelHistory.Count > 0);
            }

            RefreshBackButtonPosition();
        }

        private void RefreshBackButtonPosition()
        {
            if (backButtonRect == null)
            {
                return;
            }

            backButtonRect.anchoredPosition = currentPanel == playPanel
                ? backButtonPlayPanelPosition
                : backButtonDefaultPosition;
        }

        private void HideAllPanels()
        {
            SetPanelActive(mainPanel, false);
            SetPanelActive(playPanel, false);
            SetPanelActive(codexPanel, false);
            SetPanelActive(settingsPanel, false);
        }

        private static void SetPanelActive(GameObject panel, bool active)
        {
            if (panel != null)
            {
                panel.SetActive(active);
            }
        }

        private void HandleExitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}