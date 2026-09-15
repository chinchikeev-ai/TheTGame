# AI Task Contract

## Task ID
`UI-MAIN-SCREEN-ART`

## Goal
Compose the playable Unity main menu from the supplied MainScreen artwork.

## Owner module
UI (`Assets/Game/UI`).

## Allowed files
Main-menu presentation, MainScreen runtime textures, focused tests and status docs.

## Inputs / source of truth
`Pictures/MainScreen`, existing `Menu/Main_screen` background, GameMenuController actions.

## Acceptance criteria
- Separate supplied logo, Play, Heroes, Settings and Exit artwork renders over the clean background.
- Buttons route to existing game actions; Heroes shows Hector information and returns.
- Uniformly scaled layout fits 16:9, 4:3 and ultrawide; RU/EN labels remain readable.
- Architecture guard and focused Unity tests pass; screenshots are inspected.

## Do not change
Campaign progress, combat, authored balance and user project settings.

## Automated validation
Architecture guard, Unity compile and menu action/layout/capture tests.

## Manual validation
Inspect Canvas captures; full gameplay flow and Windows build remain separate checks.

## Known risks
Existing late-bound presenters must rebind after language-driven Canvas replacement.

## Status
DONE (implementation and focused validation)

## Result
- Added MainMenuArtwork and five byte-identical supplied PNGs with Unity import metadata.
- Updated MainMenuBackgroundOverride to use illustrated actions and Hector information.
- Added ten focused EditMode tests: artwork/actions, controller navigation/rebinding,
  modal blocking and eight RU/EN Canvas captures across four resolutions.
- Unity 6000.6.0f1 compilation and all ten tests passed; captured images inspected.
- Architecture guard passed. Full PlayMode suite and Windows build were not run.
