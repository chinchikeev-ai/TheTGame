# Settings artwork and pre-map flow repair

## Scope
- ModernSettingsPresentation retains settings actions and now uses reference-derived
  fullscreen artwork with live tabs, sliders and Back/Apply actions.
- SettingsArtworkLayout adapts the backdrop and control positions to the viewport.
- Two existing audio channels are exposed: master and music. Separate effects and
  voice buses do not exist and were not simulated with nonfunctional controls.
- Reference source: Pictures/Settings_menu/ChatGPT Image 17 сент. 2026 г., 10_03_24 (6).png.
  Generated clean backdrop removes form controls; the original provides knob artwork.

## Gameplay regression
PreMapPatronSelectionPresentation previously found chapter start by its text. The
new illustrated start action had no matching text, so binding failed and the run
started without a patron. The wave loop then waited indefinitely for GiftSelected.
The illustrated screen now exposes its actual start action for the pre-map flow.
Later unavailable chapters still cannot start.

First-wave button no longer stays unconditionally disabled. During mandatory
preparation it displays remaining seconds. It enables only after preparation and
patron selection; rejected start attempts no longer hide the preparation panel.
Existing automatic wave start and authored preparation duration are unchanged.

## Validation
All ten focused EditMode cases passed on 2026-09-20. All four settings-tab previews
were generated; Audio, Video and Controls were visually inspected after layout fixes.
Focused EditMode coverage: four chapter-layout/action cases, five settings previews
(all tabs, 4:3/16:9), and graphic-start-to-difficulty binding without save writes.
Full patron-confirmation-to-enemy-spawn PlayMode acceptance is still pending.
No EXE build. Architecture guard still reports the pre-existing scene-wide search
in ChapterOneAegeanSeaPresentation. Settings art remains pending in-game acceptance.
