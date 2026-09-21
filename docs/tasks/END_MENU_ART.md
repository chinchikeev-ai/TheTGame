# Illustrated results screen

## Goal / owner
UI: victory and defeat screens based on Pictures/Final, with separate live statistics
and three existing actions. GameMenuController remains the state/data/action owner.

## Scope / exclusions
EndMenuArtwork, GameMenuController integration, legacy result-skin opt-out, artwork
and isolated rendering tests. No balance, campaign unlock/save or restart changes.

## Acceptance
- Two distinct full-screen backgrounds, no baked sample scores.
- Separate banner, live title/chapter/summary/quote and three working buttons.
- RU/EN live content; fit 16:9, 4:3 and ultrawide without clipping controls.
- Existing callbacks preserved, no decorative input blockers over buttons.
- No EXE build unless requested.

## Art provenance / prompts
Built-in image generation edits of supplied victory/defeat images: preserve
characters, architecture, colors and side decorative signs; remove central results
cloth/text and bottom buttons, fill with gates/ruins and paving.
Separate generated assets: transparent dark crimson Trojan cloth with gold laurel,
horse crest and Greek border, empty center/no lettering; horizontal crimson/gold
laurel button with empty center/no lettering. Assets saved in Resources/EndMenu.

## Validation
Six focused EndMenuArtworkTests EditMode cases passed: victory/defeat across
1600x900, 1024x768 and 2560x1080, RU/EN. Assertions cover full-bleed background,
button bounds, preserved live summary, legacy skin opt-out and callback listeners.
Preview captures: Logs/Validation/EndMenu; test report: end-menu-art-tests.xml.
Architecture check and git diff --check passed. No EXE build.
These are isolated Canvas checks, not an end-to-end victory/retry playthrough.
Full gameplay acceptance pending. Decorative background signs retain source Russian.
