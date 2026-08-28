using UnityEngine;

// Gentle hover/bob while the helicopter is waiting for the player. Must be
// disabled before RooftopSequence starts driving this transform's position
// directly during liftoff - both writing to the same position every frame
// would fight each other exactly like the terminal open/close race did.
public class HelicopterIdle : MonoBehaviour
{
    public float bobAmplitude = 0.15f;
    public float bobSpeed = 1.5f;

    [Header("Audio (optional)")]
    public AudioClip rotorLoopClip;

    [Range(0f, 1f)]
    public float rotorVolume = 0.5f;

    private Vector3 basePosition;
    private AudioSource rotorSource;

    void Awake()
    {
        basePosition = transform.localPosition;

        if (rotorLoopClip != null)
        {
            rotorSource = gameObject.AddComponent<AudioSource>();
            rotorSource.clip = rotorLoopClip;
            rotorSource.loop = true;
            rotorSource.playOnAwake = false;
            rotorSource.spatialBlend = 0f;
            rotorSource.Play();
        }
    }

    void Update()
    {
        Vector3 p = basePosition;
        p.y += Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
        transform.localPosition = p;

        if (rotorSource != null)
            rotorSource.volume = rotorVolume * GameSettings.SfxVolume;
    }
}
