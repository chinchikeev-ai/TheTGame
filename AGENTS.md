# TheTroyGame AI Development Rules

This is the mandatory entrypoint for AI-assisted development.

## Read order
1. `AGENTS.md`
2. `docs/README.md`
3. `docs/PROJECT_STATUS.md`
4. `docs/AI_PIPELINE.md`
5. `docs/MODEL_ART_INVENTORY.md`
6. `docs/ARCHITECTURE.md`
7. `docs/MODULE_MAP.md`
8. `docs/DATA_CATALOG.md`
9. `docs/RUNTIME_GRAPH.md`
10. `docs/NAMESPACE_POLICY.md`
11. relevant GDD/roadmap/chapter document

For non-trivial work, use `docs/tasks/TASK_TEMPLATE.md`.

## Solo-developer Git workflow
This repository is maintained by one developer. Keep Git workflow simple and do not introduce team-process ceremony unless explicitly requested.

- `main` is the normal delivery branch for routine fixes, balance changes, UI changes, documentation, and configuration updates.
- For small and medium changes, commit directly to `main` by default.
- Do not create a feature branch or pull request just for process/review when the user did not ask for one.
- Use a separate branch only when it materially reduces risk: large refactors, experimental work, long-running changes, destructive migrations, or concurrent work that must stay isolated.
- If a branch is used for a requested change and the user says to ship/merge/apply it, merge it into `main` in the same task unless the user explicitly asks to keep it separate.
- Never treat "committed to a branch" or "PR opened" as equivalent to "shipped". A change is shipped only when it is actually present in `main`.
- After any merge or direct write that is supposed to be live, verify the relevant file/content from `main` before reporting completion.
- If a branch has diverged from `main`, compare changed files first and preserve newer `main` work; do not overwrite unrelated newer changes.
- PRs are optional for this solo project. Use them only when they are genuinely useful or explicitly requested.
- Do not wait for or require CI before merging unless the user explicitly asks for CI validation.
- Unity CI is manual-only. `.github/workflows/unity-ci.yml` must use `workflow_dispatch` and must not auto-run on `push` or `pull_request` unless the user explicitly requests changing that policy.
- Never manually start Unity CI unless the user explicitly asks to run it.
- Do not enable automatic Unity CI as a side effect of other Git/GitHub work.

### Commit messages
Keep commit messages short and obvious. Do not use a complicated Conventional Commits scheme.

Preferred prefixes:
- `fix:` — bug fixes
- `balance:` — gameplay/economy/pacing balance
- `ui:` — UI/HUD/presentation
- `docs:` — documentation/rules
- `art:` — art/assets
- `audio:` — sound/music
- `refactor:` — structural cleanup without intended behavior change

Examples:
- `fix: enemy click`
- `balance: chapter 1`
- `ui: hero panel`
- `docs: update rules`

One commit should describe one coherent change. Avoid vague messages like `update`, `changes`, or `misc`.

### Current task list
- `docs/TODO.md` is the single day-to-day task list.
- Keep four sections: `Сейчас`, `Сломано`, `Проверить`, `Потом`.
- Update that file instead of creating GitHub Issues, GitHub Projects, Jira tickets, or duplicate TODO/planning documents unless the developer explicitly asks for another system.
- Keep it short: remove completed items instead of using it as a historical archive.

## Documentation authority
- `docs/PROJECT_STATUS.md` is authoritative for current implementation state.
- `docs/MODEL_ART_INVENTORY.md` is authoritative for production-art completion.
- `docs/ARCHITECTURE.md` is authoritative for module ownership/dependency rules.
- `docs/EDITOR_MENU.md` is authoritative for Unity Editor menu naming and placement.
- `docs/UNITY_ROADMAP.md` describes future milestone order, not current completion.
- audit documents are diagnostic snapshots and must not override current code/status/validation.

Do not create duplicate status/roadmap documents. Update the canonical owner document instead.

## Canonical runtime layout
Runtime code belongs under `Assets/Game` only. Do not recreate `Assets/Scripts`.

- `Core` — bootstrap, state, input, logging, balance/session rules
- `Campaign` — campaign API, chapter ownership, save, chapter/wave data
- `Combat` — reusable combat primitives
- `Towers` — tower runtime/data
- `Enemies` — enemy runtime/spawning/bosses/data
- `Heroes/Hector` — Hector runtime/abilities
- `World` — map/camera/environment/presentation
- `UI` — menus/HUD only
- `Audio` — audio runtime
- `VFX` — visual effects

## Data source of truth
Authored ScriptableObject assets are the runtime source of truth.

- Tower balance -> `TowerData` assets
- Enemy balance -> `EnemyData` assets
- Wave pacing/config -> `WaveData` assets
- Chapter setup -> `ChapterData` assets
- Change balance in assets, not runtime fallback/default code
- `The Troy Game/Data/Create Missing Default Assets` may create only absent assets
- Existing authored assets must never be auto-overwritten
- Missing authored runtime data is a validation failure, never a silent fallback

