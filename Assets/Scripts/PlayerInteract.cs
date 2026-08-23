using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private Interactable currentInteractable;

    void Update()
    {
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable != null)
        {
            currentInteractable = interactable;
            Debug.Log(interactable.interactionMessage);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable != null && interactable == currentInteractable)
        {
            currentInteractable = null;
            Debug.Log("Left interactable");
        }
    }
}