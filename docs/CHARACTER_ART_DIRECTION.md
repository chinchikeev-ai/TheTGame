# Character Art Direction

Last reviewed: 2026-09-14

This document defines the visual language and silhouette rules for characters. Implementation status belongs in `MODEL_ART_INVENTORY.md`; generation details belong in `CARTOON_CHARACTER_PIPELINE.md`.

## Historical / visual target

Chapter I should read as stylized **Late Bronze Age / Mycenaean-inspired warfare**, not Classical Spartan and not medieval fantasy.

Preferred language:
- bronze and leather armor;
- spears and large shields;
- simple crested helmets;
- strong faction cloth accents;
- readable bows/quivers for archers;
- exaggerated but believable silhouettes at the gameplay camera distance.

## Faction identity

### Greeks / Achaeans
- cooler bronze and muted blue accents;
- disciplined, cleaner military silhouettes;
- heavy units read wider and more armored than regular infantry;
- commanders use stronger crest/cape/shield hierarchy.

### Trojans
- warmer bronze, dark red and gold accents;
- more regal guard/hero silhouettes;
- defensive formations should visually read as an organized city garrison rather than generic fantasy troops.

## Chapter I silhouette contract

### Greek attackers
- **Infantry:** spear + round shield.
- **Runner:** light body, smaller weapon, visibly faster silhouette.
- **Heavy Hoplite:** heavier armor + large shield + spear.
- **Shield Bearer:** shield-dominant silhouette + spear.
- **Archer:** bow + quiver; no generic crossbow presentation.
- **Menelaus:** commander crest/cape, royal shield, one-handed command weapon.

### Trojan defenders
- **Trojan Infantry:** spear + round shield.
- **Trojan Guard:** large shield + spear, formation-oriented silhouette.
- **Trojan Archer:** bow + quiver.
- **Ballista Crew:** work/tool silhouette rather than frontline infantry.
- **Priest of Apollo:** robe + sun staff/disc.
- **Fire Keeper:** pitch/fire handling equipment.

### Heroes
- **Hector:** long spear, large Trojan shield, red/gold hero crest and cape.
- **Menelaus:** colder bronze/blue commander treatment.
- **Achilles:** brighter heroic Greek treatment for later campaign use.

## Equipment sources

The current candidate pipeline can use:
- pinned Quaternius CC0 bow source;
- pinned Quaternius CC0 spear source;
- project-authored Aegean round shield candidate;
- project-authored figure-eight/tower shield candidate;
- project-authored Dendra-inspired cuirass candidate;
- project-authored boar-tusk-inspired helmet candidate;
- KayKit accessories where appropriate.

These are **candidate/source assets**, not automatic final art.

## Production paths

Generated/promoted character candidates live under:

`Assets/Game/Art/Characters/Resources/TroyProduction/Characters`

with faction/role subfolders for Greek, Trojan and Heroes.

Final art remains subject to the `DONE` gate in `MODEL_ART_INVENTORY.md`.

## Runtime readability rules

1. Role must be identifiable primarily by silhouette/equipment, not color alone.
2. Faction tint is secondary to silhouette.
3. Decorative equipment must not alter gameplay colliders.
4. Imported weapons must maintain sensible hand grip/orientation during combat animation.
5. Armor must be checked for clipping/deformation in locomotion, attack, hit and death states.
6. Hero and boss silhouettes must remain readable at normal gameplay zoom.

## Build / QA workflow

1. `git submodule update --init --recursive`
2. Build Chapter I candidates in Unity.
3. Run the Chapter I equipment/source pass.
4. Build role-specific Animator profiles.
5. Inspect in real Play Mode at gameplay camera distance.
6. Only then promote status in `MODEL_ART_INVENTORY.md`.

The runtime may retain generated/procedural fallbacks while final production art is replaced incrementally.