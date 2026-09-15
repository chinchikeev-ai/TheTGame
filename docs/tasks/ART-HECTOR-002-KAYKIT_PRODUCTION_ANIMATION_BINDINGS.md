# ART-HECTOR-002 — KayKit production animation candidate bindings

Status: **IMPLEMENTED — UNITY / PLAY MODE QA PENDING**

## Goal

Promote Hector from generic token-selected animation placeholders to a deterministic Hector-specific candidate mapping using the already-pinned CC0 KayKit source, without changing gameplay balance, collision, navigation or movement ownership.

## Scope

- keep `ChapterOneCharacterAnimationBuilder` as the shared role-controller generator;
- add an isolated post-build `HectorProductionAnimationBinder`;
- prefer the current Hector body source (`Knight.fbx`) when multiple equivalent clips exist;
- preserve existing motions when no specialized candidate is found;
- keep root motion disabled and gameplay authoritative for translation/facing;
- do not update production-acceptance manifests in this pass.

## Candidate mapping

| Hector state | Preferred semantic source |
|---|---|
| Poke | melee stab / stab / thrust / poke |
| Block | block / defend / guard |
| AbilityQ — War Cry | cheer / taunt / shout / victory |
| AbilityE — Shield Wall deploy | block / defend / guard |
| ShieldHold | block / defend / guard |
| AbilityR — Spear Throw | throw / hurl / toss / javelin |
| AbilityF — For Troy | heavy attack / power attack |
| Downed | laying/lying down idle |

`Downed` intentionally prefers a persistent ground-idle clip rather than a one-shot defeat clip because the current Animator contract exposes `IsDowned` as a bool for the full downed duration. A separate authored collapse/get-up sequence requires an explicit runtime entry/revive trigger and is outside this binding-only pass.

## Reproducibility

`CartoonCharacterAutoBuilder.BuildMissing()` now invokes the Hector binder after the shared animation profile exists. The binder can also be run directly from:

`The Troy Game/Characters/Bind Hector Production Animation Candidates`

Source provenance is documented in `docs/third_party/KAYKIT_ADVENTURERS.md`.

## Acceptance still required

Before production acceptance:

1. run Unity compile/EditMode checks;
2. inspect the resolved binding log and confirm intended KayKit clip names;
3. verify idle → movement → basic spear attack transitions;
4. verify Q anticipation/readability and gameplay impact phase;
5. verify E deploy into a stable Shield Wall hold for the full gameplay duration;
6. verify R has a readable full-body throw and that visible spear release matches the gameplay callback;
7. verify F reads as a distinct ultimate, not a normal attack;
8. verify downed pose stays stable and revive returns cleanly to guarded idle;
9. verify shield/spear hand placement, clipping and silhouette at gameplay camera distance;
10. tune action locks/contact/release phases only after the actual selected clips are visually inspected.

Do not mark Hector `DONE` or add its paths to production acceptance until these checks pass in real Play Mode.
