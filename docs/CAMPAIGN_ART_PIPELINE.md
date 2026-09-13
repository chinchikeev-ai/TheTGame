# TheTroyGame — Campaign Art Candidate Pipeline

Last reviewed: 2026-09-13

## Purpose

`CampaignArtCandidateBuilder` creates a reproducible visual candidate bank for gameplay that is planned in Chapters II–VII before final authored models are available.

These assets are not final production art. Character derivatives are `GENERATED PLACEHOLDER`; vehicles, siege engines and hero props assembled from Unity primitives are `PROCEDURAL` candidates.

## Build command

In Unity run:

`The Troy Game -> Characters -> Build Campaign Art Candidates`

The command first ensures the Chapter I production-character candidates and shared KayKit-derived animation controller exist, then writes later-campaign candidates beneath the production art tree.

## Character candidates

Generated under `Assets/Game/Art/Characters/Resources/TroyProduction/Characters`:

### Greek / Achaean
- `Enemy_Spearman`
- `Enemy_Scout`
- `Enemy_LightSwordsman`
- `Enemy_Hoplite`
- `Enemy_Myrmidon`
- `Enemy_MyrmidonVeteran`
- `Enemy_Sapper`
- `Enemy_GreekCaptain`
- `Enemy_HeroCompanion`
- `Enemy_RamCrew`

### Named heroes
- `Hero_Ajax`
- `Hero_Odysseus`

### Trojan
- `Trojan_Civilian`

The variants inherit the existing rig/controller from Chapter I candidates and receive role-specific silhouette treatment, faction tint and equipment/props.

## Vehicle / siege candidates

Generated under `Assets/Game/Art/Vehicles/Resources/TroyProduction`:

- `Vehicles/Vehicle_Chariot`
  - two-horse silhouette;
  - wooden/bronze body;
  - wheels/yoke;
  - Greek captain crew.
- `Siege/Siege_BatteringRam`
  - suspended ram beam;
  - bronze head;
  - wheeled frame;
  - hide roof;
  - two ram crew.
- `Siege/Siege_SiegeTower`
  - tall wooden body;
  - wheels;
  - front hide;
  - ladder;
  - fighting platform/crew.

## Narrative prop candidate

Generated under `Assets/Game/Art/Props/Resources/TroyProduction/Props`:

- `Prop_TrojanHorse`
  - large wooden-horse silhouette;
  - body, legs, neck, head, muzzle and mane;
  - visible hatch;
  - bronze eye accents.

## Status contract

The builder changes inventory status only from `MISSING` to one of:

- `GENERATED PLACEHOLDER` for character derivatives;
- `PROCEDURAL` for generated vehicles, siege engines and props.

It does not mark anything `DONE`.

Final promotion still requires the acceptance gate in `docs/MODEL_ART_INVENTORY.md`: authored mesh/prefab, tracked metadata, documented provenance, final materials, animation, runtime integration and real Play Mode visual QA.

## Scope boundary

This builder creates the model bank only. It does not implement Chapter II–VII gameplay, AI, wave composition, pathing, boss mechanics, vehicle physics or campaign transitions.
