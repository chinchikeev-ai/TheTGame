# Third-party character assets

This directory contains or references externally maintained art packs used as source material for TheTroyGame.

## Included now

### KayKit Character Pack: Adventurers 1.0
- Source: https://github.com/KayKit-Game-Assets/KayKit-Character-Pack-Adventures-1.0
- License: CC0 1.0
- Integration: Git submodule at `Assets/ThirdParty/KayKitAdventurers`
- Intended use: stylized/cartoon humanoid base, weapons, shields and animation source for Greek/Trojan units.

After cloning the project, initialize third-party content with:

```bash
git submodule update --init --recursive
```

## Approved but not vendored yet

### Kenney Mini Characters
- Source: https://kenney.nl/assets/mini-characters
- License: CC0 1.0
- Intended use: extra stylized crowd/unit variants if needed.

### Quaternius Universal Base Characters
- Source: https://quaternius.com/packs/universalbasecharacters.html
- License: CC0 1.0 for the free Standard release.
- Intended use: hero-grade stylized humanoid bases and retargetable animation-compatible characters.

## Project rule

Do not modify upstream packs in-place. Production-ready Troy assets should be derived into `Assets/Game/Art/Characters/...` and keep source provenance in this file.
