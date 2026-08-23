using UnityEngine;

// A laser gate with inverted PoweredObject semantics: it blocks the path
// while its power source IS powered (the laser is firing), and is safe to
// cross once that power is routed away - "turn it off to get through",
// controlled through the same terminal/power system as everything else.
public class LaserBarrier : MonoBehaviour
{
    public PoweredObject powerSource;

    private SpriteRenderer sr;
    private Collider2D col;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (powerSource == null)
            return;

        bool active = powerSource.isPowered;

        if (col != null)
            col.enabled = active;

        if (sr != null)
            sr.color = active ? new Color(1f, 0.1f, 0.1f, 0.85f) : new Color(0.3f, 0.3f, 0.3f, 0.15f);
    }
}
