# AI Task Contract

## Task ID
`CH1-UX-008-BATTLEFIELD-POLISH`

## Goal
Close five visible Chapter I gameplay issues from the current playtest: purple Troy wall guards, sea leaking around the beach sides, divine-power HUD placed at the top, enemy health too low, and missing right-click selection cancel.

## Why
These issues are directly visible in the current Chapter I combat frame and reduce readability, visual cohesion and basic RTS-style interaction quality.

## Owner module
Chapter I World/UI/Enemies input presentation.

## Allowed files
- `Assets/Game/World/ChapterOneWallLife.cs`
- `Assets/Game/World/ChapterOneCoastEdgeClosure.cs`
- `Assets/Game/World/ChapterOneCoastEdgeClosure.cs.meta`
- `Assets/Game/UI/ChapterOneUiCompactPresentation.cs`
- `Assets/Game/Towers/TowerPlacement.cs`
- `Assets/Game/Heroes/Hector/HectorInputDriver.cs`
- `Assets/Resources/Data/Enemies/*.asset`
- `Assets/Tests/EditMode/ChapterOneBattlefieldPolishTests.cs`
- `Assets/Tests/EditMode/ChapterOneBattlefieldPolishTests.cs.meta`
- this task contract

## Do not change
- encounter composition/counts;
- tower damage/economy;
- Hector abilities/stats;
- music assets;
- save schema;
- Chapter I route geometry/gameplay colliders.

## Inputs / source of truth
- current Chapter I playtest screenshot;
- `EnemyData` authored assets;
- `GameInput.SecondaryPressed()` input contract;
- existing Chapter I presentation owners.

## Acceptance criteria
- [x] Troy wall guards run the same missing-material recovery used by Hector and tower crews.
- [x] North/south coast-edge water is covered by Chapter I presentation-only land extensions without changing the authored shoreline.
- [x] Divine-power action panel is anchored at the lower-right, above the defenders control.
- [x] Every authored enemy archetype has 1.5x its prior HP multiplier.
- [x] Right mouse button cancels build mode, selected tower state and Hector selection.
- [x] Source-level regression coverage added.

## Automated validation
- [ ] `python tools/check-architecture.py` not executed in this GitHub-only change.
- [x] EditMode source-level regression test added; execution not claimed.
- [ ] PlayMode validation pending local visual/interaction QA.
- [ ] Full Unity validation pending local workflow.

## Manual validation
- Chapter I gameplay screenshot should confirm no purple wall guards, no side-water leak and correct lower HUD placement.
- Right click should visually clear active tower/build/Hector selection.
- Recheck Chapter I duration after the +50% enemy-health balance increase.

## Known risks
- Coast edge closure is presentation-only and must not overlap the intended central Aegean shoreline.
- +50% HP materially changes encounter duration and will require pacing review.
- Right click no longer issues Hector movement commands; it is now reserved for cancel/deselect as requested.

## Result
Implemented directly on `main`:
- wall-guard null-material recovery;
- Chapter I north/south coast edge closure;
- lower-right divine-power HUD placement;
- +50% HP on all seven authored enemy archetypes;
- global secondary-click deselection for tower/build/Hector state;
- source-level regression coverage.

Implementation commits run through `02bbd19100e3e04d9254a4ed58196cd582c56902`; manual visual acceptance remains pending.

## Status
`IN_PROGRESS`
