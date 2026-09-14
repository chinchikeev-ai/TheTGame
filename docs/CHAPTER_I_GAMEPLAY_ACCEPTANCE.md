# Chapter I Gameplay Acceptance — Steps 5–8

Last reviewed: 2026-09-14

This document closes the second half of the Chapter I gameplay RC gate after a Story run has already passed the hard freeze-readiness validator.

The final Chapter I gameplay baseline is tracked in:

`Assets/Game/QA/CHAPTER_I_GAMEPLAY_ACCEPTANCE.json`

The manifest starts closed by design. Tooling must never set human-acceptance or visual-QA flags automatically.

## Step 5 — Resolve or explicitly accept WARN findings

1. Complete a clean Story run at 1x with no pause.
2. Run `TheTroyGame > Validation > Gameplay Acceptance > Prepare Latest Story Candidate`.
3. The command first requires the Story report to pass `ChapterOneGameplayFreezeValidator`.
4. It binds the exact report to the manifest using:
   - telemetry `sessionId`;
   - report filename;
   - SHA-256 of the report bytes.
5. It runs the normal Chapter I analyzer and writes `ChapterI_WARN_Review_<session>.md` next to the telemetry logs.
6. For every WARN finding, either:
   - fix the issue and repeat the Story run; or
   - record an explicit design reason for accepting the warning.
7. Copy the final decisions into `storyBaseline.warningsDecisionNotes` and set `warningsReviewed=true` only after the review is complete.

Changing the bound Story session automatically invalidates downstream acceptance state: WARN review, Story human acceptance, difficulty review, visual-fit review and final gameplay freeze all reset.

## Step 6 — Record Story human acceptance

After the bound Story report is freeze-ready and all WARN findings are resolved/accepted:

- set `storyBaseline.humanAccepted=true`;
- set `storyBaseline.acceptedBy` to the actual reviewer;
- set `storyBaseline.acceptedUtc` to an ISO-8601 UTC timestamp;
- do not set `gameplayFrozen=true` yet.

`TheTroyGame > Validation > Gameplay Acceptance > Check Final Chapter I Acceptance` re-verifies the locally bound Story report, SHA-256, session id and the hard freeze validator.

Story acceptance means only that the Story pacing/economy/pressure baseline is accepted. It does not replace harder-difficulty or visual-fit validation.

## Step 7 — Strategos / Legendary pressure validation

Run one complete 1x/no-pause Chapter I playthrough on each:

- Strategos;
- Legendary.

Then run:

`TheTroyGame > Validation > Gameplay Acceptance > Analyze Story-Strategos-Legendary Pressure`

The report compares:

- authored starting Gold;
- authored gate HP;
- enemy HP/speed/count/reward multipliers;
- total prepared enemies;
- normalized peak-alive pressure;
- leak rate;
- gate-loss ratio;
- duration ratio;
- average FPS;
- a diagnostic observed-pressure score.

The observed score is not an automatic balance oracle. Player decisions differ between runs. The tool detects obvious missing/ineligible runs and material pressure inversions, then leaves the final decision to human review.

After reviewing both harder modes:

- set `difficultyPressure.strategosReviewed=true`;
- set `difficultyPressure.legendaryReviewed=true`;
- write the result/accepted exceptions in `difficultyPressure.notes`.

## Step 8 — RU/EN 16:9 visual-fit QA

Required matrix:

- 1920x1080 RU;
- 1920x1080 EN;
- 1366x768 or 1376x768 RU;
- 1366x768 or 1376x768 EN.

Unity Editor presets:

- `TheTroyGame > Validation > Visual Fit > 1920x1080 RU`
- `TheTroyGame > Validation > Visual Fit > 1920x1080 EN`
- `TheTroyGame > Validation > Visual Fit > 1376x768 RU`
- `TheTroyGame > Validation > Visual Fit > 1376x768 EN`

Use `Generate Chapter I QA Checklist` for the inspection list.

On every preset inspect the full Chapter I flow, including:

- top resources;
- encounter/wave status;
- boss HUD;
- Hector HUD and ability labels;
- defense picker;
- contextual selected-defense menu near viewport edges;
- tooltips;
- Menelaus presentation behind the HUD;
- result screen;
- critical RU/EN text truncation and overlap;
- usable battlefield area.

After each real visual pass, update the matching `visualFit` flag and record build/editor state plus accepted limitations in `visualFit.notes`.

The QA tool never sets those flags automatically.

## Final freeze

Run:

`TheTroyGame > Validation > Gameplay Acceptance > Check Final Chapter I Acceptance`

Possible states:

- `BLOCKED` — one or more prerequisite gates are incomplete or the bound Story evidence changed;
- `READY TO FREEZE` — every prerequisite is accepted, but `gameplayFrozen` is still false;
- `PASS` — all prerequisites are accepted and the manifest explicitly has `gameplayFrozen=true`.

Only after the validator reports `READY TO FREEZE` should `gameplayFrozen=true` be set as the explicit final human action.

Production-art acceptance remains independent. A frozen gameplay baseline does not imply final art is `DONE`.
