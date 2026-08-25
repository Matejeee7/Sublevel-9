using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    public Transform hidePoint;
    public Transform exitPoint;

    private bool playerInRange = false;
    private PlayerStealth playerStealth;

    void Update()
    {
        if (!playerInRange || playerStealth == null)
            return;

        if (InteractionPromptUI.InputCaptured)
            return;

        // Re-asserted every frame (not just on trigger enter) so the prompt
        // self-heals as soon as a terminal/pause menu stops capturing input.
        InteractionPromptUI.Show(playerStealth.isHidden ? "E - Exit Locker" : "E - Hide");

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerStealth.isHidden)
                playerStealth.Unhide(exitPoint);
            else
                playerStealth.Hide(hidePoint);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerStealth = other.GetComponent<PlayerStealth>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playerStealth != null && !playerStealth.isHidden)
        {
            playerInRange = false;
            playerStealth = null;

            if (!InteractionPromptUI.InputCaptured)
                InteractionPromptUI.Hide();
        }
    }
}
