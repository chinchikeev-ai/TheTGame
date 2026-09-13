# TheTroyGame — Model & Art Production Inventory

Last reviewed: 2026-09-13

## Purpose

This document is the source of truth for whether a visual asset is actually production-ready.

Do not treat an editor generator, runtime fallback, procedural primitive, source pack, or documented target as a finished model.

## Status definitions

- `DONE` — authored production asset is committed under `Assets/Game/Art/...`, has its Unity `.meta`/prefab dependencies, is used by runtime without primitive/fallback dependency, and has passed visual QA in Play Mode.
- `GENERATED PLACEHOLDER` — usable prefab can be generated from an approved third-party source pack, but it is not final Troy-specific production art.
- `PROCEDURAL` — visual is assembled at runtime/editor time from Unity primitives, generated geometry, generic materials, or presentation code.
- `MISSING` — no dedicated model/prefab exists yet.
- `SOURCE ONLY` — an approved/source asset pack exists, but a Troy production derivative has not been authored and committed.

## Production rule

A model is not `DONE` until the final derivative lives in the project under a production path such as:

`Assets/Game/Art/Characters/...`

`Assets/Game/Art/Environment/...`

`Assets/Game/Art/Structures/...`

`Assets/Game/Art/Vehicles/...`

`Assets/Game/Art/Props/...`

Generated assets under `Assets/Resources/TroyCharacters` are considered build outputs/placeholders unless they are explicitly promoted into the production art tree.

---

# Chapter I — required production set

## Heroes and combat characters

| Asset | Current status | Current source/runtime path | Chapter I requirement | Next action |
|---|---|---|---|---|
| Hector | GENERATED PLACEHOLDER | KayKit -> `Hero_Hector`; runtime hero factory | Required | Author Troy-specific Hector model, armor, shield, spear, authored locomotion/attack/hit/down/death clips |
| Menelaus | GENERATED PLACEHOLDER | KayKit -> `Hero_Menelaus`; boss runtime | Required | Author commander-grade Menelaus silhouette, crest, armor, shield/sword, boss animation set |
| Greek Infantry | GENERATED PLACEHOLDER | KayKit Greek generator | Required | Produce Late Bronze Age Achaean infantry variant |
| Greek Runner/Scout | GENERATED PLACEHOLDER | KayKit Greek generator | Required where used | Produce lighter runner silhouette and animation variant |
| Heavy Hoplite | GENERATED PLACEHOLDER | KayKit Greek generator | Required | Produce heavy shield/armor silhouette distinct at gameplay camera distance |
| Shield Bearer | GENERATED PLACEHOLDER | KayKit Greek generator | Required | Produce dedicated oversized-shield variant |
| Greek Archer | GENERATED PLACEHOLDER | KayKit Greek generator | Required | Replace generic ranged/crossbow presentation with period-appropriate bow set |
| Trojan Infantry | GENERATED PLACEHOLDER | KayKit Trojan reference generator | Presentation/support | Produce Trojan warm-bronze/red infantry variant |
| Trojan Guard | GENERATED PLACEHOLDER | KayKit Trojan reference generator + runtime guard system | Required | Produce shield-wall/guard squad production model and animation set |
| Trojan Archer | GENERATED PLACEHOLDER | KayKit Trojan reference generator | Required for Archer defense presentation | Produce production archer/crew model |
| Greek landing party | GENERATED PLACEHOLDER | Runtime warrior factory | Required presentation | Reuse production Greek set with non-gameplay decorative prefab variants and no gameplay colliders |

### Chapter I character conclusion

The gameplay roster is visually represented, but **zero Chapter I combat characters currently qualify as `DONE` under the production rule above**. The existing KayKit pipeline is the functional placeholder layer.

## Chapter I defensive structures / Tower-Units

