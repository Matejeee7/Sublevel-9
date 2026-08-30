# SUBLEVEL 9 — Third-Party Assets

## sci-fi-facility-asset-pack

- **Source**: `Assets/Art/sci-fi-facility-asset/sci-fi-facility-asset-pack/`
- **Author**: Murphy's Dad (https://murphysdad.itch.io/), pack page
  https://murphysdad.itch.io/sci-fi-facility
- **License**: CC0, exactly as stated in the pack's own `README.txt`:
  "TL;DR: use this however you like! Crediting to Murphy's Dad is
  appreciated but not necessary."
- **Used in**: Level1 only, this pass (see below). Not yet used anywhere
  else in the project.

Sprites used:

| File | Cell used | Where |
|---|---|---|
| `tileset.png` (4x5 grid, 16px cells) | `tileset_0_4` (floor tile) | Level1 floor, every room |
| `tileset.png` | `tileset_2_0` (wall strip) | Level1 wall visual overlay, rotated per segment |
| `inspector_spritesheet.png` (4x8 grid) | `_0_0` (down), `_0_2` (side, flipped for the opposite direction) | Level1 player |
| `guard_orange_spritesheet.png` (4x7 grid) | `_0_0` (down), `_0_2` (side, flipped) | Level1 enemy (The Conductor) |
| `computer_spritesheet.png` (2x1 grid) | `_1_0` | Level1 terminal visuals (all 4) |
| `crates_spritesheet.png` (3x5 grid) | `_0_3`, `_1_3` | Level1 Storage Room decoration (no collider) |
| `computer_screen_large.png` (single sprite) | whole image | Level1 Security Office decoration (no collider) |
| `blowdart_spritesheet.png` (2x1 grid) | `_0_0` | Rooftop finale weapon pickup + equipped visual |

Import settings applied to every sheet above: Sprite (2D and UI), Pixels
Per Unit 16 (matches the pack's native 16px tile size 1:1 to a Unity unit),
Point filtering, Multiple slice mode sliced into 16x16 cells (`computer_
screen_large.png` uses Single mode instead, it is not a sheet), no
compression, no mipmaps. Only the `.meta` import settings were changed —
the original PNGs are untouched.

**Not used** (present in the pack but no fitting use found or out of this
pass's scope): `button_*_spritesheet`, `computer_popup_spritesheet`,
`dash_bar_spritesheet`, `doodads_spritesheet`, `giblet_variations`,
`guard_notice.png`, `guard_white_spritesheet`, `guard_yellow_spritesheet`,
`inmate_*_spritesheet`, `keyboard_indicators_spritesheet`,
`lasers_spritesheet`, `mouse_button_indicators_spritesheet`,
`noisemaker_*_spritesheet`, `orb_*_spritesheet`, `portal_spritesheet`,
`shadow.png`, `smoke_icon_spritesheet`, `the_kid_spritesheet`.

**No suitable asset found for** (left on the existing placeholder, per
the task's explicit instruction not to fabricate substitutes):
- **Generator** — no machine/power-console sprite exists in either pack;
  `computer_screen_large.png` is a monitor, not a generator, so it was
  used for Security Office decoration instead, not mislabeled as one.
- **Keycard pickup** — no card/badge/chip sprite exists in either pack.
- **Hiding locker** — no locker/cabinet sprite exists in either pack.
- **Doors** — no door/gate sprite distinct from the plain wall segments
  exists; the existing `PoweredObject`-driven colored sprite already
  communicates open/closed state clearly, so it was left as-is rather
  than force an ambiguous wall-segment reuse.

**Weapon note**: no literal handgun/pistol sprite exists in either pack.
`blowdart_spritesheet.png` (a real, dedicated ranged-weapon asset from the
pack — a held, aimable blowpipe) was used as the closest available
substitute for the Rooftop finale's weapon, disclosed here rather than
silently relabeled as a "handgun."

## Kenney UI Pack: Sci-fi (2.0)

- **Source**: `Assets/Art/kenney-asset-pack/` (top-level `License.txt`
  inside `Assets/Art/` is this pack's license file)
- **Author/distributor**: Kenney (www.kenney.nl), creation date
  19-08-2024, per the included `License.txt`
- **License**: CC0 (Creative Commons Zero), exactly as stated in the
  included license file: "free to use in personal, educational and
  commercial projects. Support us by crediting Kenney (not mandatory)."
- **Used in**: not used in this pass. Per the task's explicit instruction,
  this pack is UI-oriented (bars, buttons, panels, cursors, crosshairs)
  and was reserved for a future UI pass rather than pressed into service
  as fake environment art.

## Square.png (existing placeholder)

Still the only visual for Tutorial, Level2–Level9, Rooftop's environment/
enemies/player, and every Level1 object with no suitable real-asset match
(see above). Gameplay code never reads from the sprite itself
(`PoweredObject`, `EnemyAI`, `SecurityCamera`, etc. only touch
`SpriteRenderer.color`/`enabled`), so it remains freely swappable
asset-by-asset without touching a script.

## Audio

Still none. `Assets/Audio/` is empty; neither imported pack contains audio.
`GameSettings.cs`'s Music/SFX volume fields remain ready for whatever
system eventually reads them — no work is blocked on this document, only
sourcing and licensing actual clips.

Only use assets you have the right to redistribute (a permissive open
license, or assets you created yourself), and keep this file up to date as
assets are added so the license terms stay traceable. Do not claim a
license beyond what a pack's own included documentation states.
