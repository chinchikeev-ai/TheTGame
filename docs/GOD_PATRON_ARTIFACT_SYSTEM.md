# TheTroyGame — Gods, Cups & Artifacts System

## 1. Purpose

Between campaign maps, the player receives a mythological choice that changes the tactical rules of the next battle and, in some cases, the rest of the campaign.

The system adds a light roguelite layer to the 2-hour story campaign without turning the game into a full roguelike.

Core structure:

**7 campaign maps × 4 artifact offers = 28 unique artifacts.**

The first divine choice happens immediately before / during the opening landing on the coast, so the player starts Chapter I already under the protection of a chosen god.

After each subsequent chapter, a new divine selection scene is shown before the next map.

---

## 2. Player Flow

### Step 1 — The Gods Appear

Before a map, four gods appear as four large mythological cards / statues / portals.

Each god communicates a gameplay direction rather than revealing an exact artifact.

Examples:
- Ares — aggression, damage, melee, risk;
- Athena — defense, tactics, range, control;
- Apollo — accuracy, archery, healing, light;
- Poseidon — knockback, slowing, battlefield disruption.

The player selects **one god as Patron of the Map**.

### Step 2 — The Four Cups

After selecting a god, the camera moves to an altar with ritual cups.

The player chooses **one sealed cup**.

The cup does not show the exact artifact beforehand.

Possible presentation:
- bronze cup;
- silver cup;
- black obsidian cup;
- golden cup.

The cup is opened / broken / filled with divine light.

### Step 3 — Artifact Reveal

A short reveal animation plays.

The god grants a named mythological artifact.

The player sees:
- artifact name;
- icon / 3D model;
- god who granted it;
- gameplay effect;
- duration: `THIS MAP` or `CAMPAIGN`;
- optional drawback.

### Step 4 — Map Begins

The selected god becomes the Patron of the Map.

The artifact modifier is applied before the first preparation phase.

---

## 3. Design Rule

The system should not be four visible stat upgrades where the mathematically strongest choice is obvious.

The player first chooses a **playstyle / god**, then accepts uncertainty through the cup reveal.

Therefore the emotional sequence is:

`Choose a God → Choose a Cup → Divine Reveal → Adapt Strategy`

The god choice is informed.
The exact artifact is partially hidden.

This makes the ritual meaningful and gives replay value.

---

## 4. Patron of the Map

The selected god provides a small guaranteed patron bonus for that map in addition to the artifact.

Recommended patron bonuses are intentionally modest, around 5–10%, because the artifact is the main reward.

Examples:

| God | Patron Direction |
|---|---|
| Zeus | lightning / emergency power |
| Athena | defense / tactical control |
| Ares | raw combat damage |
| Apollo | archery / accuracy / support |
| Artemis | range / critical strikes |
| Poseidon | slow / knockback / disruption |
| Hephaestus | towers / upgrades / construction |
| Hermes | speed / economy / cooldowns |
| Aphrodite | morale / charm / support |
| Hera | resilience / protection |
| Hades | death / kill rewards / sacrifice |
| Demeter | sustain / regeneration / economy |
| Dionysus | chaos / random bonuses |
| Nike | momentum / wave-completion bonuses |

Gods can reappear in later chapters, but **all 28 artifacts are unique**.

---

# 5. The 28 Artifacts

## MAP I — THE LANDING

The first selection happens before the Greek landing begins.

### 01. Apollo — Golden Arrow of Apollo
**Type:** Map

Effects:
- Archer Tower damage +20%.
- Archer projectile speed +25%.
- First shot against a full-health enemy has +30% critical chance.

Design purpose: teaches the player the value of ranged defense.

### 02. Athena — Aegis Fragment
**Type:** Map

Effects:
- Gate / base HP +25%.
- Trojan Guard damage resistance +15%.
- The first enemy that would breach the gate is stunned for 3 sec.

Design purpose: forgiving defensive opening.

### 03. Ares — Blood Spear
**Type:** Map

Effects:
- Hector and Trojan Guard damage +25%.
- Melee kills restore a small amount of Command Points.
- Defensive towers receive -5% attack speed.

Design purpose: aggressive hero-oriented opening with a tradeoff.

### 04. Hermes — Sandals of the Messenger
**Type:** Map

Effects:
- Hector movement speed +35%.
- Ability cooldowns -15%.
- Early-wave start bonus +15% gold.

Design purpose: mobility and active play.

---

## MAP II — ROAD TO TROY

### 05. Poseidon — Trident Shard
**Type:** Map

Effects:
- Every 8th heavy tower hit creates a shockwave.
- Shockwave pushes light enemies backward.
- Slow effects are 20% stronger.

### 06. Artemis — Moon Bowstring
**Type:** Campaign

Effects:
- All ranged towers gain +7% range permanently.
- Towers attacking enemies above 80% HP gain +10% damage.

### 07. Nike — Laurel of Victory
**Type:** Campaign

