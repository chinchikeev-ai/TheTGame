# TheTroyGame — Project Status

Last reviewed: 2026-09-14

This document is the canonical answer to **what is implemented now**. It intentionally does not duplicate the GDD or roadmap.

## Current phase

**Chapter I: The Landing — gameplay release candidate + production-art candidate pass.**

Chapter I is functionally playable as a vertical slice, but it is **not frozen** yet. Two independent gates remain:

1. Gameplay RC gate — clean real Story 1x playthrough, telemetry analysis/freeze-readiness review, WARN acceptance, Strategos/Legendary pressure review, RU/EN visual-fit QA and explicit final gameplay freeze.
2. Production-art gate — promote P0 assets from generated/procedural candidates to accepted final art after real visual QA.

Chapter II unlock plumbing exists, but Chapter II content is not implemented. Chapters III–VII remain design/roadmap work.

## Canonical runtime architecture

Runtime code remains under `Assets/Game` and uses one intentional `TheTroyGame.Runtime` assembly while cross-module dependencies still exist.

Current startup boundary:

```text
GameBootstrap
  -> CampaignController
  -> ChapterController / ChapterData
  -> GameManager + shared services
  -> ChapterRuntimeInstaller
       -> ChapterOneRuntimeInstaller (current chapter-specific runtime)
  -> EnemySpawner
  -> shared UI
```

`GameBootstrap` is chapter-agnostic. Chapter I coast/map/Hector/presentation/cinematic/guidance wiring lives behind `ChapterOneRuntimeInstaller` instead of being directly embedded in the generic bootstrap.

A future chapter should add a dedicated runtime profile/installer rather than expanding `GameBootstrap` with chapter-number conditionals.

`Assets/Scripts` is legacy and must not be recreated.

## Current gameplay systems

Implemented:

- explicit game-state flow;
- Gold economy and chapter scoring;
- data-driven `TowerData`, `EnemyData`, `EncounterData`, `ChapterData`;
- authored encounter spawn groups/routes/timing instead of hidden wave-index composition formulas;
- six Chapter I defensive roles;
- three core upgrade levels;
- Upgrade / Sell / target priority;
- one-shot build mode: successful placement exits build mode and requires a fresh selection for the next placement;
- contextual selected-unit menu positioned around the selected defense;
- placement preview, valid/invalid feedback and range visualization;
- DamagePacket / Physical / Piercing / Fire / Hero;
- Slow / Burn / ArmorBreak;
- deterministic Trojan Guard blocking reservations and refill;
- Hector movement, HP, downed/revive and Q/E/R/F;
- Hector battlefield clamping;
- Menelaus boss, commander aura, authored boss behavior binding, reinforcement calls and final-encounter objective integration;
- Chapter I victory requires Menelaus defeated;
- Menelaus reaching the gate damages it over time; defeat occurs when gate HP reaches zero;
- deterministic EN/RU language selection plus language switching;
- save/unlock flow for Chapter I -> Chapter II;
- bounded combat-speed controls;
- Chapter I telemetry, playthrough analysis, hard freeze-readiness validation and final acceptance tooling.

Pending core mechanics:

- Stun / Fear shared status implementations when later content requires them;
- tower specialization branches after Level 3;
- obstacle-aware hero/path movement if later maps require it;
- pooling before campaign-scale density if profiling shows a need.

## Encounter authoring state

Chapter I references exactly five authored `EncounterData` assets:

- Encounter 1: 8 base enemies;
- Encounter 2: 12 base enemies;
- Encounter 3: 16 base enemies;
- Encounter 4: 20 base enemies;
- Encounter 5: 25 base enemies including an explicit boss group with `behaviorId=menelaus`.

Encounter data owns preparation time, target duration, spawn cadence, pressure multipliers, ordered archetype patterns, route policy and optional special behavior ids.

`EnemySpawner` executes this authored plan and applies difficulty scaling. It no longer derives enemy archetypes from wave/index modulo formulas and no longer hardcodes `MenelausBossController` as the final spawn.

Legacy `WaveData` remains only as compatibility/history during migration and is not the runtime encounter source of truth.

## Chapter I content contract

Current target:

- 5 authored combat encounters;
- approximately 11–13 minutes at 1x;
- first preparation target 30 seconds;
- coast / Greek landing setting;
- Menelaus climax;
- tutorialized defensive placement + Hector controls;
- canonical Chapter II unlock on valid victory.

