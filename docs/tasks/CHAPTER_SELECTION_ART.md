# Illustrated Chapter Selection

## Scope and ownership
Recreate Pictures/Levels/ВыборГлавы.png as independently interactive UI elements.
GameMenuController retains navigation and run-start ownership. ChapterSelectionArtwork
owns presentation and local selection only; CampaignController provides unlock status.
No changes to campaign saves, balance, scene reloads or chapter implementation.

## Assets
- ChapterSelect/Background: generated clean map derived from the supplied reference.
- ChapterSelect/ReferenceAtlas: original source, sliced at runtime into title, markers,
  landing illustration, action/back buttons and metadata icons.
- ChapterSelect/Parchment: generated blank framed parchment for dynamic details.
- ChapterNodeMask: native circular UI stencil for marker crops.

Source prompts: clean background preserving coastline/Troy/ships/path while removing
all UI; separate right-hand carved frame with blank parchment and no text.
The original reference is preserved. These are candidate graphics, not pixel-exact
reconstruction: dynamic labels use the current project font and EN has live labels.

## Acceptance
Five selectable chapter nodes; only implemented Chapter I can start. Later chapters
show locked/in-development status rather than launch the wrong chapter. Back retains
the controller callback. RU/EN fit at 4:3 and 16:9. No user save writes in tests.

## Verification
All three focused EditMode cases passed on 2026-09-19; RU and EN captures inspected.
Widescreen preserves the reference composition with dark side margins, not stretching.
Focused EditMode tests render 1448x1086 RU, 1920x1080 RU and 1366x768 EN and check
selection, action gating, back callbacks and button screen bounds. Previews are in
Logs/Validation/ChapterSelection. Full gameplay visual acceptance remains pending.
No Windows EXE build. Architecture guard has the existing unrelated scene-wide search
finding in ChapterOneAegeanSeaPresentation.
