# Kawaii Starfall - Unity 2D Foundation Setup

This repository now contains the starter gameplay scripts for a brand-new standalone Unity 2D project:

- Project name: **Kawaii Starfall**
- Recommended local Unity project path: `Documents/UnityProjects/Kawaii_Starfall`
- Main scene target path: `Assets/Scenes/MainScene.unity`

## Folder structure to create in Unity

- Assets/Scenes
- Assets/Scripts
- Assets/Prefabs
- Assets/Art/Placeholders
- Assets/Art/Characters
- Assets/Art/Enemies
- Assets/Art/Projectiles
- Assets/Audio/Music
- Assets/Audio/SFX
- Assets/UI
- Assets/Materials
- Assets/ScriptableObjects
- Assets/Animations

## Required scripts (already provided)

- PlayerController.cs
- Bullet.cs
- Enemy.cs
- EnemySpawner.cs
- GameManager.cs
- UpgradeOrb.cs
- UIManager.cs

## MainScene construction checklist

1. Create a new Unity **2D Core** project named `Kawaii_Starfall`.
2. Copy this repo's `Assets/Scripts` folder into that project.
3. Create `Assets/Scenes/MainScene.unity`.
4. Add a Camera at `(0,0,-10)` and set orthographic size around 5.
5. Create placeholder sprites/shapes:
   - Player ship (bottom center)
   - Enemy blob prefab
   - Bullet prefab
   - Upgrade orb prefab
6. Add required components:
   - 2D colliders and rigidbodies (kinematic where appropriate)
   - Tags/layers as needed for collisions
7. Add empty GameObjects:
   - `GameManager`
   - `UIManager`
   - `EnemySpawner`
8. Attach scripts and wire inspector references.
   - `GameManager` GameObject:
     - `GameManager` component only (set `Starting Hearts` to 3 unless you are testing).
   - `UIManager` GameObject:
     - Assign `Score Text`, `Wave Text`, `Hearts Text`, `Weapon Text`.
     - Assign `Start Screen Panel`, `Title Text`, `Subtitle Text`.
     - Assign `Game Over Panel`.
   - `EnemySpawner` GameObject:
     - Assign `Enemy Prefab`.
   - Player ship GameObject:
     - Assign `Bullet Prefab` and `Fire Point` on `PlayerController`.
9. Add Canvas + TextMeshProUGUI objects for:
   - Score
   - Wave
   - Hearts
   - Weapon level
10. Add a Start Screen panel with:
   - Title TextMeshProUGUI text: `Kawaii Starfall`
   - Subtitle TextMeshProUGUI text: `A neon arcade shooter.`
   - Start Game button hooked to `UIManager.OnStartGamePressed`
11. Add a Game Over panel with restart button hooked to `UIManager.OnRestartPressed`.
12. Wire `UIManager` references for start screen/title/subtitle and game-over panel.
13. Save scene as `Assets/Scenes/MainScene.unity`.

## Stability checks before pressing Play

- Confirm Start Screen panel is active by default (or shown by `UIManager.Start`).
- Confirm Start Game button calls `UIManager.OnStartGamePressed`.
- Confirm Restart button on Game Over panel calls `UIManager.OnRestartPressed`.
- Confirm no `UIManager` inspector reference fields are left unassigned.

This keeps the project clean and modular for expansion into a full arcade shooter.
