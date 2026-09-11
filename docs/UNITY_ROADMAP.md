# TheTroyGame — Unity Implementation Roadmap v0.2

## Objective

Build the game as one production project on `main`, with no throwaway prototype branch. The target is a complete story-driven Tower Defence campaign of roughly **120 minutes**, structured as **7 chapters** and approximately **40 major combat events**.

This roadmap is subordinate to `docs/GDD_v0.1.md` (currently GDD v0.2 content). If pacing, chapter structure, bosses, tower roster, or campaign rules change in the GDD, this roadmap must be updated in the same milestone.

---

## Production Rules

1. Keep gameplay implementation in `main` unless an isolated experimental branch is technically necessary.
2. Build systems once, then author chapters from data rather than duplicating code.
3. Do not expand content before the previous milestone has passed its Definition of Done.
4. Keep chapter duration, wave duration and economy measurable through telemetry.
5. Avoid per-frame scene scans and hard-coded balance values in gameplay controllers.
6. Prefer ScriptableObject-authored data for towers, enemies, waves, chapters and balance.
7. Maintain New Input System support with Legacy Input fallback through an adapter layer.

---

## Campaign Pacing Contract

Production targets:
- standard combat map: **12–18 minutes**;
- standard wave count: **5–7 waves / major combat events**;
- ordinary wave: **60–120 seconds**;
- preparation window: **15–30 seconds**;
- boss/climax: **150–240 seconds**;
- reference map: **~15 minutes / 6 events**;
- total campaign: **38–42 combat events**, target **40**.

Reference escalation:

```text
Preparation      30–45 sec
Wave 1           ~60 sec
Pause            ~20 sec
Wave 2           ~75 sec
Pause            ~20 sec
Wave 3           ~90 sec
Pause            ~20 sec
Wave 4           ~100 sec
Pause            ~20 sec
Wave 5           ~110–120 sec
Pause            ~25 sec
Wave 6 / Boss    ~150–210 sec
```

Do not force a wave to last until `targetDuration`. The target exists for balancing and telemetry.

---

## Target Project Structure

```text
Assets/
  _TheTroyGame/
    Art/
      Materials/
      Models/
      VFX/
      UI/
    Audio/
      SFX/
      Music/
    Data/
      Enemies/
      Towers/
      Waves/
      Chapters/
      Balance/
    Prefabs/
      Enemies/
      Towers/
      Projectiles/
      Heroes/
      Environment/
      UI/
    Scenes/
      Bootstrap.unity
      MainMenu.unity
      Chapter01_Landing.unity
      Chapter02_Road.unity
      Chapter03_Gates.unity
      Chapter04_Heroes.unity
      Chapter05_Assault.unity
      Chapter06_Horse.unity
      Chapter07_TroyBurns.unity
    Scripts/
      Core/
      Combat/
      Towers/
      Enemies/
      Waves/
      Heroes/
      Economy/
      Campaign/
      UI/
      Audio/
      Save/
      Input/
```

Current prototype scripts may remain under `Assets/Scripts` during migration, but production milestones should progressively move toward the target structure rather than adding more flat-root files indefinitely.

---

# MILESTONE 0 — Architecture Foundation

## Goal

Stabilize the current vertical-slice code before adding more campaign content.

## Required systems

### GameBootstrap
Responsibilities:
- initialize persistent/global services;
- create or load runtime systems;
- settings initialization;
- scene transition entry point;
- save-load entry point later.

### GameStateController
Required states:
- Preparing;
- WaveRunning;
- BetweenWaves;
- Paused;
- Victory;
- Defeat;
- NarrativeEvent.

No gameplay controller should infer major game state only from UI visibility or `Time.timeScale`.

### EnemyRegistry
Single runtime registry of alive enemies.

Used by:
- tower targeting;
- splash queries;
- wave completion;
- boss aura logic;
- future objective systems.

Per-frame `FindObjectsByType<Enemy>` is not allowed in production gameplay paths.

### Data-driven balance
Required data types:
- `TowerData`;
- `EnemyData`;
- `WaveData`;
- later `ChapterData`.

Prototype runtime-generated defaults are acceptable temporarily, but authored ScriptableObject assets are required before Chapter II production begins.

### Damage pipeline
Target pipeline:

```text
Attack -> Hit -> Raw Damage -> Armor/Resistance -> Status Modifiers -> HP -> Death
```

Minimum damage types:
- Physical;
- Piercing;
- Fire;
- Hero.

