using UnityEngine;

public class AlarmController : MonoBehaviour
{
    public enum AlarmState { Normal, Suspicious, Alert }

    public AlarmState currentState = AlarmState.Normal;

    [Header("Automatic Decay")]
    public float suspiciousDecayTime = 8f;
    public float alertDecayTime = 15f;

    [Header("Objective feedback (optional)")]
    public string alertObjective = "Reset the alarm at a security console.";

    [Header("Audio (optional)")]
    public AudioClip alertClip;
    public AudioClip resetClip;

    private float stateTimer;
    private EnemyAI[] enemies;

    void Start()
    {
        enemies = FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
    }

    void Update()
    {
        if (currentState == AlarmState.Normal)
            return;

        stateTimer += Time.deltaTime;

        float decayTime = currentState == AlarmState.Suspicious
            ? suspiciousDecayTime
            : alertDecayTime;

        if (stateTimer >= decayTime)
        {
            SetState(AlarmState.Normal);
        }
    }

    // A minor infraction: a tripped sensor, an incorrect terminal action.
    public void RaiseSuspicion(Vector2 sourcePosition)
    {
        if (currentState == AlarmState.Alert)
            return;

        bool wasNormal = currentState == AlarmState.Normal;
        SetState(AlarmState.Suspicious);

        if (wasNormal)
            FeedbackUI.Flash("Security is suspicious...");
    }

    // The player was directly seen - full alert, security responds.
    public void TriggerAlert(Vector2 sourcePosition)
    {
        bool wasAlert = currentState == AlarmState.Alert;
        SetState(AlarmState.Alert);

        if (!wasAlert)
        {
            FeedbackUI.Flash("ALERT - security responding!");
            ObjectiveManager.SetObjective(alertObjective);
            SfxPlayer.Play(alertClip);
        }

        foreach (EnemyAI enemy in enemies)
        {
            if (enemy != null)
                enemy.Alert(sourcePosition);
        }
    }

    // Reachable through a security console so the player always has
    // a deliberate way back to Normal, on top of the automatic decay above.
    public void ResetAlarm()
    {
        SetState(AlarmState.Normal);
        SfxPlayer.Play(resetClip);
    }

    private void SetState(AlarmState newState)
    {
        currentState = newState;
        stateTimer = 0f;
    }
}
