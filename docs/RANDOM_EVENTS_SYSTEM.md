# TheTroyGame — Random Battlefield Events System

## 1. Purpose

The campaign should include **random battlefield events** that can occur during maps and temporarily change tactical conditions.

These events are not decorative. Each event must affect one or more gameplay systems:

- routes;
- visibility;
- movement speed;
- tower-unit efficiency;
- Burn / Fire mechanics;
- economy;
- build availability;
- enemy behavior;
- Divine Power;
- civilian or neutral movement.

The goal is to make repeated runs of the same map feel less deterministic while preserving authored wave structure and balance.

Random events should create short tactical problems, opportunities, or temporary rule changes.

---

# 2. Core Rules

Recommended default:

- **0–2 random events per normal map**;
- major siege maps may allow up to **3**;
- tutorial sections should suppress disruptive events until the player understands core controls;
- final scripted climax phases may disable randomness when cinematic timing matters;
- the same event should not normally trigger twice on one map;
- events must respect chapter-specific eligibility rules.

Typical event duration:

- short event: **15–30 sec**;
- medium event: **30–60 sec**;
- weather event: **45–120 sec**;
- map-state event may persist until the end of the wave or map.

The event director should avoid triggering a severe negative event during an unavoidable boss mechanic unless specifically authored.

---

# 3. Event Categories

## WEATHER

Changes battlefield conditions globally or by region.

Examples:
- rain;
- fog;
- strong wind;
- heat wave;
- thunderstorm.

## WILDLIFE / CIVILIAN

Neutral creatures or civilians cross the battlefield.

Examples:
- flock of sheep;
- frightened horses;
- merchant cart;
- refugees;
- sacred animals.

## BATTLEFIELD

Physical changes to the combat space.

Examples:
- landslide;
- fire spreads;
- collapsed wall;
- blocked route;
- broken bridge;
- supply cache discovered.

## DIVINE / MYTHOLOGICAL

Rare events associated with gods, omens, or creatures.

Examples:
- Apollo's sunlight;
- ominous eclipse;
- divine wind;
- sacred omen;
- temporary blessing or curse.

## WAR CHAOS

Unplanned consequences of battle.

Examples:
- Greek deserters;
- panicked Trojan soldiers;
- stray siege projectile;
- lost supply wagon;
- enemy formation mistake.

---

# 4. Canonical Random Events

## EVENT 01 — Flock of Sheep

**Category:** Wildlife / Civilian  
**Tone:** humorous / tactical interruption

A large flock of sheep suddenly crosses one or more routes.

Effects:
- Light enemies temporarily slow while pushing through the flock;
- Trojan projectiles may have reduced target clarity in the affected area;
- sheep can split a marching formation into smaller groups;
- Cyclops and Fire Crew automatically avoid targeting the sheep unless the player explicitly enables dangerous fire rules.

Optional interaction:
- if no sheep are killed, player receives a small **Divine Power** or score bonus;
- if the flock reaches safety, it may provide a tiny Gold / morale reward.

Design purpose:
- visual humor in the spirit of the game's mythological adventure tone;
- temporarily reshapes enemy spacing without being strongly punitive.

Recommended duration: **20–40 sec**.

---

## EVENT 02 — Rain

**Category:** Weather

A sudden Mediterranean rainstorm begins.

Effects:
- existing Burn durations are reduced;
- new burning-ground effects are weaker or extinguished faster;
- Fire Crew effectiveness -25%;
- Fire Arrow Burn chance reduced;
- movement on dirt routes may slow Heavy units slightly;
- Poseidon-aligned effects gain a small bonus.

Potential upside:
- Greek fire hazards and enemy fires are also weakened;
- overheated / burning structures stop taking environmental Burn damage.

Design purpose:
- directly challenges Troy's Fire identity and forces temporary adaptation.

Recommended duration: **45–90 sec**.

---

## EVENT 03 — Heat Wave

**Category:** Weather

Extreme dry heat covers the battlefield.

Effects:
- Burn duration +25%;
- Fire Crew ground effects last longer;
- Fire Arrows gain improved ignition chance;
- enemy Heavy infantry moves slightly slower;
- Trojan Shield Guard stamina / resistance may decrease slightly if balancing requires a drawback.

