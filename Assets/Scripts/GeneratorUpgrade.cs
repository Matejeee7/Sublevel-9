using UnityEngine;

public class GeneratorUpgrade : Interactable
{
    public int newMaxPower = 3;
    public string nextObjective = "";
    public AudioClip activateClip;
    private bool activated = false;

    public override void Interact()
    {
        if (activated)
        {
            Debug.Log("Generator already activated.");
            return;
        }

        PowerManager powerManager = FindFirstObjectByType<PowerManager>();

        if (powerManager != null)
        {
            powerManager.maxPower = newMaxPower;
        }

        activated = true;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.green;
        }

        Debug.Log("Generator activated. Max power: " + newMaxPower);
        FeedbackUI.Flash("Generator activated. Max power: " + newMaxPower);
        ObjectiveManager.SetObjective(nextObjective);
        SfxPlayer.Play(activateClip);
    }
}
