TOWER GAME v0.2 - UNITY 6 / URP
================================

WHAT IS NEW
-----------
- Live tower placement preview follows the mouse.
- Green preview = valid position; red = blocked / not enough money.
- Towers cannot be placed too close to another tower, the road, or the base.
- Real projectile GameObjects fly from tower muzzle to enemies.
- Tower head rotates toward its current target.
- Every enemy has a world-space HP bar.
- Updated HUD with coins, base HP, wave counter and build hint.
- Victory and Game Over overlay.
- Wave scaling remains automatic.

INSTALL OVER v0.1
-----------------
1. Stop Play Mode and close Unity.
2. Back up your current project if you made your own changes.
3. Copy this package's Assets folder into your Unity project root.
4. Allow Windows to replace the existing Assets/Scripts files.
5. Reopen the project and wait for compilation.
6. Open an empty scene and press Play.

CONTROLS
--------
Move mouse over the ground: tower preview.
Left click: build tower for 100 coins.

NOTES
-----
- The starter tower is free.
- Starting money: 300.
- The entire demo scene is generated from scripts at runtime.
- No prefabs, models or inspector wiring are required.
- The road is intentionally blocked for tower placement.

FILES
-----
Enemy.cs
EnemyHealthBar.cs
EnemySpawner.cs
GameBootstrap.cs
GameHUD.cs
GameManager.cs
Projectile.cs
Tower.cs
TowerFactory.cs
TowerPlacement.cs

NEXT VERSION (recommended)
--------------------------
- 3 tower classes: Machine Gun / Cannon / Slow.
- Tower selection panel and upgrades.
- Multiple enemy classes and a boss.
- Proper Canvas UI instead of OnGUI.
- SFX, VFX, hit effects and simple environment art.
