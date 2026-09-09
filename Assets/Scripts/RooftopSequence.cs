using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// The escape cutscene: walk to the helicopter, lift off, fade to black,
// load the win screen. Deliberately a plain coroutine, not Timeline - a
// short scripted sequence is all this needs.
public class RooftopSequence : MonoBehaviour
{
    public GameObject player;
    public Transform helicopterEntryPoint;
    public Transform helicopter;
    public Vector3 helicopterLiftOffset = new Vector3(0, 15, 0);
    public float playerMoveSpeed = 4f;
    public float helicopterLiftSpeed = 3f;
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1.5f;
    public string nextScene = "WinScene";

    private bool sequenceStarted = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (sequenceStarted)
            return;

        if (!other.CompareTag("Player"))
            return;

        sequenceStarted = true;
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // Reaching the helicopter is the win condition, but the walk/lift-off/
        // fade takes several seconds during which the player can't move or
        // hide. Without this, a still-chasing enemy could reach the player
        // mid-sequence, call PlayerHealth.Die(), and load GameOverScene out
        // from under the win sequence's own (delayed) scene load. Neutralize
        // every enemy and the player's own collider the instant extraction
        // begins so nothing can kill the player anymore.
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        if (playerCollider != null)
            playerCollider.enabled = false;

        foreach (EnemyAI enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
        {
            enemy.enabled = false;

            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
                enemyRb.linearVelocity = Vector2.zero;
        }

        while (Vector2.Distance(player.transform.position, helicopterEntryPoint.position) > 0.1f)
        {
            player.transform.position = Vector2.MoveTowards(
                player.transform.position,
                helicopterEntryPoint.position,
                playerMoveSpeed * Time.deltaTime);
            yield return null;
        }

        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
        if (playerSprite != null)
            playerSprite.enabled = false;

        // The equipped weapon is a separate child SpriteRenderer
        // (PlayerWeapon.weaponVisual) - hiding the player's own sprite
        // above doesn't touch it, so without this it would keep
        // rendering, left floating at the extraction point during liftoff.
        if (PlayerWeapon.Instance != null && PlayerWeapon.Instance.weaponVisual != null)
            PlayerWeapon.Instance.weaponVisual.gameObject.SetActive(false);

        FeedbackUI.Flash("Extraction complete.");

        if (helicopter != null)
        {
            // The idle hover/bob writes to this same transform every frame;
            // left running, it would fight the liftoff movement below for
            // control of helicopter.position (the same class of bug fixed
            // in MultiPowerTerminal's E-interaction race).
            HelicopterIdle idle = helicopter.GetComponent<HelicopterIdle>();
            if (idle != null)
                idle.enabled = false;

            Vector3 targetPos = helicopter.position + helicopterLiftOffset;
            while (Vector3.Distance(helicopter.position, targetPos) > 0.1f)
            {
                helicopter.position = Vector3.MoveTowards(
                    helicopter.position,
                    targetPos,
                    helicopterLiftSpeed * Time.deltaTime);
                yield return null;
            }
        }

        if (fadeCanvasGroup != null)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
                yield return null;
            }
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(nextScene);
    }
}
