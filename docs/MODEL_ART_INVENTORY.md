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
| Infantry | I | GENERATED PLACEHOLDER + SOURCE ONLY | `Enemy_Infantry`; pinned Quaternius CC0 spear + authored Aegean round shield + authored Dendra-inspired cuirass/boar-tusk helmet candidates |
| Archer | I | GENERATED PLACEHOLDER + SOURCE ONLY | `Enemy_Archer`; pinned integrity-checked Quaternius CC0 wooden bow source can replace the procedural bow after candidate build; quiver remains KayKit-based |
| Spearman | I/II | GENERATED PLACEHOLDER | `Enemy_Spearman`; campaign variant is still generated before the Chapter I source-equipment pass and is not claimed source-upgraded |
| Light Swordsman | II | GENERATED PLACEHOLDER | `Enemy_LightSwordsman` |
| Shield Bearer | I | GENERATED PLACEHOLDER + SOURCE ONLY | `Enemy_ShieldBearer`; pinned CC0 spear + authored Aegean round shield + authored cuirass/helmet candidates |
| Hoplite | II | GENERATED PLACEHOLDER | `Enemy_Hoplite` |
| Heavy Hoplite | I | GENERATED PLACEHOLDER + SOURCE ONLY | `Enemy_HeavyHoplite`; pinned CC0 spear + authored figure-eight/tower shield + authored cuirass/helmet candidates |
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
| Trojan Infantry | GENERATED PLACEHOLDER + SOURCE ONLY | `Trojan_Infantry`; pinned CC0 spear + authored Aegean round shield + authored cuirass/helmet candidates |
| Trojan Guard | GENERATED PLACEHOLDER + SOURCE ONLY | `Trojan_Guard`; pinned CC0 spear + authored figure-eight/tower shield + authored cuirass/helmet candidates |
| Trojan Archer | GENERATED PLACEHOLDER + SOURCE ONLY | `Trojan_Archer`; pinned integrity-checked Quaternius CC0 wooden bow source can replace the procedural bow after candidate build |
| Spear Wall soldiers | GENERATED PLACEHOLDER + PROCEDURAL + SOURCE ONLY spear | Trojan infantry crew + procedural post; source spear and authored round shield/armor candidates available after Chapter I equipment pass |
| Ballista crew | GENERATED PLACEHOLDER | `Trojan_BallistaCrew`; production candidate preferred when generated |
| Priests of Apollo | GENERATED PLACEHOLDER + PROCEDURAL | `Trojan_PriestApollo` + procedural shrine |
| Fire Tower crew | GENERATED PLACEHOLDER + PROCEDURAL | `Trojan_FireKeeper` + procedural fire defense |
| Civilians | GENERATED PLACEHOLDER | base civilian plus `Worker`, `Elder`, `Young` variant builders |

## Named heroes / bosses

