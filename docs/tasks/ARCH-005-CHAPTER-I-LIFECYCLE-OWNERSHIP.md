# AI Task Contract

## Task ID
`ARCH-005-CHAPTER-I-LIFECYCLE-OWNERSHIP`

## Goal
Make `ChapterOneRuntimeInstaller` the single lifecycle owner for Chapter I-specific presentation components.

## Why
Chapter I presentation was partially installed by the chapter runtime installer and partially self-created through independent `[RuntimeInitializeOnLoadMethod]` hooks. That duplicated composition responsibility, made startup order implicit, and allowed Chapter I components to exist outside the active chapter profile.

## Owner module
`Assets/Game/Campaign/Runtime/ChapterOneRuntimeInstaller.cs`

## Scope
The installer owns:
- ChapterOneShoreLife
- ChapterOneCoastEdgeClosure
- ChapterOneAegeanSeaPresentation
- ChapterOneAegeanSeaVolumePass
- ChapterOneBattlefieldDetails
- TroyCityBackdropPresentation
- TroyFireLifePresentation
- TroyGateDamagePresentation
- ChapterOneFactionStaging
- MenelausEntrancePresentation
- ChapterOneEncounterPresentation
- ChapterOneUiCompactPresentation

Shared/global UI and infrastructure are intentionally outside this task.

## Acceptance criteria
- [x] All listed Chapter I presenters are explicitly ensured by `ChapterOneRuntimeInstaller`.
- [x] Listed presenters no longer contain `RuntimeInitializeOnLoadMethod` self-install hooks.
- [x] Existing component `Start`/coroutine ordering remains available after installer composition.
- [x] Architecture checker verifies both installer ownership and absence of self-install hooks.
- [x] No gameplay/balance/save/input semantics are changed.

## Validation
- [x] Source-level ownership check added to `tools/check-architecture.py`.
- [ ] Run `python tools/check-architecture.py` in a real checkout.
- [ ] Run Unity EditMode/PlayMode validation.
- [ ] Manually verify Chapter I world/presentation after clean start and Restart.

## Remaining debt
Some shared/global components still use `RuntimeInitializeOnLoadMethod` and some Chapter I presentation code still performs bounded name-based startup lookup. Those are separate follow-up items; this task removes duplicated lifecycle ownership for the Chapter I-specific stack.

## Status
`IMPLEMENTED — UNITY VALIDATION PENDING`
