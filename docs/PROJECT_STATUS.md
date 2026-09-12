# TheTroyGame Project Status

Last reviewed: 2026-09-12

## Campaign
- Chapter I: functional vertical slice, late RC stage
- Chapter II: unlock plumbing exists; content not implemented
- Chapters III-VII: planned in GDD/Roadmap

## AI development readiness
- `AGENTS.md`: authoritative AI entrypoint
- `docs/AI_PIPELINE.md`: validation contract
- `docs/ARCHITECTURE.md`: ownership/dependency rules
- `docs/MODULE_MAP.md`: task-to-module routing
- `docs/DATA_CATALOG.md`: authored-data ownership
- `docs/RUNTIME_GRAPH.md`: runtime composition/lifecycle
- `docs/NAMESPACE_POLICY.md`: global-namespace policy until dedicated migration
- `docs/tasks/TASK_TEMPLATE.md`: non-trivial task contract
- legacy `Assets/Scripts`: removed
- GUID-preserving modular migration: complete
- fast architecture guard: `tools/check-architecture.py`
- local full validation: `tools/validate-project.ps1` / `tools/validate-project.sh`
- GitHub Actions pipeline: `.github/workflows/unity-ci.yml`

## Data authoring
- TowerData / EnemyData / WaveData / ChapterData assets are runtime sources of truth
- default-data generator is non-destructive and creates missing assets only
- missing authored runtime balance data fails fast instead of silently falling back
- DifficultyRules remains code-authored; migrate to DifficultyData only if tuning complexity requires it

## Automated validation
- Unity architecture smoke validator: implemented
- command-line architecture validation: implemented
- EditMode architecture/data tests: implemented
- isolated CampaignSave round-trip/reset/backup/Chapter VI→VII tests: implemented
- PlayMode runtime graph tests: implemented
- PlayMode acceptance tests: first-wave start/completion, victory/unlock, defeat/no-unlock, language switch, Hector Q/E/R/F safety, final-wave boss data
- GitHub Actions architecture guard: VERIFIED GREEN
- GitHub Unity test jobs: BLOCKED BEFORE UNITY START by missing repository Unity activation credentials/license configuration
- therefore Unity compile, EditMode, PlayMode and Windows build are NOT YET VERIFIED by CI
- once Unity activation is configured in repository Actions settings, the existing workflow continues automatically through tests and build

## Current gameplay
- DamageType / DamagePacket and Physical/Piercing/Fire/Hero: implemented
- Slow / Burn / ArmorBreak: implemented; Stun / Fear pending
- six Chapter I defense types: implemented
- 3 core tower upgrade levels: implemented; specialization branches pending
- Hector Q/E/R/F, HP/downed/revive/HUD: implemented; movement constraints/progression incomplete
- Menelaus boss/aura/reinforcements/final-wave integration: implemented
- Chapter I has 5 events, objectives/tutorial, score/save/unlock, EN/RU, procedural coast/landing prototype

## Remaining production risks
- first actual Unity compile/test pass after architecture migration
- final real 11–13 minute Chapter I balance validation
- projectile/enemy/VFX pooling
- production art/environment pass
- Hector movement constraints/progression
- gameplay modules still share one runtime assembly pending interface/event decoupling

## Next gate
Configure Unity activation for GitHub Actions or run `tools/validate-project.ps1` locally with Unity 6000.6.0f1. Fix any resulting compile/test failures before starting Chapter II implementation.
