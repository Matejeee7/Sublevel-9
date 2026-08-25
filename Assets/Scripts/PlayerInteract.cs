using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private Interactable currentInteractable;

    void Update()
    {
        if (currentInteractable == null)
            return;

        if (InteractionPromptUI.InputCaptured)
            return;

        InteractionPromptUI.Show(currentInteractable.interactionMessage);

        if (Input.GetKeyDown(KeyCode.E))
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
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable != null && interactable == currentInteractable)
        {
            currentInteractable = null;

            if (!InteractionPromptUI.InputCaptured)
                InteractionPromptUI.Hide();
        }
    }
}
