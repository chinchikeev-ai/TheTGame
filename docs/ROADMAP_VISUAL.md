# TheTroyGame — Visual Production Roadmap

Last reviewed: 2026-09-15

This document answers one question:

**In what order do we move TheTroyGame from the current playable/procedural visual state to a production-ready visual baseline?**

It is a roadmap, not a second art bible or completion inventory.

Canonical boundaries:

- `docs/ART_BIBLE.md` — what the game must look and feel like.
- `docs/MODEL_ART_INVENTORY.md` — authoritative asset completion status (`DONE`, `PROCEDURAL`, `GENERATED PLACEHOLDER`, etc.).
- `docs/VISUAL_OWNERSHIP.md` — which runtime owner controls each visual domain.
- `docs/TODO.md` — immediate day-to-day work only.
- this file — visual production order, dependencies and acceptance gates.

Do not mark a roadmap stage `FINAL` merely because procedural/generated presentation exists. Final art status is controlled only by `MODEL_ART_INVENTORY.md` and real Play Mode acceptance.

---

## Status legend

- `IMPLEMENTED CANDIDATE` — source/runtime presentation exists, but production-art acceptance is still pending.
- `IN PROGRESS` — current visual production focus.
- `NEXT` — next large visual pass after the current one.
- `PLANNED` — defined, but not yet the active pass.
- `QA GATE` — implementation exists; real Unity/gameplay-camera verification is required.
- `FINAL` — allowed only after the corresponding inventory/acceptance gate is explicitly passed.

Priority:

- `P0` — required before Chapter I can become the production baseline.
- `P1` — required for strong visual quality, but may follow the core production pass.
- `P2` — polish after readability and production identity are stable.

---

# Phase 0 — Production baseline and ownership

**Status: IMPLEMENTED CANDIDATE / ongoing discipline**  
**Priority: P0**

The project already has the correct documentation split and visual ownership model. New visual work must extend the existing owner rather than create overlapping builders/canvases.

Rules for every visual pass:

1. Real gameplay camera is the primary acceptance view.
2. Gameplay readability wins over decoration.
3. Decorative environment objects normally have no gameplay colliders.
4. Do not modify balance, encounter composition, tower stats, enemy stats or campaign rules as part of a visual pass.
5. Do not move gameplay routes/build points merely to fit decoration unless separately approved as game-design work.
6. Troy and Greece must remain readable by silhouette, value and palette at normal zoom.
7. Production art must replace/fill the canonical owner instead of layering a second visual system over it.

---

# Phase 1 — Chapter I Coast & Landing Environment

Chapter I must stop reading as a decorated TD board and read immediately as a **Greek landing against the fortified coast of Troy**.

## 1.1 Shoreline shape and beach material transition

**Status: IMPLEMENTED CANDIDATE**  
**Priority: P0**

Current source pass includes:

- irregular shoreline instead of one straight technical edge;
- `sea -> shallow water -> wet sand -> dry sand -> inland transition`;
- synchronized foam/shore-life against the same shoreline contour;
- pebble fields;
- low sand berms/dune variation;
- local shore boulders;
- driftwood / washed planks / small debris;
- non-collider decorative treatment.

Remaining before production acceptance:

- real Unity gameplay-camera visual review;
- verify units do not lose silhouette against wet/dry sand bands;
- verify shoreline bands do not expose mesh seams or z-fighting;
- verify 1366/1376x768 and 1920x1080 framing;
- replace/promote procedural coast geometry/materials only when final authored coast assets exist.

## 1.2 Water and surf

**Status: NEXT**  
**Priority: P0**

Goal: make the Aegean side feel like living water rather than flat colored geometry.

Scope:

- stronger near-shore water depth transition;
- readable but restrained water motion;
- broken surf instead of repeated equal foam strips;
- wet-sand response near the water edge;
- ship wake integration;
- subtle water glints at gameplay zoom;
- controlled sea mist/spray;
- no expensive full-screen water effect unless profiling supports it.

Acceptance:

- sea reads as water immediately at default camera;
- surf is visible but never competes with enemies/projectiles;
- water motion does not create visual noise behind the landing force;
- no collider or pathing changes.

## 1.3 Greek landing zone

**Status: PLANNED**  
**Priority: P0**

Goal: the beach must visibly tell the story of an active invasion.

Scope:

