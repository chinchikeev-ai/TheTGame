# TheTroyGame Project Status

Last reviewed: 2026-09-12

## Campaign
- Chapter I: functional vertical slice, late RC stage
- Chapter II: unlocked by Chapter I save, content not implemented
- Chapters III-VII: planned in GDD/Roadmap

## Core
- GameBootstrap: implemented
- GameStateController: implemented
- EnemyRegistry: implemented
- TowerRegistry: implemented
- Runtime file logging: implemented
- CampaignSave JSON + backup + migration: implemented
- Difficulty save: implemented
- Difficulty combat modifiers: implemented
- Input abstraction: partial; direct input still exists in runtime controllers

## Combat
- DamageType / DamagePacket: implemented
- Physical / Piercing / Fire / Hero: implemented
- Slow: implemented
- Burn: implemented
- ArmorBreak: implemented
- Stun: not implemented
- Fear: not implemented
- Target priority: implemented

## Towers / defense
- Archer Tower: implemented
- Ballista: implemented
- Priests of Apollo: implemented
- Spear Throwers: implemented
- Fire Tower: implemented
- Trojan Guard blocking squad: implemented
- 3 core upgrade levels: implemented
- specialization branches: not implemented

## Hector
- selection/movement: implemented
- HP/damage/downed/revive: implemented
- basic attack: implemented
- Q War Cry: implemented
- E Shield Wall: implemented
- R Spear Throw: implemented
- F For Troy! ultimate: implemented
- cooldown HUD: implemented
- movement constraints: partial/not production-ready
- progression/upgrades: not implemented

## Menelaus
- boss controller: implemented
- boss HP bar: implemented
- Commander Aura: implemented
- reinforcements: implemented
- final-wave integration: implemented

## Chapter I
- 5 combat events: implemented
- tutorial/objective UI: implemented
- scoring: implemented
- save completion: implemented
- unlock Chapter II: implemented
- EN/RU language toggle without scene reload: implemented
- coast environment: procedural prototype implemented
- landing presentation: procedural prototype implemented
- production art: not implemented
- final 11-13 minute Play Mode validation: pending

## UI / UX
- menus: implemented prototype
- level select: implemented
- difficulty selection: implemented
- result summary: implemented
- build UI: functional but fragmented between panels; production pass pending
- enemy hover details: unverified / should be audited before claiming complete

## Performance / production risks
- projectile/enemy/VFX pooling: not implemented
- 50x speed: stress mode; requires real Play Mode validation
- runtime procedural primitives: prototype quality, not production art
- some controllers still use direct New Input System / legacy fallback instead of a unified input adapter

## Next architecture work
1. EconomyController extraction
2. ScoreController extraction
3. ChapterController / CampaignController boundary
4. EditMode contract smoke tests
5. PlayMode smoke suite
6. Incremental move from Assets/Scripts to Assets/Game modules with .meta preservation
7. Pooling before scaling later chapters