# TheTroyGame — Project Status

Last reviewed: 2026-09-15

This document is the canonical answer to **what is implemented now**. It intentionally does not duplicate the GDD or roadmap.

## Current phase

Combat HUD wave pass (2026-09-20): replaced the layered rectangular wave panel with
a separate crimson/gold laurel banner and live title, countdown and progress text.
Patron portrait import now retains RGBA rather than Alpha8, fixing white silhouettes.
Four focused EditMode render checks passed (all patrons plus skin preservation).
Full in-game layout acceptance and EXE build remain pending; other HUD blocks unchanged.

Chapter I PlayMode regression hardening (2026-09-20): tests now match the actual Hector HUD interaction contract (`HectorPanel` is the clickable card), cover direct Hector movement after a screen-space command, verify Restart rebuilds exactly one runtime graph/input/EventSystem, and verify duplicate `GameBootstrap` instances self-destruct instead of creating a second graph. Source contracts are complete; Unity PlayMode execution and the RU/EN visual matrix remain pending.

Explicit runtime graph pass (2026-09-20): `GameBootstrap` now constructs `GameRuntimeContext` with Core/Chapter/UI ownership roots and explicit service references. `ChapterRuntimeInstaller` receives that context, and `ChapterOneRuntimeInstaller` creates Chapter I components through `CreateChapter<T>()` instead of `FindFirstObjectByType`/generic `EnsureComponent<T>()`. `ChapterRuntimeContext` now exposes paths, `MapBuilder`, `TowerPlacement` and Hector. `RuntimeInputBootstrap` directly owns its EventSystem, and Chapter I atmosphere receives camera/sun explicitly. `SampleScene` now binds its existing Directional Light as `RenderSettings.sun`. Architecture and PlayMode contracts were extended; Unity compile/PlayMode validation remains pending.

Owned-root reference pass (2026-09-20): Chapter I world/UI composition and menu/combat presentation binding now use explicit owner/runtime references instead of canonical-name rediscovery. The remaining `GameCanvas` lookup was removed from `ModernCombatHud`. The architecture checker now rejects `GameObject.Find("Chapter01_...")` and canonical owned-root name lookups such as `MenuCanvas`, `ModernCombatHUD`, `HectorHUD`, `SelectedTowerCard`, and `CombatActions`. Unity compile/PlayMode validation remains pending.

Runtime presentation ownership pass (2026-09-20): remaining presentation self-install hooks were classified and removed. `GameMenuController` now owns main-menu/menu-flow presenters, `ModernCombatHud` owns combat-HUD presenters, and `HectorHUD` is exclusively Chapter I installer-owned. Only `GameBootstrap`, `CampaignSave`, `RuntimeFileLogger`, and `BuildVersionOverlay` remain approved runtime initializer files. The architecture checker enforces these ownership boundaries and the hot-path scene-search rule. Unity compile/PlayMode validation remains pending.

Settings/pre-map repair (2026-09-20): reference-derived fullscreen settings artwork
uses live Audio/Video/Gameplay/Controls content. Audio exposes master/music only.
Illustrated chapter start now binds explicitly to difficulty/patron selection,
fixing the text-based binding regression. First-wave button reflects mandatory
preparation and patron readiness instead of remaining permanently disabled.
Focused EditMode checks added; full gameplay flow and Windows EXE validation pending.
Chapter I lifecycle ownership pass (2026-09-20): `ChapterOneRuntimeInstaller` is now the explicit owner of the Chapter I-specific presentation stack (shore/sea passes, battlefield details, Troy backdrop/fire/gate damage, faction staging, Menelaus entrance, encounter presentation and compact Chapter I HUD pass). Their independent `RuntimeInitializeOnLoadMethod` self-install hooks were removed, and the architecture checker now verifies both installer ownership and absence of self-install hooks. Shared/global runtime auto-start components remain outside this pass. Unity compile/PlayMode validation remains pending.

Runtime lookup hardening (2026-09-20): the remaining Chapter I `FindObjectsByType` runtime violation was removed from the Aegean sea presentation and replaced by hierarchy-scoped traversal. `EnemySpawner`, `TowerPlacement`, `GameMenuController` and `ModernCombatHud` now expose stable runtime references used by recurring UI/presentation paths. Known `GameObject.Find` / `FindFirstObjectByType` calls were removed from `Update` / `LateUpdate` consumers, including the compact Chapter I HUD pass, which now performs bounded startup binding. `tools/check-architecture.py` now rejects direct scene lookup in `Update`, `LateUpdate` and `FixedUpdate` while still allowing bounded startup/bootstrap lookup. Unity compile/PlayMode validation remains pending.

