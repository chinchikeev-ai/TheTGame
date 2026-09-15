# AI Task Contract

## Task ID
`ART-CH1-005-ANIMATOR-RECOVERY`

## Goal
Generated Chapter I character prefabs must always end with a usable `Animator` and the correct role-specific `RuntimeAnimatorController`, even when the imported KayKit character asset does not expose an `Animator` component and even when controller assets already existed before prefab regeneration.

## Why
`ChapterOneCharacterAnimationBuilder.AssignControllers()` previously skipped every prefab without an existing `Animator`. `CartoonCharacterPrefabBuilder` accepted KayKit sources with a `SkinnedMeshRenderer` even when no `Animator` was present, so the animation build could report `assigned to 0 production candidates` while controllers themselves were generated correctly.

## Owner module
Editor art/animation generation pipeline under `Assets/Editor`.

## Allowed files
- `Assets/Editor/ChapterOneCharacterAnimationBuilder.cs`
- `Assets/Editor/CartoonCharacterAutoBuilder.cs`
- `Assets/Tests/EditMode/ChapterOneGeneratedArtRecoveryTests.cs`
- `docs/tasks/ART-CH1-005-ANIMATOR-RECOVERY.md`

## Do not change
- runtime combat behavior;
- animation timing contracts in Hector/enemy controllers;
- gameplay colliders/navigation/stats;
- KayKit submodule contents or gitlink;
- `Assets/Resources/Music/BeyazGiyme.mp3`;
- art completion statuses.

## Inputs / source of truth
- `AGENTS.md`
- current `CartoonCharacterPrefabBuilder`
- current `ChapterOneCharacterAnimationBuilder`
- existing generated role controllers
- KayKit rigged/animated source assets

## Acceptance criteria
- [x] Missing `Animator` no longer causes a silent `continue` during controller assignment.
- [x] Repair places a missing `Animator` on the generated `Visual` model root when available.
- [x] Repair attempts to recover a valid imported `Avatar` through the source `SkinnedMeshRenderer.sharedMesh` asset path.
- [x] Correct role-specific controller is assigned and root motion is disabled.
- [x] Existing prefabs can be repaired without rebuilding controller assets.
- [x] `Build Missing Chapter I Art` always performs binding repair after regeneration, including when all `.controller` files already exist.
- [x] Generated-art validation reports missing Animator/controller/root-motion problems.
- [x] Source-contract EditMode tests cover the regression.

## Automated validation
- [ ] `python tools/check-architecture.py`
- [ ] Unity EditMode tests
- [ ] Unity PlayMode tests
- [ ] Full local validation/build

## Manual validation
In Unity, run `The Troy Game/Art/Build Missing Chapter I Art`, verify that the animation assignment count is non-zero, then run `The Troy Game/Characters/Repair Chapter I Animator Bindings` and `The Troy Game/Art/Validate Generated Chapter I Art`. Inspect Hector and representative Greek/Trojan/support prefabs in Play Mode to confirm the intended clips actually drive the rig.

## Known risks
KayKit import settings may expose Generic rather than Humanoid avatars. A null Avatar can still be valid for a Generic rig, so the recovery does not fail solely because no imported Avatar subasset is found. Real animation playback remains a Unity/Play Mode validation requirement.

## Result
- Added Animator/controller repair for existing generated prefabs.
- Added imported Avatar recovery where Unity exposes a valid Avatar subasset.
- Added automatic binding repair to the generated-art recovery command.
- Added binding problems to generated-art validation.
- Expanded EditMode source-contract regression coverage.
- Unity Editor/tests/build not run from this environment.

## Status
`IN_PROGRESS`
