# Defender HUD artwork

## Goal / owner
UI: replace procedural defender glyphs with six distinct illustrated portraits.
Reference: Pictures/BATTLE HUD/Battle_Hud_sample.png.

## Scope
ModernCombatHud, TroyCombatHudSkin, TroyHudArt, defender presentation, tests and
Assets/Game/Art/Resources/DefenderHud. Do not change balance, map or input commands.

## Acceptance
- Six separate portraits; live localized names, hotkeys and authored prices.
- Skin preserves art; selected, recommended and unaffordable states remain legible.
- Existing selection callbacks and hover tooltips retained.
- Focused Unity render checks and architecture guard pass.
- No EXE build; real gameplay visual acceptance remains pending.

## Status
Implemented and focused checks passed. Gameplay visual acceptance pending.

## Assets and generation
Built-in image generation, six separate PNGs saved in
Assets/Game/Art/Resources/DefenderHud. No CLI/API fallback.
The temporary source-atlas candidates were replaced before delivery.
Existing HectorHud/AbilityButton supplies the separate reusable frame.

Prompt set: one square production UI illustration per asset, hand-painted cartoon
Trojan War tower-defense, bold ink contours, sculpted highlights, deep azure blue
painted background, readable at 100 px, subject fills 85-90%, no environment,
frame, border, words, numbers, logos or UI.
- Spearman: waist-up bronze helmet/red crest/red cloak, upright spear and round shield.
- Archer: young clean-shaven archer, red-crested helmet, curved wooden bow, arrow, quiver.
- Ballista: three-quarter wooden torsion bolt launcher, rope bundles, bronze fittings,
  loaded spear, compact wheeled base; no people or tower.
- Priest: mortal older gray-bearded Apollo priest, laurel, ivory robes/red sash,
  sun-disc staff and open hand with amber blessing; no bow or helmet.
- Fire: stocky smiling engineer, red bandana, leather apron, large burning clay pot.
- Guard: heavily armored veteran, low Corinthian helmet/red crest, enormous round
  bronze shield/red horse emblem, short sword; no spear.

## Validation
- Architecture guard passed.
- Final four EditMode Canvas tests passed (1600x900 / 1024x768, RU / EN).
- PlayMode CombatHudLayoutUxTests.DefenderCards_ArePortraitLedAndReadable passed.
- Inspected final Unity Canvas captures under Logs/Validation/DefenderHud.
- Tests exercise all six selection callbacks and real TowerFactory costs, skin
  preservation, affordable/selected states, decorative raycasts and RU/EN rendering.
- Full playthrough and EXE validation are not claimed.
