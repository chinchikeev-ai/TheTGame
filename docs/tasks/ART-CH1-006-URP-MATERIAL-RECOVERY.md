# ART-CH1-006 - URP Material Recovery

## Goal
Generated Chapter I character prefabs must not render magenta in the URP runtime when KayKit source materials use non-URP shaders.

## Why
Play Mode shows multiple Greek and Trojan character meshes as magenta while project-owned procedural URP parts render correctly.

## Owner module
Editor art pipeline plus a small runtime presentation adapter under Assets/Game/World.

## Allowed files
- Assets/Editor/ChapterOneUrpMaterialRepair.cs
- Assets/Editor/CartoonCharacterAutoBuilder.cs
- Assets/Game/World/CharacterUrpMaterialAdapter.cs
- this task document

## Do not change
Gameplay balance, colliders, navigation, animation timing, KayKit submodule contents, or Assets/Resources/Music/BeyazGiyme.mp3.

## Acceptance criteria
- Existing generated character prefabs can receive URP material recovery without full regeneration.
- Build Missing Chapter I Art installs recovery automatically.
- URP materials remain unchanged.
- Non-URP materials are converted at runtime while preserving base texture/color when available.
- Validation reports prefabs missing the adapter.
- Final Play Mode check shows no magenta Chapter I character meshes.

## Validation
Unity Editor, EditMode, Play Mode and build were not executed in this environment. Local visual validation is still required.

## Status
IN_PROGRESS
