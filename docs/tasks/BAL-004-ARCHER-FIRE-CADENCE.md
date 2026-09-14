# AI Task Contract

## Task ID
`BAL-004-ARCHER-FIRE-CADENCE`

## Goal
Slow the Chapter I Archer Tower firing cadence so arrows read as deliberate draw/release shots at the current gameplay camera instead of a machine-gun stream, while keeping its approximate baseline DPS role intact.

## Why
The authored Archer Tower currently fires at `4.2` attacks/sec (about one arrow every `0.24s`). That is too fast for the visible bow draw/release presentation and makes the unit read unnaturally at gameplay scale.

## Owner module
`Assets/Resources/Data/Towers/ArcherTower.asset` — authored `TowerData` is the runtime balance source of truth.

## Allowed files
- `Assets/Resources/Data/Towers/ArcherTower.asset`
- `Assets/Tests/EditMode/ArchitectureContractTests.cs`
- this task contract

## Do not change
- generic `Tower` firing code
- Archer animation/controller timing
- projectile implementation
- other tower balance
- encounter composition

## Inputs / source of truth
- `docs/DATA_CATALOG.md`
- `TowerData.attacksPerSecond`
- existing Archer Tower baseline: 16 damage × 4.2 attacks/sec = 67.2 raw DPS

## Acceptance criteria
- [x] Archer Tower base cadence reduced from 4.2 to 1.5 attacks/sec (one shot about every 0.67s).
- [x] Base damage increased from 16 to 45 so baseline raw DPS remains approximately unchanged (67.5 vs 67.2).
- [x] Archer remains meaningfully faster than Ballista while no longer visually reading as automatic fire.
- [x] Authored data remains the source of truth; no runtime special-case is added.
- [x] EditMode contract protects the intended cadence and approximate DPS.

## Automated validation
- [ ] `python tools/check-architecture.py`
- [x] EditMode contract test updated
- [ ] PlayMode tests
- [ ] Full Unity validation when Unity is available

## Manual validation
Build an Archer Tower in Chapter I at 1x speed and verify that individual draw/release/arrow flights can be visually distinguished. Re-check Level 2/3 and Hector War Cry cadence for readability.

## Known risks
Although baseline raw DPS is preserved, larger per-shot damage changes breakpoints against low-HP targets. Chapter I telemetry should be reviewed during the next real gameplay acceptance run.

## Result
Authored balance and contract test updated. Unity Play Mode validation is still required before gameplay freeze.

## Status
`IN_PROGRESS`
