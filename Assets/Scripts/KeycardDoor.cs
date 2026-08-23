using UnityEngine;

public class KeycardDoor : MonoBehaviour
{
    public string requiredKeycardId = "Level1Keycard";

    private bool unlocked = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !unlocked)
            InteractionPromptUI.Show("E - Unlock Door");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            InteractionPromptUI.Hide();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            InventoryManager inventory =
                collision.gameObject.GetComponent<InventoryManager>();

            if (inventory == null)
                return;

            if (inventory.HasKeycard(requiredKeycardId))
            {
                UnlockDoor();
            }
            else
            {
                Debug.Log("Keycard required.");
                FeedbackUI.Flash("Keycard required.");
            }
        }
    }

    private void UnlockDoor()
    {
        if (unlocked)
            return;

        unlocked = true;

        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
            col.enabled = false;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr != null)
            sr.color = Color.green;

        InteractionPromptUI.Hide();
        FeedbackUI.Flash("Door unlocked.");

        Debug.Log("Security door unlocked.");
    }
}
