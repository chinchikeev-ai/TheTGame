# TheTroyGame — Model & Art Production Inventory

Last reviewed: 2026-09-13

## Purpose

This document is the source of truth for whether a visual asset is actually production-ready.

Do not treat an editor generator, runtime fallback, procedural primitive, source pack, or documented target as a finished model.

## Status definitions

- `DONE` — final authored production asset is committed under `Assets/Game/Art/...`, runtime uses it without placeholder/procedural dependency, required animation/materials are connected, and the asset passed real Play Mode visual QA.
- `GENERATED PLACEHOLDER` — reproducible character/prefab candidate is generated from an approved third-party/source asset, but final Troy-specific production art is not complete.
- `PROCEDURAL` — reproducible candidate/presentation is assembled from Unity primitives, generated geometry/materials or code.
- `MISSING` — no dedicated candidate or authored asset exists yet.
- `SOURCE ONLY` — approved source material exists, but no Troy derivative exists yet.

## Production rule

A production-path location alone does not make an asset `DONE`.

Preferred final roots:

- `Assets/Game/Art/Characters/...`
- `Assets/Game/Art/Environment/...`
- `Assets/Game/Art/Structures/...`
- `Assets/Game/Art/Vehicles/...`
- `Assets/Game/Art/Props/...`

Generated resources may live below those roots for runtime loading, but remain candidates until the acceptance gate at the end of this file is satisfied.

---

# Implemented art pipeline

## Chapter I character candidates

Production-first candidate root:

`Assets/Game/Art/Characters/Resources/TroyProduction/Characters/...`

Implemented by PR #17 / merge `0fe3bcc2667b46746f3e5175131dc730cd806a43`.

Includes:

- Greek Infantry;
- Runner;
- Heavy Hoplite;
- Shield Bearer;
- Archer;
- boss/commander base;
- Trojan Infantry;
- Trojan Guard;
- Trojan Archer;
- Hector;
- Menelaus;
- Achilles later-campaign base.

The pass also replaced the Chapter I archer crossbow silhouette with a bow, gave Heavy Hoplite spear + heavy shield, strengthened Late Bronze Age armor/helmet silhouettes and added hero capes/crests.

## Character animation candidates

PR #21 / merge `c65cdad0c13979bc8b49fde7c10623b55d9a20ee` adds `ChapterOneCharacterAnimationBuilder`.

It builds a shared controller from KayKit's embedded animation clips and exposes the parameters already consumed by runtime presentation:

- `Speed`;
- `Attack`;
- `Hit`;
- `Die`;
- `IsDowned` when a suitable clip is found.

Exact automatically selected clips still require visual QA, so animation remains candidate-level.

## Chapter I Tower-Unit presentation

Commits:

- `f6d9a696cfd4a1698f725175234deacb4ac9979e`
- `b8689eea95780a613b1d5dbefa1730f8874ca442`

Implemented:

- non-mechanical defenses no longer show generic barrel silhouettes;
- Archer Post uses two Trojan Archer candidates;
- Ballista uses Trojan operator candidate;
- Spear Wall uses Trojan infantry candidates;
- Guard Post uses Trojan Guard candidates;
- Priests of Apollo and Fire Tower have readable attendant silhouettes.

## Chapter I landing / ships

PR #19 / merge `a48cf743378b5e798813117789050e2d604a86a3`.

Implemented:

- production-first decorative Greek character loading;
- decorative colliders/physics disabled;
- expanded Achaean galley silhouette with hull/deck/gunwales/keel/prow/mast/sail/oars/benches/shields;
- landing debris including crates, shields, oars and standard.

## Troy gate / skyline

PR #20 / merge `a70d58da85b855ed75a4195f18d635f1a35033d4`.

Implemented:

- layered gatehouse;
- flanking wall wings;
- parapets, buttresses, masonry, wall walk, crenellations, arrow slits;
- twin hero towers;
- gate relief and braziers;
- stepped city houses;
- citadel terraces;
- temple focus;
- inner-city skyline.

## Art wiring regression protection

PR #22 / merge `0c1fc2ed6444bd592f91c73b4ccb2384e20876b4` extends the Chapter I RC validator to protect:

- production-first hero/enemy art paths;
- landing production-character use;
- decorative collider safety;
- animation-builder wiring;
- legacy primitive/capsule regressions.

## Campaign candidate bank

`CampaignArtCandidateBuilder` creates model candidates for Chapters II–VII.

See `docs/CAMPAIGN_ART_PIPELINE.md`.

It does **not** implement later-chapter gameplay and does **not** promote assets to `DONE`.

---

# Chapter I — required production set

## Heroes and combat characters

