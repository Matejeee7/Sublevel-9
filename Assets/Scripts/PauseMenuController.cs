using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject settingsPanel;

    private bool isPaused = false;

    // Lets other shared systems (the interaction prompt, terminals) ask
    // "is the pause menu currently owning input" without a scene reference.
    public static bool IsPaused { get; private set; }

    void Awake()
    {
        // Defensive reset - see MultiPowerTerminal.Awake for why: statics
        // must not leak a stale "paused" state in from a previous scene.
        isPaused = false;
        IsPaused = false;
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            CloseSettings();
            return;
        }

        if (isPaused)
        {
            Resume();
            return;
        }

        // A terminal owns input: Esc closes it (without letting it flick
        // player movement back on for a frame) before the pause menu takes
        // over, so the player only ever sees one modal UI at a time.
        if (MultiPowerTerminal.IsAnyTerminalOpen)
            MultiPowerTerminal.CloseAnyOpenTerminal(false);

        TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        IsPaused = isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (pausePanel != null)
            pausePanel.SetActive(isPaused);
    }

    public void Resume()
    {
        isPaused = false;
        IsPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Restores movement after a terminal->Esc->pause->resume chain left
        // it disabled; harmless no-op otherwise since it's already enabled.
        if (!MultiPowerTerminal.IsAnyTerminalOpen)
        {
            PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
            if (playerMovement != null)
                playerMovement.enabled = true;
        }
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }
}
