# AI Task Contract

## Task ID
`ART-003-RUNTIME-VISUAL-BINDING`

## Goal
Make Chapter I show the approved Trojan role silhouettes in the actual gameplay runtime even when generated/production character prefabs have not been built locally.

## Why
The current `main` runtime attempts to load production/generated character prefabs from `Resources`, but those generated prefab files are not tracked in the repository. On a clean/current checkout the load therefore falls back or silently omits tower crews, so editor-only visual passes are not visible in the real game.

## Owner module
`Assets/Game/Towers` for defensive-unit runtime presentation. Character source auditing is shared by `Assets/Game/Characters`, `Assets/Game/Heroes`, and `Assets/Game/Enemies`.

## Allowed files
- `Assets/Game/Towers/TowerArtDirector.cs`
- `Assets/Game/Towers/TrojanTowerCrewFallbackFactory.cs`
- `Assets/Game/Characters/RuntimeVisualAudit.cs`
- `Assets/Game/Heroes/HeroVisualFactory.cs`
- `Assets/Game/Enemies/EnemyVisualFactory.cs`
- `Assets/Tests/EditMode/RuntimeVisualBindingTests.cs`
- `docs/PROJECT_STATUS.md`
- this task contract

## Do not change
- gameplay camera
- routes/build grid
- tower balance, damage, cadence or costs
- enemy balance/spawn composition
- gameplay colliders
- public combat APIs

## Inputs / source of truth
- current Chapter I gameplay camera is canonical
- `docs/ART_BIBLE.md`
- `docs/CHARACTER_ART_DIRECTION.md`
- `docs/TROJAN_UNIT_VISUAL_BIBLE.md`
- `docs/MODEL_ART_INVENTORY.md`

## Acceptance criteria
- [x] Runtime first tries the production Trojan crew resource, then generated resource.
- [x] When both are missing, Archer/Spearman/Guard tower crews receive distinct procedural runtime silhouettes instead of disappearing.
- [x] Procedural fallback crew has no gameplay colliders.
- [x] Runtime records once-per-role visual source information in the existing runtime log.
- [x] Hector and Greek enemy factories record whether production, generated, or procedural fallback art is actually used.
- [x] No gameplay/balance values are changed.
- [ ] No architecture guard violations.

## Automated validation
- [ ] `python tools/check-architecture.py`
- [x] EditMode contract test added for role-specific collider-free Trojan fallback crew.
- [ ] PlayMode visual-source and camera inspection.
- [ ] Full project validation when Unity is available.

## Manual validation
1. Update to current `main` and enter Chapter I.
2. Build at least Archer, Spear Wall and Trojan Guard defenses.
3. Verify each now visibly contains role-specific Trojan figures from the normal gameplay camera.
4. Check the latest persistent-data runtime log for `[ART_SOURCE]` records showing the source actually used.
5. Capture a screenshot with the in-game screenshot function and compare silhouettes.

## Known risks
Procedural fallback art remains `PROCEDURAL`; it is a reliable runtime presentation layer, not final production art. Final authored prefabs should automatically supersede it when present under the existing resource paths.

## Result
Root cause confirmed: production/generated resource folders referenced by runtime do not contain tracked character prefabs in current `main`, while `TowerArtDirector.AddProductionCrew` previously returned without adding a crew when both loads failed. This change provides an explicit runtime fallback and source audit instead of a silent empty binding.

## Status
`IN_PROGRESS`
