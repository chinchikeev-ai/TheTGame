# AI Task Contract

## Task ID
`UI-001-RU_LOCALIZATION_CLEANUP`

## Goal
Chapter I player-facing UI shows coherent EN or RU copy without leaking raw English enum names, authored display names, HUD abbreviations, boss badges, or legacy Wave wording into Russian mode.

## Why
RU/EN visual-fit acceptance is not meaningful while Russian mode contains untranslated player-facing strings.

## Owner module
`Assets/Game/UI` with minimal shared-label support in `Assets/Game/Core/Balance/DifficultyRules.cs` and Hector-specific presentation in `Assets/Game/Heroes/Hector/HectorHUD.cs` where required.

## Allowed files
- `Assets/Game/UI/**`
- `Assets/Game/Heroes/Hector/HectorHUD.cs`
- `Assets/Game/Core/Balance/DifficultyRules.cs`
- localization-focused EditMode tests/checks
- `docs/TODO.md`
- `docs/PROJECT_STATUS.md`
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
- [ ] Russian mode does not expose raw `EnemyArchetype` or `TargetPriority` enum names in Chapter I UI.
- [ ] Tower cards/tooltips use localized display names, tags, level/stat abbreviations and target-priority labels.
- [ ] Menelaus boss badges/name and enemy inspector labels are localized.
- [ ] Player-facing Chapter I notifications/commentary use Encounter/Бой terminology rather than Wave/Волна.
- [ ] Difficulty labels have RU variants while preserving EN Story/Strategos/Legendary.
- [ ] No architecture guard violations.
- [ ] Existing gameplay/public contracts are preserved.

## Automated validation
- [ ] `python tools/check-architecture.py` when executable environment is available
- [ ] localization contract tests/checks updated where practical
- [ ] Unity EditMode/PlayMode not claimed unless actually run

## Manual validation
- RU pass at 1920x1080 and 1366/1376x768 after source cleanup.
- EN pass at the same resolutions to catch regressions/truncation.

## Known risks
- Longer Russian labels can expose layout/truncation issues; this change fixes source copy first, visual-fit acceptance remains a separate manual gate.

## Result
To be completed after implementation.

## Status
`IN_PROGRESS`
