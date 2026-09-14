# TheTroyGame Runtime Graph

Last reviewed: 2026-09-14

## Startup composition

```text
Unity Scene Load
    ↓
GameBootstrap
    ├─ CampaignController
    ├─ ChapterController
    ├─ GameManager
    ├─ GameStateController
    ├─ RuntimeEffects
    ├─ AncientMusicController
    ├─ ChapterRuntimeInstaller
    │   └─ runtimeProfile
    │       └─ ChapterOneRuntimeInstaller (current)
    │           ├─ MapBuilder / routes / build points
    │           ├─ coast / gate / atmosphere presentation
    │           ├─ Hector
    │           ├─ LandingPresentation
    │           ├─ ChapterOneCinematicCamera
    │           ├─ ChapterOneGuidancePresentation
    │           └─ ChapterOnePlaythroughReporter
    ├─ EnemySpawner
    └─ shared HUD / menu / settings / campaign UI
```

`GameBootstrap` composes shared runtime objects and delegates chapter-specific setup. It must not become a chapter gameplay/presentation owner.

## Campaign and encounter flow

```text
CampaignController
    ↓
ChapterController
    ↓
ChapterData
    ├─ runtimeProfile ──> ChapterRuntimeInstaller
    └─ encounters[]
           ↓
       EncounterData
           ↓
       spawnGroups[]
           ↓
       EnemySpawner
           ├─ DifficultyRules scaling
           ├─ route resolution
           ├─ EnemyData
           └─ optional behaviorId
                    ↓
            EnemyRuntimeBehaviorRegistry
                    ↓
              special runtime behavior
           ↓
       Enemy / Boss runtime
           ↓
       GameManager result
           ↓
       CampaignController
           ↓
       CampaignSave
```

Encounter composition is authored. `EnemySpawner` must not infer archetypes from encounter number/spawn index.

## Session lifecycle

```text
Application start
    ↓
Main Menu
    ↓
Level Select
    ↓
Preparing
    ↓
WaveRunning
    ↓
BetweenWaves
    ↓
... repeat ...
    ↓
Victory / Defeat
    ↓
Result summary
    ↓
Campaign save / unlock
```

`GameStateController` is the canonical runtime state boundary for Preparing, WaveRunning, BetweenWaves, Paused, Victory, Defeat and NarrativeEvent. State names retain the historical `WaveRunning`/`BetweenWaves` terminology even though authored campaign content is now represented by `EncounterData`.

## Combat flow

```text
Tower / Hector / status source
    ↓
DamagePacket + DamageType
    ↓
Enemy.ReceiveDamage
    ↓
armor/resistance/status processing
    ↓
Enemy death or continued route progress
```

Tower targeting uses `TowerRegistry`/`EnemyRegistry`; gameplay loops must not perform scene-wide searches.

## Input flow

```text
Unity Input System / legacy fallback
    ↓
GameInput
    ↓
Hector / TowerPlacement / Menu / other consumers
```

No gameplay/UI class may read `Mouse.current`, `Keyboard.current` or `Input.Get*` directly.

## Save flow

```text
Campaign-facing action
    ↓
CampaignController
    ↓
CampaignSave
    ↓
versioned JSON
    ├─ campaign_save.json
    └─ campaign_save.backup.json
```

UI must not call `CampaignSave` directly. Tests use isolated temporary storage via `CampaignSave.ConfigureStorageForTests`.

## Chapter I gameplay-freeze flow

```text
Real Story run at 1x
    ↓
ChapterOnePlaythroughReporter
    ↓
ChapterI_Playthrough_*.json + ChapterI_Waves_*.csv
    ↓
ChapterOnePlaythroughAnalyzer
    ↓
PASS / WARN / FAIL tuning findings
    ↓
ChapterOneGameplayFreezeValidator
    ↓
BLOCKED
or
READY FOR HUMAN ACCEPTANCE
    ↓
manual warning + presentation review
    ↓
explicit baseline acceptance
```

No static validator can replace the real 1x run or the human visual/readability acceptance step.
