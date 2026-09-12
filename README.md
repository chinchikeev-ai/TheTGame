# TheTroyGame

Story-driven Tower Defence about the defense and fall of Troy. Unity 6 + URP + C#.

## AI-assisted development
Start here:

1. `AGENTS.md`
2. `docs/PROJECT_STATUS.md`
3. `docs/ARCHITECTURE.md`
4. `docs/MODULE_MAP.md`
5. `docs/GDD_v0.1.md`
6. `docs/UNITY_ROADMAP.md`

`AGENTS.md` contains the mandatory rules for AI agents and contributors.

## Canonical runtime structure

```text
Assets/Game/
├── Core/
│   ├── Bootstrap/
│   ├── Input/
│   ├── Session/
│   ├── State/
│   ├── Balance/
│   ├── Localization/
│   └── Logging/
├── Campaign/
├── Combat/
├── Towers/
├── Enemies/
├── Heroes/Hector/
├── World/
├── UI/
├── Audio/
└── VFX/
```

Legacy `Assets/Scripts` must not be recreated.

## Assemblies
- `TheTroyGame.Runtime`
- `TheTroyGame.Editor`
- `TheTroyGame.EditModeTests`
- `TheTroyGame.PlayModeTests`

## Validation
In Unity:

`TheTroyGame -> Validation -> Run Architecture Smoke Checks`

Also run EditMode and PlayMode tests before treating gameplay changes as validated.

The current Chapter I target is 5 combat events and approximately 11-13 minutes of real playtime. See `docs/PROJECT_STATUS.md` for what is implemented versus still pending.
