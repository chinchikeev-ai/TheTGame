# Character Art Direction

## Factions

### Greeks / Achaeans
- Cooler bronze/steel presentation with blue accents.
- Cleaner military silhouettes.
- Heavy hoplites and bosses read as disciplined invading forces.
- Runtime enemy prefabs are generated under `Assets/Resources/TroyCharacters/Factions/Greek`.

### Trojans
- Warmer bronze, red and gold accents.
- More regal silhouettes for guards and heroes.
- Reference troop prefabs are generated under `Assets/Resources/TroyCharacters/Factions/Trojan` for later use by defensive units and cinematics.

## Heroes

Hero prefab slots:
- `Hero_Hector` — Trojan, warm bronze/red/gold.
- `Hero_Achilles` — Greek, brighter heroic bronze/gold.
- `Hero_Menelaus` — Greek commander, colder blue/bronze accent.

Generated under `Assets/Resources/TroyCharacters/Heroes`.

## Build workflow

1. Run `git submodule update --init --recursive`.
2. Open Unity.
3. Run `The Troy Game > Characters > Build All Cartoon Prefabs`.
4. Play the game and verify Hector, enemy archetypes and boss silhouettes.

The runtime retains primitive fallbacks, so missing generated prefabs do not break gameplay.
