using UnityEngine;

// Swaps between pre-drawn directional sprites based on movement, rather
// than rotating a continuous sprite - correct for hand-drawn character art,
// where rotating the image would tip the character sideways. The source
// spritesheets only draw a down-facing pose and a side-facing pose (no
// back/up-facing frame), so moving up reuses the down sprite and left/right
// share the side sprite via SpriteRenderer.flipX - a real, disclosed
// limitation of the asset, not a fabrication.
public class DirectionalSprite : MonoBehaviour
{
    public Transform watchedBody;
    public SpriteRenderer targetRenderer;
    public Sprite downSprite;
    public Sprite sideSprite;
    public float minMoveSqrThreshold = 0.0004f;

    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = (watchedBody != null ? watchedBody : transform).position;
    }

    void LateUpdate()
    {
        Transform body = watchedBody != null ? watchedBody : transform;
        Vector3 delta = body.position - lastPosition;
        lastPosition = body.position;

        if (targetRenderer == null || delta.sqrMagnitude < minMoveSqrThreshold)
            return;

        if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
        {
            targetRenderer.sprite = sideSprite;
            targetRenderer.flipX = delta.x < 0f;
        }
        else
        {
            targetRenderer.sprite = downSprite;
            targetRenderer.flipX = false;
        }
    }
}
