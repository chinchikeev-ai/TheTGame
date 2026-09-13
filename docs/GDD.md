# TheTroyGame — Game Design Document v0.2

## 1. High Concept

**Genre:** Story-driven Tower Defence + light tactical hero control  
**Setting:** Trojan War  
**Target campaign length:** ~120 minutes  
**Campaign format:** 7 chapters / maps, 38–42 combat events total  
**Player role:** Commander of Troy's defense  
**Core fantasy:** Hold Troy against escalating Greek assaults, legendary heroes, siege engines, deception, and the final collapse of the city.

TheTroyGame is a finite cinematic Tower Defence campaign rather than an endless wave mode. Each chapter is a self-contained tactical story with preparation, escalation, a climax, and a transition into the next stage of the siege.

The player is not expected to repeat identical wave loops for two hours. Routes, objectives, enemy composition, battlefield state, Hector's role, and defensive priorities must evolve continuously.

---

## 2. Design Pillars

1. **Two-hour complete story** — clear beginning, escalation, climax, and canonical ending.
2. **Every map is a mini-story** — preparation → escalation → crisis → climax.
3. **Constant mechanical variation** — avoid repetitive numbered-wave grinding.
4. **Readable tactical decisions** — simple economy, clear counters, meaningful upgrades.
5. **Hero presence** — Hector gives the player active tactical agency beyond tower placement.
6. **Escalation through spectacle** — larger armies, siege engines, heroes, breaches, fire, collapsing structures.
7. **Historical-mythic tone** — recognizable Trojan War figures without strict simulation requirements.
8. **Canonical tragedy, variable performance** — Troy falls, but the player's tactical performance meaningfully changes how the final defense unfolds and how the campaign is scored.

---

## 3. Campaign Pacing Standard

### Per-map standard

Recommended target for a normal combat map:

- map duration: **12–18 minutes**;
- waves / major combat events: **5–7**;
- normal wave duration: **60–120 seconds**;
- preparation before first wave: **30–45 seconds**;
- pause between waves: **15–30 seconds**;
- boss / climax event: **150–240 seconds**.

Baseline target:

**~15 minutes per standard map / 6 combat events.**

Typical escalation inside one map:

`60 sec → 75 sec → 90 sec → 100 sec → 120 sec → 180 sec climax`

Wave duration is a design target, not a fixed timer. A wave may end earlier if enemies are killed quickly, but encounter composition should be authored to land near the target duration on Strategos difficulty.

### Campaign-wide standard

- early maps: **10–13 min / 5 events**;
- middle maps: **14–17 min / 6 events**;
- major siege maps: **18–22 min / 7 events**;
- narrative inversion map: may contain fewer traditional waves;
- final Troy Burns map: **15–18 min**, mostly continuous survival with minimal classical pauses.

Total campaign target:

**38–42 explicit combat events, target 40.**

Boss fights and scripted assault phases count as combat events when they replace a normal wave.

---

## 4. Campaign Structure

| Chapter | Target time | Combat structure | Location | Main purpose |
|---|---:|---:|---|---|
| I. The Landing | 0–13 min | 5 events | Coast | Tutorial, basic defense, first boss |
| II. Road to Troy | 13–28 min | 6 events | Plains | Multi-route combat and mobility |
| III. The Gates | 28–47 min | 7 events | Troy walls | Siege engines and gate defense |
| IV. Heroes of Greece | 47–65 min | 6 events | Outer defenses | Elite units and hero bosses |
| V. The Great Assault | 65–86 min | 7 events | Main walls | Full-system TD siege |
| VI. The Horse | 86–102 min | 4 events | Troy / Greek camp | Narrative inversion and decisions |
| VII. Troy Burns | 102–120 min | 5 survival phases | Inside Troy | Continuous survival finale |

**Total: 40 combat events / approximately 118–122 minutes.**

Chapter boundaries are targets. Playtest telemetry should keep total campaign median completion near two hours on Strategos difficulty.

---

## 5. Core Gameplay Loop

1. Read incoming routes, enemy previews, and current objective.
2. Spend Gold on towers, troops, repairs, and upgrades.
3. Position or reposition Hector.
4. Start the wave manually when ready, or allow the preparation timer to expire.
5. Counter enemy archetypes and react to battlefield events.
6. Survive the encounter.
7. Receive Gold, Command Points, unlocks, and story consequences.
8. Rebuild or reposition before the next tactical problem.
9. Complete the chapter climax and transition to the next map/state.

