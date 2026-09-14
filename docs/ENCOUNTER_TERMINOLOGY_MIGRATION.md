# Encounter Terminology Migration

## Canonical language

Runtime gameplay terminology is **Encounter**, not Wave.

New code must use:

- `EncounterData` and `ChapterData -> EncounterData` as the authored combat contract.
- `GameState.EncounterRunning` and `GameState.BetweenEncounters`.
- `EncounterRuntime` for runtime encounter state exposed by the current spawner/session implementation.
- `CombatHudEncounterFormatter`.
- `ChapterOneEncounterPresentation`.
- `VisualEncounterPreviewPresentation`.

Do not introduce new `WaveData`, `Wave*` presentation classes, or new Wave-prefixed gameplay APIs.

## Compatibility boundary

The remaining Wave-prefixed names are intentionally contained compatibility debt and are not the domain model.

### EnemySpawner

`EnemySpawner` still owns legacy names such as `CurrentWave`, `WaveActive`, `NextWave*`, `InterWaveCountdown`, `TargetWaveDuration`, `StartWaveNow`, and serialized `maxWaves`.

Reason: this class sits on scene/runtime state and may contain serialized Unity data or existing callers. New consumers must access those values through `EncounterRuntime` instead of adding more direct Wave-prefixed dependencies.

### GameManager

`GameManager.CurrentWave` and `GameManager.MaxWaves` remain a compatibility surface. New consumers should use `EncounterRuntime.CurrentEncounter` and `EncounterRuntime.MaxEncounters`.

### GameState aliases

`GameState.WaveRunning` aliases `EncounterRunning`, and `BetweenWaves` aliases `BetweenEncounters` with the same numeric values. These aliases exist only to preserve serialized enum compatibility. New code must not use them.

### Playthrough report schema

Historical Chapter I playthrough JSON can contain `WaveReport`, `waves`, and `wave`. These names are retained until a versioned report-schema migration can read both old and new reports. They are telemetry compatibility, not gameplay architecture.

### HUD hierarchy anchor

The existing `ModernCombatHUD/"WaveStatus"` transform name may remain temporarily because hierarchy-name changes must be performed atomically with the HUD and all runtime consumers. Code semantics and user-facing copy should use Encounter.

## Removal gates

Remove the compatibility names only after all of the following are true:

1. Unity compiles cleanly after the migration.
2. Scene/prefab serialized references have been checked in the Unity Editor.
3. Historical Chapter I reports remain readable, or a schema migration exists.
4. A clean Chapter I Story run completes at 1x/no pause.
5. Acceptance/analyzer output is regenerated from that post-migration run.

Until those gates are satisfied, compatibility aliases are safer than destructive renames.
