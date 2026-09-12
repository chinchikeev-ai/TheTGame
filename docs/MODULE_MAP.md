# TheTroyGame Module Map

This file maps common tasks to the smallest correct area of the project. AI agents should use it before editing code.

## Core
`Assets/Game/Core`

- `Bootstrap/GameBootstrap.cs` — runtime composition only.
- `Input/GameInput.cs` — only gameplay-facing concrete input adapter.
- `Session/GameManager.cs` — session facade/summary.
- `Session/EconomyController.cs` — gold accounting.
- `Session/ScoreController.cs` — chapter score.
- `State/GameStateController.cs` — Preparing/WaveRunning/BetweenWaves/Paused/Victory/Defeat/NarrativeEvent.
- `Balance/BalanceCatalog.cs` — runtime lookup/fallback for authored balance data.
- `Balance/DifficultyRules.cs` — difficulty multipliers.
- `Localization/GameLanguage.cs` — EN/RU language state.
- `Logging/RuntimeFileLogger.cs` — per-launch runtime log.

Use Core for infrastructure/rules shared by multiple gameplay domains. Do not put chapter-specific behavior here.

## Campaign
`Assets/Game/Campaign`

- `CampaignController.cs` — campaign-facing runtime API.
- `ChapterController.cs` — active chapter selection/ownership.
- `Persistence/CampaignSave.cs` — JSON save implementation.
- `Data/ChapterData.cs` — chapter authoring contract.
- `Data/WaveData.cs` — wave authoring contract.

UI/gameplay should call `CampaignController`; persistence details stay behind it.

## Combat
`Assets/Game/Combat`

- `CombatDamage.cs` — `DamagePacket`, `DamageType`, damage rules.
- `Projectile.cs` — shared projectile runtime.
- `TargetPriority.cs` — targeting mode contract.

Add reusable combat mechanics here only when they are not tower/enemy/hero-specific.

## Towers
`Assets/Game/Towers`

- `Tower.cs` — tower runtime.
- `TowerData.cs` / `TowerType.cs` — tower data contract/identity.
- `TowerFactory.cs` — tower construction/view creation.
- `TowerPlacement.cs` — placement/selection commands.
- `TowerRegistry.cs` — runtime tower index.
- `TrojanGuardSquad.cs` — blocking squad runtime.
- `TowerRangeIndicator.cs` — tower-specific presentation helper.

Tower balance belongs in `TowerData`, not controller branches.

## Enemies
`Assets/Game/Enemies`

- `Enemy.cs` — common enemy runtime/status/damage receiver.
- `EnemyData.cs` — enemy authoring contract.
- `EnemyRegistry.cs` — runtime enemy index.
- `EnemySpawner.cs` — wave spawn lifecycle.
- `MenelausBossController.cs` — Chapter I boss behavior.
- `EnemyHealthBar.cs` — enemy-specific presentation.

New enemy archetypes should reuse common combat/status systems.

## Hector
`Assets/Game/Heroes/Hector`

- `HectorController.cs` — hero runtime and Q/E/R/F abilities.
- `ShieldWallZone.cs` — Shield Wall runtime zone.
- `HectorHUD.cs` — Hector-specific presentation.

Do not read mouse/keyboard APIs here; use `GameInput`.

## World
`Assets/Game/World`

- `MapBuilder.cs` — current map/routes/build-grid composition.
- `BuildPoint.cs` — buildable location runtime.
- `CameraController.cs` — map camera behavior.
- `CoastEnvironmentBuilder.cs` — Chapter I procedural coast prototype.
- `LandingPresentation.cs` — Chapter I landing set-piece.

## UI
`Assets/Game/UI`

Generic menus/HUD only:
- `GameMenuController.cs`
- `GameUIController.cs`
- `CombatControlsUI.cs`
- `CampaignProgressUI.cs`
- `ChapterFlowUI.cs`
- `BossHUD.cs`
- `GameHUD.cs`
- `ExtendedBalanceUI.cs`

UI must not access `CampaignSave` directly and must not own gameplay state.

## Audio / VFX
- `Assets/Game/Audio/AncientMusicController.cs`
- `Assets/Game/VFX/RuntimeEffects.cs`

## Data assets
Authoring assets remain under `Assets/Resources` for the current prototype workflow.

- `Resources/Chapters`
- `Resources/Data/Towers`
- `Resources/Data/Enemies`
- `Resources/Data/Waves`

Do not load Resources repeatedly from Update loops.

## Editor / Tests
- `Assets/Editor` — editor tooling and architecture smoke validation.
- `Assets/Tests/EditMode` — pure/data contract tests.
- `Assets/Tests/PlayMode` — runtime graph smoke tests.

## Task routing examples
- New tower -> Towers + TowerData asset + tests.
- New enemy -> Enemies + EnemyData asset + tests.
- New status effect -> Combat contract + Enemy receiver + tests.
- New chapter -> Campaign data + World/presentation only where unique.
- Save schema change -> Campaign/Persistence + migration tests.
- Menu/HUD change -> UI only unless a missing public command requires owner-system work.
- New key/button -> Core/Input first, then consumer.
