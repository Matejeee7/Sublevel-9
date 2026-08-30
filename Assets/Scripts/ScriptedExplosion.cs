using UnityEngine;

// A one-shot cinematic explosion: the player crossing this zone plays a
// particle burst (+ optional smoke), once. Purely pressure/spectacle for
// the rooftop chase - it never damages the player, so it never needs to be
// "fair," only clearly telegraphed by its own particle burst. TriggerNow()
// lets a script (the collapse sequence) fire it directly instead of
// waiting for player contact, still only once.
public class ScriptedExplosion : MonoBehaviour
{
    public ParticleSystem explosionParticles;
    public ParticleSystem smokeParticles;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        TriggerNow();
    }

    public void TriggerNow()
    {
        if (triggered)
            return;

        triggered = true;

        if (explosionParticles != null)
            explosionParticles.Play();

        if (smokeParticles != null)
            smokeParticles.Play();
    }
}
