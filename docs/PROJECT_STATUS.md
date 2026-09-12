# TheTroyGame Project Status

Last reviewed: 2026-09-12

## Campaign
- Chapter I: functional vertical slice, late RC stage
- Chapter II: unlocked by Chapter I save, content not implemented
- Chapters III-VII: planned in GDD/Roadmap

## Core
- GameBootstrap: implemented; canonical path `Assets/Game/Core/Bootstrap/GameBootstrap.cs`
- GameManager session facade: implemented; canonical path `Assets/Game/Core/Session/GameManager.cs`
- GameStateController: implemented
- EnemyRegistry: implemented
- TowerRegistry: implemented
- Runtime file logging: implemented
- CampaignSave JSON + backup + migration: implemented
- Difficulty save: implemented
- Difficulty combat modifiers: implemented
- EconomyController extraction: implemented
- ScoreController extraction: implemented
- CampaignController boundary: implemented
- ChapterController boundary: implemented
- GameInput facade: implemented; canonical path `Assets/Game/Core/Input/GameInput.cs`
- Hector/TowerPlacement/Menu input routed through GameInput

## Architecture v2
- `AGENTS.md`: implemented
- `docs/ARCHITECTURE.md`: implemented
- `docs/PROJECT_STATUS.md`: implemented
- architecture smoke validator: implemented
- canonical module path validation: implemented
- first GUID-preserving physical migration: implemented
- moved modules: Core/Bootstrap, Core/Session, Core/Input, Campaign controllers
- remaining physical migration: Combat, Towers, Enemies, Heroes, UI, World, Audio/VFX

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
- CameraController and some non-gameplay utilities may still use direct input and should migrate later

## Next architecture work
1. Physically migrate Combat/Towers/Enemies/Heroes/UI in GUID-preserving groups
2. Add assembly definitions after module boundaries stabilize
3. Add PlayMode smoke suite
4. Add DifficultyData ScriptableObject instead of static-only difficulty tuning
5. Add pooling before scaling later chapters
6. Add Chapter II data/runtime only after Chapter I RC validation
