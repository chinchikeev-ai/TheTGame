# AI Task Contract

## Task ID
`ART-007-CHAPTER-I-GOLDEN-REFERENCE-ALIGNMENT`

## Goal
Bring the real Chapter I battlefield and combat HUD into alignment with the newly approved Art Bible v2.0 and Golden Reference direction without changing gameplay behavior, encounter composition, economy, save data or campaign progression.

## Why
The visual direction is now locked more precisely than before. The current runtime still contains a measurable gap between implementation and the approved target, especially in battlefield spaciousness, environment noise, HUD footprint and the relationship between Hector, Magic and Defenders controls.

This task is the first production alignment pass after `ART_BIBLE.md` v2.0.

## Source of truth

- `docs/ART_BIBLE.md`
- `docs/GOLDEN_REFERENCES.md`
- current real Chapter I camera/runtime behavior
- `docs/tasks/UX-005-UNIFIED-COMBAT-HUD.md`
- `docs/MODEL_ART_INVENTORY.md` for completion/status only

Reference priority:

1. Art Bible rules;
2. repository-tracked Golden References when present;
3. current real gameplay camera and runtime constraints;
4. task-specific implementation choices.

Generated concept art must never override gameplay readability or runtime contracts.

## Golden Reference targets

Primary targets:

- `GR-002-gameplay-1920x1080.png`
- `GR-003-gameplay-1366x768.png`
- `GR-006-hector-hud.png`
- `GR-010-patron-god-zeus.png`

Supporting targets:

- `GR-007-trojan-spearman.png`
- `GR-008-greek-infantry.png`
- `GR-009-ballista-tower-unit.png`

See `docs/GOLDEN_REFERENCES.md` for what each image governs and what must not be copied literally.

## Scope

### P0 — Battlefield composition and breathing room

- Preserve the current real Chapter I camera contract.
- Make the battlefield read as a complete tactical board rather than a packed diorama.
- Keep the visual sequence clear: Aegean sea -> landing beach -> Greek expedition zone -> two attack lanes -> Trojan defense zone -> wall/gate/city.
- Increase perceived tactical breathing room by reducing unnecessary decorative density and visual collisions.
- Do not fill open ground merely because it appears empty.
- Keep two attack lanes readable at a glance.
- Reduce repeating/noisy patterns behind units and placement decisions.
- Inactive build points remain clear but visually secondary.
- Decorative props must not interfere with navigation, placement or existing gameplay colliders.

### P0 — Combat HUD footprint and hierarchy

- Battlefield remains the dominant visual area.
- Permanent HUD should normally stay within the composition budget defined by Art Bible v2.0.
- Keep top-left `Gold -> Speed -> Settings` as one utility row.
- Keep Gate health directly below and visually heavier than utility controls.
- Keep encounter/progress/timer in top-center.
- Keep contextual advice temporary and below the gate/resources cluster.
- Keep bottom-center defender deployment strip centered and collapsible where appropriate.

### P0 — Hector block

- Keep Hector bottom-left.
- Reduce its footprint toward the Art Bible target: roughly `<=18%` screen width and `<=22%` screen height at 1920x1080 where practical.
- Preserve strong portrait-led readability.
- Show only immediate-decision information.
- Avoid decorative expansion and long flavor copy.
- Keep HP/downed/respawn and ability state unmistakable.

### P0 — Magic + Defenders

- Lower-right corner owns exactly two major action openers: `MAGIC` and `DEFENDERS`.
- They should be approximately equal in size and visual weight.
- They must be readable by icon before text.
- Magic uses divine/lightning/sun/fire language as appropriate to the active patron/system.
- Defenders uses shield/helmet/formation/fortress language.
- Ready/cooldown/locked state must be visible directly on the control.
- Speed and Settings must not appear in this corner.

### P1 — Environment noise reduction

- Reduce tertiary clutter that competes with units.
- Prefer broad sand/stone/road masses over noisy micro-detail.
- Keep rocks, shrubs, debris, barrels and broken boards sparse and subordinate.
- Keep fire as a selective accent, not a map-wide orange grade.
- Maintain brighter coast / sea-blue contrast against Trojan red and warm stone.

### P1 — Faction readability

- Troy remains warm, broad, fortified and theatrical.
- Greeks remain cooler, more expeditionary and slightly sharper.
- Common units must read by silhouette/role prop before facial detail.
- Do not replace historical vocabulary with universal Spartan or medieval shorthand.

### P1 — Patron God corner

- Patron art remains corner-anchored.
- God looks diagonally inward toward the battlefield.
- Use three-quarter personality portrait rather than passport-style frontal portrait.
- Divine state remains compact and does not wash out the center.
- Avoid duplicating full Divine Power descriptions in multiple permanent locations.

