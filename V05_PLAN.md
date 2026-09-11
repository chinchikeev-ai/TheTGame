# TheTGame v0.5 — Quality Pass

Base: `main` at `ca8f4e3` (v0.2).
Branch: `feat/v0.5`.

## Goal

Convert the current runtime-generated prototype into a cleaner Tower Defense foundation without adding unnecessary mechanics.

## Scope

### 1. Runtime UI -> proper Unity UI
- Replace `GameHUD.OnGUI()` with a Canvas-based HUD.
- Top-left: Coins, Base HP, Wave.
- Bottom: tower/build controls placeholder area.
- Top-right: pause/settings entry point placeholder.
- Right-side contextual panel for selected build point/tower in later iterations.
- Responsive anchoring for 16:9 and 16:10.

### 2. CameraController
- WASD / arrow-key pan.
- Mouse-wheel zoom.
- Optional edge-pan.
- Clamp movement to map bounds.
- Smooth pan/zoom damping.
- Keep gameplay raycasts correct after camera movement.

### 3. Full map layout
- Move map-generation responsibilities out of `GameBootstrap` into `MapBuilder`.
- Separate Ground, Road, Base, Waypoints and BuildPoints.
- Define explicit map bounds for camera clamping.
- Preserve the current single-path gameplay for this milestone; architecture should allow multiple paths later.

### 4. Build points
- Replace free placement anywhere on Ground with explicit `BuildPoint` nodes.
- Hover highlights an available point.
- Click builds on that point only.
- Occupied points cannot be reused.
- Keep current tower price and TowerFactory integration.

### 5. Tower range indicator
- Selecting/hovering a tower shows a world-space range ring.
- Range ring is hidden when selection changes or the pointer leaves the tower.
- Indicator reads the actual `Tower.range` value.

### 6. Next-wave panel
- `EnemySpawner` exposes wave state instead of only running an opaque coroutine.
- HUD shows current wave and next-wave summary.
- Add a short inter-wave countdown.
- Show enemy count and HP/speed multipliers for the next wave.
- Preserve automatic wave progression for v0.5; manual Start Wave can be introduced after this quality pass.

## Refactor targets

Current v0.2 responsibilities to split:

- `GameBootstrap`: bootstrap only; create managers and call map/UI setup.
- `GameHUD`: replace IMGUI with Canvas UI controller.
- `TowerPlacement`: build-point interaction instead of arbitrary ground placement.
- `EnemySpawner`: expose wave/countdown data for UI.

New scripts:

- `CameraController.cs`
- `MapBuilder.cs`
- `BuildPoint.cs`
- `TowerRangeIndicator.cs`
- `GameUIController.cs`

## Acceptance criteria

- No gameplay HUD is rendered through `OnGUI()`.
- Camera pans and zooms smoothly and stays inside map bounds.
- Towers can only be built on visible build points.
- A tower's range can be inspected visually.
- HUD shows Coins, Base HP, current wave and next-wave/countdown information.
- Existing v0.2 projectiles, tower preview/creation logic and enemy HP bars continue working or are cleanly adapted.
- Project compiles without Console errors in Unity 6.

## Non-goals for v0.5

- New tower classes.
- Upgrade/Sell.
- Bosses.
- Multiple routes.
- Save system.
- Final art assets.
- Large audio/VFX pass.

These should come after the UI/map/camera foundation is stable.
