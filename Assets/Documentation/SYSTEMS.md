# SUBLEVEL 9 — Systems Reference

All scripts live in `Assets/Scripts/`. This describes what each system does
and how they connect — read the source for exact field names.

## Player

- **PlayerMovement** — reads WASD/arrows, drives a `Rigidbody2D` directly via
  `linearVelocity`. Disabled while a terminal is open or the player is hidden.
- **PlayerInteraction** — detects nearby `Interactable`s via trigger overlap,
  presses E to call `Interactable.Interact()`, and shows/hides the shared
  `InteractionPromptUI` with that object's message.
- **PlayerHealth** — `Die()` records the current scene via `GameProgress` and
  loads `GameOverScene`. The sole death path (enemy contact, laser
  contact-while-blocking is not lethal by itself — only a chasing enemy
  catching you is).
- **PlayerStealth** — `Hide()`/`Unhide()`: teleports the player to a hiding
  spot's point, disables movement/rendering, zeroes velocity. Cameras and
  `EnemyAI` both check `isHidden` before anything else, so hiding is a
  universal escape valve regardless of what's chasing you.
- **InventoryManager** — a single `HashSet<string>` of "keycard" ids. Not
  literally always a physical card — `CodeClueTerminal` (Level7) adds a known
  access code to the same set. `HasKeycard(id)` / `AddKeycard(id)`.

## Power

- **PowerManager** — one per scene (`GameManager` under `Managers`).
  `maxPower` / `usedPower`, `CanUsePower(amount)`, `Add/RemovePowerUsage`.
  Found via `FindFirstObjectByType`, never a serialized reference — this is
  why duplicate `PowerManager`s in a scene are a real bug (fixed in Level1/2;
  the QA validator now catches this class of issue automatically).
- **PoweredObject** — `isPowered`, `powerCost`, `TogglePower()`. By default a
  door: disables its own collider (passable) and tints `poweredColor`/
  `unpoweredColor` when toggled (fields, defaulting to the original
  hardcoded cyan/gray — every level now sets a level-specific `poweredColor`
  on its visible doors for per-floor identity, while every level shares the
  same neutral gray for "off"). Also used as an invisible "virtual power
  sink" (no collider/sprite) for things like an elevator lift or a laser's
  power source — see `PowerLightMask`/`DarknessController` below for other
  things it can drive.
- **PowerTerminal** / **MultiPowerTerminal** — `Interactable`s that toggle one
  or several `PoweredObject`s. `MultiPowerTerminal` opens a shared UI panel
  (one `TerminalPanel` per scene, referenced by every terminal instance),
  locks player movement while open, closes on E or Esc, keys 1–9 toggle the
  Nth connected system.
- **GeneratorUpgrade** — an `Interactable` that raises `PowerManager.maxPower`
  once. Always placed as an optional detour, never required to finish a
  level's critical path.
- **DarknessController** / **PowerLightMask** — purely cosmetic: tie a
  `SpriteRenderer`'s alpha (or a light/glow object's active state) to a
  `PoweredObject.isPowered`. This is what makes "power the lights or stay in
  the dark" a real decision without needing an actual dynamic-lighting system.

## Security

- **SecurityCamera** — sweeps, raycasts against `obstacleMask` (Obstacle +
  VisionBlocker layers) to build both its detection check and its visible
  cone mesh (stops exactly at walls, turns red when it currently sees you).
  On detection: if an `AlarmController` exists in the scene, calls
  `TriggerAlert()`; otherwise (Level2, Level4-7 without one) falls back to
  alerting every `EnemyAI` directly. **Never kills the player itself** — see
  `EnemyAI` for the only lethal contact.
- **CameraControlConsole** — an `Interactable` that disables every
  `SecurityCamera` in the scene, optionally unlocks one `PoweredObject` door,
  and optionally disables a list of `SecuritySensor`s.
- **AlarmController** (Level3, Level8, Level9) — `Normal` / `Suspicious` /
  `Alert`, each with automatic decay back to Normal (so the alarm can never
  trap the player permanently) plus a manual reset via `AlarmResetConsole`.
  `Alert` boosts every `EnemyAI`'s detection range for its duration.
