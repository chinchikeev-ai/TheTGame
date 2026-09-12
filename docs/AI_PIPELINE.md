# AI Development Pipeline

This file defines when an AI change is considered validated.

## Mandatory fast check
Run after every code change:

`python tools/check-architecture.py`

It blocks legacy `Assets/Scripts`, direct input outside `GameInput`, scene-wide gameplay searches, direct `CampaignSave` access from UI, missing core paths, and partial namespace migration.

## Full validation
When Unity is available:

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

## Completion rule
An agent may report code changes as implemented after source edits, but may report them as validated only if the relevant checks actually ran and passed. Never infer Unity compile/build success from static inspection.

## Data rule
Authored ScriptableObject assets are the runtime source of truth. Missing data is a validation failure. Default-data tooling creates only missing assets and must not overwrite authored assets.

## Save-test rule
Automated save tests use isolated temporary storage via `CampaignSave.ConfigureStorageForTests`; tests must never mutate the user's actual campaign save.

## Task rule
Use `docs/tasks/TASK_TEMPLATE.md` for non-trivial changes and record acceptance criteria before implementation.
