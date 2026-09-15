# AI Task Contract

## Task ID
`ART-HECTOR-003-PRODUCTION-SILHOUETTE-REFINEMENT`

## Goal
Make the generated Hector 3D candidate read as a distinct Trojan commander/hero from the real Chapter I tactical camera while keeping the existing humanoid rig, gameplay root, collider, navigation and combat contracts unchanged.

## Why
The current Hector candidate already has a KayKit body, spear, authored shield/armor candidates and animation hooks, but several silhouette pieces were root-space procedural parts. A static cape/waist/shoulder treatment can visually detach during animation and does not provide a strong enough hero hierarchy at gameplay distance.

## Owner module
`Assets/Game/Art` production-character pipeline / Editor art builders.

## Allowed files
- `Assets/Editor/HectorProductionVisualRefinementBuilder.cs`
- `Assets/Editor/CartoonCharacterAutoBuilder.cs`
- `Assets/Tests/EditMode/HectorProductionVisualPipelineTests.cs`
- corresponding `.meta` files
- this task contract

## Do not change
- Hector gameplay stats, ability behavior or timing
- gameplay root transform
- capsule collider
- navigation / pathfinding
- damage / targeting contracts
- camera
- production acceptance manifests
- `Assets/Resources/Music/BeyazGiyme.mp3`

## Inputs / source of truth
- `docs/ART_BIBLE.md` Hector contract: Heroic Large, long spear, large Trojan round shield, red/gold crest, cape, noble commander read
- `docs/MODEL_ART_INVENTORY.md`
- existing `Hero_Hector` generated pipeline
- existing Late Bronze Age shield/armor/equipment passes

## Acceptance criteria
- [x] Hector-only refinement is deterministic and reproducible.
- [x] Cape and major silhouette accents follow resolved humanoid rig bones where available.
- [x] Old root-space Hector cape/waist/shoulder procedural parts are removed before replacement.
- [x] Hero hierarchy is strengthened with oversized bronze shoulders, Trojan red cape, gold commander accents, pteruges, greaves and reinforced crest.
- [x] Existing spear, round shield/horse-emblem, authored armor and animation pipelines remain compatible.
- [x] Gameplay root/collider/navigation contracts are untouched.
- [x] EditMode source-contract coverage is added.
- [ ] Real Unity gameplay-camera visual QA confirms scale, clipping, grip, animation clearance and silhouette.
- [ ] No architecture guard violations.

## Automated validation
- [ ] `python tools/check-architecture.py` — not executable through the GitHub connector environment.
- [ ] EditMode tests — added; Unity execution not available in this environment.
- [ ] PlayMode tests — real visual acceptance still required.
- [ ] Full `tools/validate-project.ps1` or `tools/validate-project.sh` — Unity unavailable here.

## Manual validation
1. Inspect Hector at normal Chapter I camera zoom (`7..13`, baseline ~`10.6`).
2. Verify cape follows chest during idle/run/basic/Q/E/R/F/downed/revive and does not visibly detach.
3. Verify pauldrons follow arms without unacceptable shoulder clipping.
4. Verify pteruges and belt stay with pelvis and do not obscure leg motion.
5. Verify greaves track lower legs.
6. Verify shield/horse emblem, spear and crest remain the dominant identity cues.
7. Verify silhouette remains clearly above Trojan Infantry/Guard but does not become oversized relative to Menelaus/boss hierarchy.
8. Verify no visual part changes gameplay collider/navigation or projectile release behavior.

## Known risks
- Primitive/static silhouette pieces are rigid bone attachments rather than skinned cloth/armor; extreme poses can still clip.
- Hard-coded rest-pose positions are tuned as a candidate and require real Unity visual inspection.
- Final cape/armor should ultimately be authored/skinned production geometry if this candidate direction is accepted.

## Result
- Added a Hector-only rig-following silhouette refinement builder.
- Wired it into the reproducible Chapter I art auto-builder before Hector animation binding.
- Added source-contract EditMode tests for bone-following, hero cues, cleanup and pipeline ordering.
- Hector remains `GENERATED PLACEHOLDER + SOURCE ONLY`; authoritative production-art status is unchanged until real Play Mode visual QA.
- Unity/Play Mode visual QA remains pending.

## Status
`IMPLEMENTED — PLAY MODE QA PENDING`
