# Hector Production Combat Pass

Status: production-candidate runtime pass. This does not mark Hector art or animation as DONE.

## Runtime contract

Hector now uses one authoritative combat-action state instead of allowing movement, auto-attacks and abilities to overlap freely.

Actions:
- `BasicAttack`
- `WarCry`
- `ShieldWall`
- `SpearThrow`
- `Ultimate`
- `Downed`

Each action owns a short action lock. Movement and new combat commands are rejected while the action lock is active. Shield Wall additionally prevents movement and new attacks for its active defensive window.

## Facing and targeting

Basic spear attacks and Spear Throw capture a target-facing point when the action begins. Hector rotates toward that point during the locked action instead of continuing movement-facing. This is a runtime presentation candidate and still requires real gameplay-camera QA.

## Animation-phase gameplay

Gameplay application is synchronized to candidate animation phases:
- basic spear impact: existing `Poke` phase contract;
- Q / War Cry: `AbilityQ` phase callback;
- E / Shield Wall: `AbilityE` phase callback before the defensive window starts;
- R / Spear Throw: existing `AbilityR` release callback, then visible projectile flight, then arrival damage;
- F / For Troy: `AbilityF` phase callback.

All phase callbacks retain bounded timing fallbacks when the expected Animator state is unavailable.

## State-conflict protection

An incrementing action token invalidates stale callbacks after a down/cancel transition. Downed state cancels pending timed presentation callbacks and restores the carried spear. Revive clears the action state and restores the normal presentation state.

Normal player input does not queue movement or abilities behind an active combat action: `HectorInputDriver` accepts movement only when `CanMove` and abilities only when `CanAcceptCombatCommand`.

## Shield Wall

Current candidate values remain:
- duration: `4.0s`;
- incoming damage multiplier: `0.55`;
- cooldown: `22s`.

Shield mitigation is directional when a source point is supplied. Exact frontal arc, mitigation value, pose timing and VFX lifetime remain balance/visual candidates until Play Mode QA.

## Acceptance still required

Before this can be called final production combat:
1. run real Unity compile/editmode/playmode tests;
2. inspect Q/E/R/F clip choices generated from the imported animation bank;
3. verify contact/release frames and action-lock lengths;
4. verify shield pose and frontal arc from gameplay camera;
5. verify down/revive transitions and spear visibility;
6. tune cooldowns/damage/mitigation on Story, Strategos and Legendary;
7. replace placeholder/procedural VFX/audio and final hero art where required by the art freeze gate.
