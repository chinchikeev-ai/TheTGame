# The Troy Game — Trojan Unit Production Sheets

Status: production-facing companion to `TROJAN_UNIT_VISUAL_BIBLE.md`.

These sheets define concrete silhouette, proportion, costume, color and animation targets for the first Trojan production units. They are intended to guide concept, modeling, rigging, animation and gameplay-camera QA.

---

# 1. Hector — Trojan Hero

## Gameplay role

Player-controlled named hero. Mobile elite defender and visual quality benchmark for the Trojan faction.

## Scale and proportions

- Relative height: 1.08–1.15x standard Trojan infantry.
- Shoulder width: broad, but not bodybuilder-comic.
- Head: slightly enlarged for gameplay readability, but less exaggerated than comic support units.
- Torso: strong inverted triangle.
- Legs: powerful and armored; must remain readable under cape/skirts.
- Overall read: heroic, expensive, disciplined.

## Head and face

- Mature male, 30–40.
- Strong jaw, medium-to-large nose, focused eyes.
- Controlled beard or short full beard.
- Brows should suggest authority rather than rage.
- Face must remain distinct from generic infantry.

## Helmet

- Trojan/heroic bronze helmet with strong top silhouette.
- Crest/plume should create a readable direction line from gameplay view.
- Avoid overly narrow historical proportions that disappear at distance.

## Armor

- Fully armored visual target.
- Bronze cuirass / segmented or layered heroic treatment.
- Shoulder and chest masses should create a premium silhouette.
- Leg armor should be visible from angled top-down camera.
- Use red cloth to break up bronze masses.

## Shield

- Large round shield.
- Must remain a major silhouette element from top-down view.
- One horse emblem is acceptable on the shield.
- Additional horse motifs should be limited to one or two small supporting details elsewhere, not repeated across every surface.

## Spear

- Longer and visually cleaner than a standard infantry spear.
- Spearhead slightly exaggerated for gameplay readability.
- Needs a clearly defined hand grip and release socket for spear-throw gameplay.

## Cloth and cape

- Red cape is a hero identifier.
- Cape should have a large readable shape but must not cover weapon/shield silhouettes.
- Secondary red/cream cloth around the waist can support motion.

## Palette

Primary:
- Trojan red
- warm bronze
- dark leather

Secondary:
- muted cream
- selective gold highlights

Avoid large cool-blue areas.

## Idle pose

- Upright and calm.
- Shield ready but not fully braced.
- Spear carried with confidence.
- Head scans battlefield with controlled movement.

## Basic attack

- Compact disciplined spear thrust.
- Clear anticipation and contact frame.
- Minimal flailing.

## Ability character

- War Cry: large chest/open-arm expansion.
- Shield Wall: lower stance, shield becomes dominant.
- Spear Throw: strong torso rotation and clean release frame.
- For Troy!: heroic upward/forward gesture rather than magical wizard pose.

## Camera QA

At target gameplay zoom, confirm:
- Hector reads instantly among standard infantry.
- red cape, shield and spear remain separable;
- helmet crest remains visible;
- face is not required for class recognition.

## Rejection criteria

Reject if:
- silhouette is too similar to Trojan Spearman;
- armor is sparse or barbarian-like;
- shield is too small;
- cape blocks weapon readability;
- hero looks comic/foolish rather than heroic.

---

# 2. Trojan Spearman

## Gameplay role

Baseline melee defender. Visual reference for ordinary Trojan military personnel.

## Scale and proportions

- Relative height: 1.0x infantry baseline.
- Medium shoulder width.
- Compact torso.
- Slightly enlarged hands/head for stylized readability.
- Less heroic V-taper than Hector.

## Head and face

- Adult 25–40.
- Ordinary soldier face.
- Strong variation set should exist across instances: moustache, short beard, clean-shaven, different noses/brows.
- Never reuse Hector's head as the base.

## Helmet

- Simpler bronze helmet than Hector.
- Smaller crest or no major plume.
- Silhouette should still distinguish Trojan from Greek units.

## Armor

- Medium armor.
- Bronze/leather chest protection.
- Less decoration than Hector.
- Clear red or ochre cloth panels.

## Shield

- Medium round shield.
- Large enough to identify defensive infantry without becoming Guard-class scale.

## Spear

- Straight, long, simple.
- Spearhead readable but less ornate than Hector’s.

