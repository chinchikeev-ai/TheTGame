# AI Task Contract

## Task ID
`ART-005-CURRENT-RUNTIME-VISUAL-GAP-PASS`

## Goal
Bring the actual Chapter I runtime view toward the canonical `ART_BIBLE.md` using the current gameplay camera and layout, without redesigning gameplay structure.

## Why
The real gameplay screenshot reviewed on 2026-09-14 still reads as a technical/procedural prototype: repeating build-point markers dominate the battlefield, unit/tower silhouettes are weak at tactical zoom, material language is flat, faction separation is insufficient, and HUD/world styling are not yet fully unified. The art pipeline must now optimize what is visible in the real game rather than concept art or close-up assets.

## Owner module
Production art / runtime presentation, with implementation split across existing visual owners for map, towers, characters and UI.

## Do not change
- Chapter I camera angle, orthographic projection, default zoom or gameplay bounds
- pathfinding/routes
- tower placement rules
- enemy spawn logic
- combat balance
- gameplay colliders unless a separate gameplay task explicitly requires it

## Inputs / source of truth
- `docs/ART_BIBLE.md`
- current Chapter I runtime screenshot supplied by the project owner on 2026-09-14
- current runtime code and `MODEL_ART_INVENTORY.md`

## Current visual gaps from the real gameplay frame

### P0 — battlefield readability
1. **Build points are visually dominant.** Repeating flower/rosette markers create the strongest pattern on the map and compete with units, roads and landmarks.
2. **Playable units are too visually small/quiet.** Role differences are not strong enough at the default tactical camera.
3. **Tower-Units do not read in one second.** Archer, Spear Wall, Guard, Priest, Fire and Ballista need stronger primary silhouettes.
4. **Hector does not dominate the Trojan hierarchy strongly enough.** His silhouette must read as hero before detail.

### P1 — world identity
5. **Greek and Trojan sides are not separated strongly enough.** Troy needs warmer red/bronze/gold massing; Greeks need cooler blue/pale/bronze massing.
6. **Ground/material language remains prototype-like.** Too much flat/simple color; not enough deliberate sand/stone/wood/bronze/cloth hierarchy.
7. **Map composition is locally empty while repeated markers are busy.** Landmarks should carry the visual interest instead of repeated service graphics.
8. **Trojan War mood is underpowered.** Need more readable bronze, banners, heat, dust, siege/landing cues and heroic scale without increasing clutter.

### P2 — presentation coherence
9. **HUD and world still feel like separate visual systems.** UI should consistently use dark stone/charcoal + bronze/gold + Trojan red accents.
10. **VFX/animation personality is too weak to carry the canonical humor/grotesque style at gameplay distance.** Role-specific anticipation/recoil/reaction needs to become part of the read.
11. **Cartoon/grotesque personality is not strong enough in the runtime frame.** Important units need one intentionally pushed silhouette trait each.

## Implementation order

### Pass A — reduce background competition
- reduce unselected build-point scale/contrast/opacity/complexity;
- keep selected/hovered build points clear and bright;
- preserve interaction/colliders;
- simplify repeated ground noise;
- keep lanes readable as broad paths, not debug-grid decoration.

Implementation status: **implemented in runtime code; awaiting real Play Mode screenshot QA.**

### Pass B — make defense roles unmistakable
From the current gameplay camera, enforce these primary reads:
- Archer = large bow/quiver + light canopy/platform;
- Spear Wall = spear cluster + shields;
- Guard = oversized shield wall + broad bodies;
- Ballista = machine silhouette dominates crew;
- Priest = pale/gold shrine + tall solar staff/disc;
- Fire = large flame/brazier + bottle/fire-keeper silhouette.

Implementation status: **implemented in `TowerArtDirector`; awaiting real Play Mode screenshot QA.**

Current runtime cues added/strengthened:
- Archer: larger red canopy, giant bow silhouette, larger arrow bundle, slightly larger visible crew.
- Spear Wall: larger forward shields, five upright spears, longer forward spears, stronger red crest.
- Guard: larger shield wall, shield bosses, larger crew, stronger red/bronze mass.
- Ballista: wider bow, larger frame/rail/bolt and stronger machine-first read.
- Priest: larger pale/gold shrine, enlarged Apollo disc, sun rays and tall staff/disc.
- Fire: larger brazier/flame cluster, pitch rack/bottles and raised fire-bottle keeper cue.
- All additions remain presentation-only and use collider-free visual primitives.

### Pass C — hero/enemy hierarchy
- Hector = taller/broader, large round shield, long spear, crest, red cape and warm bronze/red mass;
- Menelaus = visibly larger Greek commander, cool-blue/bronze contrast, crest/cape/shield and aura;
- Greek regular archetypes separated by shield/armor/weapon mass before color.

Implementation status: **implemented in `HeroSignatureArt` and `WarriorArtDirector`; awaiting real Play Mode screenshot QA.**

Current runtime cues added/strengthened:
- Hector: larger red cape mass, broader bronze shoulders, stronger chest emblem, larger crest, fallback-safe round shield and long spear cues when source art does not already contain them.
- Menelaus: larger blue royal cape, gold chest/shoulders, larger crest, command aura and fallback-safe royal shield cue.
- Greek Infantry: explicit blue round shield + spear + blue crest.
- Runner: lighter shoulder mass, short blue crest, sash and light blade.
- Archer: large bow, quiver/arrow bundle, dark hood and blue scarf.
- Heavy Hoplite: heavier armor mass and large dark shield.
- Shield Bearer: deliberately oversized blue wall-shield and stronger plume.
- Boss: larger blue/gold command mass, cape, shield, crest and aura.
- Additions are presentation-only and do not intentionally alter combat balance or pathing.

### Pass D — materials and faction staging
- introduce stronger sand/stone/wood/bronze/cloth separation;
- warm Trojan gate/walls/banners/braziers;
- cooler Greek beachhead/camp/ships;
- add visual-only props only where they strengthen landmarks and do not affect gameplay collision.

### Pass E — UI/VFX/animation coherence
- unify combat UI with Art Bible palette/material language;
- keep HUD quiet and tactical;
- strengthen role-specific release/recoil/cast/brace/throw feedback;
- add humor/grotesque through animation and silhouette rather than extra visual noise.

## Acceptance criteria
- [ ] Unselected build points are visibly secondary to units/towers/roads.
- [ ] All six Chapter I defensive roles can be identified from the default gameplay camera without reading UI labels.
- [ ] Hector is immediately recognizable as the Trojan hero from gameplay zoom.
- [ ] Menelaus is immediately recognizable as the Greek boss when present.
- [ ] Trojan and Greek sides are distinguishable by both silhouette and palette.
- [ ] Ground/materials no longer read primarily as flat colored primitives.
- [ ] The frame reads as Trojan War + Cartoon + Grotesque Heroic Comedy before close inspection.
- [ ] UI belongs to the same stone/bronze/red visual world while remaining quieter than the battlefield.
- [ ] No gameplay paths, balance, camera or placement rules are changed.

## Validation
- Real Chapter I Play Mode screenshot at default camera/zoom after each pass.
- Additional screenshot at zoom min/max to confirm readability range.
- Compare side-by-side against the 2026-09-14 runtime screenshot.
- Production-art status remains governed by `MODEL_ART_INVENTORY.md`; passing this task does not automatically mean `DONE`.

## Status
`IN_PROGRESS — PASS A + PASS B + PASS C IMPLEMENTED, PLAY MODE QA PENDING`
