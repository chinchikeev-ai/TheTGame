# Cartoon Character Pipeline

The runtime enemy system keeps gameplay data and visuals separate.

## Source assets

KayKit Adventurers is mounted as a Git submodule:

`Assets/ThirdParty/KayKitAdventurers`

Initialize it after cloning:

```bash
git submodule update --init --recursive
```

## Build prefabs

Open Unity and run:

`The Troy Game -> Characters -> Build Cartoon Enemy Prefabs`

The editor tool scans the KayKit submodule for rigged/skinned character assets and creates runtime prefabs under:

`Assets/Resources/TroyCharacters`

Generated names:

- `Enemy_Infantry`
- `Enemy_Runner`
- `Enemy_HeavyHoplite`
- `Enemy_ShieldBearer`
- `Enemy_Archer`
- `Enemy_Boss`

`BatteringRam` deliberately keeps the existing fallback visual until a dedicated siege model is assigned.

## Runtime behavior

`EnemyVisualFactory` loads a prefab by archetype from `Resources/TroyCharacters`. If no generated prefab exists, the game falls back to the previous capsule/cube primitive, so missing art cannot break gameplay.

## Next art pass

The generated prefabs are placeholders for faction-specific production variants. The recommended next step is to duplicate them into Greek and Trojan variants, then add helmets, shields, spears, bows, faction colors, hero silhouettes, and retargeted locomotion/combat animations.
