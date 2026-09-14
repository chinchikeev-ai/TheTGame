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
- [ ] No architecture guard violations — guard was not executed in this environment.

## Automated validation
- [ ] `python tools/check-architecture.py` — not run; repository is being edited through GitHub connector only.
- [x] EditMode tests required/updated — not required for this pure transient visual tuning; existing public contracts were preserved.
- [x] PlayMode tests required/updated — no automated visual-feel assertion added; manual visual QA is the correct acceptance check for this pass.
- [ ] Full Unity validation — not run; Unity CI remains manual-only and was not requested.

## Manual validation
- Compare normal projectile, spear, fire, slow, Trojan guard and Hector hits at normal gameplay zoom.
- Verify heavy/boss/gate impacts remain clearly stronger than ordinary hits.
- Verify pooled effects do not visibly disappear under a normal wave burst.

## Known risks
More simultaneous pooled shards/particles increase transient presentation load slightly. Pool sizes remain fixed and no per-hit material allocation was added.

## Result
- changed files: `Assets/Game/Combat/CombatImpactPresentation.cs`, `Assets/Game/Combat/CombatVfxPool.cs`, `docs/TODO.md`, this task contract.
- implementation: added a short central impact beat plus outward shards to projectile/melee/Hector/gate/heavy-death events; increased fixed VFX pool capacity.
- gameplay/balance changes: none.
- validation actually executed: static source inspection only.
- remaining manual checks: all visual checks above at real gameplay zoom/density.
- implementation commits: `d1541a2b83b6b1bb0dcdd4ed9f68643c3ff8f439`, `3f8589ec49b1abb556d6167fc840bb65582c7e7b`, `42d5b621ff0b9cbd6fe1f42f1c9f0998a84593c7`.

## Status
`DONE`
