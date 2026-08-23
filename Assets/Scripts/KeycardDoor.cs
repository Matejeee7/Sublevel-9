using UnityEngine;

public class KeycardDoor : MonoBehaviour
{
    private bool unlocked = false;

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

            if (inventory.hasLevel1Keycard)
            {
                UnlockDoor();
            }
            else
            {
                Debug.Log("Keycard required.");
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

        Debug.Log("Security door unlocked.");
    }
}