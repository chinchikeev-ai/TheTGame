# AI Development Pipeline

This file defines when an AI change is considered validated.

## Solo Git delivery policy
This is a solo-developer repository. Keep delivery simple.

- Routine fixes and normal gameplay/UI/balance/documentation changes go directly to `main` by default.
- Branches and PRs are optional and should be used only when they reduce real risk or are explicitly requested.
- If a requested change is implemented on a branch and the user asks to ship/merge/apply it, merge it into `main` in the same task unless told otherwise.
- A branch commit or open PR is not delivery. Delivery means the requested change is present in `main`.
- After shipping, verify the relevant content from `main` before reporting that the change is live.
- Preserve newer unrelated `main` changes when merging diverged work.

## Mandatory fast check
Run after every code change when the execution environment allows it:

`python tools/check-architecture.py`

It blocks legacy `Assets/Scripts`, direct input outside `GameInput`, scene-wide gameplay searches, direct `CampaignSave` access from UI, missing core paths, and partial namespace migration.

## Full validation
When Unity is available and full validation is explicitly needed:

Windows:
`./tools/validate-project.ps1`

Unix:
`UNITY_EDITOR=/path/to/Unity ./tools/validate-project.sh`

Stages:
1. fast architecture guard
2. Unity architecture smoke validation
3. EditMode tests
4. PlayMode acceptance tests
5. Windows build

## CI
`.github/workflows/unity-ci.yml` mirrors the pipeline with architecture, Unity test, and Windows build jobs. Unity jobs require the repository's Unity activation configuration in GitHub Actions settings.

Unity CI is manual-only:
- trigger: `workflow_dispatch` only;
- no automatic run on `push`;
- no automatic run on `pull_request`;
- do not start Unity CI unless the user explicitly asks for it;
- do not block routine merges on Unity CI unless the user explicitly requests CI validation first.

## Completion rule
An agent may report code changes as implemented after source edits, but may report them as validated only if the relevant checks actually ran and passed. Never infer Unity compile/build success from static inspection.

A Git/GitHub change may be reported as shipped only after confirming the requested change is actually present in `main`.

## Data rule
Authored ScriptableObject assets are the runtime source of truth. Missing data is a validation failure. Default-data tooling creates only missing assets and must not overwrite authored assets.

## Save-test rule
Automated save tests use isolated temporary storage via `CampaignSave.ConfigureStorageForTests`; tests must never mutate the user's actual campaign save.

## Task rule
Use `docs/tasks/TASK_TEMPLATE.md` for non-trivial changes and record acceptance criteria before implementation.
