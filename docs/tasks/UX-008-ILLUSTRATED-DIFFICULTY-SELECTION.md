# AI Task Contract

## Task ID
`UX-008-ILLUSTRATED-DIFFICULTY-SELECTION`

## Goal
Replace the technical pre-map difficulty overlay with a full illustrated Troy-style difficulty screen based on the approved visual reference.

## Flow
Chapter I selection -> Difficulty -> Patron God -> Battle

## Difficulty cards
Three large vertical cards:
- Story / История — 190 starting gold
- Strategos / Стратег — 150 starting gold
- Legendary / Легенда — 120 starting gold

Each card includes:
- colored difficulty header
- character portrait
- short player-facing description
- starting-gold block
- motto
- selected highlight

## Interaction
- Clicking a card selects it only.
- BACK returns to chapter selection.
- NEXT confirms the selected difficulty and opens patron selection.
- Difficulty is persisted through CampaignController.
- GameManager applies the chosen starting economy/gate health before the run.
- EnemySpawner refreshes the pre-run encounter preview for the chosen difficulty.

## Acceptance criteria
- [x] Technical stacked difficulty buttons removed.
- [x] Three illustrated vertical cards implemented.
- [x] Explicit selected state.
- [x] Separate BACK and NEXT actions.
- [x] Starting gold shown as 190 / 150 / 120.
- [x] Runtime economy matches the confirmed difficulty.
- [x] First-encounter preview refreshes after difficulty confirmation.
- [x] RU/EN text supported.
- [x] EditMode coverage for card structure, selection, confirmation and 1920/1366/1376/1024 layouts.
- [x] PlayMode coverage for starting economy.
- [ ] Real Unity visual QA against the reference.

## Status
`IMPLEMENTED — VISUAL QA PENDING`
