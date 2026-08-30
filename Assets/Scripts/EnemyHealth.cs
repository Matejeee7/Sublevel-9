using System.Collections;
using UnityEngine;

// Rooftop-finale-only combat health for an EnemyAI. Deliberately a
// separate component instead of touching EnemyAI itself, so Levels1-9
// (which never add this component) are completely unaffected - their
// enemies stay exactly as unkillable as before.
public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public float hitFlashDuration = 0.08f;
    public float deathFadeDuration = 0.8f;

    public event System.Action Died;

    private int currentHealth;
    private bool isDead = false;
    private SpriteRenderer sr;
    private Color originalColor;
    private EnemyAI enemyAI;
    private Collider2D col;
    private Rigidbody2D rb;

    public bool IsDead => isDead;

    void Awake()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;

        enemyAI = GetComponent<EnemyAI>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
            return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(HitFlash());
        }
    }

    private IEnumerator HitFlash()
    {
        if (sr != null)
            sr.color = Color.white;

        yield return new WaitForSeconds(hitFlashDuration);

        if (sr != null && !isDead)
            sr.color = originalColor;
    }

    private void Die()
    {
        isDead = true;
        StopAllCoroutines();

        // Can no longer detect/chase/damage the player (EnemyAI's collision
        // callback is a MonoBehaviour message, which Unity does not invoke
        // once the component is disabled) and no longer blocks movement.
        if (enemyAI != null)
            enemyAI.enabled = false;

        if (col != null)
            col.enabled = false;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (sr != null)
            StartCoroutine(FadeOut());

        Died?.Invoke();
    }

    private IEnumerator FadeOut()
    {
        float t = 0f;
        Color start = sr.color;

        while (t < deathFadeDuration)
        {
            t += Time.deltaTime;
            Color c = start;
            c.a = Mathf.Lerp(start.a, 0f, t / deathFadeDuration);
            sr.color = c;
            yield return null;
        }
    }
}
