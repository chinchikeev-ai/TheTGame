# Character Art Direction

Last reviewed: 2026-09-14

This document defines the visual language and silhouette rules for characters. Tone is governed by `CREATIVE_DIRECTION.md`; implementation status belongs in `MODEL_ART_INVENTORY.md`; generation details belong in `CARTOON_CHARACTER_PIPELINE.md`.

## Historical / visual target

Chapter I should read as stylized **Late Bronze Age / Mycenaean-inspired warfare**, not Classical Spartan and not medieval fantasy.

Historical authenticity defines the **visual vocabulary, not the proportions**.

Preferred language:
- bronze and leather armor;
- spears and large shields;
- simple crested helmets;
- strong faction cloth accents;
- readable bows/quivers for archers;
- exaggerated cartoon silhouettes at gameplay camera distance;
- oversized equipment where it improves role readability or comic staging;
- expressive faces and body poses.

The target is not "serious stylized realism". A player should read **Trojan War + cartoon adventure** immediately.

## Current gameplay-camera contract

Character production is designed for the **existing Chapter I tactical camera**, not for a separate close-up concept-art view.

The current baseline is an orthographic, top-down/isometric-like battlefield view with approximately `73°` pitch and a default orthographic size around `10.6`.

Implications:

- silhouette and equipment hierarchy matter more than small facial detail;
- helmets, crests, shields, bows, quivers, spears, capes and shoulder mass must remain legible from above;
- upper-body and head shapes deserve more visual emphasis than tiny lower-body costume details;
- faces should still carry personality in medium shots, but a role must never depend on seeing eyes, wrinkles or mouth detail;
- faction and role must remain readable while units overlap and move;
- final visual QA is performed from the actual gameplay camera before any close-up beauty review.

The intended 3D presentation is **stylized tactical board / toy battlefield**: compact, bold, readable forms with cartoon personality. It is not a mandate for realistic materials or highly detailed hero models.

## Proportion and shape rules

Characters may use:
- slightly oversized heads, helmets, shields, hands and weapons;
- broad chest/shoulder shapes for heavies and commanders;
- narrower, more elastic shapes for runners and skirmishers;
- simplified armor layering;
- strong asymmetry for heroes and bosses;
- readable capes, crests and shields as identity anchors.

Avoid:
- realistic body proportions as a mandatory default;
- tiny historically precise details that disappear at gameplay zoom;
- grim, dirty, desaturated realism;
- generic medieval plate armor;
- silhouette dependence on color alone;
- designs that only read in portrait/close-up framing.

## Character acting

Animation and posing should support adventure-comedy readability:
- strong anticipation before attacks;
- broad attack arcs;
- visible recoil and recovery;
- expressive hit, surprise, frustration and triumph reactions;
- controlled squash/stretch where rigs allow it;
- short physical-comedy beats that never delay gameplay state;
- hero and boss gestures that remain readable at normal gameplay zoom.

Gameplay timing remains authoritative. Animation may exaggerate presentation but must not misrepresent damage windows, attack timing or threat telegraphs.

## Faction identity

### Greeks / Achaeans
- cooler bronze and muted blue accents;
- disciplined, cleaner military silhouettes;
- heavy units read wider and more armored than regular infantry;
- commanders use stronger crest/cape/shield hierarchy;
- formation discipline can contrast humorously with exaggerated individual reactions when formations break.

### Trojans
- warmer bronze, dark red and gold accents;
- more regal guard/hero silhouettes;
- defensive formations should visually read as an organized city garrison rather than generic fantasy troops;
- heroes and guards may carry broader, more theatrical shapes than rank-and-file attackers.

## Chapter I silhouette contract

### Greek attackers
- **Infantry:** spear + round shield; simple mass-unit read.
- **Runner:** light body, smaller weapon, visibly faster and more elastic silhouette.
- **Heavy Hoplite:** heavier armor + large shield + spear; broad, top-heavy read.
- **Shield Bearer:** shield-dominant silhouette + spear; shield can be intentionally oversized for clarity.
- **Archer:** bow + quiver; no generic crossbow presentation.
- **Menelaus:** commander crest/cape, royal shield, one-handed command weapon; deliberately larger, theatrical commander silhouette.

### Trojan defenders
- **Trojan Infantry:** spear + round shield.
- **Trojan Guard:** large shield + spear, formation-oriented silhouette.
- **Trojan Archer:** bow + quiver.
- **Ballista Crew:** work/tool silhouette rather than frontline infantry; readable brace/heave/recoil poses.
- **Priest of Apollo:** robe + sun staff/disc; clear casting silhouette.
- **Fire Keeper:** thrown fire-bottle / ignition-equipment silhouette; expressive throw/stoke poses.

### Heroes
- **Hector:** long spear, large Trojan shield, red/gold hero crest and cape; heroic but approachable, not grim.
- **Menelaus:** colder bronze/blue commander treatment with a broader comic-command presence.
- **Achilles:** brighter heroic Greek treatment for later campaign use; exceptionally readable speed/elite silhouette.

## Small-scale readability priority

At default gameplay zoom, read order for a character is:

1. faction color family;
2. body mass / silhouette;
3. primary weapon or shield;
4. crest, cape or role prop;
5. current action pose;
6. facial personality;
7. decorative detail.

Any design that reverses this hierarchy is too detail-dependent for the current game.

## Equipment sources

The current candidate pipeline can use:
- pinned Quaternius CC0 bow source;
- pinned Quaternius CC0 spear source;
- project-authored Aegean round shield candidate;
- project-authored figure-eight/tower shield candidate;
- project-authored Dendra-inspired cuirass candidate;
- project-authored boar-tusk-inspired helmet candidate;
- KayKit accessories where appropriate.

These are **candidate/source assets**, not automatic final art. Source realism must be reshaped/stylized when necessary to satisfy the project direction.

## Production paths

Generated/promoted character candidates live under:

`Assets/Game/Art/Characters/Resources/TroyProduction/Characters`

with faction/role subfolders for Greek, Trojan and Heroes.

Final art remains subject to the `DONE` gate in `MODEL_ART_INVENTORY.md`.

## Runtime readability rules

1. Role must be identifiable primarily by silhouette/equipment, not color alone.
2. Faction tint is secondary to silhouette.
3. Threat hierarchy must remain readable after stylization.
4. Decorative equipment must not alter gameplay colliders.
5. Imported weapons must maintain sensible hand grip/orientation during combat animation.
6. Armor must be checked for clipping/deformation in locomotion, attack, hit and death states.
7. Hero and boss silhouettes must remain readable at normal gameplay zoom.
8. Comic acting must never obscure targetability, hit timing or navigation state.
9. Final judgement uses the current Chapter I gameplay camera before close-up inspection.

## Build / QA workflow

1. `git submodule update --init --recursive`
2. Build Chapter I candidates in Unity.
3. Run the Chapter I equipment/source pass.
4. Build role-specific Animator profiles.
5. Inspect in real Play Mode at gameplay camera distance.
6. Reject candidates that read as realistic/grim/generic, or only work in close-up, even if technically functional.
7. Only then promote status in `MODEL_ART_INVENTORY.md`.

The runtime may retain generated/procedural fallbacks while final production art is replaced incrementally.
