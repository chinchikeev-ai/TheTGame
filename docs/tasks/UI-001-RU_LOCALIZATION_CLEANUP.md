# AI Task Contract

## Task ID
`UI-001-RU_LOCALIZATION_CLEANUP`

## Goal
Remove the major Chapter I RU localization leaks that currently make RU/EN visual-fit acceptance invalid: raw English enum names, authored tower display names, HUD abbreviations, boss badges, difficulty names and legacy Wave notification copy.

## Why
RU/EN visual-fit acceptance is not meaningful while Russian mode contains untranslated player-facing strings.

## Owner module
`Assets/Game/UI` with minimal shared-label support in `Assets/Game/Core/Balance/DifficultyRules.cs`.

## Allowed files
- `Assets/Game/UI/**`
- `Assets/Game/Core/Balance/DifficultyRules.cs`
- localization-focused EditMode tests/checks
- `docs/TODO.md`
- this task contract

## Do not change
- gameplay balance values
- encounter composition/pacing
- save schema
- production-art acceptance
- campaign progression
- Unity CI policy

## Inputs / source of truth
- `AGENTS.md` UI/localization rule
- `docs/MODULE_MAP.md`
- `docs/DATA_CATALOG.md`
- existing `GameLanguage.T(en, ru)` contract
- current Chapter I HUD/menu/presentation code

## Acceptance criteria
- [x] Russian mode does not expose raw `EnemyArchetype` or `TargetPriority` enum names in the enemy inspector / selected-defense UI.
- [x] Tower cards use localized display names, tags, level/stat abbreviations and target-priority labels.
- [x] Menelaus boss badges and enemy inspector labels are localized.
- [x] Combat encounter-start notification uses Encounter/Бой rather than Wave/Волна.
- [x] Difficulty labels have RU variants while preserving EN Story/Strategos/Legendary.
- [x] Existing gameplay balance and persistence contracts are untouched.

## Automated validation
- [ ] `python tools/check-architecture.py` was not executable from the GitHub-only editing environment.
- [x] Added `LocalizationContractTests` for RU/EN enemy labels, tower tags/priority and difficulty labels.
- [ ] Unity EditMode/PlayMode were not run and are not claimed.

## Manual validation
- Full player-facing localization sweep remains in `docs/TODO.md`, including newer concurrently edited HUD/menu surfaces.
- RU pass at 1920x1080 and 1366/1376x768 after the full source sweep.
- EN pass at the same resolutions to catch regressions/truncation.

## Known risks
- Longer Russian labels can expose layout/truncation issues; visual-fit acceptance remains a separate manual gate.
- `main` received concurrent HUD/menu changes during this task, so those newer surfaces must be included in the final manual localization sweep rather than overwritten by this branch.

## Result
- localized tower tags/names/selected-card abbreviations and target priorities;
- added localized enemy/archetype labels for the inspector;
- localized boss badges and difficulty labels;
- migrated encounter-start notification copy from Wave/Волна to Encounter/Бой;
- added RU/EN localization contract tests;
- added explicit full localization + visual-fit checks to the canonical TODO;
- Unity compile/EditMode/PlayMode not executed.

## Status
`DONE`
