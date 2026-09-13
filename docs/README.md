# TheTroyGame Documentation

Last reviewed: 2026-09-14

This directory is the canonical documentation set for the current Unity project.

## Read first

For development or AI-assisted work, use this order:

1. `../AGENTS.md` — mandatory repository rules for AI changes.
2. `PROJECT_STATUS.md` — what is actually implemented now.
3. `ARCHITECTURE.md` — runtime ownership and dependency rules.
4. `MODULE_MAP.md` — where each kind of change belongs.
5. `DATA_CATALOG.md` — authored runtime data and source-of-truth rules.
6. `RUNTIME_GRAPH.md` — runtime composition and lifecycle.
7. `AI_PIPELINE.md` — validation and CI truthfulness rules.
8. `MODEL_ART_INVENTORY.md` — authoritative art/model completion status.
9. `UNITY_ROADMAP.md` — what should be built next.
10. `GDD.md` — canonical game-design target.

For non-trivial work use `tasks/TASK_TEMPLATE.md`.

## Authority hierarchy

When documents disagree, resolve them in this order:

1. **Current code + automated validation** — runtime truth.
2. **`PROJECT_STATUS.md`** — documented implementation truth.
3. **`ARCHITECTURE.md` / `DATA_CATALOG.md` / `RUNTIME_GRAPH.md`** — engineering contracts.
4. **`MODEL_ART_INVENTORY.md`** — production-art completion truth.
5. **`GDD.md`** — intended game design.
6. **`UNITY_ROADMAP.md`** — planned implementation order.
7. Design-detail documents and audits — supporting reference only.

A future-design document must never be used as evidence that a feature is implemented.

## Core product documents

- `GDD.md` — campaign structure, pacing, towers, enemies, Hector, bosses and ending.
- `PROJECT_STATUS.md` — current state, blockers and next product gate.
- `UNITY_ROADMAP.md` — milestone order from the current state to v1.0.
- `TERMINOLOGY.md` — canonical naming.
- `TROY_DEFENSE_UNITS.md` — Trojan defense design; clearly separates Chapter I implemented roster from future concepts.

Future/meta-system design references:
- `GOD_PATRON_ARTIFACT_SYSTEM.md`
- `RANDOM_EVENTS_SYSTEM.md`

These remain design references until `PROJECT_STATUS.md` says the systems are implemented.

## Engineering documents

- `ARCHITECTURE.md` — module ownership and dependency direction.
- `MODULE_MAP.md` — task-to-module routing.
- `DATA_CATALOG.md` — ScriptableObject/data ownership.
- `RUNTIME_GRAPH.md` — bootstrap/runtime flow.
- `NAMESPACE_POLICY.md` — namespace migration policy.
- `AI_PIPELINE.md` — architecture guard, Unity tests, build validation and CI rules.

## Chapter I release-candidate documents

- `CHAPTER_I_RC_PLAYTEST.md` — real 1x gameplay validation protocol.
- `CHAPTER_I_ART_FREEZE.md` — strict production-art freeze gate.
- `CHAPTER_I_VISUAL_TARGET.md` — Chapter I visual target.
- `MODEL_ART_INVENTORY.md` — asset/model status and `DONE` acceptance gate.

Chapter I is not frozen until both gameplay RC and production-art gates in `PROJECT_STATUS.md` are closed.

## Art documents

Direction:
- `ART_BIBLE.md` — global art direction.
- `CHARACTER_ART_DIRECTION.md` — faction, role and silhouette rules.

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
2. One canonical roadmap: `UNITY_ROADMAP.md`.
3. One canonical game-design document: `GDD.md`.
4. One canonical production-art status document: `MODEL_ART_INVENTORY.md`.
5. Do not create a second document for the same authority/scope.
6. Delete obsolete implementation plans instead of leaving competing instructions.
7. Merge small overlapping references when one can carry the information cleanly.
8. Update status/inventory in the same change that materially changes implementation/art state.
9. Future concepts must be explicitly labeled as future/design-only.
10. Never claim Unity compile/tests/build are green unless those checks actually ran and passed.

## Removed obsolete documents

The following historical documents were intentionally removed because their useful content was consolidated elsewhere:
- root `README.txt` — early prototype instructions;
- root `V05_PLAN.md` — completed early quality-pass plan;
- `CHARACTER_SILHOUETTES.md` — merged into `CHARACTER_ART_DIRECTION.md`;
- `VISUAL_PRODUCTION_PLAN.md` — superseded by `PROJECT_STATUS.md`, `UNITY_ROADMAP.md` and `MODEL_ART_INVENTORY.md`.