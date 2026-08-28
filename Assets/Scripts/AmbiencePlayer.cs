using UnityEngine;

// One looping ambience/music bed per scene (facility hum, tenser
// industrial ambience, wind, ...). Continuously re-applies
// GameSettings.MusicVolume so a mid-level settings change takes effect
// immediately, matching MusicVolumeSlider's live-update behavior in
// StartScene/PauseMenu. Left with no clip assigned until real audio is
// sourced - a disabled AudioSource with nothing to play, not an error.
public class AmbiencePlayer : MonoBehaviour
{
    public AudioClip ambienceClip;

    [Range(0f, 1f)]
    public float baseVolume = 0.5f;

    private AudioSource source;

    void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;
        source.clip = ambienceClip;
    }

    void Start()
    {
        if (ambienceClip != null)
            source.Play();
    }

    void Update()
    {
        if (source != null)
            source.volume = baseVolume * GameSettings.MusicVolume;
    }
}
