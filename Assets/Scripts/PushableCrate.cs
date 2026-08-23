using UnityEngine;

// Marker + physics setup for a crate the player can push by walking into
// it. Ordinary Rigidbody2D/Collider2D physics does the pushing - this just
// guarantees sane, stable defaults (no rotation, no drifting, no gravity)
// regardless of how the crate was placed in the scene.
[RequireComponent(typeof(Rigidbody2D))]
public class PushableCrate : MonoBehaviour
{
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 6f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
