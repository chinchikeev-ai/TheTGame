# TheTroyGame Project Status

Last reviewed: 2026-09-13

## Campaign
- Chapter I: functional vertical slice, release-candidate gameplay/presentation stage
- Chapter II: unlock plumbing exists; content not implemented
- Chapters III-VII: planned in GDD/Roadmap

## AI development readiness
- `AGENTS.md`: authoritative AI entrypoint
- `docs/AI_PIPELINE.md`: validation contract
- `docs/PROJECT_STATUS.md`: product/runtime status
- `docs/MODEL_ART_INVENTORY.md`: authoritative production-art/model completion status
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
- Chapter I release-candidate validator: `TheTroyGame/Validation/Validate Chapter I Release Candidate`; checks data, 5-event/11-13-minute contract, Menelaus final wave, EN/RU objectives/tutorial, required presentation wiring and legacy placeholder regressions
- command-line architecture validation: implemented
- EditMode architecture/data tests: implemented
- isolated CampaignSave round-trip/reset/backup/Chapter VI→VII tests: implemented
- PlayMode runtime graph tests: implemented
- PlayMode acceptance tests cover first-wave start/completion, victory/unlock, defeat/no-unlock, language switch, Hector Q/E/R/F safety, Hector battlefield bounds, Trojan Guard block-capacity/refill, enemy death-presentation lifecycle, final-wave boss data and Menelaus objective outcome
- Chapter I pacing contract asserts auto-start target remains inside 11–13 minutes
- Chapter I 1x playthrough reporter: implemented; writes per-run JSON plus per-wave CSV under `Application.persistentDataPath/Logs`
- playthrough report captures result, pacing verdict, score, kills/leaks, economy, gate HP, tower counts, Menelaus outcome, average FPS and per-wave target/actual duration, economy deltas and peak alive-enemy pressure
- GitHub Actions architecture guard: VERIFIED GREEN on Chapter I RC combat hardening
- GitHub Unity test jobs: BLOCKED BEFORE UNITY START by missing repository Unity activation credentials/license configuration
- therefore Unity compile, EditMode, PlayMode and Windows build are NOT YET VERIFIED by CI
- local Windows build after UI polish: VERIFIED GREEN with 0 errors

## Current gameplay
- DamageType / DamagePacket and Physical/Piercing/Fire/Hero: implemented
- Slow / Burn / ArmorBreak: implemented; Stun / Fear pending
- six Chapter I defense types: implemented
- 3 core tower upgrade levels: implemented; specialization branches pending
- Hector Q/E/R/F, HP/downed/revive/HUD: implemented
- Hector movement is clamped to playable battlefield bounds and now has a locomotion/down-state presentation bridge for authored Animator controllers
- Menelaus boss/aura/reinforcements/final-wave integration: implemented
- Chapter I victory requires Menelaus to be defeated
- Menelaus reaching the Trojan gate starts gradual gate damage; Chapter I defeat happens only when gate HP reaches 0
- Menelaus defeat/breach are tracked separately in runtime telemetry
- Chapter I has 5 events, objectives/tutorial, score/save/unlock, EN/RU and a stylized coast/landing/Troy presentation
- Chapter I tutorial reflects marked build points and Hector Q/E/R/F controls
- combat tower selection uses a single right-side `+` picker; the old duplicate bottom tower strip has been removed
- tower projectiles have type-colored trails and heavy/fire projectile light; towers have basic recoil feedback on attack
- Chapter I coast has Trojan braziers, smoke columns, Greek campfires, city silhouette, fortified gate and warmer scene lighting
- enemies have lightweight locomotion motion, hit/attack presentation hooks and delayed death presentation instead of instant combat disappearance
- Menelaus has a pulsing aura ring and longer boss death presentation for readability
- Trojan Guard now owns explicit block reservations: capacity is deterministic, enemies cannot be stolen by another active squad, and slots release/refill when enemies die or leave range
- Hector abilities and Trojan Guard melee/rally trigger readable ground-pulse feedback
- Spear Wall is the default first build option; it uses a short 1.5-cell poke radius and no thrown projectile
- combat speed control uses bounded `-` / `+` controls instead of cycling into extreme speeds
- settings difficulty changes no longer reload the scene from the main menu
- Chapter II selection gives explicit in-production feedback after Chapter I unlocks it

## Production art status
- `docs/MODEL_ART_INVENTORY.md` now defines the completion contract for all characters, Tower-Units, siege assets, vehicles, environments and named heroes
- KayKit Adventurers is integrated as an approved CC0 source/base and prefab-generation pipeline
- current KayKit-derived Greek/Trojan characters and Hector/Menelaus/Achilles are `GENERATED PLACEHOLDER`, not final production models
- current Chapter I defensive structures are primarily `PROCEDURAL`; `TowerArtDirector` builds recognizable silhouettes from Unity primitives
- current Chapter I coastline, Greek landing, ships, Troy wall/gate and environmental dressing are staged procedurally rather than as a complete authored production environment kit
- `Assets/Resources/TroyCharacters` is a generated-output target and is not evidence that production assets are committed
- the production destination for promoted final art is `Assets/Game/Art/...`
- Chapter I currently has no character or Tower-Unit that qualifies as `DONE` under the production-art acceptance gate
- first production-art priorities are Hector, Menelaus, Chapter I Greek regulars, Trojan Guard/Archer, six Chapter I Tower-Units, Greek landing ships, coast kit and Troy wall/gate kit

## Remaining Chapter I RC work
- real 1x playthrough validation against the 11–13 minute target using the generated JSON/CSV playthrough report
- final economy/enemy-pressure tuning from per-wave report deltas and peak alive-enemy pressure
- final 16:9/RU visual-fit QA in real Play Mode
- verify Menelaus encounter/reinforcement pressure on Story/Strategos/Legendary using runtime playthrough data
- production-art replacement pass defined in `docs/MODEL_ART_INVENTORY.md`
- replace procedural presentation hooks with fully authored/retargeted animation clips where final art requires them
- pooling before campaign scale-up; not required for Chapter I acceptance unless profiling shows allocation spikes

## Deferred infrastructure
- Unity CI activation/full green compile-test-build cycle is intentionally deferred until repository Unity activation is configured

## Next product gate
Chapter I has two independent gates before it can be frozen as the campaign baseline:

1. **Gameplay RC gate:** run Chapter I once at 1x in the Unity Editor/build, inspect `ChapterI_Playthrough_*.json` and `ChapterI_Waves_*.csv`, tune pacing/economy/pressure, then complete 16:9/RU visual-fit QA.
2. **Production-art gate:** promote the P0 Chapter I assets in `docs/MODEL_ART_INVENTORY.md` from `GENERATED PLACEHOLDER` / `PROCEDURAL` to `DONE` with final assets under `Assets/Game/Art/...` and real Play Mode QA.

Chapter II production should not treat Chapter I art as finalized until both gates are closed.
