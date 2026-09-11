# TheTroyGame — Unity Implementation Roadmap

## Objective

Build the game as one production project on `main`, with vertical-slice milestones instead of throwaway prototypes.

Target: a complete story-driven Tower Defence campaign of roughly 120 minutes.

---

## Campaign Pacing Requirements

The campaign must be authored around time targets, not only enemy counts.

Default production target:
- normal map: **12–18 minutes**;
- normal map wave count: **5–7 waves**;
- ordinary wave duration: **60–120 seconds**;
- between-wave preparation: **15–30 seconds**;
- boss wave duration: **2–4 minutes**;
- reference map: **~15 minutes / 6 waves**.

Reference six-wave escalation:

```text
Preparation     30–45 sec
Wave 1          ~60 sec
Pause           ~20 sec
Wave 2          ~75 sec
Pause           ~20 sec
Wave 3          ~90 sec
Pause           ~20 sec
Wave 4          ~100 sec
Pause           ~20 sec
Wave 5          ~110–120 sec
Pause           ~25 sec
Wave 6 / Boss   ~150–210 sec
```

Campaign distribution target:
- early maps: **10–13 min / 5 waves**;
- middle maps: **14–17 min / 6 waves**;
- major siege maps: **18–22 min / 7 waves**;
- final Troy Burns map: **15–18 min near-continuous survival**.

Target total campaign structure: **38–42 waves / major combat events**.

Wave timing must live in authored data so designers can rebalance duration without editing gameplay code.

---

## Phase 1 — Vertical Slice

Build one polished combat map that proves the complete loop.

Required:
- 2 enemy routes converging on one Trojan base/gate;
- Start Wave button;
- automatic timer before next wave;
- 7 waves;
- light enemies;
- Heavy enemy;
- boss;
- Archer Tower;
- Ballista;
- Slow/Priest tower;
- Upgrade;
- Sell;
- placement/build zones;
- projectile system;
- muzzle flash;
- hit VFX;
- death/destruction VFX;
- basic generated SFX;
- Pause;
- Victory;
- Game Over;
- Settings;
- Level Select;
- New Input System support;
- Legacy Input fallback.

The vertical slice intentionally validates the upper end of the production wave range. Future chapters should use their authored chapter pacing rather than inheriting seven waves automatically.

Do not branch gameplay into separate prototype scenes unless technically necessary. Keep the real implementation in the main project architecture.

---

## Recommended Unity Structure

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

---

## Core Runtime Systems

### GameBootstrap
Responsible for:
- persistent services;
- save load;
- settings;
- scene transition;
- global references.

### GameStateController
States:
- Preparing;
- WaveRunning;
- BetweenWaves;
- Paused;
- Victory;
- Defeat;
- NarrativeEvent.

### WaveManager
Data-driven wave execution.

Responsibilities:
- wave index;
- countdown;
- manual Start Wave;
- enemy spawn groups;
- route assignment;
- completion detection;
- early-wave bonus;
- authored target duration;
- pacing telemetry: actual start/end time and actual duration.

The WaveManager should not assume all waves use the same spawn interval or preparation time.

### Route System
Use explicit route/path definitions rather than hard-coded movement logic.

Requirements:
- multiple routes;
- route merge points;
- runtime route enable/disable;
- alternate routes in later chapters.

### EnemyController
Composition-based enemy design.

Core properties:
- health;
- speed;
- armor;
- reward;
- route;
- resistance profile;
- status effects;
- special abilities.

### TowerController
Common tower runtime.

Components:
- target acquisition;
- aim;
- attack cadence;
- projectile/spawn attack;
- upgrade level;
- specialization;
- sell value.

### BuildSystem
Responsibilities:
- build node detection;
- selected tower type;
- affordability;
- placement validation;
- preview;
- construction;
- selection.

### EconomyManager
Tracks:
- gold;
- income/rewards;
- spending;
- refunds;
- Command Points later.

---

## Data Model

