using UnityEngine;

// Wraps a PoweredObject that can't stay on indefinitely: once powered,
// it automatically shuts itself off (and frees its power budget) after
// activeDuration, flashing a warning as the window closes. Designed for
// a "activate it, then get through before it drops" crossing - the
// window should always be generous enough to plan around, not react to.
public class UnstableCircuit : MonoBehaviour
{
    public PoweredObject target;
    public float activeDuration = 8f;
    public float warningTime = 3f;

    private float timer;
    private bool wasPowered;

    void Update()
    {
        if (target == null)
            return;

        if (target.isPowered)
        {
            if (!wasPowered)
                timer = 0f;

            timer += Time.deltaTime;

            if (timer >= activeDuration)
            {
                target.TogglePower();
            }
            else if (timer >= activeDuration - warningTime)
            {
                Flicker();
            }
        }

        wasPowered = target.isPowered;
    }

    private void Flicker()
    {
        SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
        if (sr == null)
            return;

        bool blink = Mathf.PingPong(Time.time * 6f, 1f) > 0.5f;
        sr.color = blink ? Color.red : Color.yellow;
    }
}
