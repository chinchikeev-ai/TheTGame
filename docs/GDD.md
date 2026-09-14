# TheTroyGame — Game Design Document v0.3

Last reviewed: 2026-09-14

## 1. High Concept

**Genre:** Story-driven Tower Defence + light tactical hero control  
**Setting:** Trojan War  
**Target campaign length:** ~120 minutes  
**Campaign format:** 7 chapters / maps, 38–42 major combat events total  
**Player role:** Commander of Troy's defense  
**Core fantasy:** Hold Troy against escalating Greek assaults, legendary heroes, siege engines, deception, and the final collapse of the city.

TheTroyGame is a finite cinematic Tower Defence campaign rather than an endless wave mode. Each chapter is a tactical mini-story with preparation, escalation, crisis, climax, and transition.

The player is not expected to repeat identical wave loops for two hours. Routes, objectives, enemy composition, battlefield state, Hector's role, and defensive priorities evolve continuously.

## 2. Creative Direction

Canonical formula:

**Trojan War × Cartoon Tower Defence × Adventure Comedy**

`CREATIVE_DIRECTION.md` is the authority for tone. `ART_BIBLE.md` and `CHARACTER_ART_DIRECTION.md` define visual execution.

The game should be bright, stylized, exaggerated, readable and playful rather than photorealistic or grim-dark. Historical research defines the visual vocabulary, not realistic proportions. Comedy should primarily come from expressive staging, animation, reactions and physical acting rather than constant joke dialogue.

Herc's Adventures is a tonal/readability reference only. It is not a source for copying characters, UI, environments, compositions or assets.

The canonical fall of Troy remains serious in outcome. The presentation may be playful, but the final campaign should still carry emotional weight.

## 3. Design Pillars

1. **Two-hour complete story** — clear beginning, escalation, climax and canonical ending.
2. **Every map is a mini-story** — preparation → escalation → crisis → climax.
3. **Constant mechanical variation** — avoid repetitive numbered-wave grinding.
4. **Readable tactical decisions** — simple economy, clear counters, meaningful upgrades.
5. **Hero presence** — Hector gives the player active tactical agency beyond tower placement.
6. **Escalation through spectacle** — larger armies, siege engines, heroes, breaches, fire and collapsing structures.
7. **Cartoon mythic identity** — recognizable Trojan War figures and equipment presented with bold silhouettes, exaggerated proportions and expressive acting.
8. **Canonical tragedy, variable performance** — Troy falls, but tactical performance changes how the final defense unfolds and how the campaign is scored.

## 4. Campaign Pacing Standard

Recommended target for a normal combat map:

- map duration: **12–18 minutes**;
- major combat events: **5–7**;
- normal encounter duration: **60–120 seconds**;
- preparation before first encounter: **30–45 seconds**;
- pause between encounters: **15–30 seconds**;
- boss/climax event: **150–240 seconds**.

Baseline target: **~15 minutes per standard map / ~6 combat events**.

Campaign-wide target:

- early maps: **10–13 min / 5 events**;
- middle maps: **14–17 min / 6 events**;
- major siege maps: **18–22 min / 7 events**;
- narrative inversion map may contain fewer traditional combat events;
- final Troy Burns map: **15–18 min**, mostly continuous survival.

Total campaign target: **38–42 explicit combat events, target 40**.

Encounter duration is a design target, not a fixed timer. Playtest telemetry should keep Strategos pacing near the intended chapter durations.

## 5. Campaign Structure

| Chapter | Target duration | Major events | Location | Main purpose |
|---|---:|---:|---|---|
| I. The Landing | 11–13 min | 5 | Coast | Tutorial, basic defense, Menelaus |
| II. Road to Troy | 14–16 min | 6 | Plains | Multi-route pressure and mobility |
| III. The Gates | 18–20 min | 7 | Troy walls | Siege engines and gate defense |
| IV. Heroes of Greece | 17–19 min | 6 | Outer defenses | Elite units and hero bosses |
| V. The Great Assault | 20–22 min | 7 | Main walls | Full-system TD siege |
| VI. The Horse | 14–16 min | ~4 | Troy / Greek camp | Narrative inversion and decisions |
| VII. Troy Burns | 16–18 min | ~5 phases | Inside Troy | Continuous survival finale |

