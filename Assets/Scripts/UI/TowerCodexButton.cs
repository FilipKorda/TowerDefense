using System;
using TowerDefense.BuildSystem;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public class TowerCodexButton : MonoBehaviour
    {
        [SerializeField] private TowerDefinition tower;
        [SerializeField] private Image mainImage;
        [SerializeField] private Image secondaryImage;
        [SerializeField] private Image lockOverlayImage;
        [SerializeField] private Button button;

        [SerializeField] private Color unlockedColor = Color.white;
        [SerializeField] private Color lockedColor = Color.gray;

        public bool isUnlocked;

        public TowerDefinition Tower => tower;

        public event Action<TowerCodexButton> Clicked;

        private void Awake()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            button.onClick.AddListener(HandleClicked);

            SetUnlocked(tower != null && GameSystem.CodexProgressStore.IsTowerUnlocked(tower));
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
            if (tower != null)
            {
                GameSystem.CodexProgressStore.MarkTowerSeen(tower);
            }

            Clicked?.Invoke(this);
        }
    }
}