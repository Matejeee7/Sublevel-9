using System.Text;
using TMPro;
using UnityEngine;

public class MultiPowerTerminal : Interactable
{
    public PoweredObject[] poweredObjects;

    [Header("UI")]
    public GameObject terminalPanel;
    public TMP_Text optionsText;

    private bool isTerminalOpen = false;
    private PlayerMovement playerMovement;

    public override void Interact()
    {
        isTerminalOpen = !isTerminalOpen;

        if (isTerminalOpen)
        {
            OpenTerminal();
        }
        else
        {
            CloseTerminal();
        }
    }

    void Update()
    {
        if (!isTerminalOpen)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseTerminal();
            return;
        }

        for (int i = 0; i < poweredObjects.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                ToggleObject(i);
                RefreshUI();
            }
        }
    }

    private void OpenTerminal()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (terminalPanel != null)
            terminalPanel.SetActive(true);

        RefreshUI();
    }

    private void CloseTerminal()
    {
        isTerminalOpen = false;

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (terminalPanel != null)
            terminalPanel.SetActive(false);
    }

    private void RefreshUI()
    {
        if (optionsText == null)
            return;

        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < poweredObjects.Length; i++)
        {
            if (poweredObjects[i] == null)
                continue;

            string state = poweredObjects[i].isPowered ? "ON" : "OFF";

            builder.AppendLine(
                (i + 1) + " - " +
                poweredObjects[i].objectName +
                " [" + state + "] Cost: " +
                poweredObjects[i].powerCost
            );
        }

        optionsText.text = builder.ToString();
    }

    private void ToggleObject(int index)
    {
        if (index < 0 || index >= poweredObjects.Length)
            return;

        if (poweredObjects[index] == null)
            return;

        poweredObjects[index].TogglePower();
    }
}