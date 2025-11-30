using UnityEngine;

public class MainMenuAudioManager : MonoBehaviour
{
    public static MainMenuAudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [Header("Menu Music")]
    [SerializeField] private AudioClip menuMusic;

    [Header("Button Sounds")]
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip selectSound;
    [SerializeField] private AudioClip backSound;

    [Header("Volume")]
    [SerializeField] private float musicVolume = 0.7f;
    [SerializeField] private float sfxVolume = 0.8f;

    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeAudio();
        PlayMenuMusic();
    }

    private void InitializeAudio()
    {
        // Load saved volume settings
        float masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.7f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.8f);

        // Apply volumes
        if (musicAudioSource != null)
            musicAudioSource.volume = musicVolume * masterVolume;

        if (sfxAudioSource != null)
            sfxAudioSource.volume = sfxVolume * masterVolume;

        Debug.Log("[MainMenuAudioManager] Audio initialized");
    }

    public void PlayMenuMusic()
    {
        if (musicAudioSource != null && menuMusic != null)
        {
            musicAudioSource.clip = menuMusic;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSound);
    }

    public void PlaySelectSound()
    {
        PlaySFX(selectSound);
    }

    public void PlayBackSound()
    {
        PlaySFX(backSound);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxAudioSource != null && clip != null)
        {
            sfxAudioSource.PlayOneShot(clip);
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        float masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
        
        if (musicAudioSource != null)
            musicAudioSource.volume = musicVolume * masterVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        float masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
        
        if (sfxAudioSource != null)
            sfxAudioSource.volume = sfxVolume * masterVolume;
    }

    public void SetMasterVolume(float volume)
    {
        float masterVolume = Mathf.Clamp01(volume);
        
        if (musicAudioSource != null)
            musicAudioSource.volume = musicVolume * masterVolume;

        if (sfxAudioSource != null)
            sfxAudioSource.volume = sfxVolume * masterVolume;
    }

    public void StopMusic()
    {
        if (musicAudioSource != null)
            musicAudioSource.Stop();
    }

    public void PauseMusic()
    {
        if (musicAudioSource != null)
            musicAudioSource.Pause();
    }

    public void ResumeMusic()
    {
        if (musicAudioSource != null)
            musicAudioSource.Play();
    }
}
