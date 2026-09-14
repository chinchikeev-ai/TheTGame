# TheTroyGame Documentation

Last reviewed: 2026-09-14

This directory is the canonical documentation set for the current Unity project.

## Read first

For development or AI-assisted work, use this order:

1. `../AGENTS.md` — mandatory repository rules for AI changes.
2. `TODO.md` — single current day-to-day task list (`Сейчас`, `Сломано`, `Проверить`, `Потом`).
3. `PROJECT_STATUS.md` — what is actually implemented now.
4. `ARCHITECTURE.md` — runtime ownership and dependency rules.
5. `MODULE_MAP.md` — where each kind of change belongs.
6. `DATA_CATALOG.md` — authored runtime data and source-of-truth rules.
7. `RUNTIME_GRAPH.md` — runtime composition and lifecycle.
8. `TERMINOLOGY.md` — canonical player-facing naming.
9. `ENCOUNTER_TERMINOLOGY_MIGRATION.md` — Encounter-vs-Wave compatibility boundary.
10. `AI_PIPELINE.md` — validation and CI truthfulness rules.
11. `MODEL_ART_INVENTORY.md` — authoritative art/model completion status.
12. `ART_BIBLE.md` — single canonical creative + visual direction authority.
13. `UNITY_ROADMAP.md` — what should be built next.
14. `GDD.md` — canonical game-design target.

For non-trivial work use `tasks/TASK_TEMPLATE.md`.

## Authority hierarchy

When documents disagree, resolve them in this order:

1. **Current code + automated validation** — runtime truth.
2. **`PROJECT_STATUS.md`** — documented implementation truth.
3. **`ARCHITECTURE.md` / `DATA_CATALOG.md` / `RUNTIME_GRAPH.md`** — engineering contracts.
4. **`TERMINOLOGY.md` / `ENCOUNTER_TERMINOLOGY_MIGRATION.md`** — canonical naming and compatibility boundaries.
5. **`MODEL_ART_INVENTORY.md`** — production-art completion truth.
6. **`ART_BIBLE.md`** — single creative/visual direction authority.
7. **`GDD.md`** — intended game design.
8. **`UNITY_ROADMAP.md`** — planned implementation order.
9. Design-detail documents and audits — supporting reference only.

`TODO.md` is an execution queue, not an authority over current implementation state or design contracts.

A future-design document must never be used as evidence that a feature is implemented.

## Core product documents

- `GDD.md` — campaign structure, pacing, towers, enemies, Hector, bosses and ending.
- `ART_BIBLE.md` — canonical tone, vibe, cartoon level, humor, grotesque exaggeration, selective sex appeal, faction language, characters, environment, UI, animation and VFX.
- `PROJECT_STATUS.md` — current state, blockers and next product gate.
- `TODO.md` — current work queue; keep it short and delete completed items.
- `UNITY_ROADMAP.md` — milestone order from the current state to v1.0.
- `TERMINOLOGY.md` — canonical player-facing naming: Patron God, Artifact, Divine Power and Divine Power resource.
- `ENCOUNTER_TERMINOLOGY_MIGRATION.md` — Encounter is canonical; remaining Wave-prefixed runtime names are compatibility debt only.
- `TROY_DEFENSE_UNITS.md` — Trojan defense design; clearly separates Chapter I implemented roster from future concepts.

Future/meta-system design references:
- `GOD_PATRON_ARTIFACT_SYSTEM.md`
- `RANDOM_EVENTS_SYSTEM.md`

These remain design references until `PROJECT_STATUS.md` says the systems are implemented.

## Engineering documents

- `ARCHITECTURE.md` — module ownership and dependency direction.
- `MODULE_MAP.md` — task-to-module routing.
- `DATA_CATALOG.md` — ScriptableObject/data ownership; `EncounterData` is the authored combat source of truth and `WaveData` must not be recreated.
- `RUNTIME_GRAPH.md` — bootstrap/runtime flow.
- `ENCOUNTER_TERMINOLOGY_MIGRATION.md` — safe compatibility boundary for remaining Wave-prefixed runtime names and historical telemetry.
- `NAMESPACE_POLICY.md` — namespace migration policy.
- `AI_PIPELINE.md` — architecture guard, Unity tests, build validation and CI rules.

