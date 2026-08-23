using UnityEngine;

public class AlarmEmergencyLighting : MonoBehaviour
{
    public SpriteRenderer overlay;
    public Color normalColor = new Color(1f, 0f, 0f, 0f);
    public Color alertColor = new Color(1f, 0f, 0f, 0.25f);

    private AlarmController alarmController;

    void Start()
    {
        alarmController = FindFirstObjectByType<AlarmController>();
    }

    void Update()
    {
        if (alarmController == null || overlay == null)
            return;

        overlay.color = alarmController.currentState == AlarmController.AlarmState.Alert
            ? alertColor
            : normalColor;
    }
}
