# TheTroyGame — Canonical Art Bible v2.0

Last reviewed: 2026-09-16

This is the **single canonical creative + visual direction authority** for TheTroyGame.

`MODEL_ART_INVENTORY.md` remains the authority for implementation/completion status. Current code remains the authority for actual runtime behavior. Task contracts may define implementation work, but they must not silently redefine the visual direction established here.

The purpose of this document is not only to answer **“what should the game look like?”**. It must also make it possible to reject a visually attractive asset that belongs to the wrong game.

---

## 0. Authority, precedence and interpretation

When visual sources disagree, use this order:

1. **This `ART_BIBLE.md`.**
2. Repository-tracked **Golden Reference Frames** explicitly approved under this document.
3. Current real gameplay camera, scale and runtime composition.
4. Feature/task contracts that reference this Art Bible.
5. Generated concept art and external references.

Generated images, moodboards and beauty shots are **reference material only** until explicitly promoted to Golden Reference status. A concept never overrides gameplay readability, camera scale or this document.

If an asset follows the theme but violates the production view, it fails.

If an asset is beautiful but looks like another game, it fails.

---

## 1. Visual Constitution

### 1.1 Canonical formula

**Trojan War × Cartoon × Grotesque Heroic Comedy × Selective Sex Appeal.**

### 1.2 Runtime presentation

**Stylized tactical board / toy battlefield.**

The game should feel immediately recognizable, stylish, funny, exaggerated and heroic.

The emotional target is:

- heroic;
- playful;
- confident;
- mischievous;
- colorful;
- theatrical;
- slightly absurd;
- sexy where intentionally assigned;
- increasingly dramatic as Troy approaches its fall.

A successful gameplay frame should communicate **Trojan War + cartoon adventure + personality** before the player studies details.

### 1.3 The five mandatory style pillars

Every major visual decision must support all applicable pillars:

1. **Vibe** — swagger, attitude, theatrical confidence.
2. **Cartoon** — exaggerated proportions and strong readable forms.
3. **Humor** — primarily physical, visual and performance-driven.
4. **Grotesque exaggeration** — appealing pushed traits, never gore for its own sake.
5. **Selective sex appeal** — intentional, adult-only, characterful and never generic pin-up design.

---

## 2. Style Lock — non-negotiable rules

These rules are the fastest visual gate for any asset, screenshot or concept.

### MUST

- Read as **Late Bronze Age / Mycenaean-inspired Trojan War** before reading as generic fantasy.
- Be visibly **cartoon-stylized** at gameplay distance.
- Use **large, deliberate silhouettes** rather than detail-dependent identity.
- Treat the battlefield as a **toy-like tactical board**, not a realistic diorama.
- Keep the real gameplay camera and current tactical scale as the production baseline.
- Keep Troy warm, fortified and theatrical.
- Keep Greeks cooler, expeditionary and slightly sharper in shape language.
- Make gameplay actors and states more visually important than decorative ground detail.
- Keep permanent HUD on the perimeter.
- Use one coherent material language across HUD: warm dark stone/wood, bronze, selective gold, Trojan red, parchment where appropriate.
- Prefer bold illustrated icons and portraits over thin software glyphs.
- Preserve generous tactical breathing room on the battlefield.

### MUST NOT

Reject work that reads primarily as:

- photoreal;
- painterly realistic RTS;
- grim-dark;
- dirty gray/brown historical simulation;
- generic medieval fantasy;
- generic mobile-fantasy clone;
- Classical-Spartan shorthand used everywhere;
- anime/manga character construction;
- polished Disney/Pixar imitation;
- cute toy style with no heroic edge;
- modern flat-app UI;
- sci-fi UI;
- glassmorphism;
- noisy micro-textured terrain;
- crowded diorama where every empty space is filled with props;
- concept-art camera that does not match real gameplay;
- technically clean but personality-free placeholder art.

---

## 3. Three visual layers — do not mix them

A major source of style drift is applying the right idea to the wrong layer. The game has three distinct but related visual layers.

### 3.1 WORLD / 3D BATTLEFIELD

Target:

**Stylized low-poly / primitive-friendly 3D upgraded into a coherent authored toy-battlefield look.**

World art should use:

- clean forms;
- broad value groups;
- readable terrain masses;
- limited low-frequency surface variation;
- simplified but recognizable materials;
- large props with strong silhouettes;
- controlled cel-like/light stylized shading where practical.

The world is **not** a painted illustration pasted under the HUD.

### 3.2 CHARACTERS / UNITS

Target:

**Exaggerated cartoon Bronze Age characters designed for small on-screen size.**

Character identity comes from:

- body mass;
- head/crest shape;
- shield/weapon/role prop;
- cape/cloth mass;
- posture;
- faction color block;
- animation rhythm.

Face detail is secondary at gameplay zoom.

### 3.3 UI / 2D PRESENTATION

