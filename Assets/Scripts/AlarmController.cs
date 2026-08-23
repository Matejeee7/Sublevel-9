using UnityEngine;

public class AlarmController : MonoBehaviour
{
    public enum AlarmState { Normal, Suspicious, Alert }

    public AlarmState currentState = AlarmState.Normal;

    [Header("Automatic Decay")]
    public float suspiciousDecayTime = 8f;
    public float alertDecayTime = 15f;

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

        SetState(AlarmState.Suspicious);
    }

    // The player was directly seen - full alert, security responds.
    public void TriggerAlert(Vector2 sourcePosition)
    {
        SetState(AlarmState.Alert);

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
    }

    private void SetState(AlarmState newState)
    {
        currentState = newState;
        stateTimer = 0f;
    }
}
