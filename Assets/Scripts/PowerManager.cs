using UnityEngine;

public class PowerManager : MonoBehaviour
{
    public int maxPower = 3;
    public int usedPower = 0;

    // Raised whenever usedPower or maxPower changes, so UI (PowerHUD) can
    // update immediately instead of polling.
    public event System.Action Changed;

    // Power still free to route: maxPower minus what powered objects use.
    public int AvailablePower => Mathf.Max(0, maxPower - usedPower);

    public bool CanUsePower(int amount)
    {
        return usedPower + amount <= maxPower;
    }

    public void AddPowerUsage(int amount)
    {
        usedPower += amount;
        Debug.Log("Power used: " + usedPower + "/" + maxPower);
        Changed?.Invoke();
    }

    public void RemovePowerUsage(int amount)
    {
        usedPower -= amount;
        if (usedPower < 0)
            usedPower = 0;

        Debug.Log("Power used: " + usedPower + "/" + maxPower);
        Changed?.Invoke();
    }

    public void SetMaxPower(int newMaxPower)
    {
        maxPower = newMaxPower;
        Changed?.Invoke();
    }
}
