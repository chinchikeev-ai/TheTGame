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

Chapter I character candidates are now generated under:

`Assets/Game/Art/Characters/Resources/TroyProduction/Characters/...`

They remain `GENERATED PLACEHOLDER` until final authored meshes/materials/animations are committed and pass the `DONE` acceptance gate below. A production-path location alone does not make an asset final.

## Implemented Chapter I candidate passes

- PR #17 / merge `0fe3bcc2667b46746f3e5175131dc730cd806a43`: production-first character resource path plus Late Bronze Age candidate kitbash for Hector, Menelaus, Greek core enemies and Trojan reference units.
- Tower presentation commits `f6d9a696cfd4a1698f725175234deacb4ac9979e` + `b8689eea95780a613b1d5dbefa1730f8874ca442`: non-mechanical Tower-Units no longer present generic barrels; Archer/Ballista/Spear/Guard defenses use Trojan production-candidate crews when available; Apollo/Fire defenses have dedicated readable attendants.
- PR #19 / merge `a48cf743378b5e798813117789050e2d604a86a3`: landing cinematic uses production-first Greek candidates, decorative-unit colliders/physics are disabled, Achaean galley silhouettes and landing debris are substantially expanded.
- PR #20 / merge `a70d58da85b855ed75a4195f18d635f1a35033d4`: Troy gate/fortification/citadel/temple skyline presentation is substantially deepened while gameplay geometry remains unchanged.

These are candidate/presentation passes. They do not promote any asset to `DONE` without final authored assets and real Play Mode QA.

---

# Chapter I — required production set

## Heroes and combat characters

| Asset | Current status | Current source/runtime path | Chapter I requirement | Next action |
|---|---|---|---|---|
| Hector | GENERATED PLACEHOLDER | KayKit-derived `TroyProduction/Characters/Heroes/Hero_Hector`; production-first hero factory | Required | Replace candidate kitbash with final Troy-specific hero mesh/materials and authored locomotion/attack/hit/down/death/ability clips |
| Menelaus | GENERATED PLACEHOLDER | KayKit-derived `TroyProduction/Characters/Heroes/Hero_Menelaus`; production-first hero factory | Required | Replace candidate with commander-grade final mesh/materials and boss animation set |
| Greek Infantry | GENERATED PLACEHOLDER | `TroyProduction/Characters/Greek/Enemy_Infantry` | Required | Final Achaean infantry mesh/material/animation pass |
| Greek Runner/Scout | GENERATED PLACEHOLDER | `TroyProduction/Characters/Greek/Enemy_Runner` | Required where used | Final lighter runner silhouette and authored locomotion/combat clips |
| Heavy Hoplite | GENERATED PLACEHOLDER | production candidate with heavy cuirass/helmet, spear + heavy shield | Required | Final heavy model with stronger authored armor/shield silhouette |
| Shield Bearer | GENERATED PLACEHOLDER | production candidate with spear + oversized round shield | Required | Final dedicated shield-bearer model/animations |
| Greek Archer | GENERATED PLACEHOLDER | production candidate uses bow + quiver; old generic crossbow presentation removed | Required | Final period-appropriate bow model and authored draw/release animation |
| Trojan Infantry | GENERATED PLACEHOLDER | `TroyProduction/Characters/Trojan/Trojan_Infantry` | Presentation/support | Final Trojan warm-bronze/red infantry model |
| Trojan Guard | GENERATED PLACEHOLDER | production candidate + runtime guard/blocking system | Required | Final shield-wall/guard squad meshes and formation/block/attack animations |
| Trojan Archer | GENERATED PLACEHOLDER | production candidate uses bow + quiver and is used as Archer Post crew | Required for Archer defense presentation | Final production archer/crew model + bow animation |
| Greek landing party | GENERATED PLACEHOLDER | production-first Greek prefabs; old runtime fallback retained only as fallback; decorative colliders/physics disabled | Required presentation | Final decorative landing prefab variants + authored landing/run clips |

### Chapter I character conclusion

The gameplay roster now has a dedicated production-candidate path and Late Bronze Age role silhouettes, but **zero Chapter I combat characters currently qualify as `DONE`** under the production rule. Final authored character meshes/materials/animation clips and real Play Mode visual QA are still required.

## Chapter I defensive structures / Tower-Units

