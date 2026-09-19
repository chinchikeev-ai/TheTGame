# AI Task Contract

## Task ID
`UX-006-RUNTIME-INPUT-HUD-PERIMETER`

## Goal
Give runtime UI input one project-wide owner and finish the combat-HUD perimeter/raycast contract so later visual changes cannot silently break Hector/world input.

## Why
EventSystem creation previously existed in both Chapter I runtime installation and menu enhancement. Decorative HUD Graphics also remained capable of participating in UI raycasts. The lower-right action layout did not match the Art Bible contract.

## Owner module
Primary owners: `Assets/Game/Core/Input` for runtime input composition and `Assets/Game/UI` for combat HUD presentation.

## Allowed files
- `Assets/Game/Core/Bootstrap/GameBootstrap.cs`
- `Assets/Game/Core/Input/RuntimeInputBootstrap.cs`
- `Assets/Game/Campaign/Runtime/ChapterOneRuntimeInstaller.cs`
- `Assets/Game/UI/GameMenuUxEnhancer.cs`
- `Assets/Game/UI/CombatHudUiFactory.cs`
- `Assets/Game/UI/ModernCombatHud.cs`
- `Assets/Game/Heroes/Hector/HectorHUD.cs`
- `Assets/Tests/PlayMode/CombatHudLayoutUxTests.cs`
- `docs/PROJECT_STATUS.md`
- `docs/TODO.md`
- this task contract

## Do not change
- Combat balance, economy, encounter data or hero abilities.
- Save/campaign schemas.
- Production-art acceptance status.
- User-owned music assets.

## Inputs / source of truth
- `AGENTS.md`
- `docs/ART_BIBLE.md` sections 19-23.
- `docs/tasks/UX-005-UNIFIED-COMBAT-HUD.md`.

## Acceptance criteria
- [x] `GameBootstrap` creates one `RuntimeInputBootstrap` before chapter runtime/UI composition.
- [x] Only `RuntimeInputBootstrap` creates/configures runtime `EventSystem`.
- [x] New Input System UI module receives default actions and supersedes legacy modules.
- [x] Chapter I installer and menu enhancer no longer create EventSystems.
- [x] Decorative combat HUD panels/text/icons do not block pointer raycasts.
- [x] Raycast-enabled combat HUD Graphics belong to interactive Selectables.
- [x] Hector block is materially smaller while preserving its full information/actions.
- [x] MAGIC and DEFENDERS are equal-size bottom-right actions.
- [x] Expanded defender strip is bottom-center rather than occupying the right battlefield edge.
- [x] Existing gameplay callbacks/abilities are unchanged.

## Automated validation
- [x] PlayMode contracts updated for single runtime input ownership, interactive-only raycasts, compact Hector and bottom-right action parity.
- [ ] Run `python tools/check-architecture.py` in a local checkout.
- [ ] Run EditMode/PlayMode Unity validation.
- [ ] Run full validation/build only when explicitly requested.

## Manual validation
- Confirm LMB Hector selection, HUD-card selection and RMB movement.
- Confirm pause/settings/resume do not lose input.
- Confirm MAGIC/DEFENDERS and defender dock do not cover critical battlefield lanes at 1920x1080 and 1366/1376x768 RU/EN.
- Confirm Hector remains readable at the compact scale.

## Known risks
Runtime-generated UI geometry still needs real Play Mode inspection. Source-level changes have not been compiled in Unity in this GitHub-only session.

## Result
Runtime input ownership was centralized under Core/Input; duplicate ownership was removed from Chapter I/menu code. Combat HUD factory graphics now default to non-raycast decoration, while Buttons remain interactive. Lower-right actions and defender dock were realigned to the Art Bible perimeter contract, and Hector was compacted.

## Status
`IMPLEMENTED — UNITY VALIDATION PENDING`
