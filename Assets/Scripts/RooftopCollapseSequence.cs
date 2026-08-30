using System.Collections;
using UnityEngine;

// Coordinates the Rooftop finale's combat-completion beat: tracks enemy
// kills, keeps the objective's kill counter current, and once every
// registered enemy is dead, runs a short structural-failure sequence
// (warning, camera shake, explosions, the blocker sliding away) before
// handing the objective back to "reach the helicopter." Deterministic,
// scripted movement for the blocker - no physics involved in progression.
public class RooftopCollapseSequence : MonoBehaviour
{
    [Header("Combat tracking")]
    public EnemyHealth[] enemies;
    public string killObjectivePrefix = "Eliminate rooftop security";

    [Header("Blocker")]
    public GameObject blocker;
    public Vector3 blockerOpenOffset = new Vector3(0f, 6f, 0f);
    public float blockerMoveDuration = 1.5f;

    [Header("Sequence timing")]
    public float delayBeforeCollapse = 1f;
    public float shakeDuration = 2.5f;
    public float shakeAmplitude = 0.6f;

    [Header("Effects")]
    public CameraShake cameraShake;
    public ScriptedExplosion[] explosionsToTrigger;

    [Header("Objective")]
    public string finalObjective = "Reach the helicopter.";

    private int deadCount = 0;
    private bool collapseStarted = false;

    void OnEnable()
    {
        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy != null)
                enemy.Died += HandleEnemyDied;
        }
    }

    void OnDisable()
    {
        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy != null)
                enemy.Died -= HandleEnemyDied;
        }
    }

    private void HandleEnemyDied()
    {
        deadCount++;
        ObjectiveManager.SetObjective($"{killObjectivePrefix} — {deadCount} / {enemies.Length}");

        if (deadCount >= enemies.Length && !collapseStarted)
        {
            collapseStarted = true;
            StartCoroutine(CollapseRoutine());
        }
    }

    private IEnumerator CollapseRoutine()
    {
        yield return new WaitForSeconds(delayBeforeCollapse);

        FeedbackUI.Flash("Structural failure detected!");

        if (cameraShake != null)
            cameraShake.Shake(shakeDuration, shakeAmplitude);

        foreach (ScriptedExplosion explosion in explosionsToTrigger)
        {
            if (explosion != null)
                explosion.TriggerNow();
        }

        yield return new WaitForSeconds(0.4f);

        if (blocker != null)
        {
            Vector3 start = blocker.transform.position;
            Vector3 end = start + blockerOpenOffset;
            float t = 0f;

            while (t < blockerMoveDuration)
            {
                t += Time.deltaTime;
                blocker.transform.position = Vector3.Lerp(start, end, t / blockerMoveDuration);
                yield return null;
            }

            Collider2D blockerCollider = blocker.GetComponent<Collider2D>();
            if (blockerCollider != null)
                blockerCollider.enabled = false;
        }

        ObjectiveManager.SetObjective(finalObjective);
    }
}