## Palette

- Trojan red / ochre cloth.
- bronze armor.
- warm brown leather.

## Idle pose

- Military formation stance.
- Spear angle creates strong diagonal silhouette.
- Shield at ready height.

## Attack

- Direct thrust.
- Short anticipation.
- Fast recover to line-holding posture.

## Camera QA

Must read as “basic Trojan spear infantry” immediately.

Check that:
- shield is smaller than Guard shield;
- body is lighter than Guard;
- costume is simpler than Hector;
- spear does not visually merge into body.

## Rejection criteria

Reject if:
- it looks heroic enough to compete with Hector;
- it looks like a Guard with smaller shield;
- body is too thin / archer-like;
- face and helmet are copies of another class.

---

# 3. Trojan Archer

## Gameplay role

Baseline ranged defender.

## Scale and proportions

- Relative height: 0.98–1.03x standard infantry.
- Narrow shoulders and waist.
- Long-looking arms.
- Light legs and minimal heavy armor.
- Overall center of mass should feel mobile.

## Head and face

- Adult 20–35.
- Narrower jaw than infantry.
- Alert eyes and expressive brows.
- Variants may include youthful moustache, short hair, light beard.

## Helmet / headwear

- Light helmet or cloth/bronze hybrid.
- Must not share the same heavy crest silhouette as melee classes.

## Armor

- Light torso protection.
- More fabric, less metal.
- Open shoulder/arm movement for bow readability.

## Bow

- Primary class identifier.
- Bow arc must remain clearly visible from top-down view.
- Slight exaggeration in bow thickness/curve is acceptable.

## Quiver

- Mandatory visual marker.
- Must protrude enough to read from gameplay camera.
- Arrow fletching may use warm faction accent.

## Palette

- ochre / red cloth.
- lighter bronze use.
- dark leather bow/quiver.

## Idle pose

- Slightly crouched or offset stance.
- Bow held actively rather than hanging vertically.
- Head tracks targets quickly.

## Draw / release

- Clear draw anticipation.
- Nocked arrow visible during ready state when practical.
- Strong release snap.
- Quick recover/redraw.

## Camera QA

Check:
- bow remains visible against ground and nearby units;
- quiver is obvious;
- body remains visibly lean compared with Spearman;
- ranged role reads without needing projectile VFX.

## Rejection criteria

Reject if:
- body is as bulky as infantry;
- bow disappears against torso;
- quiver cannot be seen;
- armor makes him look like a melee soldier.

---

# 4. Trojan Guard / Shield Bearer

## Gameplay role

Heavy defender / tank. Holds lanes and visually communicates durability.

## Scale and proportions

- Relative height: 0.98–1.05x standard infantry.
- Width: 1.20–1.35x standard infantry shoulder/body mass.
- Thick legs and forearms.
- Short visual neck.
- Lower center of gravity.

## Head and face

- Mature veteran, 30–50.
- Broad jaw, heavy nose, strong brow.
- Thick moustache or beard options.
- Face should feel experienced and stubborn.

## Helmet

- Heavy helmet shape.
- Less tall than Hector; broader and denser.
- Can use side plates or cheek guards to increase head mass.

## Armor

- Dense heavy bronze/leather layers.
- Large shoulder/chest masses.
- Thick greaves.
- Avoid excessive ornament; weight matters more than luxury.

## Shield

- Dominant class identifier.
- Oversized round / tower-influenced Trojan shield.
- Should cover a large portion of torso in brace pose.
- Must remain clearly larger than Spearman shield.

## Secondary weapon

- Short spear or short sword.
- Weapon should remain secondary to shield in silhouette.

## Palette

- deep red.
- darker bronze.
- dark leather.
- limited gold.

## Idle pose

- Heavy planted stance.
- Shield already forward.
- Feet wider than other soldiers.

## Brace / block

- Significant body compression.
- Shield rotates into dominant frontal plane.
- Should visually communicate “immovable”.

## Attack

- Short, forceful stab / shove.
- Heavy recover.

## Camera QA

Check:
- class reads from shield silhouette alone;
- Guard appears heavier than Spearman even without scale comparison;
- face/head mass differs from Hector;
- weapon does not visually compete with shield.

## Rejection criteria

Reject if:
- it is just a taller Spearman;
- shield is medium-sized;
- silhouette is elegant or athletic;
- stance lacks weight.

