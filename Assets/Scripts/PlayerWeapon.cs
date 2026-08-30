using UnityEngine;

// Rooftop-finale-only hitscan weapon. Disabled (hasWeapon = false) until
// WeaponPickup calls EnableWeapon(), so the player physically cannot shoot
// before picking up the gun. Mouse aim drives WeaponPivot's rotation only -
// the player's own gameplay root/collider never rotates, and WASD movement
// (PlayerMovement) is completely independent of aim direction.
public class PlayerWeapon : MonoBehaviour
{
    public static PlayerWeapon Instance { get; private set; }

    [Header("Hierarchy")]
    public Transform weaponPivot;
    public Transform weaponVisual;
    public Transform muzzle;

    [Header("Stats")]
    public float fireRate = 4f;
    public int damage = 1;
    public float maxRange = 9f;
    public LayerMask hitMask;

    [Header("Feedback")]
    public ParticleSystem muzzleFlash;
    public float muzzleFlashDuration = 0.05f;
    public LineRenderer tracerPrefab;
    public float tracerDuration = 0.05f;
    public ParticleSystem impactEffectWall;
    public ParticleSystem impactEffectEnemy;

    [Header("Audio (optional - safe if left empty)")]
    public AudioClip gunshotClip;
    public AudioClip hitClip;

    private bool hasWeapon = false;
    private float nextFireTime = 0f;
    private Camera cam;
    private PlayerMovement movement;
    private SpriteRenderer bodySprite;
    private AudioSource audioSource;

    public bool HasWeapon => hasWeapon;
    public Vector3 CurrentAimPoint { get; private set; }

    // Single source of truth for "is the player currently allowed to aim/
    // shoot" - shared by both Update() and CombatCrosshair, so the crosshair
    // never shows during pause/terminal UI or the extraction sequence
    // (RooftopSequence disables PlayerMovement the instant extraction starts).
    public bool CanAim => hasWeapon && cam != null
        && (movement == null || movement.enabled)
        && !InteractionPromptUI.InputCaptured;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cam = Camera.main;
        movement = GetComponent<PlayerMovement>();
        bodySprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        CurrentAimPoint = transform.position;

        if (weaponVisual != null)
            weaponVisual.gameObject.SetActive(false);

        if (muzzleFlash != null)
        {
            ParticleSystem.MainModule main = muzzleFlash.main;
            main.startLifetime = muzzleFlashDuration;
        }
    }

    public void EnableWeapon()
    {
        hasWeapon = true;
        if (weaponVisual != null)
            weaponVisual.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!CanAim)
            return;

        UpdateAim();

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + (1f / Mathf.Max(0.01f, fireRate));
            Fire();
        }
    }

    private void UpdateAim()
    {
        Vector3 origin = weaponPivot != null ? weaponPivot.position : transform.position;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = origin.z;

        Vector2 toMouse = mouseWorld - origin;
        float distance = toMouse.magnitude;
        Vector2 aimDir = distance > 0.0001f ? toMouse / distance : Vector2.right;

        // Crosshair clamps to the edge of maxRange so it always shows exactly
        // where a shot could actually land - Fire() reads this same point.
        CurrentAimPoint = distance > maxRange ? origin + (Vector3)(aimDir * maxRange) : mouseWorld;

        if (weaponPivot != null)
        {
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
            weaponPivot.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        if (weaponVisual != null)
        {
            // Flip on the visual child only (never the pivot/root) so the
            // sprite reads right-side-up through the full circle instead of
            // rendering upside-down while aiming into the left hemisphere.
            Vector3 scale = weaponVisual.localScale;
            float sign = aimDir.x < 0f ? -1f : 1f;
            scale.y = Mathf.Abs(scale.y) * sign;
            weaponVisual.localScale = scale;
        }

        if (bodySprite != null && Mathf.Abs(aimDir.x) > 0.01f)
            bodySprite.flipX = aimDir.x < 0f;
    }

    private void Fire()
    {
        Vector3 origin = muzzle != null ? muzzle.position : transform.position;
        Vector2 direction = CurrentAimPoint - origin;
        float dist = direction.magnitude;
        if (dist < 0.0001f)
            return;
        direction /= dist;

        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, maxRange, hitMask);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        RaycastHit2D? finalHit = null;
        foreach (RaycastHit2D h in hits)
        {
            if (h.collider == null) continue;
            // Ignore trigger volumes (weapon pickup, lockers, explosion
            // zones, the escape trigger) so a shot can pass straight through
            // them to the real wall or enemy behind. Only solid colliders
            // (walls, enemies) can ever stop or register a hit.
            if (h.collider.isTrigger) continue;
            if (h.collider.transform.IsChildOf(transform)) continue;
            finalHit = h;
            break;
        }

        Vector3 endPoint = finalHit.HasValue ? (Vector3)finalHit.Value.point : origin + (Vector3)(direction * maxRange);

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

        if (gunshotClip != null && audioSource != null)
            audioSource.PlayOneShot(gunshotClip);

        if (finalHit.HasValue)
        {
            EnemyHealth health = finalHit.Value.collider.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                PlayImpact(impactEffectEnemy, endPoint);
                if (hitClip != null && audioSource != null)
                    audioSource.PlayOneShot(hitClip);
            }
            else
            {
                PlayImpact(impactEffectWall, endPoint);
            }
        }
    }

    private void PlayImpact(ParticleSystem effect, Vector3 pos)
    {
        if (effect == null) return;
        effect.transform.position = pos;
        effect.Play();
    }
}
