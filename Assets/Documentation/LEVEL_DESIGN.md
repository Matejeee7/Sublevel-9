# SUBLEVEL 9 — Level Design

Each entry lists the floor's theme, its floor-plan shape, what mechanic it
introduces or emphasizes, its objective, and its power budget. Power budgets
are always solvable using only the base `maxPower` — a `GeneratorUpgrade` is
present on most floors but is a non-blocking reward, never a hard requirement,
unless stated otherwise.

## TutorialLevel

Teaches movement, interaction (E), powered doors, the power budget, terminals,
darkness/lighting, enemy danger, patrol behavior, hiding, and the elevator, in
isolation and in a small space. Deliberately simple — no combination of
systems yet.

## Level1 — Office Sector

Theme: corporate office floor (reception, open office, storage, security
office, elevator hall). The baseline floor plan every later level is built to
look nothing like. Systems: power terminals, a keycard door, one patrol enemy,
one locker, room darkness.

Level1 is also the project's real-asset visual prototype: floor and wall
tiles, the player, the enemy, terminal visuals, and Storage/Security room
decoration all use the sci-fi-facility-asset-pack (see
`THIRD_PARTY_ASSETS.md`) instead of the tinted-square placeholder every
other level still uses. Room layout, colliders, and every gameplay
reference are unchanged - only new decoration/visual child objects were
added. Levels2-9, Tutorial, and Rooftop's environment were deliberately
left on the placeholder pending approval of this prototype.

## Level2 — Research Laboratories

Theme: decontamination and lab spaces (arrival, decon, central lab,
observation control, specimen lab, generator lab). **New mechanic: security
cameras** with a real raycast-clipped field-of-view cone (stops at walls,
turns red when it sees you). A `CameraControlConsole` can disable the whole
camera network. Objective: restore power, navigate past the cameras, reach
Observation Control, get the access card, reach the elevator.

## Level3 — Security Operations Floor

Theme: a security command center. **New mechanic: full alarm-state system**
(`AlarmController`: Normal / Suspicious / Alert). A hub-and-spoke floor plan —
`CentralCommand` is a real 4-way junction (Lobby / Surveillance / Armory /
Elevator), and the Armory → Emergency Response → Detention → Corridor → Lobby
chain is an actual loop back to spawn, not a dead end. Cameras and a
`SecuritySensor` raise the alarm; an `AlarmResetConsole` clears it; Alert state
temporarily boosts enemy detection range. Objective: reach the elevator;
`BackupGenerator` is an optional detour down the Armory branch.

## Level4 — Medical / Biological Wing

Theme: clinical corridors, patient ward, operating room, quarantine chamber,
medical storage. **New mechanic: ventilation shortcuts** (`VentPassage`,
gated by a `fanControl` `PoweredObject` — power it down at the Operating Room
terminal to make the crawl-through safe). The vent is the only way to reach
the Quarantine Chamber without the keycard that's found there, matching the
spec's "restore power → fan stops → crawl through → emerge behind the locked
door" flow exactly. Objective: reach the elevator past the locked Corridor B
(bypassable via vent) and the Quarantine camera.

## Level5 — Storage & Logistics

Theme: one large warehouse hall with shelving as interior cover, rather than
many small rooms — deliberately large-feeling per the spec. **New mechanic:
pushable crates + a pressure plate** (`PushableCrate`, `PressurePlate`). A
crate pushed onto the plate holds the Storage Cage door open (no timing
pressure — it stays open as long as the crate does); a second free crate sits
near the watched lane as optional camera cover. Objective: reach Shipping
Control (keycard-gated) and the elevator.

## Level6 — Reactor / Power Facility

Theme: a central reactor core dominates a big chamber, with an optional
walkway above and a dead-end maintenance alcove below — visually the most
different floor plan yet. **New mechanic: unstable power**
(`UnstableCircuit` — a `PoweredObject` that shuts itself off, with a
red/yellow flicker warning, a fixed number of seconds after being powered).
Applied only to the optional walkway leading to the generator branch; the
mandatory route to the elevator has no timer at all, so the level can never
become unwinnable from a missed window.