Chapter selection fullscreen fix (2026-09-20): map now fills the viewport, while
the detail panel stays right-aligned and controls keep uniform scale. The legacy
CampaignMapPresentation skips this screen, preventing the old map showing beneath it.
Focused tests cover 4:3, 16:9 and ultrawide background bounds and legacy suppression.

Chapter selection artwork (2026-09-19): the old chapter list is replaced with a
reference-derived coastal map, separate chapter nodes, detail panel and start/back
buttons. Chapter I uses the existing start action; II-V remain non-playable with
locked/in-development messaging. RU/EN and responsive Canvas checks added; full
gameplay acceptance pending. No EXE build.

PlayMode compile fix (2026-09-19): added the explicit Unity.InputSystem assembly
reference required by CombatHudLayoutUxTests. Unity batch compilation completed
with exit code 0. No PlayMode execution or Windows player build in this check.
Architecture guard still reports the unrelated ChapterOneAegeanSeaPresentation search.

Runtime input/HUD perimeter pass (2026-09-19): runtime EventSystem ownership is now centralized in `RuntimeInputBootstrap`, created by `GameBootstrap` before chapter/UI composition. Chapter I and menu enhancement no longer create EventSystems. The combat HUD factory defaults decorative panels/text/icons to non-raycast Graphics, while interactive Buttons retain pointer ownership. Hector is compacted to ~78% scale; MAGIC and DEFENDERS are equal-size bottom-right actions; the expanded defender strip is bottom-center. PlayMode contracts were extended for these rules. Unity compile/PlayMode and RU/EN resolution QA remain pending in this GitHub-only session.

Pause-menu artwork (2026-09-18): six supplied PNG elements now decorate the
existing in-game pause buttons over a blocking dimmed battlefield. Controller
callbacks and the existing settings screen are unchanged. RU uses supplied
lettering; EN uses live text on framed panels. Focused Canvas tests cover five
preserved button callbacks and 16:9/4:3 layouts. Full gameplay QA pending; no EXE.

Gate HUD artwork (2026-09-18): compact dark/gold frame and separate illustrated gate
replace the gate resource placeholder. ModernCombatHud retains live HP/localization;
the decorative skin preserves the new icon. Three focused EditMode tests passed,
with full/half/empty HP Canvas captures. User acceptance and gameplay QA pending.
No EXE build. Architecture guard currently reports an unrelated scene-wide search
in ChapterOneAegeanSeaPresentation.

Main-screen artwork (2026-09-16): the production main menu now uses the supplied
MainScreen logo, Play, Heroes, Settings and Exit PNGs over the clean Troy background.
Textures are packaged under `Assets/Game/Art/Resources/MainScreen`. Buttons retain
the existing chapter/settings/quit flow; Heroes opens Hector information with modal
input blocking and a Back action. RU captions, hover feedback, a Settings tooltip
and uniform layout scaling are implemented. Unity compilation and ten focused
EditMode tests passed, including controller navigation/rebinding and eight RU/EN
Canvas captures at 1600x900, 1366x768, 1024x768 and 2560x1080. Architecture guard
passed. Full PlayMode acceptance and a new Windows build are not claimed.

Patron selection artwork (2026-09-15): the pre-map screen uses the supplied Gods
background and four separate illustrated cards. A pending selection is highlighted;
only confirmation applies the gift through the existing controller. Back returns to
difficulty. English captions cover the baked Russian text, and layout scales uniformly.
Runtime textures live under Assets/Game/Art/Resources/PatronSelection, not Pictures.
Unity compilation and eleven focused EditMode tests passed, including six isolated
RU/EN Canvas captures at 1600x900, 1366x768 and 1024x768. Full menu-to-battle visual
acceptance remains pending; no new Windows build is claimed.

Build badge recovery (2026-09-15): `MainMenuBuildVersionPresentation` reactivates
its existing badge and restores its sibling order after approved menu art disables
older children or appends a background. The original `ef4ef5cd` player built
successfully; this visibility failure was a menu lifecycle issue, not a missing push.

Hector HUD illustrated candidate (2026-09-15): eleven separate bitmap assets now
replace its procedural decoration, with live localized labels, portrait selection,
health and four ability cooldowns. Unity compilation, isolated RU/EN Canvas renders
at 1600x900 / 1280x720, text-fit and five-button checks passed. Full gameplay visual
acceptance and a new Windows player build are not claimed.

Unified combat HUD source follow-up (2026-09-15): the transferred `b8f03e2` work is
present in `main` and the post-merge layout ownership has been reconciled. Guidance
reserves the top-left resource zone, transient notifications reserve Hector's corner,
next-encounter portrait cards no longer share the progress region, objective copy
avoids duplicate gate/encounter telemetry, and enemy inspection yields to blocking
menus. PlayMode contracts cover these ownership/layout rules, but this GitHub-only
session could not run Unity, the target RU/EN resolution matrix or a new Windows build.

