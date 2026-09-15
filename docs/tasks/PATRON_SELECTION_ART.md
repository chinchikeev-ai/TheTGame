# Patron Selection Artwork

## Goal / Owner
UI: assemble the pre-map patron page from Pictures/Gods without changing gift balance.

## Scope
PreMapPatronSelectionPresentation, PatronSelectionArtwork, five copied runtime textures,
focused EditMode tests and canonical status documentation. Do not modify character art,
campaign saves, difficulty rules, gift effects or the build pipeline.

## Acceptance
- Background and four separate clickable cards, Athena/Ares then Apollo/Poseidon.
- Selection highlight; confirmation disabled until a card is selected.
- Card click does not apply a gift. Confirm delegates to the existing Choose method.
- Back returns to difficulty; reopening clears the pending selection.
- Aspect-preserving layout and RU/EN captions; no dependency on Pictures in a player.

## Validation
Architecture guard and focused EditMode tests; real Unity render and player-flow check.
No Windows player build requested. Artwork remains a candidate until visual acceptance.

## Status
Implemented. Architecture guard passed. Eleven EditMode cases cover all four gift
callbacks, confirmation/reset/back, resource loading, and six isolated Canvas captures
(RU/EN at 1600x900, 1366x768 and 1024x768). No player save is changed by these tests.
Full menu-to-battle gameplay acceptance and an EXE build are not claimed.
