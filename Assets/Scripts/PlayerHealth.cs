using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public void Die()
    {
        Debug.Log("PLAYER DIED");
        SceneManager.LoadScene("GameOverScene");
    }
}