### Status effect layer
Reusable effects:
- Slow — required now;
- Burn;
- ArmorBreak;
- Stun;
- Fear.

### Input abstraction
Create gameplay actions:
- Point;
- Select;
- Cancel;
- Pause;
- Ability1;
- Ability2;
- Ability3;
- Ultimate.

Gameplay code should not proliferate direct `Input.Get...` / `Mouse.current...` checks. Route them through one adapter/service.

## Definition of Done — Milestone 0

- current vertical slice still launches;
- no tower or projectile performs a full enemy scene scan per frame/impact;
- tower/enemy/wave balance is available through data objects;
- game state is explicit;
- existing menu/pause/victory/game-over loop still works;
- Unity Console has no compile errors.

---

# MILESTONE 1 — Vertical Slice Completion

## Goal

Prove the complete TD combat loop on one polished map before campaign expansion.

## Required content

- 2 enemy routes converging on one Trojan gate;
- Start Wave button;
- automatic countdown;
- exactly 7 authored test waves;
- Greek Infantry;
- Heavy Hoplite;
- Menelaus boss;
- Archer Tower;
- Ballista;
- Priests of Apollo;
- Upgrade / Sell;
- build zones;
- muzzle flash;
- projectile feedback;
- hit/death VFX;
- generated combat SFX;
- dynamic ambient/battle/boss music;
- Main Menu;
- Level Select;
- Settings;
- Pause;
- Victory / Game Over;
- dual input compatibility.

The 7-wave slice is a stress-test of the high end of production map density. It is not the default wave count for every campaign chapter.

## Missing feature required before sign-off: Hector MVP

Implement in this order:
1. selectable Hector;
2. movement between tactical nodes or validated free movement;
3. HP and basic melee attack;
4. first active ability: **War Cry** or **Shield Wall**;
5. cooldown UI;
6. death/downed handling without breaking the entire match flow.

## Menelaus boss MVP

Required:
- dedicated boss HP bar;
- identifiable silhouette;
- boss intro cue;
- Commander Aura or reinforcement call mechanic;
- death event;
- final-wave victory trigger;
- target encounter duration: **2–4 minutes**.

## Telemetry required

Track:
- level start/end;
- wave start/end;
- actual duration;
- preparation time;
- spawned/killed/leaked enemies;
- gold earned/spent/refunded;
- boss duration;
- tower mix used by player.

## Definition of Done — Vertical Slice

Player can:
1. enter from Main Menu;
2. choose Level 1;
3. place all three tower roles;
4. start the first wave manually;
5. see automatic timers for later waves;
6. complete exactly seven authored test waves;
7. encounter Heavy Hoplites;
8. fight Menelaus with a special mechanic;
9. upgrade and sell towers;
10. use Hector and one active ability;
11. see clear combat feedback;
12. pause/resume;
13. win or lose;
14. return to menu;
15. play using the New Input System without gameplay logic being tightly coupled to it;
16. produce pacing telemetry.

Only after this passes should Chapter I be considered production-ready.

---

# MILESTONE 2 — Chapter I: The Landing

## GDD target

- duration: **11–13 minutes**;
- combat events: **5**;
- location: coast;
- routes: 2;
- climax: Menelaus.

Reference event pacing:

```text
1. First Landing       ~60 sec
2. Second Beachhead    ~75 sec
3. Spear Push          ~90 sec
4. Heavy Assault       ~110 sec
5. Menelaus            ~180 sec
```

## Systems required before content authoring

- final `WaveData` authoring workflow;
- `ChapterData` initial implementation;
- tutorial triggers;
- objective text system;
- boss HP bar;
- Hector MVP;
- chapter completion result;
- save of Chapter I completion/unlock Chapter II.

## Chapter-specific work

- coast visual environment;
- two readable approach lanes;
- tutorialized Archer / Ballista / Priest usage;
- introduction of Heavy Hoplite;
- Menelaus Commander Aura encounter;
- first chapter score screen.

## Definition of Done — Chapter I

- median playtest duration is within 11–13 minutes;
- player understands build/upgrade/sell and Hector within first 5 minutes;
- Menelaus lasts roughly 2–4 minutes without becoming a pure HP sponge;
- Chapter II unlocks correctly;
- no tutorial step blocks replayability.

---

# MILESTONE 3 — Chapter II: Road to Troy

## GDD target

- duration: **14–16 minutes**;
- combat events: **6**;
- location: plains / road;
- up to 3 route possibilities;
- climax: split-lane assault.

