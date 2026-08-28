using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State { Patrol, Investigate, Chase, Search }

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float chaseSpeedMultiplier = 1.3f;
    public float arriveThreshold = 0.2f;

    [Header("Detection")]
    public Transform player;
    public float detectionRange = 4f;
    public float alertDetectionRangeMultiplier = 1.5f;

    [Header("Line Of Sight")]
    public LayerMask obstacleMask;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 1f;

    [Header("Alert")]
    public float investigateWaitTime = 2f;
    public float searchTime = 3f;

    [Header("Audio (optional)")]
    public AudioClip alertClip;

    private Rigidbody2D rb;
    private State state = State.Patrol;

    private int currentPatrolIndex = 0;
    private float patrolWaitTimer;

    private Vector2 alertTarget;
    private float alertStateTimer;

    private AlarmController alarmController;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        alarmController = FindFirstObjectByType<AlarmController>();
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        bool playerVisible = IsPlayerVisible();

        if (playerVisible)
        {
            state = State.Chase;
        }
        else if (state == State.Chase)
        {
            // Player just broke line of detection - go check their last known spot.
            state = State.Search;
            alertTarget = player.position;
            alertStateTimer = 0f;
        }

        switch (state)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Investigate:
                GoToAlertTargetThenReturn(investigateWaitTime);
                break;
            case State.Search:
                GoToAlertTargetThenReturn(searchTime);
                break;
        }
    }

    private bool IsPlayerVisible()
    {
        if (player == null)
            return false;

        PlayerStealth playerStealth = player.GetComponent<PlayerStealth>();

        if (playerStealth != null && playerStealth.isHidden)
            return false;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        float effectiveRange = detectionRange;

        if (alarmController != null && alarmController.currentState == AlarmController.AlarmState.Alert)
        {
            effectiveRange *= alertDetectionRangeMultiplier;
        }

        if (distanceToPlayer > effectiveRange)
            return false;

        return HasLineOfSight(player.position, distanceToPlayer);
    }

    // Same principle as SecurityCamera's obstacle check: a single raycast
    // toward the target on the shared Obstacle/VisionBlocker layers. A wall
    // or a closed door blocks it exactly like it blocks a camera's view.
    private bool HasLineOfSight(Vector2 targetPosition, float distance)
    {
        if (distance < 0.01f)
            return true;

        Vector2 origin = transform.position;
        Vector2 direction = (targetPosition - origin) / distance;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, obstacleMask);

        return hit.collider == null;
    }

    // Called by systems like SecurityCamera when they spot the player
    // without killing them outright - the enemy investigates instead.
    public void Alert(Vector2 position)
    {
        if (state == State.Chase)
            return;

        state = State.Investigate;
        alertTarget = position;
        alertStateTimer = 0f;
        SfxPlayer.Play(alertClip);
    }

    void Chase()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed * chaseSpeedMultiplier;
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Transform targetPoint = patrolPoints[currentPatrolIndex];

        Vector2 direction = (targetPoint.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        float distance = Vector2.Distance(transform.position, targetPoint.position);

        if (distance < arriveThreshold)
        {
            rb.linearVelocity = Vector2.zero;

            patrolWaitTimer += Time.fixedDeltaTime;

            if (patrolWaitTimer >= patrolWaitTime)
            {
                currentPatrolIndex++;

                if (currentPatrolIndex >= patrolPoints.Length)
                {
                    currentPatrolIndex = 0;
                }

                patrolWaitTimer = 0f;
            }
        }
    }

    void GoToAlertTargetThenReturn(float waitTime)
    {
        Vector2 toTarget = alertTarget - (Vector2)transform.position;
        float distance = toTarget.magnitude;

        if (distance > arriveThreshold)
        {
            rb.linearVelocity = toTarget.normalized * moveSpeed;
            return;
        }

        rb.linearVelocity = Vector2.zero;
        alertStateTimer += Time.fixedDeltaTime;

        if (alertStateTimer >= waitTime)
        {
            state = State.Patrol;
            alertStateTimer = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        PlayerStealth playerStealth =
            collision.gameObject.GetComponent<PlayerStealth>();

        if (playerStealth != null && playerStealth.isHidden)
        {
            Debug.Log("Enemy touched locker, but player is hidden.");
            return;
        }

        PlayerHealth playerHealth =
            collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.Die();
        }
    }
}