- **SecuritySensor** — a passive trigger that raises `Suspicious` when the
  player walks through it, unless disabled (usually bundled into a
  `CameraControlConsole`'s disable list).
- **AlarmEmergencyLighting** — a red overlay tied to `AlarmController.Alert`,
  the visual half of "alarm changes lighting."
- **LaserBarrier** (Level7-9) — inverts `PoweredObject`'s usual meaning: it
  blocks the path *while its power source is powered*. Still routed through
  an ordinary terminal, just interpreted backwards. Several instances start
  pre-activated (`isPowered = true` set directly, with the level's
  `PowerManager.usedPower` explicitly credited via `AddPowerUsage` at build
  time to keep the ledger accurate).
- **KeycardPickup** / **KeycardDoor** — generic credential system.
  `KeycardPickup.keycardId` is added to the player's `InventoryManager` set on
  trigger; `KeycardDoor.requiredKeycardId` checks it on E. Multiple doors can
  check the same id (Level8's shortcut) or different ids in sequence (Level8's
  vault, Level9's Access Corridor) without any new code.

## Enemies

- **EnemyAI** — a small state machine: `Patrol` / `Chase` / `Investigate` /
  `Search`. `Chase` triggers when the player is within `detectionRange`, not
  hidden (boosted during `AlarmController.Alert`), **and** in actual line of
  sight — a single `Physics2D.Raycast` against `obstacleMask`
  (Obstacle + VisionBlocker), the same principle `SecurityCamera` uses for
  its vision cone, so a closed door or wall blocks detection instead of the
  enemy seeing through it. Losing sight drops to `Search` (go to the last
  known position, wait, give up back to `Patrol`). `Alert(position)` is the
  public hook cameras/alarms use to send an enemy to investigate without a
  direct sighting. **Physical contact with the player (not hidden) is the
  only thing that calls `PlayerHealth.Die()`.**
- **HidingSpot** — E to hide/unhide at a locker; shows the interaction prompt
  for both states.

## Puzzle mechanics

- **PushableCrate** (Level5) — a `Rigidbody2D` + `BoxCollider2D` with sane
  defaults (no gravity, no rotation, damping so it stops when not being
  pushed). Ordinary physics contact with the player does the pushing — no
  custom push logic exists or is needed.
- **PressurePlate** (Level5) — stays active while a `PushableCrate` or the
  player stands on it; unlocks a linked door only while weighted, and stays
  unlocked with nobody attending it once a crate is parked there.
- **VentPassage** (Level4) — a linked pair of trigger points; E teleports the
  player between them, but only while a linked `fanControl` `PoweredObject`
  is powered ("fan stopped").
- **UnstableCircuit** (Level6, Level9) — wraps a `PoweredObject` that
  auto-shuts-off (and frees its power budget) a fixed number of seconds after
  being powered, flashing red/yellow as a warning in its last few seconds.

## Progression / meta

- **WinZone** — a trigger that checks an optional `requiredPowerObject` before
  loading `nextSceneName`.
- **SceneLoader** — menu button targets; every load resets `Time.timeScale`
  to 1 first. `LoadCurrentLevel()` reloads whatever `GameProgress` last
  recorded.
- **GameProgress** — static, PlayerPrefs-backed "last scene the player died
  in," so Game Over's Retry restarts the actual level, not always Tutorial.
- **GameSettings** — static, PlayerPrefs-backed Master/Music/SFX volume and
  fullscreen, applied immediately on change.
- **PauseMenuController** / **SettingsPanelController** /
  **MainMenuSettingsController** — Esc toggles pause and correctly drives
  `Time.timeScale`; the same settings panel construction is reused from both
  the pause menu and the start menu.
- **InteractionPromptUI** — one shared "E - ..." prompt per scene, driven by
  a static `Show(message)` / `Hide()` so any interaction script can use it
  without a per-object UI reference.
- **ObjectiveManager** — one small, persistent objective line per scene
  (top-left, unobtrusive), driven by a static `SetObjective(text)`. No
  per-level subclass and no timers: every level just sets an optional
  `nextObjective` string on the existing pickup/console that should advance
  it (`KeycardPickup`, `GeneratorUpgrade`, `CameraControlConsole`,
  `AlarmResetConsole`, `CodeClueTerminal`), plus `AlarmController.
  alertObjective` for what to show while an alert is active - configured per
  level so it never tells the player to find a console that doesn't exist
  (Level8/9 have no `AlarmResetConsole`, so their alert objective says
  "break line of sight" instead).
- **FeedbackUI** — brief on-screen messages ("Keycard required", "Not enough
  power", "Alarm reset", "Laser disabled", ...), a static `Flash(message)`
  auto-hidden after a couple of seconds. Deliberately a separate UI element
  from both `InteractionPromptUI` and `ObjectiveManager` so the three never
  overwrite each other.
- **RooftopSequence** — the escape cutscene: a plain coroutine (not Timeline),
  guarded to run at most once, that neutralizes every `EnemyAI` and the
  player's own collider the instant it starts (so nothing can kill the
  player mid-sequence and race the win), disables the helicopter's
  `HelicopterIdle` bob before taking over its position, walks the player to
  the helicopter, lifts it off, fades to black via a `CanvasGroup`, and
  loads `WinScene`.

## Visuals

- **Visual composition pattern** — every gameplay object's collider/script
  stays on the original GameObject; anything decorative lives on a child
  (usually named `Visual`) with its own `SpriteRenderer`(s), built from the
  single placeholder sprite `Assets/Art/Square.png` tinted and layered.
  Nothing decorative ever touches a parent's `transform.localScale` (that
  scales the collider too) — see `PlayerStealth`/`RooftopSequence` below for
  why hiding logic had to change to match.
- **DirectionalFacing** — rotates a visual child to face the last direction
  of travel, purely by watching a body transform's position each frame (no
  changes needed to `PlayerMovement`/`EnemyAI`). Used by both the player and
  every enemy's `Visual` child.
