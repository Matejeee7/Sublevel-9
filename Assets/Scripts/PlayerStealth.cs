using UnityEngine;

public class PlayerStealth : MonoBehaviour
{
    public bool isHidden = false;

    private PlayerMovement movement;
    private SpriteRenderer[] spriteRenderers;
    private Rigidbody2D rb;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        // Includes children so a visual built as a "Visual" child object
        // (e.g. Level1's real character sprite) still fully disappears
        // while hidden, not just whichever renderer happens to live
        // directly on this GameObject.
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        rb = GetComponent<Rigidbody2D>();
    }

    public void Hide(Transform hidingPosition)
    {
        isHidden = true;

        transform.position = hidingPosition.position;

        if (movement != null)
            movement.enabled = false;

        SetSpritesEnabled(false);

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

        SetSpritesEnabled(true);

        Debug.Log("Player exited locker");
    }

    private void SetSpritesEnabled(bool value)
    {
        if (spriteRenderers == null)
            return;

        foreach (SpriteRenderer sr in spriteRenderers)
        {
            if (sr != null)
                sr.enabled = value;
        }
    }
}
