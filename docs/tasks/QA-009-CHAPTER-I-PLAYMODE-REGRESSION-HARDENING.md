# AI Task Contract

## Task ID
`QA-009-CHAPTER-I-PLAYMODE-REGRESSION-HARDENING`

## Goal
Harden Chapter I PlayMode coverage after the runtime-graph, input, HUD and lifecycle refactors.

## Coverage added

### Hector
- HUD selection contract now matches the real interactive owner: `HectorPanel`.
- Clicking the Hector HUD card must select Hector.
- A selected Hector must accept a screen-space move command.
- The move command must change the active destination and produce physical movement.

### Restart/runtime graph
- Restart Chapter I must replace the previous bootstrap/runtime context.
- Exactly one `GameBootstrap`, `RuntimeInputBootstrap` and `EventSystem` may remain after reload.
- Runtime context references must match the rebuilt singleton/runtime owners.

### Bootstrap ownership
- `GameBootstrap` now exposes `Instance`.
- Duplicate bootstrap objects destroy themselves instead of creating a second runtime graph.
- PlayMode coverage verifies duplicate rejection.

## Acceptance criteria
- [x] Hector HUD test no longer expects the non-interactive portrait to be a Button.
- [x] Hector movement has direct PlayMode regression coverage.
- [x] Restart validates runtime graph and EventSystem uniqueness.
- [x] Duplicate bootstrap ownership has PlayMode coverage.
- [ ] Run Unity PlayMode tests.
- [ ] Run the RU/EN 1920x1080 and 1366/1376x768 visual matrix.
- [ ] Perform one clean manual Chapter I start -> pause/settings/resume -> restart -> Hector select/move pass.

## Status
`IMPLEMENTED — UNITY EXECUTION PENDING`
