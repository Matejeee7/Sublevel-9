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
            }
            else
            {
                playerStealth.Hide(hidePoint);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerStealth = other.GetComponent<PlayerStealth>();

            Debug.Log("Press E to enter locker");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playerStealth != null && !playerStealth.isHidden)
        {
            playerInRange = false;
            playerStealth = null;
        }
    }
}