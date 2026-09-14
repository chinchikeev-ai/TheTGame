# ART-002 — Trojan Chapter I roster visual pass

## Status

IMPLEMENTED IN GENERATION PIPELINE — AWAITING UNITY PLAY MODE QA

## Goal

Make the approved Chapter I Trojan roster readable as distinct characters and combat roles rather than the same body with different equipment, while preserving all existing gameplay logic and collider authority.

## Design source of truth

- `docs/TROJAN_UNIT_VISUAL_BIBLE.md`
- `docs/TROJAN_UNIT_PRODUCTION_SHEETS.md`
- `docs/CHARACTER_ART_DIRECTION.md`

## Implemented role differentiation

### Hector

- heroic large body read;
- full armour cues;
- long spear;
- large round Trojan shield with restrained horse emblem;
- red cape and horsehair crest;
- duplicate crest prevention.

### Trojan Infantry / Spearman

- standard-soldier proportions retained as the roster baseline;
- simple disciplined face markers;
- spear + round shield remain the primary role silhouette.

### Trojan Archer

- visual body narrowed and lengthened without changing the root gameplay collider;
- lighter head treatment;
- bow/quiver remain dominant top-down markers.

### Trojan Guard

- body made broader and slightly more squat than infantry;
- heavier face/beard and neck-guard cues;
- tower/figure-eight shield remains the dominant tank silhouette.

### Priest of Apollo

- substantially shorter and thinner visual body;
- soldier armour hidden for the support candidate;
- old bald head, long grey beard, age brow and long nose;
- white/gold robe and sun staff provide support-role readability.

### Fire Keeper / Fire Thrower

- pitch-pot/ladle presentation replaced by flaming-bottle language;
- bottle satchel plus multiple bottles, including a lit throwing bottle;
- manic eyes, pupils, brows and wild hair create the approved pyromaniac face read;
- base soldier armour hidden for the support candidate.

### Ballista Crew

- crew split into two visual characters while keeping the compatibility prefab:
  - lean/nervous engineer with winch/measuring tools;
  - broader loader with heavy bolt bundle and beard;
- tower production binder now uses Engineer + Loader when available and falls back to the legacy shared crew prefab otherwise.

### Cyclops

- club removed from the generated concept direction;
- oversized irregular boulder becomes the primary weapon/role marker;
- single eye moved to the face side and enlarged;
- extra asymmetrical shoulder mass reinforces a giant-monster silhouette.

## Safety / architecture constraints

- Core-unit body differentiation operates on the child `Visual` / presentation kit rather than changing the root gameplay collider.
- Added procedural presentation primitives remove their colliders immediately.
- Ballista crew instances continue to have colliders/rigidbodies disabled when attached as tower decoration.
- No tower damage, enemy stats, hero abilities, routes, encounter timings or map layout were intentionally changed.

## Files

- `Assets/Editor/TrojanCoreUnitVisualPass.cs`
- `Assets/Editor/MythicAndSupportArtCandidateBuilder.cs`
- `Assets/Editor/ChapterOneShieldCandidateBuilder.cs`
- `Assets/Game/Characters/RuntimeWarriorVisualFactory.cs`
- `Assets/Game/Characters/HeroSignatureArt.cs`
- `Assets/Game/Towers/TowerProductionArtBinder.cs`

## Required Unity rebuild order

1. `The Troy Game/Characters/Build Chapter I Production Candidates`
2. `The Troy Game/Characters/Apply Trojan Unit Visual Identity Pass`
3. `The Troy Game/Characters/Bind Late Bronze Age Shield Candidates`
4. `The Troy Game/Characters/Build Mythic and Trojan Support Candidates`
5. run the existing Chapter I animation/equipment builders as required by the normal production pipeline

`Build Mythic and Trojan Support Candidates` also reapplies the core Trojan identity pass before cloning support characters.

## Remaining acceptance work

The code/generation pass is not equivalent to final production art. Before any unit is promoted to `DONE`:

- rebuild candidates in a real Unity Editor checkout;
- run EditMode/PlayMode tests;
- inspect all units together at Chapter I gameplay camera distance and target 16:9 framing;
- verify face overlays, shield orientation, weapon grip, animation clipping and top-down silhouette;
- tune presentation-only offsets/scales after inspection;
- replace generated/procedural elements with final authored/skinned art where required by `docs/MODEL_ART_INVENTORY.md` acceptance rules.
