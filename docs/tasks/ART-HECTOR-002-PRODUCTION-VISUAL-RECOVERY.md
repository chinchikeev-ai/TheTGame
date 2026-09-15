# ART-HECTOR-002 — Hector Production Visual Recovery

Status: SOURCE CONTRACT IMPLEMENTED — UNITY VISUAL QA PENDING

## Problem

`The Troy Game/Art/Build Missing Chapter I Art` could regenerate `Hero_Hector.prefab`, apply the Hector silhouette refinement and animation binder, and still leave Hector with the generic generated equipment from `CartoonCharacterPrefabBuilder`.

The authored candidate passes for the round Trojan shield, horse emblem, Dendra-inspired cuirass, boar-tusk-inspired helmet and pinned production spear lived in separate builders. The normal generated-art recovery command did not re-apply those passes after rebuilding the core character set.

The generic generated-art validator also checked only prefab/controller existence. It could therefore report success while Hector was missing required visual elements.

## Canonical Hector contract

Per `docs/ART_BIBLE.md`, Hector must read as:

- Heroic Large;
- long spear;
- large Trojan round shield;
- red/gold crest;
- cape;
- noble/confident commander hierarchy;
- readable above regular Trojan soldiers from the real gameplay camera.

The current implementation remains a generated/source candidate, not final production art.

## Implemented recovery stack

After base character/controller generation, the standard Chapter I recovery pipeline now re-applies Hector in this order:

1. pinned production spear candidate when the already-imported local CC0 source is available;
2. authored Aegean round shield candidate;
3. six-part Trojan horse shield emblem;
4. authored Dendra-inspired cuirass candidate;
5. authored boar-tusk-inspired helmet candidate;
6. Hector-specific silhouette refinement (mantle, cape, pauldrons, belt, pteruges, greaves, reinforced crest);
7. Hector-specific animation binding.

The spear recovery path is intentionally offline-safe: it does not download anything. If the pinned Quaternius spear source is not imported, the generated procedural spear remains and validation reports the production spear as missing. The explicit source-install/equipment commands remain responsible for source installation.

## Validation contract

`The Troy Game/Characters/Validate Hector Production Visual` now verifies the generated prefab for:

- `SourceSpear_Quaternius_MedievalWeapons`;
- `Socket_SpearRelease`;
- `SourceShield_LateBronzeAge`;
- all six `HectorHorse*` emblem parts;
- `SourceArmor_DendraCandidate`;
- `SourceHelmet_BoarTuskCandidate`;
- `HeroCrest`;
- mantle / breast gold;
- three cape panels + gold edge;
- left/right pauldrons;
- hero belt + gold buckle;
- five pteruges;
- left/right greaves;
- crest gold base;
- Animator + runtime controller;
- root gameplay `CapsuleCollider`;
- absence of extra decorative colliders.

`The Troy Game/Art/Validate Generated Chapter I Art` includes these Hector-specific problems in its output instead of treating prefab existence as sufficient.

## Idempotence fix

The authored shield pass no longer multiplies the existing authored shield scale on every rebuild. If `SourceShield_LateBronzeAge` is already present, its current scale is preserved on re-application.

## Scope / non-goals

This task does not:

- mark Hector `DONE`;
- commit the generated local `Hero_Hector.prefab` as final art;
- replace the KayKit-derived base body with a bespoke final model;
- claim the generated character matches a beauty-shot concept one-to-one;
- change gameplay stats, navigation, damage, ability timing or collider dimensions;
- perform Play Mode visual acceptance.

The art direction remains `Trojan War × Cartoon × Grotesque Heroic Comedy × Selective Sex Appeal`; the gameplay camera, not a realistic concept-art render, is the production baseline.

## Required manual QA

After pulling the change in a Unity workspace:

1. ensure the KayKit submodule is checked out;
2. install the pinned Chapter I spear source if validation reports it missing;
3. run `The Troy Game/Art/Build Missing Chapter I Art`;
4. run `The Troy Game/Characters/Validate Hector Production Visual`;
5. inspect `Hero_Hector.prefab` for duplicate/offset equipment and clipping;
6. verify the large round horse-emblem shield, spear, red/gold crest, cape, shoulders, pteruges and greaves read at gameplay zoom `7..13`;
7. verify Basic/Q/E/R/F, Shield Wall, spear throw and down/revive presentation in real Play Mode;
8. confirm only the intended gameplay collider affects physics.

## Verification status

Source-level pipeline and tests were updated. Unity Editor, EditMode tests and Play Mode were not executed in the current environment. Hector remains `GENERATED PLACEHOLDER + SOURCE ONLY` until the repository acceptance gate and real visual QA pass.
