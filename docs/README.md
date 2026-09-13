# TheTroyGame Documentation

This directory is the canonical documentation set for the current Unity project.

## Read first

For development or AI-assisted work, use this order:

1. `../AGENTS.md` — mandatory repository rules for AI changes.
2. `PROJECT_STATUS.md` — current implementation state, blockers, and next gate.
3. `ARCHITECTURE.md` — runtime ownership and dependency rules.
4. `MODULE_MAP.md` — where each kind of change belongs.
5. `DATA_CATALOG.md` — authored runtime data and source-of-truth rules.
6. `RUNTIME_GRAPH.md` — runtime composition and lifecycle.
7. `AI_PIPELINE.md` — validation and CI truthfulness rules.
8. `MODEL_ART_INVENTORY.md` — authoritative production-art completion status.
9. `UNITY_ROADMAP.md` — milestone order from the current project state to v1.0.
10. `GDD.md` — canonical game design document.

For non-trivial work use `tasks/TASK_TEMPLATE.md`.

## Canonical product documents

- `GDD.md` — game design, campaign, pacing, towers, enemies, Hector, bosses, ending.
- `PROJECT_STATUS.md` — what is actually implemented now. This overrides older plans and audits when they disagree.
- `UNITY_ROADMAP.md` — what should be built next and in what order.
- `TERMINOLOGY.md` — naming conventions for gameplay and documentation.
- `TROY_DEFENSE_UNITS.md` — design reference for Troy defensive units.
- `GOD_PATRON_ARTIFACT_SYSTEM.md` — future campaign/meta-system design.
- `RANDOM_EVENTS_SYSTEM.md` — future campaign event-system design.

## Engineering documents

- `ARCHITECTURE.md` — canonical module ownership and dependency direction.
- `MODULE_MAP.md` — task-to-module routing.
- `DATA_CATALOG.md` — ScriptableObject/data ownership.
- `RUNTIME_GRAPH.md` — bootstrap/runtime flow.
- `NAMESPACE_POLICY.md` — namespace migration policy.
- `AI_PIPELINE.md` — architecture guard, Unity tests, build validation, CI rules.

## Chapter I release-candidate documents

- `CHAPTER_I_RC_PLAYTEST.md` — real 1x gameplay validation protocol.
- `CHAPTER_I_ART_FREEZE.md` — production-art freeze contract.
- `CHAPTER_I_VISUAL_TARGET.md` — visual target for the landing chapter.
- `MODEL_ART_INVENTORY.md` — asset-by-asset status and production acceptance gate.

Chapter I is not considered frozen until both the gameplay RC gate and production-art gate in `PROJECT_STATUS.md` are closed.

## Art pipeline documents

- `ART_BIBLE.md` — global art direction.
- `CHARACTER_ART_DIRECTION.md` — character-specific art rules.
- `CHARACTER_SILHOUETTES.md` — silhouette/readability reference.
- `CARTOON_CHARACTER_PIPELINE.md` — character candidate generation workflow.
- `CAMPAIGN_ART_PIPELINE.md` — wider campaign art production workflow.
- `CAMPAIGN_MODEL_AUDIT.md` — model audit workflow/output contract.
- `VISUAL_OWNERSHIP.md` — which systems own visible runtime geometry.
- `VISUAL_PRODUCTION_PLAN.md` — production-art execution plan.
- `third_party/` — provenance/license/source notes for external art assets.

`MODEL_ART_INVENTORY.md` is the only authority for whether an asset is `DONE`, `GENERATED PLACEHOLDER`, `PROCEDURAL`, `MISSING`, or `SOURCE ONLY`.

## Audits and diagnostic documents

- `GAME_UX_AUDIT.md`
- `VISUAL_UX_AUDIT.md`

These are diagnostic snapshots. They are useful for findings and design rationale, but they are not current-state authorities. When an audit conflicts with `PROJECT_STATUS.md`, current code, or automated validation, use the latter.

## Documentation maintenance rules

1. Do not create a second status or roadmap document for the same scope.
2. Update `PROJECT_STATUS.md` when implementation state materially changes.
3. Update `MODEL_ART_INVENTORY.md` in the same change that promotes or replaces production art.
4. Update `ARCHITECTURE.md` when ownership/dependency rules change.
5. Update the GDD and roadmap together when campaign structure, pacing, bosses, or core rules change.
6. Old implementation plans must be deleted or explicitly marked historical; they must not remain as competing instructions.
7. Never claim Unity compile/tests/build are green unless those checks actually ran and passed.
