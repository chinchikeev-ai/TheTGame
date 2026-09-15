# AI Task Contract

## Task ID
`ART-CH1-004-GENERATED_ASSET_RECOVERY`

## Goal
`Build Missing Chapter I Art` must detect and recover any missing required generated Chapter I character prefab or animation controller, rather than relying on a small set of probe assets.

## Why
Generated production-candidate assets are created locally in Unity and are not all committed as prefab/controller files. A single surviving probe asset could previously hide missing siblings, leaving runtime art empty or incomplete without a rebuild.

## Owner module
Art / Chapter I generated character pipeline.

## Allowed files
- `Assets/Editor/CartoonCharacterAutoBuilder.cs`
- `Assets/Tests/EditMode/ChapterOneGeneratedArtRecoveryTests.cs`
- `docs/tasks/ART-CH1-004-GENERATED-ASSET-RECOVERY.md`

## Do not change
- Gameplay logic, stats, colliders, navigation or encounter timing
- Acceptance/freeze manifests
- Third-party source pins
- `Assets/Resources/Music/BeyazGiyme.mp3`

## Inputs / source of truth
- `CartoonCharacterPrefabBuilder`
- `MythicAndSupportArtCandidateBuilder`
- `ChapterOneCharacterAnimationBuilder`
- current Chapter I production-candidate paths under `Assets/Game/Art/Characters`

## Acceptance criteria
- [x] All core Chapter I generated character prefabs are individually tracked.
- [x] All Chapter I support/mythic generated prefabs are individually tracked.
- [x] All role-specific Chapter I animation controllers are individually tracked.
- [x] Any missing member triggers the owning rebuild pass.
- [x] A validation menu command reports the exact remaining missing assets after generation.
- [x] Existing Hector visual refinement and animation binding ordering is preserved.
- [x] Existing public gameplay contracts are unchanged.

## Automated validation
- [ ] `python tools/check-architecture.py` — not executed in this connector-only change.
- [x] EditMode source-contract coverage added: `ChapterOneGeneratedArtRecoveryTests`.
- [ ] Unity EditMode tests — not executed; Unity Editor was not available in this session.
- [ ] PlayMode tests — not required for source-only recovery logic; generated output still needs local Unity execution.

## Manual validation
After pulling `main` locally in Unity:
1. Run `Tools/TheTroyGame/Art/Build Missing Chapter I Art`.
2. Run `Tools/TheTroyGame/Art/Validate Generated Chapter I Art`.
3. Confirm Console reports all required Chapter I character prefabs and animation controllers present.
4. Spot-check Hector, Menelaus, Greek regulars, Trojan regulars and support units in Project/Prefab view.

## Known risks
The recovery command rebuilds complete owning sets when any member is missing. This is intentional for deterministic recovery but may overwrite uncommitted local edits to generated candidate prefabs.

## Result
- `CartoonCharacterAutoBuilder` now uses complete required-asset manifests instead of single probe files.
- Exact post-build validation command added.
- Regression/source-contract tests added.
- Unity execution remains pending locally.

## Status
`DONE`
