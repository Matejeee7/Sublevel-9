using UnityEngine;

// Simple PlayerPrefs-backed settings, applied immediately on change.
// Music/SFX volume are stored for a future AudioManager to read - no
// audio assets exist in the project yet, so there is nothing to gate
// with them today, but the values persist correctly regardless.
public static class GameSettings
{
    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";
    private const string FullscreenKey = "Fullscreen";

    public static float MasterVolume
    {
        get { return PlayerPrefs.GetFloat(MasterVolumeKey, 1f); }
        set
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, value);
            AudioListener.volume = value;
        }
    }

    public static float MusicVolume
    {
        get { return PlayerPrefs.GetFloat(MusicVolumeKey, 1f); }
        set { PlayerPrefs.SetFloat(MusicVolumeKey, value); }
    }

    public static float SfxVolume
    {
        get { return PlayerPrefs.GetFloat(SfxVolumeKey, 1f); }
        set { PlayerPrefs.SetFloat(SfxVolumeKey, value); }
    }

    public static bool Fullscreen
    {
        get { return PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1; }
        set
        {
            PlayerPrefs.SetInt(FullscreenKey, value ? 1 : 0);
            Screen.fullScreen = value;
        }
    }

    public static void Apply()
    {
        AudioListener.volume = MasterVolume;
        Screen.fullScreen = Fullscreen;
    }
}
