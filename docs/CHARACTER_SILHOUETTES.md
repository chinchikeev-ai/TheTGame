# Character silhouettes and equipment

The cartoon character pipeline now differentiates roles by silhouette, scale, faction tint and equipment instead of color alone.

## Greek attackers
- Infantry: spear + round shield
- Runner: dagger, lighter scale
- Heavy Hoplite: heavy sword + badge shield
- Shield Bearer: spear + oversized round shield
- Archer: ranged weapon + quiver
- Commander/Boss: hero sword + spiked shield + crest

## Trojan defenders
- Infantry: spear + round barbarian shield
- Guard: sword + square shield + crest
- Archer: ranged weapon + quiver

## Heroes
- Hector: long spear + Trojan shield + red crest
- Achilles: hero sword + round shield + gold crest
- Menelaus: command sword + royal shield + blue crest

## Pipeline
Run `The Troy Game > Characters > Build All Cartoon Prefabs` after the KayKit submodule has been initialized.

Equipment is attached to discovered hand/spine/head bones when available. If expected bones are not found, the builder falls back to the prefab root instead of failing. The spear is generated procedurally because the current KayKit source pack does not include a spear model.

Every generated character gets `CharacterVisualIdentity` metadata with faction, role, character id and equipment labels. Gameplay systems do not depend on this component yet, so this remains a visual-only layer.
