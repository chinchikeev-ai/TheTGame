# TheTroyGame Data Catalog

Authored ScriptableObject assets are the runtime source of truth. Runtime gameplay must fail validation when authored balance data is missing.

## TowerData
Path: `Assets/Resources/Data/Towers`
Owner: `Assets/Game/Towers/TowerData.cs`

Assets:
- `MachineGun.asset` — Archer Tower
- `Cannon.asset` — Ballista
- `Slow.asset` — Priests of Apollo
- `SpearThrower.asset` — Spear Wall
- `FireTower.asset` — Fire Tower
- `TrojanGuard.asset` — Trojan Guard

## EnemyData
Path: `Assets/Resources/Data/Enemies`
Owner: `Assets/Game/Enemies/EnemyData.cs`

Expected archetypes:
- Infantry
- Runner
- HeavyHoplite
- ShieldBearer
- Archer
- BatteringRam
- Boss (`menelaus` in Chapter I)

## WaveData
Path: `Assets/Resources/Data/Waves`
Owner: `Assets/Game/Campaign/Data/WaveData.cs`

Chapter I uses `Wave_01.asset` through `Wave_05.asset`. WaveData owns enemy count, multipliers, spawn interval, preparation time, target duration and boss flag.

## ChapterData
Path: `Assets/Resources/Chapters`
Owner: `Assets/Game/Campaign/Data/ChapterData.cs`

Current canonical asset: `Chapter01_Landing.asset`.
Contract: chapter 1, five combat events, about 12 minutes target duration, unlock Chapter II.

## Difficulty
Current owner: `Assets/Game/Core/Balance/DifficultyRules.cs`.
Modes: Story, Strategos, Legendary.
If tuning grows materially, migrate to authored `DifficultyData` assets instead of distributing constants across gameplay controllers.

## Campaign save data
Owner: `Assets/Game/Campaign/Persistence/CampaignSave.cs`.
Stores unlocks, difficulty, per-chapter results, narrative choices, modifiers, Chapter VI → VII state and final campaign result.

## AI authoring rules
1. Change balance in the relevant `.asset`, not in runtime bootstrap defaults.
2. `TheTroyGame/Data/Create Missing Default Assets` creates only absent assets.
3. The generator must never overwrite an existing authored asset.
4. Adding a TowerType or EnemyArchetype requires its matching asset and tests.
5. Missing authored runtime data is a validation failure, never a silent fallback.
6. Preserve `.meta` files and GUIDs when moving assets.
