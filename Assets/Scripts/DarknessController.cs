using UnityEngine;

public class DarknessController : MonoBehaviour
{
    public SpriteRenderer darknessOverlay;
    public PoweredObject linkedPowerObject;

    [Range(0f, 1f)]
    public float darknessAlphaWhenOff = 0.8f;

    [Range(0f, 1f)]
    public float darknessAlphaWhenOn = 0.15f;

    void Update()
    {
        if (darknessOverlay == null || linkedPowerObject == null)
            return;

        Color color = darknessOverlay.color;

        if (linkedPowerObject.isPowered)
        {
            color.a = darknessAlphaWhenOn;
        }
        else
        {
            color.a = darknessAlphaWhenOff;
        }

        darknessOverlay.color = color;
    }
}