| Asset | Current status | Evidence/current implementation | Next action |
|---|---|---|---|
| Archer Tower / Archer Post | PROCEDURAL | `TowerArtDirector` builds platform/posts/canopy/arrow bundle from Unity primitives | Model wooden platform + Trojan archer crew + bow/arrow props |
| Ballista | PROCEDURAL | Runtime primitive deck/support/rail/bow | Dedicated ballista model with moving arms/string/bolt and crew |
| Priests of Apollo shrine | PROCEDURAL | Runtime cylinders/disc/columns | Authored shrine + priest unit(s) + readable support VFX anchor |
| Spear Wall | PROCEDURAL | Runtime spear rack/crest presentation; gameplay uses short-range poke behavior | Authored shield/spear defensive post with Trojan soldiers |
| Fire Tower | PROCEDURAL | Runtime primitive stone bowl/flame accents | Authored brazier/fire-pot defense with fuel/pitch props |
| Trojan Guard post | PROCEDURAL + GENERATED PLACEHOLDER units | Runtime primitive platform/banners/rack + generated soldiers | Authored guard station plus production Guard squad |

No Chapter I defense model is currently `DONE`.

## Chapter I environment and staging

| Asset | Current status | Current implementation | Next action |
|---|---|---|---|
| Aegean sea/water | PROCEDURAL | Runtime presentation/atmosphere layers | Authored water material/mesh treatment suitable for target camera |
| Shoreline / wet sand | PROCEDURAL | Runtime world presentation | Authored coastline terrain/mesh/material set |
| Beach / dunes / rocks / scrub | PROCEDURAL | Runtime dressing | Production modular beach kit |
| Invasion lanes / worn tracks | PROCEDURAL | Runtime map/presentation geometry | Replace remaining grid-like presentation with authored terrain blending |
| Greek ships | PROCEDURAL | `LandingPresentation` builds hull/prow/mast/sail from primitives | Dedicated Late Bronze Age/Achaean landing ship model, at least 2–3 variants/LOD silhouettes |
| Greek landing debris | PROCEDURAL / MISSING | Presentation props are generic/minimal | Add oars, shields, amphorae, ropes, crates, broken gear |
| Greek beach camp | PROCEDURAL | Campfires/standards/presentation objects | Authored tents, standards, supply props, weapon racks |
| Troy wall silhouette | PROCEDURAL | Runtime wall/city presentation | Authored modular Troy wall set |
| Troy gatehouse | PROCEDURAL | Runtime hero gate builder | Authored gatehouse with readable gate, towers, battlements |
| Troy wall towers | PROCEDURAL | Runtime geometry | Modular tower production asset |
| Troy skyline / city backdrop | PROCEDURAL | Runtime silhouette | Authored low-cost skyline/building kit |
| Trojan braziers | PROCEDURAL | Runtime effects/primitive presentation | Authored brazier prop + flame VFX anchor |
| Banners / standards | PROCEDURAL | Runtime primitive poles/cloth blocks | Authored cloth/banner meshes with faction variants |
| Rocks / vegetation | PROCEDURAL / generic | Runtime dressing | Production environment prop kit |

### Chapter I environment conclusion

The visual target is implemented as a staged procedural presentation, but the chapter still does **not** have a production environment kit. This is the largest remaining art gap after character placeholders.

---

# Full campaign character inventory

## Greek / Achaean regular units

