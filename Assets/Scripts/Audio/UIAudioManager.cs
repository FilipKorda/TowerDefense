using UnityEngine;
using UnityEngine.Audio;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public class UIAudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioMixerGroup sfxMixerGroup;
        [SerializeField] private AudioMixerGroup musicMixerGroup;

        [Header("Default Clips")]
        [SerializeField] private AudioClip defaultHoverClip;
        [SerializeField] private AudioClip defaultClickClip;
        [SerializeField] private AudioClip defaultSliderClip;
        [SerializeField] private AudioClip defaultDropdownClip;
        [SerializeField] private AudioClip defaultToggleClip;
        [SerializeField] private AudioClip defaultScrollbarClip;

        public static UIAudioManager Instance { get; private set; }

        public AudioClip DefaultHoverClip => defaultHoverClip;
        public AudioClip DefaultClickClip => defaultClickClip;
        public AudioClip DefaultSliderClip => defaultSliderClip;
        public AudioClip DefaultDropdownClip => defaultDropdownClip;
        public AudioClip DefaultToggleClip => defaultToggleClip;
        public AudioClip DefaultScrollbarClip => defaultScrollbarClip;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = sfxMixerGroup;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void PlaySfx(AudioClip clip)
        {
            if (clip == null || audioSource == null)
            {
                return;
            }

            audioSource.PlayOneShot(clip);
        }
    }
}