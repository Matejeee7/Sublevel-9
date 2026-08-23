using UnityEngine;

public class AlarmResetConsole : Interactable
{
    public override void Interact()
    {
        AlarmController alarmController = FindFirstObjectByType<AlarmController>();

        if (alarmController != null)
        {
            alarmController.ResetAlarm();
            Debug.Log("Alarm reset to NORMAL.");
        }
    }
}