Target:

**Painted Bronze Age cartoon command frame.**

UI may be richer and more illustrative than the battlefield itself:

- carved dark panels;
- bronze/gold framing;
- red cloth;
- laurel;
- parchment;
- portrait illustration;
- sculpted iconography;
- theatrical bevels and highlights.

Do **not** transfer the UI’s painterly richness onto every terrain surface.

---

## 4. Golden Reference Frames

Text alone is not enough. The project must maintain a small set of explicitly approved visual anchors.

### 4.1 Golden Reference categories

The target set is:

- Main Menu;
- Gameplay 1920×1080;
- Gameplay 1366/1376×768;
- Pause Menu;
- Settings Menu;
- Hector combat block;
- one canonical Trojan soldier;
- one canonical Greek soldier;
- one canonical Tower-Unit;
- one canonical Patron God corner presentation.

### 4.2 Current runtime anchors

Until repository-tracked reference frames are explicitly promoted:

- the existing **HectorHUD** is the strongest runtime reference for combat-HUD material language;
- the current real Chapter I camera is the authority for gameplay composition and scale;
- generated concept art is never canonical by default.

### 4.3 Promotion rule

A Golden Reference must:

1. be stored in the repository or other versioned project source;
2. be named in this section or an approved reference index;
3. be accepted from the real target camera/resolution where applicable;
4. not contradict this Art Bible.

If a reference and this document conflict, this document wins until intentionally revised.

---

## 5. Gameplay camera and scale contract

Art is built for the **current real game**, not for a separate concept-art camera.

Current Chapter I baseline:

- orthographic top-down / isometric-like tactical view;
- approximately `73°` camera pitch;
- default orthographic size around `10.6`;
- gameplay zoom roughly `7..13`;
- units are small on screen;
- the map is read as a complete battlefield, not as a cinematic close-up.

Therefore:

- silhouette matters more than face detail;
- upper body, head, weapon, shield, crest, cape and role props receive visual priority;
- tiny costume detail is secondary;
- faction/role must remain readable during movement and overlap;
- final acceptance happens from the real gameplay camera first;
- beauty-shot or portrait readability is secondary.

Do not redesign camera, map layout or gameplay scale to make a concept image work. Concept art may influence palette, props, atmosphere, silhouette and material treatment only.

---

## 6. Gameplay visual hierarchy and detail density

### 6.1 Read order

At gameplay zoom, the intended read order is:

1. faction;
2. body mass / silhouette;
3. role-defining weapon, shield or prop;
4. threat / hero hierarchy;
5. current action pose;
6. gameplay state / telegraph;
7. color accent;
8. face / personality;
9. decorative detail.

If a unit only reads because of a small face detail or tiny costume element, the design fails.

### 6.2 Three detail bands

**Primary gameplay layer**

- units;
- heroes;
- bosses;
- Tower-Units;
- projectiles;
- placement state;
- telegraphs;
- critical combat VFX.

This layer receives the highest local contrast and clearest silhouettes.

**Secondary environment layer**

- roads;
- walls;
- gates;
- ships;
- major rocks;
- tents;
- large barricades;
- large props.

Readable and attractive, but never louder than active gameplay actors.

**Tertiary decoration layer**

- small stones;
- grass tufts;
- debris;
- small barrels;
- broken boards;
- tiny environmental jokes.

Low contrast, sparse and optional. If this layer competes with units, remove it before adding more detail.

### 6.3 Breathing-room rule

Empty readable ground is a feature, not unfinished art.

The tactical board must contain visible open space between:

- lanes;
- build positions;
- units;
- props;
- walls;
- major environmental landmarks.

Never fill empty tactical space merely because it looks empty in a screenshot.

---

## 7. Historical vocabulary

The world should read as stylized **Late Bronze Age / Mycenaean-inspired Trojan War**.

Use:

- bronze and leather armor;
- spears;
- large round, figure-eight and tower-shield influences where appropriate;
- bows and visible quivers;
- simple crested helmets;
- red/ochre cloth for Troy;
- blue/pale cloth contrast for Greece;
- stone, wood, bronze, rope, cloth, fire, sand and sea;
- Bronze Age ships and fortified city forms.

Historical research defines vocabulary, not realistic proportion. Exaggeration is expected.

Avoid:

- medieval plate armor;
- generic fantasy armor kits;
- universal Spartan red-cape shorthand;
- Roman imperial visual language as the default substitute for Bronze Age forms.

---

## 8. Shape language and faction identity

Color supports faction identity. Shape must work even before color.

### 8.1 Troy

Troy = **warm, fortified, heroic, theatrical**.

Shape language:

- broad;
- stable;
- shield-like;
- blocky fortified masses;
- strong vertical banners;
- circular/sun motifs;
- thick defensive silhouettes;
- monumental gate/wall forms.

Trojan props should feel planted, defensive and weighty.

