using UnityEngine;

// Rotates a visual child to face the last direction of travel, purely by
// watching a body transform's position each frame - no changes needed to
// movement/AI scripts, so it's safe to add to player and enemy visuals
// alike without touching gameplay code.
public class DirectionalFacing : MonoBehaviour
{
    public Transform watchedBody;
    public float minMoveSqrThreshold = 0.0004f;
    public float rotateSpeed = 720f;

    private Vector3 lastPosition;
    private float currentAngle = -90f;
    private bool initialized = false;

    void LateUpdate()
    {
        Transform body = watchedBody != null ? watchedBody : transform.parent;
        if (body == null)
            return;

        if (!initialized)
        {
            lastPosition = body.position;
            initialized = true;
            return;
        }

        Vector3 delta = body.position - lastPosition;
        lastPosition = body.position;

        if (delta.sqrMagnitude > minMoveSqrThreshold)
        {
            float targetAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg - 90f;
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotateSpeed * Time.deltaTime);
        }

        transform.localRotation = Quaternion.Euler(0f, 0f, currentAngle);
    }
}