| Character | Chapter | Status | Current candidate |
|---|---:|---|---|
| Hector | I–VII | GENERATED PLACEHOLDER + SOURCE ONLY | Bronze Age hero candidate + shared Animator candidate; pinned CC0 spear + authored figure-eight shield + authored cuirass/helmet candidates |
| Menelaus | I | GENERATED PLACEHOLDER | commander/boss candidate; authored Aegean round shield and authored cuirass/helmet candidates are available through Chapter I equipment pass |
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
| Archer Post | PROCEDURAL + GENERATED PLACEHOLDER crew + SOURCE ONLY bow | wood/bronze post + Trojan Archers; pinned CC0 source bow is available through the Chapter I equipment pass | Authored structure, final archer/bow rig, authored draw/release clips, final materials |
| Ballista | PROCEDURAL + GENERATED PLACEHOLDER crew | readable mechanism + `Trojan_BallistaCrew` candidate | Authored mechanism/string/bolt/reload/fire cycle |
| Priests of Apollo | PROCEDURAL + GENERATED PLACEHOLDER crew | shrine + `Trojan_PriestApollo` candidates | Final shrine/priests/support animation/VFX |
| Spear Wall | PROCEDURAL + GENERATED PLACEHOLDER crew + SOURCE ONLY spear | spear defense + Trojan infantry crew; pinned source spear + authored round shield/armor candidates available after equipment pass | Final formation/poke/block art, final authored structure/materials |
| Fire Tower | PROCEDURAL + GENERATED PLACEHOLDER crew | brazier/pitch/flame + `Trojan_FireKeeper` | Final tower/props/keeper animation |
| Trojan Guard Post | PROCEDURAL + GENERATED PLACEHOLDER crew + SOURCE ONLY spear | platform/banners + Guards; pinned source spear + authored figure-eight shield/armor candidates available after equipment pass; runtime guard combat now invokes block/poke presentation hooks | Final station/shield-wall authored clips and authored station/materials |
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
| Generic Greek locomotion/melee/hit/death | GENERATED PLACEHOLDER | shared KayKit controller candidate; Chapter I runtime now dispatches weapon-role presentation triggers instead of routing all attacks through generic `Attack`; final authored weapon-specific clips/QA remain |
| Greek bow attack | GENERATED PLACEHOLDER + SOURCE ONLY bow geometry | Chapter I archers now exercise `Draw`/`Release` hooks during real attack events; final authored draw/release clips, weapon/hand rig and timing QA still missing |
| Greek/Trojan spear melee | GENERATED PLACEHOLDER + SOURCE ONLY spear geometry | Chapter I spear infantry, Heavy Hoplite, Shield Bearer, Hector basic attacks and Trojan Guard attacks now exercise `Poke`/spear hooks; final authored thrust/brace/formation clips and grip/orientation QA still missing |
| Shield block / brace | GENERATED PLACEHOLDER + authored static shield candidates | Aegean round and figure-eight/tower shield meshes exist; Trojan Guard reservation already exercises the `Block` hook; final hand pose, authored block animation, materials and gameplay-camera QA still missing |
| Armor deformation / clearance | GENERATED PLACEHOLDER + authored static armor candidates | Dendra-inspired cuirass and boar-tusk-inspired helmet now rigidly follow resolved torso/head bones while preserving the rest pose; meshes are still unskinned and require deformation/clipping QA and final rigged integration |
| Hector locomotion/downed | GENERATED PLACEHOLDER | basic melee now uses spear presentation hook; hero-specific final locomotion/attack/downed clips still missing |
| Hector Q/E/R/F | PROCEDURAL VFX + gameplay | ability-specific Animator hooks are driven by the actual gameplay events; authored hero body animations still missing |
| Menelaus combat/death | GENERATED PLACEHOLDER | commander animation hook is tied to real reinforcement calls; final boss-specific combat/command/death clips missing |
| Trojan Guard block/melee | GENERATED PLACEHOLDER + SOURCE ONLY spear / authored shield/armor candidates | runtime reservation/attack events exercise `Block` and spear `Poke` hooks; final formation/block/melee authored clips still missing |
| Ballista mechanism | MISSING | reload/tension/fire/recoil animation |
| Battering Ram cycle | MISSING | push/impact/recover animation |
| Siege Tower movement | MISSING | wheel/movement/assault animation |
| Chariot horse locomotion | SOURCE ONLY + generator | pinned Quaternius FBX installer and controller generator exist; Unity import/clip/orientation QA still required |

---

# Chapter I equipment source coverage

- **Bow:** pinned Quaternius Medieval Weapons CC0 source exists. `ChapterOneWeaponSourceInstaller` verifies the exact FBX by size and Git blob SHA, and `ChapterOneProductionEquipmentBuilder` replaces only the recognized primitive bow hierarchy on Greek/Trojan archer candidates. Chapter I enemy runtime now prepares the bow and routes archer attacks through `Release` followed by a redraw hook when those Animator parameters exist.
- **Spear:** pinned Quaternius Medieval Weapons CC0 source exists. `ChapterOneSpearSourceInstaller` verifies the exact FBX by size and Git blob SHA. `ChapterOneProductionEquipmentBuilder` replaces only the recognized `Spear` + `Shaft` + `BronzeTip` hierarchy on `Enemy_Infantry`, `Enemy_HeavyHoplite`, `Enemy_ShieldBearer`, `Trojan_Infantry`, `Trojan_Guard`, and `Hero_Hector`, while preserving the previous candidate local transform. Chapter I infantry-family runtime, Hector basic attacks and Trojan Guard attacks now route presentation through the spear `Poke` hook with generic `Attack` fallback.
- **Shields:** two project-authored static mesh candidates exist under `Assets/Game/Art/Characters/Equipment`: `AegeanRoundShield.obj` and `FigureEightTowerShield.obj`. `ChapterOneShieldCandidateBuilder` replaces only recognized generated `Gear_shield_*` hierarchies and preserves the old candidate pose. This intentionally avoids using a classical hoplite aspis as the generic Chapter I solution because the project targets a Late Bronze Age / Mycenaean visual direction rather than Classical Greek/Spartan styling.
- **Cuirass / helmet:** two project-authored static mesh candidates exist: `DendraCuirassCandidate.obj` and `BoarTuskHelmetCandidate.obj`. They are stylized Dendra/boar-tusk-inspired silhouettes authored for this project, **not archaeological scans and not claimed as exact reconstructions**. `ChapterOneArmorCandidateBuilder` replaces only the procedural `BronzeCuirass`, shoulder-bronze, `BronzeHelmet`, and cheek-piece parts while intentionally retaining `LeatherBelt` and `FactionCloth` for faction readability. It resolves humanoid torso/head bones first, falls back to deterministic rig-name hints, and reparents the static candidates with rest-world pose preserved. If a bone cannot be resolved, it deliberately retains root-space placement and emits a warning. This is rigid bone-follow attachment, **not skinning**. Targets are Chapter I Infantry, Heavy Hoplite, Shield Bearer, Enemy Boss/Menelaus representation, Trojan Infantry, Trojan Guard, Hector, and Hero Menelaus representation.
- **Faction cloth / hero crest / cape:** still procedural/kitbashed candidate geometry unless the underlying item comes from the KayKit source pack.
- **Swords / dagger / quiver:** current candidate pipeline uses available KayKit source accessories where present.

