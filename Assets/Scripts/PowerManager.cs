using UnityEngine;

public class PowerManager : MonoBehaviour
{
    public int maxPower = 3;
    public int usedPower = 0;

    public bool CanUsePower(int amount)
    {
        return usedPower + amount <= maxPower;
    }

    public void AddPowerUsage(int amount)
    {
        usedPower += amount;
        Debug.Log("Power used: " + usedPower + "/" + maxPower);
    }

    public void RemovePowerUsage(int amount)
    {
        usedPower -= amount;
        if (usedPower < 0)
            usedPower = 0;

        Debug.Log("Power used: " + usedPower + "/" + maxPower);
    }
}