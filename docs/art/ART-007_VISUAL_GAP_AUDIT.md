# ART-007 — Chapter I Visual Gap Audit

Date: 2026-09-17  
Task: `ART-007-CHAPTER-I-GOLDEN-REFERENCE-ALIGNMENT`

This audit is the required pre-implementation gap record for ART-007. It compares the current Chapter I runtime/source presentation with `ART_BIBLE.md` v2.0 and the approved Golden Reference direction.

This is **not** final Unity acceptance. Final judgement still requires real Play Mode review at the target resolutions.

## Evidence used

- current Chapter I runtime screenshot supplied during the ART-007 review;
- current source geometry/anchors in `ModernCombatHud`, `HectorHUD`, `PatronCommentaryPresentation` and `ChapterOneUiCompactPresentation`;
- `docs/ART_BIBLE.md` v2.0;
- `docs/GOLDEN_REFERENCES.md`;
- `docs/tasks/UX-005-UNIFIED-COMBAT-HUD.md`;
- current visual ownership contract in `docs/VISUAL_OWNERSHIP.md`.

## Gap matrix

| Area | Current Runtime / Source | Golden Target | Gap | Priority | Canonical owner |
| --- | --- | --- | --- | --- | --- |
| Battlefield scale / read | Chapter I reads as visually dense; edge HUD plus repeated small environment detail reduces perceived playable area | `GR-002`, `GR-003` | Tactical board does not breathe enough; sea -> landing -> two routes -> Troy hierarchy is weaker than target | P0 | `MapBuilder`, `ChapterOneBattlefieldDetails` and existing Chapter I environment owners |
| Hector HUD | Base `HectorHUD` canvas block is `440×292`; without compaction that is ~22.9% screen width and ~27.0% screen height at 1920×1080 | `GR-006`; Art Bible target roughly `<=18%` width / `<=22%` height | Hero block is materially larger than the approved composition budget | P0 | `HectorHUD`, Chapter I compact presentation |
| Magic / Defenders | `DivinePowerActions` source footprint is `370×132` in the upper-right region while `DefensesToggle` is `128×112` bottom-right | `GR-002`, `GR-003` | Wrong location relationship and strongly unequal visual weight | P0 | `ModernCombatHud`, Chapter I compact presentation |
| Ground / tertiary detail density | Runtime screenshot contains many small props, repeated marks and high-frequency warm detail | `GR-002` | Tertiary detail competes with units, placement state and lane read; open ground does not read as intentional breathing room | P1 | `ChapterOneBattlefieldDetails`, `MapBuilder`, existing environment presentation owners |
| Patron corner | Patron card source is a relatively wide card (`420×120`) offset down from the top-right, with portrait/card hierarchy reading more like a conventional information card | `GR-010` | Needs a more compact upper-corner presence and stronger inward-facing composition; generated portrait angle itself remains an asset QA concern | P1 | `PatronCommentaryPresentation`, Chapter I compact presentation |
| Trojan / Greek silhouettes | Runtime uses generated/source candidates for multiple character families | `GR-007`, `GR-008` | Direction is defined, but production prefab/material/gameplay-camera acceptance remains outstanding | P1 | character production pipeline / existing runtime bindings |
| Tower-Unit read | Base structures remain procedural with generated/source crew candidates | `GR-009` | Several defenses still read closer to functional structures than authored characterful Tower-Units | P1 | existing defense/tower visual owners |

## Measurable source findings

### Hector

At 1920×1080, the base `440×292` Hector block occupies approximately:

- width: `440 / 1920 = 22.9%`;
- height: `292 / 1080 = 27.0%`.

A uniform runtime scale of `0.75` yields approximately `330×219`, or:

- width: `17.2%`;
- height: `20.3%`.

That fits the current Art Bible geometry target while retaining the existing internal hierarchy and avoiding a duplicate Hector implementation.

### Magic and Defenders

The source geometry currently establishes two incompatible scales:

- Magic / Divine Power: `370×132`;
- Defenders opener: `128×112`.

ART-007 will normalize these to a paired lower-right control family with equal base footprint and a small horizontal gap. Gameplay state and input behavior must remain unchanged.

### Patron

The Patron presentation is already owned by a dedicated runtime component, so ART-007 should compact/reposition the existing owner instead of creating a new card/canvas. Portrait direction itself cannot be guaranteed by layout code and remains part of art acceptance.

## Implementation decisions

ART-007 source work will use the following constraints:

1. **Do not change gameplay camera solely to imitate concept art.** Perceived map spaciousness is improved first by HUD footprint, visual hierarchy and environment density.
2. **Do not move routes or build points as an art fix.** Any route/layout change requires a separate gameplay/design decision.
3. **Compact Hector through the existing Chapter I presentation layer** rather than introducing a second hero HUD.
4. **Pair Magic and Defenders in the lower-right** with equal footprint and icon-first labels/state.
5. **Compact Patron in the upper-right** through the existing owner/presentation layer; do not create a competing Patron canvas.
6. **Reduce tertiary environment noise only through existing environment owners.** Decorative changes must not add gameplay colliders or alter pathing.
7. **Keep runtime text localized/TextMeshPro-driven.** Do not replace live labels with baked-language images.

## Acceptance split

### Source-verifiable in this pass

- Hector compacting rule exists and stays within the target composition budget by design.
- Magic and Defenders share the lower-right action family and equal base geometry.
- Patron uses a compact upper-right composition.
- No new competing HUD owner/canvas is introduced.
- No balance, encounter, save or campaign code is intentionally changed.

### Requires real Unity / human visual evidence

- actual 1920×1080 RU/EN fit;
- actual 1376×768 RU/EN fit;
- 1366×768 where available;
- whether the battlefield subjectively and measurably reads as sufficiently spacious after source changes;
- whether sea / beach / two lanes / Troy read in one glance;
- whether tertiary environment detail remains subordinate during peak combat;
- whether Patron portrait truly looks inward with production art;
- final character/Tower-Unit silhouette acceptance;
- final production-art `DONE` status.

## Audit result

`GAP CONFIRMED — IMPLEMENTATION MAY PROCEED`

The highest-impact source-level gaps are the Hector footprint, Magic/Defenders relationship and Patron corner footprint. Environment density remains a separate visual-production pass under the existing Chapter I environment owners and must be checked from the real gameplay camera before ART-007 can be closed.