# KayKit Character Pack — Adventurers

## Source

- Repository: `KayKit-Game-Assets/KayKit-Character-Pack-Adventures-1.0`
- Project path: `Assets/ThirdParty/KayKitAdventurers`
- Integration: Git submodule
- Pinned source commit: `672074b73ba276876a19e8816ecdc5241817ab47`

## License

The upstream pack is published under **CC0 1.0**. It permits personal and commercial use without attribution.

The source remains third-party content. Troy-specific generated prefabs, equipment overlays and Animator bindings live under `Assets/Game/Art/...` or the project Editor tooling; source existence alone does not make those derivatives production-final.

## Current Chapter I use

The pack supplies the stylized rigged character source used by the generated Chapter I character candidates. Hector currently prefers the Knight character source in `CartoonCharacterPrefabBuilder`.

`HectorProductionAnimationBinder` applies a deterministic Hector-only candidate pass over the generated `ChapterOne_Hector.controller`. It prefers semantic KayKit clips for:

- spear stab/thrust (`Poke`);
- block/deploy/hold (`Block`, `AbilityE`, `ShieldHold`);
- cheer/taunt (`AbilityQ` War Cry);
- throw (`AbilityR` Spear Throw);
- heavy attack (`AbilityF` For Troy);
- laying/lying down idle (`Downed`).

When equally suitable clips exist, the binder prefers clips imported from `Knight.fbx`, matching Hector's current generated body source. Missing specialized clips leave the existing controller motion intact and are logged as warnings.

## Acceptance status

This is a **SOURCE ONLY / GENERATED PLACEHOLDER** dependency for Chapter I production art. It is not evidence of final visual acceptance.

Before Hector can move to `DONE`, real Unity Play Mode QA must still verify silhouette, rig deformation, spear/shield grip, clip timing, contact/release frames, Shield Wall hold, down/revive presentation, VFX synchronization and gameplay-camera readability.
