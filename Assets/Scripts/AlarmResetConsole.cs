using UnityEngine;

public class AlarmResetConsole : Interactable
{
    public string nextObjective = "";

    public override void Interact()
    {
        AlarmController alarmController = FindFirstObjectByType<AlarmController>();

        if (alarmController != null)
        {
            alarmController.ResetAlarm();
            Debug.Log("Alarm reset to NORMAL.");
            FeedbackUI.Flash("Alarm reset.");
            ObjectiveManager.SetObjective(nextObjective);
        }
    }
}