- stronger ship-to-shore composition;
- final landing ramps/planks;
- rope, crates, shields, spears, oars and baggage grouped naturally;
- clear primary landing point and secondary reinforcement area;
- temporary formation/regroup staging visually integrated with the beachhead;
- Greek standards and cool-blue expeditionary identity;
- controlled footprints/trampled sand only where readable.

Acceptance:

- player understands the attack direction without HUD arrows;
- landing props do not look like buildable/interactable objects;
- no clutter on the two combat lanes;
- Encounter 1 and Encounter 2 transitions visually feel connected to the same beachhead.

## 1.4 Greek ships

**Status: PROCEDURAL CANDIDATE / PLANNED PRODUCTION REPLACEMENT**  
**Priority: P0**

Scope:

- final Bronze Age-inspired galley silhouette;
- at least 2–3 visually distinct variants or controlled modular variation;
- readable hull/sail/oar hierarchy;
- moored/approaching presentation variants;
- final materials;
- LOD/performance policy if needed.

Acceptance:

- ships read as Achaean landing ships, not generic boats;
- silhouette works from normal gameplay camera;
- cinematic approach and parked ships share the same visual language.

---

# Phase 2 — Troy Defensive Side

## 2.1 Gatehouse hero structure

**Status: IMPLEMENTED PROCEDURAL CANDIDATE**  
**Priority: P0**

Owner: `TroyGateHeroBuilder`.

Next production pass:

- authored gate silhouette;
- stronger Bronze Age/Trojan shape language;
- large readable gate opening/doors;
- red/bronze banners and defensive symbols;
- damage-state readability;
- ensure Gate remains visually stronger than surrounding wall modules.

## 2.2 Outer wall and defensive towers

**Status: IMPLEMENTED PROCEDURAL CANDIDATE**  
**Priority: P0**

Owner: `ChapterOneWallLife`.

Scope:

- authored wall modules;
- controlled height variation;
- tower rhythm without repetitive tile feel;
- crenellation/defensive silhouettes readable from gameplay camera;
- guards/banners/fire accents only where they support composition.

## 2.3 Troy skyline / citadel backdrop

**Status: IMPLEMENTED PROCEDURAL CANDIDATE**  
**Priority: P1**

Owner: `TroyCityBackdropPresentation`.

Goal: Troy must feel like a city behind the defense, not a wall at the edge of a board.

Scope:

- stronger city massing;
- large simple silhouettes rather than tiny architecture noise;
- temple/citadel landmark;
- warm pale stone + Trojan red hierarchy;
- depth layers that do not confuse gameplay collision.

## 2.4 Trojan defensive dressing

**Status: PLANNED**  
**Priority: P1**

Scope:

- spear racks;
- arrow baskets;
- stone/ammunition piles;
- defensive timber;
- banners;
- signal braziers;
- small guard/staging props.

Rule: environmental storytelling only; no fake gameplay interaction.

---

# Phase 3 — Battlefield Composition & Attack Lanes

**Status: IMPLEMENTED CANDIDATE -> PLANNED POLISH**  
**Priority: P0**

Owners: `MapBuilder`, `ChapterOneBattlefieldDetails`, compatible environment owners.

Goals:

- two attack routes remain immediately readable;
- lanes look naturally trampled rather than debug paths;
- build-point pattern never dominates the frame;
- center battlefield stays clean enough for combat readability.

Next work:

- naturalize road-to-beach transition;
- tune broad ground-value groups;
- reduce repeated procedural shapes visible at normal zoom;
- reinforce route edges using terrain/value, not UI-like outlines;
- verify selected build/tower states remain visually dominant over environment.

Acceptance:

- player can read enemy direction in under one second;
- lanes do not look like a checkerboard/grid;
- projectiles, hit effects and units remain stronger than ground detail.

---

# Phase 4 — Chapter I Characters

Final character `DONE` count is currently controlled by `MODEL_ART_INVENTORY.md`; generated/source candidates are not final art.

## 4.1 Hector

**Status: GENERATED/SOURCE CANDIDATE + ANIMATION CANDIDATE**  
**Priority: P0**

Production goal:

- final heroic silhouette;
- final head/body/armor materials;
- correct shield/spear grip;
- final crest/cape hierarchy;
- authored locomotion/basic attack/Q/E/R/F/hit/down/revive presentation;
- no visible clipping at gameplay camera;
- strong selection readability.

## 4.2 Menelaus

**Status: GENERATED CANDIDATE**  
**Priority: P0**

Production goal:

