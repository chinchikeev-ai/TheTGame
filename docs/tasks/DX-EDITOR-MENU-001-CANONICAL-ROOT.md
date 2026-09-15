# DX-EDITOR-MENU-001 — Canonical Unity Editor Menu Root

## Goal
Keep every project-owned Unity Editor command discoverable under one visible top-level root: `The Troy Game`.

## Why
The project accumulated three menu naming schemes: `The Troy Game/...`, `TheTroyGame/...`, and commands nested under Unity's generic `Tools` menu. This produced duplicate navigation and made AI-generated editor tooling inconsistent.

## Owner module
Editor / developer experience (`Assets/Editor`, `Assets/Tests/EditMode`, documentation rules).

## Allowed files
- `Assets/Editor/*` menu declarations/compatibility only
- `Assets/Tests/EditMode/UnifiedEditorMenuCompatibilityTests.cs`
- `AGENTS.md`
- `docs/README.md`
- `docs/EDITOR_MENU.md`
- this task contract

## Do not change
- gameplay runtime
- balance/data values
- art assets or acceptance manifests
- `Assets/Resources/Music/BeyazGiyme.mp3`
- Unity CI trigger policy

## Inputs / source of truth
- current `main`
- all `[MenuItem]` declarations under `Assets/Editor`
- `docs/EDITOR_MENU.md`

## Acceptance criteria
- [x] Canonical visible root documented as `The Troy Game/`.
- [x] `Tools/TheTroyGame/...` and `Tools/The Troy Game/...` are forbidden for project-owned commands.
- [x] Legacy top-level `TheTroyGame/...` declarations are documented as temporary compatibility debt, not a valid pattern for new code.
- [x] Existing remaining legacy declarations are covered by `UnifiedEditorMenuCompatibility` so the Unity UI exposes canonical aliases and removes the legacy root.
- [x] EditMode source-contract test scans all `Assets/Editor/**/*.cs` literal `MenuItem` declarations and rejects Tools legacy roots.
- [x] EditMode source-contract test rejects any new `TheTroyGame/...` declaration that is not explicitly covered by the compatibility layer.
- [x] `AGENTS.md` no longer documents the old `TheTroyGame/Data/...` path.
- [x] Documentation index links the canonical editor-menu contract.

## Audit result
Source review on 2026-09-15 found:

- canonical art/campaign/character/build/sync commands already using `The Troy Game/...` after the first unification pass;
- no intended project command should remain under `Tools/TheTroyGame/...` or `Tools/The Troy Game/...`;
- 20 legacy `TheTroyGame/...` validation/data menu paths are explicitly listed in `UnifiedEditorMenuCompatibility.LegacyMenuPaths` and hidden/re-exposed under the canonical root at Editor load;
- the old data menu spelling was still present in `AGENTS.md` and was corrected.

The 20 legacy source declarations are technical debt. They should be migrated directly in their owner files in a later cleanup, after which `UnifiedEditorMenuCompatibility.cs` can be deleted and the test tightened to forbid all `TheTroyGame/...` paths outright.

## Automated validation
- [ ] `python tools/check-architecture.py` — not executed in this GitHub-only environment.
- [ ] Unity EditMode tests — not executed; test source updated.
- [ ] Unity PlayMode tests — not required for a menu/documentation contract change.
- [ ] Full Unity validation — not executed.

## Manual validation
After `git pull` and Unity script reload:

- [ ] one project top-level menu: `The Troy Game`;
- [ ] no separate top-level `TheTroyGame`;
- [ ] no project submenu under `Tools -> TheTroyGame`;
- [ ] no project submenu under `Tools -> The Troy Game`;
- [ ] validation/data/art/build/sync commands remain callable.

## Known risks
`UnifiedEditorMenuCompatibility` uses Unity editor reflection to remove legacy menu items. Source-level coverage prevents untracked legacy additions, but actual menu rendering still requires a real Unity Editor reload for final visual confirmation.

## Result
- Added `docs/EDITOR_MENU.md` as the canonical menu contract.
- Updated `AGENTS.md` and `docs/README.md`.
- Strengthened `UnifiedEditorMenuCompatibilityTests` to scan the whole editor source tree.
- Preserved current `main` UI work and did not touch gameplay/art/music.

## Status
DONE at source-contract/documentation level; Unity Editor visual verification pending.
