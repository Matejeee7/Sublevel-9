using UnityEngine;

// A one-shot cinematic explosion: the player crossing this zone plays a
// particle burst (+ optional smoke), once. Purely pressure/spectacle for
// the rooftop chase - it never damages the player, so it never needs to be
// "fair," only clearly telegraphed by its own particle burst.
public class ScriptedExplosion : MonoBehaviour
{
    public ParticleSystem explosionParticles;
    public ParticleSystem smokeParticles;
    public AudioClip explosionClip;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag("Player"))
            return;

        triggered = true;

        if (explosionParticles != null)
            explosionParticles.Play();

        if (smokeParticles != null)
            smokeParticles.Play();

        SfxPlayer.Play(explosionClip);
    }
}
