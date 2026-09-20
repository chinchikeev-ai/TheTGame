# TheTroyGame Architecture

Last reviewed: 2026-09-14

## Purpose
Keep gameplay systems small, data-driven, testable, and easy for AI agents to modify safely.

## Runtime composition
`GameBootstrap` is the generic composition root. It must not contain chapter-specific coast/map/cinematic/tutorial logic.

Startup order:

`GameBootstrap -> CampaignController -> ChapterController -> GameManager -> ChapterRuntimeInstaller -> chapter runtime installer -> EnemySpawner -> shared UI`

Current Chapter I branch:

`ChapterRuntimeInstaller -> ChapterOneRuntimeInstaller -> MapBuilder / Hector / Landing / Atmosphere / Cinematic / Guidance`

Ownership:
- `CampaignController` — campaign-facing commands/progress.
- `ChapterController` — active `ChapterData`.
- `ChapterRuntimeInstaller` — selects the chapter runtime profile.
- chapter-specific runtime installer — owns that chapter's map/presentation/runtime composition only.
- `GameStateController` — explicit runtime state.
- `GameManager` — compatibility/session facade only.
- `EconomyController` — gold accounting.
- `ScoreController` — chapter score.
- `EnemySpawner` — generic encounter lifecycle and authored spawn-plan execution.
- `CampaignSave` — persistence implementation only.

A new chapter should add a dedicated runtime profile/installer instead of adding `if (chapter == N)` branches to `GameBootstrap`.

## Dependency direction
Primary direction:

`Data assets -> rules/contracts -> gameplay runtime -> UI/VFX`

UI may read gameplay state and issue public commands. UI must not write persistence directly.

Campaign flow:

`CampaignController -> ChapterController -> ChapterData -> EncounterData -> EnemySpawner -> result -> CampaignController -> CampaignSave`

Encounter flow:

`ChapterData.encounters -> EncounterData.spawnGroups -> EnemySpawner prepared plan -> EnemyData -> optional EnemyRuntimeBehaviorRegistry behavior`

Input flow:

`Input System / Legacy fallback -> GameInput -> gameplay/UI controllers`

Combat flow:

`Tower/Hero -> DamagePacket -> Enemy -> status/damage resolution`

## Runtime modules
All runtime C# belongs under `Assets/Game`.

- `Core/Bootstrap` — generic composition root.
- `Core/Input` — input facade.
- `Core/Session` — session/economy/scoring facade.
- `Core/State` — game-state machine.
- `Core/Balance` — shared reusable balance lookup/rules; never encounter composition.
- `Core/Localization` — language selection/translation helper.
- `Core/Logging` — runtime file logging.
- `Campaign/Data` — chapter and authored encounter contracts.
- `Campaign/Runtime` — chapter runtime profile boundary/installers.
- `Campaign/Persistence` — persistent campaign progress.
- `Combat` — shared damage/projectile/targeting primitives.
- `Towers` — tower data/runtime/placement/defense squads.
- `Enemies` — enemy data/runtime/spawner/boss behaviors.
- `Heroes/Hector` — Hector runtime and abilities.
- `World` — map/camera/environment/presentation implementations.
- `UI` — menus and generic HUD.
- `Audio` — audio/music runtime.
- `VFX` — runtime effects.

`Assets/Scripts` is legacy and must not be recreated.

## Assemblies
- Runtime: `Assets/Game/TheTroyGame.Runtime.asmdef`
- Editor: `Assets/Editor/TheTroyGame.Editor.asmdef`
- EditMode tests: `Assets/Tests/EditMode/TheTroyGame.EditModeTests.asmdef`
- PlayMode tests: `Assets/Tests/PlayMode/TheTroyGame.PlayModeTests.asmdef`

The runtime currently uses one assembly intentionally. Gameplay modules still have cross-module runtime references; splitting every folder into a separate assembly now would introduce cycles. New assembly boundaries should only be introduced after those dependencies are replaced with contracts/events/interfaces.

## Core contracts
### GameBootstrap
Composition only.

Allowed responsibilities:
- ensure campaign/chapter/session/shared services;
- ensure a main camera exists;
- call `ChapterRuntimeInstaller.Install`;
- initialize the generic `EnemySpawner` with chapter-supplied runtime paths;
- ensure shared UI.

Forbidden responsibilities:
- direct Chapter I coast/gate/cinematic/tutorial wiring;
- encounter composition;
- chapter balance.

### ChapterRuntimeInstaller
Maps a `ChapterData.runtimeProfile` to a dedicated chapter runtime installer.

Unsupported profiles fail explicitly. A chapter is not enabled merely because a chapter number exists.

### Chapter-specific runtime installer
Owns chapter-specific map/presentation/runtime composition while reusing generic combat/economy/save/UI systems.

