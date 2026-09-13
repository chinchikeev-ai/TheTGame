# Chapter I Final Gameplay RC Playtest

This is the acceptance protocol for freezing Chapter I gameplay balance. It is intentionally separate from the production-art gate.

## Baseline run

Run Chapter I in the Unity Editor or a local Windows build with these conditions:

- Difficulty: `Story`
- Combat speed: `1x` for the entire run
- Language: Russian for the first visual-fit pass; repeat English fit after balance is stable
- Resolution: `1920x1080` first, then `1366x768` / `1376x768`
- Play normally from Chapter I start through the result screen
- Let normal preparation timers run unless the purpose of the run is explicitly to measure manual early-wave starts
- Do not use editor cheats, debug spawning, time acceleration, forced kills or test-only helpers
- Use Hector, upgrades, targeting, Magic and Gift as a real player would
- Finish all five combat events and the Menelaus encounter

The runtime reporter automatically writes:

- `ChapterI_Playthrough_*.json`
- `ChapterI_Waves_*.csv`

under `Application.persistentDataPath/Logs`.

## Analyze the run

After the result screen:

1. Open `TheTroyGame > Validation > Analyze Latest Chapter I Playthrough`.
2. The analyzer finds the newest Chapter I JSON report.
3. It writes `ChapterI_RC_Analysis_*.md` into the same Logs directory.
4. It opens the generated analysis and prints WARN/FAIL findings to the Unity Console.

Use `TheTroyGame > Validation > Open Chapter I Playthrough Logs` to open the report folder directly.

## RC acceptance rules

A Story baseline is eligible to freeze only when all of the following are true:

- Result is `VICTORY`.
- Menelaus is defeated and does not destroy the Trojan gate.
- Total Chapter I time is between `11:00` and `13:00` at 1x.
- All five wave snapshots are present and complete.
- No wave is grossly outside its authored target duration.
- Economy does not show obvious starvation or large unused purchasing power.
- Gate survival is neither zero nor an obviously untouched no-pressure result.
- Leak pressure is explainable and does not dominate the run.
- Peak-alive pressure does not show extreme serialization or a persistent enemy backlog.
- Average FPS does not raise a first-pass performance warning for the tested resolution.

WARN findings may be accepted only when the design reason is explicit. FAIL findings block the Story gameplay freeze.

## Tuning order

When the analyzer flags a problem, tune locally before changing global rules:

1. Identify the exact wave with the largest pacing/pressure/gate-loss deviation.
2. Check composition and spawn cadence.
3. Check enemy HP/speed only after composition/cadence.
4. Check rewards and tower/upgrade prices for economy problems.
5. Avoid hiding combat pacing problems by padding preparation time.
6. Avoid solving one bad wave by globally increasing gate HP.
7. For Wave 5, tune Menelaus reinforcement pressure separately from Menelaus identity whenever possible.

Authored balance lives under `Assets/Resources/Data/Waves`, `Assets/Resources/Data/Enemies` and `Assets/Resources/Data/Towers`.

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

Verify that the top resources, wave status, boss HUD, Hector HUD, defense picker, contextual defense-unit menu and result screen never overlap or leave the usable battlefield inaccessible.

Only after gameplay RC and visual-fit QA are accepted should Chapter I be treated as the campaign gameplay baseline. Production-art freeze remains a separate gate.
