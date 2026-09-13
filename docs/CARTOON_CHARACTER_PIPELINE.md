# Chapter I Character Candidate Pipeline

Last reviewed: 2026-09-14

This document explains how reproducible Chapter I character **candidates** are built. It does not define final-art status; `MODEL_ART_INVENTORY.md` does.

## Source assets

Primary rig/animation source:

`Assets/ThirdParty/KayKitAdventurers`

Initialize after cloning:

```bash
git submodule update --init --recursive
```

Additional equipment sources are handled by dedicated builders/installers:
- pinned Quaternius CC0 bow;
- pinned Quaternius CC0 spear;
- project-authored Aegean round shield;
- project-authored figure-eight/tower shield;
- project-authored Dendra-inspired cuirass;
- project-authored boar-tusk-inspired helmet.

Third-party provenance is documented under `docs/third_party/`.

## Output

Generated Chapter I candidates are written under:

`Assets/Game/Art/Characters/Resources/TroyProduction/Characters`

Core Chapter I set:
- `Enemy_Infantry`
- `Enemy_Runner`
- `Enemy_HeavyHoplite`
- `Enemy_ShieldBearer`
- `Enemy_Archer`
- `Enemy_Boss`
- `Trojan_Infantry`
- `Trojan_Guard`
- `Trojan_Archer`
- `Hero_Hector`
- `Hero_Menelaus`

Support candidates:
- `Trojan_BallistaCrew`
- `Trojan_PriestApollo`
- `Trojan_FireKeeper`

Later campaign builders may also create Achilles, Ajax, Odysseus, siege, chariot, civilian and mythic candidates.

## Recommended build order

1. `CartoonCharacterPrefabBuilder` — base Chapter I character candidates.
2. `MythicAndSupportArtCandidateBuilder` — Ballista crew, Apollo priest, Fire Keeper and related support candidates.
3. `ChapterOneWeaponSourceInstaller` / `ChapterOneSpearSourceInstaller` — verify/install pinned external equipment sources.
4. `ChapterOneShieldCandidateBuilder` — bind authored shield candidates.
5. `ChapterOneArmorCandidateBuilder` — bind cuirass/helmet candidates.
6. `ChapterOneProductionEquipmentBuilder` — apply the complete Chapter I equipment pass.
7. `ChapterOneCharacterAnimationBuilder` — create/assign role-specific Animator profiles.

`ModelGapClosureBuilder` can orchestrate the wider candidate pipeline, but its output is still candidate art.

## Role-specific animation profiles

The animation builder no longer relies on one generic controller for all roles. Current profiles include:
- generic;
- spear;
- archer;
- skirmisher;
- Hector;
- Menelaus;
- Ballista crew;
- Apollo priest;
- Fire Keeper.

Runtime presentation hooks currently include:

| Role | Hooks |
|---|---|
| Generic combat | `Speed`, `Attack`, `Hit`, `Die`, `IsDowned` |
| Spear / Guard | `Block`, `Poke` |
| Archer | `Draw`, `Release` |
| Hector | `AbilityQ`, `AbilityE`, `AbilityR`, `AbilityF` + combat hooks |
| Menelaus | `Command` |
| Ballista crew | `Fire`, `Reload`, `Tension` |
| Apollo priest | `Cast`, `Channel` |
| Fire Keeper | `Throw`, `Stoke` |

The builder uses source clip-name matching and fallbacks. The selected clips must be visually inspected; successful generation is not proof of animation quality.

## Tower/support presentation

`TowerCrewAnimationBridge` connects tower gameplay events to crew animation hooks.

`TowerSupportMechanismPresentation` adds current procedural mechanism feedback for:
- Ballista release/reload/tension cycle;
- Apollo focus/disc cast pulse;
- Fire Tower flame pulse.

These are presentation candidates. Final authored mechanisms and clips are still required for production-art acceptance.

## Runtime fallback order

Character factories prefer `TroyProduction/...` candidates. If unavailable, they can fall back to older generated resources and finally procedural/runtime visuals where supported.

This allows art replacement without breaking gameplay.

## Acceptance rule

A generated prefab is **not `DONE`** because:
- it exists under `Assets/Game/Art`;
- a builder generated it successfully;
- a source asset is licensed/available;
- an AnimatorController was assigned;
- it looks recognizable from the editor scene view.

Promotion to `DONE` requires the full gate in `MODEL_ART_INVENTORY.md`, including real Play Mode gameplay-camera QA.