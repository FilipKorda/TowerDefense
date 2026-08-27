using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Toggle))]
    public class UIToggleSound : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private AudioClip toggleClipOverride;
        [SerializeField] private AudioClip hoverClipOverride;

        private Toggle toggle;

        public bool isToggleInDropdown = false;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(HandleValueChanged);
        }

        private void OnDestroy()
        {
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveListener(HandleValueChanged);
            }
        }

        private void HandleValueChanged(bool isOn)
        {
            if (isToggleInDropdown) return;
            if (UIAudioManager.Instance == null)
            {
                return;
            }

            AudioClip clip = toggleClipOverride != null ? toggleClipOverride : UIAudioManager.Instance.DefaultToggleClip;
            UIAudioManager.Instance.PlaySfx(clip);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!toggle.interactable || UIAudioManager.Instance == null)
            {
                return;
            }

            AudioClip clip = hoverClipOverride != null ? hoverClipOverride : UIAudioManager.Instance.DefaultHoverClip;
            UIAudioManager.Instance.PlaySfx(clip);
        }

    }
}