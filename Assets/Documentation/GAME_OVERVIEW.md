# SUBLEVEL 9 — Game Overview

## Concept

You wake up trapped on a lower floor of a high-security research facility after
a catastrophic incident. Power is failing throughout the building. To reach
the rooftop and escape by helicopter, you must climb nine floors, each more
dangerous than the last, while managing a limited electrical supply, avoiding
security systems, and evading a hostile patrolling entity known as
**The Conductor**.

Genre: 2D top-down sci-fi horror / stealth / puzzle. Single player.

## Controls

| Action | Key |
|---|---|
| Move | WASD / Arrow Keys |
| Interact | E |
| Pause / Close menu / Close settings | Esc |
| Toggle a power terminal's connected system | 1–9 (while a terminal is open) |

## Core Gameplay Loop

Each floor is a self-contained puzzle box:

1. **Explore** the floor layout to find power terminals, keycards, and the
   objective needed to reach the elevator.
2. **Manage power.** Every floor has a limited electricity budget
   (`PowerManager.maxPower`). Doors, lights, and the elevator itself all cost
   power to run, and you usually cannot power everything at once — you must
   decide what stays on and what gets toggled off once you're past it.
3. **Avoid detection.** Security cameras and patrolling enemies will notice
   you if you're not careful. Getting spotted has consequences ranging from a
   camera alerting nearby security to a direct enemy chase — but hiding in a
   locker (`HidingSpot`) always breaks detection, camera or enemy alike.
4. **Reach the elevator** and ride it to the next floor.

Nine floors, each introducing (and then continuing to use) one additional
system, escalate this loop from a simple locked door to a floor that combines
every mechanic in the game at once.

## Progression

```
StartScene -> TutorialLevel -> Level1 -> Level2 -> ... -> Level9
           -> RooftopScene -> WinScene
```

`GameOverScene` is reachable from any gameplay scene (player death) and its
Retry button reloads whatever level you died on, not the beginning of the
game — see `GameProgress.cs`.

## Menus

- **Start Menu**: Play, Tutorial, Settings, Quit.
- **Pause Menu** (Esc during gameplay): Resume, Settings, Main Menu. Every
  gameplay scene has one.
- **Settings** (from either menu): Master/Music/SFX volume, Fullscreen —
  persisted via PlayerPrefs, applied immediately.
- **Game Over**: Retry (reloads the current level) or Main Menu.
- **Win**: shown after the rooftop escape sequence completes.

See `LEVEL_DESIGN.md` for what each floor introduces and `SYSTEMS.md` for how
the underlying scripts work.