---

## 6. Resources

### Gold

Primary construction economy.

Used for:
- towers;
- upgrades;
- repairs;
- Trojan Guard deployment;
- rebuilding after route changes.

Sources:
- enemy kills;
- wave completion;
- optional objectives;
- preserving civilians or structures;
- boss rewards;
- early-wave start bonus if retained after balancing.

Economy principle:

The player should usually be able to correct a bad defensive layout through Sell and rebuilding, but not without cost.

### Command Points

Secondary tactical resource.

Used for:
- Hector abilities;
- temporary troop deployment;
- emergency repairs;
- battlefield commands.

Command Points regenerate slowly and through combat milestones. They should not duplicate Gold's purpose.

---

## 7. Towers and Defensive Units

### 7.1 Archer Tower

**Role:** Basic DPS / anti-light infantry  
**Strengths:** Cheap, fast, reliable  
**Weaknesses:** Armor and heavy units

Core progression:
- Level 1 — standard archers;
- Level 2 — Reinforced Bow;
- Level 3 — Eagle Eye / improved attack speed;
- Specialization — Fire Arrows or Twin Archers.

### 7.2 Ballista

**Role:** Heavy single-target / anti-armor / anti-siege  
**Strengths:** Heavy Hoplites, siege engines, bosses  
**Weaknesses:** Slow firing, weak swarm control

Core progression:
- Heavy Bolt;
- Piercing Bolt;
- Reinforced Winch;
- specialization into Siege Breaker or multi-target piercing.

### 7.3 Priests of Apollo

**Role:** Crowd-control support  
**Strengths:** Slowing and weakening dangerous groups  
**Weaknesses:** Low direct damage

Effects by progression:
- movement slow;
- increased slow duration;
- armor reduction;
- high-tier fear/disruption specialization.

### 7.4 Spear Throwers

Unlocked after the vertical slice.

**Role:** Medium-range anti-heavy infantry.

### 7.5 Fire Tower

Unlocked in siege progression.

**Role:** Area denial / damage over time.

### 7.6 Trojan Guard

Deployable blocking squad rather than a classic tower.

**Role:** Stop enemies temporarily and create kill zones.

---

## 8. Upgrade / Sell Rules

Production target:

- Levels 1–3 are the universal upgrade ladder;
- after Level 3, selected towers may receive one specialization choice;
- specialization is a campaign feature, not required for the first vertical slice.

Sell refund target:

- standard difficulty: approximately **65%** of invested Gold;
- Story may refund more;
- Legendary may refund less.

Route changes are intentional. Sell exists so the game can demand tactical rebuilding without permanently punishing the player for information they could not know in advance.

---

## 9. Hector — Player-Controlled Hero

Hector exists physically on the battlefield.

Initial control design:
- selectable hero;
- movement between tactical nodes or direct ground movement;
- automatic basic melee attack when enemies are in range.

### Basic attack

Spear melee attack with moderate single-target damage.

### Ability 1 — War Cry

Recommended as the first vertical-slice ability because it directly interacts with the existing tower system.

Suggested effect:
- nearby towers +30% attack speed;
- nearby Trojan units +15% damage;
- duration: ~10 sec.

### Ability 2 — Shield Wall

Creates a temporary blocking/defensive line.

### Ability 3 — Spear Throw

High single-target burst damage against captains, siege crews, and bosses.

### Ultimate — For Troy!

~10 sec battlefield-wide power spike:
- faster tower attacks;
- Trojan damage resistance;
- slight enemy slow;
- strong audiovisual feedback.

---

## 10. Enemy Roster

### Basic
- Greek Infantry;
- Archer;
- Spearman;
- Light Swordsman.

### Defensive / Heavy
- Shield Bearer;
- Hoplite;
- Heavy Hoplite;
- Myrmidon.

### Fast
- Scout;
- Runner;
- Chariot.

### Siege
- Ram Crew;
- Battering Ram;
- Siege Tower;
- Sapper.

### Elite
- Myrmidon Veteran;
- Greek Captain;
- Hero Companion.

Enemy introduction rule:

A new enemy archetype should first appear in a readable encounter, then be combined with other archetypes later. Do not introduce multiple unfamiliar counters simultaneously unless it is a deliberate late-game stress event.

