# AI Task Contract

Use a copy of this file for any non-trivial gameplay, architecture, save, UI, chapter or balance change.

## Task ID
`<AREA>-<NUMBER>-<SHORT_NAME>`

## Goal
One concrete outcome. Describe observable behavior, not implementation preference.

## Why
Why this change exists and what player/developer problem it solves.

## Owner module
One primary owner from `docs/MODULE_MAP.md`.

## Allowed files
List files/directories the agent may modify.

## Do not change
List systems/contracts that are explicitly out of scope.

## Inputs / source of truth
Relevant GDD section, ChapterData/WaveData/TowerData/EnemyData assets, architecture docs or existing public API.

## Acceptance criteria
- [ ] Observable requirement 1
- [ ] Observable requirement 2
- [ ] No architecture guard violations
- [ ] Existing public contracts preserved unless task explicitly changes them

## Automated validation
- [ ] `python tools/check-architecture.py`
- [ ] EditMode tests required/updated
- [ ] PlayMode tests required/updated
- [ ] Full `tools/validate-project.ps1` or `tools/validate-project.sh` when Unity is available

## Manual validation
List only checks that cannot reasonably be automated yet (visual composition, feel, pacing, animation, audio, etc.).

## Known risks
State save compatibility, GUID changes, performance, scene lifecycle, balance or migration risks.

## Result
After implementation record:
- changed files
- tests added/updated
- validation actually executed
- remaining manual checks
- commit SHA

## Status
`TODO | IN_PROGRESS | BLOCKED | DONE`
