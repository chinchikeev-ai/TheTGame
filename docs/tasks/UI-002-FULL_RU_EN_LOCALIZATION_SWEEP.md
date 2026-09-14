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
- [ ] Result screen uses Encounter/Бой terminology, not Wave/Волна.
- [ ] Campaign map has RU/EN player-facing place labels.
- [ ] Settings do not expose untranslated English control/quality words in Russian mode.
- [ ] Patron selection, encounter presentation, screenshot UI and guidance contain no confirmed English leaks in Russian mode.
- [ ] Known Chapter I boss names are localized where data display names would otherwise leak English.
- [ ] Product branding/technical key names that intentionally remain language-neutral are explicitly treated as such.
- [ ] Existing gameplay/public contracts are preserved.

## Automated validation
- [ ] `python tools/check-architecture.py` — unavailable from the GitHub-only editing environment in this session.
- [ ] EditMode localization contract tests updated where deterministic helpers are introduced.
- [ ] Unity EditMode/PlayMode — not available in this session and will not be claimed.

## Manual validation
- RU pass: main menu -> Army -> campaign map -> difficulty -> patron -> Chapter I combat -> pause/settings -> result at 1920x1080 and 1366/1376x768.
- EN pass over the same flow/resolutions for truncation/regression.
- Confirm intentionally language-neutral product/keyboard labels are acceptable.

## Known risks
Longer Russian labels can expose clipping or overlap that source-level localization cannot detect. That remains the subsequent visual-fit gate.

## Result
To be completed after implementation.

## Status
`IN_PROGRESS`
