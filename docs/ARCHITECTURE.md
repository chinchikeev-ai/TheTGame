# TheTroyGame Architecture

## Purpose
Keep gameplay systems small, data-driven, testable, and easy for AI agents to modify safely.

## Runtime composition

GameBootstrap creates and wires the runtime systems. CampaignController owns campaign-facing commands. ChapterController owns the active ChapterData. GameStateController owns explicit game states. GameManager is a compatibility/session facade and should shrink over time. EconomyController owns gold. ScoreController owns scoring. EnemySpawner owns wave lifecycle and spawning. CampaignSave owns persistence only.

Current startup order:

GameBootstrap -> CampaignController -> ChapterController -> GameManager -> GameStateController -> Map/Spawner -> Hero/UI.

## Dependency direction

Data assets -> core rules -> gameplay runtime -> UI/VFX.

UI may read gameplay state and call public controllers. UI should not write CampaignSave directly. Gameplay systems must not depend on concrete UI objects.

## Ownership

### GameBootstrap
Composition only. No gameplay balance or chapter rules.
Canonical path: `Assets/Game/Core/Bootstrap/GameBootstrap.cs`.

### GameStateController
Preparing, WaveRunning, BetweenWaves, Paused, Victory, Defeat, NarrativeEvent.

### GameManager
Session facade. Do not add unrelated responsibilities.
Canonical path: `Assets/Game/Core/Session/GameManager.cs`.

### GameInput
Single gameplay-facing input facade. Gameplay controllers must not read Mouse/Keyboard/Input directly.
Canonical path: `Assets/Game/Core/Input/GameInput.cs`.

### EconomyController
Current gold, earned gold, spent gold and accounting policy.

### ScoreController
Calculates chapter score from session-result data and difficulty multiplier.

### CampaignController
Campaign-facing runtime API: unlock state, difficulty, chapter completion and progress access. UI should depend on this controller rather than persistence details.
Canonical path: `Assets/Game/Campaign/CampaignController.cs`.

### ChapterController
Owns active ChapterData selection/loading. Chapter-specific runtime orchestration should grow from this boundary rather than from GameManager.
Canonical path: `Assets/Game/Campaign/ChapterController.cs`.

### CampaignSave
Persistent state only: unlocked chapters, difficulty, chapter results, narrative choices, modifiers and campaign result.

### EnemySpawner
Wave preparation, spawn cadence, route assignment, reinforcements and wave completion.

### Registries
EnemyRegistry and TowerRegistry are the runtime indexes. High-frequency gameplay code must use registries instead of scene-wide searches.

## Combat contract
All damage uses DamagePacket and DamageType. Reusable statuses use the shared Slow, Burn and ArmorBreak APIs. Future Stun/Fear should extend this shared layer.

## Data ownership
Tower balance -> TowerData.
Enemy balance -> EnemyData.
Wave balance -> WaveData.
Chapter setup -> ChapterData.
Difficulty tuning -> shared difficulty rules/data.

## Campaign target flow
CampaignController -> ChapterController -> ChapterData -> WaveData -> EnemySpawner -> chapter result -> CampaignController -> CampaignSave.

Chapter content should be authored from data. Unique chapter scripts may orchestrate set pieces but must not fork generic combat/save systems.

## Physical layout

Assets/Game/Core/Bootstrap
Assets/Game/Core/Session
Assets/Game/Core/Input
Assets/Game/Combat
Assets/Game/Towers
Assets/Game/Enemies
Assets/Game/Heroes/Hector
Assets/Game/Campaign
Assets/Game/World
Assets/Game/UI
Assets/Game/Audio
Assets/Game/VFX
Assets/Game/Debug
Assets/Editor
Assets/Tests/EditMode
Assets/Tests/PlayMode

Physical migration is incremental. Never move a Unity asset without its `.meta` file; stable GUIDs are mandatory.

## Refactor policy
1. Define ownership and contract.
2. Add the new owner while preserving the old public API.
3. Delegate old code to the new owner.
4. Add smoke validation/tests.
5. Only then move or rename Unity files.
6. Move files in GUID-preserving groups and validate before moving the next module.