- unmistakable commander/boss silhouette;
- cooler Greek bronze/blue hierarchy;
- cape/crest/shield identity;
- final command, attack, hit and death presentation;
- boss arrival must visually exceed regular enemy entrances.

## 4.3 Greek regular roster

**Status: GENERATED/SOURCE CANDIDATES**  
**Priority: P0**

Order:

1. Infantry
2. Runner
3. Heavy Hoplite
4. Shield Bearer
5. Archer

Each must have a distinct one-second role read from gameplay zoom.

## 4.4 Trojan combat/support roster

**Status: GENERATED/SOURCE CANDIDATES**  
**Priority: P0/P1**

Order:

1. Trojan Guard
2. Trojan Archer
3. Spear infantry
4. Ballista crew
5. Priest of Apollo
6. Fire Keeper

Acceptance for all characters:

- correct faction/role silhouette;
- final runtime source uses production prefab;
- intentional colliders;
- correct materials;
- required animation hooks connected;
- real gameplay-camera QA passed.

---

# Phase 5 — Tower-Units / Defenses

**Status: PROCEDURAL STRUCTURES + GENERATED CREW CANDIDATES**  
**Priority: P0**

Production order:

1. Trojan Guard Post
2. Archer Post
3. Spear Wall
4. Ballista
5. Priests of Apollo
6. Fire Tower

For every Tower-Unit:

- authored structure rather than primitive assembly;
- one-second role silhouette;
- visible crew/role identity;
- clear L1/L2/L3 progression without tiny cosmetic-only changes;
- final recoil/cast/reload/brace/throw presentation;
- selection/range rings remain readable;
- structure never hides enemies passing nearby.

After all six base defenses are accepted:

- authored L2/L3 visual differentiation;
- later specialization branch visuals.

---

# Phase 6 — Combat Animation, Projectiles & Impact Readability

**Status: IMPLEMENTED CANDIDATE / QA REQUIRED**  
**Priority: P0**

Current project already has runtime presentation hooks. The next goal is not more hooks; it is authored visual quality and timing.

Order:

1. basic arrow flight / impact;
2. spear thrust / flight / impact;
3. Trojan Guard block/poke;
4. Hector basic + Q/E/R/F;
5. fire projectile / burn feedback;
6. slow/support projectile feedback;
7. heavy hit hierarchy;
8. gate hit hierarchy;
9. Menelaus death/defeat presentation.

Acceptance:

- ordinary hits are readable but restrained;
- heavy/boss/gate events are clearly stronger;
- animation never lies about gameplay timing;
- no projectile/VFX disappearance under Chapter I peak density;
- VFX does not obscure target selection or build placement.

---

# Phase 7 — Opening Flow & Cinematic Staging

**Status: IMPLEMENTED CANDIDATE**  
**Priority: P0/P1**

Current sequence:

`coast establishing shot -> Greek fleet approach -> shore landing -> authored preparation -> Encounter 1 -> first assault repelled -> Greek regroup/reinforcement -> Encounter 2`

Next visual pass after environment production:

- reframe opening camera using the final coast/ship silhouettes;
- improve fleet depth and scale;
- improve first-contact landing composition;
- integrate beach debris and reinforcement staging;
- tune title/card timing against real environment contrast;
- preserve gameplay control and mandatory first preparation.

Acceptance:

- opening feels like the start of a chapter, not a camera flyover on a TD board;
- player understands setting, attacker, defender and immediate objective before Encounter 1;
- camera returns cleanly to gameplay framing.

---

# Phase 8 — Combat HUD & UI Production Art

**Status: LAYOUT/HIERARCHY IMPLEMENTED CANDIDATE; FINAL ART/QA PENDING**  
**Priority: P0/P1**

The HUD layout should not be redesigned again without a specific QA failure. Focus now shifts to final art and fit.

Priority order:

1. final Gold / Speed / Settings utility icons and frames;
2. final Gate treatment;
3. defender portraits/cards;
4. selected-defense + tooltip art;
5. final Hector HUD acceptance;
6. Menelaus/Boss HUD acceptance;
7. Patron portraits/commentary panel acceptance;
8. enemy inspector;
9. next-encounter cards;
10. notifications/tutorial parchment fit.

Required QA matrix:

- 1920x1080 EN;
- 1920x1080 RU;
- 1376x768 EN;
- 1376x768 RU;
- 1366x768 where practical.

No duplicate canvas/layout ownership may be introduced.

---

# Phase 9 — Lighting, Materials & Atmosphere

**Status: PROCEDURAL CANDIDATE**  
**Priority: P1**