The playthrough reporter writes JSON + per-encounter CSV under `Application.persistentDataPath/Logs` with:

- result and difficulty;
- telemetry schema version;
- 1x compliance and pause use;
- total duration and per-encounter actual/target duration;
- kills / leaks;
- Gold earned / spent / refunded;
- gate HP;
- tower mix;
- Menelaus result;
- peak alive-enemy pressure;
- average FPS.

Validation and acceptance flow:

1. `TheTroyGame/Validation/Analyze Latest Chapter I Playthrough` -> detailed PASS/WARN/FAIL tuning report.
2. `TheTroyGame/Validation/Check Chapter I Gameplay Freeze Readiness` -> independent hard-gate decision.
3. `TheTroyGame/Validation/Gameplay Acceptance/Prepare Latest Story Candidate` -> binds the exact accepted-candidate Story report to the tracked manifest using session id + SHA-256 and generates a WARN review checklist.
4. `TheTroyGame/Validation/Gameplay Acceptance/Analyze Story-Strategos-Legendary Pressure` -> compares the latest three 1x/no-pause difficulty runs and reports pressure inversions/eligibility problems.
5. `TheTroyGame/Validation/Visual Fit/...` -> deterministic RU/EN QA presets and checklist for the required 16:9 matrix.
6. `TheTroyGame/Validation/Gameplay Acceptance/Check Final Chapter I Acceptance` -> prevents final gameplay freeze until Story/WARN/difficulty/visual-fit gates are explicitly accepted.

The canonical final gameplay acceptance state is tracked in:

`Assets/Game/QA/CHAPTER_I_GAMEPLAY_ACCEPTANCE.json`

Changing the bound Story session resets downstream WARN, Story acceptance, difficulty, visual-fit and final-freeze approvals so stale acceptance cannot carry over to new balance.

Freeze readiness requires Story, Chapter I, victory, Menelaus defeated/no completed breach, verified 1x/no-pause telemetry, 11:00–13:00 total duration, five complete encounter snapshots, no encounter >=40% away from its duration target and gate HP above zero.

A `READY FOR HUMAN ACCEPTANCE` result does **not** automatically declare gameplay frozen. `gameplayFrozen=true` is valid only after all steps in `CHAPTER_I_GAMEPLAY_ACCEPTANCE.md` are explicitly accepted.

Exact manual protocols:

- `CHAPTER_I_RC_PLAYTEST.md`
- `CHAPTER_I_GAMEPLAY_ACCEPTANCE.md`

## UI / combat readability

Implemented presentation work includes:

- responsive combat HUD for 1920x1080 and 1366/1376x768 target layouts;
- unified Trojan combat HUD cards using the same dark-stone, bronze-framed, portrait-led language as the Hector block;
- one direct Divine Power action with effect/readiness copy instead of competing duplicate controls;
- separate encounter, boss, Hector, build and selected-unit regions;
- selected-defense contextual menu around the unit;
- selected patron portrait and event commentary in a dedicated upper-right battlefield observer panel;
- distinct EN/RU patron reactions for wave flow, defenses, gate pressure, kill milestones, Hector, Menelaus and Divine Power use;
- tower hover/selection/range feedback;
- projectile trails and type-specific hit feedback;
- tower recoil;
- Menelaus aura, boss warning and longer boss death presentation;
- Hector ability pulses and hit feedback;
- improved coast, road, Greek landing, ships, Trojan gate/walls, banners, braziers and atmosphere;
- cinematic Chapter I opening camera pass;
- gameplay-camera character combat presentation pass for Greek Archer nocked-arrow lifecycle and Hector spear flight/restore behavior.

Final RU/EN 16:9 real visual QA is still required before Chapter I gameplay freeze and before production-art acceptance.

## Character and defensive-unit animation candidates

Role-specific Animator profiles exist for generic infantry, spear infantry, archer, skirmisher, Hector, Menelaus, Ballista crew, Apollo priest and Fire Keeper.

Runtime hooks include:

- Hector: Attack / Q / E / R / F / Hit / Downed;
- Menelaus: Command;
- Trojan Guard: Block / Poke;
- Spear Wall: Poke;
- Archer Post: Draw / Release;
- Ballista: Fire / Reload / Tension;
- Priests of Apollo: Cast / Channel;
- Fire Keeper: Throw / Stoke.

