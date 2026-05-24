# MainScene Wiring Checklist (Unity 2022.3 LTS)

Open Unity 2022.3 LTS, then run `Kawaii Starfall -> Build Main Scene` to auto-create and wire the first playable scene/prefabs.

Sprite selection behavior:
- Player sprite: `Assets/Art/player/playerShip1_blue.png`
- Enemy sprite: `Assets/Art/enemies/enemyBlack2.png`
- Bullet sprite: `Assets/Art/projectiles/laserBlue02.png`
- If any configured sprite is missing, the builder falls back to generated placeholders.

If you want to verify or adjust manually, open `Assets/Scenes/MainScene.unity` and check these Inspector assignments:

- GameManager: add `GameManager` script.
- UIManager: add `UIManager` script and assign TMP fields:
  - Score Text, Wave Text, Hearts Text, Weapon Text
  - Start Screen Panel, Title Text (Kawaii Starfall), Subtitle Text (A neon arcade shooter)
  - Game Over Panel
- EnemySpawner: add `EnemySpawner` script and assign Enemy Prefab.
- Player: add `PlayerController` script and assign Bullet Prefab + Fire Point.
- Enemy prefab: add `Enemy`, Collider2D (trigger), Rigidbody2D (kinematic), optional Upgrade Orb prefab.
- Bullet prefab: add `Bullet`, Collider2D (trigger), Rigidbody2D (kinematic).
- Upgrade orb prefab: add `UpgradeOrb`, Collider2D (trigger), Rigidbody2D (kinematic).
- Canvas:
  - Start screen panel with Start Game button -> `UIManager.OnStartGamePressed`
  - HUD TMP texts for Hearts/Score/Wave
  - Game Over panel with Restart button -> `UIManager.OnRestartPressed`

Expected result:
- Scene includes a generated dark starfield background visible during both Start Screen and gameplay.
- Start Screen fades out on Start Game while keeping the background active.
- Play shows start screen first.
- No spawn/shooting until Start Game is clicked.
- Start Game hides start screen and begins Wave 1.