| Asset | Status | Current implementation | Remaining to DONE |
|---|---|---|---|
| Hector | GENERATED PLACEHOLDER | production-first KayKit-derived hero candidate, Bronze Age kit, shared Animator candidate | Final authored hero mesh/materials, hero-specific ability/combat clips, Play Mode QA |
| Menelaus | GENERATED PLACEHOLDER | commander candidate with command sword, shield, armor, cape, shared Animator candidate | Final boss mesh/materials, boss-specific animation set, Play Mode QA |
| Greek Infantry | GENERATED PLACEHOLDER | production candidate | Final Achaean mesh/material/animation pass |
| Greek Runner | GENERATED PLACEHOLDER | production candidate | Final runner/scout identity and animation pass |
| Heavy Hoplite | GENERATED PLACEHOLDER | heavy cuirass/helmet, spear + heavy shield candidate | Final heavy authored model/animations |
| Shield Bearer | GENERATED PLACEHOLDER | spear + oversized shield candidate | Final authored shield-bearer model/animations |
| Greek Archer | GENERATED PLACEHOLDER | bow + quiver candidate | Final bow model + draw/release animation |
| Trojan Infantry | GENERATED PLACEHOLDER | warm bronze/red candidate | Final authored Trojan infantry model |
| Trojan Guard | GENERATED PLACEHOLDER | guard candidate + gameplay blocker | Final formation/block/melee animations and mesh |
| Trojan Archer | GENERATED PLACEHOLDER | bow + quiver candidate; used as Archer Post crew | Final authored archer + bow animation |
| Greek landing party | GENERATED PLACEHOLDER | production-first candidates, decorative colliders/physics removed | Final decorative variants + authored landing/run animation |

**Chapter I character DONE count: 0.**

## Defensive structures / Tower-Units

| Asset | Status | Current implementation | Remaining to DONE |
|---|---|---|---|
| Archer Post | PROCEDURAL + GENERATED PLACEHOLDER crew | wood/bronze post + 2 Trojan Archer candidates | Authored structure + final crew/animations |
| Ballista | PROCEDURAL + GENERATED PLACEHOLDER crew | readable ballista form + operator candidate | Authored mechanism, string/bolt, reload/fire animation, crew |
| Priests of Apollo | PROCEDURAL | shrine + two priest silhouettes | Authored shrine + final priests + support animation/VFX anchors |
| Spear Wall | PROCEDURAL + GENERATED PLACEHOLDER crew | spear/shield defense + 2 infantry candidates | Authored post/formation + poke/block animation |
| Fire Tower | PROCEDURAL | brazier/pitch/flame + fire-keeper silhouette | Authored fire defense + final props/keeper if retained |
| Trojan Guard Post | PROCEDURAL + GENERATED PLACEHOLDER crew | platform/banners + 2 Guard candidates | Authored station + final Guard squad |

**Chapter I defense DONE count: 0.**

## Environment and staging

| Asset | Status | Current implementation | Remaining to DONE |
|---|---|---|---|
| Aegean sea | PROCEDURAL | runtime sea/foam presentation | Authored water material/mesh treatment |
| Shoreline / wet sand | PROCEDURAL | runtime coastline | Authored terrain/mesh/material set |
| Beach / dunes / rocks / scrub | PROCEDURAL | runtime dressing | Modular authored coast kit |
| Invasion lanes | PROCEDURAL | gameplay paths with presentation treatment | Authored terrain blending while preserving gameplay paths |
| Greek ships | PROCEDURAL | expanded galley candidate | Authored ship model kit with variants/LOD |
| Landing debris | PROCEDURAL | crates/shields/oars/standard | Authored beachhead prop kit |
| Greek camp | PROCEDURAL | campfires/standards/minimal props | Authored tent/supply/weapon-rack kit |
| Troy wall | PROCEDURAL | fortified wall-wing presentation | Authored modular wall kit |
| Troy gatehouse | PROCEDURAL | layered gate complex | Authored gatehouse prefab/mesh |
| Troy towers | PROCEDURAL | enhanced hero towers | Authored modular tower set |
| Troy skyline | PROCEDURAL | houses/citadel/temple/inner wall | Authored low-cost city kit |
| Braziers | PROCEDURAL | readable flame focal props | Authored brazier + final VFX anchor |
| Banners | PROCEDURAL | runtime cloth blocks/poles | Authored cloth/banner variants |
| Vegetation / rocks | PROCEDURAL / generic | runtime dressing | Authored environment prop kit |

**Chapter I environment DONE count: 0.**

---

# Full campaign model bank

## Greek / Achaean regular units

