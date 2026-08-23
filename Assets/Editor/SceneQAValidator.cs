using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Reusable structural QA pass for any gameplay scene: catches the class of
// bugs listed in the project spec's bug-fixing checklist (duplicate
// PowerManagers, unwired terminal/camera/enemy references, zero-sized
// colliders, etc). Run via -executeMethod SceneQAValidator.ValidateCurrentLevel
// with -sceneName <Assets/Scenes/X.unity> or call Validate(path) directly.
public static class SceneQAValidator
{
    public static void ValidateCurrentLevel()
    {
        string path = "Assets/Scenes/Level3.unity";
        foreach (string arg in System.Environment.GetCommandLineArgs())
        {
            if (arg.StartsWith("-sceneName="))
                path = arg.Substring("-sceneName=".Length);
        }
        Validate(path);
    }

    public static void Validate(string scenePath)
    {
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        int pass = 0, fail = 0;

        void Check(string label, bool condition)
        {
            if (condition) { pass++; }
            else { fail++; Debug.LogError("QA FAIL [" + scenePath + "]: " + label); }
        }

        PowerManager[] pms = Object.FindObjectsByType<PowerManager>(FindObjectsSortMode.None);
        Check("At most one PowerManager", pms.Length <= 1);

        InventoryManager[] invs = Object.FindObjectsByType<InventoryManager>(FindObjectsSortMode.None);
        Check("At most one InventoryManager (Player)", invs.Length <= 1);

        foreach (EnemyAI e in Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
        {
            Check(e.name + ".player assigned", e.player != null);
            Check(e.name + " has a Rigidbody2D", e.GetComponent<Rigidbody2D>() != null);
        }

        foreach (SecurityCamera c in Object.FindObjectsByType<SecurityCamera>(FindObjectsSortMode.None))
        {
            Check(c.name + ".player assigned", c.player != null);
        }

        foreach (MultiPowerTerminal t in Object.FindObjectsByType<MultiPowerTerminal>(FindObjectsSortMode.None))
        {
            Check(t.name + ".terminalPanel assigned", t.terminalPanel != null);
            Check(t.name + ".optionsText assigned", t.optionsText != null);
            if (t.poweredObjects != null)
            {
                foreach (var po in t.poweredObjects)
                    Check(t.name + " has no null PoweredObject entries", po != null);
            }
        }

        foreach (HidingSpot h in Object.FindObjectsByType<HidingSpot>(FindObjectsSortMode.None))
        {
            Check(h.name + ".hidePoint assigned", h.hidePoint != null);
            Check(h.name + ".exitPoint assigned", h.exitPoint != null);
        }

        foreach (WinZone w in Object.FindObjectsByType<WinZone>(FindObjectsSortMode.None))
        {
            Check(w.name + " has a non-empty nextSceneName", !string.IsNullOrEmpty(w.nextSceneName));
        }

        foreach (CameraControlConsole cc in Object.FindObjectsByType<CameraControlConsole>(FindObjectsSortMode.None))
        {
            if (cc.sensorsToDisable != null)
                foreach (var s in cc.sensorsToDisable)
                    Check(cc.name + " has no null sensor entries", s != null);
        }

        int zeroSized = 0;
        foreach (BoxCollider2D b in Object.FindObjectsByType<BoxCollider2D>(FindObjectsSortMode.None))
            if (b.size.x <= 0.001f || b.size.y <= 0.001f) zeroSized++;
        Check("No zero-sized BoxCollider2D", zeroSized == 0);

        Debug.Log("QA_VALIDATION_DONE scene=" + scenePath + " pass=" + pass + " fail=" + fail);
    }
}
