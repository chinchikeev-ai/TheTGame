# TheTroyGame Module Map

Last reviewed: 2026-09-14

This file maps common tasks to the smallest correct area of the project. AI agents should use it before editing code.

## Core
`Assets/Game/Core`

- `Bootstrap/GameBootstrap.cs` — generic runtime composition only; delegates chapter-specific setup.
- `Input/GameInput.cs` — only gameplay-facing concrete input adapter.
- `Session/GameManager.cs` — session facade/summary.
- `Session/EconomyController.cs` — gold accounting.
- `Session/ScoreController.cs` — chapter score.
- `State/GameStateController.cs` — Preparing/WaveRunning/BetweenWaves/Paused/Victory/Defeat/NarrativeEvent.
- `Balance/BalanceCatalog.cs` — reusable TowerData/EnemyData lookup and low-level defaults; **not encounter composition**.
- `Balance/DifficultyRules.cs` — difficulty multipliers.
- `Localization/GameLanguage.cs` — EN/RU language state.
- `Logging/RuntimeFileLogger.cs` — per-launch runtime log.

Use Core for infrastructure/rules shared by multiple gameplay domains. Do not put chapter-specific behavior here.

## Campaign
`Assets/Game/Campaign`

- `CampaignController.cs` — campaign-facing runtime API.
- `ChapterController.cs` — active chapter selection/ownership.
- `Persistence/CampaignSave.cs` — JSON save implementation.
- `Data/ChapterData.cs` — chapter identity, encounter references and runtime profile.
- `Data/EncounterData.cs` — authored encounter composition, timing, routes and special behavior ids.
- `Data/WaveData.cs` — legacy compatibility only; do not use for new content.
- `Runtime/ChapterRuntimeInstaller.cs` — generic runtime-profile dispatcher.
- `Runtime/ChapterOneRuntimeInstaller.cs` — Chapter I-only world/Hector/cinematic/guidance composition.

UI/gameplay should call `CampaignController`; persistence details stay behind it.

New chapter rule: add authored `ChapterData` + `EncounterData` + a supported runtime profile/installer. Do not add chapter-number conditionals to `GameBootstrap`.

## Combat
`Assets/Game/Combat`

- `CombatDamage.cs` — `DamagePacket`, `DamageType`, damage rules.
- `Projectile.cs` — shared projectile runtime/pool.
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
- `EnemyData.cs` — reusable enemy authoring contract.
- `EnemyRegistry.cs` — runtime enemy index.
- `EnemySpawner.cs` — generic authored-encounter execution and difficulty-scaled spawn lifecycle.
- `EnemyRuntimeBehaviorRegistry.cs` — explicit special behavior id -> component binding.
- `MenelausBossController.cs` — Chapter I boss behavior and reinforcement profile.
- `EnemyHealthBar.cs` — enemy-specific presentation.

New enemy archetypes should reuse common combat/status systems. Encounter composition belongs in `EncounterData`, not `EnemySpawner`.

## Hector
`Assets/Game/Heroes/Hector`

- `HectorController.cs` — hero runtime and Q/E/R/F abilities.
- `ShieldWallZone.cs` — Shield Wall runtime zone.
- `HectorHUD.cs` — Hector-specific presentation.

Do not read mouse/keyboard APIs here; use `GameInput`.

## World
`Assets/Game/World`

- `MapBuilder.cs` — current Chapter I map/routes/build-grid implementation.
- `BuildPoint.cs` — buildable location runtime.
- `CameraController.cs` — map camera behavior.
- `CoastEnvironmentBuilder.cs` — Chapter I procedural coast implementation.
- `LandingPresentation.cs` — Chapter I landing set-piece.

World implementations may remain chapter-specific; ownership/wiring belongs in the matching chapter runtime installer.

## UI
`Assets/Game/UI`

Shared menus/HUD live here. UI must not access `CampaignSave` directly and must not own gameplay state.

Key runtime surfaces include:
- `GameMenuController.cs`
- `ModernCombatHud.cs`
- `CombatControlsUI.cs`
- `CampaignMapPresentation.cs`
- `BossHUD.cs`
- settings/result/menu presentation helpers.

## Audio / VFX
- `Assets/Game/Audio/AncientMusicController.cs`
- `Assets/Game/VFX/RuntimeEffects.cs`

## Data assets
Authoring assets remain under `Assets/Resources` for the current workflow.

- `Resources/Chapters` — chapter assets.
- `Resources/Data/Encounters` — encounter composition/pacing source of truth.
- `Resources/Data/Towers` — tower data.
- `Resources/Data/Enemies` — enemy data.
- `Resources/Data/Waves` — legacy compatibility only.

Do not load Resources repeatedly from Update loops.

## Editor / Tests
- `Assets/Editor/ChapterOneReleaseValidator.cs` — Chapter I data/runtime/presentation contract.
- `Assets/Editor/ChapterOnePlaythroughAnalyzer.cs` — telemetry analysis/tuning findings.
- `Assets/Editor/ChapterOneGameplayFreezeValidator.cs` — hard gameplay-freeze readiness gate.
- `Assets/Editor` — other editor tooling and architecture/art validation.
- `Assets/Tests/EditMode` — pure/data contract tests.
- `Assets/Tests/PlayMode` — runtime graph/gameplay acceptance tests.

## Task routing examples
- New tower -> Towers + TowerData asset + tests.
- New enemy -> Enemies + EnemyData asset + tests.
- Change enemy mix/cadence/routes -> EncounterData only unless runtime capability is missing.
- New status effect -> Combat contract + Enemy receiver + tests.
- New chapter -> ChapterData + EncounterData + Campaign/Runtime installer + chapter-specific World/presentation.
- Save schema change -> Campaign/Persistence + migration tests.
- Menu/HUD change -> UI only unless a missing public command requires owner-system work.
- New key/button -> Core/Input first, then consumer.