## Systems required before content authoring

- authored ScriptableObject assets for `TowerData`, `EnemyData`, `WaveData`, `ChapterData`;
- route IDs and route enable/disable support;
- lane pressure indicators;
- enemy tags;
- targeting priority foundation;
- Chariot movement support;
- campaign economy carry-over rules finalized.

## New gameplay

- Chariots;
- shielded enemies;
- route priority changes;
- two-lane simultaneous pressure;
- more tactical build-zone layouts.

## Definition of Done — Chapter II

- route switching works without special-case code in `Enemy`;
- player must react to lane priority rather than reuse one fixed tower layout;
- chapter duration stays within 14–16 minutes;
- no authored wave requires source-code edits.

---

# MILESTONE 4 — Chapter III: The Gates

## GDD target

- duration: **18–20 minutes**;
- combat events: **7**;
- location: Trojan walls and main gate;
- first full siege chapter.

## Systems required before content authoring

- destructible objective framework;
- gate HP separate from generic base HP;
- repair system;
- siege-unit damage type/behavior;
- wall build positions;
- object damage VFX states;
- objective failure conditions.

## New enemies/content

- Ram Crew;
- Battering Ram;
- Siege Tower;
- Sapper;
- Spear Throwers unlocked.

## Climax

Temporary breach:
- route opens dynamically;
- objective changes mid-wave;
- emergency interior defense begins.

## Definition of Done — Chapter III

- siege units interact with the gate instead of behaving as reskinned infantry;
- repairs are economically meaningful;
- temporary breach changes route graph at runtime;
- target duration remains within 18–20 minutes.

---

# MILESTONE 5 — Chapter IV: Heroes of Greece

## GDD target

- duration: **17–19 minutes**;
- combat events: **6**;
- core theme: elite enemies and Greek heroes.

## Systems required before content authoring

- boss scripting framework;
- multi-phase boss support;
- reusable boss intro/outro events;
- enemy aura/buff system;
- elite resistance profiles;
- Hector full combat framework.

## New content

- Myrmidons;
- Greek Captains;
- Ajax;
- Achilles;
- Priests of Apollo full debuff kit;
- Hector abilities expanded.

## Boss requirements

### Ajax
- frontal mitigation;
- shield stance;
- countered by positioning/crossfire/fire.

### Achilles
- high mobility;
- rapidly defeats blockers;
- temporary near-invulnerability;
- vulnerability window tied to scripted combat state.

## Definition of Done — Chapter IV

- Ajax and Achilles require visibly different strategies;
- boss logic is data/script driven rather than hard-coded into the wave loop;
- Hector is tactically relevant but cannot solo encounters;
- target duration remains within 17–19 minutes.

---

# MILESTONE 6 — Chapter V: The Great Assault

## GDD target

- duration: **20–22 minutes**;
- combat events: **7**;
- strongest traditional TD chapter.

## Prerequisites

All core combat systems must be production-ready before Chapter V. Do not add foundational combat architecture during this chapter unless a blocking defect is found.

## Required gameplay

- all tower types available;
- mixed infantry/heavy/siege formations;
- captains and elites;
- temporary route changes;
- disabled/destroyed build positions;
- wall segment events;
- fire hazards;
- Trojan reinforcements.

## Definition of Done — Chapter V

- all previous combat systems combine without excessive bespoke scripting;
- performance remains acceptable at maximum intended enemy density;
- no per-frame scene-wide enemy/tower scans;
- chapter remains within 20–22 minutes;
- tower roster has no obviously dominant universal answer.

---

# MILESTONE 7 — Chapter VI: The Horse

## GDD target

- duration: **14–16 minutes**;
- approximately **4 major interactive/combat events**;
- deliberate pacing reset;
- not a conventional TD chapter.

## Systems required before content authoring

- narrative state machine;
- dialogue/event presentation;
- scouting interaction points;
- campaign choice storage;
- chapter modifiers;
- day/night transition support;
- save checkpoint before Chapter VII.

## Required sequence

1. Greek assault pressure collapses.
2. Enemy camp appears abandoned.
3. Ships appear to leave.
4. Player performs limited scouting/defensive allocation.
5. Trojan Horse is discovered.
6. Player makes preparation choices.
7. Transition to night.

## Choice outputs

Choices must modify Chapter VII mechanically, for example:
- starting inner-city defenders;
- gate/wall state;
- starting gold;
- Fire Tower bonuses;
- delay before internal Greek spawns;
- evacuation readiness.

