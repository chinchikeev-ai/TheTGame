# AI Task Contract

## Task ID
`UX-003-MINOR-GAMEPLAY-FIXES`

## Goal
Fix five Chapter I UX/gameplay issues: align the audio settings UI, keep selected-defense controls near the selected unit, choose one divine patron before battle for the whole map, enforce the authored 30-second first preparation, and constrain Hector to authored roads with a gate-side start.

## Why
The current runtime contradicts the intended player flow: audio controls are visually scattered, contextual defense controls are screen-pinned, divine gifts behave as per-wave consumables, the first encounter can bypass its authored preparation, and Hector can cross arbitrary terrain.

## Owner module
Primary owner: `UI`, with focused changes in `Enemies`, `Heroes/Hector`, `Campaign/Runtime`, `Core/Session`, and `Towers` where the gameplay contract requires it.

## Allowed files
- `Assets/Game/UI/...`
- `Assets/Game/Enemies/EnemySpawner.cs`
- `Assets/Game/Heroes/Hector/...`
- `Assets/Game/Campaign/Runtime/ChapterOneRuntimeInstaller.cs`
- `Assets/Game/Core/Session/GameManager.cs`
- `Assets/Game/Towers/Tower.cs`
- `Assets/Tests/EditMode/...`
- this task contract

## Do not change
- Chapter I encounter composition or enemy counts
- authored `Chapter01_01.asset` preparation value (already 30 seconds)
- campaign save schema
- Hector combat ability timings beyond patron damage multiplication
- video/gameplay/control settings behavior
- later chapter content

## Inputs / source of truth
- `AGENTS.md`
- `docs/MODULE_MAP.md`
- `docs/GDD.md`
- `docs/GOD_PATRON_ARTIFACT_SYSTEM.md`
- `Assets/Resources/Data/Encounters/Chapter01_01.asset`
- existing `ModernSettingsPresentation`, `ModernCombatHud`, `EnemySpawner`, `GameManager`, and Hector runtime contracts

## Acceptance criteria
- [x] Audio settings are presented as two aligned, readable cards for master and music volume while retaining immediate preview.
- [x] Selected defense controls follow the selected unit and remain clamped inside screen bounds.
- [x] Exactly one patron can be selected before the first encounter and cannot be reselected during the map.
- [x] The first encounter cannot be manually started before its authored 30-second preparation expires.
- [x] The first encounter also waits for patron selection if the 30-second countdown expires first.
- [x] Hector starts on the road immediately before the Troy gate.
- [x] Off-road Hector commands project onto authored routes and cross between routes only through a shared route junction.
- [ ] No architecture guard violations.
- [x] Existing public contracts preserved where practical (`UseGift()` retained as legacy compatibility; `MoveTo` retained).

## Automated validation
- [ ] `python tools/check-architecture.py`
- [x] EditMode tests added for 30-second authored prep, one patron per map, route projection, and gate start.
- [ ] PlayMode tests run/updated.
- [ ] Full `tools/validate-project.ps1` or `tools/validate-project.sh` when Unity is available.

## Manual validation
- Open Settings > Audio at 1366x768 and 1920x1080 in RU and EN; verify the two cards do not overlap and percentages/sliders align.
- Select defenses near each screen edge; verify the contextual panel stays near the unit and fully on-screen.
- Start Chapter I; verify patron chooser is above preparation UI, has no usable cancel, and cannot be reopened after selection.
- Attempt to start encounter 1 before 30 seconds; verify no enemy spawns.
- Right-click open terrain with Hector; verify he remains on road geometry and uses the shared road section to switch routes.

## Known risks
- Runtime-generated HUD/settings hierarchy means presentation helpers bind after the UI is constructed; real Play Mode validation is required for ordering and resolution edge cases.
- Patron effects implemented here are a minimal map-duration runtime slice, not the full 28-artifact campaign progression system.

## Result
- changed files: pending final diff
- tests added: `MinorGameplayUxContractTests.cs`
- validation actually executed: GitHub source review only; Unity/architecture validation not available in this execution environment
- remaining manual checks: listed above
- commit SHA: branch contains multiple focused commits; final head recorded in PR

## Status
`IN_PROGRESS`
