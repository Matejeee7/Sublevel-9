using UnityEngine;

// Central one-shot SFX playback point, mirroring the FeedbackUI/
// InteractionPromptUI "one shared instance per scene, static access"
// pattern so any gameplay script can trigger a sound with one line
// without managing its own AudioSource. Respects GameSettings.SfxVolume
// (Master is already applied globally via AudioListener.volume). Safe to
// call with no clip assigned - does nothing, no warning, no error.
public class SfxPlayer : MonoBehaviour
{
    private static SfxPlayer instance;
    private AudioSource source;

    void Awake()
    {
        instance = this;
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f;
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public static void Play(AudioClip clip, float volumeScale = 1f)
    {
        if (instance == null || instance.source == null || clip == null)
            return;

        instance.source.PlayOneShot(clip, volumeScale * GameSettings.SfxVolume);
    }
}
