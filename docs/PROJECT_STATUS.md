# TheTroyGame Project Status

Last reviewed: 2026-09-13

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
- PlayMode acceptance tests cover first-wave start/completion, victory/unlock, defeat/no-unlock, language switch, Hector Q/E/R/F safety, final-wave boss data and Menelaus objective outcome
- Chapter I pacing contract now asserts auto-start target remains inside 11–13 minutes
- GitHub Actions architecture guard: VERIFIED GREEN
- GitHub Unity test jobs: BLOCKED BEFORE UNITY START by missing repository Unity activation credentials/license configuration
- therefore Unity compile, EditMode, PlayMode and Windows build are NOT YET VERIFIED by CI
- local Windows build after UI polish: VERIFIED GREEN with 0 errors

## Current gameplay
- DamageType / DamagePacket and Physical/Piercing/Fire/Hero: implemented
- Slow / Burn / ArmorBreak: implemented; Stun / Fear pending
- six Chapter I defense types: implemented
- 3 core tower upgrade levels: implemented; specialization branches pending
- Hector Q/E/R/F, HP/downed/revive/HUD: implemented; movement constraints/progression incomplete
- Menelaus boss/aura/reinforcements/final-wave integration: implemented
- Chapter I victory now requires Menelaus to be defeated
- Menelaus reaching the Trojan gate is an immediate chapter defeat even when gate HP remains
- Menelaus defeat/breach are tracked separately in runtime telemetry
- Chapter I has 5 events, objectives/tutorial, score/save/unlock, EN/RU, procedural coast/landing prototype
- Chapter I tutorial reflects marked build points and Hector Q/E/R/F controls
- combat tower selection now uses a single right-side `+` picker; the old duplicate bottom tower strip has been removed

## Remaining Chapter I RC work
- real 1x playthrough validation against 11–13 minute target
- final economy/enemy-pressure tuning from runtime logs
- UI/UX production pass for result screen, combat readability and 16:9/RU layout fit
- environment/landing presentation polish
- Hector movement constraints and battlefield bounds
- verify Trojan Guard blocking under high enemy density
- verify Menelaus encounter/reinforcement pressure on Story/Strategos/Legendary
- pooling before campaign scale-up

## Deferred infrastructure
- Unity CI activation/full green compile-test-build cycle is intentionally deferred until later

## Next product gate
Finish Chapter I RC gameplay/UX/presentation pass, then perform one real 1x run and use its runtime log for the final balance adjustment before Chapter II production.
