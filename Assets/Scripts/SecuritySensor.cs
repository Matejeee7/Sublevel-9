using UnityEngine;

public class SecuritySensor : MonoBehaviour
{
    private bool disabled = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (disabled)
            return;

        if (!other.CompareTag("Player"))
            return;

        AlarmController alarmController = FindFirstObjectByType<AlarmController>();

        if (alarmController != null)
        {
            alarmController.RaiseSuspicion(transform.position);
        }
    }

    public void Disable()
    {
        disabled = true;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = Color.green;
    }
}
