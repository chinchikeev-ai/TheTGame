# TheTroyGame — Model & Art Production Inventory

Last reviewed: 2026-09-14

This is the authoritative source for **art/model completion status**. Design intent belongs in the GDD/art-direction documents; generation workflow belongs in the pipeline documents.

## Status definitions

Wave HUD banner (2026-09-20): `SOURCE ONLY` pending gameplay visual acceptance.
Generated transparent WaveHud/Banner is integrated with live text and preserved by
the HUD skin. Patron portrait RGBA import verified by colored-pixel rendering tests.

Settings screen (2026-09-20): `SOURCE ONLY` pending in-game visual acceptance.
Reference-derived Settings_Illustrated backdrop and Settings_Reference knob atlas
are used by ModernSettingsPresentation with live controls; no fake audio buses.

Chapter selection (2026-09-19): `SOURCE ONLY` pending real Play Mode visual acceptance.
Reference atlas plus generated clean map and blank parchment are integrated under
Assets/Game/Art/Resources/ChapterSelect. Separate runtime elements support live text
and chapter selection; this is not a single screenshot used as the whole interface.

Pause menu (2026-09-18): `SOURCE ONLY` pending real Play Mode visual acceptance.
Six supplied pieces are integrated under `Assets/Game/Art/Resources/PauseMenu`;
isolated Canvas previews are not final gameplay acceptance.

- `DONE` — final production asset is committed under `Assets/Game/Art/...`, runtime uses it, required materials/animations/colliders are correct, and real Play Mode visual QA passed.
- `GENERATED PLACEHOLDER` — reproducible prefab/model candidate exists, but Troy-specific final production art is incomplete.
- `PROCEDURAL` — visible presentation is assembled from primitives/generated geometry/code.
- `SOURCE ONLY` — approved source exists, but the final Troy derivative is not committed/verified.
- `MISSING` — no dedicated candidate/source/implementation exists.

A builder succeeding does **not** promote an asset to `DONE`.

## UI character portraits

Defender HUD cards (2026-09-20): SOURCE ONLY pending gameplay visual acceptance.
Six generated square portraits under Assets/Game/Art/Resources/DefenderHud are
integrated as individual runtime images: Spearman, Archer, Ballista, Priest, Fire,
Guard. Separate live labels/prices and a shared Hector frame form real buttons.
This change does not replace or promote any 3D unit models.

Gate HUD: GENERATED PLACEHOLDER pending user acceptance. Two separate generated
textures under Assets/Game/Art/Resources/GateHud are wired into the resource block;
live text and HP fill remain independent. Three isolated Unity render tests passed.

Main screen (2026-09-16): SOURCE ONLY supplied artwork integrated as a UI candidate.
Five original transparent PNGs under `Assets/Game/Art/Resources/MainScreen` provide
the logo and four actions over the existing `Menu/Main_screen` background. Eight
isolated RU/EN Unity Canvas captures were inspected; full in-game acceptance remains
pending. No 3D asset or production-art completion status is promoted by this change.

Patron selection page: SOURCE ONLY supplied artwork integrated as a UI candidate.
Five textures under Assets/Game/Art/Resources/PatronSelection provide the background
and four individual selection cards. Runtime wiring is present; visual QA is pending.

| Asset | Status | Current candidate / note |
|---|---|---|
| Ares patron portrait | GENERATED PLACEHOLDER | AI-generated transparent upper-body HUD candidate facing the battlefield; real Unity visual QA pending |
| Athena patron portrait | GENERATED PLACEHOLDER | AI-generated transparent upper-body HUD candidate facing the battlefield; real Unity visual QA pending |
| Apollo patron portrait | GENERATED PLACEHOLDER | AI-generated transparent upper-body HUD candidate facing the battlefield; real Unity visual QA pending |
| Poseidon patron portrait | GENERATED PLACEHOLDER | AI-generated transparent upper-body HUD candidate facing the battlefield; real Unity visual QA pending |

These portraits support the patron-commentary UX but are not `DONE` until the final UI art direction and RU/EN gameplay-layout QA are accepted.

## Chapter I characters

Hector HUD artwork: GENERATED PLACEHOLDER pending in-game visual acceptance.
Eleven dedicated bitmaps under `Assets/Game/Art/Resources/HectorHud` are wired into
`HectorHUD`; the portrait direction is approved, the assembled block awaits review.
This is HUD illustration only and does not change Hector's 3D model status.

