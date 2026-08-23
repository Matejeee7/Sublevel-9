using UnityEngine;

// Opens/closes the settings panel from the main menu. Deliberately
// separate from PauseMenuController - there is no "pause" concept on
// the main menu, just a panel to show or hide.
public class MainMenuSettingsController : MonoBehaviour
{
    public GameObject settingsPanel;

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
}
