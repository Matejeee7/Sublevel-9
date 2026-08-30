using UnityEngine;

// Rooftop-finale-only weapon pickup. Uses the same Interactable/
// PlayerInteraction path as every other pickup in the game (terminals,
// keycards, generators) rather than a separate prompt system.
public class WeaponPickup : Interactable
{
    public PlayerWeapon targetWeapon;
    public string nextObjective = "";
    private bool collected = false;

    public override void Interact()
    {
        if (collected || targetWeapon == null)
            return;

        collected = true;
        targetWeapon.EnableWeapon();

        FeedbackUI.Flash("Weapon acquired.");
        if (!string.IsNullOrEmpty(nextObjective))
            ObjectiveManager.SetObjective(nextObjective);

        gameObject.SetActive(false);
    }
}
