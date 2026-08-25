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

    // True whenever another modal UI owns player input (a power terminal
    // panel, the pause menu). The prompt must never render on top of those,
    // so every Show() call is gated on this and Update() force-hides the
    // prompt the instant capture begins, regardless of which script last
    // called Show() and forgot to Hide() it.
    public static bool InputCaptured =>
        PauseMenuController.IsPaused || MultiPowerTerminal.IsAnyTerminalOpen;

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

    void Update()
    {
        if (promptText != null && promptText.gameObject.activeSelf && InputCaptured)
            promptText.gameObject.SetActive(false);
    }

    public static void Show(string message)
    {
        if (instance == null || instance.promptText == null)
            return;

        if (InputCaptured)
        {
            Hide();
            return;
        }

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
