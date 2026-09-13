# Character Art Direction

## Factions

### Greeks / Achaeans
- Cooler bronze presentation with blue accents.
- Cleaner military silhouettes.
- Heavy hoplites and bosses read as disciplined invading forces.
- Chapter I production candidates are generated under `Assets/Game/Art/Characters/Resources/TroyProduction/Characters/Greek`.
- Archers use a bow silhouette; avoid generic medieval crossbows in the Chapter I baseline.

### Trojans
- Warmer bronze, red and gold accents.
- More regal silhouettes for guards and heroes.
- Chapter I reference/production candidates are generated under `Assets/Game/Art/Characters/Resources/TroyProduction/Characters/Trojan`.

## Heroes

Hero prefab slots:
- `Hero_Hector` — Trojan, warm bronze/red/gold, long spear, large shield, hero cape.
- `Hero_Achilles` — Greek, brighter heroic bronze/gold; later-campaign candidate.
- `Hero_Menelaus` — Greek commander, colder blue/bronze accent, one-handed command sword, royal shield, commander cape.

Generated candidates live under:

`Assets/Game/Art/Characters/Resources/TroyProduction/Characters/Heroes`

## Build workflow

1. Run `git submodule update --init --recursive`.
2. Open Unity.
3. Run `The Troy Game > Characters > Build Chapter I Production Candidates`.
4. Play the game and verify Hector, enemy archetypes, Trojan defenders and Menelaus at gameplay camera distance.
5. Promote an asset to `DONE` only after the acceptance gate in `docs/MODEL_ART_INVENTORY.md` is satisfied.

The runtime retains older generated and procedural fallbacks, so missing production candidates do not break gameplay.

## Historical direction

Chapter I should read as stylized Late Bronze Age / Mycenaean-inspired warfare rather than Classical Spartan or medieval fantasy. Prefer spears, bronze cuirasses, large shields, simple crested helmets, cloth faction accents and bow silhouettes. Final authored art may replace all current kitbash geometry while preserving these role silhouettes.