### 8.2 Greeks / Achaeans

Greece = **cooler, organized, numerous, expeditionary**.

Shape language:

- slightly sharper;
- more directional;
- spear-like;
- sail-like;
- narrower massing;
- expedition camp structures;
- repeated formation rhythm.

Greek silhouettes should feel more mobile and campaign-oriented than Trojan defenders.

### 8.3 Divine / mythic

Divine elements may use:

- circular discs;
- rays;
- stars;
- controlled glow;
- symbolic silhouettes;
- stronger symmetry.

Divine shapes must still belong to the Bronze Age cartoon world.

---

## 9. Canonical color system

The values below are reference anchors for UI, concept work and material tuning. They are not a command to flatten all materials into exact solid colors.

| Token | Reference | Primary use |
| --- | --- | --- |
| `TroyRed` | `#8F211C` | Trojan cloth, major action framing |
| `TroyDeepRed` | `#4A1715` | deep cloth/shadow/action backing |
| `Bronze` | `#A66A32` | primary metal language |
| `GoldFocus` | `#D8A43D` | selection, premium focus, active trim |
| `WarmStone` | `#D2B88F` | pale/warm stone |
| `Sand` | `#D9B875` | Chapter I ground family |
| `Charcoal` | `#241A17` | dark panel bodies / deepest UI value |
| `DarkWood` | `#4A2B1D` | UI/world timber family |
| `Parchment` | `#E5D1A4` | advice/information surfaces |
| `GreekBlue` | `#61788D` | Greek cloth/faction contrast |
| `SeaBlue` | `#2F7F9F` | Aegean water family |
| `FireOrange` | `#E66B24` | restrained fire/impact accent |
| `ReadyGreen` | `#6F8D50` | confirmation/readiness only |

Rules:

- Troy should read warmer than Greece.
- Greek blue is a faction color, not the generic interface color.
- Gold is selective; if everything is gold, nothing is selected.
- Fire orange is an accent, not the overall Chapter I grade.
- Green is functional and sparse.
- Critical state must never rely on color alone.

---

## 10. Materials and lighting

### 10.1 Rendering target

Use:

- clean material separation;
- readable cel-like/light stylized shading where practical;
- broad value groups;
- controlled roughness/specular response;
- strong form lighting;
- restrained stylized texture variation.

Avoid:

- photoreal PBR as the primary look;
- noisy texture detail;
- muddy gray/brown realism;
- uniform material response across every object;
- random saturated colors with no faction logic.

### 10.2 Material rules

**Bronze**

- warm;
- readable highlight edge;
- slightly exaggerated material identity;
- not mirror-metal;
- not rust-brown iron.

**Cloth**

- identity comes from silhouette/fold mass/color first;
- broad folds beat tiny weave texture;
- red Trojan cloth should remain visible at gameplay zoom.

**Wood**

- simple grain direction;
- clear plank/bevel construction;
- avoid high-frequency photographic grain.

**Stone**

- broad warm planes;
- selective cracks/chips;
- do not turn every surface into noisy rubble texture.

**Sand / ground**

- low-frequency variation;
- soft readable path separation;
- avoid repeating checkerboard or tile noise.

### 10.3 Lighting

Chapter I lighting should feel:

- bright;
- warm;
- coastal;
- clear;
- heroic;
- readable.

Use directional sunlight and clear cast shadows to reinforce form. Do not crush the map into dark cinematic contrast that damages gameplay readability.

---

## 11. Chapter I environment contract

Chapter I = **bright warm coastal landing under pressure**.

The full battlefield should read as:

**Aegean sea → landing beach → Greek expedition zone → two attack lanes → Trojan defense zone → wall / gate / city.**

Required scene read:

- Aegean sea on the west/landing side;
- sandy landing beach;
- two readable attack lanes;
- Greek ships/camp/landing debris;
- Troy on the east/defended side;
- wall, towers, gatehouse, banners and braziers;
- city silhouette stronger than random decoration.

Environment family:

- sand;
- pale and warm stone;
- bronze;
- red cloth;
- wood;
- rope;
- sea blue;
- sparse green scrub;
- selective smoke/fire.

### 11.1 Battlefield readability rules

- Build points remain clearly interactive but visually **secondary** when not selected.
- Repeating build-point shapes must not become the strongest pattern on screen.
- Roads/lanes remain readable without looking like debug tiles.
- Ground variation remains broad and low-frequency.
- Units, towers, projectiles and combat feedback carry stronger contrast than decorative ground detail.
- Decorative props should generally have no gameplay colliders.
- Props must support lane readability rather than visually close off usable space.
- The player should perceive **space between combat objects**, not a packed historical diorama.

`Troy = Fire` is a campaign motif, not a command to make Chapter I an orange inferno. Fire is an accent against bright coast, blue sea and pale stone.

---

## 12. Character design rules

