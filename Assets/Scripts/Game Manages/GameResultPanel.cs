using TowerDefense.EnemySystem;
using TowerDefense.PlayerSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public class GameResultPanel : MonoBehaviour
    {
        [SerializeField] private EnemySpawner enemySpawner;
        [SerializeField] private GameObject resultPanel;

        [Header("Win")]
        [SerializeField] private GameObject winPanel;
        [SerializeField] private Image[] winStarIcons;
        [SerializeField] private Button winMainMenuButton;
        [SerializeField] private Button winRestartButton;

        [Header("Lose")]
        [SerializeField] private GameObject losePanel;
        [SerializeField] private Button loseMainMenuButton;
        [SerializeField] private Button loseRestartButton;

        [SerializeField] private string mainMenuSceneName = "MainMenu";

        private bool gameEnded;


        private void Start()
        {
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.OnPlayerDied += HandleLose;
            }
        }

        private void Awake()
        {
            HideAllImmediate();

            winMainMenuButton.onClick.AddListener(GoToMainMenu);
            winRestartButton.onClick.AddListener(RestartLevel);
            loseMainMenuButton.onClick.AddListener(GoToMainMenu);
            loseRestartButton.onClick.AddListener(RestartLevel);
        }

        private void OnDestroy()
        {
            winMainMenuButton.onClick.RemoveListener(GoToMainMenu);
            winRestartButton.onClick.RemoveListener(RestartLevel);
            loseMainMenuButton.onClick.RemoveListener(GoToMainMenu);
            loseRestartButton.onClick.RemoveListener(RestartLevel);
        }

        private void OnEnable()
        {
            enemySpawner.OnAllWavesCompleted += HandleWin;
           
        }

        private void OnDisable()
        {
            enemySpawner.OnAllWavesCompleted -= HandleWin;
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.OnPlayerDied -= HandleLose;
            }
        }

        private void HandleWin()
        {
            if (gameEnded)
            {
                return;
            }

            gameEnded = true;
            Time.timeScale = 0f;

            int stars = GameSystem.StarRatingCalculator.CalculateStars(PlayerStats.Instance.CurrentHp, PlayerStats.Instance.MaxHp);
            RefreshWinStars(stars);

            winPanel.SetActive(true);
            resultPanel.SetActive(true);
        }

        private void RefreshWinStars(int stars)
        {
            int clampedStars = Mathf.Clamp(stars, 0, winStarIcons.Length);

            for (int i = 0; i < winStarIcons.Length; i++)
            {
                if (winStarIcons[i] != null)
                {
                    winStarIcons[i].enabled = i < clampedStars;
                }
            }
        }

        private void HandleLose()
        {
            if (gameEnded)
            {
                return;
            }

            gameEnded = true;
            Time.timeScale = 0f;

            losePanel.SetActive(true);
            resultPanel.SetActive(true);
        }

        private void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }

        private void HideAllImmediate()
        {
            gameEnded = false;

            winPanel.SetActive(false);
            resultPanel.SetActive(false);

            losePanel.SetActive(false);
            resultPanel.SetActive(false);
        }
    }
}