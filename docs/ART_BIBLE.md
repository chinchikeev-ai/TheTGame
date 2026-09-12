# TheTroyGame — Art Bible

Last reviewed: 2026-09-12

## Core Identity

**Troy = Fire.**

The player is defending a bronze-age city whose visual language becomes hotter, more dramatic, and more mythological as the campaign approaches the fall of Troy.

Troy should feel:

- fortified;
- disciplined;
- heroic;
- bronze and sunlit;
- increasingly smoky, ember-lit, and desperate.

Greece should feel:

- cooler by contrast;
- organized and numerous;
- sea-borne in Chapter I;
- increasingly elite, siege-heavy, and mythological over time.

## Palette

Primary Troy palette:

- bronze;
- gold;
- dark red;
- terracotta;
- charcoal;
- fire orange.

Usage:

- bronze: armor, tower hardware, UI borders;
- gold: important text, highlights, rewards, divine emphasis;
- dark red: banners, main menu buttons, danger/action accents;
- terracotta: stone, walls, roads, warm environment surfaces;
- charcoal: shadows, smoke, panel backgrounds;
- fire orange: flames, selected states, VFX, late-campaign escalation.

Avoid letting the game become one flat orange/brown screen. Use blue sea/sky, pale stone, dark smoke, and Greek cloth/armor to create contrast.

## Chapter I — The Landing

Mood:

- warm coastal daylight;
- Trojan city behind the defense;
- Greek pressure arriving from the sea;
- readable beachhead routes;
- first sparks of the larger siege.

Required first-viewport signals:

- Troy / Trojan gate or wall;
- coastline / Greek landing direction;
- visible build positions;
- warm fire/bronze UI accents;
- clear enemy approach paths.

## UI Direction

Menus:

- large heroic title;
- stone/bronze/gold treatment;
- dark red primary action;
- readable against illustrated background;
- avoid plain prototype blue panels.

Combat HUD:

- quiet, readable, anchored zones;
- top-left: economy and gate state;
- top-center: current wave/event and preview;
- right-side: compact build `+` and tower list;
- selected tower panel on the right, separated from build picker;
- no duplicate map/wave/time displays.

Text:

- Russian and English must fit the same UI containers.
- Important commands should be short.
- Result screens can be denser, but combat HUD should stay scannable.

## Tower-Unit Visual Rules

Troy does not build abstract turrets. Defenses are **Tower-Units**:

`Build Position -> Trojan Unit / Crew / Creature -> Functions as Tower`

Core silhouettes:

- Archer Tower: light, fast, human archers, red/gold accents;
- Ballista: large wooden/bronze machine, crew identity, heavy bolt;
- Priests of Apollo: bright solar/gold support presence;
- Spear Wall: first close-defense infantry post, short poke radius, visible spears;
- Fire Tower: fire pots, pitch, orange VFX, smoke;
- Trojan Guard: shield wall/blocking soldiers;
- Cyclops later: large mythic artillery silhouette.

## Enemy Visual Rules

Greek enemies should be readable by role:

- infantry: simple mass unit;
- runner: faster/lighter silhouette;
- shield bearer/heavy hoplite: shield/armor mass;
- archer: ranged posture;
- battering ram: siege object, not a reskinned infantry;
- Menelaus: boss silhouette, command aura, distinct health presentation.

## VFX Direction

Minimum Chapter I VFX:

- projectile trail;
- impact flash;
- death feedback;
- tower muzzle/release feedback;
- Menelaus aura;
- War Cry pulse;
- Shield Wall placement;
- fire/ember accents on Trojan UI and environment.

Fire should be useful feedback, not only decoration.

## Audio Direction

Music states:

- menu / preparation: calmer antique mode;
- battle: more percussion and pressure;
- boss: heavier, more dramatic layer.

SFX priorities:

- bow release / arrow hit;
- Ballista tension and bolt impact;
- fire pot / burning ground;
- shield/armor impact;
- wave horn;
- boss intro;
- menu confirm/back.

## Asset Rules

- Track Unity `.meta` files for every committed asset.
- Keep local MCP/user machine settings out of Git.
- Store production game assets under `Assets`, not only in external reference folders.
- Reference folders like `Pictures` can stay local unless explicitly needed as source material.
