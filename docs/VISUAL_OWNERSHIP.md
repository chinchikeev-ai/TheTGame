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
| Main combat HUD layout/content, controls, wave strip, build details | `ModernCombatHud` | Legacy combat/build canvases; secondary layout owner |
| Combat HUD decorative sprites/colors/unique icons | `TroyCombatHudSkin` | Panel position/size/anchors; duplicate content/icons |
| Chapter I objective/tutorial guidance | `ChapterOneGuidancePresentation` | Legacy `ChapterFlowUI` canvas |
| Hector health and ability HUD | `HectorHUD` | Main combat resources/build UI |
| Boss health/mechanics HUD | `BossHUD` | Main combat resources/build UI |
| Wave intro/boss warning card | `ChapterOneWavePresentation` | Persistent wave status bar |
| Visual next-wave enemy cards | `VisualWavePreviewPresentation` | General combat HUD |
| Combat notifications | `CombatNotificationPresentation` | Objective/tutorial card |
| End-of-battle menu/result shell | `GameMenuController` | Extra result canvases |
| Detailed end-of-battle metrics inside `MenuCanvas/EndMenu` | `ResultScreenPresentation` | Independent result screen/canvas |

## Compatibility-only UI

These classes must not create visual canvases. They remain only so older serialized references or shared state do not break:

- `GameUIController`
- `ChapterFlowUI`
- `CampaignProgressUI`
- `BuildDefenseInfoPresentation`
- `CombatControlsUI` — speed state/actions only; graphics belong to `ModernCombatHud`.

`ExtendedBalanceUI` is dormant and is not auto-created; its enemy-hover inspector is a separate optional diagnostic surface, not part of the canonical combat HUD.

## Decorator rule

A decorator may skin an object owned elsewhere, but it must not become a second layout or content builder. `TroyCombatHudSkin` may replace panel/button sprites and colors, replace the existing `CoinIcon` sprite, and add non-overlapping semantic icons. It must not resize/re-anchor HUD panels or create a second icon for information already rendered by `ModernCombatHud`.

## Character visual resolution order

1. Production prefab under `TroyProduction/...`.
2. Generated faction placeholder under `TroyCharacters/Factions/...`.
3. Procedural runtime fallback.

Do not add a second legacy `Resources` lookup path for the same character identity.
