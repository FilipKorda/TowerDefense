using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Scrollbar))]
    public class UIScrollbarSound : MonoBehaviour, IPointerUpHandler
    {
        [SerializeField] private AudioClip scrollbarClipOverride;

        public void OnPointerUp(PointerEventData eventData)
        {
            if (UIAudioManager.Instance == null)
            {
                return;
            }

            AudioClip clip = scrollbarClipOverride != null ? scrollbarClipOverride : UIAudioManager.Instance.DefaultScrollbarClip;
            UIAudioManager.Instance.PlaySfx(clip);
        }
    }
}