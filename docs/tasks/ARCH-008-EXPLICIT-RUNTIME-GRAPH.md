# AI Task Contract

## Task ID
`ARCH-008-EXPLICIT-RUNTIME-GRAPH`

## Goal
Replace scene-search-based composition with an explicit runtime object graph.

## Runtime graph
`GameBootstrap` creates one `GameRuntimeContext` with:
- `RuntimeGraph/Core`
- `RuntimeGraph/Chapter`
- `RuntimeGraph/UI`

The context owns references to the active camera/sun and the core runtime services, chapter context, combat HUD and menu.

## Chapter graph
`ChapterRuntimeInstaller` receives `GameRuntimeContext` instead of a raw camera.
`ChapterOneRuntimeInstaller` creates Chapter I components through `runtime.CreateChapter<T>()`.

`ChapterRuntimeContext` exposes:
- authored paths
- `MapBuilder`
- `TowerPlacement`
- `HectorController`

## Scene inputs
The authored `SampleScene` supplies:
- tagged Main Camera
- Directional Light assigned to `RenderSettings.sun`

Fallback camera/sun creation remains bounded inside `GameBootstrap` and does not scan the scene.

## Additional ownership cleanup
- `RuntimeInputBootstrap` owns its child EventSystem directly.
- `ChapterOneAtmosphereController` receives camera/sun from the composition root and owns its generated volume/air roots.

## Acceptance criteria
- [x] `GameBootstrap` contains no `FindFirstObjectByType` / `GameObject.Find`.
- [x] `ChapterRuntimeInstaller` contains no scene search.
- [x] `ChapterOneRuntimeInstaller` contains no scene search.
- [x] Generic `EnsureComponent<T>()` is removed from both runtime composition roots.
- [x] Chapter I components are created through the explicit runtime context.
- [x] Runtime input owns its EventSystem without scene lookup.
- [x] Chapter atmosphere receives camera/sun explicitly.
- [x] Architecture guard rejects scene search in composition roots.
- [x] PlayMode runtime-graph contract verifies context references.
- [ ] Run the architecture checker in a real checkout.
- [ ] Run Unity EditMode/PlayMode validation.
- [ ] Verify clean start and Restart Chapter I.

## Status
`IMPLEMENTED — UNITY VALIDATION PENDING`
