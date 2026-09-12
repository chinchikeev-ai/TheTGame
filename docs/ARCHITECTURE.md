# TheTroyGame Architecture

## Purpose
Keep gameplay systems small, data-driven, testable, and easy for AI agents to modify safely.

## Runtime composition
`GameBootstrap` creates and wires runtime systems.

Startup order:

`GameBootstrap -> CampaignController -> ChapterController -> GameManager -> GameStateController -> Map/Spawner -> Hero/UI`

Ownership:
- `CampaignController` — campaign-facing commands/progress.
- `ChapterController` — active `ChapterData`.
- `GameStateController` — explicit runtime state.
- `GameManager` — compatibility/session facade only.
- `EconomyController` — gold accounting.
- `ScoreController` — chapter score.
- `EnemySpawner` — wave lifecycle/spawning.
- `CampaignSave` — persistence implementation only.

## Dependency direction
Primary direction:

`Data assets -> rules/contracts -> gameplay runtime -> UI/VFX`

UI may read gameplay state and issue public commands. UI must not write persistence directly.

Campaign flow:

`CampaignController -> ChapterController -> ChapterData/WaveData -> EnemySpawner -> result -> CampaignController -> CampaignSave`

Input flow:

`Input System / Legacy fallback -> GameInput -> gameplay/UI controllers`

Combat flow:

`Tower/Hero -> DamagePacket -> Enemy -> status/damage resolution`

## Runtime modules
All runtime C# belongs under `Assets/Game`.

- `Core/Bootstrap` — composition root.
- `Core/Input` — input facade.
- `Core/Session` — session/economy/scoring facade.
- `Core/State` — game-state machine.
- `Core/Balance` — shared balance lookup/rules.
- `Core/Localization` — language selection/translation helper.
- `Core/Logging` — runtime file logging.
- `Campaign` — campaign/chapter runtime and persistent progress.
- `Combat` — shared damage/projectile/targeting primitives.
- `Towers` — tower data/runtime/placement/defense squads.
- `Enemies` — enemy data/runtime/spawner/bosses.
- `Heroes/Hector` — Hector runtime and abilities.
- `World` — map/camera/environment/presentation.
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
Composition only. No balance/chapter gameplay logic.

### GameManager
Session facade. Do not add new unrelated responsibilities.

### GameInput
Only location allowed to access concrete mouse/keyboard APIs for gameplay input.

### CampaignController
Only campaign-facing API UI/gameplay should normally call for unlocks, difficulty and completion.

### CampaignSave
Persistence implementation; not a UI dependency.

### Registries
`EnemyRegistry` and `TowerRegistry` are runtime indexes. High-frequency code uses them instead of scene-wide searches.

## Combat contract
All reusable damage uses `DamagePacket` and `DamageType`.
Current types: Physical, Piercing, Fire, Hero.
Current reusable statuses: Slow, Burn, ArmorBreak.
Future Stun/Fear should extend the shared layer rather than create one-off implementations.

## Data ownership
- Tower balance -> `TowerData`.
- Enemy balance -> `EnemyData`.
- Wave balance -> `WaveData`.
- Chapter setup -> `ChapterData`.
- Difficulty tuning -> shared difficulty rules/data.

Chapter content should be authored from data. Unique chapter scripts may orchestrate set pieces but must not fork generic combat/save systems.

## Automated architecture guardrails
`ArchitectureSmokeValidator` verifies:
- canonical module paths;
- absence of legacy `Assets/Scripts`;
- required asmdefs;
- no direct mouse/keyboard APIs outside `GameInput`;
- no scene-wide `FindObjects*` gameplay searches;
- no direct `CampaignSave` calls from UI;
- Chapter I/Tower/Enemy/Wave data contracts;
- Economy/Score contracts.

EditMode and PlayMode test assemblies provide contract and runtime-graph smoke tests.

## Refactor policy
1. Define ownership/contract.
2. Add new owner while preserving public API.
3. Delegate old implementation.
4. Add automated validation/tests.
5. Move/rename with `.meta` GUID preservation.
6. Remove legacy location only after canonical location exists.
7. Update architecture/status docs in the same change set.
