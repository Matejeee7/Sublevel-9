using TMPro;
using UnityEngine;

// One small, persistent objective line per scene, driven entirely by
// gameplay progression (existing scripts call SetObjective when something
// real happens - a pickup, a console used, an alarm state change). No
// timers, no per-level subclassing - every level just configures which
// existing component says what via a "next objective" string.
public class ObjectiveManager : MonoBehaviour
{
    private static ObjectiveManager instance;

    public TMP_Text objectiveText;
    public string initialObjective = "Reach the elevator.";

    void Awake()
    {
        instance = this;
        SetObjective(initialObjective);
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public static void SetObjective(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        if (instance == null || instance.objectiveText == null)
            return;

        instance.objectiveText.text = text;
    }
}
