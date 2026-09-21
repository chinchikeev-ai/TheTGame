# HUD layout recovery

## Owner / goal
UI. Restore the shared compact layout in real gameplay, not only isolated previews.
Player logs on 2026-09-20 repeatedly report failure to bind all compact HUD roots.
Screenshots show unscaled panels, stacked guidance and duplicate wave notifications.
Root cause: ModernCombatHud cached a Transform before Canvas replaced it with a
RectTransform. Creating the correct transform up front preserves the owner reference.

## Scope
ModernCombatHud, ChapterOneUiCompactPresentation, patron, guidance, boss and notification presenters.
No balance, artwork, map, save or input changes. No EXE build requested.

## Acceptance
- Each explicit owner root binds independently; missing/late roots do not disable layout.
- Replaced roots rebind without rewriting unchanged roots every frame.
- Patron remains at top right after resolution changes.
- One contextual guidance card at a time; no duplicate wave-start toast.
- Toast height matches actual entries.
- Decorative boss graphics do not intercept pointer input.

## Validation
Focused missing-root EditMode test and runtime-owner PlayMode test passed.
Architecture guard passed. Full CombatHudLayoutUxTests: 18/18 passed.
EditMode missing/late-root regression: 1/1 passed.
Full visual acceptance at runtime resolutions remains required.

The full suite also required contract updates: ornament is part of WaveHudArtwork,
corner anchors belong to CombatActionPair, advice uses compact offsets, patron
portrait tests must close the blocking menu, and selectable lookup must include
inactive objects when checking hidden UI. No assertion was removed to hide a failure.