| Asset | First major use | Status | Notes |
|---|---:|---|---|
| Infantry | I | GENERATED PLACEHOLDER | KayKit generator |
| Archer | I | GENERATED PLACEHOLDER | Needs period bow presentation |
| Spearman | I/II | GENERATED PLACEHOLDER by infantry/shield variants | Needs explicit dedicated production variant |
| Light Swordsman | II | MISSING | No dedicated production model |
| Shield Bearer | I | GENERATED PLACEHOLDER | KayKit generator |
| Hoplite | II | GENERATED PLACEHOLDER by heavy variant | Needs dedicated production variant |
| Heavy Hoplite | I | GENERATED PLACEHOLDER | KayKit generator |
| Scout | II | GENERATED PLACEHOLDER by Runner | Needs explicit production identity |
| Runner | I/II | GENERATED PLACEHOLDER | KayKit generator |
| Chariot | II | MISSING | Vehicle + horses + rider required |
| Ram Crew | III | MISSING | Crew identity required separately from ram |
| Battering Ram | III | MISSING | Generator explicitly skips this archetype |
| Siege Tower | III | MISSING | Large siege object + ladders/crew presentation |
| Sapper | III/V | MISSING | Dedicated light specialist silhouette |
| Myrmidon | IV | MISSING | Elite Achilles faction identity |
| Myrmidon Veteran | IV/V | MISSING | Elite variant |
| Greek Captain | IV/V | MISSING | Commander silhouette below hero tier |
| Hero Companion | IV/V | MISSING | Elite hero-support unit |

## Trojan units

| Asset | Status | Notes |
|---|---|---|
| Trojan Infantry | GENERATED PLACEHOLDER | Reference generator |
| Trojan Guard | GENERATED PLACEHOLDER | Reference generator + gameplay blocker |
| Trojan Archer | GENERATED PLACEHOLDER | Reference generator |
| Spear Wall soldiers | GENERATED PLACEHOLDER / PROCEDURAL | Needs dedicated formation set |
| Ballista crew | MISSING | No production crew prefab |
| Priests of Apollo | MISSING | Shrine exists procedurally; priest character model not production-authored |
| Fire Tower crew | MISSING | Optional but useful for Tower-Unit identity |
| Civilians | MISSING | Required for Chapter VII evacuation |

## Heroes / named characters

| Character | Main chapter | Status | Production requirement |
|---|---:|---|---|
| Hector | I–VII | GENERATED PLACEHOLDER | Highest priority hero asset |
| Menelaus | I | GENERATED PLACEHOLDER | Chapter I boss production asset |
| Achilles | IV/V | GENERATED PLACEHOLDER | Generator slot exists; encounter production art pending |
| Ajax | IV | MISSING | Boss model + shield stance set |
| Odysseus | VII | MISSING | Boss model + disruption/deception presentation |

## Mythic / special units

| Asset | Chapter | Status | Notes |
|---|---:|---|---|
| Cyclops | Later campaign | MISSING | Planned mythic artillery/tower-unit silhouette |
| Trojan Horse | VI/VII | MISSING | Hero environment/narrative asset; must support exterior and breach staging |

---

# Full campaign environment / structure inventory

| Asset family | First chapter | Status | Requirement |
|---|---:|---|---|
| Coastal terrain kit | I | PROCEDURAL | Sand, wet shoreline, dunes, rocks, scrub |
| Greek ship kit | I | PROCEDURAL | Landing ships + variations |
| Greek beach camp kit | I | PROCEDURAL | Tents, crates, standards, supplies |
| Trojan wall/gate kit | I/III | PROCEDURAL | Walls, towers, gatehouse, battlements |
| Plains/road terrain kit | II | MISSING | Multi-route landscape, grass, dirt, rocks |
| Chariot props/vehicle set | II | MISSING | Vehicle art and destruction feedback |
| Siege prop kit | III | MISSING | Ram, siege tower, ladders, debris |
| Damaged outer defenses kit | IV | MISSING | Broken walls, rubble, burned props |
| Great Assault destruction states | V | MISSING | Disabled wall segments, temporary routes, destroyed build positions |
| Greek abandoned camp / Horse scene | VI | MISSING | Camp abandonment, Horse hero asset, scouting props |
| Troy interior streets/buildings | VII | MISSING | Interior modular city kit |
| Burning Troy destruction states | VII | MISSING | Burning buildings, blocked streets, collapse variants, evacuation dressing |

---

# Animation production inventory

Current code exposes presentation hooks for locomotion, attack, hit, death/downed and hero states, but authored production clips are not the same thing as those hooks.

