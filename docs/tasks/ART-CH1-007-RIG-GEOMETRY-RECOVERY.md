# ART-CH1-007 — Rig Geometry Recovery

## Goal
Generated Chapter I characters attach weapons, shields, armor and hero silhouette pieces to resolved rig bones at gameplay-readable proportions instead of silently falling back to the character root.

## Why
Play Mode QA showed oversized/misaligned shields, weapons and armor. The current builders can attach gear to `root.transform` when a hand/torso lookup fails, and authored meshes inherit placeholder scale even though source assets use different dimensions.

## Owner module
Editor art generation / character presentation.

## Allowed files
- `Assets/Editor/CartoonCharacterPrefabBuilder.cs`
- `Assets/Editor/ChapterOneProductionEquipmentBuilder.cs`
- `Assets/Editor/ChapterOneShieldCandidateBuilder.cs`
- `Assets/Editor/ChapterOneArmorCandidateBuilder.cs`
- `Assets/Editor/HectorProductionVisualRefinementBuilder.cs`
- `Assets/Editor/CartoonCharacterAutoBuilder.cs`
- new editor rig/geometry recovery utilities
- EditMode source-contract tests
- this task document

## Do not change
- gameplay stats, colliders, navigation or spawn logic
- Hector ability timing/damage contracts
- KayKit submodule contents/gitlink
- `Assets/Resources/Music/BeyazGiyme.mp3`

## Acceptance criteria
- no weapon/shield/critical armor builder silently falls back to the character root when a rig attachment bone is unresolved;
- rig lookup uses Humanoid bones when available and `SkinnedMeshRenderer.bones`/normalized transform names otherwise;
- authored spear/shield/armor sizes are normalized against actual character body extent rather than copied placeholder scale;
- existing generated Chapter I prefabs can be repaired without deleting/rebuilding the whole project;
- Hector spear and shield are attached to right/left hand respectively and remain hero-readable at gameplay zoom;
- generated-art validation reports unresolved/mis-scaled critical geometry;
- no gameplay contracts change.

## Automated validation
- EditMode/source-contract tests updated.
- architecture guard required locally.
- Unity Editor/Play Mode validation required locally before production acceptance.

## Manual validation
At 1920x1080 gameplay camera verify Hector, Greek/Trojan infantry, shield bearers and archers: no root-space floating gear, no torso-covering giant shield, no giant spear/bow, no obvious armor offset/clipping in Idle/Move/Attack/Block.

## Known risks
Static armor remains rigid bone-following candidate geometry, not skinned final production art. Pose-specific clipping can still require authored mesh/offset tuning after this recovery.

## Status
IN_PROGRESS
