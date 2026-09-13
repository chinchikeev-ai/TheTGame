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
| Runtime combat HUD | `ModernCombatHud` | Legacy `GameUIController` |

## Character visual resolution order

1. Production prefab under `TroyProduction/...`.
2. Generated faction placeholder under `TroyCharacters/Factions/...`.
3. Procedural runtime fallback.

Do not add a second legacy `Resources` lookup path for the same character identity.
