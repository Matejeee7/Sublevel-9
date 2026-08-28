using UnityEngine;

// Stays active while a crate (or the player) is standing on it. Used to
// hold a door open - push a crate on top and it keeps holding even after
// the player walks away, unlike a one-shot switch.
public class PressurePlate : MonoBehaviour
{
    public GameObject doorToUnlock;

    private int weightCount = 0;
    private bool wasActive = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsValidWeight(other))
        {
            weightCount++;
            UpdateDoor();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsValidWeight(other))
        {
            weightCount = Mathf.Max(0, weightCount - 1);
            UpdateDoor();
        }
    }

    private bool IsValidWeight(Collider2D other)
    {
        return other.CompareTag("Player") || other.GetComponent<PushableCrate>() != null;
    }

    private void UpdateDoor()
    {
        if (doorToUnlock == null)
            return;

        bool active = weightCount > 0;

        Collider2D col = doorToUnlock.GetComponent<Collider2D>();
        if (col != null)
            col.enabled = !active;

        SpriteRenderer sr = doorToUnlock.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = active ? Color.green : new Color(0.75f, 0.2f, 0.2f);

        if (active != wasActive)
        {
            FeedbackUI.Flash(active ? "Pressure plate activated." : "Pressure plate released.");
            wasActive = active;
        }
    }
}
