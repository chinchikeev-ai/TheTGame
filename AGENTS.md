# TheTroyGame AI Development Rules

This file is the primary entrypoint for AI-assisted development. Read it before changing gameplay code.

## Source of truth
1. `docs/GDD_v0.1.md` — current GDD (filename retained for history).
2. `docs/UNITY_ROADMAP.md` — implementation milestones and DoD.
3. `docs/ARCHITECTURE.md` — ownership and dependency rules.
4. `docs/PROJECT_STATUS.md` — current implementation state.

If code and docs disagree, do not silently guess. Verify current code, then update the relevant status/documentation together with the implementation.

## Architecture rules
- Gameplay configuration belongs in ScriptableObjects, not hard-coded MonoBehaviour branches.
- Core systems must not depend on a specific chapter, boss or tower implementation.
- No `FindObjectsByType`, `FindObjectOfType`, scene scans or `Resources.Load` inside high-frequency gameplay loops.
- Towers use `TowerRegistry`; enemies use `EnemyRegistry`. Add registries for other high-frequency domains when needed.
- Damage must go through `DamagePacket` / `DamageType`.
- Reusable effects must use the shared status-effect APIs instead of bespoke per-unit damage logic.
- Campaign completion/progress must go through `CampaignSave`.
- New difficulty tuning must go through the shared difficulty rules/data layer.
- Prefer composition and small controllers over expanding `GameManager`.
- Do not create new global singletons unless ownership cannot reasonably live in an existing session/controller.

## Ownership
- `GameBootstrap`: composition/bootstrap only.
- `GameStateController`: explicit runtime state transitions.
- `GameManager`: compatibility facade/session summary. Do not add new unrelated responsibilities.
- `EconomyController`: gold income/spend/refund accounting.
- `ScoreController`: chapter score calculation.
- `CampaignSave`: persistent campaign data only.
- `EnemySpawner`: spawning and wave lifecycle only.
- `Tower`: tower runtime behavior only.
- `HectorController`: Hector runtime/abilities only.

## Data ownership
- Tower balance -> `TowerData`.
- Enemy balance -> `EnemyData`.
- Wave balance -> `WaveData`.
- Chapter setup -> `ChapterData`.
- Difficulty tuning -> shared difficulty rules/data.

Do not add chapter-specific balance constants to controllers when a data asset can own them.

## Performance rules
- No scene-wide searches from `Update`.
- Avoid allocations in `Update`, targeting loops and status ticks.
- Copy registries only when mutation during iteration is possible.
- Repeated/high-frequency projectiles, enemies and VFX should migrate to pooling before production scale.

## Save rules
- Persistent campaign state is stored in versioned JSON through `CampaignSave`.
- Any save schema change must increment the schema version and include migration/normalization.
- Never remove existing save fields without migration.

## UI and localization
- User-facing gameplay text must support EN/RU through the existing language layer.
- Language changes must not reload the active scene unless explicitly required.
- Runtime UI must not own gameplay state.

## Change discipline
For each task:
1. Identify the owner system.
2. Read the relevant data contract and controller.
3. Make the smallest coherent change.
4. Preserve existing public APIs when practical.
5. Update `docs/PROJECT_STATUS.md` for meaningful feature-status changes.
6. Add/update smoke tests for contracts that can be validated in EditMode.

## Required validation after gameplay changes
- Unity compiles with zero Console errors.
- Chapter I starts.
- A wave can start and complete.
- Victory and defeat paths work.
- Campaign save can load/write.
- Language toggle works without scene crash.
- Hector Q/E/R/F abilities do not throw.
- Menelaus final encounter can finish.

The assistant environment cannot run Unity Play Mode unless a Unity-capable tool/session is explicitly available. Never claim Play Mode or build validation without actually running it.