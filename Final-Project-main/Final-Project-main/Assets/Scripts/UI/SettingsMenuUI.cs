using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenuUI : MonoBehaviour
{
    [Header("Settings Controls")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    [Header("UI")]
    [SerializeField] private Button applyButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;

    [Header("Settings Keys")]
    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string FULLSCREEN_KEY = "Fullscreen";

    private Resolution[] availableResolutions;

    private void Start()
    {
        SetupButtonListeners();
    }

    public void Initialize()
    {
        LoadSettings();
    }

    private void SetupButtonListeners()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);

        if (applyButton != null)
            applyButton.onClick.AddListener(ApplySettings);

        if (backButton != null)
            backButton.onClick.AddListener(BackToMenu);

        // Setup resolution dropdown
        SetupResolutionDropdown();
    }

    private void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null) return;

        availableResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        foreach (Resolution resolution in availableResolutions)
        {
            // FIXED: Use refreshRateRatio.value instead of refreshRate
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(
                $"{resolution.width}x{resolution.height}@{resolution.refreshRateRatio.value:F0}Hz"
            ));

            if (resolution.width == Screen.currentResolution.width &&
                resolution.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = resolutionDropdown.options.Count - 1;
            }
        }

        resolutionDropdown.value = currentResolutionIndex;
    }

    private void LoadSettings()
    {
        // Load volume settings
        float masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.8f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.8f);
        bool isFullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;

        if (masterVolumeSlider != null)
            masterVolumeSlider.value = masterVolume;

        if (musicVolumeSlider != null)
            musicVolumeSlider.value = musicVolume;

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = sfxVolume;

        if (fullscreenToggle != null)
            fullscreenToggle.isOn = isFullscreen;

        UpdateVolumeDisplay();
    }

    private void OnMasterVolumeChanged(float value)
    {
        if (masterVolumeText != null)
            masterVolumeText.text = $"Master: {(value * 100):F0}%";
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (musicVolumeText != null)
            musicVolumeText.text = $"Music: {(value * 100):F0}%";
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (sfxVolumeText != null)
            sfxVolumeText.text = $"SFX: {(value * 100):F0}%";
    }

    private void OnFullscreenToggled(bool isFullscreen)
    {
        Debug.Log($"Fullscreen toggled: {isFullscreen}");
    }

    private void UpdateVolumeDisplay()
    {
        if (masterVolumeSlider != null && masterVolumeText != null)
            masterVolumeText.text = $"Master: {(masterVolumeSlider.value * 100):F0}%";

        if (musicVolumeSlider != null && musicVolumeText != null)
            musicVolumeText.text = $"Music: {(musicVolumeSlider.value * 100):F0}%";

        if (sfxVolumeSlider != null && sfxVolumeText != null)
            sfxVolumeText.text = $"SFX: {(sfxVolumeSlider.value * 100):F0}%";
    }

    private void ApplySettings()
    {
        // Save volume settings
        if (masterVolumeSlider != null)
            PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, masterVolumeSlider.value);

        if (musicVolumeSlider != null)
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolumeSlider.value);

        if (sfxVolumeSlider != null)
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolumeSlider.value);

        // Save fullscreen setting
        if (fullscreenToggle != null)
            PlayerPrefs.SetInt(FULLSCREEN_KEY, fullscreenToggle.isOn ? 1 : 0);

        // Apply resolution
        if (resolutionDropdown != null && availableResolutions.Length > 0)
        {
            Resolution selectedResolution = availableResolutions[resolutionDropdown.value];
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, 
                fullscreenToggle != null ? fullscreenToggle.isOn : true);
        }

        // Apply fullscreen
        if (fullscreenToggle != null)
            Screen.fullScreen = fullscreenToggle.isOn;

        PlayerPrefs.Save();
        Debug.Log("[SettingsMenuUI] Settings applied and saved");
    }

    private void BackToMenu()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenToggled);

        if (applyButton != null)
            applyButton.onClick.RemoveListener(ApplySettings);

        if (backButton != null)
            backButton.onClick.RemoveListener(BackToMenu);
    }
}
