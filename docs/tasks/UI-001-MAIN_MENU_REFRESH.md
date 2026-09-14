# UI-001 Main Menu Refresh

## Goal
Make the approved 2026-09-14 main-menu reference the canonical player-facing main menu: large THE TROY GAME / GODS DEFENSE logo at upper-left, large PLAY button at upper-right, HEROES / TOWERS / UPGRADES / SHOP below it, SETTINGS in the upper-right corner, and EXIT in the lower-right corner.

## Why
The previous runtime menu drifted away from the approved reference and exposed prototype CONTINUE / NEW CAMPAIGN / CHAPTER SELECT controls on the main screen. The player should see the approved Troy cartoon-adventure composition instead.

## Owner module
`Assets/Game/UI`

## Allowed files
- `Assets/Game/UI/MainMenuBackgroundOverride.cs`
- `Assets/Game/UI/MainMenuBackgroundOverride.cs.meta`
- `Assets/Resources/Menu/ApprovedMainMenu/*`
- `docs/tasks/UI-001-MAIN_MENU_REFRESH.md`
- `docs/PROJECT_STATUS.md`

## Do not change
- Campaign progression or save implementation
- Chapter runtime installers
- Combat gameplay
- Chapter I encounter data

## Inputs / source of truth
- User-approved main-menu reference supplied on 2026-09-14.
- `docs/CREATIVE_DIRECTION.md`: Trojan War x Cartoon Tower Defence x Adventure Comedy.
- Existing `GameMenuController` public/runtime behavior for level select, settings and exit.

## Acceptance criteria
- [x] Main screen matches the approved reference composition instead of the prototype translucent-panel layout.
- [x] Visible main-menu actions are PLAY / HEROES / TOWERS / UPGRADES / SHOP / SETTINGS / EXIT.
- [x] PLAY opens the existing chapter-select flow.
- [x] SETTINGS calls the existing settings flow.
- [x] EXIT keeps the existing editor/build quit behavior.
- [x] HEROES / TOWERS / UPGRADES / SHOP remain clickable and show a localized in-development notice until their dedicated screens are implemented.
- [x] Menu actions have hover / press feedback using unscaled time while the menu is paused.
- [x] The approved reference image is stored losslessly enough for the target 16:9 menu presentation without the previous tiny low-quality background override.
- [ ] No architecture guard violations.

## Automated validation
- [ ] `python tools/check-architecture.py`
- [ ] Unity compile / EditMode / PlayMode / build when Unity is available

## Manual validation
- Check exact composition at 1920x1080 and 1366/1376x768 in real Play Mode.
- Check pointer hitboxes line up with the visible approved buttons.
- Check SETTINGS and EXIT behavior.
- Check hover/press glow readability.

## Known risks
- The approved art currently contains English labels baked into the reference. Full RU visual localization requires a later layered-art pass with separate logo/button sprites.
- Dedicated HEROES / TOWERS / UPGRADES / SHOP screens are not implemented yet, so these actions intentionally show an in-development notice.

## Result
Implementation is in progress. Final validation state and commit SHA must be recorded after the architecture check and Unity QA.

## Status
IN_PROGRESS
