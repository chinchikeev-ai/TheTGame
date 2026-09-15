# AI Task Contract

## Task ID
`UX-005-UNIFIED-COMBAT-HUD`

## Goal
Bring the remaining Chapter I combat HUD into the same dark-stone, bronze-framed, portrait-led visual language as the lower-left Hector block.

## Why
The Hector block reads as an intentional game interface while several other runtime HUD regions still look like unrelated prototype panels. Divine Power is also represented by two competing controls.

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
- [x] Resources and encounter state use semantic portrait/icon anchors.
- [x] Divine Power has one clear, direct control with effect/readiness copy.
- [x] Enemy inspector uses the same panel/portrait hierarchy.
- [x] Legacy `MagicToggle`, `MagicFlyout`, `CombatActions` and in-combat Patron selection are absent.
- [x] Guidance is kept below the top-left resource/gate cluster.
- [x] Combat notifications reserve the lower-left Hector region and disappear when empty.
- [x] Next-encounter portrait cards are separated from the centered encounter progress/speed controls.
- [x] Objective copy no longer repeats encounter progress and gate HP already visible in the main HUD.
- [x] Enemy inspector yields to blocking menus.
- [x] RU/EN content keeps existing responsive target containers.
- [x] Existing gameplay contracts are preserved.

## Automated validation
- [x] Architecture guard passed on the original `b8f03e2` HUD implementation/merge baseline.
- [x] PlayMode layout contracts were extended for single Divine Power ownership, semantic portraits, perimeter anchors and non-competing HUD zones.
- [ ] Re-run `git diff --check` and `python tools/check-architecture.py` for the final follow-up tree when a local checkout is available.
- [ ] Re-run EditMode and PlayMode tests for the final follow-up tree when Unity is available.
- [ ] Run full `tools/validate-project.ps1` / `tools/validate-project.sh` and Windows build when Unity is available.

## Manual validation
Required real Play Mode matrix remains:
- 1920x1080 RU;
- 1920x1080 EN;
- 1376x768 RU;
- 1376x768 EN;
- 1366x768 additionally when available.

For every target verify preparation, active encounter, between-encounter pause, defense dock/hover/selection, enemy inspection, Hector selection/cooldowns, Divine Power ready/cooldown, Patron commentary, Menelaus normal/low HP, damaged gate, pause, victory and defeat. Confirm no clipping, panel overlap, unsafe edge margins or important battlefield occlusion.

## Known risks
- Runtime-generated layout still requires real Play Mode visual QA.
- The final follow-up tree has not been compiled or tested in Unity from this GitHub-only environment.
- The user's modified music asset is unrelated and must remain untouched.

## Result
- The transferred `b8f03e2` HUD work is now present in `main` through merge commit `050867b` together with the newer illustrated Hector and Patron-commentary work.
- Main runtime panels/buttons use the same sliced Trojan art family as Hector and the direct Divine Power action remains the sole in-combat magic control.
- Static follow-up found and corrected post-merge layout regressions: guidance had returned to the resource block, notifications occupied Hector's corner, and next-encounter cards occupied the encounter progress/speed region.
- Guidance now carries objective-only tactical copy rather than duplicating gate HP and encounter progress.
- Enemy inspection now hides behind blocking menus while preserving the selected enemy for return from pause/settings.
- PlayMode contracts now protect the semantic portraits/icons, single Divine Power ownership, perimeter anchors and the corrected safe zones.
- No gameplay, balance, economy, save, wave/encounter composition or Patron-selection behavior was changed.
- Unity is unavailable in this execution environment, so the required RU/EN resolution matrix, final EditMode/PlayMode run and Windows player build are not claimed.

## Status
`IMPLEMENTED — FINAL UNITY VALIDATION PENDING`
