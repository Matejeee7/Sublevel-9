using UnityEngine;

public class PlayerStealth : MonoBehaviour
{
    public bool isHidden = false;

    private PlayerMovement movement;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Hide(Transform hidingPosition)
    {
        isHidden = true;

        transform.position = hidingPosition.position;

        if (movement != null)
            movement.enabled = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        Debug.Log("Player entered locker");
    }

    public void Unhide(Transform exitPosition)
    {
        isHidden = false;

        transform.position = exitPosition.position;

        if (movement != null)
            movement.enabled = true;

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        Debug.Log("Player exited locker");
    }
}