Campaign target: approximately **118–122 minutes / ~40 major combat events**.

## 6. Core Gameplay Loop

1. Read incoming routes, enemy preview and current objective.
2. Spend Gold on defensive units, repairs and upgrades.
3. Position or reposition Hector.
4. Start the encounter manually when ready or allow preparation time to expire.
5. Counter enemy archetypes and react to battlefield events.
6. Survive the encounter.
7. Receive Gold, Command Points, unlocks and story consequences.
8. Rebuild or reposition before the next tactical problem.
9. Complete the chapter climax and transition to the next map/state.

## 7. Resources

### Gold

Primary construction economy used for defensive units, upgrades, repairs, Trojan Guard deployment and rebuilding after route changes.

Sources include enemy kills, encounter completion, optional objectives, preserved civilians/structures, boss rewards and any retained early-start bonus.

The player should usually be able to correct a bad defensive layout through Sell and rebuilding, but not without cost.

### Command Points

Secondary tactical resource used for Hector abilities, temporary troop deployment, emergency repairs and battlefield commands. It should not duplicate Gold's purpose.

## 8. Defensive Roster

### Archer Tower
Basic DPS / anti-light infantry. Cheap, fast and reliable; weak against armor and heavy units.

### Ballista
Heavy single-target / anti-armor / anti-siege. Strong against Heavy Hoplites, siege engines and bosses; weak against swarms.

### Priests of Apollo
Crowd-control support. Slow and debuff dangerous groups; low direct damage.

### Spear Wall
Close defensive anti-heavy role with short reach and strong readable formation silhouette.

### Fire Tower
Area denial / damage over time with strong fire readability.

### Trojan Guard
Deployable blocking squad rather than a classic abstract turret.

All defenses should read as Trojan units, crews or structures, not modern/generic turrets.

## 9. Upgrade / Sell Rules

Production target:

- Levels 1–3 are the universal upgrade ladder;
- later progression may introduce specialization after Level 3;
- specialization is not required for Chapter I gameplay freeze.

Sell refund target:

- Strategos: approximately **65%** of invested Gold;
- Story may refund more;
- Legendary may refund less.

## 10. Hector — Player-Controlled Hero

Hector exists physically on the battlefield and provides active tactical control beyond tower placement.

Current reference kit:

- basic spear attack;
- **Q — War Cry**: local offensive buff;
- **E — Shield Wall**: defensive/blocking utility;
- **R — Spear Throw**: high single-target burst;
- **F — For Troy!**: large battlefield power spike.

Exact runtime values belong to implementation/balance data, not this GDD.

## 11. Enemy Roster

Core archetypes include:

- Greek Infantry;
- Runner / Scout;
- Archer;
- Shield Bearer;
- Hoplite / Heavy Hoplite;
- Myrmidons and elite captains;
- Chariot;
- Battering Ram / Siege Tower / Sapper;
- hero companions and bosses.

A new archetype should first appear in a readable encounter, then be combined with other archetypes later. Do not introduce multiple unfamiliar counters simultaneously unless intentionally creating a late-game stress event.

## 12. Bosses

### Menelaus — Chapter I

Mechanic: **Commander Aura**.

- buffs nearby Greek troops;
- calls reinforcements;
- teaches that a boss changes surrounding combat, not only HP quantity.

### Ajax — Chapter IV

Mechanic: frontal defense, heavy durability and positioning counterplay.

### Achilles — Chapter IV/V

Mechanic: fast elite hero with strong pressure and staged vulnerability windows.

### Odysseus — Chapter VII

Mechanic: disruption, deception, route changes and interior breach pressure.

Bosses require distinct health presentation and readable intro/mechanic communication.

