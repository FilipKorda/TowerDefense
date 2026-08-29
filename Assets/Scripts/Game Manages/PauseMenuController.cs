using TowerDefense.PlayerSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        [SerializeField] private GameObject[] panelsToToggle;

        private bool isPaused;

        private void Awake()
        {
            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(Resume);
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartLevel);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(GoToMainMenu);
            }

            HidePauseImmediate();
        }

        private void OnDestroy()
        {
            if (resumeButton != null)
            {
                resumeButton.onClick.RemoveListener(Resume);
            }

            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(RestartLevel);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.RemoveListener(GoToMainMenu);
            }
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        public void Pause()
        {
            if (isPaused || (PlayerStats.Instance != null && !PlayerStats.Instance.IsAlive))
            {
                return;
            }

            isPaused = true;
            Time.timeScale = 0f;

            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
            }

            TogglePanel(false);
        }

        public void Resume()
        {
            if (!isPaused)
            {
                return;
            }

            isPaused = false;
            Time.timeScale = 1f;

            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }

            TogglePanel(true);
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }

        private void HidePauseImmediate()
        {
            isPaused = false;

            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
        }

        private void TogglePanel(bool toggle)
        {
            foreach (GameObject panel in panelsToToggle)
            {
                if (panel != null)
                {
                    panel.SetActive(toggle);
                }
            }
        }
    }
}