# ART-HECTOR-001 — Hector Animation & Action Contract

Status: IMPLEMENTED — PLAY MODE QA PENDING
Owner: Production Art / Gameplay Presentation
Scope: Chapter I Hector

## Objective

Make Hector read as a deliberate hero character in combat instead of a generic animated unit. Every gameplay action must have a distinct pose, timing phase and presentation hook that can later be replaced by final authored animation clips without changing gameplay logic.

## In scope

- idle / breathing;
- locomotion;
- basic spear attack;
- normal hit reaction;
- shield-block reaction;
- Q — War Cry;
- E — Shield Wall wind-up plus persistent defensive hold;
- R — Spear Throw wind-up / release / follow-through;
- F — For Troy ultimate;
- downed state;
- revive/recovery presentation;
- Animator parameter/state contract for the final Hector rig;
- deterministic fallback motion while the final authored clips are not accepted;
- animation-phase gameplay callbacks already used for basic/Q/E/R/F;
- carried-spear visibility during R;
- no root-motion ownership of gameplay movement.

## Animator contract

Required parameters for the final Hector controller:

- `Speed` (float) — locomotion blend;
- `Attack` (trigger) — generic fallback only;
- `Poke` (trigger) — basic spear thrust;
- `Hit` (trigger) — unblocked hit reaction;
- `AbilityQ` (trigger) — War Cry;
- `AbilityE` (trigger) — Shield Wall deploy;
- `AbilityR` (trigger) — Spear Throw;
- `AbilityF` (trigger) — For Troy ultimate;
- `Blocking` (bool) — persistent Shield Wall hold;
- `IsDowned` (bool) — downed/recovery state;
- `Die` (trigger) — generic character compatibility; Hector Chapter I normally uses Downed/Revive instead of permanent death.

Root motion must stay disabled. `HectorController` owns translation and combat facing.

## Action language

| Action | Readable motion | Gameplay/contact phase |
|---|---|---|
| Idle | guarded stance, shield forward, spear ready, subtle breathing | none |
| Move | fast tactical run, shield controlled, spear carried | continuous |
| Basic attack | short anticipation → direct spear thrust → recover | ~0.48 normalized |
| Hit | brief torso/head recoil without losing facing | immediate feedback |
| Shield block | shield absorbs impact; small upper-body recoil | on blocked hit |
| Q War Cry | plant feet → open/raise weapon side → command shout → settle | ~0.38 normalized |
| E Shield Wall | shield rises and body braces → defensive effect deploys → persistent hold | deploy ~0.42; hold for gameplay duration |
| R Spear Throw | draw spear behind shoulder → full-body throw → release → follow-through → recover | release ~0.52 normalized; damage on projectile arrival |
| F For Troy | gather power → heroic wide pose / command strike → battlefield pulse → recover | ~0.50 normalized |
| Downed | collapse/grounded defeated pose; no locomotion/actions | until revive timer |
| Revive | recover to guarded stance; spear/shield presentation restored | after gameplay revive |

## Implemented engineering behavior

- `HectorPresentationBridge` now exposes the gameplay animation-phase hooks used by `HectorController` for Q/E/F as well as R/basic attack.
- Shield Wall deploy and Shield Wall hold are separate phases. The persistent hold begins from the E gameplay impact phase instead of racing the E trigger on the same frame.
- The Hector Animator candidate has a dedicated `Blocking` bool and `ShieldHold` state.
- R clip selection first looks for true throw semantics: `throw`, `hurl`, `toss`, `javelin`; spear thrust/poke is fallback only.
- Procedural fallback motion has distinct anticipation/action/recovery for Basic/Q/E/R/F rather than static generic poses.
- Procedural fallback includes guarded idle, locomotion, normal-hit reaction, blocked-hit reaction, persistent Shield Wall hold, downed and revive motion.
- Spear visibility is restored on revive and after projectile arrival.
- Root motion remains disabled in the animation builder.
- No Hector gameplay balance values were changed.

## Rules

1. Q/E/R/F must remain visually distinct even if source clips are temporary.
2. R must prefer a true throw/hurl clip over a spear thrust clip when one exists.
3. E deploy animation and persistent Shield Wall hold are separate phases; the hold must not interrupt the E wind-up before its gameplay impact frame.
4. Shield Wall hold lasts for the full gameplay defensive window, not merely the initial action lock.
5. Downed invalidates pending action callbacks; revive restores a coherent weapon/pose state.
6. Final authored clips may replace candidate/fallback motion without changing Hector gameplay timings or action ownership.

## Acceptance status

Engineering/candidate contract is implemented in source. Added EditMode API-contract coverage for Hector actions/presentation hooks.

Still NOT production accepted. Required before `DONE`:

- final Hector rig and authored hero clips installed;
- spear hand/socket and shield grip/IK verified;
- exact contact/release frames tuned in real Play Mode;
- gameplay-camera silhouette/readability accepted;
- Q/E/R/F effects synchronized with final clips;
- real animation visual QA passed.

Unity compile/EditMode/PlayMode and real visual QA were not executed in this change because the available environment does not have a working Unity activation path. Do not update production-art acceptance manifests from this task alone.