## 13. Chapter Breakdown

### Chapter I — The Landing

**Target duration:** 11–13 min  
**Combat events:** 5  
**Location:** Trojan coast

Purpose:

- teach build points and defensive placement;
- teach Upgrade / Sell / Start Encounter;
- introduce Hector controls;
- establish two converging routes;
- end with Menelaus and Commander Aura.

Canonical runtime source of truth is the five authored `EncounterData` assets referenced by `Chapter01_Landing.asset`. Current implementation truth belongs in `PROJECT_STATUS.md` and `DATA_CATALOG.md`.

The target event arc is:

1. Landing Party;
2. Second Beachhead;
3. Shield Line;
4. Greek Push;
5. Menelaus climax.

Chapter I is the reference implementation for later campaign chapters.

### Chapter II — Road to Troy

**Target:** 14–16 min / 6 encounters.

Introduces 2–3 route pressure patterns, Chariots, shielded formations, changing lane priority and a larger battlefield.

### Chapter III — The Gates

**Target:** 18–20 min / 7 encounters.

Introduces gate/wall objectives, Battering Ram, Siege Tower, repairs, breach-driven route changes and wall defense positions.

### Chapter IV — Heroes of Greece

**Target:** 17–19 min / 6 encounters.

Introduces elite formations, Ajax, Achilles and stronger hero-centric combat.

### Chapter V — The Great Assault

**Target:** 20–22 min / 7 encounters.

The strongest traditional Tower Defence chapter: full defensive roster, infantry/heavy/siege/elite combinations, route disruption, disabled positions, wall events and fire hazards.

### Chapter VI — The Horse

**Target:** 14–16 min / ~4 major events.

Intentionally not a normal TD chapter. Uses scouting, narrative events, day/night transition and campaign choices that modify Chapter VII.

The canonical fall of Troy remains fixed; choices alter tactical conditions and performance, not the historical ending.

### Chapter VII — Troy Burns

**Target:** 16–18 min / ~5 continuous survival phases.

Enemies spawn from internal breach points. Classical between-wave pauses are mostly removed.

Core phases:

1. The Horse Opens;
2. Streets Lost;
3. Evacuation;
4. Odysseus;
5. Troy Burns.

Final objective: **hold long enough for evacuation to complete**.

Final scoring includes civilians saved, structures preserved, total leaks, tower losses, Gold efficiency, bosses defeated, optional objectives and survival duration.

## 14. Narrative Choice System

Choices influence mechanics without creating an expensive branching campaign.

Examples:

- strengthen walls;
- train/allocate guards;
- stockpile pitch/resources;
- inspect the Horse;
- reinforce the inner city.

Choices may modify Chapter VII starting economy, internal spawn delay, surviving build nodes, evacuation readiness and initial unit positioning.

The save system persists narrative choices and campaign modifiers; implementation truth is in `CampaignSave.cs` / `DATA_CATALOG.md`.

## 15. Encounter Design Philosophy

An encounter exists to create a tactical question, not to fill time.

Valid purposes include:

- introduce an enemy;
- teach a counter;
- test a newly unlocked defense;
- overload one lane;
- split pressure between lanes;
- fake pressure on one route;
- introduce siege;
- combine siege with escorts;
- introduce elite units;
- create a Hector-centric emergency;
- prepare a boss mechanic;
- provide a short recovery event;
- deliver the chapter climax.

Do not force every event into a generic `Wave 1...Wave N` presentation. Boss encounters, breaches, evacuation phases and survival stages may use named events.

Author encounter composition and pacing in `EncounterData`, never hidden source-code formulas. `WaveData` is legacy compatibility only and is not the production authoring path.

## 16. Difficulty Modes

### Story

More forgiving economy, enemy pressure and recovery windows.

### Strategos

Default intended experience and source of campaign pacing targets.

### Legendary

Reduced economy, higher tactical pressure, tougher enemy composition and less forgiving preparation windows.

