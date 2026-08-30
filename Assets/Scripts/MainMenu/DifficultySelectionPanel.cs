using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using TowerDefense.Combat;
using TowerDefense.GameSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public class DifficultySelectionPanel : MonoBehaviour
    {
        [Header("Difficulty")]
        [SerializeField] private DifficultyButton[] difficultyButtons;
        [SerializeField] private TextMeshProUGUI infoNameText;
        [SerializeField] private TextMeshProUGUI infoDescriptionText;
        [SerializeField] private TextMeshProUGUI infoWaveCountText;
        [SerializeField] private TextMeshProUGUI infoPlayerHpText;
        [SerializeField] private TextMeshProUGUI allEnemiesHpUpText;
        [SerializeField] private TextMeshProUGUI infoEnemyTypesText;
        [SerializeField] private Button startButton;

        [Header("Level")]
        [SerializeField] private LevelButton[] levelButtons;
        [SerializeField] private TextMeshProUGUI selectedLevelText;

        [Header("Shared Colors")]
        [SerializeField] private Color selectedButtonColor = new Color(0.3f, 0.6f, 1f);
        [SerializeField] private Color normalButtonColor = Color.white;

        private DifficultyDefinition selectedDifficulty;
        private DifficultyButton selectedDifficultyButton;
        private LevelButton selectedLevelButton;

        public LevelDefinition SelectedLevel => selectedLevelButton != null ? selectedLevelButton.Level : null;
        public DifficultyDefinition SelectedDifficulty => selectedDifficulty;

        private void Awake()
        {
            SubscribeDifficultyButtons();
            SubscribeLevelButtons();
            RefreshLevelUnlocks();

            if (startButton != null)
            {
                startButton.onClick.AddListener(HandleStartClicked);
            }

            SelectDefaultDifficulty();
            selectedLevelText.text = "Selected: None";
        }

        private void OnDestroy()
        {
            if (startButton != null)
            {
                startButton.onClick.RemoveListener(HandleStartClicked);
            }

            UnsubscribeDifficultyButtons();
            UnsubscribeLevelButtons();
        }

        private void SubscribeDifficultyButtons()
        {
            if (difficultyButtons == null)
            {
                return;
            }

            foreach (DifficultyButton difficultyButton in difficultyButtons)
            {
                if (difficultyButton != null)
                {
                    difficultyButton.Clicked += HandleDifficultyButtonClicked;
                }
            }
        }

        private void UnsubscribeDifficultyButtons()
        {
            if (difficultyButtons == null)
            {
                return;
            }

            foreach (DifficultyButton difficultyButton in difficultyButtons)
            {
                if (difficultyButton != null)
                {
                    difficultyButton.Clicked -= HandleDifficultyButtonClicked;
                }
            }
        }

        private void SelectDefaultDifficulty()
        {
            if (difficultyButtons == null || difficultyButtons.Length == 0 || difficultyButtons[0] == null)
            {
                RefreshDifficultyInfo();
                return;
            }

            HandleDifficultyButtonClicked(difficultyButtons[0]);
        }

        private void HandleDifficultyButtonClicked(DifficultyButton clickedButton)
        {
            selectedDifficultyButton = clickedButton;
            selectedDifficulty = clickedButton.Difficulty;

            foreach (DifficultyButton difficultyButton in difficultyButtons)
            {
                if (difficultyButton != null)
                {
                    difficultyButton.SetSelected(difficultyButton == clickedButton);
                }
            }

            RefreshAllLevelStars();
            RefreshDifficultyInfo();
        }

        private void RefreshAllLevelStars()
        {
            if (levelButtons == null || selectedDifficulty == null)
            {
                return;
            }

            foreach (LevelButton levelButton in levelButtons)
            {
                if (levelButton != null)
                {
                    levelButton.RefreshStarsForDifficulty(selectedDifficulty.DifficultyIndex);
                }
            }
        }

        private void RefreshLevelUnlocks()
        {
            if (levelButtons == null || levelButtons.Length == 0)
            {
                return;
            }

            LevelButton[] orderedButtons = levelButtons
                .Where(button => button != null && button.Level != null)
                .OrderBy(button => button.Level.LevelIndex)
                .ToArray();

            for (int i = 0; i < orderedButtons.Length; i++)
            {
                if (i == 0)
                {
                    orderedButtons[i].SetUnlocked(true);
                    continue;
                }

                LevelButton previousButton = orderedButtons[i - 1];
                int previousStars = LevelProgressStore.GetStars(previousButton.Level);

                orderedButtons[i].SetUnlocked(previousStars >= 1);
            }
        }

        private void SubscribeLevelButtons()
        {
            if (levelButtons == null)
            {
                return;
            }

            foreach (LevelButton levelButton in levelButtons)
            {
                if (levelButton != null)
                {
                    levelButton.Clicked += HandleLevelButtonClicked;
                }
            }
        }

        private void UnsubscribeLevelButtons()
        {
            if (levelButtons == null)
            {
                return;
            }

            foreach (LevelButton levelButton in levelButtons)
            {
                if (levelButton != null)
                {
                    levelButton.Clicked -= HandleLevelButtonClicked;
                }
            }
        }

        private void HandleLevelButtonClicked(LevelButton clickedButton)
        {
            selectedLevelButton = clickedButton;

            foreach (LevelButton levelButton in levelButtons)
            {
                if (levelButton != null)
                {
                    levelButton.SetSelected(levelButton == clickedButton);
                }
            }

            if (selectedLevelText != null && clickedButton.Level != null)
            {
                selectedLevelText.text = $"Selected: {clickedButton.Level.LevelName}";
            }
        }

        private void RefreshDifficultyInfo()
        {
            if (selectedDifficulty == null)
            {
                ClearDifficultyInfo();
                return;
            }

            if (infoNameText != null)
            {
                infoNameText.text = selectedDifficulty.DifficultyName;
            }

            if (infoDescriptionText != null)
            {
                infoDescriptionText.text = selectedDifficulty.Description;
            }

            if (infoWaveCountText != null)
            {
                infoWaveCountText.text = $"Waves: {selectedDifficulty.WaveCount}";
            }

            if (infoPlayerHpText != null)
            {
                infoPlayerHpText.text = $"HP: {selectedDifficulty.PlayerHp:0.#}";
            }

            if (allEnemiesHpUpText != null)
            {
                allEnemiesHpUpText.text = selectedDifficulty.EnemyHpBonus > 0f
                    ? $"All enemies has +{selectedDifficulty.EnemyHpBonus} HP"
                    : "All enemies has normal HP ";
            }

            if (infoEnemyTypesText != null)
            {
                infoEnemyTypesText.text = BuildEnemyTypesText(selectedDifficulty.EnemyTypes);
            }

            if (startButton != null)
            {
                startButton.interactable = true;
            }
        }

        private static string BuildEnemyTypesText(IReadOnlyList<EnemySystem.EnemyDefinition> enemyTypes)
        {
            if (enemyTypes == null || enemyTypes.Count == 0)
            {
                return "Enemies: -";
            }

            StringBuilder builder = new StringBuilder("Enemies:\n");

            for (int i = 0; i < enemyTypes.Count; i++)
            {
                EnemySystem.EnemyDefinition enemy = enemyTypes[i];

                if (enemy == null)
                {
                    continue;
                }

                builder.Append($" | <color=#FFFFFF>{enemy.DisplayName}</color>");
                builder.Append($" | <color=#FF3B30> HP: {enemy.MaxHp:0.#}</color>");
                builder.Append($" | <color=#3B82F6>{enemy.MovementType}</color>");
                builder.Append(BuildResistancesText(enemy));

                if (i < enemyTypes.Count - 1)
                {
                    builder.Append('\n');
                }
            }

            return builder.ToString();
        }

        private static string BuildResistancesText(EnemySystem.EnemyDefinition enemy)
        {
            StringBuilder resistanceBuilder = new StringBuilder();

            foreach (DamageType damageType in Enum.GetValues(typeof(DamageType)))
            {
                float resistance = enemy.GetResistance(damageType);

                if (resistance <= 0f)
                {
                    continue;
                }

                resistanceBuilder.Append($" | <color=#A855F7>{damageType} Res: {resistance * 100f:0}%</color>");
            }

            return resistanceBuilder.ToString();
        }

        private void ClearDifficultyInfo()
        {
            if (infoNameText != null)
            {
                infoNameText.text = string.Empty;
            }

            if (infoDescriptionText != null)
            {
                infoDescriptionText.text = string.Empty;
            }

            if (infoWaveCountText != null)
            {
                infoWaveCountText.text = string.Empty;
            }

            if (infoPlayerHpText != null)
            {
                infoPlayerHpText.text = string.Empty;
            }

            if (allEnemiesHpUpText != null)
            {
                allEnemiesHpUpText.text = string.Empty;
            }

            if (infoEnemyTypesText != null)
            {
                infoEnemyTypesText.text = string.Empty;
            }

            if (startButton != null)
            {
                startButton.interactable = false;
            }
        }

        private void HandleStartClicked()
        {
            if (SelectedLevel == null || string.IsNullOrEmpty(SelectedLevel.ScenePath))
            {
                Debug.LogWarning("DifficultySelectionPanel: brak wybranego poziomu lub nie przypisano sceny w LevelDefinition.");
                return;
            }

            GameSession.SetSelection(SelectedLevel, selectedDifficulty);
            SceneManager.LoadScene(SelectedLevel.ScenePath);
        }
    }
}