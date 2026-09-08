using System;
using TMPro;
using TowerDefense.GameSystem;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public class LevelButton : MonoBehaviour
    {
        [SerializeField] private LevelDefinition level;
        [SerializeField] private Image[] starIcons;
        [SerializeField] private GameObject[] difficultyIcons;
        [SerializeField] private TextMeshProUGUI nameText;

        [SerializeField] private Button button;
        [SerializeField] private Image targetImage;
        [SerializeField] private Sprite selectedSprite;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Color lockedColor = new Color(0.25f, 0.25f, 0.25f);
        [SerializeField] private Color unlockedColor = Color.white;

        private int earnedStars;
        private int completedDifficultyMask;
        private bool isUnlocked = true;
        private bool isSelected;

        public LevelDefinition Level => level;

        public event Action<LevelButton> Clicked;

        private void Awake()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (targetImage == null)
            {
                targetImage = GetComponent<Image>();
            }

            if (normalSprite == null && targetImage != null)
            {
                normalSprite = targetImage.sprite;
            }

            button.onClick.AddListener(HandleClicked);

            if (level != null)
            {
                SetCompletedDifficultyMask(LevelProgressStore.GetCompletedDifficultyMask(level));
                nameText.text = level.LevelName;
            }

            RefreshLockState();
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(HandleClicked);
        }

        public void SetStars(int stars)
        {
            earnedStars = Mathf.Clamp(stars, 0, starIcons != null ? starIcons.Length : 0);
            RefreshStars();
        }

        public void RefreshStarsForDifficulty(int difficultyIndex)
        {
            int stars = level != null ? LevelProgressStore.GetStarsForDifficulty(level, difficultyIndex) : 0;
            SetStars(stars);
        }

        public void SetCompletedDifficultyMask(int mask)
        {
            completedDifficultyMask = mask;
            RefreshDifficultyIcons();
        }

        public void SetUnlocked(bool unlocked)
        {
            isUnlocked = unlocked;
            RefreshLockState();
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            RefreshSprite();
        }

        private void RefreshStars()
        {
            for (int i = 0; i < starIcons.Length; i++)
            {
                if (starIcons[i] != null)
                {
                    starIcons[i].enabled = i < earnedStars;
                }
            }
        }

        private void RefreshDifficultyIcons()
        {
            for (int i = 0; i < difficultyIcons.Length; i++)
            {
                if (difficultyIcons[i] != null)
                {
                    bool isCompleted = (completedDifficultyMask & (1 << i)) != 0;
                    difficultyIcons[i].SetActive(isCompleted);
                }
            }
        }

        private void RefreshLockState()
        {
            button.interactable = isUnlocked;

            if (targetImage != null)
            {
                targetImage.color = isUnlocked ? unlockedColor : lockedColor;
            }
        }

        private void RefreshSprite()
        {

            targetImage.sprite = isSelected ? selectedSprite : normalSprite;
        }

        private void HandleClicked()
        {
            Clicked?.Invoke(this);
        }
    }
}