Prefer ScriptableObjects for static design data.

### TowerData
Suggested fields:
- id;
- displayName;
- cost;
- damage;
- range;
- attacksPerSecond;
- projectileSpeed;
- damageType;
- upgrade definitions;
- sellRatio;
- prefab;
- icon.

### EnemyData
Suggested fields:
- id;
- displayName;
- maxHP;
- speed;
- armor;
- reward;
- baseDamage;
- resistances;
- tags;
- prefab.

Tags:
- Light;
- Heavy;
- Shielded;
- Siege;
- Elite;
- Hero;
- Boss.

### WaveData
Required structure should support both composition and pacing:

```text
WaveData
  displayName
  preparationTime
  targetDuration
  isBossWave
  groups[]
    enemyType
    count
    spawnInterval
    routeId
    delay
```

`targetDuration` is a design target used for balancing and telemetry; the game does not need to forcibly terminate a wave when this time elapses.

Recommended authored duration range:
- early ordinary wave: 60–75 sec;
- middle wave: 75–100 sec;
- late ordinary wave: 100–120 sec;
- boss wave: 150–240 sec.

### ChapterData
Contains:
- scene;
- chapter name;
- unlocked towers;
- starting gold;
- objectives;
- wave set;
- narrative events;
- boss references;
- targetChapterDuration;
- pacingProfile.

Suggested pacing profiles:
- Early;
- Standard;
- Siege;
- Narrative;
- Survival.

Chapter validation should warn when authored waves plus preparation windows are obviously outside the chapter's target duration budget.

---

## Targeting

Initial target modes:
- First;
- Last;
- Strongest;
- Weakest;
- Closest.

At MVP, UI may expose only `First`.

Later advanced towers can use priority by enemy tag.

---

## Damage Pipeline

Recommended centralized calculation:

```text
Attack -> Hit -> Raw Damage -> Resistance/Armor -> Status Modifiers -> HP -> Death
```

Avoid implementing damage rules separately in each tower.

Damage types can begin with:
- Physical;
- Piercing;
- Fire;
- Hero.

---

## Status Effects

Create reusable effects:
- Slow;
- Burn;
- ArmorBreak;
- Stun;
- Fear.

MVP requires:
- Slow.

Fire tower later uses:
- Burn.

Priests later can use:
- Slow + ArmorBreak.

---

## Tower MVP

### Archer
Fast physical ranged tower.

### Ballista
Slow heavy projectile.

Bonus vs:
- Heavy;
- Siege.

### Priest / Slow Tower
Low damage or no damage.

Applies movement reduction to enemies inside range.

---

## Heavy Enemy MVP

Purpose: force Ballista usage.

Suggested prototype stats relative to standard infantry:
- HP: 4x;
- speed: 0.65x;
- armor: high;
- base damage: 2x;
- reward: 2.5x.

Exact numbers must be balance data, not constants in code.

---

## Boss MVP

Use Menelaus or a generic Greek Captain during the first vertical slice.

Boss requirements:
- boss health bar;
- high HP;
- identifiable model/silhouette;
- one special mechanic;
- intro cue;
- death event;
- victory trigger if final wave;
- authored target encounter duration of roughly 2–4 minutes.

Recommended first mechanic:
**Commander Aura** — nearby enemies receive movement or durability buff.

---

## Hector

Do not implement all hero complexity before basic TD works.

Order:
1. selectable Hector;
2. movement;
3. health/combat;
4. Spear Throw;
5. War Cry;
6. Shield Wall;
7. Ultimate.

Use separate ability components or ability definitions rather than a single large Hero script.

---

## UI MVP

Top HUD:
- Gold;
- Gate HP;
- Wave 3/7;
- Next Wave countdown.

Bottom build bar:
- Archer;
- Ballista;
- Priest.

Selected tower panel:
- name;
- level;
- damage;
- range;
- attack speed;
- Upgrade;
- Sell.

Screens:
- Main Menu;
- Level Select;
- Settings;
- Pause;
- Victory;
- Game Over.

