# Golden Reference Frames

This folder is the canonical repository location for approved visual reference binaries used by `docs/ART_BIBLE.md` and indexed in `docs/GOLDEN_REFERENCES.md`.

## Required files

```text
GR-001-main-menu.png
GR-002-gameplay-1920x1080.png
GR-003-gameplay-1366x768.png
GR-004-pause-menu.png
GR-005-settings-menu.png
GR-006-hector-hud.png
GR-007-trojan-spearman.png
GR-008-greek-infantry.png
GR-009-ballista-tower-unit.png
GR-010-patron-god-zeus.png
```

## Rules

- These images are documentation references, not runtime assets.
- Do not place them under `Assets/` unless a separate production-art task explicitly promotes a derivative into runtime use.
- `docs/ART_BIBLE.md` remains the highest visual authority.
- `docs/GOLDEN_REFERENCES.md` defines what each reference governs and what must not be copied literally.
- Do not silently overwrite a Golden Reference with a materially different image.
- Generated/baked text visible in a reference does not authorize baked runtime UI text; reusable runtime UI must continue to use localized TMP/runtime labels.
- Gameplay screenshots must be reviewed against the real gameplay camera and target resolution before promotion from candidate to Golden.

## Current state

The reference set is defined and the initial candidates have been generated, but the ten PNG binaries still need to be copied into this folder using the exact canonical names above.
