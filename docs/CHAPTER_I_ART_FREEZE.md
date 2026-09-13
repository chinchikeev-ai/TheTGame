# Chapter I Production Art Freeze

`ChapterOneArtFreezeValidator` is the strict visual-production gate for Chapter I. It intentionally separates technical integrity from human visual acceptance.

## Status semantics

- `PASS` — the requirement is technically valid and, where required, explicitly accepted.
- `BLOCKED` — the asset/pipeline is structurally usable, but required human visual QA or production acceptance is not complete.
- `BROKEN` — a required asset, material, Animator contract, data asset, source contract, or manifest is missing/invalid.

`readyForFreeze` is true only when both `BLOCKED == 0` and `BROKEN == 0`.

## Chapter I runtime roster

The gate follows the actual Chapter I runtime roster in `BalanceCatalog`:

- Greek Infantry
- Runner
- Heavy Hoplite
- Shield Bearer
- Archer
- Menelaus / Boss
- Hector

Tower-Unit support candidates required by the six defenses are also checked:

- Trojan Infantry
- Trojan Guard
- Trojan Archer
- Priest of Apollo
- Fire Keeper
- Ballista Crew

The six defense data contracts are:

- `MachineGun` — Archer Tower
- `Cannon` — Ballista
- `Slow` — Priests of Apollo
- `SpearThrower` — Spear Wall
- `FireTower`
- `TrojanGuard`

## Production acceptance

Character prefabs are not considered frozen merely because they exist. Every required production prefab must also be listed in:

`Assets/Game/Art/PRODUCTION_ACCEPTANCE.json`

Do not add a path there until the asset has passed gameplay-camera inspection and the normal `DONE` rules in `MODEL_ART_INVENTORY.md`.

## Human QA acceptance

The chapter-level freeze gates live in:

`Assets/Game/Art/CHAPTER_I_FREEZE_ACCEPTANCE.json`

All flags start as `false` and must remain false until the corresponding real Play Mode review is complete:

- `playModeVisualQa`
- `englishFramingQa`
- `russianFramingQa`
- `towerUnitVisualQa`
- `environmentVisualQa`
- `animationVisualQa`

Reference visual QA resolution is `1920x1080`.

The manifest is evidence of acceptance, not an automation target. Builders and validators must never set these booleans automatically.

## Technical checks

The validator checks:

1. existing Chapter I release-candidate contract;
2. actual Chapter I enemy roster mapping;
3. six defense type contract;
4. required production character prefabs;
5. renderers and non-null materials;
6. `CharacterVisualIdentity`;
7. Animator + controller assignment;
8. shared controller parameters `Speed`, `Attack`, `Hit`, `Die`, `IsDowned`;
9. all six authored `TowerData` assets;
10. `TowerArtDirector`, `TowerProductionArtBinder`, L2/L3 visual wiring and `TowerFactory` binding;
11. coast, Greek landing ship, landing presentation, Troy gate/damage states, skyline, atmosphere, wall/battlefield/shore life and cinematic camera source contracts;
12. explicit EN/RU, Tower-Unit, environment, animation and Play Mode QA acceptance.

## Unity menu

Audit current state:

`TheTroyGame > Validation > Audit Chapter I Art Freeze`

Generate the Chapter I character/support candidates and shared animation controller first, then audit:

`TheTroyGame > Validation > Build Chapter I Candidates + Audit Art Freeze`

The build command does not mark anything accepted.

## Batch mode

Unix:

```sh
UNITY_EDITOR=/path/to/Unity ./tools/audit-chapter1-art.sh
```

Windows PowerShell:

```powershell
$env:UNITY_EDITOR = "C:\\Path\\To\\Unity.exe"
.\\tools\\audit-chapter1-art.ps1
```

Batch mode exits non-zero while the chapter is `BLOCKED` or `BROKEN`. That is expected until real visual acceptance is complete.

## Reports

Generated locally under ignored validation logs:

- `Logs/Validation/ChapterOneArtFreeze.json`
- `Logs/Validation/ChapterOneArtFreeze.md`
- `Logs/Validation/chapter1-art-freeze.log`

A green Architecture Guard confirms only that the validator contract exists and has not been structurally weakened. It is not a substitute for Unity compilation, Play Mode, or visual QA.
