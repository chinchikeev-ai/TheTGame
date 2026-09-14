# AI Task Contract

## Task ID
`UI-005-SIMPLE-MAIN-MENU-FLOW`

## Goal
Simplify the current approved main menu to four player-facing choices and make the path into battle explicit and short.

## Why
The current approved menu exposes PLAY / HEROES / TOWERS / UPGRADES / SHOP plus SETTINGS / EXIT, while several sections are placeholders. For the current solo-developed vertical slice this creates unnecessary menu clutter.

## Owner module
`Assets/Game/UI`.

## Allowed files
- `Assets/Game/UI/SimpleMainMenuPresentation.cs`
- `Assets/Game/UI/SimpleMainMenuPresentation.cs.meta`
- `Assets/Game/UI/PreMapPatronSelectionPresentation.cs`
- `Assets/Game/Campaign/CampaignController.cs`
- `docs/TODO.md`
- `docs/PROJECT_STATUS.md`
- this task contract

## Do not change
- combat balance values
- authored encounter composition
- save schema/version
- approved main-menu background art asset
- Unity CI configuration

## Inputs / source of truth
- current `MainMenuBackgroundOverride` approved-menu runtime layer
- current `CampaignMapPresentation`
- current `ModernSettingsPresentation`
- current patron selection flow
- developer decision: `PLAY / ARMY / SETTINGS / EXIT`

## Acceptance criteria
- [x] Main menu exposes only four active choices: PLAY, ARMY, SETTINGS, EXIT.
- [x] Legacy HEROES / TOWERS / UPGRADES / SHOP actions are blocked from interaction.
- [x] ARMY is a single lightweight screen with Hector / Defenders / Enemies tabs.
- [x] PLAY keeps the existing campaign map.
- [x] Chapter I selection opens difficulty selection before Patron selection.
- [x] Difficulty is selected explicitly as Story / Strategos / Legendary through `CampaignController`.
- [x] Patron selection remains mandatory before the map begins and cannot be changed after start.
- [ ] No architecture guard violations.

## Automated validation
- [ ] `python tools/check-architecture.py`
- [ ] EditMode: existing CampaignSave difficulty persistence coverage remains applicable.
- [ ] PlayMode: not run in this task.
- [ ] Full Unity validation: not run unless explicitly requested.

## Manual validation
- Main menu shows only PLAY / ARMY / SETTINGS / EXIT as usable navigation.
- ARMY opens and all three tabs switch content; Back/Escape returns to main menu.
- PLAY -> Chapter I -> Difficulty -> Patron -> Battle works.
- Back from Patron returns to Difficulty; Back from Difficulty returns to Chapter Select.
- Settings and Exit still work.
- RU/EN labels fit at 1920x1080 and 1366/1376x768.

## Known risks
The approved background image still contains historical baked menu artwork. The simplified presentation deliberately overlays and blocks the obsolete interactive regions without replacing that art asset yet.

## Result
- changed files: pending
- validation actually executed: none yet
- remaining manual checks: all checks above
- commit SHA: pending

## Status
`IN_PROGRESS`