## Level7 — AI Core / Data Center

Theme: a geometric zigzag of server corridors with one dead-end detour.
**New mechanics: laser barriers + access codes.** `LaserBarrier` inverts
`PoweredObject`'s usual meaning — the barrier is dangerous *while powered*,
so a terminal on the safe side lets you shut it down before crossing. It
starts pre-activated, genuinely consuming power from the level's budget until
disabled. `CodeClueTerminal` teaches an access code (reusing
`InventoryManager`'s keycard-id set) via a readable, re-readable maintenance
log — no number-entry guessing, the "code" is just knowledge the player has
picked up. Objective: read the log in the Cooling Room detour, disable the
laser, reach the code-locked AI Core Chamber, then the elevator.

## Level8 — Executive / Restricted Research

Theme: two symmetric wings off a central lobby — cleaner, more expensive
office spaces than any prior floor. **No new mechanic** — per the spec, this
floor's difficulty comes purely from *combining* everything built so far:
alarm + camera (west wing), a laser (east wing), three patrolling enemies, and
**multi-stage authorization**. The vault needs both Authorization Card A and
B, built from two ordinary `KeycardDoor`s in series (no new "needs two
credentials" code was needed). A third `KeycardDoor`, keyed to Card A, gates a
direct shortcut between the two wings' far ends — opens only once you've
cleared the east wing, letting you skip re-crossing its laser and enemy to
reach the west wing's card.

## Level9 — Final Internal Floor

Theme: the largest interior map, structured as a deliberate pacing spine —
safe → puzzle → tension → stealth → brief relief → final objective → escape —
rather than every room containing every mechanic. Combines every system from
Level3–8: `AlarmController`, two cameras, two `LaserBarrier`s, two
`UnstableCircuit`s (one an optional generator detour, one gating the actual
finale), dual keycard authorization, five patrolling enemies, two hiding
spots, and a permanently dim `StealthWing`. The rooftop access door is itself
an `UnstableCircuit` — power it and reach the exit within the window, a
deliberate callback to Level6 for the finale's climactic beat. Objective:
restore rooftop access; leads to `RooftopScene`.

## RooftopScene

Theme: night, wind, a dim blue-tinted mood overlay. Deliberately the smallest
map in the game, but built as a two-stage finale rather than a plain run.

**Stage 1 - stealth/evasion.** Roof Run has eight obstacles (AC units, vent
stacks, a satellite dish, crate clusters) for cover, two hiding spots, and
four enemies: `TheConductor_Final` is active immediately (a map-spanning
detection range, "already behind you" from the first beat); three more
(`RoofA`/`RoofB`/`HeliPad`) have realistic detection ranges and engage
progressively via ordinary distance+line-of-sight as the player advances.
The Helicopter Pad is physically walled off behind a `RooftopBarrier` at
the only gap between the two rooms - the helicopter cannot be reached yet.

**Stage 2 - combat finale.** A `WeaponPickup` (a real weapon sprite, see
`THIRD_PARTY_ASSETS.md`) sits early in Roof Run. Picking it up enables
`PlayerWeapon`, a mouse-aimed hitscan handgun, and flips the objective to a
live "Eliminate rooftop security — N / 4" counter. The player is still
fully vulnerable to enemy contact during this stage - `EnemyHealth` only
removes an enemy's ability to fight back once it is dead. Once all 4 are
eliminated, `RooftopCollapseSequence` runs a short scripted sequence
(warning flash, Cinemachine-driven camera shake, two explosions, the
barrier sliding open) and restores the "reach the helicopter" objective.

Reaching the helicopter's `EscapeTrigger` fires the existing
`RooftopSequence` (unchanged, still guarded to run once): every `EnemyAI`
and the player's own collider are disabled instantly, so nothing can kill
the player during the walk/lift-off/fade - only the combat stage before it
is genuinely dangerous. Player walks to the helicopter, it lifts off, the
screen fades to black, `WinScene` loads. No power system on this floor -
it's a pure evasion-then-combat escape beat.