---

# 5. Priest of Apollo

## Gameplay role

Support unit: heal, buff and sacred fire/sun effects depending on gameplay implementation.

## Scale and proportions

- Relative height: 0.88–0.95x standard infantry.
- Shoulder width: 0.72–0.82x standard infantry.
- Thin arms and legs.
- Slight stoop.
- Larger robe/cloth shape than body mass.

## Head and face

- Age 55–75.
- Long/narrow face.
- Visible wrinkles and eye bags.
- Grey/white beard and hair.
- Calm stern expression rather than madness.

## Headwear

- Laurel, cloth wrap, small ritual headpiece or bald/grey-haired silhouette.
- Must not resemble soldier helmet.

## Clothing

- Long robes/tunics.
- Cream, pale ochre and gold with small Trojan red ties.
- Cloth should create a thin vertical silhouette.

## Props

Choose a clear primary prop:
- staff;
- sacred bowl;
- solar disc;
- ritual flame vessel.

Avoid carrying multiple equally strong props.

## Idle pose

- Hands held in ritual-ready position.
- Slow breathing and subtle robe movement.
- Head slightly forward, observant.

## Cast / channel

- Deliberate hand arcs.
- Upright extension during peak cast.
- Solar/fire accent should originate from prop/hands clearly.

## Camera QA

Check:
- visibly shorter/thinner than military units;
- old age reads in portrait/close view;
- support role reads from prop and robe silhouette;
- no confusion with Fire Thrower.

## Rejection criteria

Reject if:
- body is muscular or broad;
- face looks middle-aged heroic;
- armor dominates costume;
- stance resembles infantry.

---

# 6. Fire Keeper / Fire Thrower

## Gameplay role

Special ranged area-damage unit throwing burning bottles.

## Scale and proportions

- Relative height: 0.96–1.02x standard infantry.
- Narrow-to-medium torso.
- Forward lean.
- Asymmetric shoulder line.
- Large hands for bottle readability.

## Head and face

- Age 30–50.
- Wild eyes.
- Strong asymmetry in expression.
- Burnt, messy or uneven hair/beard.
- Wide grin, clenched teeth or delighted manic expression.

## Clothing

- Partial protective leather/cloth.
- Sooted/burnt edges.
- Straps, bottle loops and ragged details.
- Avoid full military uniform.

## Primary weapon

- Burning bottle / fire flask.
- Flame at bottle mouth or ignited rag must read clearly.

## Ammunition silhouette

- Multiple bottles on belt, harness or satchel.
- Bottle cluster should distinguish him before attack starts.

## Palette

- Trojan red/brown base.
- strong orange/yellow flame accent.
- black/soot details.

## Idle pose

- Restless weight shifts.
- Small twitchy head/hand motion.
- May admire flame or shake bottle impatiently.

## Throw

- Big overcommitted arc.
- Slight comic imbalance on release.
- Fast recovery into excited ready pose.

## Camera QA

Check:
- flame/bottle is readable at target zoom;
- body language differs radically from Priest;
- satchel/ammo remains visible;
- silhouette communicates instability.

## Rejection criteria

Reject if:
- unit looks like a soldier with torch;
- expression is neutral;
- bottle ammunition cannot be seen;
- body posture is symmetrical and disciplined.

---

# 7. Ballista Crew

## Gameplay role

Support crew operating the ballista as a living defense emplacement.

## Composition rule

At least two visibly different crew silhouettes must be present. Three is preferable if performance and staging allow it.

### 7.1 Master Ballistician

- Height: 0.94–1.00x standard infantry.
- Build: thin / wiry.
- Age: 35–55.
- Face: narrow, focused, irritated.
- Props: tool, crank handle, aiming pointer or firing lever interaction.
- Motion: fast hands, impatient commands.

### 7.2 Mechanism Assistant

- Height: around infantry baseline.
- Build: sturdy/average.
- Role read: reloads, pulls mechanism, handles bolt.
- Motion: practical, repetitive, efficient.

### 7.3 Heavy Loader (optional)

- Height: 1.00–1.08x.
- Build: broad/heavy.
- Role read: carries large bolt or tensions mechanism.
- Motion: slow power, occasional comic strain.

## Crew palette

Shared Trojan red/bronze family, but each crew member should have different cloth/armor distribution.

## Camera QA

