# Hector HUD illustrated assets

## Goal and owner
Replace the existing HectorHUD procedural decoration with individually authored
bitmap assets matching the user's approved Trojan HUD reference and Hector portrait.
Owner: Assets/Game/Heroes/Hector/HectorHUD.cs.

## Scope
Hector HUD, its dedicated art folder, explicit editor preview and canonical status.
The Chapter I compacting pass no longer rescales HectorHUD; its owner defines its size.
No balance, saves, other HUD blocks or Windows player build changes.

## Acceptance
- Eleven separately replaceable PNG assets, with live RU/EN labels and health.
- Portrait selection and four ability commands retain their existing callbacks.
- Cooldown overlays use filled sprites and show seconds.
- Unity compilation and RU/EN isolated Canvas render, including text-fit checks.
- Architecture guard passes. Full gameplay/overlap acceptance remains manual.

## Art source and prompts
Tool: built-in image_gen. Reference: user's approved HUD image dated 2026-09-15
00_04_09 and approved generated Hector portrait.
All assets: Assets/Game/Art/Resources/HectorHud/*.png.
Shared direction: hand-painted cartoon Trojan strategy HUD, bold ink outlines,
bronze highlights, pale limestone, red cloth/enamel; no baked text or values.

Individual generation requests:
- Portrait: preserve approved Hector, replace baked checkerboard with dark teal for a circular UI mask.
- PortraitFrame: bronze circular rim, golden lower-left laurel, empty teal center, transparent exterior.
- Panel: limestone frame and sill, bronze trim, red corner cloth, quiet dark wood interior.
- Nameplate: empty oxblood enamel name banner with gold Greek trim.
- HealthTrack: empty dark recessed horizontal track with bronze rim; source margins excluded by sprite rect.
- Parchment: blank ivory paper quote area with narrow bronze edge.
- AbilityButton: empty square limestone/bronze button with red enamel center.
- warcry: golden ancient war horn with sound marks.
- shieldwall: three overlapping bronze shields.
- spear: diagonal spear and motion streaks.
- ultimate: radiant Trojan crested helmet.

## Validation
Passed: architecture guard; Unity script compilation; isolated Canvas renders at
1600x900 EN and 1280x720 RU; text-fit and five-button presence checks. Enlarged
1000x720 RU preview also inspected. EXE build and full PlayMode suite not run.
Explicit editor entry point: HectorHudArtPreview.ValidateAndCapture.
This creates an isolated preview scene, never a campaign session or save.
Rendered sample states are layout previews, not evidence of gameplay acceptance.