This is a favorable Fire-faction event.

Recommended duration: **45–75 sec**.

---

## EVENT 04 — Strong Wind

**Category:** Weather

A sudden crosswind changes projectile behavior.

Effects:
- Archer accuracy or projectile travel time is temporarily reduced;
- Ballista remains mostly unaffected because of heavy bolts;
- Fire spreads farther in the wind direction;
- smoke drifts across parts of the map and may reduce visibility.

Optional directional mechanic:
- wind direction is shown in HUD;
- Fire Crew can exploit downwind lanes.

Recommended duration: **30–60 sec**.

---

## EVENT 05 — Dense Fog

**Category:** Weather

Fog rolls over the coast or plain.

Effects:
- enemy preview distance reduced;
- ranged Tower-Units lose part of their effective range;
- melee blockers remain unaffected;
- Priests of Apollo may partially reveal enemies inside their aura;
- Apollo Artifact / Divine Power effects can counter the fog.

Design purpose:
- makes information and short-range defense temporarily more valuable.

Recommended duration: **30–60 sec**.

---

## EVENT 06 — Frightened Horses

**Category:** Wildlife / War Chaos

Loose horses or a broken chariot team runs across the battlefield.

Effects:
- knocks aside some Light infantry;
- briefly disrupts both Trojan and Greek formations;
- may temporarily block a build point or lane edge;
- Chariot-type Greek enemies can lose formation efficiency.

Positive/chaotic event rather than pure punishment.

Recommended duration: **15–25 sec**.

---

## EVENT 07 — Merchant / Supply Cart

**Category:** Civilian / Economy

A Trojan supply cart crosses the defensive area.

Player choice:

### Protect It
If the cart reaches its destination:
- Gold reward;
- one temporary repair discount;
- optional Divine Power bonus.

### Ignore It
No penalty beyond losing the opportunity.

Enemies may attack the cart if it crosses an active route.

Design purpose:
- creates a short optional objective during a wave.

---

## EVENT 08 — Greek Supply Wagon Lost

**Category:** War Chaos / Economy

A Greek supply wagon enters the wrong route or is abandoned.

If destroyed before it escapes:
- bonus Gold;
- possible temporary reduction in the next Greek Heavy / Siege group.

If ignored:
- it exits normally with no major penalty.

This event creates an opportunity rather than a forced objective.

---

## EVENT 09 — Landslide / Rockfall

**Category:** Battlefield

A cliff section collapses.

Possible outcomes:
- one route becomes temporarily blocked;
- enemies reroute to another lane;
- some units are damaged;
- a new temporary chokepoint appears.

Cyclops interaction:
- Cyclops attacks near unstable terrain can have a very small chance to accelerate the collapse when explicitly authored.

Duration:
- usually until end of wave;
- occasionally map-persistent.

---

## EVENT 10 — Spreading Fire

**Category:** Battlefield / Fire

A brazier, siege projectile or burning building ignites nearby terrain.

Effects:
- creates an uncontrolled Burn zone;
- can damage both enemies and vulnerable friendly units;
- may close a path or force movement around it;
- Fire-aligned Tower-Units may gain temporary bonuses nearby.

Rain immediately weakens or ends this event.

This event reinforces Troy's elemental identity while showing that fire is not always under the player's control.

---

## EVENT 11 — Broken Aqueduct / Water Channel

**Category:** Battlefield

Water floods part of a route.

Effects:
- movement speed reduced;
- Burn is removed when enemies enter the flooded area;
- Poseidon / Charybdis control effects become stronger in the zone;
- heavy units suffer a larger movement penalty than Light units.

Can be beneficial or harmful depending on build.

---

## EVENT 12 — Apollo's Sunbreak

**Category:** Divine

Clouds open and intense sunlight covers part of the battlefield.

Effects:
- ranged visibility restored;
- Apollo Priests gain stronger aura;
- Fire effects gain small duration bonus;
- hidden / fogged enemies become visible;
- Divine Power generation slightly increases while active.

Rare positive event.

Recommended duration: **30–45 sec**.

---

## EVENT 13 — Ominous Eclipse

**Category:** Divine / Negative