Goal: unify all accepted geometry/characters into one authored visual world.

Scope:

- final sun/key-light direction and value range;
- sea/sand/stone/wood/bronze/cloth material separation;
- Trojan warm-vs-Greek cool contrast;
- controlled shadows;
- dust/smoke/fire accents;
- selective sea haze;
- avoid orange-overload in Chapter I;
- preserve clean tactical readability.

Acceptance:

- characters separate from terrain without outline dependency;
- bronze reads as bronze;
- sea and wet sand separate clearly;
- Troy reads warmer than Greek landing zone;
- no pink/missing materials;
- no atmosphere that hides enemies or projectiles.

---

# Phase 10 — Chapter I Visual QA & Art Freeze

**Status: QA GATE**  
**Priority: P0**

Visual production is not finished until real Unity evidence exists.

Required sequence:

1. compile cleanly in Unity;
2. run required EditMode/PlayMode validation;
3. complete one full Story run at 1x/no-pause;
4. inspect opening, all 5 Encounters, Menelaus climax and result flow;
5. verify RU/EN 16:9 UI matrix;
6. inspect gameplay-camera silhouette/readability for every P0 character and Tower-Unit;
7. inspect projectiles/hits at normal and peak density;
8. inspect coast/sea/road/build-point hierarchy;
9. inspect Gate/Troy skyline against HUD and boss states;
10. update `MODEL_ART_INVENTORY.md` only for assets that actually satisfy the `DONE` gate;
11. complete the Chapter I art-freeze process.

Until this passes, Chapter I remains a **production-art candidate**, not a frozen visual baseline.

---

# Phase 11 — Chapter II–VII Visual Expansion

**Status: PLANNED**  
**Priority: after Chapter I baseline**

Do not scale visual production across the full campaign before Chapter I establishes the reusable quality bar.

## Chapter II — Plains / advance

Visual focus:

- open dry terrain;
- longer road composition;
- field camps;
- chariot introduction;
- larger army movement;
- less coast, more dust/grass/earth.

## Chapter III — Siege pressure

Visual focus:

- siege engines;
- battering ram;
- siege tower;
- wall pressure;
- denser military staging;
- stronger structural hit feedback.

## Chapter IV — Damaged outer defenses

Visual focus:

- broken defensive lines;
- damaged wall modules;
- debris and emergency reinforcement;
- escalating smoke/fire while maintaining readability.

## Chapter V — Troy under collapse

Visual focus:

- destruction states;
- burning structures;
- civilians/evacuation where required;
- more dramatic light/value contrast;
- battlefield damage accumulation.

## Chapter VI — Abandoned Greek camp / Horse setup

Visual focus:

- eerie emptiness after previous army density;
- abandoned props/tents;
- Trojan Horse as dominant visual landmark;
- deceptive calm.

## Chapter VII — Troy interior / final destruction

Visual focus:

- interior city spaces;
- collapse/burning states;
- strong narrative staging;
- maximum campaign fire/destruction motif;
- preserve character/action readability despite spectacle.

Campaign-wide environment/character production must reuse the Chapter I rules for ownership, material language, gameplay-camera acceptance and explicit `DONE` status.

---

# Current execution order

As of 2026-09-15, the recommended visual sequence is:

1. **Water + surf production pass.**
2. **Greek landing zone / ships integration.**
3. **Troy gate + wall production pass.**
4. **Battlefield lane/ground readability cleanup.**
5. **Hector + Menelaus final production character acceptance.**
6. **Five Greek regular enemy production assets.**
7. **Six Trojan Tower-Units and crews.**
8. **Combat animation/projectile/impact production pass.**
9. **Opening cinematic restaging on final environment.**
10. **Combat HUD final-art + RU/EN fit pass.**
11. **Lighting/material/atmosphere unification.**
12. **Full Chapter I visual QA + art freeze.**
13. **Only then expand the production baseline into Chapter II–VII.**

---

# Definition of visual success for Chapter I

A successful final Chapter I frame must read, without explanation, as:

**A stylized, cartoon Bronze Age Greek landing attacking the fortified coast of Troy, with clear tactical lanes, memorable characters, readable defenses and a strong but unobtrusive command HUD.**

The player should never perceive the scene primarily as:

- a debug grid;
- a collection of primitive placeholders;
- generic fantasy TD;
- a set of disconnected UI systems;
- decorative clutter obscuring gameplay.

The final quality bar is the real gameplay camera, not isolated beauty shots.
