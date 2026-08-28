# SUBLEVEL 9 — Third-Party Assets

No external art, audio, or font assets have been added to the project.

Every visual in every level is still built from the single placeholder
sprite `Assets/Art/Square.png` — nothing else has been imported. The final
visual pass turned this from flat colored squares into small compositions
of that same sprite (a player/enemy body + a facing accent; a camera body +
lens; a terminal + screen; a door + emitter nodes; desks, server racks,
medical beds, and the other per-level signature props are all 2-3 tinted
rectangles each), but it is still one sprite, tinted, scaled, and layered —
not new art. This remains intentional groundwork: gameplay code never reads
from the sprite itself (`PoweredObject`, `EnemyAI`, `SecurityCamera`, and
every other script only touch `SpriteRenderer.color`/`enabled`), and every
new visual composition lives on its own child GameObject separate from the
gameplay object it decorates, so any of it can be swapped for real art
asset-by-asset without touching a script or a collider.

UI controls (sliders, toggles) use Unity's own built-in editor UI skin
resources (`UI/Skin/*.psd`, shipped with the Unity Editor itself — not a
third-party download), which is why they're functional but visually plain.

## Audio

Still none. `Assets/Audio/` is empty. The final polish pass added the
*playback infrastructure* (`SfxPlayer`, `AmbiencePlayer`, one of each per
gameplay scene) and wired an `AudioClip` field + a guarded play call into
every category below that maps onto an existing script, but every field was
left unassigned — no clip exists to attach, and none should be invented or
misattributed. A clip dropped into any of these fields in the Inspector
works immediately; nothing else needs to change.

| Category | Hook | Status |
|---|---|---|
| Door power on/off | `PoweredObject.powerOnClip` / `.powerOffClip` | missing |
| Terminal open/close | `MultiPowerTerminal.openClip` / `.closeClip` | missing |
| Keycard pickup | `KeycardPickup.pickupClip` | missing |
| Alarm trigger/reset | `AlarmController.alertClip` / `.resetClip` | missing |
| Camera detection | `SecurityCamera.detectionClip` | missing |
| Enemy alert | `EnemyAI.alertClip` | missing |
| Vent entry | `VentPassage.enterClip` | missing |
| Pressure plate activate/release | `PressurePlate.activateClip` / `.releaseClip` | missing |
| Generator activation | `GeneratorUpgrade.activateClip` | missing |
| Objective/feedback cue | `FeedbackUI.cueClip` (all `FeedbackUI.Flash()` messages) | missing |
| Explosion (Rooftop) | `ScriptedExplosion.explosionClip` | missing |
| Helicopter rotor loop | `HelicopterIdle.rotorLoopClip` (creates its own looping AudioSource only if assigned) | missing |
| Music/ambience (per scene) | `AmbiencePlayer.ambienceClip` on the scene's `AmbiencePlayer` object | missing |
| UI click | not wired — StartScene/PauseMenu UI is the project owner's own in-progress work, left untouched this pass | missing |
| Rooftop wind | use the Rooftop scene's `AmbiencePlayer` | missing |

`GameSettings.cs`'s Music/SFX volume fields are now actually consumed:
`SfxPlayer` scales every one-shot by `SfxVolume`, `AmbiencePlayer` scales its
loop by `MusicVolume` every frame (so a live settings change takes effect
immediately, matching the sliders' existing behavior). No audio integration
work is blocked on this document — only sourcing and licensing real clips.

## What still needs sourcing

- Real art for the player, "The Conductor" enemy, doors, terminals, cameras,
  vents, lasers, keycards, generators, crates, the helicopter, and a wall/
  floor tileset per level — see the visual composition note above for where
  each one's child GameObjects live.
- Every audio clip in the table above.

Only use assets you have the right to redistribute (a permissive open
license, or assets you created yourself), and keep this file up to date as
assets are added so the license terms stay traceable.