### 12.1 Cartoon is mandatory

Use:

- stylized proportions;
- slightly oversized heads where useful;
- oversized shields, helmets, bows, spears, hands and role props;
- simplified armor layers;
- expressive faces;
- strong body-shape contrast;
- clear color blocking;
- controlled squash/stretch where technically safe.

Avoid realistic body proportion as the default target and avoid detail that only works in close-up.

### 12.2 Grotesque exaggeration

Grotesque means **appealing exaggeration**, not gore or body horror.

Each important role should have at least one intentionally pushed trait, for example:

- too-wide shoulders;
- too-large shield;
- too-long spear;
- too-thin nervous engineer;
- too-broad guard;
- too-hunched pyromaniac;
- too-grand hero crest;
- too-big Cyclops hand/boulder;
- exaggerated nose, brow, beard, jaw, posture or cape shape.

The purpose is silhouette, humor and personality.

### 12.3 Selective sex appeal

Sex appeal is a deliberate art-direction layer for **adult characters only**.

Adult female characters may use:

- confident posture;
- attractive silhouette;
- stylish costume shaping;
- expressive face and hair;
- strong body rhythm where appropriate;
- charisma, competence and attitude;
- elegant or provocative styling that still belongs to the Bronze Age cartoon world.

Adult male characters:

- reserve deliberately sexy/glamorous treatment for roughly **10%** of male designs;
- the rest of the roster should cover heroic, rugged, comic, old, heavy, nervous, strange and monstrous archetypes.

Sexy does **not** mean explicit nudity, modern fetishwear or sacrificing role readability.

Rule: **appealing first, characterful second, never generic pin-up.**

---

## 13. Character body archetypes

Use visible body-type contrast across the roster.

### Heroic Large
Tall, broad shoulders, narrower waist, upright command posture. Named heroes and elite figures.

### Standard Soldier
Medium height, sturdy, compact military stance.

### Lean Agile
Narrow torso, lighter equipment, longer-looking limbs, active posture.

### Heavy Broad
Very wide torso, dense armor mass, compressed neck, planted stance.

### Old Thin Mystic
Shorter, narrow shoulders, thin limbs, ritual posture, visibly older.

### Unstable Maniac
Asymmetric, hunched/forward posture, loose gear, dangerous energy.

### Giant Monster
Massive scale, asymmetric anatomy, huge hands/feet/shoulders, monster-first silhouette.

Do not reuse one heroic body/head across all classes.

---

## 14. Chapter I Trojan character contract

### Hector

- Heroic Large.
- Mature adult.
- Long spear, large Trojan round shield, red/gold crest, cape.
- Noble, confident, controlled.
- Strong jaw / mature heroic face.
- Full visual hierarchy must be obvious from gameplay zoom.
- Humor comes from timing/reactions, not from making Hector look foolish.
- Stronger heroic glamour is allowed, but commander/hero readability comes first.

### Trojan Spearman

- Standard Soldier.
- Spear + round shield.
- Reliable, disciplined professional soldier.
- Red/ochre, bronze, leather.
- Clearly below Hector in visual hierarchy.

### Trojan Archer

- Lean Agile.
- Large readable bow arc + visible quiver.
- Lighter armor.
- Quick, alert, energetic silhouette.
- Must not look like infantry with a bow glued on.

### Trojan Guard

- Heavy Broad.
- Huge shield + spear.
- Dense bronze mass, deep red, planted stance.
- Veteran read.
- Shield is a class identifier and may be intentionally oversized.

### Ballista Crew

Crew should read as a small cast rather than duplicated soldiers:

- nervous/thin master ballistician or engineer;
- practical assistant/loader;
- optional heavy loader for comic contrast.

Tools, ammunition and work posture matter more than soldier armor.

### Priest of Apollo

- Old Thin Mystic.
- Cream/ochre/light robes + gold/bronze.
- Sun staff/disc or sacred vessel.
- Deliberate ritual movement.
- Must not read as infantry.

### Fire Keeper / Fire Thrower

- Unstable Maniac.
- Hunched/asymmetric posture.
- Bottle bundles, ignition source, visible flame.
- Manic enthusiasm, dangerous comic energy.
- Must not look like a normal soldier holding a torch.

### Cyclops

- Giant Monster.
- One dominant eye.
- Huge body, huge arm/hand, boulder.
- Heavy full-body windup and slow recovery.
- Must not be a scaled-up normal human.

---

## 15. Greek enemy character contract

- **Infantry:** spear + round shield; simple mass-unit silhouette.
- **Runner:** lighter, narrower, more elastic and fast-looking.
- **Heavy Hoplite:** wider, more armored, strong top-heavy read.
- **Shield Bearer:** shield-dominant silhouette.
- **Archer:** bow + quiver; no crossbow substitution.
- **Battering Ram:** siege-object silhouette, not infantry with different stats.
- **Menelaus:** oversized commander silhouette, crest/cape/shield hierarchy, cooler bronze/blue presence, theatrical command energy.

