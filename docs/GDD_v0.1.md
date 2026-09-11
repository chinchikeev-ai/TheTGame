# TheTroyGame — Game Design Document v0.1

## 1. High Concept

**Genre:** Story-driven Tower Defence + light tactical hero control  
**Setting:** Trojan War  
**Target campaign length:** ~120 minutes  
**Player role:** Commander of Troy's defense  
**Core fantasy:** Hold Troy against escalating Greek assaults, legendary heroes, siege engines, deception, and the final collapse of the city.

The game is designed as a finite cinematic TD campaign rather than an endless wave mode. Every 15–25 minutes the battlefield, routes, enemy composition, objectives, and rules change.

---

## 2. Design Pillars

1. **Two-hour complete story** — beginning, escalation, climax, ending.
2. **Constant mechanical variation** — avoid repetitive wave grinding.
3. **Readable tactical decisions** — simple economy, clear counters, meaningful upgrades.
4. **Hero presence** — Hector gives the player active tactical agency.
5. **Escalation through spectacle** — larger armies, siege engines, heroes, fire, collapsing structures.
6. **Historical-mythic tone** — recognizable Trojan War figures without requiring strict simulation.

---

## 3. Campaign Structure

| Chapter | Time | Location | Main Purpose |
|---|---:|---|---|
| I. The Landing | 0–15 min | Coast | Tutorial and first defense |
| II. Road to Troy | 15–30 min | Plains | Multi-route combat |
| III. The Gates | 30–50 min | Troy walls | Siege gameplay |
| IV. Heroes of Greece | 50–70 min | Outer defenses | Elite units + boss escalation |
| V. The Great Assault | 70–90 min | Main walls | Full-system TD battle |
| VI. The Horse | 90–105 min | Troy / quiet phase | Narrative inversion |
| VII. Troy Burns | 105–120 min | Inside city | Survival finale |

Total target playtime: **115–125 minutes**.

---

## 4. Core Gameplay Loop

1. Observe incoming routes and wave composition.
2. Spend resources on towers, troops, and upgrades.
3. Reposition Hector and use abilities.
4. Counter enemy archetypes.
5. Survive wave/event.
6. Receive resources, unlocks, or story events.
7. Battlefield state changes and next tactical problem begins.

---

## 5. Resources

### Gold
Used for towers, upgrades, repairs, and troop deployments.

Sources:
- enemy kills;
- wave completion;
- bonus objective completion;
- preserving civilians / structures;
- boss phase rewards.

### Command Points
Secondary tactical resource used for:
- Hector abilities;
- temporary troop deployment;
- emergency repairs;
- battlefield commands.

Command Points regenerate slowly and through combat milestones.

---

## 6. Towers and Defensive Units

### 6.1 Archer Tower
**Role:** Basic DPS / anti-light infantry  
**Strengths:** Cheap, fast, reliable  
**Weaknesses:** Armor

Upgrade paths:
- Reinforced Bow — damage
- Eagle Eye — range
- Fire Arrows — damage over time
- Twin Archers — attack speed / multi-shot

### 6.2 Ballista
**Role:** Heavy single-target damage  
**Strengths:** Armor, siege units, bosses  
**Weaknesses:** Slow firing, poor swarm control

Upgrade paths:
- Heavy Bolt
- Piercing Bolt
- Reinforced Winch
- Siege Breaker

### 6.3 Spear Throwers
**Role:** Medium-range anti-heavy infantry  
**Strengths:** Hoplites, shield units  
**Weaknesses:** Fast swarms

Upgrade paths:
- Barbed Spears
- Heavy Javelin
- Volley
- Veteran Crew

### 6.4 Fire Tower
**Role:** Area denial / AoE  
**Strengths:** Dense infantry groups  
**Weaknesses:** Expensive, weaker vs isolated targets

Upgrade paths:
- Burning Pitch
- Wider Burn Zone
- Longer Burn
- Greek Fire Prototype

### 6.5 Priests of Apollo
**Role:** Slow / debuff support  
**Strengths:** Crowd control  
**Weaknesses:** Low direct damage

Effects:
- movement slow;
- armor reduction;
- temporary fear / disruption at high tier.

### 6.6 Trojan Guard
**Role:** Road blocking melee unit  
**Strengths:** Stops enemies, creates kill zones  
**Weaknesses:** Can die and requires redeployment

Upgrade paths:
- Larger squad
- Shield Wall
- Veteran Guard
- Royal Guard

---

## 7. Upgrade / Sell Rules

Each defensive unit has 3 core levels plus one specialization decision.

Example:
- Level 1: basic unit
- Level 2: +damage / effectiveness
- Level 3: strong specialization
- Level 4: branch A or B

Sell returns **60–75%** of invested gold depending on difficulty and upgrade tier.

The player should be encouraged to rebuild strategy when routes change.

---

## 8. Hector — Player-Controlled Hero

Hector exists physically on the battlefield and can be moved between predefined tactical nodes or freely navigated depending on final control design.

### Basic attack
Melee spear attack with moderate damage.

