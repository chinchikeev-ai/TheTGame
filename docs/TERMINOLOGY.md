# TheTroyGame — Canonical Terminology

This file defines the canonical player-facing terminology for the divine progression system.

## Бог-покровитель / Patron God

One of the four main gods selected before a map.

Current primary patrons:
- Ares;
- Athena;
- Apollo;
- Poseidon.

The selected god determines the player's artifact build direction for the coming battle and the campaign run.

## Артефакты / Artifacts

**Artifacts are the Gifts of the Gods.**

Use `Artifact / Артефакт` as the primary gameplay and UI term.

Meaning:
- passive divine gifts;
- granted by a Patron God;
- alter stats, rules, synergies or tactical possibilities;
- 7 unique artifacts per god;
- 4 gods × 7 artifacts = 28 artifacts total.

Recommended UI language:

- `ПОДАРОК БОГА`
- `АРТЕФАКТ ПОЛУЧЕН`
- `ВЫБЕРИТЕ АРТЕФАКТ`
- `АРТЕФАКТЫ: 3 / 7`

Do not treat `Gift` as a separate gameplay item type. A Gift of a God is an Artifact.

## Божественные силы / Divine Powers

**Divine Powers are active battlefield abilities.**

This is the canonical replacement for the previous working term `Divine Magic / Божественная магия`.

Meaning:
- active player-triggered powers;
- used directly during battle;
- have cooldown, charges and/or Divine Power cost;
- visually spectacular;
- separate from passive Artifacts;
- separate from Hector's normal hero abilities.

The game contains **7 Divine Powers**.

Recommended UI language:

- `БОЖЕСТВЕННЫЕ СИЛЫ`
- `ВЫБЕРИТЕ БОЖЕСТВЕННЫЕ СИЛЫ`
- `БОЖЕСТВЕННАЯ СИЛА ГОТОВА`
- `СИЛА НЕДОСТУПНА`
- `БОЖЕСТВЕННАЯ СИЛА: ГРОМ ЗЕВСА`

## Божественная энергия / Divine Power

Resource spent to activate Divine Powers.

This is not an artifact and not a spell.

Possible player-facing Russian terms:
- `Божественная энергия` — preferred;
- `Благосклонность богов` — alternative for a more mythological tone.

Preferred production term: **Божественная энергия / Divine Power**.

## Final terminology hierarchy

```text
БОГ-ПОКРОВИТЕЛЬ
    ↓
АРТЕФАКТ
(подарок выбранного бога; пассивный эффект)

БОЖЕСТВЕННАЯ СИЛА
(активная способность в бою)
    ↓
БОЖЕСТВЕННАЯ ЭНЕРГИЯ
(ресурс для применения силы)
```

## Canonical system formula

**4 Бога-покровителя × 7 Артефактов = 28 Артефактов**

plus

**7 Божественных сил**

The terms `Divine Magic`, `Magic`, `Spell`, and `Божественная магия` are deprecated as primary UI/GDD terminology and should be replaced by `Divine Power / Божественная сила` whenever referring to this system.
