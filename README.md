# TheTroyGame

Story-driven Tower Defence about the defense and fall of Troy.

**Engine:** Unity 6 + URP + C#  
**Campaign target:** ~120 minutes, 7 chapters, ~40 major combat events  
**Current production focus:** Chapter I — gameplay RC + production-art freeze

## Documentation

Start with [`docs/README.md`](docs/README.md).

For AI-assisted development, `AGENTS.md` is mandatory. The current implementation state is always tracked in `docs/PROJECT_STATUS.md`.

## Canonical runtime structure

```text
Assets/Game/
├── Core/
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

Runtime code belongs under `Assets/Game`. Legacy `Assets/Scripts` must not be recreated.

## Data source of truth

Authored ScriptableObject assets under `Assets/Resources` are the runtime source of truth for tower, enemy, wave and chapter configuration. Default-data tooling may create missing assets only and must never overwrite authored data.

## Validation

Fast architecture guard:

```bash
python tools/check-architecture.py
```

Full Windows validation:

```powershell
./tools/validate-project.ps1
```

Full Unix validation:

```bash
UNITY_EDITOR=/path/to/Unity ./tools/validate-project.sh
```

Full validation means architecture checks → Unity smoke validation → EditMode → PlayMode → build. GitHub Actions Unity jobs require repository Unity activation configuration; static architecture success alone is not a Unity compile/test/build pass.

## Chapter I target

- 5 combat events
- approximately 11–13 minutes at 1x
- Hector Q/E/R/F
- six defensive unit/tower roles
- Menelaus final encounter
- EN/RU UI
- real playthrough telemetry + RC analyzer
- production art promoted only after Play Mode visual QA

See `docs/PROJECT_STATUS.md`, `docs/CHAPTER_I_RC_PLAYTEST.md`, and `docs/MODEL_ART_INVENTORY.md` for the current gate.