### Ability 1 — Shield Wall
Creates a temporary defensive line.

Effects:
- blocks enemies;
- reduces incoming damage;
- protects nearby Trojan Guard.

### Ability 2 — War Cry
Buffs nearby defenses.

Suggested values:
- +30% attack speed
- +15% damage
- duration: 10 sec

### Ability 3 — Spear Throw
High single-target burst damage.

Effective against:
- captains;
- siege crew;
- weakened bosses.

### Ultimate — For Troy!
Duration: ~10 seconds.

Effects:
- towers attack faster;
- Trojan troops gain damage resistance;
- enemies are slightly slowed;
- strong audio/visual feedback.

---

## 9. Enemy Roster

### Basic
- Greek Infantry
- Archer
- Spearman
- Light Swordsman

### Defensive / Heavy
- Shield Bearer
- Hoplite
- Heavy Hoplite
- Myrmidon

### Fast
- Scout
- Runner
- Chariot

### Siege
- Ram Crew
- Battering Ram
- Siege Tower
- Sapper

### Elite
- Myrmidon Veteran
- Greek Captain
- Royal Guard
- Hero Companion

---

## 10. Bosses

### Menelaus
**Chapter:** I or II  
**Mechanic:** Commander aura

Effects:
- buffs nearby Greek troops;
- periodically calls reinforcements.

Counterplay: isolate and burst him.

### Ajax
**Chapter:** IV  
**Mechanic:** Massive frontal defense

Effects:
- extreme HP;
- frontal damage reduction;
- shield stance.

Counterplay:
- crossfire;
- fire damage;
- Hector engagement.

### Odysseus
**Chapter:** VI/VII  
**Mechanic:** Disruption and deception

Effects:
- changes enemy route;
- temporarily disables a tower;
- creates fake attack indicators;
- opens interior access points.

### Achilles
**Chapter:** IV/V  
**Mechanic:** Fast elite hero

Effects:
- high movement speed;
- rapidly kills blocking units;
- temporary near-invulnerability phases;
- requires a vulnerability trigger before burst phase.

The vulnerability should be presented cinematically rather than as a literal target-the-heel gimmick.

---

## 11. Chapter Breakdown

# Chapter I — The Landing
**Duration:** 0–15 min

Map:
- coast;
- two enemy routes;
- simple build zones.

Introduces:
- placement;
- upgrades;
- sell;
- Hector movement;
- first ability.

Enemies:
- infantry;
- archers;
- light spear units.

Final event:
- Menelaus mini-boss.

Target waves: 4–5.

---

# Chapter II — Road to Troy
**Duration:** 15–30 min

Map:
- wider plains;
- 3 possible routes;
- chokepoints.

Introduces:
- Ballista;
- shield enemies;
- Chariots;
- changing lane priority.

Target waves: 5.

Final pressure event:
- split assault on two lanes.

---

# Chapter III — The Gates
**Duration:** 30–50 min

Map:
- Trojan walls;
- main gate;
- wall tower slots;
- siege approach routes.

Introduces:
- Battering Ram;
- Siege Tower;
- Spear Throwers;
- repair mechanic.

Objective layer:
- protect the gate HP;
- destroy siege engines before breach.

Target waves: 6.

Final event:
- temporary breach and emergency defense.

---

# Chapter IV — Heroes of Greece
**Duration:** 50–70 min

Map:
- outer defenses / damaged battlefield.

Introduces:
- Myrmidons;
- elite captains;
- Priests of Apollo;
- stronger hero usage.

Bosses:
- Ajax;
- Achilles.

Target structure:
- 3 normal waves;
- Ajax phase;
- recovery;
- Myrmidon assault;
- Achilles boss fight.

---

# Chapter V — The Great Assault
**Duration:** 70–90 min

Map:
- main walls at maximum pressure.

All tower types unlocked.

Enemies:
- mixed infantry;
- heavy units;
- siege engines;
- captains;
- elite formations.

Dynamic events:
- wall segment disabled;
- route temporarily opens;
- reinforcement arrival;
- fire hazard.

This chapter should be the strongest traditional Tower Defence section.

Target waves: 6–7.

---

# Chapter VI — The Horse
**Duration:** 90–105 min

Purpose: pacing reset and narrative inversion.

Sequence:
1. Greek pressure suddenly decreases.
2. Enemy camps appear abandoned.
3. Ships appear to depart.
4. Player receives limited scouting objectives.
5. Trojan Horse discovered.
6. Narrative decision sequence.
7. Transition to night.

Possible decisions:
- reinforce walls;
- strengthen inner city;
- inspect horse;
- allocate guards;
- preserve resources.

The historical outcome remains the fall of Troy, but player decisions modify final chapter advantages and disadvantages.

---

# Chapter VII — Troy Burns
**Duration:** 105–120 min

Map:
- city interior;
- streets;
- palace route;
- evacuation route;
- Trojan Horse spawn zone;
- dynamically opening paths.

Core twist:
Enemies do not only enter from the map edge. They spawn from internal breach points.

