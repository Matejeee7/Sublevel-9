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

        if (helicopter != null)
        {
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
