# TheTroyGame — Unity Roadmap

Last reviewed: 2026-09-14

This roadmap answers **what should be built next**. Current implementation truth lives in `PROJECT_STATUS.md`; game-design truth lives in `GDD.md`.

## Production principles

1. Build on the canonical runtime under `Assets/Game`; never recreate `Assets/Scripts`.
2. Use authored `TowerData`, `EnemyData`, `EncounterData`, and `ChapterData` as runtime sources of truth.
3. Encounter composition/pacing belongs in `EncounterData`, never in hidden spawn-index formulas.
4. Chapter-specific map/presentation composition belongs behind a chapter runtime profile/installer, never directly in `GameBootstrap`.
5. Do not start full production of a later chapter before the previous chapter passes its required gate.
6. Measure pacing/economy/pressure through telemetry instead of balancing only by feel.
7. Production art is `DONE` only under the acceptance rules in `MODEL_ART_INVENTORY.md`.
8. Keep shared combat, save, input, UI and campaign systems reusable across chapters.

## Campaign target

| Chapter | Target duration | Major events | Core focus |
|---|---:|---:|---|
| I. The Landing | 11–13 min | 5 | Tutorial, coast defense, Menelaus |
| II. Road to Troy | 14–16 min | 6 | Multi-route pressure, mobility |
| III. The Gates | 18–20 min | 7 | Siege engines, gate/wall defense |
| IV. Heroes of Greece | 17–19 min | 6 | Elite enemies, Ajax/Achilles |
| V. The Great Assault | 20–22 min | 7 | Full-system siege |
| VI. The Horse | 14–16 min | ~4 | Narrative inversion, choices |
| VII. Troy Burns | 16–18 min | ~5 phases | Continuous survival finale |

Campaign target: approximately 118–122 minutes / ~40 major combat events.

---

# M0 — Architecture foundation — COMPLETE

Completed baseline:

- canonical `Assets/Game` module structure;
- explicit runtime state;
- campaign/chapter controllers;
- registries instead of scene-wide gameplay scans;
- reusable TowerData/EnemyData contracts;
- authored ChapterData/EncounterData campaign content;
- generic `GameBootstrap` -> `ChapterRuntimeInstaller` boundary;
- dedicated `ChapterOneRuntimeInstaller` for coast/map/Hector/cinematic/guidance wiring;
- generic EnemySpawner executing authored encounter plans;
- explicit enemy runtime behavior registry for special encounter behaviors;
- shared damage/status pipeline;
- input abstraction;
- save/versioning foundation;
- EditMode/PlayMode test assemblies;
- architecture/release validators.

`WaveData` is legacy compatibility only and is not the runtime content-authoring path for future chapters.

No broad architecture rewrite is required before Chapter I freeze. Further refactors should be driven by a concrete Chapter II+ requirement.

---

# M1 — Chapter I gameplay RC — CURRENT

## Goal

Freeze Chapter I gameplay as the reference implementation for the rest of the campaign.

## Already implemented

- 5 authored EncounterData assets;
- Chapter I base encounter counts 8 / 12 / 16 / 20 / 25;
- authored Menelaus behavior binding in Encounter 5;
- current Story/Strategos/Legendary count composition preserved through the data migration;
- six defense roles;
- build/upgrade/sell/priority flow;
- one-shot build mode and contextual selected-defense UI;
- Hector Q/E/R/F;
- Menelaus final encounter;
- save/unlock Chapter II;
- deterministic EN/RU language selection;
- telemetry/reporter/analyzer;
- verified 1x/no-pause telemetry contract;
- Chapter I gameplay-freeze readiness validator;
- tracked `CHAPTER_I_GAMEPLAY_ACCEPTANCE.json` manifest;
- exact Story candidate binding by telemetry session id + SHA-256;
- WARN-review template generation;
- Story/Strategos/Legendary comparative pressure analyzer;
- 1920x1080 and 1376x768 RU/EN visual-fit presets + QA checklist;
- final gameplay acceptance validator with `BLOCKED` / `READY TO FREEZE` / `PASS` states;
- Chapter I release-candidate validator;
- responsive HUD target layout;
- current combat/readability pass.

## Remaining evidence gate

The tooling for steps 5–8 is complete. The remaining work is real Play Mode/human evidence and must not be fabricated:

1. Complete a clean Story run at 1x/no-pause using the current EncounterData.
2. Run the analyzer and hard Gameplay Freeze Readiness validator.
3. Bind the freeze-ready Story report to the acceptance manifest.
4. Fix or explicitly accept every analyzer WARN and record the decision.
5. Record explicit human Story acceptance.
6. Complete clean 1x/no-pause Strategos and Legendary runs and review the comparative pressure report.
7. Complete 1920x1080 + 1366/1376x768 RU/EN visual-fit QA and record evidence/limitations.
8. Run the final acceptance validator; set `gameplayFrozen=true` only when it reports `READY TO FREEZE`.

Changing the bound Story report invalidates downstream WARN, human, difficulty, visual-fit and freeze approvals, preventing stale acceptance from carrying over to changed balance.

Definition of Done: `CHAPTER_I_RC_PLAYTEST.md` and `CHAPTER_I_GAMEPLAY_ACCEPTANCE.md` pass with explicit accepted evidence and the final validator reports `PASS`. `READY FOR HUMAN ACCEPTANCE` alone is not the final freeze.

---

# M2 — Chapter I production-art freeze — CURRENT IN PARALLEL

## Goal

Replace or formally accept the visible candidate art needed for a showable Chapter I.

## P0 order