---

## 11. Bosses

### Menelaus — Chapter I climax

**Mechanic:** Commander Aura.

- buffs nearby Greek troops;
- periodically calls reinforcements;
- designed as the player's first lesson that a boss changes surrounding combat, not only HP quantity.

Counterplay:
- isolate;
- Ballista focus;
- Hector burst when available.

### Ajax — Chapter IV

**Mechanic:** Frontal defense.

- extreme durability;
- frontal damage reduction;
- shield stance;
- vulnerable to crossfire, fire, and Hector engagement.

### Achilles — Chapter IV climax / Chapter V narrative consequence

**Mechanic:** Fast elite hero.

- high speed;
- destroys blocking units quickly;
- near-invulnerability phases;
- vulnerability window triggered by encounter state.

The vulnerability should be staged cinematically rather than implemented as a literal heel-click gimmick.

### Odysseus — Chapter VII tactical boss

**Mechanic:** Disruption and deception.

- route changes;
- temporary tower disable;
- false attack indicators;
- interior breach activation.

---

## 12. Chapter Breakdown

### Chapter I — The Landing

**Target duration:** 11–13 min  
**Combat events:** 5  
**Location:** Trojan coast

Purpose:
- teach build points;
- Archer Tower;
- Upgrade / Sell;
- Start Wave;
- first use of Hector;
- introduce two converging routes.

Suggested structure:

1. **Landing Party** — ~60 sec — basic infantry, one route emphasized.
2. **Second Beachhead** — ~75 sec — both routes active.
3. **Shield Line** — ~90 sec — first Heavy Hoplite introduction.
4. **Greek Push** — ~110 sec — mixed formation and lane pressure.
5. **Menelaus** — ~180 sec — boss + Commander Aura reinforcements.

Preparation gaps: 15–25 sec after the opening tutorial setup.

Chapter reward:
- Ballista unlocked permanently.

---

### Chapter II — Road to Troy

**Target duration:** 14–16 min  
**Combat events:** 6  
**Location:** Plains and road network

Introduces:
- 3 possible approach lanes;
- Chariots;
- shield formations;
- changing lane priority;
- larger build area.

Suggested escalation:

1. scouts and infantry — 60 sec;
2. fast lane pressure — 75 sec;
3. shield formation — 90 sec;
4. chariot attack — 100 sec;
5. split assault — 120 sec;
6. captain-led multi-route climax — 160–180 sec.

Chapter objective:
- stop Greek forces from reaching the road into Troy.

---

### Chapter III — The Gates

**Target duration:** 18–20 min  
**Combat events:** 7  
**Location:** Main Trojan gate and walls

Introduces:
- gate HP as a distinct objective;
- Battering Ram;
- Siege Tower;
- Spear Throwers;
- repairs;
- wall build positions.

Suggested structure:

1. infantry probe — 60 sec;
2. first ram — 80 sec;
3. shield escort — 90 sec;
4. dual siege pressure — 105 sec;
5. siege tower — 120 sec;
6. temporary breach — 120–150 sec;
7. emergency gate defense climax — 180–210 sec.

This is the first large siege map.

---

### Chapter IV — Heroes of Greece

**Target duration:** 17–19 min  
**Combat events:** 6  
**Location:** Damaged outer defenses

Introduces:
- Myrmidons;
- elite captains;
- Priests of Apollo advanced utility;
- stronger Hector usage.

Structure:

1. elite infantry test — ~75 sec;
2. Myrmidon wave — ~90 sec;
3. Ajax encounter — ~150 sec;
4. short recovery assault — ~75 sec;
5. combined elite push — ~110 sec;
6. Achilles encounter — ~180–220 sec.

This chapter should feel character-driven rather than like a normal sequence of anonymous waves.

---

### Chapter V — The Great Assault

**Target duration:** 20–22 min  
**Combat events:** 7  
**Location:** Main walls under maximum pressure

All core defensive systems are available.

Enemy mix:
- infantry;
- Heavy Hoplites;
- Myrmidons;
- Chariots;
- siege engines;
- captains.

Dynamic battlefield events:
- wall segment disabled;
- temporary route opens;
- Trojan reinforcements arrive;
- fire hazard appears;
- one build position may be destroyed.

Encounter durations should escalate from ~75 sec to a final **3–4 minute** siege climax.

