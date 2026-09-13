# TheTroyGame — Project Status

Last reviewed: 2026-09-13

This document is the canonical answer to **what is implemented now**. It intentionally does not duplicate the GDD or roadmap.

## Current phase

**Chapter I: The Landing — gameplay release candidate + production-art candidate pass.**

Chapter I is functionally playable as a vertical slice, but it is **not frozen** yet. Two independent gates remain:

1. Gameplay RC gate — real 1x playthrough, telemetry analysis, difficulty pressure checks, RU/EN visual-fit QA.
2. Production-art gate — promote P0 assets from generated/procedural candidates to accepted final art after Play Mode visual QA.

Chapter II unlock plumbing exists, but Chapter II content is not implemented. Chapters III–VII remain design/roadmap work.

## Canonical project structure

Runtime code:

```text
Assets/Game/
├── Core/
├── Campaign/
├── Combat/
├── Towers/
├── Enemies/
├── Heroes/Hector/
├── World/
├── UI/
├── Audio/
└── VFX/
```

`Assets/Scripts` is legacy and must not be recreated.

Runtime currently remains in one intentional `TheTroyGame.Runtime` assembly because module references still cross folder boundaries. Assembly splitting is deferred until contracts/events remove those cycles.

## Current gameplay systems

Implemented:

- explicit game-state flow;
- Gold economy and chapter scoring;
- data-driven `TowerData`, `EnemyData`, `WaveData`, `ChapterData`;
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
- Menelaus boss, commander aura, reinforcement calls and final-wave objective integration;
- Chapter I victory requires Menelaus defeated;
- Menelaus reaching the gate damages it over time; defeat occurs when gate HP reaches zero;
- EN/RU gameplay text and language switching;
- save/unlock flow for Chapter I → Chapter II;
- bounded combat-speed controls;
- Chapter I telemetry and playthrough reporting.

Pending core mechanics:

- Stun / Fear shared status implementations;
- tower specialization branches after Level 3;
- obstacle-aware hero/path movement if later maps require it;
- pooling before campaign-scale density unless Chapter I profiling proves it necessary sooner.

## Chapter I content contract

Current target:

- 5 combat events;
- approximately 11–13 minutes at 1x;
- coast / Greek landing setting;
- Menelaus climax;
- tutorialized defensive placement + Hector controls;
- canonical Chapter II unlock on valid victory.

The playthrough reporter writes JSON + per-wave CSV under `Application.persistentDataPath/Logs` with:

- result and difficulty;
- total duration and per-wave actual/target duration;
- kills / leaks;
- Gold earned / spent / refunded;
- gate HP;
- tower mix;
- Menelaus result;
- peak alive-enemy pressure;
- average FPS.

`TheTroyGame/Validation/Analyze Latest Chapter I Playthrough` produces an RC Markdown report with PASS/WARN/FAIL findings and wave-local tuning actions.

Exact manual protocol: `CHAPTER_I_RC_PLAYTEST.md`.

## UI / combat readability

Implemented presentation work includes:

- responsive combat HUD for 1920x1080 and 1366/1376x768 target layouts;
- separate wave, boss, Hector, build and selected-unit regions;
- selected-defense contextual menu around the unit;
- tower hover/selection/range feedback;
- projectile trails and type-specific hit feedback;
- tower recoil;
- Menelaus aura, boss warning and longer boss death presentation;
- Hector ability pulses and hit feedback;
- improved coast, road, Greek landing, ships, Trojan gate/walls, banners, braziers and atmosphere;
- cinematic Chapter I opening camera pass.

Final RU/EN 16:9 Play Mode QA is still required before Chapter I freeze.

## Character and defensive-unit animation candidates

Role-specific Animator profiles now exist for:

- generic infantry;
- spear infantry;
- archer;
- skirmisher;
- Hector;
- Menelaus;
- Ballista crew;
- Apollo priest;
- Fire Keeper.

Runtime presentation hooks include:

- Hector: Attack / Q / E / R / F / Hit / Downed;
- Menelaus: Command;
- Trojan Guard: Block / Poke;
- Spear Wall: Poke;
- Archer Post: Draw / Release;
- Ballista: Fire / Reload / Tension;
- Priests of Apollo: Cast / Channel;
- Fire Keeper: Throw / Stoke.

`TowerSupportMechanismPresentation` drives visible mechanism feedback for Ballista, Apollo shrine and Fire Tower. The old procedural enemy bob/squash layer no longer fights authored Animator controllers.

These are **presentation/production candidates**, not proof of final production animation quality.

## Production art status

`MODEL_ART_INVENTORY.md` is authoritative for asset completion.

Current rules:

- KayKit and other third-party sources are source material / candidate bases, not final completion by themselves;
- generated character prefabs remain `GENERATED PLACEHOLDER` until accepted;
- procedural tower/environment geometry remains `PROCEDURAL` until replaced/promoted;
- `DONE` requires final derivative assets under `Assets/Game/Art/...`, runtime adoption, and real Play Mode visual QA.

Recent P0 presentation passes improved:

- Hector silhouette;
- Menelaus silhouette;
- Trojan Guard;
- Trojan Archer / Archer Post;
- Spear Wall;
- Ballista + crew;
- Priests of Apollo;
- Fire Keeper / Fire Tower.

This does **not** change their production-art acceptance status automatically.

## Automated validation

Implemented:

- `python tools/check-architecture.py`;
- Unity architecture smoke validator;
- Chapter I release-candidate validator;
- EditMode architecture/data/save tests;
- PlayMode runtime and gameplay acceptance tests;
- Chapter I pacing contract;
- Chapter I playthrough reporter + analyzer;
- CI workflow `.github/workflows/unity-ci.yml`.

The Chapter I RC validator protects current presentation contracts including defensive-unit/support animation hooks and production-art wiring.

### Verified state

- Architecture guard: verified green on current Chapter I work.
- A previous local Windows build after UI polish was reported green with 0 errors.
- GitHub Unity EditMode/PlayMode/build jobs are **not considered verified** while repository Unity activation is not configured.

Static inspection or architecture success must never be described as Unity compile/test/build success.

## Remaining Chapter I work

### Gameplay RC

1. Complete one clean Story run at 1x.
2. Run the latest-playthrough analyzer.
3. Resolve all FAIL findings.
4. Resolve or explicitly accept WARN findings.
5. Freeze Story pacing/economy/pressure baseline.
6. Repeat pressure validation on Strategos and Legendary.
7. Perform final 1920x1080 + 1366/1376x768 RU/EN Play Mode visual-fit QA.

### Production art

P0 order:

1. Hector + Menelaus final character acceptance.
2. Chapter I Greek regulars: Infantry / Runner / Heavy Hoplite / Shield Bearer / Archer.
3. Trojan Guard / Archer and all six defensive structures/units.
4. Greek landing ships + coast kit.
5. Troy wall/gate environment kit.
6. Final authored/retargeted animation review.

### Performance

Profile real Chapter I density before adding pooling. Pooling becomes mandatory before campaign-scale content if allocations or frame spikes are visible.

## Deferred infrastructure

Unity CI activation and a full green compile → EditMode → PlayMode → build cycle are deferred until repository Unity activation is configured.

## Next product gate

Do not treat Chapter I as the campaign baseline until both are true:

**Gameplay RC:** clean Story 1x run → analyzer has no unresolved FAIL → Strategos/Legendary pressure check → RU/EN 16:9 QA.

**Production art:** P0 Chapter I assets satisfy `MODEL_ART_INVENTORY.md` acceptance and are visually verified in real Play Mode.

Only after both gates close should Chapter II move into full content production.
