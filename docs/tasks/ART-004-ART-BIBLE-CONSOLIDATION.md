# AI Task Contract

## Task ID
`ART-004-ART-BIBLE-CONSOLIDATION`

## Goal
Consolidate the project's creative tone, character direction, Trojan roster rules and Chapter I visual target into one canonical `docs/ART_BIBLE.md` that matches the real current gameplay view and contains no competing art-direction authorities.

## Why
Art direction is currently split across several overlapping documents. The overlap allows future AI/art work to drift between "cute cartoon", "serious stylized Bronze Age", close-up concept art and the actual tactical gameplay view. The project needs one final visual contract.

## Owner module
Documentation / production art direction.

## Allowed files
- `docs/ART_BIBLE.md`
- `docs/CREATIVE_DIRECTION.md`
- `docs/CHARACTER_ART_DIRECTION.md`
- `docs/TROJAN_UNIT_VISUAL_BIBLE.md`
- `docs/CHAPTER_I_VISUAL_TARGET.md`
- `docs/README.md`
- this task contract

## Do not change
- gameplay code
- Chapter I camera or map
- balance
- production-art completion statuses in `MODEL_ART_INVENTORY.md`

## Inputs / source of truth
- current Chapter I runtime view and camera are the visual baseline
- current `main` code remains runtime truth
- `MODEL_ART_INVENTORY.md` remains the only authority for art completion status
- user-approved pillars: vibe, cartoon, humor, grotesque exaggeration, selective sex appeal

## Acceptance criteria
- [x] One canonical creative + visual direction document: `docs/ART_BIBLE.md`.
- [x] Current tactical/top-down gameplay view is explicitly the production baseline.
- [x] Cartoon, humor and grotesque exaggeration are mandatory pillars rather than optional flavor.
- [x] Sex appeal is explicitly defined for adult female characters and selectively for roughly 10% of adult male character designs.
- [x] Sex appeal is expressed through charisma, silhouette, pose and costume styling rather than explicit nudity.
- [x] Chapter I environment/readability rules reflect the real game: coast, lanes, Troy gate/walls, Greek landing, visible but subordinate build points.
- [x] Trojan/Greek faction language, character silhouettes, tower-unit roles, animation, UI, VFX and acceptance rules are consolidated.
- [x] Old overlapping direction docs become compatibility pointers instead of competing authorities.
- [x] `docs/README.md` authority hierarchy points to `ART_BIBLE.md` as the single creative/visual authority.

## Automated validation
- [ ] No code validation required for documentation-only change.

## Manual validation
1. Read `ART_BIBLE.md` from top to bottom without consulting other creative-direction documents.
2. Verify an artist/AI can answer: camera/view, vibe, cartoon level, grotesque level, humor style, sex-appeal rule, faction palette, character silhouette, tower visual language, Chapter I environment, UI and production acceptance.
3. Verify old creative docs contain no conflicting duplicated rules.

## Known risks
The exact runtime camera remains code-owned and may change later. The Art Bible describes the current baseline and must be updated if a dedicated gameplay/camera task changes it.

## Result
Documentation consolidation only. No claim of Unity validation or production-art completion.

## Status
`IN_PROGRESS`
