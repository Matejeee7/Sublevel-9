using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State { Patrol, Investigate, Chase, Search, Return }

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
    // A search/investigation that has not finished by then (for example the
    // spot is blocked by the hidden player) is abandoned and the guard goes home.
    public float maxSearchDuration = 8f;

    [Header("Return Home")]
    public float trailSpacing = 1f;

    private Rigidbody2D rb;
    private State state = State.Patrol;

    private int currentPatrolIndex = 0;
    private float patrolWaitTimer;

    private Vector2 alertTarget;
    private float alertStateTimer;

    private AlarmController alarmController;

    // Where the guard started in the scene. After losing the player it walks
    // back here and restarts its patrol. The trail is the route it took away
    // from its post; it is walked in reverse so the way home never cuts through
    // a wall (guards move in straight lines and have no pathfinding).
    private Vector2 homePosition;
    private readonly List<Vector2> trail = new List<Vector2>();
    private State lastState = State.Patrol;
    private float alertElapsed;
    private Vector2 lastProgressPosition;
    private float progressTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        alarmController = FindFirstObjectByType<AlarmController>();
        homePosition = transform.position;
        lastProgressPosition = homePosition;
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
            alertElapsed = 0f;
        }

        TrackTrail();

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
            case State.Return:
                ReturnHome();
                break;
        }
    }

    // Remembers the path walked away from the post (or from the last point
    // of a return trip that was interrupted by seeing the player again).
    private void TrackTrail()
    {
        bool away = state == State.Chase || state == State.Investigate || state == State.Search;

        if (state != lastState)
        {
            if (away && (lastState == State.Patrol || lastState == State.Return))
                AddCrumb(true);
            else if (state == State.Patrol)
                trail.Clear();

            lastState = state;
        }

        if (away)
            AddCrumb(false);
    }

    private void AddCrumb(bool force)
    {
        Vector2 position = transform.position;

        if (trail.Count == 0 || Vector2.Distance(trail[trail.Count - 1], position) >= (force ? 0.05f : trailSpacing))
            trail.Add(position);
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
        alertElapsed = 0f;
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
        alertElapsed += Time.fixedDeltaTime;

        if (alertElapsed >= maxSearchDuration)
        {
            BeginReturn();
            return;
        }

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
            BeginReturn();
    }

    private void BeginReturn()
    {
        state = State.Return;
        alertStateTimer = 0f;
        alertElapsed = 0f;
        progressTimer = 0f;
        lastProgressPosition = transform.position;
    }

    // Walks back along the trail to the post, then restarts the patrol from
    // its first point (or stands still if the guard has no patrol points).
    void ReturnHome()
    {
        Vector2 target = trail.Count > 0 ? trail[trail.Count - 1] : homePosition;
        Vector2 toTarget = target - (Vector2)transform.position;

        if (toTarget.magnitude > arriveThreshold)
        {
            rb.linearVelocity = toTarget.normalized * moveSpeed;

            // Blocked (for example by a door that closed meanwhile): skip
            // this step instead of standing still forever.
            progressTimer += Time.fixedDeltaTime;
            if (progressTimer >= 1.5f)
            {
                if (Vector2.Distance(lastProgressPosition, transform.position) < 0.3f)
                    ArriveAtReturnStep();

                lastProgressPosition = transform.position;
                progressTimer = 0f;
            }
            return;
        }

        ArriveAtReturnStep();
    }

    private void ArriveAtReturnStep()
    {
        progressTimer = 0f;
        lastProgressPosition = transform.position;

        if (trail.Count > 0)
        {
            trail.RemoveAt(trail.Count - 1);
            return;
        }

        rb.linearVelocity = Vector2.zero;
        currentPatrolIndex = 0;
        patrolWaitTimer = 0f;
        state = State.Patrol;
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
