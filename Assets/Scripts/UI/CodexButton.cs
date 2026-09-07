using System;
using TowerDefense.EnemySystem;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public class CodexButton : MonoBehaviour
    {
        [SerializeField] private EnemyDefinition enemy;
        [SerializeField] private Image mainImage;
        [SerializeField] private Image secondaryImage;
        [SerializeField] private Image lockOverlayImage;
        [SerializeField] private Button button;

        [SerializeField] private Color unlockedColor = Color.white;
        [SerializeField] private Color lockedColor = Color.gray;

        public bool isUnlocked;

        public EnemyDefinition Enemy => enemy;

        public event Action<CodexButton> Clicked;

        private void Awake()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            button.onClick.AddListener(HandleClicked);

            SetUnlocked(enemy != null && GameSystem.CodexProgressStore.IsEnemyUnlocked(enemy));
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(HandleClicked);
        }

        public void SetUnlocked(bool unlocked)
        {
            isUnlocked = unlocked;

            Color targetColor = unlocked ? unlockedColor : lockedColor;

            if (mainImage != null)
            {
                mainImage.color = targetColor;
            }

            if (secondaryImage != null)
            {
                secondaryImage.color = targetColor;
            }

            if (lockOverlayImage != null)
            {
                lockOverlayImage.enabled = !unlocked;
            }

            if (button != null)
            {
                button.interactable = unlocked;
            }
        }

        private void HandleClicked()
        {
            if (enemy != null)
            {
                GameSystem.CodexProgressStore.MarkEnemySeen(enemy);
            }

            Clicked?.Invoke(this);
        }
    }
}