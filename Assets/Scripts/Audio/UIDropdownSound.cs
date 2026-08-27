using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TMP_Dropdown))]
    public class UIDropdownSound : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private AudioClip dropdownClipOverride;
        [SerializeField] private AudioClip hoverClipOverride;

        private TMP_Dropdown dropdown;

        private void Awake()
        {
            dropdown = GetComponent<TMP_Dropdown>();
            dropdown.onValueChanged.AddListener(HandleValueChanged);
        }

        private void OnDestroy()
        {
            if (dropdown != null)
            {
                dropdown.onValueChanged.RemoveListener(HandleValueChanged);
            }
        }

        private void HandleValueChanged(int index)
        {
            if (UIAudioManager.Instance == null)
            {
                return;
            }

            AudioClip clip = dropdownClipOverride != null ? dropdownClipOverride : UIAudioManager.Instance.DefaultDropdownClip;
            UIAudioManager.Instance.PlaySfx(clip);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!dropdown.interactable || UIAudioManager.Instance == null)
            {
                return;
            }

            AudioClip clip = hoverClipOverride != null ? hoverClipOverride : UIAudioManager.Instance.DefaultHoverClip;
            UIAudioManager.Instance.PlaySfx(clip);
        }
    }
}