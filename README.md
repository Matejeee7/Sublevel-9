# Sublevel 9

**Sublevel 9** is a 2D top-down stealth and puzzle game developed in **Unity 6** as a university project.

The player must escape a nine-level underground facility by avoiding security guards, managing limited power, solving environmental puzzles, and using different systems introduced throughout the facility.

## Gameplay

Each level introduces new mechanics and combines them with systems from previous levels. The main focus is on stealth, exploration, resource management, and environmental puzzles.

### Features

- 9 main facility levels and a tutorial
- Guard AI with detection, chasing, searching, and returning to patrol
- Hiding system using lockers
- Limited power management system
- Power terminals and generators
- Security cameras and alarm states
- Ventilation passages
- Pushable crates and pressure plates
- Timed and unstable power circuits
- Laser barriers and access codes
- Keycards and multi-stage authorization
- Interactive objectives and HUD
- Pause, settings, game over, and retry systems
- Rooftop combat and extraction finale
- Background music and UI audio

## Level Progression

The game gradually introduces new mechanics:

| Level | Main Focus |
|------|------------|
| Tutorial | Basic movement, interaction, stealth and power |
| Level 1 | Power management and keycards |
| Level 2 | Security systems |
| Level 3 | Alarm states and surveillance |
| Level 4 | Ventilation and alternate routes |
| Level 5 | Pushable crates and pressure plates |
| Level 6 | Timed power systems |
| Level 7 | Laser barriers and access codes |
| Level 8 | Multi-stage authorization |
| Level 9 | Combination of previous mechanics |
| Rooftop | Combat and extraction finale |

## Controls

| Input | Action |
|------|--------|
| WASD | Move |
| E | Interact |
| Esc | Pause / close interface |
| Mouse | Aim during rooftop combat |
| Left Mouse Button | Shoot during rooftop combat |

## Power System

Power is a limited resource used to activate different systems throughout the facility.

The HUD displays the currently available power and the maximum power capacity:

`POWER: available / maximum`

Generators and other mechanics can modify the available capacity, requiring the player to decide which systems should receive power.

## Enemy AI

Security guards can:

- Patrol designated areas
- Detect the player using line of sight
- Chase detected players
- Investigate and search after losing the player
- Lose track of a player hiding in a locker
- Return to their original area and resume patrol

## Rooftop Finale

After escaping the underground facility, the player reaches the rooftop for the final encounter.

The player acquires a weapon, fights the remaining guards, survives the collapsing rooftop sequence, and reaches the helicopter extraction point.

## Technology

- **Engine:** Unity 6
- **Language:** C#
- **Rendering:** Universal Render Pipeline (2D)
- **Platform:** Windows
- **Version Control:** Git

## Project Structure

The repository contains the complete Unity project.

To open the project:

1. Clone the repository.
2. Open Unity Hub.
3. Select **Add project from disk**.
4. Select the cloned project folder.
5. Open the project using the appropriate Unity 6 version.
6. Open `StartScene` and press Play.

Unity-generated folders such as `Library`, `Temp`, `Logs`, and build output are excluded from the repository.

## Screenshots

Screenshots can be added here to showcase gameplay, levels, and the rooftop finale.

<!--
Example:

![Main Menu](Screenshots/main-menu.png)
![Gameplay](Screenshots/gameplay.png)
![Rooftop](Screenshots/rooftop.png)
-->

## Author

**Matej P.**

Software Engineering student at FERIT Osijek.

## About

Sublevel 9 was developed as a university game development project, with a focus on implementing and combining multiple gameplay systems within a complete playable game.
