# Chapter I Defensive Combat Pass

Status: runtime defensive-combat candidate implemented; real Unity Play Mode visual QA and balance tuning remain required.

## Scope

This bundled pass closes the Chapter I shield/block presentation and gameplay loop as one production block:

- `CharacterPresentationState` now owns a persistent logical block state through `SetBlocking(bool)` while preserving the existing `Block` trigger fallback.
- Trojan Guard enters brace while it owns enemy reservations, keeps the defensive state active, and turns toward the nearest reserved attacker.
- Trojan Guard receives candidate damage reduction while braced and uses shield-specific impact VFX/audio instead of the normal hit reaction for blocked hits.
- Hector Shield Wall now creates a timed defensive window instead of being only a slow-zone presentation.
- Hector receives candidate damage reduction for the Shield Wall duration.
- Hector keeps a defensive presentation state active for the same duration as the gameplay window.
- Shield Wall factory accepts the gameplay duration so zone lifetime and hero defensive state stay synchronized.
- Blocked hits use a dedicated procedural shield tone and bronze impact pulse.

## Candidate tuning

These values are gameplay candidates, not release balance:

- Trojan Guard braced damage multiplier: `0.65` (35% reduction).
- Hector Shield Wall damage multiplier: `0.55` (45% reduction).
- Hector Shield Wall duration: `4.0 s`.
- Trojan Guard frontal brace turn speed: `8 deg-like interpolation units/s` through `Quaternion.Slerp`.

## Runtime ownership

`CharacterPresentationState` owns the persistent block-state contract and animation parameter fallback.

`TrojanGuardSquad` owns reservations, brace facing, candidate mitigation and blocked-hit feedback.

`HectorController` owns Shield Wall duration, cooldown and candidate mitigation.

`HectorPresentationBridge` keeps the defensive pose synchronized with the gameplay window and routes blocked-hit presentation.

`RuntimeEffects` supplies reproducible procedural shield-impact audio until authored production audio exists.

## Acceptance limitations

This pass does not promote shield/character art to `DONE`. Remaining production work:

1. authored shield-brace/hold/release clips;
2. exact shield hand grip and IK;
3. authored blocked-hit reaction clips;
4. final shield impact VFX/audio;
5. front/side/back mitigation tuning if attack-source data is standardized across all damage paths;
6. difficulty-specific Guard/Hector mitigation balance;
7. gameplay-camera inspection and EN/RU 16:9 QA;
8. actual Unity Play Mode verification once CI activation is available.
