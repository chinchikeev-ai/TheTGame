# TheTroyGame Project Status

Last reviewed: 2026-09-12

## Campaign
- Chapter I: functional vertical slice, late RC stage
- Chapter II: unlocked by Chapter I save, content not implemented
- Chapters III-VII: planned in GDD/Roadmap

## Core
- GameBootstrap: implemented
- GameManager session facade: implemented
- GameStateController: implemented
- EnemyRegistry: implemented
- TowerRegistry: implemented
- Runtime file logging: implemented
- CampaignSave JSON + backup + migration: implemented
- Difficulty save/combat modifiers: implemented
- EconomyController: extracted
- ScoreController: extracted
- CampaignController boundary: implemented
- ChapterController boundary: implemented
- GameInput facade: implemented
- gameplay/UI input routed through GameInput

## Architecture
- `AGENTS.md`: implemented and authoritative AI entrypoint
- `docs/ARCHITECTURE.md`: implemented
- `docs/MODULE_MAP.md`: implemented
- architecture smoke validator: implemented
- GUID-preserving runtime migration: COMPLETE
- legacy `Assets/Scripts`: removed
- runtime assembly: `TheTroyGame.Runtime`
- editor assembly: `TheTroyGame.Editor`
- EditMode test assembly: implemented
- PlayMode test assembly: implemented
- direct Input outside GameInput: guarded by validator
- scene-wide FindObjects gameplay search: guarded by validator
- direct CampaignSave access from UI: guarded by validator

## Canonical modules
- `Assets/Game/Core`
- `Assets/Game/Campaign`
- `Assets/Game/Combat`
- `Assets/Game/Towers`
- `Assets/Game/Enemies`
- `Assets/Game/Heroes/Hector`
- `Assets/Game/World`
- `Assets/Game/UI`
- `Assets/Game/Audio`
- `Assets/Game/VFX`

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
- final 11-13 minute real Play Mode validation: pending

## UI / UX
- menus: implemented prototype
- level select: implemented
- difficulty selection: implemented
- result summary: implemented
- build UI: functional but fragmented; production pass pending
- enemy hover details: should be verified in Unity before claiming production-ready

## Automated validation
- architecture/editor smoke validator: implemented
- EditMode contract tests: implemented
- PlayMode runtime graph tests: implemented
- actual Unity execution in assistant environment: NOT AVAILABLE, therefore compile/test pass is still unverified

## Performance / production risks
- projectile/enemy/VFX pooling: not implemented
- 50x speed: stress mode; requires real Play Mode validation
- runtime procedural primitives: prototype quality, not production art
- runtime modules still share one assembly because gameplay dependencies are cyclic; split further only after interface/event decoupling

## Next product work after Unity validation
1. Fix any compile/test failures exposed by Unity after this migration
2. Final Chapter I 11-13 minute balance pass using runtime logs
3. Pool projectiles/enemies/VFX before Chapter II scale
4. Add Hector movement constraints/progression
5. Replace static DifficultyRules with authored DifficultyData if balance iteration demands it
6. Start Chapter II only after Chapter I RC validation
