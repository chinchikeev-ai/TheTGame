# TheTroyGame — Troy Defense Units

Last reviewed: 2026-09-14

This document defines the **design language and roster intent** for Trojan defenses. It is not an implementation-status document. Current implementation truth lives in `PROJECT_STATUS.md`; art completion lives in `MODEL_ART_INVENTORY.md`.

## Faction identity

**Troy = bronze fortification + fire + Apollo + mythological escalation.**

Core visual language:
- bronze;
- dark red;
- gold;
- charcoal;
- embers and burning pitch;
- solar / Apollo symbolism.

The defense should look like an army occupying fortified positions, not a collection of abstract fantasy turrets.

Canonical visual rule:

`Build Position → visible Trojan unit / crew / war machine → Tower-Unit gameplay role`

## Chapter I implemented roster

Chapter I currently uses exactly six repeatable defensive roles:

| Defense | Runtime type | Primary role | Current presentation hook |
|---|---|---|---|
| Archer Post | `MachineGun` | fast ranged / anti-light | `Draw` / `Release` |
| Ballista | `Cannon` | anti-heavy / boss | `Fire` / `Reload` / `Tension` |
| Priests of Apollo | `Slow` | support / slow | `Cast` / `Channel` |
| Spear Wall | `SpearThrower` | short-range anti-heavy | `Poke` |
| Fire Tower | `FireTower` | AoE / Burn | `Throw` / `Stoke` |
| Trojan Guard | `TrojanGuard` | block / frontline control | `Block` / `Poke` |

Important naming rule: runtime enum `SpearThrower` is legacy/internal; the player-facing Chapter I concept is **Spear Wall**. It attacks with short melee spear thrusts and does not throw javelins.

### Archer Post
- basic ranged DPS;
- strong against light infantry;
- later Fire Arrow specialization is allowed by the design, but is not part of the current Chapter I specialization system.

### Ballista
- slow, high-impact anti-heavy / anti-boss defense;
- intended later counters: siege engines and heavily armored targets;
- current mechanism presentation is procedural, not final authored production art.

### Priests of Apollo
- support/control role;
- current Chapter I implementation emphasizes slow/support;
- future progression may add armor weakening or solar/fire amplification.

### Spear Wall
- first basic close-defense option;
- short poke radius;
- anti-heavy / anti-chariot design intent;
- no thrown projectile in the Chapter I baseline.

### Fire Tower
- signature Trojan elemental defense;
- AoE / Burn / area-denial role;
- Burn is the core fire-status identity for Troy.

### Trojan Guard
- physical blocking squad;
- creates kill zones for ranged defenses;
- deterministic block capacity/reservations are part of the current gameplay implementation.

## Upgrade philosophy

Current gameplay supports three core levels. The longer-term design may add post-Level-3 specializations.

Examples of allowed future directions:
- Archer Post → Fire Arrows / Twin Volley;
- Ballista → Siege Breaker / piercing line;
- Spear Wall → heavy spear / heated spearhead;
- Fire Tower → larger burn zone / Inferno;
- Trojan Guard → larger formation / Royal Guard;
- Priests of Apollo → stronger solar support / armor weakening.

These are design targets, not currently implemented specialization branches.

## Fire combat identity

Burn should remain the signature Trojan elemental status.

Potential sources across the full campaign:
- Fire Tower;
- Fire Arrows;
- heated Ballista bolts;
- environmental fire;
- Apollo artifacts/powers;
- later mythological artillery.

Not every defense should apply Burn. Fire should create a network of synergies rather than convert all damage to fire.

## Future / later-campaign defensive concepts

The following are **design concepts only unless `PROJECT_STATUS.md` explicitly says otherwise**:

### Cyclops Artillery
Heavy AoE artillery using thrown stones. Possible late specialization: volcanic/burning boulder.

### Scylla
Unique mythological multi-target defense. Intended as a legendary special deployment, not a repeatable normal tower.

### Charybdis
Unique mythological control/vortex defense. Intended to pull/slow groups and create AoE setup opportunities.

Current design rule for the two unique sea creatures:
- maximum 1 Scylla per map;
- maximum 1 Charybdis per map;
- duplicates are not allowed.

These concepts must not be described as implemented until they exist in runtime data/gameplay and are reflected in `PROJECT_STATUS.md`.

## Campaign escalation

Expected progression:

**Early:** Archers, Spear Wall, Ballista, Guard.  
**Mid:** Fire Tower, Priests, stronger siege/formation counters.  
**Late:** mythological defenses, Divine Powers and Artifacts where the GDD/roadmap calls for them.

## Documentation authority

When documents disagree:
1. `PROJECT_STATUS.md` — implementation truth.
2. `GDD.md` — game-design truth.
3. this file — Trojan defense design detail.
4. `MODEL_ART_INVENTORY.md` — art/model completion truth.

Do not infer implementation from a concept described here.