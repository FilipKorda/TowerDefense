using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Slider))]
    public class UISliderSound : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler
    {
        [SerializeField] private AudioClip sliderClipOverride;
        [SerializeField] private AudioClip hoverClipOverride;

        public void OnPointerUp(PointerEventData eventData)
        {
            if (UIAudioManager.Instance == null)
            {
                return;
            }

            AudioClip clip = sliderClipOverride != null ? sliderClipOverride : UIAudioManager.Instance.DefaultSliderClip;
            UIAudioManager.Instance.PlaySfx(clip);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (UIAudioManager.Instance == null)
            {
                return;
            }

            AudioClip clip = hoverClipOverride != null ? hoverClipOverride : UIAudioManager.Instance.DefaultHoverClip;
            UIAudioManager.Instance.PlaySfx(clip);
        }
    }
}