| Asset | Status | Current candidate / note |
|---|---|---|
| Greek Infantry | GENERATED PLACEHOLDER + SOURCE ONLY | KayKit-derived candidate + spear/shield/armor candidate pass |
| Runner | GENERATED PLACEHOLDER | role-specific light/skirmisher candidate |
| Heavy Hoplite | GENERATED PLACEHOLDER + SOURCE ONLY | spear + large shield + armor candidates |
| Shield Bearer | GENERATED PLACEHOLDER + SOURCE ONLY | spear + shield + armor candidates |
| Greek Archer | GENERATED PLACEHOLDER + SOURCE ONLY | bow/quiver candidate; draw/release hooks exist; a visible procedural nocked-arrow candidate is shown while ready, hidden at Release, and restored on Draw; flight still uses the existing pooled procedural arrow |
| Menelaus / Boss | GENERATED PLACEHOLDER | commander silhouette + command hook; final boss clips/materials pending |
| Hector | GENERATED PLACEHOLDER + SOURCE ONLY | hero silhouette + spear/shield/armor candidates + explicit `Poke`/`Block` profile states + Q/E/R/F hooks; one combat-action state prevents overlapping movement/attacks/abilities; Q/E/F gameplay now resolves through candidate animation phases; R releases the carried spear into visible flight; down/revive cancels stale callbacks and restores weapon state |
| Trojan Infantry | GENERATED PLACEHOLDER + SOURCE ONLY | spear/shield/armor candidate pass |
| Trojan Guard | GENERATED PLACEHOLDER + SOURCE ONLY | shield/spear/armor candidates + block/poke hooks; runtime keeps a persistent brace while enemies are reserved and faces the nearest reserved attacker |
| Trojan Archer | GENERATED PLACEHOLDER + SOURCE ONLY | bow candidate + draw/release hooks |
| Ballista Crew | GENERATED PLACEHOLDER | support candidate + fire/reload/tension hooks |
| Priest of Apollo | GENERATED PLACEHOLDER | support candidate + cast/channel hooks |
| Fire Keeper | GENERATED PLACEHOLDER | support candidate + throw/stoke hooks |

**Chapter I final character DONE count: 0.**

## Chapter I equipment sources

| Equipment | Status | Notes |
|---|---|---|
| Bow | SOURCE ONLY + generated integration | pinned Quaternius CC0 source; candidate replacement pipeline creates `Socket_ArrowRelease`; runtime presentation adds a procedural nocked-arrow candidate and synchronizes hide/restore with Release/Draw |
| Spear | SOURCE ONLY + generated integration | pinned Quaternius CC0 source; candidate replacement pipeline creates `Socket_SpearRelease`; Hector runtime presentation hides the carried spear during flight and restores it on arrival |
| Aegean round shield | GENERATED PLACEHOLDER | project-authored static candidate; persistent brace/block runtime exists but authored grip/hold animation is still pending |
| Figure-eight/tower shield | GENERATED PLACEHOLDER | project-authored static candidate; persistent brace/block runtime exists but authored grip/hold animation is still pending |
| Dendra-inspired cuirass | GENERATED PLACEHOLDER | project-authored static candidate; currently rigid bone-follow, not final skinning |
| Boar-tusk-inspired helmet | GENERATED PLACEHOLDER | project-authored static candidate |
| Swords / dagger / quiver | GENERATED PLACEHOLDER / SOURCE | current KayKit accessories where available |

Final grip, nock/socket offsets, scale, materials, skin deformation, clipping and gameplay-camera readability still require real Unity QA.

## Chapter I Tower-Units

| Asset | Status | Current implementation | Remaining to DONE |
|---|---|---|---|
| Archer Post | PROCEDURAL + GENERATED PLACEHOLDER crew | wood/bronze post + Trojan Archers + draw/release hooks | authored structure, final archer rig/clips/materials |
| Ballista | PROCEDURAL + GENERATED PLACEHOLDER crew | readable mechanism + two crew candidates + procedural fire/reload/tension cycle | final authored mechanism/string/bolt/crew animation/materials |
| Priests of Apollo | PROCEDURAL + GENERATED PLACEHOLDER crew | shrine + two priests + cast/channel + Apollo pulse | final shrine/priests/support animation/VFX/materials |
| Spear Wall | PROCEDURAL + GENERATED PLACEHOLDER crew + SOURCE ONLY spear | shield/spear line + infantry crew + poke hook | final formation/structure/materials/authored clips |
| Fire Tower | PROCEDURAL + GENERATED PLACEHOLDER crew | brazier/fire presentation + Fire Keeper + throw/stoke + flame pulse | final tower/props/keeper clips/materials |
| Trojan Guard Post | PROCEDURAL + GENERATED PLACEHOLDER crew + SOURCE ONLY spear | shield-wall station + Guards + persistent reservation-driven brace, nearest-attacker facing, candidate damage mitigation, shield impact feedback and animation-phase spear damage | final station/formation, authored brace/hold/release clips/materials, mitigation balance and Play Mode timing QA |
| Level 2/3 differentiation | PROCEDURAL | upgrade visual markers via `TowerProductionArtBinder` | authored per-tower L2/L3 meshes and specialization variants |

