# AI Task Contract

## Task ID
`QA-001-CHAPTER-I-QA-SUMMARY`

## Goal
After every completed Chapter I Play Mode run that produced telemetry, automatically create a concise human-readable Markdown QA summary and a machine-readable JSON summary next to the telemetry, stating whether the run can be bound to gameplay acceptance and what to do next.

## Why
The existing telemetry and RC analysis contain the required facts, but they are too raw for fast human acceptance. A tester should immediately see whether a run is blocked, why, and which validation/menu action comes next without manually interpreting telemetry fields.

## Owner module
Editor / Tests (`Assets/Editor`, `Assets/Tests/EditMode`) as defined by `docs/MODULE_MAP.md`.

## Allowed files
- `Assets/Editor/ChapterOneGameplayFreezeValidator.cs`
- `Assets/Editor/ChapterOneQaSummaryAutoReporter.cs`
- `Assets/Tests/EditMode/ChapterOneQaSummaryContractTests.cs`
- matching `.meta` files
- `docs/tasks/QA-001-CHAPTER-I-QA-SUMMARY.md`

## Do not change
- Chapter I authored balance/data.
- Runtime telemetry schema or gameplay behavior.
- Gameplay acceptance manifest values.
- Campaign save schema.
- Art/content status.

## Inputs / source of truth
- `Assets/Game/Core/Telemetry/ChapterOnePlaythroughReporter.cs`
- `Assets/Editor/ChapterOnePlaythroughAnalyzer.cs`
- `Assets/Editor/ChapterOneGameplayFreezeValidator.cs`
- `Assets/Game/QA/CHAPTER_I_GAMEPLAY_ACCEPTANCE.json`
- `docs/PROJECT_STATUS.md`
- `docs/MODULE_MAP.md`

## Acceptance criteria
- [x] A completed Chapter I telemetry report can produce `ChapterI_QA_Summary_YYYY-MM-DD_HH-mm-ss.md` beside the telemetry.
- [x] The same evaluation produces neighboring `ChapterI_QA_Summary_YYYY-MM-DD_HH-mm-ss.json`.
- [x] JSON contains `verdict`, `canBindAcceptance`, `mainReason`, `nextAction`, and source report/session identity.
- [x] Markdown clearly states verdict, human-readable reasons, and concrete next actions.
- [x] Existing gameplay-freeze hard blockers remain the authority for `canBindAcceptance`.
- [x] Average FPS below 45 remains a WARN and is surfaced to the tester without silently becoming a new hard freeze blocker.
- [x] A clean Story baseline points the tester to `TheTroyGame > Validation > Gameplay Acceptance > Prepare Latest Story Candidate`.
- [x] Summary generation is triggered after a completed Play Mode run and survives normal Unity domain reload by persisting the pending report path in `SessionState`.
- [x] Existing public runtime contracts are unchanged.

## Automated validation
- [ ] `python tools/check-architecture.py`
- [x] EditMode contract tests added for summary outputs and Play Mode reload handoff.
- [ ] Full EditMode suite in Unity.
- [ ] PlayMode acceptance suite.
- [ ] Full `tools/validate-project.ps1` or `tools/validate-project.sh` when Unity is available.

## Manual validation
1. Complete Chapter I Story with a deliberately invalid condition such as 2x speed and verify Markdown/JSON say `BLOCKED` and give a specific retry action.
2. Complete a clean Story 1x no-pause Victory in 11:00-13:00 and verify `canBindAcceptance=true` / `READY_TO_BIND` unless another hard blocker exists.
3. Verify a run below 45 average FPS surfaces a performance warning while preserving hard-gate semantics.
4. Exit Play Mode normally and confirm summary files appear beside the telemetry without requiring a separate menu command.

## Known risks
- Automatic processing depends on a completed run having written `ChapterOnePlaythroughReporter.LastReportPath` before Play Mode exits.
- Unity compilation and real Play Mode behavior still require validation in a Unity-capable environment.
- Human-readable blocker mapping intentionally covers common freeze blockers and falls back to the original validator text for uncommon cases.

## Result
- Changed `ChapterOneGameplayFreezeValidator.cs` to create human and machine QA summaries using the existing hard freeze rules.
- Added `ChapterOneQaSummaryAutoReporter.cs` to process the just-completed report after returning to Edit Mode, with `SessionState` persistence across domain reload.
- Added EditMode contract tests.
- Validation actually executed in this change: source/diff review only; Unity tests/build have not been run from this environment.
- Remaining manual checks: real invalid/valid Chapter I Play Mode runs and file-output inspection.
- Commit SHA: recorded by Git history on branch `qa/chapter1-summary`.

## Status
`IN_PROGRESS`
