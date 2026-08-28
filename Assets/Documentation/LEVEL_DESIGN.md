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

Theme: a central reactor core dominates a big chamber, with a walkway above
and a maintenance alcove below — visually the most different floor plan yet,
and (after its gameplay rebuild) the strongest single power-routing puzzle
in the campaign. **New mechanic: unstable power** (`UnstableCircuit` — a
`PoweredObject` that shuts itself off, with a red/yellow flicker warning, a
fixed number of seconds after being powered; the walkway's window is a
generous 24s, not a reflex-test).

Route: Control Room A powers the reactor door → Reactor Chamber (one
`Conductor` patrols the ring, one camera watches it) → either ride the
24s walkway window up to the Generator Room, or duck into the Maintenance
Alcove (always open) and power its fan to vent-shortcut there directly,
bypassing the ring's camera/patrol entirely. The vent is bidirectional and
untimed, so once discovered it's also the permanent way back down — no
need to re-cross the walkway a second time. A Secondary Power Node in the
Generator Room controls the chamber's optional lighting (`DarknessController`
+ a `ReactorLightPower` sink), a real budget choice once other systems are
already drawing power. `maxPower` is 4: the closing state (reactor door 1 +
exit door 1 + elevator 2 = 4) leaves no room for anything left on from
earlier, so finishing requires consciously reclaiming power.

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
reach the west wing's card. A second, optional vent connects Restricted Lab
Entry directly to a corner of Secure Archive next to Authorization Card B, a
pure stealth bypass around both the lab's patrol and the archive's
camera+patrol.

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
restore rooftop access; leads to `RooftopScene`. An optional vent connects
Power Junction directly into the Laser Vault, letting a stealthy player skip
the Surveillance Corridor's camera+patrol and the laser decision entirely.

## RooftopScene

Theme: night, wind, a dim blue-tinted mood overlay. Deliberately the smallest
map in the game, but built as a proper staged finale rather than a plain
run: two rooms (a long Roof Run, then the Helicopter Pad), eight obstacles
(AC units, vent stacks, a satellite dish, crate clusters) for cover and
line-of-sight breaks without ever blocking the direct path to the
helicopter, and four enemies. `TheConductor_Final` is active immediately
(a map-spanning detection range, "already behind you" from the first
beat); three more (`RoofA`/`RoofB`/`HeliPad`) sit further along the route
with realistic detection ranges (10, not the original's 60) and engage
progressively through ordinary distance+line-of-sight as the player
advances, rather than a scripted spawner — no enemy starts in front of the
player. Two `ScriptedExplosion` zones (particle burst + drifting smoke,
offset from the walking line) add cinematic pressure at two points along
the run; two ambient spark emitters and three pulsing emergency lights add
atmosphere. Two hiding spots remain as an escape valve. No power system on
this floor — it's a pure chase-and-escape beat.

Reaching the helicopter's `EscapeTrigger` fires `RooftopSequence` (only
once - guarded, no duplicate trigger/coroutine risk): every `EnemyAI` and
the player's own collider are disabled the instant extraction starts (so
nothing can kill the player mid-sequence and race the win), the player
walks to the helicopter, its `HelicopterIdle` hover/bob is disabled so the
liftoff movement doesn't fight it for control of the transform, it lifts
off, the screen fades to black, `WinScene` loads. The helicopter itself is
a modular `Body`/`TailBoom`/`MainRotor`/`TailRotor` composition (a spinning
rotor bar over a simple fuselage) rather than the original flat placeholder
square — gameplay logic (`RooftopSequence`) stays independent of it, so the
visual can be swapped for real art without touching a script.
