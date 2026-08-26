using UnityEngine;

// Continuous rotation for a helicopter rotor sprite - the spin itself is
// what reads as "helicopter" from directly above, not the body shape.
public class RotorSpin : MonoBehaviour
{
    public float degreesPerSecond = 720f;

    void Update()
    {
        transform.Rotate(0f, 0f, degreesPerSecond * Time.deltaTime);
    }
}