**Chapter I Tower-Unit DONE count: 0.**

## Chapter I environment

| Asset family | Status | Current candidate |
|---|---|---|
| Coast / shoreline | PROCEDURAL | sea, wet sand, beach/dunes/rocks/scrub |
| Greek landing ships | PROCEDURAL | Achaean galley presentation; final variants/LOD missing |
| Greek beach camp | PROCEDURAL | campfires, standards and landing props |
| Troy wall / gate / skyline | PROCEDURAL | fortified gatehouse, wall wings, towers, citadel/temple backdrop |
| Atmosphere | PROCEDURAL | braziers, smoke, lighting, battlefield dressing, cinematic camera |

**Chapter I environment DONE count: 0.**

## Animation / mechanism coverage

| Set | Status | Current state / remaining work |
|---|---|---|
| Generic locomotion / hit / death | GENERATED PLACEHOLDER | role controllers exist; imported clips are selected deterministically by scored token matching; final authored clips/QA pending |
| Spear combat | GENERATED PLACEHOLDER | `Poke` hooks wired to relevant runtime attacks; spear profiles use deterministic thrust/stab/poke candidate selection; enemy/Hector/Guard damage waits for the animation impact phase with a bounded fallback; final authored thrust/brace clips and timing QA pending |
| Archer combat | GENERATED PLACEHOLDER + PROCEDURAL projectile/ammo presentation | `Draw` / `Release` wired; ranged attack waits for Release; `CharacterWeaponPresentation` hides the nocked-arrow candidate at release, pooled projectile flight starts from the resolved bow socket hierarchy, damage applies on arrival, and the nocked arrow returns on redraw; final arrow asset, string/nock alignment and timing pending |
| Hector | GENERATED PLACEHOLDER + PROCEDURAL VFX/projectile | authoritative `HectorCombatAction` locks movement/new commands during basic/Q/E/R/F actions; basic spear and Q/E/F gameplay use normalized animation-phase callbacks with bounded fallback; R additionally detaches the carried spear, flies visibly and applies arrival damage; Shield Wall starts its defensive window from the E phase; target-facing is held during attack/throw; downed invalidates pending callbacks and revive restores presentation state | final hero-specific clips, exact action-lock lengths, contact/release frames, Shield Wall pose/VFX lifetime and Play Mode QA |
| Character projectile flight | PROCEDURAL | `CombatFlightPresentation` provides pooled arrow/spear geometry, target tracking, oriented ballistic arc and arrival callbacks; `CharacterWeaponSocketResolver` resolves explicit release markers/source weapon/humanoid hand fallbacks; `CharacterWeaponPresentation` owns carried/nocked weapon visibility state; no projectile physics collider is used | replace procedural geometry/ammo presentation with production assets; tune hand grip, nock/release offsets, speed/arc/contact frames in real Play Mode |
| Shield block | GENERATED PLACEHOLDER + PROCEDURAL feedback | `CharacterPresentationState.SetBlocking` provides a persistent logical defensive state while preserving the existing `Block` trigger fallback; Trojan Guard braces while reservations exist and turns toward its nearest reserved attacker; Hector Shield Wall holds defensive state for its gameplay duration; blocked hits use procedural bronze pulse/tone feedback | authored brace/hold/release and blocked-hit reaction clips, shield hand IK/grip, final VFX/audio, directional mitigation tuning and Play Mode QA |
| Menelaus | GENERATED PLACEHOLDER | command hook tied to reinforcements; deterministic command candidate/fallback is logged; repeated gate attacks resolve damage at the attack impact phase; final boss-specific clips pending |
| Ballista mechanism | PROCEDURAL | `TowerSupportMechanismPresentation` performs release/reload/tension feedback; final authored mechanism animation pending |
| Apollo shrine mechanism | PROCEDURAL | cast pulse/disc motion exists; final authored support presentation pending |
| Fire Tower mechanism | PROCEDURAL | flame pulse exists; final authored fire/keeper presentation pending |
| Battering Ram cycle | MISSING | later chapter push/impact/recover animation |
| Siege Tower movement | MISSING | later chapter wheel/movement/assault animation |
| Chariot horse locomotion | SOURCE ONLY + generator | pinned horse source + generator; real import/orientation/clip QA pending |

The current Chapter I animation builder/runtime timing/projectile/defensive layer is an auditable candidate system, not final animation production. It uses normalized Animator-state phases where the expected state is available, bounded fallback timing otherwise, procedural pooled character projectiles, deterministic release-point resolution, temporary runtime weapon/ammunition visibility synchronization, persistent logical block-state, reservation-driven Guard brace/facing, a timed Hector Shield Wall mitigation window and a Hector combat-action lock that prevents conflicting locomotion/ability presentation. Exact hero clips, action-lock timing, hand/shield grip, bow-string/nock alignment, release-socket offsets, flight arcs, contact frames, brace/hold clips, mitigation values, projectile meshes and pose quality still require real Unity Play Mode inspection before production acceptance.