The battlefield darkens unexpectedly.

Effects:
- ranged Tower-Unit acquisition range reduced;
- Greek Elite morale / movement may increase slightly;
- Divine Power generation temporarily reduced;
- fires and glowing projectiles become visually more prominent.

Priests of Apollo can mitigate part of the effect.

Recommended duration: **30–60 sec**.

---

## EVENT 14 — Divine Favor

**Category:** Divine

A rare god-specific short blessing occurs.

One effect is selected based on current Patron God or randomly from eligible gods.

Examples:

- Ares: Trojan melee damage +20%;
- Athena: structures gain temporary resistance;
- Apollo: ranged attack speed +15%;
- Poseidon: enemy movement speed -15%.

Duration: **20–30 sec**.

This should be uncommon and visually announced.

---

## EVENT 15 — Greek Formation Breaks

**Category:** War Chaos

A Greek group receives conflicting orders and temporarily loses formation discipline.

Effects:
- enemies spread apart or stop briefly;
- shield / aura bonuses are disabled for several seconds;
- creates a short attack opportunity.

This event helps avoid every random event feeling punitive.

---

## EVENT 16 — Trojan Panic

**Category:** War Chaos

A nearby breach or explosion causes temporary panic.

Effects:
- one non-elite Trojan Guard squad loses blocking efficiency;
- nearby support aura weakens;
- Hector or a Divine Power can immediately cancel panic.

If Hector reaches the affected area:
- War Cry ends the event;
- small Command Point reward.

Design purpose:
- gives Hector more reactive map responsibilities.

---

## EVENT 17 — Stray Siege Projectile

**Category:** War Chaos / Battlefield

An off-screen siege engine launches an inaccurate projectile.

The impact zone is telegraphed several seconds beforehand.

Possible consequences:
- damages enemies;
- damages a defensive structure;
- creates debris / temporary obstruction;
- starts a fire.

Critical rule:
- never hit a tower with no warning;
- clear telegraph gives the player time to react when relocation mechanics are available.

---

## EVENT 18 — Refugees Crossing

**Category:** Civilian

Trojan civilians attempt to cross part of the battlefield.

Effects:
- creates a temporary protection objective;
- some lanes or build zones become temporarily unavailable;
- reward for saving a high percentage:
  - Gold;
  - score;
  - Divine Power;
  - final campaign civilian statistic.

Best suited to later siege chapters.

---

# 5. Event Frequency by Chapter

## Chapter I — The Landing

Allowed:
- sheep;
- rain after tutorial section;
- wind;
- Greek formation break;
- supply cart.

Avoid:
- route destruction;
- severe fog during tutorial;
- civilian panic systems.

Target random events: **0–1**.

## Chapter II — Road to Troy

Strong event map.

Allowed:
- sheep;
- horses;
- supply wagons;
- rain;
- heat;
- wind;
- fog;
- landslide.

Target: **1–2**.

## Chapter III — The Gates

Allowed:
- rain;
- fire spread;
- stray siege projectile;
- broken water channel;
- supply cart;
- Trojan panic.

Target: **1–2**.

## Chapter IV — Heroes of Greece

Allowed:
- divine events;
- eclipse;
- sunbreak;
- formation break;
- panic;
- heat / rain.

Target: **1–2**.

## Chapter V — The Great Assault

Highest systemic chaos.

Target: **2–3 events**.

All battlefield-safe event families may appear.

## Chapter VI — The Horse

Random events should be quieter and more narrative:
- omen;
- refugees;
- animals;
- weather;
- abandoned supply cart.

Target: **0–1**.

## Chapter VII — Troy Burns

Mostly authored chaos.

Use only compatible random variants:
- collapsing structures;
- refugees;
- spreading fire;
- water release;
- panic;
- divine omen.

Target: **1–2**, but do not interfere with mandatory finale scripting.

---

# 6. Positive / Negative / Mixed Balance

The random-event pool must not become a punishment generator.

Recommended distribution:

- **30% positive opportunity**;
- **30% negative complication**;
- **40% mixed / situational**.

Examples:

Positive:
- Greek formation breaks;
- supply wagon;
- Apollo Sunbreak.