Role readability comes from silhouette/equipment before color.

---

## 16. Tower-Unit visual contract

Troy does not build anonymous fantasy turrets.

Defenses are **Tower-Units**:

`Build Position -> Trojan Unit / Crew / Creature -> Functions as Tower`

At gameplay zoom each defense must be recognizable in about one second.

### Archer Tower

- human archers;
- large bow/quiver cues;
- lighter wooden platform/canopy language;
- red/gold accents;
- quick draw/release acting.

### Ballista

- large wooden/bronze siege frame;
- heavy bolt;
- visible crew roles;
- brace/heave/recoil/reload acting.

### Priests of Apollo

- shrine / solar-disc read;
- light robes + gold;
- ritual channel/cast acting;
- visually bright support role.

### Spear Wall

- visible spear cluster;
- shields;
- compact short-range defense read;
- disciplined poke/brace acting.

### Fire Tower

- fire bowl / pitch / bottles;
- orange fire and controlled smoke;
- unstable fire-keeper silhouette;
- throw/stoke acting.

### Trojan Guard

- shield-wall silhouette;
- broad armored soldiers;
- blocking/brace/poke acting.

### Cyclops later

- huge mythic artillery body;
- boulder is the role-defining prop.

---

## 17. Animation direction

Animation communicates personality and gameplay state simultaneously.

Use:

- strong line of action;
- anticipation -> action -> recovery;
- broad but fast reactions;
- confident hero gestures;
- exaggerated recoil;
- readable panic/surprise/triumph;
- controlled squash/stretch;
- role-specific motion rhythm.

Examples:

- Hector: controlled heroic weight;
- Spearman: direct disciplined thrusts;
- Archer: quick draw/release;
- Guard: brace, low center of gravity, heavy recovery;
- Priest: deliberate ritual gestures;
- Fire Keeper: twitchy overcommitted throws and delighted fire reactions;
- Cyclops: huge windup, full-body throw, slow recovery.

Gameplay timing, hit windows and telegraphs remain authoritative. Animation may exaggerate but must never lie about combat state.

Humor should come primarily from timing, overcommitment, reaction and contrast rather than constant joke text.

---

## 18. VFX direction

VFX should be bold, readable and slightly theatrical.

Minimum Chapter I language:

- projectile trails;
- impact flashes;
- death feedback;
- release/muzzle feedback;
- fire/ember accents;
- Menelaus aura;
- Hector War Cry pulse;
- Shield Wall placement/readiness;
- clear slow/burn/support feedback where needed.

VFX rules:

- gameplay communication first;
- decorative spectacle second;
- use large readable shapes rather than particle noise;
- do not hide unit silhouettes or placement state;
- important telegraphs must survive bright sand, dark walls and sea backgrounds;
- persistent glows should be restrained.

---

## 19. UI Design System

UI belongs to the same world as the battlefield. It must feel like a **painted Bronze Age cartoon command frame**, not a neutral software overlay.

The approved combat-HUD direction is perimeter-based: it frames the battlefield while leaving the tactical center readable.

### 19.1 Core visual language

Use:

- dark warm stone / charcoal / dark-brown panel bodies;
- bronze and selective gold framing;
- Trojan red cloth, ribbons and primary-action accents;
- parchment only for advice/information panels that should read as field notes;
- shield, column, spear, laurel, banner and fortress motifs;
- bold illustrated icons rather than thin software glyphs;
- warm highlights and restrained fire/ember accents;
- exaggerated but readable cartoon bevels, shadows and silhouettes;
- clear separation between frame, content and interactive state.

Avoid:

- plain prototype rectangles;
- generic blue sci-fi panels;
- flat modern-mobile cards;
- excessive gradients;
- glassmorphism;
- tiny gray text;
- large opaque blocks over the tactical center.

### 19.2 Canonical reusable component grammar

Reusable UI should be built from a small visual grammar rather than redrawing every screen independently.

Canonical components:

- `PanelDark` — dark stone/wood system panel;
- `PanelParchment` — temporary advice/information;
- `ButtonSecondary` — stone/wood neutral action;
- `ButtonPrimary` — Trojan red + gold primary action;
- `TabNormal` — neutral category tab;
- `TabSelected` — brighter gold/amber selected tab;
- `PortraitFrameHero`;
- `PortraitFrameEnemy`;
- `PortraitFrameGod`;
- `ResourceChip`;
- `AbilityTile`;
- `DefenderCard`;
- `BossBlock`;
- `Tooltip/ContextCard`;
- `EncounterBanner`;
- `BuildPointMarker`.

Every interactive component should define at least:

- normal;
- hover/focus;
- pressed;
- selected where applicable;
- disabled/locked;
- cooldown where applicable.

