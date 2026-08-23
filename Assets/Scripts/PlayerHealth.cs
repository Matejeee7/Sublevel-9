using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public void Die()
    {
        Debug.Log("PLAYER DIED");
        GameProgress.SetCurrentLevel(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("GameOverScene");
    }
}