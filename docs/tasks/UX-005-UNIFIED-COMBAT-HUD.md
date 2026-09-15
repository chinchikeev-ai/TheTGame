# AI Task Contract

## Task ID
`UX-005-UNIFIED-COMBAT-HUD`

## Goal
Bring the remaining Chapter I combat HUD into the same dark-stone, bronze-framed, portrait-led visual language as the lower-left Hector block.

## Why
The Hector block reads as an intentional game interface while several other runtime HUD regions still look like unrelated prototype panels. Divine Power is also represented by two competing controls.

## Owner module
Primary owner: `Assets/Game/UI`.

## Allowed files
- `Assets/Game/UI/ModernCombatHud.cs`
- `Assets/Game/UI/CombatHudUiFactory.cs`
- `Assets/Game/UI/TroyCombatHudSkin.cs`
- `Assets/Game/UI/EnemyInspectorPresentation.cs`
- `Assets/Game/UI/CombatNotificationPresentation.cs`
- `Assets/Game/UI/BossHUD.cs`
- `Assets/Game/UI/ChapterOneGuidancePresentation.cs`
- `Assets/Game/Campaign/Runtime/ChapterOneRuntimeInstaller.cs`
- `Assets/Tests/PlayMode/CombatHudLayoutUxTests.cs`
- `docs/PROJECT_STATUS.md`
- `docs/TODO.md`
- this task contract

## Do not change
- Combat, economy, encounter, tower or Hector gameplay behavior.
- Patron selection timing.
- Save schema or campaign progression.
- Production-art status.

## Inputs / source of truth
- `docs/ART_BIBLE.md`, UI direction.
- `docs/GDD.md`, combat HUD requirements.
- `docs/VISUAL_OWNERSHIP.md`.
- Existing `HectorHUD` visual language.

## Acceptance criteria
- [x] Main HUD panels use the same sliced Trojan panel art as Hector.
- [x] Resources and encounter state use semantic portrait/icon anchors.
- [x] Divine Power has one clear, direct control with effect/readiness copy.
- [x] Enemy inspector uses the same panel/portrait hierarchy.
- [x] RU/EN content keeps existing responsive target containers.
- [x] No architecture guard violations.
- [x] Existing gameplay contracts are preserved.

## Automated validation
- [x] `python tools/check-architecture.py`
- [x] PlayMode layout tests updated.
- [ ] Full Unity validation when Unity is available.

## Manual validation
- Inspect 1920x1080 and 1366/1376x768 in RU and EN.
- Confirm all panels remain readable over bright and dark battlefield regions.
- Confirm the single Divine Power action remains obvious during cooldown and no-enemy states.

## Known risks
- Runtime-generated layout still requires real Play Mode visual QA.
- The user's modified music asset is unrelated and must remain untouched.

## Result
- Merge validation: Unity compilation and six selected EditMode tests
  (`DivinePatronCommentaryTests`, `LocalizationContractTests`) passed; architecture
  guard passed. Full PlayMode visual QA and Windows player build were not run.
- Local merge reconciliation (2026-09-15): preserved the illustrated Hector HUD,
  current Encounter API, RU/EN enemy labels and newer menu/balance changes.
- Imported patron portraits and event queue now use the existing
  `PatronCommentaryPresentation` owner with `DivinePatronCommentaryCatalog`.
  Generic notifications remain separate; no duplicate observer canvas is created.
- Direct Divine Power control is named `DivinePowerActions`, avoiding obsolete
  `CombatActions` migration behavior. Existing build-card geometry is preserved.
- Main runtime panels and buttons now use the same sliced Trojan art as Hector.
- Top resources, encounter status, Divine Power and enemy inspection use semantic portrait/icon anchors.
- Removed the duplicate corner magic/flyout and obsolete clarity presenter; `ModernCombatHud` owns one direct Divine Power action.
- Moved guidance, boss and inspector cards away from neighboring HUD regions.
- Architecture guard passed; Unity is unavailable in this environment, so real Play Mode visual QA remains.

## Status
`DONE`
