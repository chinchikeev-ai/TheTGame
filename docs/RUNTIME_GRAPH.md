# TheTroyGame Runtime Graph

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
    ├─ MapBuilder
    │   └─ routes / build points / environment
    ├─ EnemySpawner
    ├─ TowerPlacement
    ├─ HectorController
    ├─ LandingPresentation
    ├─ GameUIController
    └─ GameMenuController
```

`GameBootstrap` composes runtime objects. It must not become a gameplay owner.

## Campaign flow

```text
CampaignController
    ↓
ChapterController
    ↓
ChapterData
    ↓
WaveData
    ↓
EnemySpawner
    ↓
Enemy / Boss runtime
    ↓
GameManager result
    ↓
CampaignController
    ↓
CampaignSave
```

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

GameStateController is the canonical runtime state boundary for Preparing, WaveRunning, BetweenWaves, Paused, Victory, Defeat and NarrativeEvent.

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

UI must not call CampaignSave directly. Tests use isolated temporary storage via `CampaignSave.ConfigureStorageForTests`.

## Validation flow

```text
AI change
    ↓
python tools/check-architecture.py
    ↓
Unity ArchitectureSmokeValidator
    ↓
EditMode tests
    ↓
PlayMode acceptance tests
    ↓
Windows build
    ↓
PASS / FAIL
```

Local one-command entrypoints:
- Windows: `tools/validate-project.ps1`
- Unix: `tools/validate-project.sh`

GitHub Actions mirrors the same intent in `.github/workflows/unity-ci.yml`.
