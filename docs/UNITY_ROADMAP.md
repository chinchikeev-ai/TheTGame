# TheTroyGame — Unity Implementation Roadmap

## Objective

Build the game as one production project on `main`, with vertical-slice milestones instead of throwaway prototypes.

Target: a complete story-driven Tower Defence campaign of roughly 120 minutes.

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
- early-wave bonus.

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
Suggested structure:

```text
WaveData
  preparationTime
  groups[]
    enemyType
    count
    spawnInterval
    routeId
    delay
```

### ChapterData
Contains:
- scene;
- chapter name;
- unlocked towers;
- starting gold;
- objectives;
- wave set;
- narrative events;
- boss references.

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
- victory trigger if final wave.

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
- 2 routes.

### Chapter II — Road
- 3 route possibilities;
- Chariots;
- more tactical build zones.

### Chapter III — Gates
- gate HP;
- Battering Rams;
- Siege Towers;
- repairs;
- wall build positions.

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

---

## Phase 4 — The Great Assault

Full TD sandbox chapter using all systems.

Requirements:
- multi-wave combined arms;
- siege;
- route changes;
- destroyed build positions;
- temporary breaches;
- environmental hazards.

---

## Phase 5 — Trojan Horse

This chapter must not play like standard TD.

Implement:
- narrative state machine;
- scouting points;
- choices;
- transition to night;
- modifiers passed into Chapter VII.

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
14. play with the New Input System without gameplay code depending directly on it.

Only after this passes should production expand to the full campaign.
