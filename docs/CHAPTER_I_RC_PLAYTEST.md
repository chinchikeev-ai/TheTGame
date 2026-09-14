# Chapter I Final Gameplay RC Playtest

Last reviewed: 2026-09-14

This is the acceptance protocol for freezing Chapter I gameplay balance. It is intentionally separate from the production-art gate.

## Baseline run

Run Chapter I in the Unity Editor or a local Windows build with these conditions:

- Difficulty: `Story`
- Combat speed: `1x` for the entire run
- Do **not** pause after the run starts; Chapter I pacing currently uses unscaled wall-clock time
- Language: Russian for the first visual-fit pass; repeat English fit after balance is stable
- Resolution: `1920x1080` first, then `1366x768` / `1376x768`
- Play normally from Chapter I start through the result screen
- Let normal preparation timers run unless the purpose of the run is explicitly to measure manual early-encounter starts
- Do not use editor cheats, debug spawning, time acceleration, forced kills or test-only helpers
- Use Hector, upgrades, targeting, Magic and Gift as a real player would
- Finish all five authored encounters and the Menelaus encounter

The runtime reporter automatically writes:

- `ChapterI_Playthrough_*.json`
- `ChapterI_Waves_*.csv`

under `Application.persistentDataPath/Logs`.

Current telemetry schema records both combat-speed compliance and whether pause was used. Older reports that do not contain these verification fields are not eligible for the gameplay freeze.

## Analyze the run

After the result screen:

1. Open `TheTroyGame > Validation > Analyze Latest Chapter I Playthrough`.
2. The analyzer finds the newest Chapter I JSON report.
3. It writes `ChapterI_RC_Analysis_*.md` into the same Logs directory.
4. It prints WARN/FAIL findings to the Unity Console.
5. Then run `TheTroyGame > Validation > Check Chapter I Gameplay Freeze Readiness`.
6. The freeze validator independently re-checks all hard gates and writes `ChapterI_GameplayFreeze_Readiness_*.md`.

Use `TheTroyGame > Validation > Open Chapter I Playthrough Logs` to open the report folder directly.

The freeze validator can return only:

- `BLOCKED` — one or more hard acceptance gates failed;
- `READY FOR HUMAN ACCEPTANCE` — hard gates pass, but WARN findings and real presentation still require review.

It never marks gameplay frozen automatically.

## Hard gameplay-freeze gates

A Story baseline is eligible for human acceptance only when all of the following are true:

- Report uses the current telemetry schema with 1x/no-pause evidence.
- Report belongs to Chapter I.
- Difficulty is exactly `Story`.
- Combat speed stayed at exactly `1x` for the entire run.
- Pause was not used after the run started.
- Result is `VICTORY`.
- Menelaus is defeated and does not reach a completed gate-breach state.
- Total Chapter I time is between `11:00` and `13:00`.
- Exactly five encounter snapshots are present.
- Every encounter snapshot is complete.
- Every encounter `1..5` is represented in the run.
- No individual encounter differs from its authored duration target by `40%` or more.
- Gate HP remains above zero.

The detailed analyzer additionally evaluates economy, gate-pressure quality, leaks, peak-alive pressure and observed FPS. Those findings remain part of human acceptance even when the hard freeze gate says `READY FOR HUMAN ACCEPTANCE`.

## Tuning order

When the analyzer flags a problem, tune locally before changing global rules:

1. Identify the exact encounter with the largest pacing/pressure/gate-loss deviation.
2. Check authored spawn composition and cadence.
3. Check encounter/enemy HP and speed only after composition/cadence.
4. Check rewards and tower/upgrade prices for economy problems.
5. Avoid hiding combat pacing problems by padding preparation time.
6. Avoid solving one bad encounter by globally increasing gate HP.
7. For Encounter 5, tune Menelaus reinforcement pressure separately from Menelaus identity whenever possible.

Runtime encounter composition/pacing now lives under `Assets/Resources/Data/Encounters`. Reusable enemy stats live under `Assets/Resources/Data/Enemies`; tower stats live under `Assets/Resources/Data/Towers`.

Do not tune Chapter I by adding encounter-number/index formulas back into `BalanceCatalog` or `EnemySpawner`.

## Steps 5–8 acceptance workflow

The canonical final gameplay-acceptance state is:

`Assets/Game/QA/CHAPTER_I_GAMEPLAY_ACCEPTANCE.json`

After the Story report returns `READY FOR HUMAN ACCEPTANCE`:

1. Run `TheTroyGame > Validation > Gameplay Acceptance > Prepare Latest Story Candidate`.
2. The exact Story report is bound to the manifest by `sessionId`, filename and SHA-256.
3. Review `ChapterI_WARN_Review_<session>.md`; fix or explicitly accept every WARN with a documented reason.
4. Record Story human acceptance in the manifest only after WARN review.
5. Complete clean 1x/no-pause Strategos and Legendary runs.
6. Run `TheTroyGame > Validation > Gameplay Acceptance > Analyze Story-Strategos-Legendary Pressure` and review the pressure comparison.
7. Complete the four RU/EN visual-fit passes using the presets under `TheTroyGame > Validation > Visual Fit`.
8. Run `TheTroyGame > Validation > Gameplay Acceptance > Check Final Chapter I Acceptance`.

The final validator returns:

- `BLOCKED` — one or more acceptance gates are incomplete or bound Story evidence changed;
- `READY TO FREEZE` — all prerequisite gates are accepted and the only remaining action is explicit final human freeze;
- `PASS` — every prerequisite is accepted and `gameplayFrozen=true` is explicitly recorded.

Changing the bound Story baseline invalidates downstream WARN, difficulty, visual-fit and final-freeze acceptance so stale approvals cannot carry over to new balance.

Full details: `CHAPTER_I_GAMEPLAY_ACCEPTANCE.md`.

## Difficulty validation after Story acceptance

Once Story has a clean accepted 1x baseline:

1. Repeat Chapter I on `Strategos`.
2. Repeat Chapter I on `Legendary`.
3. Keep both runs at 1x with no pause so telemetry is comparable.
4. Run `Analyze Story-Strategos-Legendary Pressure`.
5. Review authored multipliers, prepared enemy counts, peak-alive pressure, leak rate, gate loss, duration, FPS and any detected pressure inversion.
6. Record the accepted result or exception in `difficultyPressure.notes` before setting the Strategos/Legendary review flags.

The objective is increasing tactical pressure without breaking the Chapter I pacing identity or turning the Menelaus encounter into an HP-only wall. The diagnostic pressure score never replaces human review.

## Visual-fit QA before final gameplay freeze

Complete real Play Mode checks at minimum for:

- `1920x1080` RU
- `1920x1080` EN
- `1366x768` or `1376x768` RU
- `1366x768` or `1376x768` EN

Editor presets are available under `TheTroyGame > Validation > Visual Fit`. Use `Generate Chapter I QA Checklist` for the required inspection list.

Verify that the top resources, encounter status, boss HUD, Hector HUD, defense picker, contextual defense-unit menu, tooltips and result screen never overlap or leave the usable battlefield inaccessible. Inspect critical RU/EN text truncation and viewport-edge behavior through the full chapter, not only the initial frame.

The visual-fit tool never sets acceptance flags automatically. Update the matching manifest fields only after the real visual pass and record evidence/limitations in `visualFit.notes`.

Only after the final acceptance validator reports `READY TO FREEZE` should `gameplayFrozen=true` be set. Production-art freeze remains a separate gate.
