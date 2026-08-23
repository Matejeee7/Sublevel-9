using UnityEngine;

public class KeycardPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        InventoryManager inventory =
            other.GetComponent<InventoryManager>();

        if (inventory == null)
            return;

        inventory.hasLevel1Keycard = true;

        Debug.Log("Level 1 Keycard acquired");

        Destroy(gameObject);
    }
}