Events:
- buildings catch fire;
- streets become blocked;
- new passages open;
- some defenses are destroyed;
- civilians must be evacuated;
- alarms / combat audio intensify.

Boss:
- Odysseus as tactical disruption encounter.

Final objective options:
- Hold the palace gate;
- Protect evacuation route;
- Survive until evacuation completes.

Recommended final duration:
- 12–15 minutes continuous survival.

Ending:
- Troy falls;
- result screen based on civilians saved, structures preserved, heroes defeated, gold efficiency, and final defense duration.

---

## 12. Narrative Choice System

Choices should influence mechanics without creating an impossible branching-content burden.

Examples:

### Strengthen Walls
Benefit:
- +gate HP
- cheaper repairs

Cost:
- fewer starting units inside city

### Train More Guards
Benefit:
- stronger Trojan Guard

Cost:
- less gold for towers

### Stockpile Pitch
Benefit:
- Fire Tower bonuses

Cost:
- reduced economy

### Inspect the Horse
Benefit:
- delayed internal Greek spawn in Chapter VII

But:
- cannot prevent the canonical fall entirely.

---

## 13. Wave Philosophy

Do not build the game as dozens of nearly identical numbered waves.

Each wave should have a tactical purpose:
- introduce enemy;
- test counter;
- combine counters;
- overload one lane;
- fake pressure;
- introduce siege;
- introduce elite;
- boss setup;
- recovery;
- climax.

Recommended total explicit combat waves across campaign: **30–35**, plus bosses and scripted events.

---

## 14. Difficulty Modes

### Story
- generous gold;
- slower enemies;
- forgiving leaks;
- hero ability cooldown reduction.

### Strategos
Default intended experience.

### Legendary
- reduced economy;
- tougher siege engines;
- fewer build refunds;
- stronger boss mechanics;
- more aggressive route changes.

---

## 15. UI

### Combat HUD
Top:
- gold;
- Command Points;
- base/gate HP;
- chapter objective;
- wave status / countdown.

Bottom:
- tower selection panel;
- selected tower details;
- Upgrade;
- Sell;
- Hector abilities.

### Required screens
- Main Menu
- Continue
- Chapter Select / Level Select
- Settings
- Pause
- Victory / Chapter Complete
- Game Over
- Campaign Results

---

## 16. Controls

Target support:
- mouse + keyboard;
- Unity New Input System;
- legacy compatibility where practical.

Core actions:
- select build point;
- choose tower;
- place tower;
- select tower;
- upgrade;
- sell;
- select Hector;
- move Hector;
- trigger abilities;
- pause;
- speed control (optional).

---

## 17. Audio / VFX Direction

Visual tone:
- warm Mediterranean daylight early;
- dust and smoke during siege;
- orange-red fire palette in finale;
- monumental stone architecture;
- readable silhouettes over realism.

Required combat effects:
- projectile trails;
- arrow impacts;
- ballista recoil;
- fire zones;
- shield impacts;
- destruction particles;
- gate damage;
- building fire;
- boss intro effects.

Audio:
- bows;
- ballista tension/release;
- impacts;
- crowd/army ambience;
- horns;
- fire;
- gate impacts;
- hero ability stingers.

Prototype can use generated/in-engine SFX without external audio dependencies.

---

## 18. Victory / Failure

Chapter failure conditions can include:
- base/gate HP reaches 0;
- critical objective destroyed;
- evacuation target fails.

Final campaign scoring:
- civilians saved;
- gate integrity;
- total leaks;
- towers lost;
- gold efficiency;
- bosses defeated;
- optional objectives;
- completion time.

---

## 19. MVP Scope

First playable slice should include:
- one map;
- two converging enemy routes;
- 7 waves;
- Start Wave button;
- automatic countdown;
- Archer / Ballista / Priest or Slow equivalent;
- Upgrade / Sell;
- normal + heavy enemy;
- one boss;
- Hector basic movement and one ability;
- muzzle/projectile/impact/destruction feedback;
- basic SFX;
- menu;
- pause;
- victory/game over;
- New Input System + fallback input strategy.

This slice proves the full combat loop before building the 120-minute campaign.

---

## 20. Post-MVP Roadmap

### v0.2
- Hector full ability set
- additional towers
- siege enemies
- chapter transitions

### v0.3
- Chapters I–III
- persistent campaign state
- narrative events

### v0.4
- bosses
- Chapters IV–V
- advanced route changes

### v0.5
- Trojan Horse chapter
- city survival finale
- ending and campaign score

### v1.0
- balance pass
- visual polish
- sound/music
- accessibility
- difficulty modes
- save system
- optimization

---

## 21. Success Criteria

The design succeeds if:
- the player understands the combat loop in under 5 minutes;
- no 15-minute period feels mechanically identical to the previous one;
- bosses demand different tactical responses;
- the final 15 minutes feel structurally different from the opening 90 minutes;
- full campaign length remains near 2 hours without filler;
- the player feels escalating pressure and the inevitability of Troy's fall while still having meaningful tactical agency.
