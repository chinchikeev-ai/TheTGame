# AI Task Contract

## Task ID
`UI-002-FULL_RU_EN_LOCALIZATION_SWEEP`

## Goal
Remove the remaining player-facing English/Russian localization leaks across the current Chapter I menu, campaign and combat UI so the RU/EN visual-fit matrix can be evaluated honestly.

## Why
Visual-fit acceptance is invalid while Russian mode still contains untranslated English words or legacy Wave terminology.

## Owner module
`Assets/Game/UI`

## Allowed files
- `Assets/Game/UI/**`
- localization-focused EditMode tests
- `docs/TODO.md`
- this task contract

## Do not change
- gameplay balance
- encounter composition or pacing
- save schema
- campaign progression rules
- production-art acceptance
- internal Wave-prefixed compatibility APIs or hierarchy anchors unless they are player-facing text

## Inputs / source of truth
- `AGENTS.md`
- `docs/PROJECT_STATUS.md`
- `docs/MODULE_MAP.md`
- `docs/ENCOUNTER_TERMINOLOGY_MIGRATION.md`
- existing `GameLanguage.T(en, ru)` contract
- current `main` player-facing UI code

## Acceptance criteria
- [x] Result screen uses Encounter/Бой terminology, not Wave/Волна.
- [x] Campaign map has RU/EN player-facing place labels.
- [x] Settings do not expose untranslated English control/quality words in Russian mode.
- [x] Patron selection, encounter presentation and screenshot UI have confirmed leaks removed.
- [x] Known Chapter I boss names are localized where data display names would otherwise leak English.
- [x] Product branding/technical key names that intentionally remain language-neutral are explicitly treated as such.
- [x] Existing gameplay/public contracts are preserved by the source changes.

## Automated validation
- [ ] `python tools/check-architecture.py` — unavailable from the GitHub-only editing environment in this session.
- [x] `LocalizationContractTests` extended for localized Menelaus encounter-preview display.
- [ ] Unity EditMode/PlayMode — not available in this session and not claimed.

## Manual validation
- RU pass: main menu -> Army -> campaign map -> difficulty -> patron -> Chapter I combat -> pause/settings -> result at 1920x1080 and 1366/1376x768.
- EN pass over the same flow/resolutions for truncation/regression.
- Confirm intentionally language-neutral product/keyboard labels are acceptable.

## Known risks
Longer Russian labels can expose clipping or overlap that source-level localization cannot detect. That remains the subsequent visual-fit gate.

## Result
- Result screen migrated from `WAVES/ВОЛНЫ` to `ENCOUNTERS/БОИ` and Russian gate health no longer uses `HP`.
- Main menu subtitle and Army copy were completed in Russian; `THE TROY GAME` remains intentional product branding and Q/E/R/F, WASD, F12 and ESC remain keyboard names.
- Campaign-map `TROY` label now renders `ТРОЯ` in Russian.
- Settings localize `ARROWS`; Russian graphics quality displays a language-safe numeric level rather than raw Unity quality names.
- Athena patron description no longer exposes `HP` in Russian.
- Menelaus boss-arrival and encounter-preview names are localized.
- Screenshot button is `СНИМОК` in Russian.
- No gameplay balance, save, campaign progression or internal compatibility API changes were made.
- Unity compile/EditMode/PlayMode were not executed; the RU/EN visual-fit pass remains manual.

## Status
`DONE`
