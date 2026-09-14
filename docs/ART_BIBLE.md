# TheTroyGame — Art Bible

Last reviewed: 2026-09-14

## Core Identity

**Trojan War × Cartoon Tower Defence × Adventure Comedy.**

**Troy = Fire.**

The player is defending a Bronze Age city whose visual language becomes hotter, more dramatic, and more mythological as the campaign approaches the fall of Troy. The presentation must stay colorful, exaggerated, readable and characterful rather than realistic or grim-dark.

Troy should feel:

- fortified;
- disciplined;
- heroic;
- bronze and sunlit;
- colorful and theatrical;
- increasingly smoky, ember-lit, and desperate.

Greece should feel:

- cooler by contrast;
- organized and numerous;
- sea-borne in Chapter I;
- visually bold and readable by silhouette;
- increasingly elite, siege-heavy, and mythological over time.

`CREATIVE_DIRECTION.md` is the canonical tone contract. Historical research defines visual vocabulary, not realistic proportions.

## Style Contract

Characters, structures and props should use:

- stylized cartoon proportions;
- oversized helmets, shields, weapons and readable props where useful;
- simplified, strong shape language;
- expressive faces and body poses;
- exaggerated anticipation, recoil and hit reactions;
- controlled squash/stretch where rigs allow it;
- high readability at gameplay camera distance;
- saturated but disciplined color separation between factions and roles.

Avoid:

- photorealism;
- grim-dark desaturation;
- generic medieval-fantasy silhouettes;
- over-detailed armor that collapses at gameplay zoom;
- muddy brown/gray environments;
- solemn historical-simulator presentation.

## Canonical In-Game Presentation Baseline

The **current Chapter I gameplay view is the production baseline**. Art must be designed for the game that already exists rather than redesigning the game around a separate concept illustration.

Current visual/readability contract:

- top-down / isometric-like orthographic gameplay camera;
- default Chapter I camera uses approximately `73°` pitch and `10.6` orthographic size;
- normal gameplay is viewed from tactical distance, not cinematic close-up distance;
- map scale, route readability, gate objective placement and unit scale are treated as working constraints;
- the world should read as a **stylized tactical board / toy battlefield** built from simple, bold 3D forms;
- current primitive/procedural geometry is acceptable as blockout and runtime fallback, but final art should replace the technical-prototype look with authored stylized meshes, stronger material language and better silhouette hierarchy;
- all final decisions are judged first from the real gameplay camera. Beauty-shot readability is secondary.

Do **not** use a separately painted 2D/isometric concept map as a new camera/layout target. Concept art may inform palette, props, silhouettes and atmosphere only. It must not force a redesign of the established Chapter I gameplay view unless a dedicated gameplay task explicitly changes camera or map layout.

### Visual-layer replacement rule

Improve the existing game by replacing or refining presentation layers while preserving gameplay structure:

- keep routes, gameplay colliders, spawn logic, tower placement and objective logic stable unless a gameplay task says otherwise;
- replace or refine character prefabs, tower-unit visuals, environment meshes, materials, props, VFX and UI presentation;
- decorative additions should avoid gameplay colliders unless collision is intentionally part of gameplay;
- simple geometry remains desirable where it supports readability and performance, but it should no longer look like unstyled colored primitives.

## Tone and Comedy

Humor is primarily visual and physical, not constant joke dialogue.

Preferred devices:

- expressive preparation and recovery poses;
- oversized equipment used for readable silhouette and comic timing;
- surprised, frustrated, panicked or triumphant reactions;
- broad boss entrances and hero gestures;
- small environmental gags that never obscure tactical state.

Comedy must not erase the stakes. Chapter VII remains emotionally serious in outcome even though the visual language stays stylized.

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

Avoid letting the game become one flat orange/brown screen. Use blue sea/sky, pale stone, dark smoke, Greek cloth/armor and brighter faction accents to create contrast.

## Chapter I — The Landing

Mood:

- bright warm coastal daylight;
- adventurous opening energy;
- Trojan city behind the defense;
- Greek pressure arriving from the sea;
- readable beachhead routes;
- playful character acting during landing and combat;
- first sparks of the larger siege.

Required first-viewport signals:

- Troy / Trojan gate or wall;
- coastline / Greek landing direction;
- visible build positions;
- warm fire/bronze UI accents;
- clear enemy approach paths;
- immediately readable cartoon-adventure identity.

## UI Direction

Menus:

- large heroic title;
- stone/bronze/gold treatment;
- dark red primary action;
- readable against illustrated background;
- stylized rather than photorealistic decoration;
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

Tower crews should have readable cartoon acting: draw, brace, heave, recoil, celebrate or panic where appropriate without delaying gameplay logic.

At the current tactical camera, a defense must be identifiable before the player reads its small decorative details. The primary silhouette should communicate its role: spear cluster, bow/quiver, large ballista frame, bright solar shrine, fire source, or shield wall.

## Enemy Visual Rules

Greek enemies should be readable by role:

- infantry: simple mass unit;
- runner: lighter, more elastic silhouette;
- shield bearer/heavy hoplite: shield/armor mass;
- archer: clear bow/quiver posture;
- battering ram: siege object, not a reskinned infantry;
- Menelaus: oversized commander silhouette, command aura, distinct health presentation.

Role readability must come from silhouette and equipment before color.

## Animation Direction

Animation should favor:

- strong line of action;
- readable anticipation → action → recovery;
- exaggerated but fast hit reactions;
- broad hero/boss gestures;
- controlled squash/stretch where technically safe;
- clear telegraphs at gameplay camera distance.

Gameplay timings remain authoritative. Visual exaggeration must not desynchronize damage timing or mislead the player.

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

VFX should be bold, readable and slightly theatrical. Fire should be useful feedback, not only decoration.

## Audio Direction

Music states:

- menu / preparation: lighter adventurous antique mode;
- battle: more percussion and pressure;
- boss: heavier, more dramatic layer;
- finale: dramatic treatment with reduced overt comedy.

SFX priorities:

- bow release / arrow hit;
- Ballista tension and bolt impact;
- fire pot / burning ground;
- shield/armor impact;
- wave horn;
- boss intro;
- menu confirm/back.

SFX may use slightly exaggerated impact and movement accents when they improve readability and comic timing.

## Production Acceptance

A candidate is off-direction if it is technically correct but reads as realistic, grim, generic medieval fantasy, visually solemn, too detailed to read at gameplay distance, or only works in a close-up while failing in the current gameplay camera.

Production-art `DONE` requires consistency with:

- `CREATIVE_DIRECTION.md`;
- this Art Bible;
- `CHARACTER_ART_DIRECTION.md` where applicable;
- `MODEL_ART_INVENTORY.md` acceptance gates.

## Asset Rules

- Track Unity `.meta` files for every committed asset.
- Keep local MCP/user machine settings out of Git.
- Store production game assets under `Assets`, not only in external reference folders.
- Reference folders like `Pictures` can stay local unless explicitly needed as source material.
