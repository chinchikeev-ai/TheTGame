# AI Task Contract

## Task ID
`UX-004-GAMEPLAY-FEEDBACK-ROUND3`

## Goal
Apply the latest Chapter I playtest feedback: halve starting gold and earned enemy rewards, increase enemy counts by 1.5x, restore the Hector command block, support click-to-inspect enemy stats, make combat magic self-explanatory, and require patron selection before the map run starts.

## Why
The latest human playtest still had excessive economy/enemy-count slack and exposed several interaction regressions or unclear mechanics. Patron selection also occurred inside the combat map instead of as a pre-map decision.

## Owner module
Primary owner: `Assets/Game/UI` with supporting balance/session/runtime changes.

## Allowed files
- `Assets/Game/Core/Balance/DifficultyRules.cs`
- `Assets/Game/Core/Session/GameManager.cs`
- `Assets/Game/Campaign/Runtime/ChapterOneRuntimeInstaller.cs`
- `Assets/Game/UI/*Patron*`
- `Assets/Game/UI/EnemyInspectorPresentation.cs`
- `Assets/Game/UI/CombatActionClarityPresentation.cs`
- `Assets/Tests/EditMode/MinorGameplayUxContractTests.cs`
- this task contract

## Do not change
- Tower costs or sell-refund behavior.
- Enemy HP/speed difficulty multipliers.
- Authored encounter composition/pacing assets in this task; the requested 1.5x count increase is applied through difficulty count multipliers.
- Hector road movement contracts.
- Chapter sequencing or save schema.

## Inputs / source of truth
- Human playtest feedback from 2026-09-14.
- `docs/DATA_CATALOG.md` difficulty/EncounterData ownership rules.
- Existing `GameManager`, `ModernCombatHud`, `HectorHUD`, `Enemy`, and `GameMenuController` behavior.

## Acceptance criteria
- [x] Starting gold is exactly halved for Story, Strategos, and Legendary.
- [x] Enemy-kill reward multipliers are exactly halved for Story, Strategos, and Legendary.
- [x] Enemy count multipliers are exactly 1.5x their previous values while preserving Story < Strategos < Legendary.
- [x] Apollo pre-map gold is reduced to +50 to stay proportional to the halved economy.
- [x] Chapter I explicitly installs HectorHUD so the lower-corner hero block is not dependent only on runtime auto-create ordering.
- [x] Clicking a live enemy shows current HP, speed, armor, arrow resistance, gate damage, bounty, route progress, and block state.
- [x] Magic UI states the exact effect: all enemies, 120 damage, 50% slow for 5 seconds, 30-second cooldown/readiness.
- [x] In-map patron selection is removed from the active runtime flow.
- [x] Chapter I selection opens a mandatory patron screen before starting the map; GameManager also rejects patron changes after BeginRun.
- [ ] No architecture guard violations.
- [x] Existing public gameplay contracts preserved except the explicitly changed patron timing/economy.

## Automated validation
- [ ] `python tools/check-architecture.py`
- [x] EditMode contract tests updated for economy/count/patron rules.
- [ ] PlayMode tests required/updated.
- [ ] Full `tools/validate-project.ps1` or `tools/validate-project.sh` when Unity is available.

## Manual validation
- Start Chapter I from chapter select and confirm the patron screen appears before combat/map activation.
- Confirm each god applies the displayed effect and there is no selectable gift button during combat.
- Confirm Hector block is visible bottom-left during combat and hidden behind blocking menus.
- Click multiple enemy archetypes/models and verify the inspector opens and live HP updates.
- Verify the Divine Storm button copy/readiness/cooldown is readable at 1366x768 and 1920x1080.
- Complete a fresh Story 1x/no-pause run and review pacing/economy telemetry.

## Known risks
- The pre-map presentation hooks the current runtime-generated Chapter I menu button and calls the existing non-public `StartLevel` entrypoint through reflection because the menu controller does not expose a public start command. This should be replaced by an explicit public menu/navigation contract if the campaign menu architecture is refactored.
- UI is generated at runtime and still needs visual PlayMode validation.
- Increased counts can materially raise simultaneous enemy load and must be performance-checked in Unity.

## Result
- Balance/session rules updated for exact requested economy/count ratios.
- Added pre-map patron selector, enemy inspector, combat magic clarity presentation.
- Removed the old in-map patron start controller.
- Chapter I runtime now explicitly installs HectorHUD and the new presentation controllers.
- EditMode contract assertions updated.
- Unity/architecture/full project validation not yet executed in this environment.

## Status
`IN_PROGRESS`
