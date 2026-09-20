using UnityEngine;

// Simple PlayerPrefs-backed settings, applied immediately on change.
// Master volume drives AudioListener.volume. Music and SFX volume are
// multipliers read by MusicManager (music) and ButtonHoverSound (the UI
// sound effect). Changed fires whenever any value changes so audio
// sources that are already playing can update at once.
public static class GameSettings
{
    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";
    private const string FullscreenKey = "Fullscreen";

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
        get { return PlayerPrefs.GetFloat(MusicVolumeKey, 1f); }
        set
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, value);
            Changed?.Invoke();
        }
    }

    public static float SfxVolume
    {
        get { return PlayerPrefs.GetFloat(SfxVolumeKey, 1f); }
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