Effects:
- Completing a wave without a leak gives +8% bonus gold.
- Bonus stacks as a campaign score modifier, not infinitely in combat.

### 08. Demeter — Horn of Plenty
**Type:** Map

Effects:
- Starting gold +20%.
- Enemy kill gold -5%.
- Sell refund +10 percentage points.

Design purpose: encourages rebuilding and experimentation.

---

## MAP III — THE GATES

### 09. Hephaestus — Hammer of the Forge
**Type:** Campaign

Effects:
- First upgrade of every tower is 12% cheaper.
- Repair actions are 20% cheaper.

### 10. Hera — Crown of the Queen
**Type:** Map

Effects:
- Gate HP +30%.
- Nearby towers gain +10% attack speed while the gate is above 75% HP.

### 11. Zeus — Thunderbolt Splinter
**Type:** Map

Effects:
- Unlocks one manual divine strike per wave.
- Lightning damages and briefly stuns enemies in a small area.
- Bosses cannot be fully stunned; they are slowed instead.

### 12. Hestia — Eternal Hearth Ember
**Type:** Campaign

Effects:
- Structures regenerate 1% of max HP between waves.
- Fire-based Trojan defenses gain +8% burn duration.

---

## MAP IV — HEROES OF GREECE

### 13. Ares — Helm of War
**Type:** Map

Effects:
- Hector damage +35% against Elite / Hero enemies.
- Hector takes +10% damage from all sources.

### 14. Athena — Owl of Pallas
**Type:** Map

Effects:
- Enemy wave composition is revealed during preparation.
- Route priority is displayed before the wave starts.
- Tactical pause can be used once per wave for 3 sec.

### 15. Aphrodite — Golden Girdle
**Type:** Map

Effects:
- Enemy captains lose 20% of their aura effectiveness.
- Once per wave, a group of light enemies hesitates for 4 sec.

### 16. Apollo — Lyre of Radiance
**Type:** Campaign

Effects:
- Hero ability effects last +8% longer.
- Trojan units near Hector regenerate slowly between engagements.

---

## MAP V — THE GREAT ASSAULT

### 17. Hades — Coin of the Underworld
**Type:** Map

Effects:
- Elite and Boss kills produce +35% gold.
- Normal enemy kill rewards -10%.

### 18. Hephaestus — Chains of the Forge
**Type:** Map

Effects:
- Ballista and siege-defense towers deal +30% damage to Siege enemies.
- Their attack speed is -8%.

### 19. Artemis — Crescent of the Hunt
**Type:** Map

Effects:
- Towers gain +20% damage against enemies farther than 70% of their maximum range.
- Minimum-range enemies receive no bonus.

### 20. Dionysus — Cup of Divine Madness
**Type:** Map

At the start of each wave, one random blessing is activated:
- +20% tower attack speed;
- +25% hero damage;
- +20% gold rewards;
- +25% slow effectiveness;
- +15% tower range.

One minor random drawback is also applied.

Design purpose: controlled chaos and replayability.

---

## MAP VI — THE HORSE

This map is less combat-heavy, so artifacts modify scouting, information, preparation, and what carries into the final battle.

### 21. Hermes — Caduceus
**Type:** Campaign / Final-map carryover

Effects:
- Scouting actions complete faster.
- One additional warning about an internal breach is revealed before Chapter VII.
- Hector movement speed +10% in the finale.

### 22. Athena — Thread of Strategy
**Type:** Final-map carryover

Effects:
- Player sees two possible internal Greek spawn zones before entering Troy Burns.
- Receives two free build relocations during the finale.

### 23. Hecate — Torch of the Crossroads
**Type:** Final-map carryover

Effects:
- Hidden / alternate routes are visible earlier.
- The first newly opened enemy route in Chapter VII is delayed by 20 sec.

### 24. Tyche — Fortune's Coin
**Type:** Final-map carryover

Before Chapter VII, flip the coin and receive one large random benefit:
- +25% starting gold;
- +20% gate HP;
- +20% tower damage for first 5 min;
- +30% Command Points;
- one free high-tier tower.

No negative result; uncertainty is the cost.

---

## MAP VII — TROY BURNS

The final divine choice happens immediately before the last survival map.
These blessings are intentionally dramatic and do not need to remain balanced for future chapters.

### 25. Zeus — Last Thunder of Olympus
**Type:** Final Map

Effects:
- Three manual lightning strikes for the entire finale.
- Each strike deals massive AoE damage.
- Boss / Hero enemies receive reduced but significant damage.

### 26. Hades — Pact of the Fallen
**Type:** Final Map

Effects:
- Every 20 Trojan deaths summon a temporary Fallen Guard squad.
- Fallen troops disappear after a short duration.

Theme: the dead of Troy rise for one final defense.

### 27. Nike — Wings of the Last Stand
**Type:** Final Map

