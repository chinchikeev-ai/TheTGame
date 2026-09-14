# AI Task Contract

## Task ID
`UI-003-IN-GAME-SCREENSHOT`

## Goal
Allow the player/developer to capture the current rendered game view during play with one key press and save it as a PNG for visual review.

## Why
Current art work must be reviewed against the real Chapter I gameplay camera. A built-in screenshot command makes it easy to capture the actual runtime view instead of relying on external tools or separate concept renders.

## Owner module
`Assets/Game/UI` for the user-facing screenshot command/feedback, with keyboard access routed through `GameInput` in `Assets/Game/Core/Input`.

## Allowed files
- `Assets/Game/Core/Input/GameInput.cs`
- `Assets/Game/Core/Bootstrap/GameBootstrap.cs`
- `Assets/Game/UI/GameScreenshotController.cs`
- `Assets/Tests/EditMode/GameScreenshotControllerTests.cs`
- this task contract

## Do not change
- Chapter I camera position, angle, zoom or bounds
- gameplay routes, spawning, balance or combat
- campaign save schema
- existing screenshot resolution / graphics settings

## Inputs / source of truth
- `docs/ARCHITECTURE.md`
- `docs/MODULE_MAP.md`
- `docs/AI_PIPELINE.md`
- current Chapter I gameplay camera is the visual-review baseline

## Acceptance criteria
- [x] `F12` requests a screenshot through `GameInput`; no direct keyboard API is added outside `GameInput`.
- [x] Screenshot is saved as PNG under `Application.persistentDataPath/Screenshots`.
- [x] File names are timestamped and deterministic in format.
- [x] Capture includes the current rendered game view at the current resolution.
- [x] A short non-interactive confirmation toast appears after capture so it is not included in the captured frame.
- [x] Screenshot controller is composed by `GameBootstrap` and therefore works in normal runtime flow.
- [x] Existing gameplay/public contracts remain unchanged.

## Automated validation
- [ ] `python tools/check-architecture.py`
- [x] EditMode filename contract test added
- [ ] PlayMode capture smoke test (manual file/render verification is more useful for this feature)
- [ ] Full `tools/validate-project.ps1` or `tools/validate-project.sh` when Unity is available

## Manual validation
1. Enter Chapter I Play Mode.
2. Position the gameplay camera on the battlefield.
3. Press `F12` once.
4. Confirm a short saved-screenshot toast appears after capture.
5. Confirm a PNG appears under the game's persistent-data `Screenshots` folder.
6. Open the PNG and verify it matches the current game resolution/view and does not contain the post-capture toast.

## Known risks
- Actual output path is platform-specific because it lives under `Application.persistentDataPath`.
- `ScreenCapture.CaptureScreenshot` writes asynchronously; visual/file verification still requires a real Unity run.

## Result
Implementation is committed in source. Unity compile/Play Mode/file-output validation has not been claimed in this task session.

## Status
`IN_PROGRESS`
