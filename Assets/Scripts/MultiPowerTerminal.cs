using System.Text;
using TMPro;
using UnityEngine;

public class MultiPowerTerminal : Interactable
{
    public PoweredObject[] poweredObjects;

    [Header("UI")]
    public GameObject terminalPanel;
    public TMP_Text optionsText;
    public TMP_Text powerUsageText;

    [Header("Audio (optional)")]
    public AudioClip openClip;
    public AudioClip closeClip;

    // Global ownership so the pause menu and the interaction prompt can
    // always ask "is a terminal currently capturing input" without holding
    // a reference to a specific instance.
    public static bool IsAnyTerminalOpen { get; private set; }
    private static MultiPowerTerminal openTerminal;

    private bool isTerminalOpen = false;
    private PlayerMovement playerMovement;
    private PowerManager powerManager;

    // PlayerInteraction.Interact() (open) and this script's own Update()
    // (close) both react to the same E keypress in the same frame, and
    // Input.GetKeyDown stays true for the whole frame no matter how many
    // scripts read it. Without this guard, whichever of the two runs
    // second sees the state the first one just changed and immediately
    // undoes it - the terminal opens then closes (or closes then reopens)
    // within a single frame, which looks exactly like "E does nothing."
    private int lastToggleFrame = -1;

    void Awake()
    {
        // Defensive reset: these statics must never leak a "terminal open"
        // state in from a previous scene (domain doesn't reload between
        // scene loads in a build), so every terminal clears them on its
        // own Awake, before any Interact() can run.
        IsAnyTerminalOpen = false;
        openTerminal = null;
    }

    public override void Interact()
    {
        if (Time.frameCount == lastToggleFrame)
            return;

        OpenTerminal();
    }

    void Update()
    {
        if (!isTerminalOpen)
            return;

        if (Time.frameCount != lastToggleFrame && Input.GetKeyDown(KeyCode.E))
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
        lastToggleFrame = Time.frameCount;
        isTerminalOpen = true;
        IsAnyTerminalOpen = true;
        openTerminal = this;

        playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (terminalPanel != null)
            terminalPanel.SetActive(true);

        InteractionPromptUI.Hide();
        SfxPlayer.Play(openClip);

        RefreshUI();
    }

    // reenableMovement is false when the pause menu is the one closing this
    // terminal (Esc flow) - movement stays owned by the pause menu instead
    // of flicking back on for a frame before Time.timeScale takes over.
    public void CloseTerminal(bool reenableMovement = true)
    {
        lastToggleFrame = Time.frameCount;
        isTerminalOpen = false;
        IsAnyTerminalOpen = false;
        if (openTerminal == this)
            openTerminal = null;

        if (playerMovement != null && reenableMovement)
            playerMovement.enabled = true;

        if (terminalPanel != null)
            terminalPanel.SetActive(false);

        SfxPlayer.Play(closeClip);
    }

    public static void CloseAnyOpenTerminal(bool reenableMovement = true)
    {
        if (openTerminal != null)
            openTerminal.CloseTerminal(reenableMovement);
    }

    private void RefreshUI()
    {
        if (powerManager == null)
            powerManager = FindFirstObjectByType<PowerManager>();

        if (optionsText != null)
        {
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

        if (powerUsageText != null && powerManager != null)
        {
            powerUsageText.text = "Power Usage: " + powerManager.usedPower + " / " + powerManager.maxPower;
        }
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
