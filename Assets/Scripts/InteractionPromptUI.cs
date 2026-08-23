using TMPro;
using UnityEngine;

// One shared "E - ..." prompt per scene, shown near the bottom of the
// screen. Static access means any interaction script (terminals, hiding
// spots, keycard doors) can show/hide it without a per-object UI
// reference to wire up.
public class InteractionPromptUI : MonoBehaviour
{
    private static InteractionPromptUI instance;

    public TMP_Text promptText;

    void Awake()
    {
        instance = this;

        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public static void Show(string message)
    {
        if (instance == null || instance.promptText == null)
            return;

        instance.promptText.text = message;
        instance.promptText.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        if (instance == null || instance.promptText == null)
            return;

        instance.promptText.gameObject.SetActive(false);
    }
}
