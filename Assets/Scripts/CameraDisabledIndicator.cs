using UnityEngine;

// Purely visual: watches the parent SecurityCamera's enabled state (set
// false by DisableCamera()) and dims the camera body/lens to clearly show
// it's been taken offline, without touching detection logic at all.
public class CameraDisabledIndicator : MonoBehaviour
{
    public SpriteRenderer body;
    public SpriteRenderer lens;
    public Color activeBodyColor = new Color(0.25f, 0.27f, 0.3f);
    public Color disabledBodyColor = new Color(0.12f, 0.12f, 0.13f);
    public Color activeLensColor = new Color(1f, 0.75f, 0f);
    public Color disabledLensColor = new Color(0.2f, 0.2f, 0.2f);

    private Behaviour cameraScript;

    void Start()
    {
        cameraScript = GetComponentInParent<SecurityCamera>();
    }

    void Update()
    {
        bool active = cameraScript == null || cameraScript.enabled;

        if (body != null)
            body.color = active ? activeBodyColor : disabledBodyColor;

        if (lens != null)
            lens.color = active ? activeLensColor : disabledLensColor;
    }
}