This is the strongest traditional Tower Defence chapter in the campaign.

---

### Chapter VI — The Horse

**Target duration:** 14–16 min  
**Combat events:** 4, mixed with narrative/scouting  
**Location:** Troy exterior, abandoned Greek camp, city gate

Purpose:
- reset pacing after the Great Assault;
- create uncertainty;
- convert player choices into modifiers for the finale.

Sequence:

1. final scattered Greek retreat skirmish;
2. scouting objective;
3. suspicious/false alarm combat event;
4. Trojan Horse discovery and guard allocation event.

Narrative choices:
- reinforce walls;
- strengthen inner city;
- inspect the Horse;
- allocate guards;
- stockpile Gold / pitch.

The historical outcome remains fixed: Troy ultimately falls.

Player decisions modify:
- starting Gold in Chapter VII;
- internal enemy spawn delay;
- number of surviving build nodes;
- civilian evacuation time;
- initial Hector/Guard position.

This chapter intentionally does **not** follow the standard six-wave structure.

---

### Chapter VII — Troy Burns

**Target duration:** 16–18 min  
**Combat structure:** 5 continuous survival phases  
**Location:** City interior

Core twist:

Enemies spawn from internal breach points rather than only from map edges.

Classical between-wave pauses are mostly removed. Short 5–15 sec breathing windows may occur after scripted milestones.

Survival phases:

1. **The Horse Opens** — internal Greek spawn begins.
2. **Streets Lost** — new routes open, buildings burn.
3. **Evacuation** — player must protect civilians moving through the city.
4. **Odysseus** — tactical disruption encounter.
5. **Troy Burns** — 3–4 minute final survival climax.

Dynamic events:
- buildings catch fire;
- streets become blocked;
- alternate passages open;
- defenses are destroyed;
- alarms and battle ambience intensify;
- Hector is increasingly required away from optimal tower positions.

Final objective:

**Hold long enough for evacuation to complete.**

Troy cannot be permanently saved. Success is defined by what survives and how long the player holds.

Ending sequence:

**TROЯ / TROY BURNS**

- walls and buildings burn;
- surviving civilians leave the city;
- player control fades into cinematic framing;
- final campaign results appear.

Final scoring:
- civilians saved;
- gate integrity before breach;
- total leaks;
- towers lost;
- Gold efficiency;
- bosses defeated;
- optional objectives;
- final survival duration.

---

## 13. Narrative Choice System

Choices influence mechanics without creating an expensive branching campaign.

### Strengthen Walls

Benefit:
- more gate HP;
- cheaper repairs.

Cost:
- fewer starting units inside Troy.

### Train More Guards

Benefit:
- stronger Trojan Guard.

Cost:
- less starting Gold for towers.

### Stockpile Pitch

Benefit:
- Fire Tower bonuses;
- more effective fire zones.

Cost:
- reduced general economy.

### Inspect the Horse

Benefit:
- delays first internal Greek spawn in Chapter VII.

Constraint:
- cannot prevent the canonical fall.

---

## 14. Wave Design Philosophy

A wave exists to create a tactical question, not to fill time.

Valid purposes include:
- introduce an enemy;
- teach a counter;
- test a newly unlocked tower;
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

Campaign target:

**40 major combat events**, acceptable range **38–42**.

Do not force every event into a `Wave 1...Wave 7` presentation. Boss encounters, breaches, evacuation phases, and survival stages may use named events instead.

---

## 15. Difficulty Modes

### Story

- +Gold;
- slower enemies;
- lower enemy HP scaling;
- more forgiving leaks;
- faster Hector ability regeneration;
- higher Sell refund.

### Strategos

Default intended experience and source of pacing targets.

### Legendary

- reduced Gold;
- tougher siege engines;
- lower Sell refund;
- stronger boss mechanics;
- more aggressive route changes;
- fewer preparation windows.

Difficulty should change tactical pressure, not merely multiply HP excessively.

---

## 16. UI

### Combat HUD

Top:
- Gold;
- Command Points;
- Gate/Base HP;
- chapter objective;
- current event / wave;
- next-wave preview and countdown.

Bottom:
- tower selection;
- selected tower details;
- Upgrade;
- Sell;
- Hector abilities.

### Required screens

