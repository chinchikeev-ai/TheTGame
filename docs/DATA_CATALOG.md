# TheTroyGame Data Catalog

Last reviewed: 2026-09-14

Authored ScriptableObject assets are the runtime source of truth. Runtime gameplay must fail validation when authored campaign/balance data is missing.

## TowerData
Path: `Assets/Resources/Data/Towers`  
Owner: `Assets/Game/Towers/TowerData.cs`

Assets:
- `ArcherTower.asset` — Archer Tower
- `Ballista.asset` — Ballista
- `PriestsOfApollo.asset` — Priests of Apollo
- `SpearWall.asset` — Spear Wall
- `FireTower.asset` — Fire Tower
- `TrojanGuard.asset` — Trojan Guard

These filenames are canonical and intentionally match gameplay terminology. Preserve the existing `.meta` GUIDs when moving or renaming them.

## EnemyData
Path: `Assets/Resources/Data/Enemies`  
Owner: `Assets/Game/Enemies/EnemyData.cs`

Current archetypes:
- Infantry
- Runner
- HeavyHoplite
- ShieldBearer
- Archer
- BatteringRam
- Boss (`menelaus` in Chapter I)

`EnemyData` owns reusable enemy stats/identity. It does **not** decide which enemies appear in an encounter.

## EncounterData
Path: `Assets/Resources/Data/Encounters`  
Owner: `Assets/Game/Campaign/Data/EncounterData.cs`

This is the runtime source of truth for encounter composition and pacing.

Each encounter owns:
- encounter id/number;
- preparation time;
- target combat duration;
- default spawn interval;
- encounter HP/speed multipliers;
- ordered spawn groups;
- enemy archetype patterns;
- repeat counts;
- fixed or round-robin route selection;
- optional group start delay / local spawn interval;
- local HP/speed multipliers;
- optional runtime behavior id such as `menelaus`.

Chapter I canonical assets:
- `Chapter01_01.asset` — 8 base enemies;
- `Chapter01_02.asset` — 12 base enemies;
- `Chapter01_03.asset` — 16 base enemies;
- `Chapter01_04.asset` — 20 base enemies;
- `Chapter01_05.asset` — 25 base enemies including the explicitly authored Menelaus group.

Difficulty count scaling is applied to the authored non-boss plan. Boss entries remain explicit and are not duplicated by global count scaling.

Enemy composition must **not** be reconstructed from encounter number, spawn index or modulo formulas in runtime code.

## ChapterData
Path: `Assets/Resources/Chapters`  
Owner: `Assets/Game/Campaign/Data/ChapterData.cs`

Current canonical asset: `Chapter01_Landing.asset`.

A chapter owns:
- chapter identity/title;
- `runtimeProfile` used by `ChapterRuntimeInstaller`;
- ordered `EncounterData` references;
- target chapter duration;
- objectives/tutorial copy;
- next chapter unlock.

Chapter I contract: five authored encounters, about 12 minutes target duration, runtime profile `chapter01_landing`, unlock Chapter II.

## Legacy WaveData
Path: `Assets/Resources/Data/Waves`  
Owner: `Assets/Game/Campaign/Data/WaveData.cs`

`WaveData` is retained only as compatibility/history during migration. `EnemySpawner` no longer reads it and it is **not** an authority for runtime encounter composition.

Do not author new chapter content as `WaveData`. New and migrated content belongs in `EncounterData` referenced by `ChapterData`.

## Difficulty
Current owner: `Assets/Game/Core/Balance/DifficultyRules.cs`.
Modes: Story, Strategos, Legendary.

Current difficulty modifies starting economy/gate health plus encounter enemy count/HP/speed/rewards. If tuning grows materially, migrate to authored `DifficultyData` rather than distributing new constants across gameplay controllers.

## Campaign save data
Owner: `Assets/Game/Campaign/Persistence/CampaignSave.cs`.

Stores unlocks, difficulty, per-chapter results, narrative choices, modifiers, Chapter VI → VII state and final campaign result.

## AI authoring rules
1. Change reusable tower/enemy balance in the relevant `.asset`.
2. Change encounter composition/pacing in `EncounterData`, not in `EnemySpawner`/`BalanceCatalog` conditions.
3. Change chapter sequencing/runtime profile/objectives in `ChapterData`.
4. `TheTroyGame/Data/Create Missing Default Assets` may create only low-level TowerData/EnemyData defaults; authored encounters are never regenerated automatically.
5. The generator must never overwrite an existing authored asset.
6. Adding a TowerType or EnemyArchetype requires its matching asset and tests.
7. Adding a new chapter requires a supported `runtimeProfile` and authored encounters before it can be enabled.
8. Missing authored runtime data is a validation failure, never a silent fallback.
9. Preserve `.meta` files and GUIDs when moving assets.
10. Keep asset filenames aligned with canonical gameplay terminology; do not reintroduce legacy names such as MachineGun/Cannon/Slow/SpearThrower.