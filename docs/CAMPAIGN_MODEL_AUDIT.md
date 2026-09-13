# Campaign Model Audit

`CampaignModelAuditValidator` is the machine-checkable source for current campaign model coverage.

It does not replace `docs/MODEL_ART_INVENTORY.md`. The inventory defines production meaning; the validator checks whether generated/imported assets actually exist and satisfy the structural contract.

## Statuses

- `DONE` — prefab is structurally valid **and** its exact asset path is explicitly listed in `Assets/Game/Art/PRODUCTION_ACCEPTANCE.json` after real production visual QA.
- `CANDIDATE` — asset/pipeline exists and passes structural checks, but production acceptance is not recorded.
- `MISSING` — required prefab/source/pipeline is absent.
- `BROKEN` — asset exists but violates its contract, for example missing renderer/material, required Animator/controller missing, wrong Resources path, decorative collider present, or pipeline token missing.

The acceptance manifest starts empty deliberately. Builders and recognizable procedural art never promote an asset to `DONE` automatically.

## Coverage

The audit currently covers:

- Greek/Achaean regular roster;
- Trojan combat/support roster;
- Hector, Menelaus, Achilles, Ajax and Odysseus;
- Cyclops;
- Chariot, Battering Ram, Siege Tower and Trojan Horse;
- Chapter II plains/road candidates;
- Chapter III siege breach/staging candidates;
- Chapter IV damaged defenses;
- Chapter V destruction-state candidates;
- Chapter VI Greek camp / Trojan Horse plaza;
- Chapter VII Troy interior, burning/collapsed buildings and evacuation street;
- Tower-Unit production binding and L2/L3 visual contract;
- pinned animated horse installer and chariot-horse integration;
- full candidate-set and environment builder contracts.

## Unity menu

Current assets only:

`TheTroyGame > Validation > Audit Campaign Models`

Build/generate the complete candidate set first, then audit:

`TheTroyGame > Validation > Build Candidates + Audit Campaign Models`

The build+audit route may install the pinned CC0 horse source when it is absent, so it requires network access for that first import.

## Command line

### Windows

Current generated/imported state:

```powershell
$env:UNITY_EDITOR="C:\path\to\Unity.exe"
.\tools\audit-models.ps1
```

Build candidates and then audit:

```powershell
$env:UNITY_EDITOR="C:\path\to\Unity.exe"
.\tools\audit-models.ps1 -BuildCandidates
```

### Unix

Current generated/imported state:

```sh
UNITY_EDITOR=/path/to/Unity ./tools/audit-models.sh
```

Build candidates and then audit:

```sh
BUILD_CANDIDATES=1 UNITY_EDITOR=/path/to/Unity ./tools/audit-models.sh
```

## Outputs

Generated reports are intentionally ignored by Git:

- `Logs/Validation/CampaignModelAudit.json`
- `Logs/Validation/CampaignModelAudit.md`
- `Logs/Validation/model-audit.log`

Batch mode returns a non-zero exit code when any required row is `MISSING` or `BROKEN`.

## Production acceptance rule

Do not add a path to `PRODUCTION_ACCEPTANCE.json` until all applicable `MODEL_ART_INVENTORY.md` acceptance gates have passed, including gameplay-camera readability, materials/shaders, collider policy, required animation, and real Play Mode QA.