Combat HUD visual-hierarchy pass (2026-09-15): Gold, combat speed and Settings now
form the canonical top-left utility row with Gate health directly below; speed has been
removed from the encounter center. Defender cards, hover details and selected-defense
context are more strongly portrait-led; enemy inspection and Menelaus use larger
character portraits; contextual tutorial guidance uses the existing Hector parchment
art; and next-encounter/notification cards were tightened for battlefield readability.
Settings remains owned by `GameMenuController`; the combat HUD delegates through the
small `CombatMenuBridge` without changing the restored main-menu layout. This source
pass still requires real RU/EN Play Mode QA.

Chapter I opening-flow presentation pass (2026-09-15): the opening camera, Greek fleet
approach and decorative landing party now hand off to the authored Encounter flow rather
than attempting to start Encounter 1. `EnemySpawner` + `EncounterData` remain the sole
authority for preparation timing and combat start. After Encounter 1 the presentation
shows a first-assault-repelled lull, stages reinforcement galleys and a regrouping Greek
beachhead, then clears that temporary formation when Encounter 2 actually begins. The
flow is source-complete but still requires real Unity timing, camera and RU/EN visual QA.

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

The former `WaveData` pipeline was removed on 2026-09-14. `EncounterData` is the sole authored combat composition/pacing authority. Remaining Wave-prefixed members in `EnemySpawner`, `GameManager`, HUD hierarchy names and historical telemetry are compatibility naming/schema debt only; do not reintroduce `WaveData`.

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

- responsive combat HUD targeting 1920x1080 and 1366/1376x768 layouts;
- unified Trojan combat HUD cards using the same dark-stone, bronze-framed, portrait-led language as the Hector block;
- top-left Gold + combat speed + compact Settings utility row with Gate health directly underneath;
- encounter center reserved for encounter number/timer, threat, progress and Start Encounter instead of speed/settings utility controls;
- one direct Divine Power action with effect/readiness copy instead of competing duplicate controls;
- enlarged portrait-led defender build cards with hotkey, localized name and Gold cost;
- portrait-led defender hover details and selected-defense contextual card;
- guidance/objective cards reserved below the top-left resource/gate cluster;
- contextual tutorial/advice presented on the existing Hector parchment art;
- transient notifications placed above the lower-left Hector block and hidden when empty;
- larger next-encounter portrait cards separated from centered encounter progress;
- objective guidance copy avoids repeating gate HP and encounter progress already visible in the primary HUD;
- enemy inspector uses Trojan panel/large-portrait hierarchy and hides behind blocking menus;
- Menelaus boss HUD uses a dedicated large portrait with name/role/HP/mechanics hierarchy;
- separate encounter, boss, Hector, build and selected-unit regions;
- selected-defense contextual menu around the unit;
- pre-map Patron God selection; the selected Patron cannot be changed after the map starts;
- upper-right selected Patron portrait/commentary for Chapter I event feedback;
- obsolete in-map `DivineGiftChoice` / `CombatActions` removed from `ModernCombatHud`; the combat HUD now uses direct `EncounterRuntime` state and Encounter/Бой player-facing copy without `CombatHudLegacyCleanup`;
- distinct EN/RU patron reactions for wave flow, defenses, gate pressure, kill milestones, Hector, Menelaus and Divine Power use;
- tower hover/selection/range feedback;
- projectile trails and type-specific hit feedback;
- tower recoil;
- Menelaus aura, boss warning and longer boss death presentation;
- Hector ability pulses and hit feedback;
- improved coast, road, Greek landing, ships, Trojan gate/walls, banners, braziers and atmosphere;
- cinematic Chapter I opening camera pass;
- staged Chapter I opening handoff: fleet approach + decorative shore formation during authored preparation, Encounter 1 reaction, first-assault lull, reinforcement beachhead cue and cleanup on actual Encounter 2 start;
- gameplay-camera character combat presentation pass for Greek Archer nocked-arrow lifecycle and Hector spear flight/restore behavior.

Final RU/EN 16:9 real visual QA is still required before Chapter I gameplay freeze and before production-art acceptance. The latest unified-HUD visual-hierarchy and opening-flow source passes must also be recompiled and rerun through EditMode/PlayMode/full validation in a Unity-capable checkout before their visual QA can be considered accepted.

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
8. Perform and record the 1920x1080 + 1366/1376x768 RU/EN visual-fit matrix, including opening-flow timing/camera/text fit.
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