See `docs/DATA_CATALOG.md`.

## Art/model source of truth
`docs/MODEL_ART_INVENTORY.md` is authoritative for production-art completion status.

Do not mark or describe a model as finished merely because:
- KayKit or another source pack contains a usable base;
- an editor tool can generate a prefab;
- runtime fallback/procedural geometry renders a recognizable object;
- the GDD or Art Bible specifies the target;
- presentation code builds the silhouette from Unity primitives.

Use the inventory statuses exactly: `DONE`, `GENERATED PLACEHOLDER`, `PROCEDURAL`, `MISSING`, `SOURCE ONLY`.

A production model becomes `DONE` only after the final derivative is committed under `Assets/Game/Art/...`, runtime uses it instead of the fallback/procedural representation, and Play Mode visual QA has been performed. Update `docs/MODEL_ART_INVENTORY.md` in the same change that promotes an asset.

## Architecture rules
- `GameBootstrap` composes runtime systems only
- `GameManager` is a session facade; do not grow unrelated responsibilities
- `CampaignController` is the campaign-facing runtime API
- `ChapterController` owns the active chapter
- only campaign infrastructure may access `CampaignSave` directly; UI uses `CampaignController`
- gameplay/UI input goes through `GameInput`
- direct `Mouse.current`, `Keyboard.current` and `Input.Get*` are forbidden outside `GameInput`
- towers use `TowerRegistry`; enemies use `EnemyRegistry`
- no `FindObjectsByType` / `FindObjectsOfType` in runtime gameplay code
- `GameObject.Find` / `FindFirstObjectByType` are allowed only for bounded startup/bootstrap/binding work when no owner reference exists yet; never call them from `Update`, `LateUpdate`, `FixedUpdate` or other high-frequency loops
- prefer explicit owner references, registries or stable runtime instances over name-based scene lookup
- damage goes through `DamagePacket` / `DamageType`
- reusable effects use shared combat/status APIs
- avoid `Resources.Load` in high-frequency loops
- prefer small owners/controllers over new global singletons

## Namespace policy
Keep runtime classes in the global namespace for now. Do not introduce `namespace TheTroyGame` in isolated files. Namespace migration must be a dedicated repository-wide task. See `docs/NAMESPACE_POLICY.md`.

## Save rules
- persistent state is versioned JSON through `CampaignSave`
- schema changes require version bump plus migration/normalization
- never delete a persisted field without migration
- automated tests use isolated temporary storage and must never mutate the user's real save

## Performance rules
- no scene scans from Update
- avoid allocations in targeting/status/update loops
- snapshot registries only when mutation during iteration is possible
- move repeated projectiles/enemies/VFX to pooling before later campaign scale

## UI/localization
- user-facing gameplay text supports EN/RU through the language layer
- language switching must not reload the active scene
- UI must not own gameplay state or persistence implementation
- project-owned Unity Editor commands use the single canonical root `The Troy Game/`; do not add `TheTroyGame/...`, `Tools/TheTroyGame/...`, or `Tools/The Troy Game/...` menu paths. See `docs/EDITOR_MENU.md`.

## Required change workflow
1. Identify owner module/system using `docs/MODULE_MAP.md`.
2. Read the relevant data contract, current status, and model/art inventory when visual assets are involved.
3. For non-trivial work define acceptance criteria from `docs/tasks/TASK_TEMPLATE.md`.
4. Make the smallest coherent change.
5. Preserve public APIs when practical.
6. Update `docs/PROJECT_STATUS.md` for meaningful status changes.
7. Update `docs/MODEL_ART_INVENTORY.md` when asset status changes.
8. Update the GDD/roadmap/architecture document only when its owned contract changes.
9. Add/update EditMode or PlayMode tests.
10. Run `python tools/check-architecture.py`.
11. When Unity is available, run full validation before claiming completion.

## Full validation
Windows: `./tools/validate-project.ps1`

Unix: `UNITY_EDITOR=/path/to/Unity ./tools/validate-project.sh`

Full stages: architecture guard -> Unity architecture checks -> EditMode -> PlayMode acceptance tests -> Windows build.

GitHub Actions mirrors this flow in `.github/workflows/unity-ci.yml`, but the GitHub workflow is manual-only and runs only when explicitly requested.

## Validation truthfulness
An agent may say code was implemented after editing source. It may say Unity compile/tests/build are validated only when those checks actually ran and passed.

Required gameplay acceptance coverage includes Chapter I start/wave completion, victory/unlock, defeat/no-unlock, save round-trip/backup, language switch without reload, Hector Q/E/R/F safety, and final-wave boss data.
