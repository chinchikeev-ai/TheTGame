# TheTroyGame — Model & Art Production Inventory

Last reviewed: 2026-09-13

## Purpose

This file is the source of truth for model/art completion. A generator, source pack, procedural silhouette, runtime fallback, or production-path prefab is not automatically final art.

## Status definitions

- `DONE` — final authored production asset is committed under `Assets/Game/Art/...`, runtime uses it, required materials/animations/colliders are correct, and it passed real Play Mode visual QA.
- `GENERATED PLACEHOLDER` — reproducible model/prefab candidate exists but final Troy-specific production art is incomplete.
- `PROCEDURAL` — reproducible presentation is assembled from Unity primitives/generated geometry/code.
- `SOURCE ONLY` — approved source exists or has an installer, but the final generated/imported Troy asset is not committed/verified yet.
- `MISSING` — no dedicated source/candidate/production asset exists.

`DONE` may only be used after the acceptance gate at the end of this file.

---

# Current campaign coverage

## Greek / Achaean units

| Asset | First use | Status | Current candidate |
|---|---:|---|---|
| Infantry | I | GENERATED PLACEHOLDER | `Enemy_Infantry` |
| Archer | I | GENERATED PLACEHOLDER | `Enemy_Archer`, bow/quiver candidate |
| Spearman | I/II | GENERATED PLACEHOLDER | `Enemy_Spearman` |
| Light Swordsman | II | GENERATED PLACEHOLDER | `Enemy_LightSwordsman` |
| Shield Bearer | I | GENERATED PLACEHOLDER | `Enemy_ShieldBearer` |
| Hoplite | II | GENERATED PLACEHOLDER | `Enemy_Hoplite` |
| Heavy Hoplite | I | GENERATED PLACEHOLDER | `Enemy_HeavyHoplite`, spear + heavy shield |
| Scout | II | GENERATED PLACEHOLDER | `Enemy_Scout` |
| Runner | I/II | GENERATED PLACEHOLDER | `Enemy_Runner` |
| Chariot | II | PROCEDURAL + SOURCE ONLY | `Vehicle_Chariot`; pinned animated CC0 horse FBX installer + chariot horse builder exist; generated result still requires Unity import/QA |
| Ram Crew | III | GENERATED PLACEHOLDER | `Enemy_RamCrew` |
| Battering Ram | III | PROCEDURAL | `Siege_BatteringRam` |
| Siege Tower | III | PROCEDURAL | `Siege_SiegeTower` |
| Sapper | III/V | GENERATED PLACEHOLDER | `Enemy_Sapper` |
| Myrmidon | IV | GENERATED PLACEHOLDER | `Enemy_Myrmidon` |
| Myrmidon Veteran | IV/V | GENERATED PLACEHOLDER | `Enemy_MyrmidonVeteran` |
| Greek Captain | IV/V | GENERATED PLACEHOLDER | `Enemy_GreekCaptain` |
| Hero Companion | IV/V | GENERATED PLACEHOLDER | `Enemy_HeroCompanion` |

**Roster candidate coverage: complete for the current GDD. Final authored coverage: incomplete.**

## Trojan units / support

| Asset | Status | Current candidate |
|---|---|---|
| Trojan Infantry | GENERATED PLACEHOLDER | `Trojan_Infantry` |
| Trojan Guard | GENERATED PLACEHOLDER | `Trojan_Guard` |
| Trojan Archer | GENERATED PLACEHOLDER | `Trojan_Archer` |
| Spear Wall soldiers | GENERATED PLACEHOLDER + PROCEDURAL | Trojan infantry crew + procedural post |
| Ballista crew | GENERATED PLACEHOLDER | `Trojan_BallistaCrew`; production candidate preferred when generated |
| Priests of Apollo | GENERATED PLACEHOLDER + PROCEDURAL | `Trojan_PriestApollo` + procedural shrine |
| Fire Tower crew | GENERATED PLACEHOLDER + PROCEDURAL | `Trojan_FireKeeper` + procedural fire defense |
| Civilians | GENERATED PLACEHOLDER | base civilian plus `Worker`, `Elder`, `Young` variant builders |

