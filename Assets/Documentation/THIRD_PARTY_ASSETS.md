# SUBLEVEL 9 — Third-Party Assets

No external art, audio, or font assets have been added to the project.

Every visual in every level (walls, floors, doors, terminals, cameras,
enemies, keycards, crates, the helicopter, etc.) is the single placeholder
sprite `Assets/Art/Square.png`, tinted and scaled per object. This is
intentional groundwork, not an oversight: gameplay code never reads from
this sprite directly (`PoweredObject`, `EnemyAI`, `SecurityCamera`, and every
other script only touch `SpriteRenderer.color`/`enabled`, never the sprite
itself), so every placeholder can be swapped for real art asset-by-asset
without touching a single script.

UI controls (sliders, toggles) use Unity's own built-in editor UI skin
resources (`UI/Skin/*.psd`, shipped with the Unity Editor itself — not a
third-party download), which is why they're functional but visually plain.

## What still needs sourcing

If/when real assets are added, record them here with: asset name, creator/
source, license, and where used. At minimum this project would benefit from:

- A player sprite + Idle/Walk animation (spec section 21).
- An enemy ("The Conductor") sprite with a readable silhouette.
- A tileset for walls/floors distinguishing room types visually instead of
  through flat color tinting.
- Icons or sprites for terminals, cameras, keycards, crates, the generator,
  and the helicopter.
- Ambient/facility hum, door open/close, terminal interaction, generator
  start, camera movement/detection warning, alarm, enemy alert, elevator, and
  rooftop wind/helicopter SFX, plus a music track or two.

`GameSettings.cs` already has Music/SFX volume fields ready for whatever
`AudioManager` eventually reads them — no audio integration work is blocked
on this document, only the actual sourcing and licensing of clips.

Only use assets you have the right to redistribute (a permissive open
license, or assets you created yourself), and keep this file up to date as
assets are added so the license terms stay traceable.
