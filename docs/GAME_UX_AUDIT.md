# TheTroyGame — Game UX Audit

Status: active implementation audit  
Scope: Main Menu, Chapter Select, Settings, Pause, Results, first interaction flow

## Executive Summary

The previous menu implementation was functional but still read as a prototype. The main reason was architectural: the entire menu stack was generated at runtime by `GameMenuController`, and the settings screen exposed only simple `VOLUME +/-`, fullscreen, language, difficulty, and Back controls.

The current P0 pass keeps the existing runtime-UI architecture to avoid destabilizing the project, but reorganizes it into a clearer modern game flow and introduces persistent user settings.

### UX score before P0 pass

| Area | Score |
|---|---:|
| Main Menu hierarchy | 4/10 |
| Settings usability | 3/10 |
| Navigation consistency | 5/10 |
| Visual hierarchy | 4/10 |
| Resolution adaptability | 5/10 |
| Feedback / button states | 4/10 |
| Localization UX | 5/10 |
| Commercial readiness | 3/10 |
| Overall UX | **4.1/10** |

### Target after current menu pass

**7/10 UX baseline** before bespoke art assets, animations, controller navigation, and final scene-based polish.

---

# 1. Main Menu

## Previous problems

- Only three primary actions: Play, Settings, Exit.
- No distinction between continue, new campaign, and chapter selection.
- Large title/buttons floated directly over the background without a strong composition container.
- Little hierarchy between primary and secondary actions.
- Button styling was visually close to a debug/prototype menu.
- The background art existed, but the UI did not frame it like a modern title screen.

## P0 changes implemented

- Added a dedicated hero/menu panel over the Troy background.
- Created a clear action hierarchy:
  - Continue;
  - New Campaign;
  - Chapter Select;
  - Settings;
  - Exit.
- Added Troy/fire identity copy and stronger bronze/red visual language.
- Primary action uses a highlighted state; secondary actions use stone/ghost styles.
- Added version/pre-alpha information outside the main interaction stack.
- Canvas now uses `Scale With Screen Size`, 1920×1080 reference, 50/50 width-height matching.

## Remaining P1 work

- Continue should become disabled/hidden when there is no meaningful save progress.
- New Campaign should have a confirmation flow if campaign progress already exists.
- Add subtle entrance animation / button hover movement and audio.
- Replace built-in runtime font with the final project typography system.
- Validate safe layout visually at 1366×768, 1920×1080, 2560×1440 and ultrawide.

---

# 2. Settings

## Previous problems

The previous settings screen contained:

- Volume +;
- Volume -;
- Fullscreen;
- Language;
- Difficulty;
- Back.

This is insufficient for a modern PC game and creates unnecessary repeated clicking.

## P0 changes implemented

A new persistent settings service was introduced:

`Assets/Game/Core/Settings/GameUserSettings.cs`

Current user-facing settings include:

### Audio
- Master Volume slider;
- Music Volume slider.

### Display
- Window Mode / Fullscreen;
- Resolution;
- VSync;
- FPS Limit;
- Graphics Quality.

### Game
- Language;
- Difficulty.

### Actions
- Reset Defaults;
- Apply;
- Back.

Settings are stored through `PlayerPrefs` where appropriate and reapplied on menu startup.

## Remaining P1/P2 work

- Separate SFX / UI / Voice buses require a proper AudioMixer or audio-routing pass; do not fake these sliders before the underlying channels exist.
- Add Camera Shake and Screen Flash accessibility settings when those systems expose a real runtime toggle.
- Persist graphics-quality selection explicitly in the same settings service.
- Replace cycling selectors with dropdown/left-right selector widgets if final UI assets support them.
- Add unsaved-change indication if settings become deferred rather than applied immediately.

---

# 3. Localization

## Previous risk

Runtime strings were individually replaced through a large switch. This approach becomes fragile as the menu grows and can create collisions between identical translations.

Example discovered during this pass:

- `Continue` and `Resume` both translated to `ПРОДОЛЖИТЬ`, producing a duplicate switch case in C#.

## P0 solution

- Removed dependence on the large text-replacement switch.
- Language changes rebuild the runtime UI using the selected locale.
- Pause action now uses the clearer Russian label `ВЕРНУТЬСЯ В ИГРУ`.
- Dynamic values are resynchronized after UI rebuild.

## P2 recommendation

Move menu strings to a centralized localization table / Unity Localization package when content volume justifies it.

---

# 4. Chapter Select

## Previous problems

- Functioned as a simple vertical list.
- Weak distinction between unlocked, locked, and in-production content.
- Little narrative framing.

## P0 changes

- Converted into a focused chapter card.
- Chapter I is the strong primary action.
- Chapter II state communicates locked vs unlocked/in-production status.
- Added explanatory feedback for unavailable Chapter II.

## P1 recommendation

Move toward visual chapter cards/map nodes when Chapter II+ content is production-ready.

---

# 5. Pause UX

## P0 improvements

Pause now follows a conventional hierarchy:

1. Resume;
2. Settings;
3. Restart Chapter;
4. Main Menu;
5. Exit.

The primary Resume action is visually dominant.

## Remaining work

- Add confirmation for destructive navigation when a run is in progress.
- Add controller focus/default selection.
- Add pause blur/dim treatment if compatible with current render pipeline.

---

# 6. Result Screen

## P0 improvements

- Statistics are contained inside a focused result card.
- Retry and Chapter Select are presented as clear next actions.
- Result information retains current campaign/gameplay data rather than replacing the underlying logic.

## P1 recommendation

Reorganize statistics into 3–5 major metrics plus expandable detail rather than a dense text report once final icons are available.

---

# 7. Visual Language

Current implementation intentionally uses the existing runtime UI system but establishes a consistent direction:

- charcoal/dark-brown panels;
- bronze outlines;
- dark red primary buttons;
- warm gold active text;
- stone secondary buttons;
- restrained separators;
- Troy/fire visual identity.

This is a structural UX pass, not the final art pass.

Final commercial quality still requires:

- authored sprites/panels;
- final typography;
- icon system;
- hover/transition animation;
- UI SFX;
- controller navigation;
- real Unity Play Mode visual QA.

---

# 8. Current UX Architecture

Current runtime flow:

```text
Main Menu
├── Continue -> Chapter Select
├── New Campaign -> Chapter Select
├── Chapter Select
├── Settings
└── Exit

Chapter Select
├── Chapter I -> Gameplay
├── Chapter II -> locked/in-production feedback
└── Back

Gameplay
└── Pause
    ├── Resume
    ├── Settings
    ├── Restart Chapter
    ├── Main Menu
    └── Exit
```

Settings are shared between Main Menu and Pause and return to the correct context.

---

# 9. P1 Priority Queue

1. Validate compilation and Play Mode after Git pull.
2. Visual QA at 1366×768 and 1920×1080 first.
3. Add confirmation modal for restart/main-menu exit during an active run.
4. Give Continue real save-state semantics.
5. Give New Campaign a real reset/confirmation flow.
6. Add keyboard/controller navigation and first-selected button behavior.
7. Add UI hover/click SFX.
8. Add 120–180 ms transition animation between menu states.
9. Replace `LegacyRuntime.ttf` with final fonts.
10. Convert the result screen from text-heavy summary to visual metrics.

---

# 10. Definition of Done for this P0 pass

P0 is structurally complete when:

- main menu has modern hierarchy;
- settings are grouped and persistent;
- volume uses sliders rather than +/- buttons;
- resolution/fullscreen/VSync/FPS/quality are exposed;
- language switch does not leave mixed-language UI;
- pause hierarchy is conventional;
- layouts scale from a 1920×1080 reference;
- existing level-start, pause, result and campaign flow remains connected.

Visual approval still requires opening the current `main` in Unity and checking Play Mode; Git-only editing cannot substitute for rendered UI inspection.
