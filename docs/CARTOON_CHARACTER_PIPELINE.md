# Cartoon Character Pipeline

The runtime enemy system keeps gameplay data and visuals separate.

## Source assets

KayKit Adventurers is mounted as a Git submodule:

`Assets/ThirdParty/KayKitAdventurers`

Initialize it after cloning:

```bash
git submodule update --init --recursive
```

## Build Chapter I production candidates

Open Unity and run:

`The Troy Game -> Characters -> Build Chapter I Production Candidates`

The editor tool scans the KayKit submodule for rigged/skinned character assets, applies faction tint/equipment plus a Late Bronze Age kitbash layer, and writes prefabs under:

`Assets/Game/Art/Characters/Resources/TroyProduction/Characters`

Generated Chapter I names include:

- `Enemy_Infantry`
- `Enemy_Runner`
- `Enemy_HeavyHoplite`
- `Enemy_ShieldBearer`
- `Enemy_Archer`
- `Enemy_Boss`
- `Trojan_Infantry`
- `Trojan_Guard`
- `Trojan_Archer`
- `Hero_Hector`
- `Hero_Menelaus`

`Hero_Achilles` is also generated as a later-campaign candidate.

`BatteringRam` remains intentionally excluded until a dedicated siege model is authored.

## Visual corrections in the production candidate pass

- Achaean and Trojan characters receive distinct bronze/cloth faction treatment.
- Heavy infantry gets a heavier cuirass/helmet silhouette.
- Archers use a bow silhouette instead of the previous generic crossbow presentation.
- Hector and Menelaus receive commander/hero capes and stronger armor silhouettes.
- Heavy Hoplite uses spear + heavy shield rather than a two-handed sword + shield combination.
- Menelaus uses a one-handed command sword + royal shield.

## Runtime behavior

`EnemyVisualFactory` and `HeroVisualFactory` first load `TroyProduction/...` resources. If a production candidate is absent, they fall back to the older generated `TroyCharacters/...` path and then to runtime procedural visuals.

This preserves gameplay while production art is replaced incrementally.

## Status rule

These generated prefabs remain `GENERATED PLACEHOLDER` in `MODEL_ART_INVENTORY.md` until final authored assets are committed, connected to final animation/materials, and pass visual QA. Merely living under `Assets/Game/Art` does not make them `DONE`.
