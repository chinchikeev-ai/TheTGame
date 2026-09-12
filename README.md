# TheTroyGame

Story-driven Tower Defence about the defense and fall of Troy. Unity 6 + URP + C#.

## AI-assisted development
Read in this order:
1. `AGENTS.md`
2. `docs/AI_PIPELINE.md`
3. `docs/PROJECT_STATUS.md`
4. `docs/ARCHITECTURE.md`
5. `docs/MODULE_MAP.md`
6. `docs/DATA_CATALOG.md`
7. `docs/RUNTIME_GRAPH.md`
8. `docs/NAMESPACE_POLICY.md`
9. relevant GDD / roadmap / chapter docs

For non-trivial work use `docs/tasks/TASK_TEMPLATE.md`.

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

Legacy `Assets/Scripts` must not be recreated.

## Data source of truth
Authored ScriptableObject assets under `Assets/Resources` are the runtime source of truth for tower/enemy/wave/chapter configuration. The default-data generator creates missing assets only and must never overwrite existing authored data.

## Assemblies
- `TheTroyGame.Runtime`
- `TheTroyGame.Editor`
- `TheTroyGame.EditModeTests`
- `TheTroyGame.PlayModeTests`

## Validation
Fast architecture guard:
`python tools/check-architecture.py`

Full Windows validation:
`./tools/validate-project.ps1`

Full Unix validation:
`UNITY_EDITOR=/path/to/Unity ./tools/validate-project.sh`

Full pipeline: architecture checks -> EditMode tests -> PlayMode acceptance tests -> Windows build.

CI: `.github/workflows/unity-ci.yml`. Unity CI jobs require repository Unity activation configuration in GitHub Actions settings.

The current Chapter I target is five combat events and approximately 11-13 minutes of real playtime. See `docs/PROJECT_STATUS.md` for implementation status.
