# TheTroyGame AI Development Rules

This is the primary entrypoint for AI-assisted development. Read it before changing code.

## Read order
1. `AGENTS.md`
2. `docs/PROJECT_STATUS.md`
3. `docs/ARCHITECTURE.md`
4. `docs/MODULE_MAP.md`
5. Relevant GDD/roadmap/chapter document

## Source of truth
- `docs/GDD_v0.1.md` — current GDD; filename retained for history.
- `docs/UNITY_ROADMAP.md` — implementation milestones and DoD.
- `docs/ARCHITECTURE.md` — ownership/dependency rules.
- `docs/MODULE_MAP.md` — canonical physical modules.
- `docs/PROJECT_STATUS.md` — current implementation state.

If code and docs disagree, verify current code, then update status/docs together with the implementation.

## Canonical runtime layout
Runtime code belongs under `Assets/Game` only.

- `Core` — bootstrap, state, input, logging, balance/session rules.
- `Campaign` — active chapter, campaign API, save persistence, chapter/wave data contracts.
- `Combat` — damage, projectiles, targeting and reusable combat primitives.
- `Towers` — tower/defense runtime and data.
- `Enemies` — enemy runtime, spawning, bosses and enemy data.
- `Heroes/Hector` — Hector runtime and abilities.
- `World` — map, camera, environment and presentation.
- `UI` — menus/HUD only.
- `Audio` — music/audio runtime.
- `VFX` — runtime visual effects.

Do not recreate `Assets/Scripts`.

## Assembly boundaries
- Runtime: `Assets/Game/TheTroyGame.Runtime.asmdef`
- Editor: `Assets/Editor/TheTroyGame.Editor.asmdef`
- EditMode tests: `Assets/Tests/EditMode/TheTroyGame.EditModeTests.asmdef`
- PlayMode tests: `Assets/Tests/PlayMode/TheTroyGame.PlayModeTests.asmdef`

Do not add a new assembly until dependencies are explicitly designed. Avoid cyclic module dependencies.

## Architecture rules
- Gameplay configuration belongs in ScriptableObjects, not hard-coded MonoBehaviour branches.
- Core systems must not depend on a specific chapter, boss or tower implementation unless they are composition/bootstrap code.
- No `FindObjectsByType`, `FindObjectOfType`, `FindObjectsOfType`, scene scans or `Resources.Load` inside high-frequency gameplay loops.
- Towers use `TowerRegistry`; enemies use `EnemyRegistry`.
- Damage goes through `DamagePacket` / `DamageType`.
- Reusable effects use the shared status-effect layer.
- Campaign-facing runtime code/UI uses `CampaignController`; only campaign infrastructure may access `CampaignSave` directly.
- Input-facing gameplay/UI uses `GameInput`; direct `Mouse.current`, `Keyboard.current` or `Input.Get*` is forbidden outside `GameInput`.
- Difficulty tuning goes through shared difficulty rules/data.
- Prefer composition and small controllers over expanding `GameManager`.
- Do not create new global singletons without a clear ownership reason.

## Ownership
- `GameBootstrap`: composition only.
- `GameStateController`: explicit runtime state transitions.
- `GameManager`: session facade/summary; do not grow unrelated responsibilities.
- `EconomyController`: gold accounting.
- `ScoreController`: chapter score calculation.
- `CampaignController`: campaign-facing commands/progress.
- `ChapterController`: active chapter ownership.
- `CampaignSave`: persistence implementation only.
- `EnemySpawner`: spawning and wave lifecycle only.
- `Tower`: tower runtime behavior only.
- `HectorController`: Hector runtime/abilities only.

## Data ownership
- Tower balance -> `TowerData`.
- Enemy balance -> `EnemyData`.
- Wave balance -> `WaveData`.
- Chapter setup -> `ChapterData`.
- Difficulty tuning -> shared difficulty rules/data.

## Performance rules
- No scene-wide searches from `Update`.
- Avoid allocations in targeting/status/update loops.
- Snapshot registries only when mutation during iteration is possible.
- Migrate repeated projectiles/enemies/VFX to pooling before later campaign scale.

## Save rules
- Persistent state is versioned JSON through `CampaignSave`.
- Save schema changes require version bump plus migration/normalization.
- Never delete a persisted field without migration.

## UI/localization
- User-facing gameplay text supports EN/RU through the language layer.
- Language changes must not reload the active scene.
- UI must not own gameplay state or persistence implementation.

## Required change workflow
1. Identify owner module/system.
2. Read its contract/data asset.
3. Make the smallest coherent change.
4. Preserve public APIs when practical.
5. Update `docs/PROJECT_STATUS.md` for meaningful status changes.
6. Add/update EditMode or PlayMode contract tests.
7. Run `TheTroyGame/Validation/Run Architecture Smoke Checks`.

## Required validation after gameplay changes
- Unity compiles with zero Console errors.
- Architecture smoke checks pass.
- EditMode tests pass.
- PlayMode tests pass.
- Chapter I starts; wave can complete.
- Victory/defeat and save/load work.
- Language toggle works without scene crash.
- Hector Q/E/R/F do not throw.
- Menelaus encounter can finish.

The assistant environment cannot claim Unity compile, Play Mode or build success unless those were actually run in a Unity-capable session.