## Chapter I release-candidate documents

- `CHAPTER_I_RC_PLAYTEST.md` — real 1x gameplay validation protocol.
- `CHAPTER_I_GAMEPLAY_ACCEPTANCE.md` — explicit gameplay acceptance/freeze contract.
- `CHAPTER_I_ART_FREEZE.md` — strict production-art freeze gate.
- `MODEL_ART_INVENTORY.md` — asset/model status and `DONE` acceptance gate.

`CHAPTER_I_VISUAL_TARGET.md` remains only as a compatibility pointer to the canonical Chapter I visual section in `ART_BIBLE.md`.

Chapter I is not frozen until both gameplay RC and production-art gates in `PROJECT_STATUS.md` are closed.

## Art documents

Canonical direction:
- `ART_BIBLE.md` — the only creative + visual direction authority.

Compatibility pointers retained so old links do not break:
- `CREATIVE_DIRECTION.md`
- `CHARACTER_ART_DIRECTION.md`
- `TROJAN_UNIT_VISUAL_BIBLE.md`
- `CHAPTER_I_VISUAL_TARGET.md`

Do not add new art-direction rules to those pointer files. Update `ART_BIBLE.md` instead.

Pipeline / ownership:
- `CARTOON_CHARACTER_PIPELINE.md` — current Chapter I character/equipment/animation candidate workflow.
- `CAMPAIGN_ART_PIPELINE.md` — wider campaign candidate workflow.
- `CAMPAIGN_MODEL_AUDIT.md` — model audit workflow/output contract.
- `VISUAL_OWNERSHIP.md` — ownership of visible runtime geometry.
- `third_party/` — external-source provenance/license notes.

Status:
- `MODEL_ART_INVENTORY.md` — the only authority for `DONE`, `GENERATED PLACEHOLDER`, `PROCEDURAL`, `SOURCE ONLY`, `MISSING`.

## Audits

- `GAME_UX_AUDIT.md`
- `VISUAL_UX_AUDIT.md`

Audits are diagnostic snapshots. They preserve findings/rationale but do not override current code, `PROJECT_STATUS.md` or automated validation.

## Documentation maintenance rules

1. One canonical status document: `PROJECT_STATUS.md`.
2. One canonical current task list: `TODO.md`.
3. One canonical roadmap: `UNITY_ROADMAP.md`.
4. One canonical game-design document: `GDD.md`.
5. One canonical production-art status document: `MODEL_ART_INVENTORY.md`.
6. One canonical creative + visual direction document: `ART_BIBLE.md`.
7. Canonical runtime combat terminology is Encounter; do not create new Wave-prefixed gameplay APIs or `WaveData`.
8. Canonical player-facing divine terminology comes from `TERMINOLOGY.md`; runtime legacy names do not override it.
9. Do not create a second document for the same authority/scope.
10. Delete obsolete implementation plans instead of leaving competing instructions.
11. Merge overlapping references when one canonical document can carry the information cleanly.
12. Update status/inventory in the same change that materially changes implementation/art state.
13. Future concepts must be explicitly labeled as future/design-only.
14. Never claim Unity compile/tests/build are green unless those checks actually ran and passed.
15. Do not promote production art that conflicts with `ART_BIBLE.md` even if it is technically complete.

## Removed obsolete documents

The following historical documents were intentionally removed because their useful content was consolidated elsewhere:
- root `README.txt` — early prototype instructions;
- root `V05_PLAN.md` — completed early quality-pass plan;
- `CHARACTER_SILHOUETTES.md` — merged into the canonical Art Bible through the character-direction consolidation;
- `VISUAL_PRODUCTION_PLAN.md` — superseded by `PROJECT_STATUS.md`, `UNITY_ROADMAP.md` and `MODEL_ART_INVENTORY.md`.