- **CameraDisabledIndicator** — purely observational: watches whether the
  parent `SecurityCamera` component is still `enabled` (false once
  `DisableCamera()` fires) and dims the camera's body/lens sprites to match,
  without touching detection logic at all.
- **RotorSpin** / **HelicopterIdle** — continuous rotation for a rotor
  sprite, and a gentle hover/bob for the helicopter while it waits.
  `RooftopSequence` explicitly disables `HelicopterIdle` before its own
  liftoff loop starts writing to the same transform each frame — otherwise
  the two would fight for `helicopter.position`, the exact class of
  same-frame race that also affected `MultiPowerTerminal`'s E-interaction.
- **PulsingLight** — a simple alpha sine-pulse for a `SpriteRenderer`, reused
  for terminal screens, generator indicator lights, specimen tanks, server
  rack status lights, and Rooftop's emergency lights.
- **ScriptedExplosion** — a one-shot trigger zone (Rooftop only) that plays
  a paired burst + smoke `ParticleSystem` once when the player crosses it.
  Never damages the player — pure cinematic pressure.
- **PlayerStealth**/**RooftopSequence** player-hide note — both used to call
  `GetComponent<SpriteRenderer>()` directly on the player root; now
  `GetComponentsInChildren<SpriteRenderer>(true)`, so hiding/the helicopter-
  entry walk still fully hides the player now that its visual lives on a
  child `Visual` object instead of the root.

## Audio

- **SfxPlayer** — one shared instance per scene (the `InteractionPromptUI`/
  `FeedbackUI` "static access" pattern), a single `AudioSource` for one-shot
  playback. `SfxPlayer.Play(clip)` scales by `GameSettings.SfxVolume` and is
  a safe no-op if `clip` is null — which it is everywhere right now; no audio
  assets exist yet (see `THIRD_PARTY_ASSETS.md`). Every category the task
  scope covers has an `AudioClip` field wired at its trigger point (door
  power, terminal open/close, keycard pickup, alarm, camera detection, enemy
  alert, vent entry, pressure plate, generator, a generic feedback/objective
  cue via `FeedbackUI.Flash`, and Rooftop's explosions) - dropping a clip
  into any of them in the Inspector makes it play, nothing else to change.
- **AmbiencePlayer** — one looping music/ambience bed per scene (also one
  per gameplay scene already), continuously re-scaled by
  `GameSettings.MusicVolume` so a live settings change takes effect
  immediately. No clip assigned yet on any instance.
