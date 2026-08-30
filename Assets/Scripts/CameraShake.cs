using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

// Restrained camera shake for the Rooftop collapse sequence. Drives
// Cinemachine's own CinemachineBasicMultiChannelPerlin component rather
// than the camera's transform directly - Cinemachine recalculates its
// camera's transform every frame from its own pipeline, so anything that
// wrote to the transform directly would just get overwritten. Perlin noise
// is the pipeline's own noise stage, so it composites correctly instead.
public class CameraShake : MonoBehaviour
{
    public CinemachineBasicMultiChannelPerlin perlin;
    public float maxAmplitudeGain = 1.2f;

    private Coroutine activeShake;

    public void Shake(float duration, float amplitude)
    {
        if (perlin == null)
            return;

        if (activeShake != null)
            StopCoroutine(activeShake);

        activeShake = StartCoroutine(ShakeRoutine(duration, Mathf.Min(amplitude, maxAmplitudeGain)));
    }

    private IEnumerator ShakeRoutine(float duration, float amplitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float damper = 1f - Mathf.Clamp01(elapsed / duration);
            perlin.AmplitudeGain = amplitude * damper;
            yield return null;
        }

        perlin.AmplitudeGain = 0f;
    }
}