`ChapterOneRuntimeInstaller` currently owns the Landing map, Hector placement, coast/Troy presentation, Chapter I cinematic, guidance, telemetry reporter, and the Chapter I-specific presentation lifecycle stack. Chapter I presenters must be installed here and must not self-create through `RuntimeInitializeOnLoadMethod`.

### EnemySpawner
Executes the active chapter's authored `EncounterData`.

It may:
- apply difficulty scaling to the authored plan;
- resolve fixed/round-robin routes;
- execute spawn timing;
- attach explicitly requested runtime behavior profiles.

It must not:
- infer enemy archetypes from encounter number or spawn index;
- hardcode Menelaus as the final enemy;
- own chapter-specific reinforcement composition.

Legacy Wave-prefixed members inside `EnemySpawner` are compatibility naming only. New consumers use `EncounterRuntime`; no new Wave-prefixed gameplay APIs may be introduced.

### EnemyRuntimeBehaviorRegistry
Attaches explicitly named special runtime behaviors such as `menelaus`.

An unknown behavior id is a configuration error, not a silent fallback.

### GameManager
Session facade. Do not add new unrelated responsibilities. Chapter-specific objective rules should continue moving out of this class as later chapter objective contracts are introduced.

Legacy `CurrentWave` / `MaxWaves` remain compatibility names only. New encounter-facing consumers use `EncounterRuntime`.

### GameInput
Only location allowed to access concrete mouse/keyboard APIs for gameplay input.

### CampaignController
Only campaign-facing API UI/gameplay should normally call for unlocks, difficulty and completion.

### CampaignSave
Persistence implementation; not a UI dependency.

### Registries
`EnemyRegistry` and `TowerRegistry` are runtime indexes. High-frequency code uses them instead of scene-wide searches.

### Presentation lifecycle ownership
Runtime presentation creation is explicit:

- global runtime initializers are limited to `GameBootstrap`, `CampaignSave`, `RuntimeFileLogger`, and `BuildVersionOverlay`;
- Chapter I-specific presentation is owned by `ChapterOneRuntimeInstaller`;
- menu presentation is owned by `GameMenuController`;
- combat-HUD presentation is owned by `ModernCombatHud`.

Owner-managed presenters must not self-create through `RuntimeInitializeOnLoadMethod`. New presentation components must be attached to the nearest existing owner instead of adding another global auto-start hook.

### Runtime lookup policy
Runtime-wide enumeration with `FindObjectsByType` / `FindObjectsOfType` is forbidden in gameplay code.

Bounded lookup during composition/startup is allowed when a runtime owner has not yet supplied a reference. `GameObject.Find` and `FindFirstObjectByType` must not run from `Update`, `LateUpdate`, `FixedUpdate` or equivalent high-frequency loops. Frequently accessed systems expose stable runtime references or registries instead.

Name-based lookup remains transitional wiring debt. New code should prefer explicit references from the composition root, owner APIs, registries, or stable runtime instances.


## Combat contract
All reusable damage uses `DamagePacket` and `DamageType`.
Current types: Physical, Piercing, Fire, Hero.
Current reusable statuses: Slow, Burn, ArmorBreak.
Future Stun/Fear should extend the shared layer rather than create one-off implementations.

## Data ownership
- Tower balance -> `TowerData`.
- Enemy balance -> `EnemyData`.
- Encounter composition/pacing/routes -> `EncounterData`.
- Chapter sequencing/runtime profile/objectives -> `ChapterData`.
- Difficulty tuning -> shared difficulty rules/data.
- `WaveData` -> removed; do not recreate or use as a fallback.

`EncounterData` is the only authored combat-encounter data authority. Remaining Wave-prefixed runtime names are compatibility surface only and do not imply a Wave data model.

Chapter content should be authored from data. Unique chapter scripts may orchestrate set pieces but must not fork generic combat/save systems.

## Architecture guardrails
`ArchitectureSmokeValidator`, `ChapterOneReleaseValidator` and contract tests protect:
- canonical module paths;
- absence of legacy `Assets/Scripts`;
- required asmdefs;
- no direct mouse/keyboard APIs outside `GameInput`;
- no scene-wide `FindObjects*` gameplay searches;
- no direct `CampaignSave` calls from UI;
- Chapter I/Tower/Enemy/Encounter data contracts;
- authored encounter composition instead of hidden `BalanceCatalog` formulas;
- generic `GameBootstrap` -> chapter runtime installer boundary;
- Economy/Score contracts.

EditMode and PlayMode test assemblies provide contract and runtime-graph smoke tests.

## Refactor policy
1. Define ownership/contract.
2. Add new owner while preserving public behavior.
3. Delegate old implementation.
4. Add automated validation/tests.
5. Move/rename with `.meta` GUID preservation.
6. Remove legacy location only after canonical location exists.
7. Update architecture/status docs in the same change set.
