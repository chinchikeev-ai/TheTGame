# ART-001 — Hector visual rework

## Status

IN PROGRESS

## Goal

Bring Hector's Chapter I production candidate and runtime fallback into the approved Trojan unit visual direction: a clearly heroic, fully armoured silhouette with a long spear, a large round Trojan shield, a strong red/bronze/gold faction read, and a readable red crest/cape from the gameplay camera.

## Design source of truth

- `docs/TROJAN_UNIT_VISUAL_BIBLE.md`
- `docs/TROJAN_UNIT_PRODUCTION_SHEETS.md`
- `docs/CHARACTER_ART_DIRECTION.md`
- `docs/CARTOON_CHARACTER_PIPELINE.md`

## Scope

- Correct Hector's generated shield family from figure-eight/square to a large round Trojan shield.
- Give the procedural runtime fallback a dedicated Hector silhouette rather than the generic hero body kit.
- Add full-body armour cues to the fallback so chest, shoulders, forearms, knees and lower legs read as armoured at gameplay scale.
- Put the approved horse motif on the shield rather than repeating it across the costume.
- Keep the long spear, red cape and red horsehair crest as primary hero-read elements.
- Prevent duplicate hero crests when generated/source art already contains one.
- Keep gameplay logic, collider authority, combat timings and Chapter I map layout unchanged.

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
4. The round shield carries a simple readable horse emblem in the procedural fallback.
5. Red cape and crest remain strong top-down faction/hero markers.
6. Decorative visual parts add no gameplay colliders.
7. Existing hero gameplay systems and public APIs are unchanged.
8. Production shield binding uses the authored round shield candidate for Hector.
9. Existing generated/source crest is not doubled by `HeroSignatureArt`.
10. Final status remains candidate/placeholder until Unity Play Mode QA passes.

## Planned files

- `Assets/Game/Characters/RuntimeWarriorVisualFactory.cs`
- `Assets/Game/Characters/HeroSignatureArt.cs`
- `Assets/Editor/CartoonCharacterPrefabBuilder.cs`
- `Assets/Editor/ChapterOneShieldCandidateBuilder.cs`
- documentation/status notes as needed

## Validation plan

- Review the generated hierarchy/names and collider ownership statically.
- Run `python tools/check-architecture.py` when a local checkout is available.
- Rebuild Chapter I production candidates in Unity.
- Run EditMode/PlayMode tests.
- Inspect Hector in the actual Chapter I gameplay camera at target 16:9 framing before production acceptance.