- Main Menu;
- Continue;
- Chapter Select / Level Select;
- Settings;
- Pause;
- Chapter Complete;
- Game Over;
- Campaign Results.

Boss encounters require a dedicated boss health presentation.

---

## 17. Controls

Target support:
- mouse + keyboard;
- Unity New Input System;
- Legacy Input fallback where required.

Core actions:
- select build point;
- choose tower;
- place tower;
- select tower;
- upgrade;
- sell;
- select Hector;
- move Hector;
- use abilities;
- pause;
- Start Wave;
- optional speed control.

Gameplay systems should consume abstract input actions rather than directly depend on one Unity input backend.

---

## 18. Audio / VFX Direction

### Visual progression

- Chapters I–II: warm Mediterranean daylight;
- Chapters III–V: dust, smoke, siege damage;
- Chapter VI: unsettling calm transitioning to night;
- Chapter VII: orange-red fire, smoke, collapsing Troy.

Required effects:
- projectile trails;
- arrow impacts;
- Ballista recoil;
- fire zones;
- shield impacts;
- destruction particles;
- gate damage;
- building fire;
- boss introductions.

### Audio

Core palette:
- bows;
- Ballista tension/release;
- shield/armor impacts;
- horns;
- army ambience;
- fire;
- gate impacts;
- Hector ability stingers.

Music direction:
- antique / Greek-modal atmosphere;
- calm menu / preparation layer;
- battle layer during waves;
- heavier percussion / chant treatment for boss encounters;
- strongest dramatic treatment reserved for Troy Burns.

Prototype may use generated/in-engine audio. Production can replace it with authored licensed/original music and SFX later.

---

## 19. Victory / Failure

Chapter failure conditions may include:
- Gate/Base HP reaches 0;
- critical objective destroyed;
- evacuation requirement fails;
- scripted survival condition not met.

Most chapters end in tactical victory even though the overall story trends toward Troy's fall.

Final campaign outcome is canonical but scoring is variable.

---

## 20. Vertical Slice Scope

The first production vertical slice proves one complete TD map before campaign expansion.

Required:
- one map;
- 2 converging enemy routes;
- **7 authored waves as a stress-test map**;
- Start Wave;
- automatic preparation countdown;
- Archer Tower;
- Ballista;
- Priests of Apollo / Slow support;
- Upgrade / Sell;
- normal + Heavy enemy;
- Menelaus or equivalent boss;
- Hector movement and first ability;
- projectile / muzzle / hit / destruction feedback;
- generated SFX;
- menu;
- pause;
- victory/game over;
- New Input System + fallback strategy.

The campaign average is approximately six events per standard map. The vertical slice deliberately uses seven to validate the upper bound used by siege chapters.

---

## 21. Production Roadmap Alignment

### Foundation / Vertical Slice

- complete TD loop;
- data-driven TowerData / EnemyData / WaveData;
- EnemyRegistry;
- GameStateController;
- two-route map;
- 7-wave stress test;
- Hector first playable version.

### Chapters I–III

- tutorialized Landing chapter;
- Road multi-route chapter;
- full gate/siege mechanics;
- persistent campaign state.

### Chapters IV–V

- Ajax;
- Achilles;
- Myrmidons;
- advanced route changes;
- all core towers;
- full Hector kit.

### Chapter VI

- narrative state machine;
- Horse choices;
- night transition;
- campaign modifiers.

### Chapter VII

- internal spawns;
- dynamic streets;
- fire system;
- evacuation;
- Odysseus;
- Troy Burns finale.

### v1.0 pass

- full campaign balancing against pacing telemetry;
- visual polish;
- authored audio replacement where appropriate;
- accessibility;
- difficulty tuning;
- save system;
- optimization;
- QA and build validation.

---

## 22. Success Criteria

The design succeeds if:
- the player understands the core TD loop within 5 minutes;
- a normal chapter feels like a 12–18 minute mini-story, not six identical waves;
- encounter duration escalates naturally inside each map;
- bosses require different tactical responses;
- Hector changes tactical decisions rather than behaving like passive decoration;
- the campaign contains roughly 40 meaningful combat events without filler;
- Chapter VI visibly breaks the established rhythm;
- the final 15–18 minutes feel structurally different from the preceding campaign;
- median Strategos campaign completion remains near 120 minutes;
- the player feels both meaningful tactical agency and the growing inevitability of Troy's fall.