The bow, spear, shield, cuirass and helmet candidates improve silhouette/source quality and now have stronger runtime/rig-follow integration, but are not final Troy-authored animation rigs or accepted production art. Imported orientation, scale, grip, materials, skin deformation, animation clearance, clip quality and gameplay-camera readability still require real Unity visual QA.

---

# Remaining genuine model/art gaps

The campaign no longer has a major GDD roster item with no candidate at all. The remaining gaps are production-quality gaps rather than missing roster entries:

1. final authored/rigged hero/enemy meshes, skinned armor integration and materials;
2. final authored weapon-specific combat clips and hero/boss animation sets;
3. final authored Tower-Unit structures and L2/L3/specialization variants;
4. final Ballista/Ram/Siege Tower mechanisms and animations;
5. final chariot integration after real horse FBX import and visual QA;
6. authored Greek ship variants/LOD;
7. final civilian variants and evacuation animations;
8. authored intact/damaged/destroyed/burning states for walls, gates, buildings, siege objects and key narrative props;
9. final authored terrain/environment modules for all chapters;
10. real gameplay-camera and Play Mode QA.

---

# Reproducible builders

Current candidate pipeline includes:

- `CartoonCharacterPrefabBuilder` — Chapter I character candidates;
- `ChapterOneWeaponSourceInstaller` — pinned/integrity-checked Quaternius CC0 wooden bow source installer;
- `ChapterOneSpearSourceInstaller` — pinned/integrity-checked Quaternius CC0 spear source installer;
- `ChapterOneShieldCandidateBuilder` — binds project-authored Aegean round and figure-eight/tower shield meshes to recognized Chapter I generated shield poses;
- `ChapterOneArmorCandidateBuilder` — replaces only procedural bronze armor/helmet pieces with project-authored Dendra-inspired cuirass and boar-tusk-inspired helmet static mesh candidates and rigidly attaches them to resolved torso/head bones while preserving rest pose;
- `ChapterOneProductionEquipmentBuilder` — applies bow, spear, shield and armor candidate replacement after Chapter I character generation;
- `ChapterOneCharacterAnimationBuilder` — shared character Animator candidate;
- `CampaignArtCandidateBuilder` — later Greek/Trojan/heroes/chariot/siege/Trojan Horse candidates;
- `MythicAndSupportArtCandidateBuilder` — Priest, Fire Keeper, Ballista Crew, Cyclops;
- `CampaignEnvironmentCandidateBuilder` — Chapter II–VII environment candidates;
- `CampaignCivilianVariantBuilder` — Worker/Elder/Young civilians;
- `CampaignHorseSourceInstaller` — pinned/integrity-checked animated CC0 horse source installer;
- `CampaignChariotHorseBuilder` — replaces primitive chariot horses with imported animated source and builds a locomotion controller;
- `ModelGapClosureBuilder` — one-shot full candidate-set build, including the Chapter I equipment source pass.

Third-party provenance is documented in:

- `docs/third_party/QUATERNIUS_HORSE.md`
- `docs/third_party/QUATERNIUS_MEDIEVAL_WEAPONS.md`

The Late Bronze Age shield, cuirass and helmet meshes in `Assets/Game/Art/Characters/Equipment` are project-authored candidate geometry and therefore do not require third-party provenance.

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
