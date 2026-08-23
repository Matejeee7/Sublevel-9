using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadStartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }

    public void LoadTutorialLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TutorialLevel");
    }

    public void LoadLevel1()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level1");
    }

    public void LoadGameOverScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameOverScene");
    }

    public void LoadWinScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("WinScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }
}