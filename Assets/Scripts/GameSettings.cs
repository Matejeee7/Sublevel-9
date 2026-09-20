using UnityEngine;

// Simple PlayerPrefs-backed settings, applied immediately on change.
// All three volumes are plain 0..1 linear values (0 = silent, 1 = full).
// Master drives AudioListener.volume, Music is the music AudioSource volume
// (MusicManager) and SFX is the volume of the UI hover sound (ButtonHoverSound).
// Changed fires whenever any value changes so audio that is already playing
// can update at once.
public static class GameSettings
{
    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";
    private const string FullscreenKey = "Fullscreen";

    // Used until the player saves a value of their own.
    public const float DefaultMusicVolume = 0.8f;
    public const float DefaultSfxVolume = 0.8f;

    public static event System.Action Changed;

    public static float MasterVolume
    {
        get { return PlayerPrefs.GetFloat(MasterVolumeKey, 1f); }
        set
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, value);
            AudioListener.volume = value;
            Changed?.Invoke();
        }
    }

    public static float MusicVolume
    {
        get { return PlayerPrefs.GetFloat(MusicVolumeKey, DefaultMusicVolume); }
        set
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, value);
            Changed?.Invoke();
        }
    }

    public static float SfxVolume
    {
        get { return PlayerPrefs.GetFloat(SfxVolumeKey, DefaultSfxVolume); }
        set
        {
            PlayerPrefs.SetFloat(SfxVolumeKey, value);
            Changed?.Invoke();
        }
    }

    public static bool Fullscreen
    {
        get { return PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1; }
        set
        {
            PlayerPrefs.SetInt(FullscreenKey, value ? 1 : 0);
            Screen.fullScreen = value;
            Changed?.Invoke();
        }
    }

    public static void Apply()
    {
        AudioListener.volume = MasterVolume;
        Screen.fullScreen = Fullscreen;
    }

    // Load the saved values when the game starts, whichever scene it starts
    // from, so a saved master volume / fullscreen choice is applied without
    // the player having to open the settings panel first.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ApplyOnStartup()
    {
        Apply();
    }
}
