using UnityEngine;
using UnityEngine.UI;

// Wires slider/toggle UI to GameSettings. Works in both the pause menu
// and the start menu - it only touches Slider/Toggle, not text, so it
// doesn't care whether the surrounding menu uses legacy Text or TMP.
public class SettingsPanelController : MonoBehaviour
{
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle fullscreenToggle;

    void OnEnable()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.SetValueWithoutNotify(GameSettings.MasterVolume);

        if (musicVolumeSlider != null)
            musicVolumeSlider.SetValueWithoutNotify(GameSettings.MusicVolume);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.SetValueWithoutNotify(GameSettings.SfxVolume);

        if (fullscreenToggle != null)
            fullscreenToggle.SetIsOnWithoutNotify(GameSettings.Fullscreen);
    }

    public void OnMasterVolumeChanged(float value)
    {
        GameSettings.MasterVolume = value;
    }

    public void OnMusicVolumeChanged(float value)
    {
        GameSettings.MusicVolume = value;
    }

    public void OnSfxVolumeChanged(float value)
    {
        GameSettings.SfxVolume = value;
    }

    public void OnFullscreenChanged(bool value)
    {
        GameSettings.Fullscreen = value;
    }
}
