# ART-001 — Hector visual rework

## Status

IMPLEMENTED — AWAITING UNITY PLAY MODE QA

## Goal

Bring Hector's Chapter I production candidate and runtime fallback into the approved Trojan unit visual direction: a clearly heroic, fully armoured silhouette with a long spear, a large round Trojan shield, a strong red/bronze/gold faction read, and a readable red crest/cape from the gameplay camera.

## Design source of truth

- `docs/TROJAN_UNIT_VISUAL_BIBLE.md`
- `docs/TROJAN_UNIT_PRODUCTION_SHEETS.md`
- `docs/CHARACTER_ART_DIRECTION.md`
- `docs/CARTOON_CHARACTER_PIPELINE.md`

## Implemented

- Hector's production shield binding now uses the large authored round shield candidate rather than the square/figure-eight family.
- The production shield gets a restrained horse emblem; the horse motif is not repeated across the full costume.
- The procedural Hector fallback now has a dedicated large round shield, long spear, red cape and full-body armour cues across torso, shoulders, forearms, knees and greaves.
- `HeroSignatureArt` no longer doubles the horsehair crest when the underlying generated/source candidate already contains one.
- The Trojan core-unit visual identity pass gives Hector a stronger hero body read while preserving root gameplay collider authority.
- No combat timing, ability logic, map layout or gameplay API was intentionally changed.

## Out of scope

- Final authored/skinned Hector mesh.
- New combat animations or timing changes.
- Hand IK/grip tuning.
- Map/environment rework.
- Promotion of Hector to `DONE` before real Unity Play Mode visual QA.

## Acceptance criteria

1. Hector reads differently from the standard Trojan infantry at the Chapter I gameplay camera.
2. Hector is visibly armoured across torso, shoulders, forearms and lower legs.
3. Hector carries a long spear and a large round shield.
4. The round shield carries a simple readable horse emblem.
5. Red cape and crest remain strong top-down faction/hero markers.
6. Decorative visual parts add no gameplay colliders.
7. Existing hero gameplay systems and public APIs are unchanged.
8. Production shield binding uses the authored round shield candidate for Hector.
9. Existing generated/source crest is not doubled by `HeroSignatureArt`.
10. Final status remains candidate/placeholder until Unity Play Mode QA passes.

## Files implemented

- `Assets/Game/Characters/RuntimeWarriorVisualFactory.cs`
- `Assets/Game/Characters/HeroSignatureArt.cs`
- `Assets/Editor/ChapterOneShieldCandidateBuilder.cs`
- `Assets/Editor/TrojanCoreUnitVisualPass.cs`

## Validation completed without Unity

- Presentation additions use collider-free decorative primitives.
- Root Hector gameplay collider values were not changed by the visual pass.
- Shield candidate binding remains a presentation/editor pipeline concern.
- Duplicate-crest prevention excludes the `HeroSignatureArt` subtree itself.

## Remaining acceptance work

- Rebuild Chapter I production candidates in a real Unity Editor checkout.
- Run EditMode/PlayMode tests.
- Inspect shield face/emblem orientation, armour clipping, spear grip and crest/cape silhouette in the actual Chapter I gameplay camera at target 16:9 framing.
- Tune only presentation offsets/scales after that inspection.
- Do not promote Hector to `DONE` until the repository's art acceptance gate is satisfied.