The canonical fall of Troy cannot be prevented.

## Definition of Done — Chapter VI

- chapter feels structurally different from Chapters I–V;
- choices are persisted and visibly affect Chapter VII;
- no fake branching promises an alternate canonical ending;
- duration remains within 14–16 minutes.

---

# MILESTONE 8 — Chapter VII: Troy Burns

## GDD target

- duration: **16–18 minutes**;
- approximately **5 survival phases**;
- near-continuous pressure;
- minimal classic between-wave pauses.

## Systems required before content authoring

- internal spawn points;
- dynamic route activation/deactivation;
- evacuation objective system;
- fire propagation/hazard system;
- destructible/blocked streets;
- survival timer;
- civilian objective tracking;
- campaign result aggregation.

## Survival phases

```text
1. The Horse Opens
2. Streets Lost
3. Evacuation
4. Odysseus
5. Troy Burns
```

## Odysseus mechanics

- route manipulation;
- false attack indicators;
- temporary tower disruption;
- opens internal access points.

## Finale

The player is not trying to achieve a canonical military victory. The measurable success is:
- civilians saved;
- evacuation time gained;
- structures preserved;
- enemy heroes defeated;
- gold efficiency;
- defense duration.

End state: **Troy falls / Troy Burns**, followed by campaign score and epilogue.

## Definition of Done — Chapter VII

- gameplay is clearly different from the first five chapters;
- internal spawns and dynamic routes work reliably;
- choices from Chapter VI materially affect difficulty;
- finale lasts 16–18 minutes in target difficulty;
- ending and campaign result screen complete the full campaign loop.

---

# MILESTONE 9 — Campaign Completion / v1.0

## Required passes

### Balance
- Story / Strategos / Legendary difficulty;
- economy curves;
- tower counters;
- hero cooldowns;
- boss duration;
- wave pacing;
- refund ratios.

### Save system
Persist:
- unlocked chapter;
- selected difficulty;
- chapter scores;
- narrative choices;
- settings;
- campaign modifiers;
- final result.

Do not serialize runtime GameObjects directly.

### Performance
Use pooling for:
- enemies;
- projectiles;
- impact VFX;
- transient damage UI.

Targets:
- no scene-wide scans in hot gameplay paths;
- no avoidable allocations every frame;
- controlled target-query frequency;
- stable performance at Chapter V density and Chapter VII survival pressure.

### UX/accessibility
- resolution scaling;
- readable tower ranges;
- clear route telegraphing;
- color-independent threat indicators;
- separate music/SFX volume;
- pause/settings persistence;
- control hints;
- difficulty explanation.

### Audio/VFX
Replace procedural placeholders selectively with production assets where quality warrants it while retaining legal/licensing clarity.

Music states:
- ambient/menu;
- battle;
- boss;
- Chapter VI tension/night;
- Troy Burns finale.

---

# Development Order Summary

```text
0. Architecture Foundation
        ↓
1. Vertical Slice + Hector MVP
        ↓
2. Chapter I — The Landing
        ↓
3. Chapter II — Road to Troy
        ↓
4. Chapter III — The Gates
        ↓
5. Chapter IV — Heroes of Greece
        ↓
6. Chapter V — The Great Assault
        ↓
7. Chapter VI — The Horse
        ↓
8. Chapter VII — Troy Burns
        ↓
9. v1.0 Balance / Save / Optimization / Polish
```

---

# Global Quality Gates

Before moving to the next milestone:

1. Unity project compiles with zero errors.
2. Current milestone can be completed from Main Menu to its intended end state.
3. No new balance constants are added directly to combat controllers when the value belongs in data.
4. New enemy or tower types do not require rewriting the generic wave loop.
5. Pacing telemetry is reviewed against target duration.
6. New systems have a clear owner and do not duplicate existing responsibilities.
7. Main campaign flow remains replayable after the change.
8. Documentation is updated in the same milestone when architecture or GDD behavior changes.

---

# Immediate Next Work

The current codebase should now focus on completing **Milestone 0 + the missing portion of Milestone 1**, not Chapter II or later content.

Priority order:

1. Unity compile / Play Mode validation of the architecture pass.
2. Finish input abstraction layer.
3. Add authored ScriptableObject asset workflow.
4. Implement pacing telemetry.
5. Implement Hector MVP.
6. Implement Menelaus boss HP bar + Commander Aura.
7. Tune the seven-wave vertical slice to target pacing.
8. Only then convert the slice into production Chapter I content.
