using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public class GameSettingsController : MonoBehaviour
    {
        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private string musicVolumeParam = "MusicVolume";
        [SerializeField] private string sfxVolumeParam = "SfxVolume";

        [Header("Music")]
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Slider musicVolumeSlider;

        [Header("SFX")]
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Slider sfxVolumeSlider;

        [Header("Quality")]
        [SerializeField] private Button lowQualityButton;
        [SerializeField] private Button mediumQualityButton;
        [SerializeField] private Button highQualityButton;
        [SerializeField] private Color selectedQualityColor = new Color(0.3f, 0.6f, 1f);
        [SerializeField] private Color normalQualityColor = Color.white;

        [Header("Display")]
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullscreenToggle;

        private const string MusicEnabledKey = "Settings_MusicEnabled";
        private const string MusicVolumeKey = "Settings_MusicVolume";
        private const string SfxEnabledKey = "Settings_SfxEnabled";
        private const string SfxVolumeKey = "Settings_SfxVolume";
        private const string QualityLevelKey = "Settings_QualityLevel";
        private const string ResolutionIndexKey = "Settings_ResolutionIndex";
        private const string FullscreenKey = "Settings_Fullscreen";

        private const float MutedVolumeDb = -80f;

        private static readonly Vector2Int[] SupportedResolutions =
        {
            new Vector2Int(1280, 720),
            new Vector2Int(1366, 768),
            new Vector2Int(1600, 900),
            new Vector2Int(1920, 1080),
            new Vector2Int(2048, 1152),
            new Vector2Int(2560, 1440),
            new Vector2Int(2560, 1600),
            new Vector2Int(3200, 1800),
            new Vector2Int(3440, 1440),
            new Vector2Int(3840, 2160)
        };

        private void Awake()
        {
            musicToggle.onValueChanged.AddListener(HandleMusicToggleChanged);
            musicVolumeSlider.onValueChanged.AddListener(HandleMusicVolumeChanged);
            sfxToggle.onValueChanged.AddListener(HandleSfxToggleChanged);
            sfxVolumeSlider.onValueChanged.AddListener(HandleSfxVolumeChanged);
            lowQualityButton.onClick.AddListener(() => SetQualityLevel(0));
            mediumQualityButton.onClick.AddListener(() => SetQualityLevel(1));
            highQualityButton.onClick.AddListener(() => SetQualityLevel(2));

            SetupResolutionDropdown();

            resolutionDropdown.onValueChanged.AddListener(HandleResolutionChanged);
            fullscreenToggle.onValueChanged.AddListener(HandleFullscreenChanged);

            LoadSettings();
        }

        private void OnDestroy()
        {
            musicToggle.onValueChanged.RemoveListener(HandleMusicToggleChanged);
            musicVolumeSlider.onValueChanged.RemoveListener(HandleMusicVolumeChanged);
            sfxToggle.onValueChanged.RemoveListener(HandleSfxToggleChanged);
            sfxVolumeSlider.onValueChanged.RemoveListener(HandleSfxVolumeChanged);
            resolutionDropdown.onValueChanged.RemoveListener(HandleResolutionChanged);
            fullscreenToggle.onValueChanged.RemoveListener(HandleFullscreenChanged);
        }

        private void SetupResolutionDropdown()
        {
            List<string> options = new List<string>();

            foreach (Vector2Int resolution in SupportedResolutions)
            {
                options.Add($"{resolution.x} x {resolution.y}");
            }

            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(options);
        }

        private void LoadSettings()
        {
            bool musicEnabled = PlayerPrefs.GetInt(MusicEnabledKey, 1) == 1;
            float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.75f);
            bool sfxEnabled = PlayerPrefs.GetInt(SfxEnabledKey, 1) == 1;
            float sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 0.75f);
            int qualityLevel = PlayerPrefs.GetInt(QualityLevelKey, 1);
            int resolutionIndex = PlayerPrefs.GetInt(ResolutionIndexKey, GetClosestCurrentResolutionIndex());
            bool fullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;

            musicToggle.SetIsOnWithoutNotify(musicEnabled);
            musicVolumeSlider.SetValueWithoutNotify(musicVolume);
            sfxToggle.SetIsOnWithoutNotify(sfxEnabled);
            sfxVolumeSlider.SetValueWithoutNotify(sfxVolume);
            resolutionDropdown.SetValueWithoutNotify(resolutionIndex);
            fullscreenToggle.SetIsOnWithoutNotify(fullscreen);

            ApplyMusicState(musicEnabled, musicVolume);
            ApplySfxState(sfxEnabled, sfxVolume);
            ApplyQualityLevel(qualityLevel);
            RefreshQualityButtonsVisual(qualityLevel);
            ApplyResolution(resolutionIndex, fullscreen);
        }

        private static int GetClosestCurrentResolutionIndex()
        {
            int closestIndex = 0;
            int closestDistance = int.MaxValue;

            for (int i = 0; i < SupportedResolutions.Length; i++)
            {
                int distance = Mathf.Abs(SupportedResolutions[i].x - Screen.width) + Mathf.Abs(SupportedResolutions[i].y - Screen.height);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }

        private void HandleMusicToggleChanged(bool enabled)
        {
            PlayerPrefs.SetInt(MusicEnabledKey, enabled ? 1 : 0);
            PlayerPrefs.Save();

            float volume = musicVolumeSlider != null ? musicVolumeSlider.value : 0.75f;
            ApplyMusicState(enabled, volume);
        }

        private void HandleMusicVolumeChanged(float volume)
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, volume);
            PlayerPrefs.Save();

            bool enabled = musicToggle == null || musicToggle.isOn;
            ApplyMusicState(enabled, volume);
        }

        private void HandleSfxToggleChanged(bool enabled)
        {
            PlayerPrefs.SetInt(SfxEnabledKey, enabled ? 1 : 0);
            PlayerPrefs.Save();

            float volume = sfxVolumeSlider != null ? sfxVolumeSlider.value : 0.75f;
            ApplySfxState(enabled, volume);
        }

        private void HandleSfxVolumeChanged(float volume)
        {
            PlayerPrefs.SetFloat(SfxVolumeKey, volume);
            PlayerPrefs.Save();

            bool enabled = sfxToggle == null || sfxToggle.isOn;
            ApplySfxState(enabled, volume);
        }

        private void ApplyMusicState(bool enabled, float volume)
        {
            float decibels = enabled ? LinearToDecibel(volume) : MutedVolumeDb;
            audioMixer.SetFloat(musicVolumeParam, decibels);
        }

        private void ApplySfxState(bool enabled, float volume)
        {
            float decibels = enabled ? LinearToDecibel(volume) : MutedVolumeDb;

            audioMixer.SetFloat(sfxVolumeParam, decibels);
        }

        private static float LinearToDecibel(float linear)
        {
            if (linear <= 0.0001f)
            {
                return MutedVolumeDb;
            }

            return Mathf.Log10(linear) * 20f;
        }

        private void SetQualityLevel(int qualityLevel)
        {
            PlayerPrefs.SetInt(QualityLevelKey, qualityLevel);
            PlayerPrefs.Save();

            ApplyQualityLevel(qualityLevel);
            RefreshQualityButtonsVisual(qualityLevel);
        }

        private static void ApplyQualityLevel(int qualityLevel)
        {
            QualitySettings.SetQualityLevel(qualityLevel, true);
        }

        private void RefreshQualityButtonsVisual(int qualityLevel)
        {
            SetButtonColor(lowQualityButton, qualityLevel == 0);
            SetButtonColor(mediumQualityButton, qualityLevel == 1);
            SetButtonColor(highQualityButton, qualityLevel == 2);
        }

        private void SetButtonColor(Button button, bool selected)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = selected ? selectedQualityColor : normalQualityColor;
            button.colors = colors;
        }

        private void HandleResolutionChanged(int index)
        {
            PlayerPrefs.SetInt(ResolutionIndexKey, index);
            PlayerPrefs.Save();

            bool fullscreen = fullscreenToggle == null || fullscreenToggle.isOn;
            ApplyResolution(index, fullscreen);
        }

        private void HandleFullscreenChanged(bool fullscreen)
        {
            PlayerPrefs.SetInt(FullscreenKey, fullscreen ? 1 : 0);
            PlayerPrefs.Save();

            int index = resolutionDropdown != null ? resolutionDropdown.value : GetClosestCurrentResolutionIndex();
            ApplyResolution(index, fullscreen);
        }

        private static void ApplyResolution(int index, bool fullscreen)
        {
            if (index < 0 || index >= SupportedResolutions.Length)
            {
                return;
            }

            Vector2Int resolution = SupportedResolutions[index];
            Screen.SetResolution(resolution.x, resolution.y, fullscreen);
        }
    }
}