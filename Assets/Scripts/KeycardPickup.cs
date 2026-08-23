using UnityEngine;

public class KeycardPickup : MonoBehaviour
{
    public string keycardId = "Level1Keycard";
    public string nextObjective = "";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        InventoryManager inventory =
            other.GetComponent<InventoryManager>();

        if (inventory == null)
            return;

        inventory.AddKeycard(keycardId);

        Debug.Log(keycardId + " acquired");
        FeedbackUI.Flash(keycardId + " acquired");
        ObjectiveManager.SetObjective(nextObjective);

        Destroy(gameObject);
    }
}