1. Hector.
2. Menelaus.
3. Greek Infantry / Runner / Heavy Hoplite / Shield Bearer / Archer.
4. Trojan Guard / Trojan Archer.
5. Archer Post / Ballista / Priests of Apollo / Spear Wall / Fire Tower / Trojan Guard station.
6. Greek ships and landing props.
7. Coast kit.
8. Troy wall/gate kit.
9. Final authored/retargeted animation review.

Current runtime already exposes animation hooks for Hector, Menelaus, Guard, Spear Wall, Archer Post, Ballista crew, Apollo priests and Fire Keeper. Character-combat presentation also includes weapon release sockets, animation-phase impacts, projectile flights, Greek Archer nocked-arrow lifecycle and Hector carried-spear flight/restore behavior. These support final art but do not make candidate art final by themselves.

Definition of Done: required P0 rows in `MODEL_ART_INVENTORY.md` satisfy the `DONE` contract and Chapter I passes real visual QA.

---

# M3 — Browser/Web release path

Start once Chapter I is stable enough that browser-specific debugging will not hide gameplay churn.

## Required work

- Unity Web build command in the editor/build pipeline;
- responsive 16:9 web shell;
- fullscreen handling;
- audio start/unmute after user gesture;
- browser-safe save persistence validation;
- loading/progress presentation;
- static deployment artifact;
- static hosting deployment;
- optional PWA manifest/service worker after baseline Web build is proven;
- browser performance pass at Chapter I peak pressure.

Desktop web is the first target. Mobile browser controls are a separate milestone because the current game assumes mouse/keyboard-style interaction and Hector abilities.

Definition of Done: fresh browser session can load, play Chapter I, save progress, reload, and complete the chapter on supported desktop browsers without layout or audio blockers.

---

# M4 — Chapter II: Road to Troy

Target: 14–16 minutes / 6 encounters.

## Foundation already available

Chapter II no longer needs a second spawn-loop implementation. It should reuse:

- `ChapterData`;
- `EncounterData` spawn groups;
- generic `EnemySpawner`;
- generic difficulty scaling;
- `ChapterRuntimeInstaller` boundary;
- existing combat/economy/save/UI contracts.

## New requirements

- authored `Chapter02` ChapterData;
- six authored EncounterData assets;
- dedicated Chapter II runtime profile/installer;
- 2–3 route pressure patterns authored in encounter data;
- clearer lane-pressure preview where required;
- Chariot gameplay support;
- shielded formations and more varied enemy mix;
- chapter-specific map/environment kit;
- chapter result and unlock flow using existing campaign contracts.

Avoid adding a second implementation of systems already solved in Chapter I.

Definition of Done: chapter is authored through shared systems, uses no hidden source-code composition table, and lands in the 14–16 minute target without bespoke combat forks.

---

# M5 — Chapter III: The Gates

Target: 18–20 minutes / 7 encounters.

Required additions:

- dedicated destructible-objective framework;
- gate/wall damage states;
- repair economy;
- battering ram / siege tower / sapper behaviors;
- breach-driven route changes;
- wall defense positions;
- siege-specific VFX/readability.

Definition of Done: siege units interact with objectives as siege units, temporary breach alters the battlefield, and repairs are strategically meaningful.

---

# M6 — Chapter IV: Heroes of Greece

Target: 17–19 minutes / 6 encounters.

Required additions:

- reusable multi-phase boss framework;
- reusable boss intro/outro presentation;
- elite resistance/aura profiles;
- Ajax encounter;
- Achilles encounter;
- Hector combat/progression expansion if needed by the chapter.

Definition of Done: Ajax and Achilles demand visibly different tactics and are implemented through reusable boss contracts rather than encounter-loop special cases.

---

# M7 — Chapter V: The Great Assault

Target: 20–22 minutes / 7 encounters.

Focus:

- full tower roster;
- infantry + heavy + siege + elite formations;
- route disruption;
- disabled/destroyed defense positions;
- wall events and fire hazards;
- Trojan reinforcement set pieces;
- campaign-scale performance profiling and pooling where required.

Definition of Done: all established combat systems combine without one universal dominant defense and without frame/allocation regressions at intended maximum density.

---

# M8 — Chapter VI: The Horse

Target: 14–16 minutes / ~4 major events.

This is intentionally not a normal TD chapter.

Required additions:

- narrative event/state presentation;
- scouting interactions;
- campaign-choice persistence;
- chapter modifiers;
- day/night transition;
- choices that materially modify Chapter VII.

The canonical fall of Troy remains fixed; choices change tactical conditions/performance, not the historical ending.

---

# M9 — Chapter VII: Troy Burns

Target: 16–18 minutes / ~5 continuous survival phases.

Required additions:

- internal Greek spawn points;
- dynamic route activation;
- evacuation objectives;
- civilian tracking;
- fire/hazard propagation;
- blocked/destructible streets;
- survival timer;
- Odysseus encounter;
- campaign result aggregation and epilogue.

Definition of Done: finale is structurally different from Chapters I–V, Chapter VI choices matter, and the complete campaign ends with Troy's canonical fall plus performance scoring.

---

# M10 — v1.0 completion pass

## Balance

- Story / Strategos / Legendary campaign curves;
- economy and refund curves;
- tower counter matrix;
- Hector progression/cooldowns;
- boss duration and readability;
- chapter pacing.

## Production

- final art/audio/VFX acceptance;
- loading and transition polish;
- accessibility/settings pass;
- save migration/backup verification;
- desktop/Web performance pass;
- clean release builds.

## Documentation gate

Before v1.0, `PROJECT_STATUS.md`, `UNITY_ROADMAP.md`, `MODEL_ART_INVENTORY.md`, `GDD.md`, architecture and third-party provenance documents must agree with shipped behavior.
