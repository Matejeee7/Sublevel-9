using UnityEngine;

// A readable console/log that teaches the player an access code, reusing
// InventoryManager's keycard-id set (a known code is just another kind of
// credential). Re-readable, unlike a one-shot pickup - it's signage, not
// an item.
public class CodeClueTerminal : Interactable
{
    public string codeId = "AICoreOverrideCode";
    private bool learned = false;

    public override void Interact()
    {
        InventoryManager inventory = FindFirstObjectByType<InventoryManager>();

        if (inventory != null)
        {
            inventory.AddKeycard(codeId);
        }

        if (!learned)
        {
            learned = true;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = Color.green;
        }

        Debug.Log("ACCESS CODE LOGGED: " + codeId);
    }
}
