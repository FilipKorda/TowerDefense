using System;
using TMPro;
using TowerDefense.GameSystem;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public class DifficultyButton : MonoBehaviour
    {
        [SerializeField] private DifficultyDefinition difficulty;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Button button;
        [SerializeField] private Image targetImage;
        [SerializeField] private Sprite selectedSprite;
        [SerializeField] private Sprite normalSprite;

        public DifficultyDefinition Difficulty => difficulty;

        public event Action<DifficultyButton> Clicked;

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

            if (difficulty != null && nameText != null)
            {
                nameText.text = difficulty.DifficultyName;
            }
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(HandleClicked);
        }

        public void SetSelected(bool selected)
        {
            if (targetImage == null)
            {
                return;
            }

            targetImage.sprite = selected ? selectedSprite : normalSprite;
        }

        private void HandleClicked()
        {
            Clicked?.Invoke(this);
        }
    }
}