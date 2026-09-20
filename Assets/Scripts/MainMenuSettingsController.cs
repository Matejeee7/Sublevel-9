using UnityEngine;

// Opens/closes the settings panel from the main menu. Deliberately
// separate from PauseMenuController - there is no "pause" concept on
// the main menu, just a panel to show or hide.
//
// The panel and the main menu share one Canvas, so while Settings is open
// every main-menu element listed in hideWhileOpen (title, subtitle,
// buttons, controls hint) is hidden instead of being drawn under/over the
// panel. Closing Settings restores them exactly.
public class MainMenuSettingsController : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject[] hideWhileOpen;

    void Update()
    {
        if (settingsPanel != null && settingsPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            CloseSettings();
    }

    public void OpenSettings()
    {
        SetMenuVisible(false);

        // Guarantee the panel is the top-most element of the menu canvas.
        transform.SetAsLastSibling();

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        SetMenuVisible(true);
    }

    private void SetMenuVisible(bool visible)
    {
        if (hideWhileOpen == null)
            return;

        foreach (GameObject item in hideWhileOpen)
        {
            if (item != null)
                item.SetActive(visible);
        }
    }
}