## Explicit non-goals

Do not change:

- combat rules;
- encounter composition;
- enemy counts;
- spawn timing;
- economy/balance;
- tower stats;
- Hector gameplay behavior;
- Patron selection behavior;
- save schema;
- campaign progression;
- Chapter I victory/defeat logic;
- current gameplay camera solely to match a concept image.

Do not claim final production-art acceptance merely because the runtime looks closer to the references.

## Visual Gap Audit required before implementation

Before changing art/layout, record the current gap using this structure:

| Area | Current Runtime | Golden Target | Gap | Priority | Owner |
| --- | --- | --- | --- | --- | --- |
| Battlefield scale/read | current screenshot | `GR-002` | describe measurable visual gap | P0/P1 | module/file |
| Hector HUD | current runtime | `GR-006` | size/hierarchy gap | P0 | UI |
| Magic/Defenders | current runtime | `GR-002/003` | placement/scale/state gap | P0 | UI |
| Ground/detail density | current runtime | `GR-002` | noise/readability gap | P1 | Environment |
| Patron corner | current runtime | `GR-010` | angle/size/readiness gap | P1 | UI |
| Trojan/Greek silhouettes | current runtime | `GR-007/008` | faction/role gap | P1 | Character art |
| Tower-Unit read | current runtime | `GR-009` | anonymous turret/crew gap | P1 | Tower art |

The audit must distinguish real runtime defects from differences that are intentionally constrained by gameplay or implementation.

## Acceptance criteria

### Battlefield

- [ ] Battlefield is the largest visual area at 1920x1080 and 1366/1376x768.
- [ ] Sea, landing zone, two attack lanes and Trojan gate/city are readable in one glance.
- [ ] Open tactical ground remains visibly open.
- [ ] Tertiary decoration no longer competes with units/build positions.
- [ ] Build points remain interactive but secondary when idle.
- [ ] No decorative change alters gameplay navigation/colliders.

### HUD

- [ ] Gold + Speed + Settings form one top-left row.
- [ ] Gate is directly below and visually stronger than utility controls.
- [ ] Encounter/timer remains top-center.
- [ ] Hector remains compact bottom-left and stays near the Art Bible geometry target.
- [ ] Defender strip is centered and does not hide the central fight.
- [ ] Magic and Defenders are paired bottom-right with approximately equal visual weight.
- [ ] Patron God remains corner-anchored and looks inward.
- [ ] Context advice does not become a permanent wall of text.
- [ ] Important gameplay state is not represented by color alone.

### Responsive

- [ ] 1920x1080 RU passes without overlap/clipping.
- [ ] 1920x1080 EN passes without overlap/clipping.
- [ ] 1376x768 RU passes without overlap/clipping.
- [ ] 1376x768 EN passes without overlap/clipping.
- [ ] 1366x768 is checked when available.
- [ ] Critical labels remain readable without globally shrinking typography into illegibility.

### Style

- [ ] Result reads as stylized Trojan War cartoon rather than realistic RTS.
- [ ] Troy vs Greek faction contrast remains visible.
- [ ] Environment supports gameplay actors instead of out-noising them.
- [ ] UI materials remain dark warm stone/wood + bronze/gold + Trojan red + selective parchment.
- [ ] No new flat-software/debug-looking UI becomes visually dominant.

## Implementation rules

- Reuse existing authored HUD skin/components where practical.
- Prefer scalable/9-sliced reusable UI art over one-off baked panels.
- Runtime text remains TextMeshPro/localized text; do not introduce language-specific baked button text.
- Keep icons separate from reusable backgrounds where state/context changes.
- Preserve Unity `.meta` files for all committed assets.
- Do not add decorative colliders unless separately required by gameplay.

## Validation

Required automated checks after implementation where the environment supports them:

- `git diff --check`
- architecture/static validation
- relevant EditMode tests
- relevant PlayMode layout/interaction tests
- full project validation scripts

Required real visual QA:

- Chapter I preparation state;
- active encounter;
- between-encounter pause;
- defender selection/hover/placement;
- Hector selected/downed/revived;
- Magic ready/cooldown;
- Patron commentary;
- Menelaus active/low HP;
- Gate danger state;
- Pause/Settings;
- victory/defeat.

## Definition of done

This task is complete when:

1. the gap audit is recorded;
2. the runtime is materially closer to the Golden Reference composition without gameplay regressions;
3. the target RU/EN resolution matrix has been visually checked;
4. automated validation passes in a Unity-capable checkout;
5. documentation/status is updated;
6. no production-art `DONE` claim is made unless the independent art-freeze requirements are also satisfied.

## Status

`PLANNED — GOLDEN REFERENCE BINARIES + RUNTIME ALIGNMENT PENDING`