| Asset | First use | Status | Candidate / requirement |
|---|---:|---|---|
| Infantry | I | GENERATED PLACEHOLDER | Chapter I candidate exists |
| Archer | I | GENERATED PLACEHOLDER | bow candidate exists |
| Spearman | I/II | GENERATED PLACEHOLDER | `Enemy_Spearman` explicit campaign candidate |
| Light Swordsman | II | GENERATED PLACEHOLDER | `Enemy_LightSwordsman` candidate |
| Shield Bearer | I | GENERATED PLACEHOLDER | Chapter I candidate |
| Hoplite | II | GENERATED PLACEHOLDER | `Enemy_Hoplite` candidate |
| Heavy Hoplite | I | GENERATED PLACEHOLDER | Chapter I heavy candidate |
| Scout | II | GENERATED PLACEHOLDER | `Enemy_Scout` candidate |
| Runner | I/II | GENERATED PLACEHOLDER | Chapter I candidate |
| Chariot | II | PROCEDURAL | `Vehicle_Chariot` candidate with two horse silhouettes, wheels/yoke and Greek crew |
| Ram Crew | III | GENERATED PLACEHOLDER | `Enemy_RamCrew` candidate |
| Battering Ram | III | PROCEDURAL | `Siege_BatteringRam` candidate with frame, suspended beam, bronze head, wheels, hide roof, crew |
| Siege Tower | III | PROCEDURAL | `Siege_SiegeTower` candidate with wheels, ladder, hide front, fighting platform |
| Sapper | III/V | GENERATED PLACEHOLDER | `Enemy_Sapper` candidate with satchel/tool silhouette |
| Myrmidon | IV | GENERATED PLACEHOLDER | `Enemy_Myrmidon` black/bronze elite candidate |
| Myrmidon Veteran | IV/V | GENERATED PLACEHOLDER | `Enemy_MyrmidonVeteran` candidate |
| Greek Captain | IV/V | GENERATED PLACEHOLDER | `Enemy_GreekCaptain` commander candidate |
| Hero Companion | IV/V | GENERATED PLACEHOLDER | `Enemy_HeroCompanion` elite candidate |

## Trojan units

| Asset | Status | Candidate / requirement |
|---|---|---|
| Trojan Infantry | GENERATED PLACEHOLDER | Chapter I candidate |
| Trojan Guard | GENERATED PLACEHOLDER | Chapter I candidate |
| Trojan Archer | GENERATED PLACEHOLDER | Chapter I candidate |
| Spear Wall soldiers | GENERATED PLACEHOLDER / PROCEDURAL | crew candidates exist; final formation animation missing |
| Ballista crew | GENERATED PLACEHOLDER | Trojan operator candidate exists |
| Priests of Apollo | PROCEDURAL | readable attendants exist; final character mesh absent |
| Fire Tower crew | PROCEDURAL | Fire Keeper silhouette exists |
| Civilians | GENERATED PLACEHOLDER | `Trojan_Civilian` candidate for Chapter VII evacuation |

## Named heroes / bosses

| Character | Main chapter | Status | Candidate / requirement |
|---|---:|---|---|
| Hector | I–VII | GENERATED PLACEHOLDER | Chapter I hero candidate |
| Menelaus | I | GENERATED PLACEHOLDER | Chapter I boss candidate |
| Achilles | IV/V | GENERATED PLACEHOLDER | hero candidate generated from base pipeline |
| Ajax | IV | GENERATED PLACEHOLDER | `Hero_Ajax` candidate with oversized shield/hero silhouette |
| Odysseus | VII | GENERATED PLACEHOLDER | `Hero_Odysseus` cloak/satchel candidate |

## Mythic / narrative assets

| Asset | Chapter | Status | Candidate / requirement |
|---|---:|---|---|
| Cyclops | Later | MISSING | No dedicated candidate yet |
| Trojan Horse | VI/VII | PROCEDURAL | `Prop_TrojanHorse` large wooden-horse candidate with hatch |

---

# Campaign environment / structure inventory

| Asset family | First chapter | Status | Current state / requirement |
|---|---:|---|---|
| Coastal terrain kit | I | PROCEDURAL | staged; final authored coast kit missing |
| Greek ship kit | I | PROCEDURAL | galley candidate exists; authored variants missing |
| Greek beach camp | I | PROCEDURAL | partial staging; authored camp kit missing |
| Trojan wall/gate kit | I/III | PROCEDURAL | strong candidate staging; authored modular kit missing |
| Plains/road terrain kit | II | MISSING | still needs dedicated Chapter II environment candidate |
| Chariot vehicle | II | PROCEDURAL | `Vehicle_Chariot` candidate exists |
| Siege prop kit | III | PROCEDURAL | ram + siege tower candidates exist; final models/destruction states missing |
| Damaged outer defenses | IV | MISSING | no dedicated environment kit yet |
| Great Assault destruction states | V | MISSING | no authored/candidate destruction kit yet |
| Greek abandoned camp / Horse scene | VI | PROCEDURAL / MISSING | Horse candidate exists; abandoned-camp environment still missing |
| Troy interior | VII | MISSING | modular interior city kit still missing |
| Burning Troy states | VII | MISSING | burning/collapse/blocked-street variants still missing |

