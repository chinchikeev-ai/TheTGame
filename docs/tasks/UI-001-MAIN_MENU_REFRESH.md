# UI-001 Main Menu Refresh

## Goal
Replace the flat prototype-style main-menu controls with distinct illustrated button assets and a cleaner Troy-themed composition while preserving the existing campaign, settings and exit behavior.

## Owner module
`Assets/Game/UI`

## Allowed files
- `Assets/Game/UI/GameMenuController.cs`
- `Assets/Game/UI/GameMenuUiFactory.cs`
- `Assets/Resources/Menu/Buttons/*`
- `docs/PROJECT_STATUS.md`

## Do not change
- Campaign progression or save implementation
- Chapter runtime installers
- Combat gameplay

## Acceptance criteria
- [ ] Main menu visibly differs from the old translucent-panel layout.
- [ ] CONTINUE / NEW CAMPAIGN / CHAPTER SELECT / SETTINGS / EXIT are separate illustrated UI images.
- [ ] Button labels remain EN/RU Unity text.
- [ ] Existing click handlers are preserved.
- [ ] Hover / press feedback still uses unscaled time while the menu is paused.
- [ ] EXIT still stops Play Mode in Editor and quits the built application.
- [ ] No architecture guard violations.

## Automated validation
- [ ] `python tools/check-architecture.py`
- [ ] Unity compile / EditMode / PlayMode / build when Unity is available

## Manual validation
- Check 1920x1080 and 1366/1376x768 composition in real Play Mode.
- Check RU/EN text fit.

## Status
IN_PROGRESS
