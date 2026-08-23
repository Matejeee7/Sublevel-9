using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    public Transform hidePoint;
    public Transform exitPoint;

    private bool playerInRange = false;
    private PlayerStealth playerStealth;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (playerStealth == null)
                return;

            if (playerStealth.isHidden)
            {
                playerStealth.Unhide(exitPoint);
                InteractionPromptUI.Show("E - Hide");
            }
            else
            {
                playerStealth.Hide(hidePoint);
                InteractionPromptUI.Show("E - Exit Locker");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerStealth = other.GetComponent<PlayerStealth>();

            InteractionPromptUI.Show("E - Hide");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playerStealth != null && !playerStealth.isHidden)
        {
            playerInRange = false;
            playerStealth = null;
            InteractionPromptUI.Hide();
        }
    }
}
