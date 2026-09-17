# TheTroyGame — Golden Reference Index

Last reviewed: 2026-09-17

This document is the **approved Golden Reference index** referenced by `docs/ART_BIBLE.md` section 4.

`ART_BIBLE.md` remains the highest visual authority. Golden References make that direction concrete; they do not override the Art Bible, gameplay rules, camera contract, localization, or runtime implementation.

## 1. Storage

All documentation-only Golden Reference images belong under:

`docs/art/golden-references/`

Do **not** place these reference images under `Assets/` unless a separate task explicitly promotes a derivative into runtime production art.

Canonical file names:

```text
docs/art/golden-references/
├── GR-001-main-menu.png
├── GR-002-gameplay-1920x1080.png
├── GR-003-gameplay-1366x768.png
├── GR-004-pause-menu.png
├── GR-005-settings-menu.png
├── GR-006-hector-hud.png
├── GR-007-trojan-spearman.png
├── GR-008-greek-infantry.png
├── GR-009-ballista-tower-unit.png
└── GR-010-patron-god-zeus.png
```

Keep the files as lossless PNG unless repository size becomes a problem. Do not resize the gameplay references below their intended review resolution.

## 2. Reference authority matrix

| ID | File | Primary authority | What must be copied | What must NOT be copied blindly |
| --- | --- | --- | --- | --- |
| `GR-001` | `GR-001-main-menu.png` | Main-menu art direction | heroic/cartoon tone, red cloth + bronze + warm stone language, theatrical composition, strong focal Play action, Troy/coast identity | generated labels, exact button count, baked English text, exact background geometry |
| `GR-002` | `GR-002-gameplay-1920x1080.png` | Chapter I gameplay composition at primary 16:9 target | battlefield dominance, open tactical space, sea -> coast -> lanes -> Troy read, perimeter HUD ownership, compact Hector, centered defender strip, paired Magic/Defenders | exact enemy counts, exact prop placement, exact camera geometry if it conflicts with current runtime, generated text or balance values |
| `GR-003` | `GR-003-gameplay-1366x768.png` | Low-resolution HUD/readability target | same zone ownership as `GR-002`, condensed HUD, readable major labels/numbers, battlefield visibility | uniform scaling-down of all UI, exact generated values/text, any layout that conflicts with safe-area/runtime constraints |
| `GR-004` | `GR-004-pause-menu.png` | Pause/menu blocking-surface language | centered dominant panel, dimmed gameplay behind, stone/wood/bronze/red hierarchy, large readable actions | baked labels as sprites, exact generated iconography, excessive decoration that harms lower resolutions |
| `GR-005` | `GR-005-settings-menu.png` | Settings screen composition and component language | ornamental header, left navigation, large content frame, bottom secondary/primary actions, reusable component logic | text baked into PNG buttons, exact generated settings values, one-off non-localizable controls |
| `GR-006` | `GR-006-hector-hud.png` | Hero block material and hierarchy | portrait-led read, compact status, HP priority, restrained ability row, dark body + bronze/gold trim + Trojan red | oversized portrait/frame, exact ability art if gameplay changes, baked runtime values |
| `GR-007` | `GR-007-trojan-spearman.png` | Canonical Trojan mass-soldier language | broad/stable silhouette, bronze/leather, red/ochre, spear + round shield, disciplined but cartoon read | exact anatomy, literal close-up detail that disappears at gameplay zoom, Classical-Spartan shorthand as universal design |
| `GR-008` | `GR-008-greek-infantry.png` | Canonical Greek mass-soldier language | cooler blue/pale cloth, slightly sharper expeditionary silhouette, spear + shield, organized mass-unit readability | exact lambda/emblem if later lore/art direction changes, overly heroic elite proportions for common infantry |
| `GR-009` | `GR-009-ballista-tower-unit.png` | Tower-Unit / crew design | visible crew, large readable siege mechanism, nervous engineer + practical loader contrast, wood/bronze/red Troy identity | treating it as an anonymous fantasy turret, exact platform architecture if gameplay footprint differs |
| `GR-010` | `GR-010-patron-god-zeus.png` | Patron God HUD corner treatment | corner anchoring, three-quarter inward gaze, illustrated personality, compact readiness state, divine motif integrated into Trojan HUD language | large permanent opaque panel, passport-style portrait, full generated copy duplicated elsewhere in HUD |

## 3. Golden Reference interpretation rules

Golden References are **directional contracts**, not screenshots to reproduce pixel-for-pixel.

Use them to lock:

- visual hierarchy;
- relative scale;
- silhouette language;
- palette family;
- material family;
- HUD zone ownership;
- density and breathing room;
- character/tower personality;
- menu composition language.

Do not use them to lock unless separately documented:

- gameplay values;
- enemy count;
- exact unit position;
- exact map geometry;
- exact copy;
- generated text embedded in an image;
- localization strings;
- exact icon meaning;
- exact runtime camera transform;
- final animation timing.

When a generated reference disagrees with `ART_BIBLE.md` or current gameplay contracts, the generated detail loses.

## 4. Runtime vs reference art

The files in this folder are **documentation references**.

They do not become runtime production assets merely by existing in the repository.

Runtime artwork must still:

1. live under the appropriate `Assets/Game/Art/...` path;
2. include Unity `.meta` files;
3. be wired into runtime;
4. pass real gameplay-camera visual QA;
5. be promoted through the status rules in `MODEL_ART_INVENTORY.md`.

A Golden Reference may contain baked text for communicating the target composition, but reusable runtime UI sprites should still follow `ART_BIBLE.md`: **background/frame art separate, icons separate where practical, localized text through TMP/runtime UI**.

## 5. Required reference set

The canonical v1 Golden Reference set consists of exactly these ten files:

1. `GR-001-main-menu.png`
2. `GR-002-gameplay-1920x1080.png`
3. `GR-003-gameplay-1366x768.png`
4. `GR-004-pause-menu.png`
5. `GR-005-settings-menu.png`
6. `GR-006-hector-hud.png`
7. `GR-007-trojan-spearman.png`
8. `GR-008-greek-infantry.png`
9. `GR-009-ballista-tower-unit.png`
10. `GR-010-patron-god-zeus.png`

Additional references require an explicit documentation update. Do not let the folder become an uncurated moodboard.

## 6. Current status

The ten visual candidates were approved as the **initial Golden Reference direction** during the 2026-09-17 art-direction pass.

They are not considered repository-tracked Golden References until the corresponding binary files are copied into `docs/art/golden-references/` using the exact canonical names above.

Until then:

- `ART_BIBLE.md` remains canonical;
- current runtime remains the implementation truth;
- the generated images remain approved candidates, not repository evidence.

## 7. Review lifecycle

Reference states:

- `CANDIDATE` — useful visual direction, not yet promoted;
- `GOLDEN` — explicitly listed here and binary present under the canonical path;
- `SUPERSEDED` — replaced by a newer reference and no longer used for new work.

Promotion to `GOLDEN` requires:

1. binary stored under the canonical path;
2. this index names it;
3. no conflict with `ART_BIBLE.md`;
4. for gameplay/HUD references, acceptance against the real target camera/resolution.

Do not silently overwrite a Golden Reference with a materially different image. If the direction changes, update this index and record the change in the relevant art task.
