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

        MultiPowerTerminal[] allTerminals = Object.FindObjectsByType<MultiPowerTerminal>(FindObjectsSortMode.None);

        foreach (WinZone w in Object.FindObjectsByType<WinZone>(FindObjectsSortMode.None))
        {
            Check(w.name + " has a non-empty nextSceneName", !string.IsNullOrEmpty(w.nextSceneName));

            // A WinZone gated on a PoweredObject is only completable if some
            // terminal in the scene can actually toggle it - this is the
            // class of bug that let Level4/6/7's elevator terminal sit
            // unreachably behind its own door while every reference-validity
            // check above still passed.
            if (w.requiredPowerObject != null)
            {
                bool controlled = false;
                foreach (MultiPowerTerminal t in allTerminals)
                {
                    if (t.poweredObjects == null) continue;
                    foreach (var po in t.poweredObjects)
                        if (po == w.requiredPowerObject) controlled = true;
                }
                Check(w.name + ".requiredPowerObject (" + w.requiredPowerObject.name + ") is controlled by some MultiPowerTerminal", controlled);
            }
        }

        // Every KeycardDoor's required id must be obtainable somewhere in
        // the scene (a KeycardPickup or a CodeClueTerminal granting the
        // same id) - otherwise the door can never be opened at all.
        var obtainableIds = new System.Collections.Generic.HashSet<string>();
        foreach (KeycardPickup kp in Object.FindObjectsByType<KeycardPickup>(FindObjectsSortMode.None))
            obtainableIds.Add(kp.keycardId);
        foreach (CodeClueTerminal cc in Object.FindObjectsByType<CodeClueTerminal>(FindObjectsSortMode.None))
            obtainableIds.Add(cc.codeId);

        foreach (KeycardDoor kd in Object.FindObjectsByType<KeycardDoor>(FindObjectsSortMode.None))
        {
            Check(kd.name + " requiredKeycardId (\"" + kd.requiredKeycardId + "\") is obtainable somewhere in the scene",
                obtainableIds.Contains(kd.requiredKeycardId));
        }

        // Vent pairs must link back to each other and agree on which fan
        // gates the crossing, or one end can strand the player.
        foreach (VentPassage vp in Object.FindObjectsByType<VentPassage>(FindObjectsSortMode.None))
        {
            Check(vp.name + ".linkedExit assigned", vp.linkedExit != null);
            Check(vp.name + ".fanControl assigned", vp.fanControl != null);

            if (vp.linkedExit != null)
            {
                VentPassage other = vp.linkedExit.GetComponentInParent<VentPassage>();
                if (other == null)
                    other = vp.linkedExit.GetComponent<VentPassage>();
                Check(vp.name + ".linkedExit points at a point belonging to another VentPassage", other != null);
                if (other != null)
                {
                    Check(vp.name + " <-> " + other.name + " link is reciprocal (each points back at the other)",
                        other.linkedExit != null && Vector3.Distance(other.linkedExit.position, vp.transform.position) < 0.5f);
                    Check(vp.name + " and " + other.name + " share the same fanControl",
                        other.fanControl == vp.fanControl);
                }
            }
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

        // --- Rooftop combat finale (only relevant when these objects
        // exist, so this never affects Levels1-9 or Tutorial). ---
        PlayerWeapon weapon = Object.FindFirstObjectByType<PlayerWeapon>();
        RooftopCollapseSequence collapse = Object.FindFirstObjectByType<RooftopCollapseSequence>();
        if (weapon != null || collapse != null)
        {
            Check("PlayerWeapon exists", weapon != null);
            if (weapon != null)
            {
                Check("PlayerWeapon.weaponPivot assigned", weapon.weaponPivot != null);
                Check("PlayerWeapon.weaponVisual assigned", weapon.weaponVisual != null);
                Check("PlayerWeapon.weaponVisual starts inactive (cannot shoot before pickup)",
                    weapon.weaponVisual == null || !weapon.weaponVisual.gameObject.activeSelf);
                Check("PlayerWeapon.muzzle (MuzzlePoint) assigned", weapon.muzzle != null);
                Check("PlayerWeapon.maxRange > 0", weapon.maxRange > 0f);
            }

            CombatCrosshair crosshair = Object.FindFirstObjectByType<CombatCrosshair>(FindObjectsInactive.Include);
            Check("CombatCrosshair exists", crosshair != null);
            if (crosshair != null)
                Check("CombatCrosshair.crosshairImage assigned", crosshair.crosshairImage != null);

            RooftopSequence extraction = Object.FindFirstObjectByType<RooftopSequence>();
            Check("Helicopter extraction (RooftopSequence) still exists", extraction != null);

            WeaponPickup[] pickups = Object.FindObjectsByType<WeaponPickup>(FindObjectsSortMode.None);
            Check("Exactly one WeaponPickup exists", pickups.Length == 1);
            foreach (var p in pickups)
                Check(p.name + ".targetWeapon assigned", p.targetWeapon != null);

            EnemyHealth[] allHealth = Object.FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
            Check("Exactly 4 Rooftop enemies have EnemyHealth", allHealth.Length == 4);

            Check("RooftopCollapseSequence exists", collapse != null);
            if (collapse != null)
            {
                Check("RooftopCollapseSequence.enemies has 4 entries", collapse.enemies != null && collapse.enemies.Length == 4);
                Check("RooftopCollapseSequence.blocker assigned", collapse.blocker != null);
                if (collapse.blocker != null)
                {
                    Collider2D blockerCol = collapse.blocker.GetComponent<Collider2D>();
                    Check("Barrier starts with its collider enabled (cannot open early)",
                        blockerCol != null && blockerCol.enabled && !blockerCol.isTrigger);
                }
                Check("RooftopCollapseSequence.cameraShake assigned", collapse.cameraShake != null);
                Check("RooftopCollapseSequence.finalObjective is non-empty", !string.IsNullOrEmpty(collapse.finalObjective));

                if (collapse.enemies != null)
                {
                    foreach (var e in collapse.enemies)
                        Check("RooftopCollapseSequence has no null enemy entries", e != null);
                }
            }
        }

        Debug.Log("QA_VALIDATION_DONE scene=" + scenePath + " pass=" + pass + " fail=" + fail);
    }
}