## Named heroes / bosses

| Character | Chapter | Status | Current candidate |
|---|---:|---|---|
| Hector | I–VII | GENERATED PLACEHOLDER | Bronze Age hero candidate + shared Animator candidate |
| Menelaus | I | GENERATED PLACEHOLDER | commander/boss candidate |
| Achilles | IV/V | GENERATED PLACEHOLDER | hero candidate |
| Ajax | IV | GENERATED PLACEHOLDER | oversized-shield hero candidate |
| Odysseus | VII | GENERATED PLACEHOLDER | cloak/satchel hero candidate |

## Mythic / narrative

| Asset | Chapter | Status | Current candidate |
|---|---:|---|---|
| Cyclops | Later | GENERATED PLACEHOLDER | `Mythic_Cyclops`, giant kitbash candidate |
| Trojan Horse | VI/VII | PROCEDURAL | `Prop_TrojanHorse`, large wooden horse with visible hatch |

---

# Tower-Units

| Asset | Status | Current implementation | Remaining to DONE |
|---|---|---|---|
| Archer Post | PROCEDURAL + GENERATED PLACEHOLDER crew | wood/bronze post + Trojan Archers | Authored structure, bow animations, final materials |
| Ballista | PROCEDURAL + GENERATED PLACEHOLDER crew | readable mechanism + `Trojan_BallistaCrew` candidate | Authored mechanism/string/bolt/reload/fire cycle |
| Priests of Apollo | PROCEDURAL + GENERATED PLACEHOLDER crew | shrine + `Trojan_PriestApollo` candidates | Final shrine/priests/support animation/VFX |
| Spear Wall | PROCEDURAL + GENERATED PLACEHOLDER crew | spear defense + infantry crew | Final formation/poke/block art |
| Fire Tower | PROCEDURAL + GENERATED PLACEHOLDER crew | brazier/pitch/flame + `Trojan_FireKeeper` | Final tower/props/keeper animation |
| Trojan Guard Post | PROCEDURAL + GENERATED PLACEHOLDER crew | platform/banners + Guards | Final station/shield-wall animation |
| Level 2/3 differentiation | PROCEDURAL | runtime upgrade visual markers via `TowerProductionArtBinder` | Authored per-tower L2/L3 meshes and specialization variants |

**Tower-Unit DONE count: 0.**

---

# Environment / structure coverage

| Asset family | First chapter | Status | Current candidate |
|---|---:|---|---|
| Coastal terrain / shoreline | I | PROCEDURAL | sea, wet sand, beach/dunes/rocks/scrub presentation |
| Greek ship kit | I | PROCEDURAL | expanded Achaean galley candidate; final variants/LOD missing |
| Greek beach camp | I | PROCEDURAL | campfires/standards/landing props |
| Trojan wall/gate/skyline | I/III | PROCEDURAL | fortified gatehouse, wall wings, towers, citadel/temple skyline |
| Plains / road terrain | II | PROCEDURAL | `Env_PlainsRoadSegment`, junction, roadside set |
| Siege breach / staging | III | PROCEDURAL | wall breach, ladder/shield/tool staging |
| Damaged outer defenses | IV | PROCEDURAL | `Env_DamagedOuterDefense` |
| Great Assault destruction | V | PROCEDURAL | destroyed build node + damaged/collapsed wall state |
| Abandoned Greek camp | VI | PROCEDURAL | `Env_AbandonedGreekCamp` |
| Trojan Horse plaza | VI | PROCEDURAL | ceremonial plaza candidate |
| Troy interior | VII | PROCEDURAL | house + street modules |
| Burning / collapsed Troy | VII | PROCEDURAL | burning house, collapsed house, evacuation street |