| Asset | Current status | Evidence/current implementation | Next action |
|---|---|---|---|
| Archer Tower / Archer Post | PROCEDURAL + GENERATED PLACEHOLDER crew | procedural wood/bronze post; generic barrel removed; two `Trojan_Archer` production candidates used as visible crew when available | Replace structure with authored wooden post and final Trojan archer crew/animations |
| Ballista | PROCEDURAL + GENERATED PLACEHOLDER crew | procedural deck/support/rail/bow plus Trojan infantry operator | Dedicated authored ballista mesh, mechanism/reload/fire animation and final crew |
| Priests of Apollo shrine | PROCEDURAL | shrine silhouette plus two dedicated procedural Apollo-priest attendants | Authored shrine + final priest character model(s) + animation/VFX anchors |
| Spear Wall | PROCEDURAL + GENERATED PLACEHOLDER crew | shield/spear post plus two Trojan infantry candidates; no generic barrel | Authored defensive formation/post + final spear-wall soldiers and poke/block animations |
| Fire Tower | PROCEDURAL | brazier/fire/pitch presentation plus dedicated Fire Keeper silhouette; no generic barrel | Authored fire-pot/brazier defense, final keeper/crew if retained, fuel/pitch props |
| Trojan Guard post | PROCEDURAL + GENERATED PLACEHOLDER crew | platform/banners/rack plus two `Trojan_Guard` candidates; no generic barrel | Authored guard station + final Guard squad |

No Chapter I defense model is currently `DONE`.

## Chapter I environment and staging

| Asset | Current status | Current implementation | Next action |
|---|---|---|---|
| Aegean sea/water | PROCEDURAL | runtime presentation/atmosphere layers | Authored water material/mesh treatment suitable for target camera |
| Shoreline / wet sand | PROCEDURAL | runtime world presentation | Authored coastline terrain/mesh/material set |
| Beach / dunes / rocks / scrub | PROCEDURAL | runtime dressing | Production modular beach kit |
| Invasion lanes / worn tracks | PROCEDURAL | runtime map/presentation geometry | Replace remaining grid-like presentation with authored terrain blending |
| Greek ships | PROCEDURAL | expanded Achaean-galley presentation: layered hull/deck/gunwales/keel, prow, mast/yard/sail, benches, oars and hull shields | Dedicated authored Late Bronze Age/Achaean ship model kit, 2–3 variants/LOD silhouettes |
| Greek landing debris | PROCEDURAL | crates, discarded shields, oars and Greek beach standard now staged during landing | Authored prop kit: amphorae, rope, crates, shields, broken gear, oars |
| Greek beach camp | PROCEDURAL | campfires/standards/presentation objects | Authored tents, standards, supply props, weapon racks |
| Troy wall silhouette | PROCEDURAL | flanking wall wings, parapets, buttresses, crenellations and masonry detail integrated with gate | Authored modular Troy wall set |
| Troy gatehouse | PROCEDURAL | layered gatehouse, deep recess/doors, jamb/lintel, wall walk, twin towers, arrow slits, relief, braziers | Authored gatehouse mesh/prefab with same gameplay silhouette |
| Troy wall towers | PROCEDURAL | enhanced twin hero towers with crowns, rings, crenellations and arrow slits | Modular authored tower production asset |
| Troy skyline / city backdrop | PROCEDURAL | stepped houses, citadel terraces, temple focus and inner-city wall skyline | Authored low-cost skyline/building kit |
| Trojan braziers | PROCEDURAL | runtime effects/primitive presentation | Authored brazier prop + flame VFX anchor |
| Banners / standards | PROCEDURAL | runtime primitive poles/cloth blocks | Authored cloth/banner meshes with faction variants |
| Rocks / vegetation | PROCEDURAL / generic | runtime dressing | Production environment prop kit |

### Chapter I environment conclusion

Chapter I now reads much more clearly as an Achaean coastal landing against fortified Troy, but it still lacks a true authored production environment kit. The biggest remaining art gap is replacing procedural coast/ships/Troy structures with final meshes/materials while preserving the current gameplay/readability composition.

---

# Full campaign character inventory

## Greek / Achaean regular units