Negative:
- eclipse;
- Trojan panic;
- rain for a Fire-heavy build.

Mixed:
- sheep;
- wind;
- landslide;
- broken aqueduct;
- spreading fire.

The same event can be positive for one build and negative for another. This is desirable.

---

# 7. Event Warning UI

Events must be clearly communicated.

Recommended sequence:

1. short icon / horn / environmental cue;
2. banner appears near top-center;
3. one-line mechanical summary;
4. event starts after a short warning when reaction is possible.

Examples:

**RAINSTORM**  
`Fire effects weakened for 60 sec`

**FLOCK OF SHEEP**  
`Neutral flock entering the eastern route`

**LANDSLIDE**  
`Western route may be blocked`

**APOLLO'S FAVOR**  
`Visibility and ranged support increased`

Do not force the player to infer important numerical effects only from visuals.

---

# 8. Event Director Architecture

Recommended runtime system:

```text
RandomEventDirector
  chapterId
  eventPool[]
  minEvents
  maxEvents
  cooldownBetweenEvents
  excludedCombatStates[]
  activeEvent
  eventHistory[]
```

Each event definition should be data-driven:

```text
RandomEventData
  id
  displayName
  category
  weight
  minChapter
  eligibleChapters[]
  duration
  warningTime
  tags[]
  incompatibleTags[]
  positiveNegativeClass
  prefab / VFX references
  gameplayModifiers[]
```

Avoid hard-coding event logic inside WaveManager.

The event system should subscribe to game state and temporarily apply modifiers through reusable gameplay systems.

---

# 9. Anti-Frustration Rules

1. Do not trigger two major negative events at once.
2. Do not fire a random event during a boss cinematic.
3. Do not destroy an upgraded Tower-Unit randomly without telegraph and counterplay.
4. Do not make an objective mathematically impossible because of RNG.
5. Random events must never be required to win.
6. Favor variety over raw statistical difficulty.
7. Repeat protection: recently seen events receive reduced selection weight.
8. Difficulty level may change event severity, not only event frequency.
9. Story difficulty should bias toward neutral/positive events.
10. Legendary difficulty may allow stronger mixed consequences but must still provide counterplay.

---

# 10. Interaction with Existing Systems

## Fire Faction

Weather and battlefield events should directly interact with Troy's Fire identity:

- Rain suppresses Burn;
- Heat strengthens Burn;
- Wind spreads Fire;
- flooding extinguishes Fire;
- uncontrolled fire creates risk and opportunity.

## Cyclops

Cyclops benefits from clustered enemies and chokepoints created by events.

Examples:
- sheep divide a formation;
- Charybdis / water events group or slow targets;
- landslide creates a high-value artillery chokepoint.

## Scylla

Strong during formation disruption because it can attack multiple targets at once.

## Charybdis

Receives thematic synergy with:
- rain;
- flooded routes;
- broken aqueduct;
- Poseidon-aligned events.

## Hector

Hector can react to:
- panic;
- refugee defense;
- supply cart protection;
- sudden route changes.

This helps the hero remain tactically active beyond direct damage.

## Artifacts and Divine Powers

Artifacts may:
- alter event probability;
- reduce penalties;
- increase rewards;
- transform an event.

Divine Powers can provide emergency counters.

Example:
- Apollo effect clears Fog;
- Poseidon Power exploits flooded terrain;
- Athena protection helps during stray siege impact;
- Ares Fury stabilizes a panic-driven frontline.

---

# 11. Initial Production Scope

Do not implement all events immediately.

Recommended first production set:

1. Flock of Sheep;
2. Rain;
3. Heat Wave;
4. Strong Wind;
5. Supply Cart;
6. Landslide;
7. Spreading Fire;
8. Greek Formation Break.

These eight events are enough to validate:
- random scheduling;
- environmental modifiers;
- route interaction;
- economy opportunity;
- Fire-system interaction;
- positive vs negative balance.

After validation, expand toward the full event pool.

---

# 12. Canonical Principle

Random events should make the player say:

**"The plan changed — what do I do now?"**

not:

**"The game randomly decided I lose."**

The system exists to create tactical stories and memorable moments inside an authored campaign, not to replace authored encounter design with RNG.
