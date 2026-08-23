using UnityEngine;

public class PoweredObject : MonoBehaviour
{
    public string objectName = "Powered Object";
    public int powerCost = 1;
    public bool isPowered = false;

    [Header("Door Behavior")]
    public bool disableColliderWhenPowered = true;
    public bool hideWhenPowered = false;

    [Header("Visual Behavior")]
    public bool showOnlyWhenPowered = false;

    private PowerManager powerManager;
    private SpriteRenderer spriteRenderer;
    private Collider2D objectCollider;

    void Start()
    {
        powerManager = FindFirstObjectByType<PowerManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        objectCollider = GetComponent<Collider2D>();

        UpdateState();
    }

    public void TogglePower()
    {
        if (!isPowered)
        {
            if (powerManager.CanUsePower(powerCost))
            {
                isPowered = true;
                powerManager.AddPowerUsage(powerCost);
            }
            else
            {
                Debug.Log("Not enough power!");
            }
        }
        else
        {
            isPowered = false;
            powerManager.RemovePowerUsage(powerCost);
        }

        UpdateState();
    }

    private void UpdateState()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isPowered ? Color.cyan : Color.gray;

            if (hideWhenPowered && isPowered)
            {
                spriteRenderer.enabled = false;
            }
            else if (showOnlyWhenPowered)
            {
                spriteRenderer.enabled = isPowered;
            }
            else
            {
                spriteRenderer.enabled = true;
            }
        }

        if (objectCollider != null && disableColliderWhenPowered)
        {
            objectCollider.enabled = !isPowered;
        }
    }
}