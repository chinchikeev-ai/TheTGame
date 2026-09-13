# TheTroyGame — Unity Roadmap

Last reviewed: 2026-09-13

This roadmap answers **what should be built next**. Current implementation truth lives in `PROJECT_STATUS.md`; game-design truth lives in `GDD.md`.

## Production principles

1. Build on the canonical runtime under `Assets/Game`; never recreate `Assets/Scripts`.
2. Use authored `TowerData`, `EnemyData`, `WaveData`, and `ChapterData` as runtime sources of truth.
3. Do not start full production of a later chapter before the previous chapter passes its required gate.
4. Measure pacing/economy/pressure through telemetry instead of balancing only by feel.
5. Production art is `DONE` only under the acceptance rules in `MODEL_ART_INVENTORY.md`.
6. Do not claim Unity compile/tests/build are green unless they actually ran and passed.
7. Keep shared combat, save, input, UI and campaign systems reusable across chapters.

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
- data-driven tower/enemy/wave/chapter configuration;
- shared damage/status pipeline;
- input abstraction;
- save/versioning foundation;
- EditMode/PlayMode test assemblies;
- architecture guard and CI workflow.

No further architecture rewrite is required before Chapter I freeze unless a concrete blocker appears.

---

# M1 — Chapter I gameplay RC — CURRENT

## Goal

Freeze Chapter I gameplay as the reference implementation for the rest of the campaign.

## Already implemented

- 5-event chapter contract;
- six defense roles;
- build/upgrade/sell/priority flow;
- one-shot build mode and contextual selected-defense UI;
- Hector Q/E/R/F;
- Menelaus final encounter;
- save/unlock Chapter II;
- EN/RU;
- telemetry/reporter/analyzer;
- Chapter I release-candidate validator;
- responsive HUD target layout;
- current combat/readability pass.

## Remaining gate

1. Clean Story run at 1x.
2. Analyze latest playthrough.
3. Resolve all FAIL findings.
4. Resolve/accept WARN findings.
5. Freeze Story pacing/economy/pressure.
6. Repeat pressure check on Strategos and Legendary.
7. Run 1920x1080 and 1366/1376x768 RU/EN visual-fit QA.

Definition of Done: `CHAPTER_I_RC_PLAYTEST.md` passes without unresolved blockers.

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

Current runtime already exposes animation hooks for Hector, Menelaus, Guard, Spear Wall, Archer Post, Ballista crew, Apollo priests and Fire Keeper. These hooks support final art but do not make candidate art final by themselves.

Definition of Done: required P0 rows in `MODEL_ART_INVENTORY.md` satisfy the `DONE` contract and Chapter I passes real Play Mode visual QA.

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
- GitHub Pages or equivalent static hosting deployment;
- optional PWA manifest/service worker after baseline Web build is proven;
- browser performance pass at Chapter I peak pressure.

Desktop web is the first target. Mobile browser controls are a separate milestone because the current game assumes mouse/keyboard-style interaction and Hector abilities.

Definition of Done: fresh browser session can load, play Chapter I, save progress, reload, and complete the chapter on supported desktop browsers without layout or audio blockers.

---

# M4 — Chapter II: Road to Troy

Target: 14–16 minutes / 6 events.

## New requirements

- authored Chapter II data, no source-code wave authoring;
- 2–3 route pressure patterns;
- clearer lane-pressure preview;
- Chariot gameplay support;
- shielded formations and more varied enemy mix;
- chapter-specific environment kit;
- chapter result and unlock flow using existing campaign contracts.

Avoid adding a second implementation of systems already solved in Chapter I.

Definition of Done: chapter is fully authored through shared systems and lands in target duration without bespoke combat forks.

---

# M5 — Chapter III: The Gates

Target: 18–20 minutes / 7 events.

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

Target: 17–19 minutes / 6 events.

Required additions:

- reusable multi-phase boss framework;
- reusable boss intro/outro presentation;
- elite resistance/aura profiles;
- Ajax encounter;
- Achilles encounter;
- Hector combat/progression expansion if needed by the chapter.

Definition of Done: Ajax and Achilles demand visibly different tactics and are implemented through reusable boss contracts rather than wave-loop special cases.

---

# M7 — Chapter V: The Great Assault

Target: 20–22 minutes / 7 events.

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
- clean release builds;
- complete CI compile/test/build once Unity activation is configured.

## Documentation gate

Before v1.0, `PROJECT_STATUS.md`, `UNITY_ROADMAP.md`, `MODEL_ART_INVENTORY.md`, `GDD.md`, architecture and third-party provenance documents must agree with shipped behavior.
