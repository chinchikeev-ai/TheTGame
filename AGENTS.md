# TheTroyGame AI Development Rules

This is the mandatory entrypoint for AI-assisted development.

## Read order
1. `AGENTS.md`
2. `docs/AI_PIPELINE.md`
3. `docs/PROJECT_STATUS.md`
4. `docs/ARCHITECTURE.md`
5. `docs/MODULE_MAP.md`
6. `docs/DATA_CATALOG.md`
7. `docs/RUNTIME_GRAPH.md`
8. `docs/NAMESPACE_POLICY.md`
9. relevant GDD/roadmap/chapter document

For non-trivial work, use `docs/tasks/TASK_TEMPLATE.md`.

## Canonical runtime layout
Runtime code belongs under `Assets/Game` only. Do not recreate `Assets/Scripts`.

- `Core` — bootstrap, state, input, logging, balance/session rules
- `Campaign` — campaign API, chapter ownership, save, chapter/wave data
- `Combat` — reusable combat primitives
- `Towers` — tower runtime/data
- `Enemies` — enemy runtime/spawning/bosses/data
- `Heroes/Hector` — Hector runtime/abilities
- `World` — map/camera/environment/presentation
- `UI` — menus/HUD only
- `Audio` — audio runtime
- `VFX` — visual effects

## Data source of truth
Authored ScriptableObject assets are the runtime source of truth.

- Tower balance -> `TowerData` assets
- Enemy balance -> `EnemyData` assets
- Wave pacing/config -> `WaveData` assets
- Chapter setup -> `ChapterData` assets
- Change balance in assets, not runtime fallback/default code
- `TheTroyGame/Data/Create Missing Default Assets` may create only absent assets
- Existing authored assets must never be auto-overwritten
- Missing authored runtime data is a validation failure, never a silent fallback

See `docs/DATA_CATALOG.md`.

## Architecture rules
- `GameBootstrap` composes runtime systems only
- `GameManager` is a session facade; do not grow unrelated responsibilities
- `CampaignController` is the campaign-facing runtime API
- `ChapterController` owns the active chapter
- only campaign infrastructure may access `CampaignSave` directly; UI uses `CampaignController`
- gameplay/UI input goes through `GameInput`
- direct `Mouse.current`, `Keyboard.current` and `Input.Get*` are forbidden outside `GameInput`
- towers use `TowerRegistry`; enemies use `EnemyRegistry`
- no `FindObjectsByType`, `FindObjectsOfType` or other scene-wide gameplay searches
- damage goes through `DamagePacket` / `DamageType`
- reusable effects use shared combat/status APIs
- avoid `Resources.Load` in high-frequency loops
- prefer small owners/controllers over new global singletons

## Namespace policy
Keep runtime classes in the global namespace for now. Do not introduce `namespace TheTroyGame` in isolated files. Namespace migration must be a dedicated repository-wide task. See `docs/NAMESPACE_POLICY.md`.

## Save rules
- persistent state is versioned JSON through `CampaignSave`
- schema changes require version bump plus migration/normalization
- never delete a persisted field without migration
- automated tests use isolated temporary storage and must never mutate the user's real save

## Performance rules
- no scene scans from Update
- avoid allocations in targeting/status/update loops
- snapshot registries only when mutation during iteration is possible
- move repeated projectiles/enemies/VFX to pooling before later campaign scale

## UI/localization
- user-facing gameplay text supports EN/RU through the language layer
- language switching must not reload the active scene
- UI must not own gameplay state or persistence implementation

## Required change workflow
1. Identify owner module/system using `docs/MODULE_MAP.md`.
2. Read the relevant data contract and current status.
3. For non-trivial work define acceptance criteria from `docs/tasks/TASK_TEMPLATE.md`.
4. Make the smallest coherent change.
5. Preserve public APIs when practical.
6. Update `docs/PROJECT_STATUS.md` for meaningful status changes.
7. Add/update EditMode or PlayMode tests.
8. Run `python tools/check-architecture.py`.
9. When Unity is available, run full validation before claiming completion.

## Full validation
Windows: `./tools/validate-project.ps1`

Unix: `UNITY_EDITOR=/path/to/Unity ./tools/validate-project.sh`

Full stages: architecture guard -> Unity architecture checks -> EditMode -> PlayMode acceptance tests -> Windows build.

GitHub Actions mirrors this flow in `.github/workflows/unity-ci.yml`.

## Validation truthfulness
An agent may say code was implemented after editing source. It may say Unity compile/tests/build are validated only when those checks actually ran and passed.

Required gameplay acceptance coverage includes Chapter I start/wave completion, victory/unlock, defeat/no-unlock, save round-trip/backup, language switch without reload, Hector Q/E/R/F safety, and final-wave boss data.