### 19.3 Text and icons are separate assets

For reusable UI components:

- **do not bake labels into PNG backgrounds**;
- **do not bake language-specific text into reusable buttons**;
- use TextMeshPro/localized runtime text;
- keep icons separate when they need to change by context/state;
- use 9-sliced or otherwise scalable frames where practical.

Baked text is allowed only for intentional fixed artwork such as a logo, emblem or approved one-language decorative title where localization is not required.

This rule applies to AI-generated UI assets as well.

---

## 20. Combat HUD layout contract

### 20.1 Overall geometry budget

The battlefield must remain the dominant visual surface.

Target budgets at 16:9:

- permanent HUD should normally occupy **no more than ~25%** of total screen area;
- if permanent HUD approaches **30%**, it requires explicit review;
- the central tactical region must remain visually open;
- only encounter/boss information may meaningfully project into the upper-center field;
- contextual panels must collapse or disappear when their information is no longer needed.

These are composition budgets, not pixel-perfect runtime assertions. Readability and interaction remain authoritative.

### 20.2 Top-left — economy and utility cluster

- Gold is the first and most immediate value.
- **Speed and Settings sit directly to the right of Gold in the same horizontal cluster.**
- Speed shows active value (`1x`, `2x`, etc.) without opening a menu.
- Settings is compact.
- Gate/Fortress health sits below and is visually heavier than utility buttons.
- Gold, Speed and Settings must not be scattered into different corners.

### 20.3 Top-center — encounter state

- Encounter number/progress and timer occupy the strongest top-center banner.
- Remaining enemies / next pressure message belongs inside or directly below the same banner.
- Boss banner/health appears directly below when active.
- Boss portrait, name, role, HP and key mechanics belong to one coherent block.
- Boss information must not be duplicated in another permanent panel.

Target: encounter banner should normally remain under roughly **42% screen width** and **12% screen height** before a temporary boss extension.

### 20.4 Upper corners — Patron God presence

- Patron Gods are illustrated personalities integrated into the corner, not passport cards.
- God looks diagonally inward toward battlefield center.
- Favor three-quarter face/body angle.
- Outer frame may crop at the screen edge.
- Divine readiness, patron name and one short state line may integrate into the composition.
- Glow/rays stay near the corner and must not wash out the tactical center.

### 20.5 Left-middle — contextual advice

- Tutorial/boss/advice appears below the gate cluster.
- Temporary/contextual, not a permanent wall of text.
- Parchment treatment preferred.
- Keep away from the primary enemy lane where possible.
- Prefer short copy over smaller font.

### 20.6 Bottom-left — Hector / hero state

- Hector gets a strong portrait and **compact** status block.
- Show only immediate decision information: availability/downed, HP/respawn and critical ability state where needed.
- Flavor text is one short line maximum.
- When downed, failure state is unmistakable.

Target at 1920×1080:

- roughly **≤18% screen width**;
- roughly **≤22% screen height**;
- compactness takes priority over decorative expansion.

### 20.7 Bottom-center — defender deployment strip

- Main tactical selection bar.
- Unit cards use role icon/portrait, hotkey, name and Gold cost.
- Selected defender gets clear gold/orange active treatment.
- Locked/unaffordable remains readable but visually quieter.
- Strip feels like a Trojan command dais.
- It may collapse/reduce when no build interaction is needed.

### 20.8 Bottom-right — Magic and Defenders

The lower-right corner is reserved for two major action openers:

- **MAGIC**;
- **DEFENDERS**.

Rules:

- the two controls should be approximately equal in size and visual weight;
- both are compact high-recognition illustrated buttons;
- Magic uses divine/sun/fire/lightning language;
- Defenders uses shield/helmet/formation/fortress language;
- ready/cooldown/locked state is visible directly on the control;
- Speed and Settings never belong here.

### 20.9 Battlefield visibility rule

The HUD frames the fight; it does not sit on top of it.

- Keep central tactical area open.
- Permanent HUD hugs edges/corners.
- Do not cover build-circle centers, hero movement space, primary lanes or boss telegraphs.
- If two HUD blocks compete, lower-priority content collapses, moves or hides.

Player attention order:

1. battlefield threats and placement;
2. boss/encounter state;
3. gate survival;
4. gold and available actions;
5. defender selection;
6. Hector state;
7. patron/magic readiness;
8. advice/flavor text.

---

## 21. UI state, typography and icon rules

### 21.1 State language

- **Trojan red / dark red:** primary action framing, danger, damage pressure, important hostile state;
- **gold / bronze / warm orange:** selected, active, important, interactable, premium focus;
- **pale parchment / warm cream:** readable information surface;
- **green:** readiness/confirmation only, use sparingly;
- **desaturated gray/brown:** unavailable, locked, cooldown or secondary;
- **Greek blue/cool accents:** faction context, never generic HUD chrome.

