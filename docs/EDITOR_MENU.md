# Unity Editor Menu Contract

Last reviewed: 2026-09-15

## Purpose

The project must expose its custom Unity Editor commands through one predictable top-level menu. This avoids duplicated commands, inconsistent navigation, and AI-generated menu drift.

## Canonical root

The only canonical top-level root for project-owned Unity Editor commands is:

`The Troy Game/`

Examples:

- `The Troy Game/Art/Build Missing Chapter I Art`
- `The Troy Game/Characters/Build Chapter I Production Candidates`
- `The Troy Game/Validation/Validate Chapter I Release Candidate`
- `The Troy Game/Data/Create Missing Default Assets`
- `The Troy Game/Build/Windows Visible Map`
- `The Troy Game/Sync/Check origin main`

## Forbidden roots

Do not add new project commands under any of these forms:

- `TheTroyGame/...`
- `Tools/TheTroyGame/...`
- `Tools/The Troy Game/...`
- another new top-level project name for the same commands

The product/repository name may still appear as `TheTroyGame` in class names, asmdefs, namespaces/policies, paths, or internal identifiers. This contract applies specifically to Unity Editor menu paths declared with `MenuItem`.

## Current transition state

The visible Unity menu is already unified under `The Troy Game`.

Some older validation/data source files still contain legacy `[MenuItem("TheTroyGame/...")]` declarations. `Assets/Editor/UnifiedEditorMenuCompatibility.cs` currently:

1. exposes canonical aliases under `The Troy Game/...`;
2. removes the legacy `TheTroyGame` menu entries after Editor load;
3. preserves the existing implementation methods without changing gameplay or validation behavior.

This compatibility layer is temporary technical debt. New code must not add additional legacy declarations. When the remaining legacy declarations are migrated directly in their owner files, remove the compatibility aliases and removal reflection in the same change.

## Canonical categories

Use these second-level groups unless a clearly better existing category already owns the command:

- `The Troy Game/Art/...` — generated/imported art recovery and art validation.
- `The Troy Game/Characters/...` — character, equipment, animation and campaign art builders.
- `The Troy Game/Validation/...` — release, architecture, gameplay, art-freeze and audit checks.
- `The Troy Game/Data/...` — authored/default data maintenance commands.
- `The Troy Game/Build/...` — player/build commands.
- `The Troy Game/Sync/...` — repository/editor synchronization checks.

Do not create parallel categories that differ only by spelling, spacing, or placement under Unity's generic `Tools` menu.

## Implementation rules

For every new `MenuItem`:

1. Start the path with `The Troy Game/`.
2. Reuse an existing second-level category where practical.
3. Keep menu labels action-oriented and specific.
4. Do not duplicate an existing command under a second path.
5. If a command is batchmode-only and should not be user-facing, do not add a `MenuItem` just for discoverability.
6. Renaming a menu path must not change the underlying gameplay/data behavior.

## Validation

`Assets/Tests/EditMode/UnifiedEditorMenuCompatibilityTests.cs` scans all C# files under `Assets/Editor` and guards the menu contract.

The test must fail when:

- a `Tools/TheTroyGame/...` declaration appears;
- a `Tools/The Troy Game/...` declaration appears;
- a new `TheTroyGame/...` legacy declaration exists but is not explicitly covered by the compatibility layer;
- the compatibility layer stops publishing the canonical `The Troy Game/` root while legacy declarations still exist.

When the legacy declarations reach zero, simplify the test to forbid `TheTroyGame/...` outright and delete `UnifiedEditorMenuCompatibility.cs`.

## Manual QA

After menu-path changes, reload/compile the Unity Editor and confirm:

- only one project top-level menu is visible: `The Troy Game`;
- no `Tools -> TheTroyGame` or `Tools -> The Troy Game` project submenu remains;
- no separate top-level `TheTroyGame` remains;
- expected commands are reachable under the canonical categories;
- invoking a moved command still executes the same underlying method.

Source-level checks do not replace this real Unity Editor menu inspection.
