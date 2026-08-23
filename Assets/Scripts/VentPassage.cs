using UnityEngine;

// A crawl-through shortcut between two points. Blocked while the linked
// fan is still running (fanControl not powered) - power it down at a
// vent control terminal to make the passage safe to use.
public class VentPassage : MonoBehaviour
{
    public Transform linkedExit;
    public PoweredObject fanControl;

    private bool playerInRange = false;
    private Transform playerTransform;

    void Update()
    {
        if (!playerInRange || playerTransform == null)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (fanControl != null && !fanControl.isPowered)
            {
                Debug.Log("The ventilation fan is still running. Shut it down first.");
                return;
            }

            if (linkedExit != null)
            {
                playerTransform.position = linkedExit.position;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;
        playerTransform = other.transform;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerTransform = null;
        }
    }
}