Check:
- crew does not read as clones;
- ballista remains main object, crew supports it;
- reload/fire roles are visible through placement and pose.

## Rejection criteria

Reject if:
- same body/head duplicated;
- all crew idle identically;
- crew silhouette obscures ballista mechanism.

---

# 8. Cyclops

## Gameplay role

Elite mythic heavy thrower using large rocks.

## Scale and proportions

- Relative height: 1.65–1.95x standard infantry depending on final camera test.
- Shoulder width: extreme.
- Hands: oversized.
- Feet: oversized.
- One arm may be slightly larger/more dominant to support asymmetry.
- Neck: short and thick.

## Head and face

- One large central eye is the primary facial marker.
- Heavy brow ridge.
- Broad mouth/jaw.
- Face should feel monster-like, not like a human with an eye painted on forehead.

## Clothing

- Minimal rough cloth/leather.
- Rope, hide and primitive straps.
- Do not dress him like Trojan infantry.

## Primary prop

- Large boulder.
- Boulder must be oversized enough to communicate class from gameplay view.

## Idle pose

- Heavy breathing.
- Weight shifts through hips/shoulders.
- Slight hunch.
- May hold or pick up rock depending on state.

## Throw

- Long anticipation.
- Full body rotation.
- Massive release.
- Slow recovery with visible inertia.

## Camera QA

Check:
- scale feels mythic but does not block entire lane;
- single eye remains readable in hero/inspection views;
- boulder is obvious from gameplay camera;
- silhouette differs from all humanoid units before facial detail is visible.

## Rejection criteria

Reject if:
- model is simply a scaled human;
- body is symmetrical and athletic;
- rock is too small;
- costume resembles regular Trojan armor.

---

# 9. Shared modeling rules

## Silhouette-first modeling

Before detail sculpting, every unit must pass a flat-shaded gameplay-camera test.

Required views:
- actual Chapter I gameplay camera;
- 3/4 character presentation view;
- front silhouette;
- side silhouette.

## Exaggeration budget

Exaggerate the features that communicate role:

- Hector: cape, helmet, shield, spear.
- Spearman: spear/shield relationship.
- Archer: bow/quiver.
- Guard: shield and width.
- Priest: thin old body and ritual prop.
- Fire Thrower: bottles/flame/face posture.
- Cyclops: scale, eye, hands and boulder.

Avoid adding decorative detail that does not improve role read.

## Material direction

Keep materials stylized and readable:

- broad value groups;
- controlled highlights;
- minimal noisy microdetail;
- bronze, leather, cloth and wood should separate by color/value as well as roughness;
- avoid photoreal PBR as the dominant look.

## LOD / gameplay readability

Fine detail may disappear at distance. Every critical role marker must survive downsampling and gameplay zoom.

---

# 10. Shared rigging and animation rules

- Preserve weapon and shield sockets as explicit transform points.
- Keep release sockets for arrows, spears and thrown bottles.
- Use stable feet and readable root motion even if gameplay movement is code-driven.
- Avoid tiny finger/face motions as the only source of character personality.
- Anticipation, contact/release and recovery must be readable in body silhouette.
- Animation timing must preserve gameplay authority already present in runtime systems.

---

# 11. Production order

## Phase A — benchmark four

1. Hector
2. Trojan Spearman
3. Trojan Archer
4. Trojan Guard

These four establish the faction's hero, baseline infantry, ranged and heavy-defense language.

## Phase B — support / specialty

5. Priest of Apollo
6. Fire Keeper / Fire Thrower
7. Ballista Crew

## Phase C — mythic

8. Cyclops

Do not produce all classes in parallel before the Phase A silhouettes are approved.

---

# 12. Acceptance checklist per production unit

Before a production unit can be proposed for final runtime use:

- [ ] silhouette approved from actual gameplay camera;
- [ ] body type clearly distinct from adjacent classes;
- [ ] head/face not reused from another class without meaningful redesign;
- [ ] faction palette compliant;
- [ ] primary weapon/prop readable;
- [ ] idle pose supports role;
- [ ] attack/cast/throw motion concept supports role;
- [ ] gameplay sockets defined where relevant;
- [ ] no major clipping in key poses;
- [ ] materials fit stylized art direction;
- [ ] integration does not change gameplay authority;
- [ ] final asset still passes repository-wide `MODEL_ART_INVENTORY.md` DONE criteria.
