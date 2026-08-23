using UnityEngine;
using UnityEngine.SceneManagement;

public class WinZone : MonoBehaviour
{
    public string nextSceneName = "Level1";
    public PoweredObject requiredPowerObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (requiredPowerObject != null && !requiredPowerObject.isPowered)
        {
            Debug.Log("Elevator has no power.");
            FeedbackUI.Flash("Elevator has no power.");
            return;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
