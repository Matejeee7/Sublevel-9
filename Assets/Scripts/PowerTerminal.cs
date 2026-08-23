using UnityEngine;

public class PowerTerminal : Interactable
{
    public PoweredObject targetObject;

    public override void Interact()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("PowerTerminal has no target object assigned.");
            return;
        }

        targetObject.TogglePower();
    }
}