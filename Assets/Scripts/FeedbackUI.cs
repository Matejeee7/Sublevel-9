using System.Collections;
using TMPro;
using UnityEngine;

// Brief on-screen feedback ("Keycard required", "Not enough power", "Alarm
// reset") - deliberately a separate element from the persistent E-interact
// prompt (InteractionPromptUI) and the objective line (ObjectiveManager) so
// none of the three ever overwrite each other.
public class FeedbackUI : MonoBehaviour
{
    private static FeedbackUI instance;

    public TMP_Text feedbackText;
    public float displayDuration = 1.6f;
    public AudioClip cueClip;

    private Coroutine hideRoutine;

    void Awake()
    {
        instance = this;

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public static void Flash(string message)
    {
        if (instance == null)
            return;

        instance.ShowInternal(message);
    }

    private void ShowInternal(string message)
    {
        if (feedbackText == null)
            return;

        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);
        SfxPlayer.Play(cueClip);

        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        hideRoutine = null;
    }
}
