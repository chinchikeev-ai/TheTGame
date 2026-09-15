# AI Task Contract

## Task ID
`UX-005-UNIFIED-COMBAT-HUD`

## Goal
Bring the remaining Chapter I combat HUD into the same dark-stone, bronze-framed, portrait-led visual language as the lower-left Hector block.

## Why
The Hector block reads as an intentional game interface while several other runtime HUD regions still look like unrelated prototype panels. Divine Power was also represented by competing controls before this cutover.

## Owner module
Primary owner: `Assets/Game/UI`.

## Allowed files
- `Assets/Game/UI/ModernCombatHud.cs`
- `Assets/Game/UI/CombatHudUiFactory.cs`
- `Assets/Game/UI/TroyCombatHudSkin.cs`
- `Assets/Game/UI/EnemyInspectorPresentation.cs`
- `Assets/Game/UI/CombatNotificationPresentation.cs`
- `Assets/Game/UI/BossHUD.cs`
- `Assets/Game/UI/ChapterOneGuidancePresentation.cs`
- `Assets/Game/UI/VisualEncounterPreviewPresentation.cs`
- `Assets/Game/UI/GameMenuController.cs` only for the existing Settings-owner entry point used by the combat HUD
- `Assets/Game/Campaign/Runtime/ChapterOneRuntimeInstaller.cs`
- `Assets/Tests/PlayMode/CombatHudLayoutUxTests.cs`
- `docs/PROJECT_STATUS.md`
- `docs/TODO.md`
- this task contract

## Do not change
- Combat, economy, encounter, tower or Hector gameplay behavior.
- Patron selection timing.
- Save schema or campaign progression.
- Production-art status.

## Inputs / source of truth
- `docs/ART_BIBLE.md`, UI direction.
- `docs/GDD.md`, combat HUD requirements.
- `docs/VISUAL_OWNERSHIP.md`.
- Existing `HectorHUD` visual language.

## Acceptance criteria
- [x] Main HUD panels use the same sliced Trojan panel art as Hector.
- [x] Gold, combat speed and compact Settings control share one top-left utility cluster; Gate health remains directly below.
- [x] Encounter center is reserved for encounter/progress/start state rather than utility controls.
- [x] Resources and encounter state use semantic portrait/icon anchors.
- [x] Divine Power has one clear, direct control with effect/readiness copy.
- [x] Defender cards are portrait-led with larger role art, hotkey, localized name and Gold cost.
- [x] Hover details and selected-defense context use larger defender portraits and clearer hierarchy.
- [x] Enemy inspector uses the same panel/portrait hierarchy with a materially larger enemy portrait.
- [x] Menelaus boss block uses the dedicated illustrated Menelaus portrait and a stronger boss/role/HP hierarchy.
- [x] Contextual tutorial/advice uses the existing Hector parchment art rather than another system-stone panel.
- [x] Next-encounter enemy cards use larger readable portraits while staying outside the centered progress region.
- [x] Combat notifications stay compact, transient and outside Hector's lower-left block.
- [x] Legacy `MagicToggle`, `MagicFlyout`, `CombatActions` and in-combat Patron selection are absent.
- [x] Guidance is kept below the top-left resource/gate cluster.
- [x] Objective copy does not repeat encounter progress and Gate HP already visible in the primary HUD.
- [x] Enemy inspector yields to blocking menus.
- [x] Settings remains owned by `GameMenuController`; `ModernCombatHud` only invokes its public combat-settings command.
- [x] RU/EN content keeps existing responsive target containers.
- [x] Existing gameplay contracts are preserved.

## Automated validation
- [x] Architecture guard passed on the original `b8f03e2` HUD implementation/merge baseline.
- [x] PlayMode layout contracts were extended for single Divine Power ownership, semantic portraits, perimeter anchors and non-competing HUD zones.
- [x] Second visual-pass PlayMode contracts now cover top-left utility ownership, absence of speed controls from encounter center, portrait-led defender/selected/enemy/boss cards and parchment tutorial presentation.
- [ ] Re-run `git diff --check` and `python tools/check-architecture.py` for this follow-up tree when a local checkout is available.
- [ ] Re-run EditMode and PlayMode tests for this follow-up tree when Unity is available.
- [ ] Run full `tools/validate-project.ps1` / `tools/validate-project.sh` and Windows build when Unity is available.

## Manual validation
Required real Play Mode matrix remains:
- 1920x1080 RU;
- 1920x1080 EN;
- 1376x768 RU;
- 1376x768 EN;
- 1366x768 additionally when available.

For every target verify preparation, active encounter, between-encounter pause, defense dock/hover/selection, enemy inspection, Hector selection/cooldowns, Divine Power ready/cooldown, Patron commentary, Menelaus normal/low HP, damaged gate, Settings/pause, victory and defeat. Confirm no clipping, panel overlap, unsafe edge margins or important battlefield occlusion.

Specific follow-up checks:
- Gold -> Speed -> Settings reads as one top-left utility row and Gate remains visually heavier below it.
- Settings opens the existing settings surface, freezes combat, and Back returns through Pause before Resume.
- Defender cards remain readable at 1366/1376x768 in RU and EN.
- Selected-defense card and enemy inspector do not occupy the same battlefield region at the same time in common selection cases.
- Parchment tutorial remains readable over both bright coast and dark combat areas.
- Menelaus portrait, name, role, HP and mechanics badges fit without colliding with encounter or Patron blocks.

## Known risks
- Runtime-generated layout still requires real Play Mode visual QA.
- The second visual-pass tree has not been compiled or tested in Unity from this GitHub-only environment.
- The user's modified music asset is unrelated and must remain untouched.

## Result
- The transferred `b8f03e2` HUD work is present in `main` together with the illustrated Hector and Patron-commentary work.
- First follow-up corrected post-merge layout regressions: guidance/resource overlap, notification/Hector overlap, next-encounter/progress overlap and enemy-inspector blocking-menu visibility.
- Second source pass moves combat speed out of the encounter center and places Gold + Speed + Settings in the canonical top-left utility cluster while preserving Gate health underneath.
- The combat Settings button delegates to the existing `GameMenuController` Settings owner; no duplicate Settings canvas/controller was introduced.
- Defender build cards, hover tooltip and selected-defense context now use substantially larger defender portraits and a stronger command-card hierarchy.
- Enemy inspection now reads as a portrait-led threat card; Menelaus now uses a dedicated large portrait-led boss block.
- Contextual tutorial guidance now uses the existing Hector parchment art with dark ink-like typography.
- Next-encounter portrait cards and transient notifications were tightened for readability without taking additional visual-domain ownership.
- PlayMode contracts were updated to protect the new ownership and geometry rules.
- No combat, balance, economy, save, encounter composition, Patron-selection or Hector gameplay behavior was changed.
- Unity is unavailable in this execution environment, so the required RU/EN resolution matrix, final EditMode/PlayMode run and Windows player build are not claimed.

## Status
`IMPLEMENTED — FINAL UNITY VALIDATION PENDING`