---

## Input

Primary: Unity Input System package.

Create abstract actions:
- Point;
- Select;
- Cancel;
- Pause;
- Ability1;
- Ability2;
- Ability3;
- Ultimate.

Keep gameplay code independent of direct `Input.GetMouseButton` calls.

If Legacy Input compatibility is required, route it through a fallback adapter.

---

## Save Data

Campaign save should eventually contain:
- unlocked chapter;
- difficulty;
- chapter scores;
- choices;
- settings;
- campaign modifiers.

Do not serialize runtime GameObjects directly.

---

## Phase 2 — Chapters I–III

After vertical slice validation:

### Chapter I — Landing
- tutorial;
- coast map;
- Menelaus;
- 2 routes;
- 5 waves;
- target duration 10–13 minutes.

### Chapter II — Road
- 3 route possibilities;
- Chariots;
- more tactical build zones;
- 5–6 waves;
- target duration 13–15 minutes.

### Chapter III — Gates
- gate HP;
- Battering Rams;
- Siege Towers;
- repairs;
- wall build positions;
- 6–7 waves;
- target duration 17–20 minutes.

---

## Phase 3 — Hero Escalation

Add:
- Myrmidons;
- Ajax;
- Achilles;
- elite enemy logic;
- boss phase scripting;
- Priests of Apollo;
- Hector full abilities.

Chapter IV target: roughly 6 major combat events in 16–18 minutes.

---

## Phase 4 — The Great Assault

Full TD sandbox chapter using all systems.

Requirements:
- multi-wave combined arms;
- siege;
- route changes;
- destroyed build positions;
- temporary breaches;
- environmental hazards;
- 7 waves;
- target duration 18–22 minutes.

---

## Phase 5 — Trojan Horse

This chapter must not play like standard TD.

Implement:
- narrative state machine;
- scouting points;
- choices;
- transition to night;
- modifiers passed into Chapter VII.

Target: 12–15 minutes with roughly 4–5 short combat events mixed into narrative/scouting beats.

---

## Phase 6 — Troy Burns

New gameplay rules:
- internal enemy spawn points;
- route activation at runtime;
- fire zones;
- destroyed streets;
- evacuation objective;
- survival timer;
- Odysseus disruption mechanics.

Target: 15–18 minutes of near-continuous survival rather than a conventional wave/pause structure.

This is the final systems test of the campaign architecture.

---

## Performance Priorities

Use pooling for:
- enemies;
- projectiles;
- impact VFX;
- damage numbers where applicable.

Avoid:
- `FindObjectOfType` in per-frame code;
- per-enemy expensive path recalculation;
- excessive instantiated particle systems;
- tower-wide scene scans every frame.

Target acquisition should use controlled overlap queries or centralized enemy tracking.

---

## Pacing Telemetry

Before expanding beyond the vertical slice, add lightweight development telemetry for:
- chapter start/end time;
- wave start/end time;
- actual wave duration;
- preparation duration;
- enemies spawned/killed/leaked;
- gold earned/spent;
- boss encounter duration.

For each WaveData, compare `actualDuration` against `targetDuration` during playtests.

Use telemetry to tune enemy count, spawn interval, movement speed, HP and route pressure. Do not pad a short wave with idle waiting merely to reach its target time.

---

## Definition of Done — Vertical Slice

The vertical slice is complete when a clean build can be launched and the player can:

1. enter from Main Menu;
2. select the level;
3. place 3 different towers;
4. start the first wave manually;
5. see subsequent wave countdowns;
6. fight exactly 7 authored waves;
7. encounter normal and Heavy enemies;
8. encounter a boss;
9. upgrade and sell towers;
10. see clear firing/hit/death feedback;
11. pause/resume;
12. win or lose;
13. return to menu;
14. play with the New Input System without gameplay code depending directly on it;
15. record actual wave durations for balancing against the pacing targets.

Only after this passes should production expand to the full campaign.
