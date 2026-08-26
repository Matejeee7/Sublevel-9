using UnityEngine;

// A simple alpha pulse for a warning-light sprite (SpriteRenderer only, no
// dynamic lighting dependency) - used for the rooftop's emergency lights.
public class PulsingLight : MonoBehaviour
{
    public SpriteRenderer target;
    public float pulseSpeed = 2f;

    [Range(0f, 1f)]
    public float minAlpha = 0.2f;

    [Range(0f, 1f)]
    public float maxAlpha = 0.9f;

    void Update()
    {
        if (target == null)
            return;

        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        Color c = target.color;
        c.a = Mathf.Lerp(minAlpha, maxAlpha, t);
        target.color = c;
    }
}