## Later-campaign candidate coverage

These have candidates or procedural coverage, but are **not final art**:

### Greek / enemy roster
- Spearman — GENERATED PLACEHOLDER
- Light Swordsman — GENERATED PLACEHOLDER
- Hoplite — GENERATED PLACEHOLDER
- Scout — GENERATED PLACEHOLDER
- Chariot — PROCEDURAL + SOURCE ONLY
- Ram Crew — GENERATED PLACEHOLDER
- Battering Ram — PROCEDURAL
- Siege Tower — PROCEDURAL
- Sapper — GENERATED PLACEHOLDER
- Myrmidon — GENERATED PLACEHOLDER
- Myrmidon Veteran — GENERATED PLACEHOLDER
- Greek Captain — GENERATED PLACEHOLDER
- Hero Companion — GENERATED PLACEHOLDER

### Named / mythic
- Achilles — GENERATED PLACEHOLDER
- Ajax — GENERATED PLACEHOLDER
- Odysseus — GENERATED PLACEHOLDER
- Cyclops — GENERATED PLACEHOLDER
- Trojan Horse — PROCEDURAL

### Environments
Candidate/procedural coverage exists for:
- Chapter II plains/road;
- Chapter III siege/breach staging;
- Chapter IV damaged outer defenses;
- Chapter V destruction states;
- Chapter VI abandoned Greek camp / Horse plaza;
- Chapter VII Troy interior / burning-collapse states.

Final authored environment coverage remains incomplete.

## Current reproducible builders

Character/equipment:
- `CartoonCharacterPrefabBuilder`
- `ChapterOneWeaponSourceInstaller`
- `ChapterOneSpearSourceInstaller`
- `ChapterOneShieldCandidateBuilder`
- `ChapterOneArmorCandidateBuilder`
- `ChapterOneProductionEquipmentBuilder` — installs the pinned bow/spear candidates and writes explicit `Socket_ArrowRelease` / `Socket_SpearRelease` markers onto generated weapon roots
- `ChapterOneCharacterAnimationBuilder` — deterministic per-role Animator candidates, including spear/Hector `Poke` + `Block`, bow `Draw` + `Release`, commander/hero hooks, and explicit binding/fallback logs
- `MythicAndSupportArtCandidateBuilder`

Campaign-wide candidates:
- `CampaignArtCandidateBuilder`
- `CampaignEnvironmentCandidateBuilder`
- `CampaignCivilianVariantBuilder`
- `CampaignHorseSourceInstaller`
- `CampaignChariotHorseBuilder`
- `ModelGapClosureBuilder`

Third-party provenance:
- `docs/third_party/QUATERNIUS_HORSE.md`
- `docs/third_party/QUATERNIUS_MEDIEVAL_WEAPONS.md`

## Remaining genuine production gaps

1. final authored/rigged hero and enemy meshes/materials;
2. final skinned armor/weapon/shield integration and authored hand grip/IK;
3. final weapon-specific combat clips, shield brace/hold/release clips and hero/boss animation sets;
4. final authored arrow/spear projectile meshes plus nock/release/contact tuning;
5. final authored six Chapter I Tower-Unit structures and L2/L3/specialization variants;
6. final authored Ballista / Ram / Siege Tower mechanisms;
7. final chariot integration;
8. Greek ship variants/LOD;
9. final civilians and evacuation animation;
10. authored damaged/destroyed/burning states for key structures;
11. final environment modules for the campaign;
12. real gameplay-camera and Play Mode QA, including exact contact/release-frame, grip, block-state, mitigation and projectile-flight tuning.

## Acceptance gate for `DONE`

An asset can move to `DONE` only when all applicable checks pass:

1. final mesh/prefab exists under `Assets/Game/Art/...`;
2. Unity `.meta` is tracked;
3. third-party provenance/license is documented where applicable;
4. runtime uses the final production asset rather than the placeholder/procedural fallback;
5. collider setup is intentional and decorative variants cannot affect gameplay physics;
6. materials/textures follow `ART_BIBLE.md` and render correctly;
7. silhouette is readable from the gameplay camera;
8. required animations are connected to runtime presentation hooks;
9. no missing-material/pink-shader state exists;
10. real Play Mode QA passes at target 16:9 layouts and relevant EN/RU framing;
11. where required by Chapter I freeze, the production/QA acceptance manifests are explicitly updated by a human after inspection.

## Reporting rule

Never call an asset finished solely because a builder can generate it, a source exists, a runtime fallback renders it, or a prefab is stored under `Assets/Game/Art`. Use `DONE` only after the full acceptance gate passes.