Effects:
- For every 3 minutes survived, all defenses gain +5% damage and +5% attack speed.
- Stacks until the end of the battle.

Theme: the closer Troy comes to destruction, the stronger its defenders fight.

### 28. Aphrodite — Veil of Troy
**Type:** Final Map

Effects:
- Civilians move +35% faster.
- Enemies targeting civilians hesitate briefly on first contact.
- Evacuation objective requires 15% fewer civilians to reach the safe point for minimum victory tier.

Theme: protection of Troy's people rather than its walls.

---

# 6. Artifact Duration Classes

Three duration classes are required.

### MAP
Strong modifier that lasts only for the selected map.

Use for dramatic effects that would become overpowered if stacked through the whole campaign.

### CAMPAIGN
Smaller permanent modifier retained after the map.

Campaign artifacts stack and define the player's build over the 2-hour run.

### FINAL CARRYOVER
Acquired during The Horse and specifically modifies the Troy Burns finale.

This keeps Chapter VI mechanically relevant even though it contains less traditional TD combat.

---

# 7. Cup Logic

The cups should create uncertainty without making the choice feel arbitrary.

Recommended model:

1. Player chooses one of four gods.
2. Selected god determines the artifact family / gameplay archetype.
3. Three or four cups appear.
4. Each cup contains one valid variant from that god's available artifact pool.
5. Player chooses one.
6. Artifact is revealed.

For the first production version, it is acceptable to use one authored artifact per god per chapter while preserving the cup reveal presentation.

Later versions can add artifact variants or rarity without changing the UI flow.

---

# 8. Rarity — Optional Later Layer

Do not require rarity for MVP.

If added later:

- Bronze — common / clean stat modifier;
- Silver — stronger tactical modifier;
- Gold — major rule-changing modifier;
- Divine — rare campaign-defining modifier.

The visual material of the cup can hint at rarity without revealing the exact effect.

---

# 9. UI / Presentation

The god-selection sequence should feel like a reward ceremony, not a shop menu.

Recommended sequence duration: **20–35 seconds** if the player makes decisions quickly.

### Screen A — Choose Your Patron

Four monumental god representations.

Each card shows:
- god name;
- symbol;
- 2–3-word gameplay identity;
- no exact artifact stats.

Example:

`ATHENA — DEFENSE / TACTICS`

### Screen B — The Altar

After choosing Athena, the other gods fade away.

An altar appears with cups.

Text:

**ATHENA HAS HEARD YOUR PRAYER**

**CHOOSE YOUR CUP**

### Screen C — Reveal

Cup opens in divine light.

Large artifact presentation:

**AEGIS FRAGMENT**

`Gate HP +25%`
`Trojan Guard resistance +15%`

Badge:

`THIS MAP`

Button:

**ACCEPT THE GIFT**

---

# 10. Campaign Save Data

Save the following:

```text
DivineRunState
  currentPatronGod
  currentMapArtifact
  collectedCampaignArtifacts[]
  discoveredArtifacts[]
  finalCarryoverArtifact
  godSelectionHistory[]
```

`discoveredArtifacts[]` allows an artifact codex / collection screen later.

---

# 11. Artifact Codex

The game should eventually have an **Artifacts of Olympus** collection page.

28 slots are displayed.

Before discovery:
- silhouette;
- `???`;
- associated god may remain hidden.

After discovery:
- 3D object / illustration;
- name;
- god;
- mythology note;
- gameplay effect;
- number of times selected.

This gives a completion goal beyond finishing the 2-hour campaign once.

---

# 12. Balance Principles

1. No artifact should make one tower optimal for every situation.
2. Permanent campaign bonuses should generally be weaker than map-only bonuses.
3. Strong bonuses may have a meaningful drawback.
4. Artifacts should change decisions, not only increase numbers.
5. The player must be able to win without receiving a specific artifact.
6. The final-map artifacts may intentionally be spectacular and stronger because there is no later economy to break.
7. Repeating the campaign should produce noticeably different builds.

---

# 13. Integration with the 7-Map Campaign

Campaign flow becomes:

```text
INTRO
  ↓
DIVINE CHOICE #1 — 4 gods / cups
  ↓
MAP I — THE LANDING
  ↓
DIVINE CHOICE #2
  ↓
MAP II — ROAD TO TROY
  ↓
DIVINE CHOICE #3
  ↓
MAP III — THE GATES
  ↓
DIVINE CHOICE #4
  ↓
MAP IV — HEROES OF GREECE
  ↓
DIVINE CHOICE #5
  ↓
MAP V — THE GREAT ASSAULT
  ↓
DIVINE CHOICE #6
  ↓
MAP VI — THE HORSE
  ↓
DIVINE CHOICE #7
  ↓
MAP VII — TROY BURNS
  ↓
ENDING
```

There are exactly **7 divine-choice moments** and **28 authored artifact offers** across the campaign structure.

The first one is part of the opening experience rather than an intermission, fulfilling the same system before the landing begins.
