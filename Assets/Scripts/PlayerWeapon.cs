using UnityEngine;

// Rooftop-finale-only hitscan weapon. Disabled (hasWeapon = false) until
// WeaponPickup enables it, so the player physically cannot shoot before
// picking up the gun. Aims at the mouse world position on a visual child
// only - the player's own gameplay root/collider never rotates.
public class PlayerWeapon : MonoBehaviour
{
    [Header("Setup")]
    public Transform weaponVisual;
    public Transform muzzle;
    public LayerMask hitMask;

    [Header("Stats")]
    public float fireRate = 4f;
    public int damage = 1;
    public float range = 20f;

    [Header("Feedback")]
    public ParticleSystem muzzleFlash;
    public LineRenderer tracerPrefab;
    public float tracerDuration = 0.05f;
    public ParticleSystem impactEffect;

    private bool hasWeapon = false;
    private float nextFireTime = 0f;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        if (weaponVisual != null)
            weaponVisual.gameObject.SetActive(false);
    }

    public void EnableWeapon()
    {
        hasWeapon = true;

        if (weaponVisual != null)
            weaponVisual.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!hasWeapon || cam == null)
            return;

        if (InteractionPromptUI.InputCaptured)
            return;

        AimAtMouse();

        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + (1f / Mathf.Max(0.01f, fireRate));
            Fire();
        }
    }

    private void AimAtMouse()
    {
        if (weaponVisual == null)
            return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = weaponVisual.position.z;

        Vector2 direction = (mouseWorld - weaponVisual.position);
        if (direction.sqrMagnitude < 0.0001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        weaponVisual.rotation = Quaternion.Euler(0f, 0f, angle);

        // Flip vertically instead of showing the sprite upside-down when
        // aiming into the left half-plane.
        Vector3 scale = weaponVisual.localScale;
        scale.y = Mathf.Abs(scale.y) * (Mathf.Abs(angle) > 90f ? -1f : 1f);
        weaponVisual.localScale = scale;
    }

    private void Fire()
    {
        Vector3 origin = muzzle != null ? muzzle.position : transform.position;

        // weaponVisual.right reflects the rotation set in AimAtMouse and is
        // unaffected by that method's cosmetic Y-scale flip (a Y-flip
        // mirrors the sprite around its local X-axis, which does not move
        // the barrel tip drawn along that same axis) - no extra correction
        // needed here.
        Vector2 direction = weaponVisual != null ? (Vector2)(weaponVisual.right) : Vector2.right;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, range, hitMask);

        // A raycast whose origin starts inside a collider reports a hit at
        // distance 0 against it - guard explicitly rather than relying on
        // muzzle offset alone to keep the player's own collider out of range.
        if (hit.collider != null && hit.collider.transform.IsChildOf(transform))
            hit = Physics2D.Raycast(origin + (Vector3)(direction * 0.55f), direction, range - 0.55f, hitMask);

        Vector3 endPoint = hit.collider != null ? (Vector3)hit.point : origin + (Vector3)(direction * range);

        if (muzzleFlash != null)
        {
            muzzleFlash.transform.position = origin;
            muzzleFlash.Play();
        }

        if (tracerPrefab != null)
        {
            LineRenderer tracer = Instantiate(tracerPrefab, Vector3.zero, Quaternion.identity);
            tracer.positionCount = 2;
            tracer.SetPosition(0, origin);
            tracer.SetPosition(1, endPoint);
            Destroy(tracer.gameObject, tracerDuration);
        }

        if (hit.collider != null)
        {
            if (impactEffect != null)
            {
                impactEffect.transform.position = hit.point;
                impactEffect.Play();
            }

            EnemyHealth health = hit.collider.GetComponent<EnemyHealth>();
            if (health != null)
                health.TakeDamage(damage);
        }
    }
}
