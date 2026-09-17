# Gate HUD Artwork

Owner: ModernCombatHud layout; GateHudArtwork decoration only.
Scope: first HUD block only, pending user approval before encounters/boss/defenders/abilities.
Reference: Pictures/BATTLE HUD/Battle_Hud_sample.png.

Acceptance: compact dark/gold frame, separate fortress icon, live localized HP label,
horizontal fill at full/half/empty health; old skin must not overwrite new artwork.
No balance, navigation, save, other HUD blocks or EXE build changes.

Assets: GateHud/Panel.png and Gate.png, generated with the built-in image tool.
Prompts: isolated empty dark wood horizontal HUD frame with thin beveled gold border
and corner rivets; isolated cartoon limestone Trojan gatehouse with bronze trim,
crimson banners and torches, transparent background, no text or health bar.
Both original bitmaps retained, with panel export margins excluded by sprite rect.

Validation: focused EditMode render/skin tests and architecture guard.
Status: candidate pending user visual acceptance and full gameplay QA.

Result: 3/3 EditMode tests passed; captures in Logs/Validation/GateHud.
Architecture guard blocked by the existing ChapterOneAegeanSeaPresentation scene search.
Windows player not built. Other HUD blocks intentionally unchanged.
