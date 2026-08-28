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

    // Lets different door "families" (reactor door, executive door, ...)
    // read as visually distinct without changing the on/off logic itself -
    // defaults match the original hardcoded cyan/gray exactly, so any
    // instance that doesn't set these looks exactly as before.
    public Color poweredColor = Color.cyan;
    public Color unpoweredColor = Color.gray;

    [Header("Audio (optional)")]
    public AudioClip powerOnClip;
    public AudioClip powerOffClip;

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
                SfxPlayer.Play(powerOnClip);
            }
            else
            {
                Debug.Log("Not enough power!");
                FeedbackUI.Flash("Not enough power.");
            }
        }
        else
        {
            isPowered = false;
            powerManager.RemovePowerUsage(powerCost);
            SfxPlayer.Play(powerOffClip);
        }

        UpdateState();
    }

    private void UpdateState()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isPowered ? poweredColor : unpoweredColor;

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
