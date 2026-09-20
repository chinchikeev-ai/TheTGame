# AI Task Contract

## Task ID
`ARCH-004-RUNTIME-LOOKUP-HARDENING`

## Goal
Remove unbounded runtime scene scans and prevent scene lookup from reappearing in per-frame gameplay/presentation paths.

## Why
The architecture rules prohibited scene-wide gameplay searches, but the static guard only detected `FindObjectsByType` / `FindObjectsOfType`. Several presentation/UI systems also used `GameObject.Find` or `FindFirstObjectByType` from `Update` / `LateUpdate`, creating hidden name coupling and repeated scene traversal.

## Owner module
Primary owner: `Assets/Game/Core` architecture/runtime composition, with targeted owner APIs in Enemies, Towers and UI.

## Allowed files
- `tools/check-architecture.py`
- `AGENTS.md`
- `docs/ARCHITECTURE.md`
- `docs/PROJECT_STATUS.md`
- `docs/TODO.md`
- `Assets/Game/Enemies/EnemySpawner.cs`
- `Assets/Game/Towers/TowerPlacement.cs`
- `Assets/Game/UI/GameMenuController.cs`
- `Assets/Game/UI/ModernCombatHud.cs`
- Chapter I UI/presentation consumers that were performing hot-path scene lookup
- `Assets/Game/World/ChapterOneAegeanSeaPresentation.cs`
- this task contract

## Do not change
- Encounter composition or balance.
- Save/campaign schema.
- Input semantics.
- Production-art acceptance.
- User-owned music assets.

## Acceptance criteria
- [x] Runtime `FindObjectsByType` / `FindObjectsOfType` violation removed from Chapter I Aegean presentation.
- [x] Repeated `GameObject.Find` / `FindFirstObjectByType` calls removed from known `Update` / `LateUpdate` paths.
- [x] Frequently accessed runtime owners expose stable references for EnemySpawner, TowerPlacement, GameMenuController and ModernCombatHud.
- [x] `ChapterOneUiCompactPresentation` performs bounded startup binding instead of scene scans every `LateUpdate`.
- [x] Architecture checker rejects direct scene lookup inside `Update`, `LateUpdate` and `FixedUpdate`.
- [x] Architecture rules explicitly distinguish forbidden runtime-wide scans from bounded startup/bootstrap lookup.
- [x] Existing gameplay behavior remains unchanged.

## Automated validation
- [x] Source-level inspection confirms no direct scene-search token remains in the modified hot-path methods.
- [x] Existing hard ban for `FindObjectsByType` / `FindObjectsOfType` remains in the checker.
- [ ] Run `python tools/check-architecture.py` in a local checkout.
- [ ] Run Unity EditMode/PlayMode validation.

## Manual validation
- Chapter I starts normally and all five encounters run.
- Pause/settings/resume keep menu and HUD references valid.
- Restart Chapter I does not leave stale runtime owner instances.
- Aegean legacy water is still hidden and the replacement sea renders correctly.
- Compact HUD presentation applies after runtime UI creation.

## Known risks
Name-based startup binding remains transitional debt in several presentation systems. Multiple `RuntimeInitializeOnLoadMethod` auto-create components also remain and should be consolidated separately through composition ownership rather than hidden behind a generic helper.

## Result
The current hard scene enumeration violation was replaced with hierarchy-scoped traversal. Core runtime owners now expose stable references used by hot-path consumers. The static architecture guard now enforces the per-frame lookup rule that was previously documentation-only.

## Status
`IMPLEMENTED — UNITY VALIDATION PENDING`
