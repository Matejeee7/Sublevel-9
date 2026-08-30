using UnityEngine;
using UnityEngine.UI;

// Rooftop-finale-only aiming reticle. Reads PlayerWeapon.CurrentAimPoint -
// the exact same world point Fire() uses - so the crosshair never shows a
// spot a shot couldn't actually reach. Hidden until the weapon is picked up,
// and hidden again whenever PlayerWeapon.CanAim goes false (pause, terminal
// UI, or the extraction sequence), handing the system cursor back each time.
public class CombatCrosshair : MonoBehaviour
{
    public RectTransform crosshairRect;
    public Image crosshairImage;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        SetVisible(false);
    }

    void Update()
    {
        PlayerWeapon weapon = PlayerWeapon.Instance;
        bool shouldShow = weapon != null && weapon.CanAim && cam != null;

        SetVisible(shouldShow);
        if (!shouldShow)
            return;

        crosshairRect.position = cam.WorldToScreenPoint(weapon.CurrentAimPoint);
    }

    private void SetVisible(bool visible)
    {
        if (crosshairImage != null)
            crosshairImage.enabled = visible;
        Cursor.visible = !visible;
    }

    void OnDisable()
    {
        Cursor.visible = true;
    }
}
