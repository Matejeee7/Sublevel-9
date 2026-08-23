using UnityEngine;

public class PowerLightMask : MonoBehaviour
{
    public PoweredObject linkedPowerObject;
    public GameObject lightMaskObject;
    public GameObject lightGlowObject;

    void Update()
    {
        if (linkedPowerObject == null)
            return;

        bool isOn = linkedPowerObject.isPowered;

        if (lightMaskObject != null)
            lightMaskObject.SetActive(isOn);

        if (lightGlowObject != null)
            lightGlowObject.SetActive(isOn);
    }
}