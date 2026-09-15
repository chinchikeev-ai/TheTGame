# ART-CH1-006 - URP Material Recovery

## Goal
Generated Chapter I character prefabs and runtime-created character instances must not render magenta in the URP runtime when KayKit source materials use unsupported/non-URP shaders.

## Why
Play Mode shows multiple Greek and Trojan character meshes as magenta while project-owned procedural URP parts render correctly. The first recovery pass installed an adapter on generated prefabs, but a later Play Mode screenshot still showed magenta. Runtime factories finish assembling/enhancing heroes, enemies and tower crews after prefab instantiation, so relying only on `Awake()` is insufficient. A shader can also carry an URP-looking name while being unsupported in the active runtime.

## Owner module
Editor art pipeline plus runtime character presentation/material finalization.

## Allowed files
- Assets/Editor/ChapterOneUrpMaterialRepair.cs
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
- Existing generated character prefabs can receive URP material recovery without full regeneration.
- Build Missing Chapter I Art installs recovery automatically.
- Runtime factories execute a final material pass after hero/enemy/tower-crew assembly.
- Supported URP materials remain unchanged.
- Unsupported or non-URP materials are converted at runtime while preserving base texture/color when available.
- Runtime visual audit can show when material repair actually replaced materials.
- Validation reports prefabs missing the adapter.
- Final Play Mode check shows no magenta Chapter I character meshes.

## Validation
Source-contract regression tests were updated, but Unity Editor, EditMode, Play Mode and build were not executed in this environment. Local visual validation is still required.

## Status
IN_PROGRESS
