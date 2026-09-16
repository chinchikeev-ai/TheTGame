# DX-BUILD-001 — Build Stamp and Runtime Visual Audit

## Goal
Restore a visible build identity in packaged Windows builds without mutating `ProjectSettings.asset`, and add precise runtime diagnostics for magenta Chapter I character visuals.

## Why
`BuildVersionInfo` can read a player-side `build_version.stamp`, and `BuildVersionStampEditor` already has pre/post-build stamp support, but the redesigned menu no longer has a dedicated runtime badge using `BuildVersionInfo.CompactMenuBadge`. The Windows command also now writes the final player-side stamp explicitly after a successful build as a clean-tree-safe fallback. Separately, repeated shader/material recovery passes have not removed magenta character visuals, so the next build must log the actual visual source and renderer/material/shader/property-block state used at runtime rather than applying further speculative fixes.

## Scope
- `Assets/Editor/BuildPlayerCommand.cs`
- `Assets/Game/Core/BuildVersionOverlay.cs`
- `Assets/Game/Characters/RuntimeVisualAudit.cs`
- `Assets/Game/Heroes/HeroVisualFactory.cs`
- `Assets/Game/Enemies/EnemyVisualFactory.cs`
- source-contract tests
- this task document

## Do not change
- gameplay balance, combat, navigation, colliders
- KayKit submodule contents/gitlink
- `Assets/Resources/Music/BeyazGiyme.mp3`
- `ProjectSettings.asset` version fields during build

## Acceptance criteria
- Windows visible-map build writes `build_version.stamp` to `<exe>_Data/build_version.stamp` after a successful build;
- stamp value is produced from current git branch + SHA via `BuildVersionInfo.ComposeStampedVersion`;
- build does not modify `ProjectSettings.asset` for version stamping;
- menu/pause states show a compact top-left build badge sourced from `BuildVersionInfo.CompactMenuBadge`;
- combat HUD is not covered by the build badge;
- runtime visual audit records visual source plus renderer name/type, mesh name, material name, shader name/support state, base texture name, base color, and material-property-block color overrides;
- hero and enemy factories call the detailed audit after final visual assembly/material repair;
- duplicate audit spam is bounded;
- no gameplay contract changes.

## Validation
Source-level contract coverage added. Unity Editor, player build and Play Mode were not executed in this environment. The user's normal update/build script performs the actual local Unity build. Runtime logs from the resulting player are the acceptance evidence for the visual diagnosis.

## Status
IMPLEMENTED_SOURCE_QA_PENDING
