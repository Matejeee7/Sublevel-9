using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Detection")]
    public Transform player;
    public float detectionRange = 4f;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 1f;

    private Rigidbody2D rb;

    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    private float waitTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        float distanceToPlayer =
            Vector2.Distance(transform.position, player.position);

        PlayerStealth playerStealth = player.GetComponent<PlayerStealth>();

        if (playerStealth != null && playerStealth.isHidden)
        {
            isChasing = false;
            Patrol();
            return;
        }

        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }
        else if (distanceToPlayer > detectionRange + 1f)
        {
            isChasing = false;
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void ChasePlayer()
    {
        Vector2 direction =
            (player.position - transform.position).normalized;

        rb.linearVelocity = direction * moveSpeed;
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Transform targetPoint =
            patrolPoints[currentPatrolIndex];

        Vector2 direction =
            (targetPoint.position - transform.position).normalized;

        rb.linearVelocity = direction * moveSpeed;

        float distance =
            Vector2.Distance(transform.position, targetPoint.position);

        if (distance < 0.2f)
        {
            rb.linearVelocity = Vector2.zero;

            waitTimer += Time.fixedDeltaTime;

            if (waitTimer >= patrolWaitTime)
            {
                currentPatrolIndex++;

                if (currentPatrolIndex >= patrolPoints.Length)
                {
                    currentPatrolIndex = 0;
                }

                waitTimer = 0f;
            }
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