| Animation set | Status | Required for production |
|---|---|---|
| Generic Greek locomotion | GENERATED PLACEHOLDER / runtime motion | Authored/retargeted idle/walk/run |
| Greek melee attack | GENERATED PLACEHOLDER / hook | Authored spear/sword attacks |
| Greek ranged attack | GENERATED PLACEHOLDER / hook | Authored bow release |
| Greek hit/death | GENERATED PLACEHOLDER / hook | Authored readable hit/death clips |
| Hector locomotion | GENERATED PLACEHOLDER / hook | Authored hero locomotion |
| Hector Q/E/R/F | PROCEDURAL VFX + gameplay | Authored ability body animations |
| Hector down/revive | GENERATED PLACEHOLDER / hook | Authored down/revive clips |
| Menelaus locomotion/combat/death | GENERATED PLACEHOLDER / hook | Boss-grade authored clips |
| Trojan Guard block/melee | GENERATED PLACEHOLDER / hook | Formation/block/attack clips |
| Ballista mechanism | MISSING | Reload/tension/fire/recoil animation |
| Battering Ram cycle | MISSING | Push/impact/recover loop |
| Siege Tower movement | MISSING | Wheel/motion/assault states |
| Chariot movement/combat | MISSING | Horse/chariot animation solution |

---

# Production priorities

## P0 — freeze Chapter I visual baseline

1. Hector production model + animation set.
2. Menelaus production model + boss animation set.
3. Greek Chapter I regular set: Infantry, Heavy Hoplite, Shield Bearer, Archer, Runner.
4. Trojan Guard + Trojan Archer production variants.
5. Chapter I Tower-Units: Archer, Ballista, Priests, Spear Wall, Fire Tower, Guard post.
6. Greek landing ship model kit.
7. Chapter I coast kit: shoreline, beach, rocks/scrub, tracks, camp props.
8. Troy wall/gate kit.
9. Replace decorative runtime warriors with non-gameplay production decorative prefabs.
10. 16:9/RU Play Mode visual QA after production replacements.

## P1 — Chapter II/III production

1. Plains/road environment kit.
2. Chariot.
3. Dedicated Hoplite/Spearman/Swordsman variants.
4. Battering Ram + Ram Crew.
5. Siege Tower.
6. Sapper.
7. Expanded Troy wall/gate damage states.

## P2 — Chapter IV/V

1. Achilles production hero.
2. Ajax.
3. Myrmidon + Veteran.
4. Greek Captain / Hero Companion.
5. Outer-defense and large-siege destruction kits.

## P3 — Chapter VI/VII

1. Trojan Horse.
2. Odysseus.
3. Troy interior modular kit.
4. Civilians.
5. Burning/destruction variants.
6. Evacuation props and final cinematic assets.

---

# Acceptance gate for marking an asset DONE

An asset can move to `DONE` only when all applicable checks pass:

1. Final mesh/prefab exists in `Assets/Game/Art/...`.
2. `.meta` files are tracked.
3. Source/provenance/license is documented when derived from third-party material.
4. Runtime references the production prefab instead of primitive/fallback generation.
5. Collider setup is intentional; decorative versions do not keep gameplay colliders.
6. Materials/textures match `ART_BIBLE.md` faction palette and visual identity.
7. Silhouette is readable from the gameplay camera.
8. Required animation set is connected to the existing presentation hooks.
9. No missing-material/pink-shader state in target render pipeline.
10. Asset has been checked in real Play Mode at 16:9 and against both EN/RU UI framing where relevant.

## AI implementation rule

When reporting project status, AI agents must use this inventory and must not describe a model as finished solely because:

- an editor builder can generate it;
- a KayKit source model exists;
- a runtime fallback renders something;
- the GDD/Art Bible specifies it;
- procedural presentation code creates a recognizable silhouette.

Use the exact inventory status unless a newer verified commit promotes the asset and updates this document.