**Environment candidate coverage now exists for Chapters I–VII. Final authored environment coverage remains incomplete.**

---

# Animation / mechanism coverage

| Animation set | Status | Remaining |
|---|---|---|
| Generic Greek locomotion/melee/hit/death | GENERATED PLACEHOLDER | shared KayKit controller candidate; final weapon-specific clips/QA |
| Greek bow attack | GENERATED PLACEHOLDER | final draw/release animation |
| Hector locomotion/downed | GENERATED PLACEHOLDER | hero-specific final clips |
| Hector Q/E/R/F | PROCEDURAL VFX + gameplay | authored body animations |
| Menelaus combat/death | GENERATED PLACEHOLDER | boss-specific clips |
| Trojan Guard block/melee | GENERATED PLACEHOLDER / hook | final formation/block/melee clips |
| Ballista mechanism | MISSING | reload/tension/fire/recoil animation |
| Battering Ram cycle | MISSING | push/impact/recover animation |
| Siege Tower movement | MISSING | wheel/movement/assault animation |
| Chariot horse locomotion | SOURCE ONLY + generator | pinned Quaternius FBX installer and controller generator exist; Unity import/clip/orientation QA still required |

---

# Remaining genuine model/art gaps

The campaign no longer has a major GDD roster item with no candidate at all. The remaining gaps are production-quality gaps rather than missing roster entries:

1. final authored hero/enemy meshes/materials;
2. final authored Tower-Unit structures and L2/L3/specialization variants;
3. final Ballista/Ram/Siege Tower mechanisms and animations;
4. final chariot integration after real horse FBX import and visual QA;
5. authored Greek ship variants/LOD;
6. final civilian variants and evacuation animations;
7. authored intact/damaged/destroyed/burning states for walls, gates, buildings, siege objects and key narrative props;
8. final authored terrain/environment modules for all chapters;
9. real gameplay-camera and Play Mode QA.

---

# Reproducible builders

Current candidate pipeline includes:

- `CartoonCharacterPrefabBuilder` — Chapter I character candidates;
- `ChapterOneCharacterAnimationBuilder` — shared character Animator candidate;
- `CampaignArtCandidateBuilder` — later Greek/Trojan/heroes/chariot/siege/Trojan Horse candidates;
- `MythicAndSupportArtCandidateBuilder` — Priest, Fire Keeper, Ballista Crew, Cyclops;
- `CampaignEnvironmentCandidateBuilder` — Chapter II–VII environment candidates;
- `CampaignCivilianVariantBuilder` — Worker/Elder/Young civilians;
- `CampaignHorseSourceInstaller` — pinned/integrity-checked animated CC0 horse source installer;
- `CampaignChariotHorseBuilder` — replaces primitive chariot horses with imported animated source and builds a locomotion controller;
- `ModelGapClosureBuilder` — one-shot full candidate-set build.

Horse provenance is documented in `docs/third_party/QUATERNIUS_HORSE.md`.

---

# Acceptance gate for `DONE`

An asset can move to `DONE` only when all applicable checks pass:

1. final mesh/prefab exists under `Assets/Game/Art/...`;
2. Unity `.meta` is tracked;
3. third-party provenance/license is documented where applicable;
4. runtime uses the final production prefab rather than a placeholder/procedural fallback;
5. collider setup is intentional and decorative variants cannot affect gameplay physics;
6. materials/textures follow `ART_BIBLE.md` and render correctly;
7. silhouette is readable from the gameplay camera;
8. required animations are connected to runtime presentation hooks;
9. no missing-material/pink-shader state exists;
10. real Play Mode QA passes at target 16:9 layouts and relevant EN/RU framing.

## AI reporting rule

Never call an asset finished solely because a builder can generate it, a source model exists, a runtime fallback renders it, or this inventory lists a candidate. Use `DONE` only after the full acceptance gate passes.