Never use color alone for a critical state. Pair it with icon, label, fill, shape, contrast or motion.

### 21.2 Typography

- bold, compact, high-readability display text for major labels;
- uppercase allowed for major combat labels;
- rapidly changing numbers are larger and cleaner than prose;
- avoid long sentences in permanent HUD;
- Russian and English use the **same target containers**;
- rewrite copy before shrinking text aggressively;
- boss names, encounter numbers, Gold, Gate HP and hotkeys remain readable at `1366×768`.

### 21.3 Icons and portraits

- use one illustrated icon family;
- avoid mixing flat line icons with painted portraits;
- role-defining prop/silhouette beats face detail on unit cards;
- magic icons use bold emblem shapes;
- portraits preserve helmet/crest/hair/weapon cues;
- hero/god portraits feel like personalities, not inventory thumbnails.

### 21.4 Interaction motion

Recommended button feedback:

- hover/focus: `1.04–1.06x` plus warm highlight;
- press: about `0.92x` compression;
- release: short overshoot near `1.08x`, then settle;
- use unscaled time where menus can pause gameplay.

Use stronger motion only for meaningful state changes:

- boss arrival;
- Divine Power ready;
- Hector downed/returning;
- Gate danger;
- newly unlocked action.

Do not make everything pulse continuously.

---

## 22. Responsive layout and safe areas

Primary composition is authored at **1920×1080 / 16:9** and must be validated at **1366×768** and **1376×768**.

Rules:

- anchor edge UI to real screen edges/safe areas;
- preserve zone ownership across resolutions;
- top-left utility cluster stays together;
- top-center encounter block stays centered;
- Patron God stays corner-attached and looks inward;
- bottom-center defender strip stays centered;
- Magic/Defenders remain bottom-right;
- Hector remains bottom-left;
- collapse decoration before shrinking critical interaction targets;
- do not solve low resolution by shrinking all text into unreadability.

Each fact has one primary home:

- Gold: top-left;
- Speed/Settings: immediately right of Gold;
- Gate health: top-left below utility;
- Encounter/timer: top-center;
- Boss: central boss block;
- Hector: bottom-left;
- Defender selection/cost: bottom-center;
- Patron Divine state: Patron corner and/or Magic surface without duplicated full descriptions;
- Magic/Defenders entry points: bottom-right.

---

## 23. Menu-screen direction

Pause, Settings, Main Menu and other blocking surfaces must use the same world language as combat HUD.

### 23.1 Menu composition

- one dominant focal panel rather than many competing rectangles;
- dark wood/stone/bronze base;
- red cloth and laurel as selective decoration;
- large readable controls;
- strong separation between decorative frame and runtime text;
- background art may remain visible but subdued.

### 23.2 Settings-specific rule

Settings UI should be composed from reusable components:

- blank header art + TMP title/subtitle;
- blank normal/selected tabs + separate icons + TMP labels;
- empty content frame + runtime rows;
- blank secondary button + runtime label/icon;
- blank red primary button + runtime label/icon.

Do not generate separate baked-text PNG buttons for every language or label.

---

## 24. AI asset generation contract

Because AI is part of the production workflow, prompts must preserve the same art direction automatically.

### 24.1 Required prompt ingredients

Every art-generation request should specify, where relevant:

- `TheTroyGame` canonical style formula;
- correct visual layer: WORLD / CHARACTER / UI;
- real gameplay camera if gameplay-visible;
- faction;
- role/silhouette requirement;
- material family;
- palette family;
- gameplay readability requirement;
- output type and transparency;
- whether text/icons must be excluded.

### 24.2 Canonical style phrase

Use this as a stable prompt anchor:

> **Stylized Bronze Age Trojan War cartoon; grotesque heroic comedy; strong readable toy-battlefield silhouettes; warm bronze/red Troy vs cooler blue/pale Greek contrast; authored stylized 3D world with painted Bronze Age cartoon UI.**

### 24.3 Mandatory negative direction

When generating gameplay/world assets, explicitly avoid:

- photorealism;
- painterly realistic RTS;
- medieval fantasy;
- anime;
- hyper-detailed PBR;
- cluttered diorama composition;
- noisy ground micro-detail;
- cinematic camera changes;
- tiny realistic proportions.

When generating reusable UI sprites, explicitly include:

- `NO TEXT`;
- `NO LETTERS`;
- `NO LANGUAGE-SPECIFIC LABELS`;
- transparent background where appropriate;
- isolated reusable component;
- separate icon when state/context can change.

### 24.4 AI output is never auto-approved

AI output must pass the same gameplay-camera, faction, silhouette, composition and implementation review as manually created art.

---

## 25. Visual acceptance checklist

A visual candidate is acceptable only when the answers below are all satisfactory.

### 25.1 Five-second identity check

Within five seconds, can a reviewer tell that this is:

