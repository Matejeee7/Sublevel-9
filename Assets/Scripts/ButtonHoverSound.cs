using UnityEngine;
using UnityEngine.EventSystems;

// Plays a short blip the moment the pointer enters a menu button.
// PointerEnter fires once per hover (not every frame the pointer stays
// over the button), so this can't spam the sound while the mouse sits
// still on a button. Loudness follows the SFX volume setting.
[RequireComponent(typeof(AudioSource))]
public class ButtonHoverSound : MonoBehaviour, IPointerEnterHandler
{
    public AudioClip hoverClip;

    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverClip != null)
            source.PlayOneShot(hoverClip, GameSettings.SfxVolume);
    }
}