Character combat presentation now also includes explicit weapon-release sockets, animation-phase impact timing, visible character projectile flights, Greek Archer nocked-arrow presentation and Hector carried-spear hide/flight/restore behavior. These remain production candidates pending real Unity pose/grip/timing/gameplay-camera QA.

`TowerSupportMechanismPresentation` drives visible mechanism feedback for Ballista, Apollo shrine and Fire Tower. These remain presentation/production candidates, not proof of final authored animation quality.

## Production art status

`MODEL_ART_INVENTORY.md` is authoritative for asset completion.

Current rules:

- third-party sources are source material/candidate bases, not completion by themselves;
- generated character prefabs remain `GENERATED PLACEHOLDER` until accepted;
- procedural tower/environment geometry remains `PROCEDURAL` until replaced/promoted;
- `DONE` requires final derivative assets under `Assets/Game/Art/...`, runtime adoption and real visual QA.

Runtime visual binding now reports the actual source used for heroes, Greek enemies and Trojan tower crews (`PRODUCTION_RESOURCE`, `GENERATED_RESOURCE` or `PROCEDURAL_FALLBACK`). Archer, Spear Wall and Trojan Guard tower crews also have role-specific procedural silhouettes when their generated/production prefab resources are absent, so a clean checkout no longer silently loses those visible defenders. These fallback figures remain `PROCEDURAL`, not final production art.

Current Chapter I final production-art counts remain zero for characters, Tower-Units and environment families until that acceptance gate is actually passed.

## Validation tooling

Implemented:

- architecture/static contract checker;
- Unity architecture smoke validator;
- Chapter I release-candidate validator;
- authored encounter contract checks;
- EditMode architecture/data/save/gameplay-acceptance contract tests;
- PlayMode runtime/gameplay acceptance tests;
- Chapter I playthrough reporter;
- Chapter I playthrough analyzer;
- Chapter I gameplay-freeze readiness validator;
- Chapter I Story acceptance manifest binding + SHA-256 evidence;
- Chapter I WARN review template generation;
- Story/Strategos/Legendary pressure comparison analyzer;
- RU/EN visual-fit QA presets/checklist;
- final Chapter I gameplay acceptance validator;
- Chapter I art-freeze validator.

Static validation is not a substitute for the required real Chapter I runs or visual inspection.

## Remaining Chapter I work

### Gameplay RC

Tooling for steps 5–8 is implemented; the human/Play Mode evidence is not fabricated.

1. Complete one clean Story run at 1x/no-pause with the current authored EncounterData.
2. Run the latest-playthrough analyzer.
3. Run Gameplay Freeze Readiness.
4. Resolve every hard blocker.
5. Bind the freeze-ready Story candidate and resolve/explicitly accept every WARN finding.
6. Record human Story acceptance in `CHAPTER_I_GAMEPLAY_ACCEPTANCE.json`.
7. Complete Strategos + Legendary 1x/no-pause runs, review the difficulty pressure report, and record the decisions.
8. Perform and record the 1920x1080 + 1366/1376x768 RU/EN visual-fit matrix.
9. Run the final gameplay acceptance validator and set `gameplayFrozen=true` only when it reports `READY TO FREEZE`.

No balance freeze is claimed yet because the required real playthrough/visual evidence has not been accepted in this work session.

### Production art

P0 order:

1. Hector + Menelaus final character acceptance.
2. Chapter I Greek regulars: Infantry / Runner / Heavy Hoplite / Shield Bearer / Archer.
3. Trojan Guard / Archer and all six defensive structures/units.
4. Greek landing ships + coast kit.
5. Troy wall/gate environment kit.
6. Final authored/retargeted animation review.

### Performance

Profile real Chapter I density before expanding campaign-scale spawn pressure. Projectile pooling already exists; add broader pooling only where measurements justify it.

## Next product gate

Do not treat Chapter I as the campaign baseline until both are true:

**Gameplay RC:** clean Story 1x/no-pause run -> hard freeze readiness -> WARN review -> human Story acceptance -> Strategos/Legendary pressure review -> RU/EN 16:9 QA -> explicit final gameplay freeze.

**Production art:** required P0 Chapter I assets satisfy `MODEL_ART_INVENTORY.md` acceptance and are visually verified in real gameplay.

The encounter/runtime architecture is ready to support Chapter II content authoring, but full Chapter II production should still wait for the Chapter I gameplay baseline to be accepted.
