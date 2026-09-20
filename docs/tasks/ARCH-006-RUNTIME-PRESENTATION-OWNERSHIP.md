# AI Task Contract

## Task ID
`ARCH-006-RUNTIME-PRESENTATION-OWNERSHIP`

## Goal
Classify and centralize the remaining runtime presentation lifecycle so self-install hooks exist only for true infrastructure.

## Final ownership

### Approved global runtime initializers
Only these files may use `[RuntimeInitializeOnLoadMethod]`:
- `GameBootstrap.cs`
- `CampaignSave.cs`
- `RuntimeFileLogger.cs`
- `BuildVersionOverlay.cs`

### Chapter I-owned
Owned by `ChapterOneRuntimeInstaller`:
- `HectorHUD`
- the Chapter I presentation stack defined by ARCH-005

### Menu-owned
Owned by `GameMenuController`:
- `MainMenuBackgroundOverride`
- `SimpleMainMenuPresentation`
- `MenuFlowStylePresentation`
- `MainMenuBuildVersionPresentation`

### Combat-HUD-owned
Owned by `ModernCombatHud`:
- `BossHUD`
- `CombatNotificationPresentation`
- `TroyCombatHudSkin`
- `VisualEncounterPreviewPresentation`
- `SelectedTowerContextPanelFollower`

## Acceptance criteria
- [x] Menu presenters no longer self-install.
- [x] Menu presenters are created and initialized by `GameMenuController`.
- [x] Combat HUD presenters no longer self-install.
- [x] Combat HUD presenters are created by `ModernCombatHud`.
- [x] `HectorHUD` no longer self-installs and remains Chapter I installer-owned.
- [x] Boss/menu/panel binding no longer performs direct scene search from Update/LateUpdate.
- [x] Architecture checker rejects runtime initializers outside the approved infrastructure list.
- [x] Architecture checker verifies menu/combat/chapter presentation ownership.

## Validation
- [x] Source-level inspection confirms owner-managed presenters have no `RuntimeInitializeOnLoadMethod`.
- [x] Source-level inspection confirms no direct `GameObject.Find` / `FindFirstObjectByType` in their Update/LateUpdate/FixedUpdate methods.
- [ ] Run `python tools/check-architecture.py` in a real checkout.
- [ ] Run Unity EditMode/PlayMode validation.
- [ ] Verify clean start, Pause/Resume and Restart Chapter I.

## Result
Runtime presentation creation is now explicit and hierarchical rather than distributed through independent auto-start hooks.

## Status
`IMPLEMENTED — UNITY VALIDATION PENDING`
