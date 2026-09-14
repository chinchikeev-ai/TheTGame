# AI Task Contract

## Task ID
`BAL-004-CHAPTER1-STORY-PRESSURE`

## Goal
Bring Chapter I Story-mode pacing closer to the 11-13 minute target while making encounters 1, 2, 4 and 5 materially more demanding than the reported 07:26 playthrough.

## Why
Latest human playthrough feedback: Story victory at 1x completed in 07:26 and encounters 1, 2, 4 and 5 felt too fast/easy. Encounter 3 was not called out and should remain the local pacing reference.

## Owner module
Campaign encounter balance / Chapter I authored data.

## Allowed files
- `Assets/Game/Core/Balance/DifficultyRules.cs`
- `Assets/Resources/Data/Encounters/Chapter01_01.asset`
- `Assets/Resources/Data/Encounters/Chapter01_02.asset`
- `Assets/Resources/Data/Encounters/Chapter01_04.asset`
- `Assets/Resources/Data/Encounters/Chapter01_05.asset`
- `docs/tasks/BAL-004-CHAPTER1-STORY-PRESSURE.md`

## Do not change
- Encounter 3 authored composition/pacing.
- Enemy archetype reusable stats.
- Tower/Hector combat stats.
- Chapter sequencing, objectives, saves, UI, telemetry schema.
- Menelaus boss duplication rules.

## Inputs / source of truth
- Human Story playthrough: victory, 1x, pause used, 07:26 total, encounters 1/2/4/5 too fast.
- `docs/DATA_CATALOG.md` EncounterData ownership rules.
- `Assets/Resources/Data/Encounters/Chapter01_01.asset` through `Chapter01_05.asset`.
- `Assets/Game/Core/Balance/DifficultyRules.cs`.
- Chapter I target duration: approximately 11-13 minutes.

## Acceptance criteria
- [x] Encounters 1, 2, 4 and 5 have materially higher authored enemy pressure and longer spawn timelines.
- [x] Encounter 3 authored data is unchanged.
- [x] Story remains easier than Strategos in HP/count/economy while reducing the previous large Story advantage.
- [x] Menelaus remains exactly one explicit boss entry.
- [ ] Fresh human Story run at 1x and no pause lands in 11:00-13:00 or provides telemetry for the next tuning pass.
- [ ] No architecture guard violations.
- [x] Existing public contracts preserved.

## Automated validation
- [ ] `python tools/check-architecture.py`
- [ ] EditMode tests required/updated — no new runtime logic added; existing encounter-data validation is expected to cover authored asset integrity.
- [ ] PlayMode tests required/updated — no new runtime behavior introduced.
- [ ] Full `tools/validate-project.ps1` or `tools/validate-project.sh` when Unity is available.

## Manual validation
- Run Chapter I on Story at 1x without pause.
- Confirm total duration against 11:00-13:00.
- Record per-encounter duration/pressure, especially 1, 2, 4 and 5.
- Confirm wave 3 still feels correctly paced relative to the new curve.
- Confirm Menelaus remains beatable on Story without becoming trivial.

## Known risks
Authored encounter changes affect Strategos and Legendary too; those modes should be re-run after Story reaches target. Increased enemy counts may change economy because more kills create more reward opportunities, partially offset by the reduced Story reward multiplier.

## Result
- Changed Story start gold/gate HP/HP/count/reward multipliers to narrow the previous Story safety margin.
- Increased authored pressure for encounters 1, 2, 4 and 5 via `baseCount` and spawn pacing.
- Encounter 3 left untouched.
- Validation actually executed: GitHub-side content review only; Unity/architecture/full validation not run in this environment.
- Remaining manual check: fresh no-pause 1x Story playthrough.

## Status
`IN_PROGRESS`
