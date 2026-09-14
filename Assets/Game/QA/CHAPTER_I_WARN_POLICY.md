# Chapter I WARN Acceptance Policy

This policy applies to `ChapterOnePlaythroughAnalyzer` findings. WARN is not an automatic pass. Every WARN from the Story acceptance run must be either fixed and re-tested or explicitly accepted in `CHAPTER_I_GAMEPLAY_ACCEPTANCE.json` with a concrete reason.

## WARN categories

### ECONOMY
- Fix when spend ratio is below 45% because meaningful purchases/upgrades are not attractive or available.
- Fix when spend ratio is above 92% and one imperfect purchase creates an unrecoverable starvation state.
- Accept only when the observed run still provides at least one meaningful recovery/choice path and the result is intentional.

### GATE
- Fix Story balance when the gate ends below 25% HP unless the damage is clearly attributable to a deliberate high-risk player choice.
- Fix when the gate ends above 90% with zero leaks and the chapter is demonstrably trivial.
- Accept only when the tested Story run still contains meaningful tactical pressure without threatening baseline fairness.

### LEAKS
- Fix when leak rate exceeds 12% because of a specific encounter/archetype/cadence issue.
- Accept only when leaks are intentional pressure and do not dominate the Story baseline.

### PERFORMANCE
- Fix when average FPS is below 45 at an acceptance resolution.
- Do not accept a low-FPS warning for the frozen Chapter I baseline without a documented hardware/environment exception and a separate successful target-hardware run.

### ENCOUNTER PACING
- A duration delta of 25% to 40% is WARN; 40%+ remains FAIL.
- Fix local composition, spawn cadence, congestion or HP before changing preparation time.
- Accept only when total chapter pacing remains 11-13 minutes and the encounter's pacing difference is intentionally justified by its role.

### ENCOUNTER PRESSURE
- Fix serialized pressure when peak alive is below 20% of prepared enemies for encounters with at least 8 enemies.
- Fix backlog/congestion when peak alive is above 72% and at least 8 enemies are alive simultaneously.
- Accept only when the shape is intentional and does not create trivial or unreadable combat.

### ENCOUNTER DEFENSE
- Gate loss/leaks during an encounter are diagnostic WARN unless gate HP reaches zero.
- Fix when the loss comes from an unintended archetype/counter mismatch or unfair cadence spike.
- Accept when the damage is intentional pressure and Story remains recoverable.

### ENCOUNTER ECONOMY
- Fix when encounter 2+ accumulates money because available counters/upgrades are not compelling.
- Accept only when saving is an intentional strategic option rather than absence of useful spending choices.

### BASELINE
- Non-Story runs are never accepted as the frozen Story baseline.
- Strategos and Legendary are separate pressure-validation passes.

## Acceptance rule

The final Story candidate must have zero FAIL findings. Every remaining WARN must be listed in `warningsDecisionNotes` as either `FIXED` with the replacement run/session or `ACCEPTED` with a specific design reason. Generic notes such as "looks fine" or "expected" are not sufficient.
