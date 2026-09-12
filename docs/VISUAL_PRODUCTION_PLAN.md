# TheTroyGame — Visual Production Plan

Last reviewed: 2026-09-13

Goal: turn Chapter I / The Landing into the polished vertical slice before expanding later chapters.

## P0 — Stabilize Current Slice

- Keep Unity compile at zero errors.
- Fix compiled-build restart and return-to-menu through scene `buildIndex`.
- Remove duplicate combat HUD information.
- Replace permanent bottom build bar with right-side `+` tower picker.
- Keep tower selection in one place only; combat speed/magic/gift controls must not recreate tower purchase buttons.
- Commit all required `.meta` files for current tracked assets.
- Keep `ProjectSettings/McpUnitySettings.json` ignored.
- Verify MCP connection, Unity console, and PlayMode status after changes.
- Produce a fresh Windows build once Unity/MCP is responsive enough.

Done when:

- C# compilation reports 0 errors;
- main menu opens;
- Level 1 starts;
- `+` opens tower list and selecting a tower closes it;
- pause restart and result retry reload the scene in a Windows build;
- no duplicate wave/map/time HUD remains.

## P1 — Chapter I Presentation Pass

- Recompose Chapter I camera view around coast, Trojan gate, and Greek landing routes.
- Add stronger Trojan fire identity: banners, braziers, smoke, warm light.
- Improve build point readability and selected/hover states.
- Give core Tower-Units clearer silhouettes.
- Improve Menelaus boss aura and intro/readability.
- Improve Hector visual/cooldown feedback.
- Polish main menu, level select, pause, and result screen into one visual language.

Done when:

- Chapter I looks visually coherent at first launch;
- player can understand routes/build points without reading debug-like text;
- boss and hero are visually distinct.

## P2 — Feedback / Audio / UX

- Add tower attack SFX and hit/death SFX.
- Add better projectile trails, hit flashes, death effects, and Fire Tower burn feedback. Initial typed projectile trails and tower recoil are implemented.
- Split music/SFX volume settings.
- Add color-independent route/threat indicators.
- Tune UI for Russian and English labels at 16:9 and common windowed resolutions.
- Review runtime telemetry after one real 1x Chapter I run.

Done when:

- combat feels responsive without overwhelming the screen;
- important state changes have sound and visual feedback;
- UI remains readable in both languages.

## P3 — Commercial Polish Expansion

- Replace remaining placeholder/generated primitives with production-ready assets.
- Add simple animations for enemy movement/death and tower attack cadence. Initial tower attack recoil is implemented.
- Add pooling for enemies, projectiles, and transient VFX.
- Add first random battlefield events only after Chapter I core pacing is stable.
- Prepare Chapter II content only after Chapter I passes final RC gates.

Done when:

- Chapter I is a credible store-page/demo slice;
- performance remains stable under intended enemy density;
- project is ready for Chapter II production without rewriting core systems.
