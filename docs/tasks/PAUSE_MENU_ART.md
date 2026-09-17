# Illustrated Pause Menu

Validation: three focused EditMode cases exercise five retained callbacks and
render RU 1600x900, RU 1024x768, EN 1600x900 under Logs/Validation/PauseMenu.
English uses live labels on existing framed artwork because source PNG text is
baked in Russian. Existing settings content is not redesigned by this change.
Architecture guard remains blocked by the unrelated scene-wide search in
ChapterOneAegeanSeaPresentation. Full gameplay/restart/quit QA remains pending.

Owner: UI. Existing GameMenuController retains all five button callbacks, time-scale,
settings return, restart and quit behavior. PauseMenuArtwork owns artwork and fitting.
Use six supplied Game_menu images as separate runtime assets, not a screenshot overlay.
Keep the actual paused battlefield visible behind a blocking dim layer.

Acceptance: five working buttons; no old decorative override; RU artwork / EN captions;
uniform fit at 16:9 and 4:3; settings returns to pause. No new EXE build.
Full in-game acceptance remains pending user review.
