using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private readonly HashSet<string> keycards = new HashSet<string>();

    public bool HasKeycard(string keycardId)
    {
        return keycards.Contains(keycardId);
    }

    public void AddKeycard(string keycardId)
    {
        keycards.Add(keycardId);
    }
}
