using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public class UIButtonSound : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private AudioClip hoverClipOverride;
        [SerializeField] private AudioClip clickClipOverride;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(HandleClick);
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!button.interactable || UIAudioManager.Instance == null)
            {
                return;
            }

            AudioClip clip = hoverClipOverride != null ? hoverClipOverride : UIAudioManager.Instance.DefaultHoverClip;
            UIAudioManager.Instance.PlaySfx(clip);
        }

        private void HandleClick()
        {
            if (UIAudioManager.Instance == null)
            {
                return;
            }

            AudioClip clip = clickClipOverride != null ? clickClipOverride : UIAudioManager.Instance.DefaultClickClip;
            UIAudioManager.Instance.PlaySfx(clip);
        }
    }
}