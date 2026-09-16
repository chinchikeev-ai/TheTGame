# ART-CH1-006 - URP Material Recovery

## Goal
Generated Chapter I character prefabs and runtime-created character instances must not render magenta in the URP runtime when KayKit source materials use unsupported/non-URP shaders.

## Why
Repeated Play Mode screenshots showed Greek and Trojan character meshes remaining magenta while project-owned procedural URP parts rendered correctly. Runtime-only recovery was insufficient because the character builder persisted cloned KayKit materials into generated prefabs. The durable fix is therefore asset-level: generated character renderer slots must reference project-owned materials derived from the known-good `RuntimeColorMaterial`, with runtime conversion retained only as a safety net.

## Owner module
Editor art pipeline plus runtime character presentation/material finalization.

## Allowed files
- Assets/Editor/ChapterOneUrpMaterialRepair.cs
- Assets/Editor/CartoonCharacterPrefabBuilder.cs
- Assets/Editor/CartoonCharacterAutoBuilder.cs
- Assets/Game/World/CharacterUrpMaterialAdapter.cs
- Assets/Game/Heroes/HeroVisualFactory.cs
- Assets/Game/Enemies/EnemyVisualFactory.cs
- Assets/Game/Towers/TowerFactory.cs
- Assets/Game/Towers/TowerProductionArtBinder.cs
- Assets/Tests/EditMode/ChapterOneGeneratedArtRecoveryTests.cs
- this task document

## Do not change
Gameplay balance, colliders, navigation, animation timing, KayKit submodule contents, or Assets/Resources/Music/BeyazGiyme.mp3.

## Acceptance criteria
- New generated character prefabs do not persist KayKit/external shaders in renderer material slots.
- Existing generated character prefabs can be repaired without full regeneration.
- Each repaired renderer slot points to a deterministic project-owned material under `Assets/Game/Art/Characters/Resources/TroyProduction/Materials`.
- Stable materials use the same shader as `Assets/Resources/RuntimeColorMaterial.mat` and preserve source texture/UV transform/base color when available.
- Bright Unity-error magenta is never copied forward as a base color.
- Build Missing Chapter I Art invokes URP material repair automatically.
- Runtime factories retain final material recovery as a safety net after hero/enemy/tower-crew assembly.
- Validation reports missing adapter, missing materials, external/non-stabilized materials and shader mismatches.
- Final Play Mode check shows no magenta Chapter I character meshes.

## Validation
Source-contract regression tests were updated, but Unity Editor, EditMode, Play Mode and build were not executed in this environment. Local visual validation is still required.

## Status
IN_PROGRESS
