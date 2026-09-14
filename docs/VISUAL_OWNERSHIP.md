# Visual Ownership

One visual domain has one runtime owner. New visual work must replace or extend that owner instead of adding another overlapping builder.

## Chapter I

| Domain | Canonical owner | Must not own |
|---|---|---|
| Gameplay grid, routes, anchors, build points | `MapBuilder` | Gate/wall/city art |
| Static coast ground, sea, tents, rocks, parked ships | `CoastEnvironmentBuilder` | Foam, sea glints, fires, Troy structures |
| Dynamic shoreline life | `ChapterOneShoreLife` | Base coast geometry |
| Coastal scrub/stakes compatibility dressing | `ChapterOneVisualEnhancer` | Troy city, debris, gate/wall |
| Battlefield debris, standards, route markers | `ChapterOneBattlefieldDetails` | Coast base, Troy city |
| Greek landing ship visual | `GreekLandingShipVisualFactory` | Gameplay/cinematic timing |
| Landing cinematic | `LandingPresentation` | Independent ship geometry implementation |
| Troy central gatehouse | `TroyGateHeroBuilder` | Outer wall/city skyline |
| Troy outer wall and wall life | `ChapterOneWallLife` | Central gatehouse/city skyline |
| Troy city/citadel backdrop | `TroyCityBackdropPresentation` | Gate/wall |
| Troy fire/window/torch life | `TroyFireLifePresentation` | Structural geometry |
| Gate damage feedback | `TroyGateDamagePresentation` | Gate geometry |
| Tower gameplay object/anchors | `TowerFactory` | Visible tower geometry |
| Tower visible geometry/crew | `TowerArtDirector` | Tower stats/targeting |
| Projectile impact visuals | `CombatImpactPresentation` | Audio |
| Runtime effect audio/general non-projectile effects | `RuntimeEffects` | Projectile impact graphics |
| Main combat HUD layout/content, controls, wave strip, contextual selected-defense panel, build details, corner magic controls | `ModernCombatHud` | Secondary combat/build canvas or layout owner |
| Combat HUD decorative sprites/colors/unique icons | `TroyCombatHudSkin` | Panel position/size/anchors; duplicate content/icons |
| Settings UI including audio layout | `ModernSettingsPresentation` | Runtime layout polishers or duplicate settings canvases |
| Chapter I objective/tutorial guidance | `ChapterOneGuidancePresentation` | Secondary tutorial/objective canvas |
| Hector health and ability HUD | `HectorHUD` | Main combat resources/build UI |
| Boss health/mechanics HUD | `BossHUD` | Main combat resources/build UI |
| Wave intro/boss warning card | `ChapterOneWavePresentation` | Persistent wave status bar |
| Visual next-wave enemy cards | `VisualWavePreviewPresentation` | General combat HUD |
| Combat notifications | `CombatNotificationPresentation` | Objective/tutorial card |
| End-of-battle menu and detailed results | `GameMenuController` | Extra result canvases or compatibility result builders |

## Runtime services without visual ownership

- `CombatControlsUI` owns combat-speed state/actions only. Graphics belong to `ModernCombatHud`.
- `GameStateController` owns session-state transitions; it is not a HUD owner.

## Migration debt

These components are still active or intentionally retained during a cutover. They are not canonical end-state owners and must not accumulate new responsibilities:

- `HectorMotionFallbackAnimator` is a runtime animation safety net until Hector's production Animator is frozen. Authored animation wins when available.
- Procedural character/environment visual factories remain fallback paths until production-art freeze; they are not the preferred production source.

`CombatCornerControlsPresentation` has been removed. Its BuildDock placement, compact gift panel and corner magic UX are now owned directly by `ModernCombatHud`.

`AudioSettingsLayoutPolisher` has been removed. The audio cards, labels, sliders and percentages are now authored directly by `ModernSettingsPresentation`.

Previously retained compatibility shells and dormant legacy canvases have been removed. Do not reintroduce `GameHUD`, `GameUIController`, `ChapterFlowUI`, `CampaignProgressUI`, `BuildDefenseInfoPresentation`, `TowerContextActionHud`, `MenuSceneNavigationFix`, `ResultScreenPresentation`, `ExtendedBalanceUI`, `CombatCornerControlsPresentation`, or `AudioSettingsLayoutPolisher`.

## Editor art tooling

Generated Chapter I art is explicit tooling, not an editor-startup side effect. Use `Tools/TheTroyGame/Art/Build Missing Chapter I Art` when generated candidates need to be rebuilt. Do not add hidden `[InitializeOnLoad]` art generation.

## Decorator rule

A decorator may skin an object owned elsewhere, but it must not become a second layout or content builder. `TroyCombatHudSkin` may replace panel/button sprites and colors, replace the existing `CoinIcon` sprite, and add non-overlapping semantic icons. It must not resize/re-anchor HUD panels or create a second icon for information already rendered by `ModernCombatHud`.

## Character visual resolution order

1. Production prefab under `TroyProduction/...`.
2. Generated faction placeholder under `TroyCharacters/Factions/...`.
3. Procedural runtime fallback.

Do not add a second legacy `Resources` lookup path for the same character identity.