| Asset | First major use | Status | Notes |
|---|---:|---|---|
| Infantry | I | GENERATED PLACEHOLDER | Chapter I production candidate exists |
| Archer | I | GENERATED PLACEHOLDER | Chapter I candidate now uses bow silhouette |
| Spearman | I/II | GENERATED PLACEHOLDER by infantry/shield variants | Needs explicit dedicated production variant |
| Light Swordsman | II | MISSING | No dedicated production model |
| Shield Bearer | I | GENERATED PLACEHOLDER | Chapter I production candidate exists |
| Hoplite | II | GENERATED PLACEHOLDER by heavy variant | Needs dedicated production variant |
| Heavy Hoplite | I | GENERATED PLACEHOLDER | Heavy spear/shield candidate exists |
| Scout | II | GENERATED PLACEHOLDER by Runner | Needs explicit production identity |
| Runner | I/II | GENERATED PLACEHOLDER | Chapter I production candidate exists |
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
| Trojan Infantry | GENERATED PLACEHOLDER | Chapter I production candidate exists and is reused by Ballista/Spear Wall presentation |
| Trojan Guard | GENERATED PLACEHOLDER | Chapter I candidate + gameplay blocker + Guard post crew |
| Trojan Archer | GENERATED PLACEHOLDER | Chapter I candidate + Archer Post crew |
| Spear Wall soldiers | GENERATED PLACEHOLDER / PROCEDURAL | candidate crew now present; final formation animation still required |
| Ballista crew | GENERATED PLACEHOLDER | Trojan infantry candidate is used as operator; dedicated final crew still required |
| Priests of Apollo | PROCEDURAL | readable priest silhouettes now exist; final character model absent |
| Fire Tower crew | PROCEDURAL | Fire Keeper silhouette now exists; final character model optional/pending |
| Civilians | MISSING | Required for Chapter VII evacuation |

## Heroes / named characters

| Character | Main chapter | Status | Production requirement |
|---|---:|---|---|
| Hector | I–VII | GENERATED PLACEHOLDER | production candidate path exists; final hero asset remains P0 |
| Menelaus | I | GENERATED PLACEHOLDER | production candidate path exists; final Chapter I boss asset remains P0 |
| Achilles | IV/V | GENERATED PLACEHOLDER | later-campaign production candidate generated; final encounter art pending |
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
| Coastal terrain kit | I | PROCEDURAL | current staging improved; final sand/wet shoreline/dunes/rocks/scrub kit missing |
| Greek ship kit | I | PROCEDURAL | galley candidate silhouette improved; authored ship variants missing |
| Greek beach camp kit | I | PROCEDURAL | landing debris expanded; authored camp kit missing |
| Trojan wall/gate kit | I/III | PROCEDURAL | fortified city candidate substantially improved; authored modular set missing |
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
| Greek ranged attack | GENERATED PLACEHOLDER / hook | Authored bow draw/release |
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

1. Hector final production model + animation set. **Candidate pass implemented.**
2. Menelaus final production model + boss animation set. **Candidate pass implemented.**
3. Greek Chapter I regular set: Infantry, Heavy Hoplite, Shield Bearer, Archer, Runner. **Candidate pass implemented.**
4. Trojan Guard + Trojan Archer production variants. **Candidate pass implemented.**
5. Chapter I Tower-Units: Archer, Ballista, Priests, Spear Wall, Fire Tower, Guard post. **Presentation/crew pass implemented; authored models still pending.**
6. Greek landing ship model kit. **Procedural galley pass implemented; authored ship kit still pending.**
7. Chapter I coast kit: shoreline, beach, rocks/scrub, tracks, camp props. **Procedural presentation exists; authored environment kit pending.**
8. Troy wall/gate kit. **Fortified-city presentation pass implemented; authored modular kit pending.**
9. Replace decorative runtime warriors with non-gameplay production decorative prefabs. **Production-first candidates now used and colliders/physics disabled; final decorative variants/animations pending.**
10. 16:9/RU Play Mode visual QA after production replacements. **Still required.**

## P1 — Chapter II/III production

1. Plains/road environment kit.
2. Chariot.
3. Dedicated Hoplite/Spearman/Swordsman variants.
4. Battering Ram + Ram Crew.
5. Siege Tower.
6. Sapper.
7. Expanded Troy wall/gate damage states.

## P2 — Chapter IV/V

1. Achilles final production hero.
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
