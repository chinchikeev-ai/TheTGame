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

## Freeze rule

Once a Story report returns `READY FOR HUMAN ACCEPTANCE`:

1. Review every remaining analyzer WARN finding.
2. Explicitly accept or fix each warning.
3. Perform the required RU/EN visual/readability pass.
4. Record the baseline as accepted in `PROJECT_STATUS.md` only after that inspection.
5. After acceptance, change Chapter I balance only for a documented regression or deliberately reopened balance task.

Until those steps are complete, Chapter I remains an RC even if the hard automated gate passes.

## Difficulty validation after Story freeze

Once Story has a clean accepted 1x baseline:

1. Repeat Chapter I on `Strategos`.
2. Repeat Chapter I on `Legendary`.
3. Use the same analyzer for each run.
4. Treat the harder modes as pressure-validation passes, not as replacements for the Story baseline.

The objective is increasing tactical pressure without breaking the Chapter I pacing identity or turning the Menelaus encounter into an HP-only wall.

## Visual-fit QA after gameplay freeze

After gameplay tuning is stable, complete real Play Mode checks at minimum for:

- `1920x1080` RU
- `1920x1080` EN
- `1366x768` or `1376x768` RU
- `1366x768` or `1376x768` EN

Verify that the top resources, encounter status, boss HUD, Hector HUD, defense picker, contextual defense-unit menu and result screen never overlap or leave the usable battlefield inaccessible.

Only after gameplay RC and visual-fit QA are accepted should Chapter I be treated as the campaign gameplay baseline. Production-art freeze remains a separate gate.
