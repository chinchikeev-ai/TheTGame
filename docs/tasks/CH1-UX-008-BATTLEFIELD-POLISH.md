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
- [ ] Water no longer appears around the north/south sides of the playable beach frame.
- [ ] Divine-power action panel is anchored at the lower-right, above the defenders control.
- [ ] Every authored enemy archetype has 1.5x its prior HP multiplier.
- [ ] Right mouse button cancels build mode and selected tower state.
- [ ] Source-level regression coverage added.

## Automated validation
- [ ] `python tools/check-architecture.py`
- [ ] EditMode tests required/updated
- [ ] PlayMode tests required/updated
- [ ] Full Unity validation when available

## Manual validation
- Chapter I gameplay screenshot confirms no purple wall guards, no side-water leak and correct lower HUD placement.
- Right click visually clears the active tower/build selection.

## Known risks
- Coast edge closure is presentation-only and must not overlap the intended central Aegean shoreline.
- +50% HP materially changes encounter duration and will require later pacing review.

## Result
In progress.

## Status
`IN_PROGRESS`
