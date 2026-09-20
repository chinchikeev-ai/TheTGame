# Wave HUD artwork pass

Reference: Pictures/BATTLE HUD/ChatGPT Image 16 сент. 2026 г., 23_56_35 (3).png.
Scope: top-center wave banner and the white patron portrait defect only.
Map art, defender cards, hero layout, economy, speed and combat rules are unchanged.

ModernCombatHud owns geometry and live text. WaveHudArtwork owns the banner sprite.
TroyCombatHudSkin preserves the authored banner rather than adding another panel/icon.
Progress remains live as resolved/total/percent on the lower plaque; duplicate bar hidden.

Generated asset prompt: isolated transparent crimson cloth banner with gold rim,
symmetrical laurels and spear ornaments, empty black/gold progress plaque, no text.
Source output is preserved; runtime crops transparent margins without bitmap editing.

Portrait imports previously used textureFormat 1 (Alpha8); format 4 retains RGBA.
Four EditMode cases render the banner through the common skin and verify each patron
texture is not Alpha8 and produces colored pixels. Logs/Validation/WaveHud contains
the captures. No real gameplay visual acceptance or Windows build in this pass.