---

# Animation inventory

| Animation set | Status | Remaining |
|---|---|---|
| Generic Greek locomotion | GENERATED PLACEHOLDER | shared KayKit controller candidate; final clip selection/QA |
| Greek melee attack | GENERATED PLACEHOLDER | candidate trigger/controller exists; final weapon-specific attack clips |
| Greek ranged attack | GENERATED PLACEHOLDER | candidate trigger/controller exists; final bow draw/release |
| Greek hit/death | GENERATED PLACEHOLDER | candidate hit/death states exist; visual QA/final clips |
| Hector locomotion | GENERATED PLACEHOLDER | shared controller candidate; hero-specific final pass |
| Hector Q/E/R/F | PROCEDURAL VFX + gameplay | authored body animations still needed |
| Hector down/revive | GENERATED PLACEHOLDER | candidate downed state; final hero clips needed |
| Menelaus combat/death | GENERATED PLACEHOLDER | shared candidate controller; boss-specific final clips needed |
| Trojan Guard block/melee | GENERATED PLACEHOLDER / hook | final formation/block attack clips needed |
| Ballista mechanism | MISSING | reload/tension/fire/recoil animation |
| Battering Ram cycle | MISSING | push/impact/recover animation |
| Siege Tower movement | MISSING | wheel/movement/assault animation |
| Chariot movement/combat | MISSING | horse/chariot animation solution |

---

# Production priorities

## P0 — Chapter I freeze

Candidate/presentation passes are implemented for:

1. Hector;
2. Menelaus;
3. Greek Chapter I regular roster;
4. Trojan Guard / Archer / Infantry;
5. all six Tower-Unit presentations;
6. Greek landing/galley presentation;
7. Troy wall/gate/skyline presentation;
8. production-first decorative landing units;
9. shared character animation-controller candidate.

Still required before Chapter I art can be called finished:

1. final authored character meshes/materials;
2. final authored Tower-Unit models;
3. authored coast/ship/Troy environment kit;
4. hero/boss/tower-specific final animation pass;
5. 16:9 real Play Mode visual QA, including RU UI;
6. collider/material/shader validation in the actual Unity build.

## P1 — Chapter II/III

Candidate bank now exists for Spearman, Scout, Light Swordsman, Hoplite, Chariot, Ram Crew, Battering Ram, Siege Tower and Sapper.

Still missing at model/environment level:

- final authored versions of those candidates;
- Chapter II plains/roads environment;
- expanded Chapter III wall damage/breach states;
- mechanism/vehicle animations.

## P2 — Chapter IV/V

Candidate bank now exists for:

- Achilles;
- Ajax;
- Myrmidon;
- Myrmidon Veteran;
- Greek Captain;
- Hero Companion.

Still missing:

- final authored hero/elite models;
- outer-defense environment kit;
- large-siege/destruction states;
- encounter-specific animation/cinematics.

## P3 — Chapter VI/VII

Candidate bank now exists for:

- Trojan Horse;
- Odysseus;
- Trojan Civilian.

Still missing:

- final Trojan Horse hero prop;
- final Odysseus model/animations;
- civilian variants/evacuation animations;
- abandoned Greek camp environment;
- Troy interior modular kit;
- burning/collapse states.

---

# Acceptance gate for `DONE`

An asset can move to `DONE` only when all applicable checks pass:

1. Final mesh/prefab exists in `Assets/Game/Art/...`.
2. Unity `.meta` files are tracked.
3. Third-party provenance/license is documented when applicable.
4. Runtime references the final production prefab instead of relying on placeholder/procedural fallback.
5. Collider setup is intentional; decorative variants cannot affect gameplay physics.
6. Materials/textures follow `ART_BIBLE.md` faction palette and render correctly.
7. Silhouette is readable from the gameplay camera.
8. Required animation is connected to runtime presentation hooks.
9. No missing-material/pink-shader state exists in the target render pipeline.
10. Asset has passed real Play Mode QA at target 16:9 layouts and relevant EN/RU framing.

## AI implementation rule

AI agents must use this inventory when reporting art/model completion and must never call an asset finished merely because:

- an editor builder can generate it;
- a KayKit source model exists;
- a runtime fallback renders something;
- a procedural candidate looks recognizable;
- the GDD/Art Bible specifies it.

Use the exact inventory status until a newer verified change updates this file.