- Trojan War;
- cartoon;
- heroic;
- personality-driven;
- this game rather than a generic fantasy RTS?

If not, reject or revise.

### 25.2 Gameplay check

- Does it read from the real gameplay camera?
- Is faction clear?
- Is role clear in roughly one second?
- Does silhouette work before small detail?
- Is action/telegraph readable in motion?
- Is tactical ground less visually dominant than active gameplay actors?
- Is there enough breathing room around lanes/build positions?

### 25.3 Style-drift check

Reject if the result has drifted toward:

- realistic historical diorama;
- painterly RTS concept art;
- generic fantasy;
- medieval visual language;
- mobile flat UI;
- sci-fi UI;
- orange/brown monotony;
- overdecorated HUD;
- empty personality-free low-poly placeholder art.

### 25.4 UI check

- Is the battlefield still the largest visual area?
- Can Gold, Gate HP, encounter/timer and current defender be found in under one second?
- Are Speed and Settings next to Gold?
- Are Magic and Defenders together bottom-right and equal in visual weight?
- Is Hector compact and readable bottom-left?
- Is boss information centralized?
- Does contextual advice use parchment and disappear when irrelevant?
- Are RU/EN layouts readable at target resolutions?
- Are reusable backgrounds free of baked labels?

---

## 26. Audio direction

Music:

- menu/preparation: lighter adventurous antique energy;
- battle: stronger percussion and pressure;
- boss: heavier dramatic layer;
- finale: reduced overt comedy, stronger emotional weight.

SFX priorities:

- bow release / arrow hit;
- Ballista tension / release / impact;
- fire pot / burning ground;
- shield / armor impact;
- wave horn;
- boss intro;
- UI confirm/back.

SFX may be slightly exaggerated when that improves readability and comedy.

---

## 27. Campaign tonal arc

- Chapters I–II: bright, adventurous, energetic, playful.
- Chapters III–IV: larger spectacle, stronger hero personalities and mythic elements.
- Chapter V: maximum siege spectacle; comedy lives mainly in reactions.
- Chapter VI: strange calm, suspicion, deliberate tonal pause.
- Chapter VII: tragic climax; same stylized language, less overt comedy, stronger emotional weight.

The fall of Troy remains serious in outcome even though the visual language is cartoon and grotesque.

---

## 28. Production acceptance / definition of done

A visual candidate is directionally acceptable only when:

1. it reads correctly from the real gameplay camera;
2. faction is clear;
3. role is clear in roughly one second;
4. silhouette and role prop work before small details;
5. cartoon stylization is obvious;
6. at least one intentional exaggerated/personality trait is present where appropriate;
7. the asset has attitude rather than neutral prototype energy;
8. humor supports rather than obscures gameplay;
9. selective sex-appeal treatment follows this bible where applicable;
10. palette/materials fit faction and world;
11. environment does not out-noise gameplay actors;
12. HUD respects zone ownership and battlefield visibility;
13. gameplay colliders and logic remain unchanged unless a separate gameplay task requires it;
14. close-up polish does not compensate for poor gameplay readability;
15. the asset passes the relevant target-resolution check.

Production-art status is **not** assigned here. `MODEL_ART_INVENTORY.md` remains the authority for `DONE`, `GENERATED PLACEHOLDER`, `PROCEDURAL`, `SOURCE ONLY` and `MISSING`.

Final `DONE` still requires final derivative assets, runtime adoption and real visual QA according to repository rules.

---

## 29. Art workflow rule

For any new or revised asset:

1. identify the visual layer: WORLD / CHARACTER / UI;
2. design for the real gameplay camera first if gameplay-visible;
3. check faction and role silhouette;
4. apply cartoon exaggeration;
5. add one strong personality/grotesque hook where appropriate;
6. apply selective sex appeal only according to this bible;
7. verify material and palette consistency;
8. verify detail-density hierarchy;
9. verify gameplay readability in motion;
10. verify responsive UI/state rules where applicable;
11. inspect close-up second;
12. compare against Golden References;
13. only then consider production acceptance.

---

## 30. Asset implementation rules

- Track Unity `.meta` files for committed assets.
- Production game assets live under `Assets`.
- Local/reference folders do not count as runtime adoption.
- Decorative additions should avoid gameplay colliders unless intentionally gameplay-relevant.
- Runtime procedural fallbacks may remain during migration but are not final art by default.
- Reusable UI backgrounds should be scalable where practical.
- UI text should normally be runtime text, not baked into image assets.
- Language-specific duplication of identical decorative UI is discouraged.

---

## 31. Final visual test

Before approving any major screenshot, ask three questions:

1. **Does this unmistakably look like TheTroyGame?**
2. **Does the real gameplay remain clearer than the decoration?**
3. **Could another competent artist or AI reproduce the same visual family from this Art Bible without guessing the genre?**

If any answer is “no”, the visual direction is not yet sufficiently locked.