Difficulty should change tactical pressure, not merely multiply HP excessively.

## 17. UI

Combat HUD must present:

- Gold / economy;
- Gate/Base HP;
- chapter objective;
- current event;
- next-event preview/countdown where relevant;
- selected defense details;
- Upgrade / Sell;
- Hector state and abilities;
- dedicated boss HP/presentation during boss encounters.

Required campaign screens include Main Menu, Continue, Chapter Select, Settings, Pause, Chapter Complete, Game Over and Campaign Results.

Russian and English must fit the same target layouts.

## 18. Controls

Primary target: mouse + keyboard through the Unity input abstraction.

Core actions:

- select build point;
- choose/place defense;
- select defense;
- upgrade/sell/change priority;
- select/move Hector;
- use Hector abilities;
- Start Encounter;
- pause;
- bounded speed control.

Gameplay systems should consume abstract input actions rather than depend directly on one backend.

## 19. Audio / VFX Direction

Visual progression:

- Chapters I–II: bright Mediterranean daylight and adventurous energy;
- Chapters III–V: dust, smoke, siege damage and larger spectacle;
- Chapter VI: unsettling calm transitioning to night;
- Chapter VII: orange-red fire, smoke and collapsing Troy with reduced overt comedy.

Required readable effects include projectile trails, impacts, Ballista recoil, fire zones, shield impacts, gate damage, destruction, boss introductions and Hector ability feedback.

Music should evolve from lighter antique/adventure atmosphere into heavier siege and finale treatment.

## 20. Victory / Failure

Chapter failure conditions may include:

- Gate/Base HP reaches 0;
- critical objective destroyed;
- evacuation requirement fails;
- scripted survival condition not met.

Most chapters end in tactical victory even though the overall story trends toward Troy's fall.

Final campaign outcome is canonical but performance and scoring are variable.

## 21. Chapter I Reference Implementation

The old seven-wave stress-test vertical-slice concept is obsolete.

The production reference implementation is now **Chapter I: The Landing** with:

- five authored `EncounterData` encounters;
- two converging routes;
- six defensive roles;
- Upgrade / Sell / target priority;
- Hector Q/E/R/F;
- Menelaus + Commander Aura;
- tutorial/objective flow;
- responsive RU/EN HUD;
- telemetry and gameplay-freeze acceptance tooling;
- persistent Chapter I → II unlock.

Chapter I is not considered frozen until the real Play Mode acceptance and production-art gates in `PROJECT_STATUS.md` are complete.

## 22. Production Roadmap Alignment

### Current

Freeze Chapter I gameplay and production art as the campaign reference.

### Next

- Chapter II: authored ChapterData/EncounterData, multi-route pressure and Chariot gameplay;
- Chapter III: destructible objectives, gate/wall state, siege behavior and repair economy;
- Chapter IV: reusable multi-phase boss framework, Ajax and Achilles;
- Chapter V: full-system siege and campaign-scale density;
- Chapter VI: narrative state presentation and choice persistence;
- Chapter VII: internal spawns, evacuation, hazards, Odysseus and result aggregation;
- v1.0: campaign balance, final art/audio/VFX, accessibility, save migration/backup verification, browser/desktop performance and release QA.

Implementation sequencing authority belongs to `UNITY_ROADMAP.md`.

## 23. Success Criteria

The design succeeds if:

- the player understands the core TD loop within 5 minutes;
- a normal chapter feels like a tactical mini-story rather than identical waves;
- encounter duration escalates naturally inside each map;
- bosses require different tactical responses;
- Hector changes tactical decisions rather than behaving like passive decoration;
- the campaign contains roughly 40 meaningful combat events without filler;
- Chapter VI visibly breaks the established rhythm;
- the final 15–18 minutes feel structurally different from the preceding campaign;
- median Strategos campaign completion remains near 120 minutes;
- the visual identity consistently reads as Trojan War + cartoon adventure;
- the player feels both meaningful tactical agency and the growing inevitability of Troy's fall.