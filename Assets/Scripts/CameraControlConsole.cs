using UnityEngine;

public class CameraControlConsole : Interactable
{
    public PoweredObject doorToUnlock;
    private bool used = false;

    public override void Interact()
    {
        if (used)
        {
            Debug.Log("Camera network already disabled.");
            return;
        }

        SecurityCamera[] cameras = FindObjectsByType<SecurityCamera>(FindObjectsSortMode.None);
        foreach (SecurityCamera camera in cameras)
            camera.DisableCamera();

        if (doorToUnlock != null && !doorToUnlock.isPowered)
            doorToUnlock.TogglePower();

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.green;

        used = true;
        Debug.Log("SECURITY CAMERAS OFFLINE. Specimen Lab access enabled.");
    }
}
