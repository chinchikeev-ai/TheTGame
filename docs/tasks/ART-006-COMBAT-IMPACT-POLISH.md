# AI Task Contract

## Task ID
`ART-006-COMBAT-IMPACT-POLISH`

## Goal
Make Chapter I combat impacts read clearly and feel punchier from the gameplay camera without changing combat balance.

## Why
Current runtime already has projectile, melee, hero and death presentation, but most hits read as small generic particles. The first polish pass should improve visual feedback before final production character art exists.

## Owner module
`Assets/Game/Combat` presentation layer.

## Allowed files
- `Assets/Game/Combat/CombatImpactPresentation.cs`
- `Assets/Game/Combat/CombatVfxPool.cs`
- `docs/TODO.md`
- this task contract

## Do not change
- damage values, cooldowns, tower/enemy/hero balance
- authored EncounterData/TowerData/EnemyData
- production-art completion statuses
- camera controls

## Inputs / source of truth
- `docs/ART_BIBLE.md`
- `docs/MODEL_ART_INVENTORY.md`
- existing `CombatImpactPresentation` / `CombatVfxPool` public APIs

## Acceptance criteria
- [x] Projectile hits gain a readable central impact beat and directional-looking shards.
- [x] Melee and Hector hits gain a stronger, short-lived top-down impact beat.
- [x] Heavy/gate/boss events remain visually larger than normal hits.
- [x] Existing public presentation methods remain callable without gameplay changes.
- [ ] No architecture guard violations.

## Automated validation
- [ ] `python tools/check-architecture.py`
- [ ] EditMode tests required/updated — not required for pure transient visual tuning; existing public contracts are preserved.
- [ ] PlayMode tests required/updated — manual visual QA is required for feel/readability.
- [ ] Full Unity validation — not run unless explicitly requested.

## Manual validation
- Compare normal projectile, spear, fire, slow, Trojan guard and Hector hits at normal gameplay zoom.
- Verify heavy/boss/gate impacts remain clearly stronger than ordinary hits.
- Verify pooled effects do not visibly disappear under a normal wave burst.

## Known risks
More simultaneous pooled shards/particles increase transient presentation load slightly. Pool sizes are still fixed and no per-hit material allocation is added.

## Result
- changed files: pending
- validation actually executed: none yet
- remaining manual checks: all visual checks above
- commit SHA: pending

## Status
`IN_PROGRESS`
