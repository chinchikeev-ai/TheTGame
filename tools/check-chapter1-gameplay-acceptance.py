#!/usr/bin/env python3
import json
from pathlib import Path
import sys

ROOT = Path(__file__).resolve().parents[1]

MANIFEST = ROOT / "Assets/Game/QA/CHAPTER_I_GAMEPLAY_ACCEPTANCE.json"
ACCEPTANCE = ROOT / "Assets/Editor/ChapterOneGameplayAcceptanceValidator.cs"
DIFFICULTY = ROOT / "Assets/Editor/ChapterOneDifficultyPressureAnalyzer.cs"
VISUAL = ROOT / "Assets/Editor/ChapterOneVisualFitQaTool.cs"
LANGUAGE = ROOT / "Assets/Game/Core/Localization/GameLanguage.cs"
DOC = ROOT / "docs/CHAPTER_I_GAMEPLAY_ACCEPTANCE.md"

required_paths = [MANIFEST, ACCEPTANCE, DIFFICULTY, VISUAL, LANGUAGE, DOC]
errors = []
for path in required_paths:
    if not path.exists():
        errors.append(f"missing required Chapter I gameplay acceptance path: {path.relative_to(ROOT)}")

manifest = None
if MANIFEST.exists():
    try:
        manifest = json.loads(MANIFEST.read_text(encoding="utf-8"))
    except Exception as exc:
        errors.append(f"could not parse gameplay acceptance manifest: {exc}")

if isinstance(manifest, dict):
    story = manifest.get("storyBaseline") or {}
    difficulty = manifest.get("difficultyPressure") or {}
    visual = manifest.get("visualFit") or {}

    if manifest.get("schemaVersion") != 1:
        errors.append("gameplay acceptance manifest schemaVersion must be 1")

    required_story = (
        "sourceSessionId", "sourceReportFile", "sourceReportSha256",
        "warningsReviewed", "warningsDecisionNotes", "humanAccepted",
        "acceptedBy", "acceptedUtc",
    )
    required_difficulty = ("strategosReviewed", "legendaryReviewed", "notes")
    required_visual = (
        "ru1920x1080", "en1920x1080",
        "ru1366or1376x768", "en1366or1376x768", "notes",
    )

    for key in required_story:
        if key not in story:
            errors.append(f"gameplay acceptance storyBaseline missing key: {key}")
    for key in required_difficulty:
        if key not in difficulty:
            errors.append(f"gameplay acceptance difficultyPressure missing key: {key}")
    for key in required_visual:
        if key not in visual:
            errors.append(f"gameplay acceptance visualFit missing key: {key}")
    if "gameplayFrozen" not in manifest:
        errors.append("gameplay acceptance manifest missing gameplayFrozen")

    if manifest.get("gameplayFrozen") is True:
        required_true = [
            (story.get("warningsReviewed"), "storyBaseline.warningsReviewed"),
            (story.get("humanAccepted"), "storyBaseline.humanAccepted"),
            (difficulty.get("strategosReviewed"), "difficultyPressure.strategosReviewed"),
            (difficulty.get("legendaryReviewed"), "difficultyPressure.legendaryReviewed"),
            (visual.get("ru1920x1080"), "visualFit.ru1920x1080"),
            (visual.get("en1920x1080"), "visualFit.en1920x1080"),
            (visual.get("ru1366or1376x768"), "visualFit.ru1366or1376x768"),
            (visual.get("en1366or1376x768"), "visualFit.en1366or1376x768"),
        ]
        for value, label in required_true:
            if value is not True:
                errors.append(f"gameplayFrozen=true requires {label}=true")

        required_text = [
            (story.get("sourceSessionId"), "storyBaseline.sourceSessionId"),
            (story.get("sourceReportFile"), "storyBaseline.sourceReportFile"),
            (story.get("sourceReportSha256"), "storyBaseline.sourceReportSha256"),
            (story.get("warningsDecisionNotes"), "storyBaseline.warningsDecisionNotes"),
            (story.get("acceptedBy"), "storyBaseline.acceptedBy"),
            (story.get("acceptedUtc"), "storyBaseline.acceptedUtc"),
            (difficulty.get("notes"), "difficultyPressure.notes"),
            (visual.get("notes"), "visualFit.notes"),
        ]
        for value, label in required_text:
            if not isinstance(value, str) or not value.strip():
                errors.append(f"gameplayFrozen=true requires non-empty {label}")

contracts = {
    ACCEPTANCE: (
        "Prepare Latest Story Candidate",
        "ComputeSha256",
        "warningsReviewed",
        "humanAccepted",
        "strategosReviewed",
        "legendaryReviewed",
        "READY TO FREEZE",
        "gameplayFrozen=true is invalid",
    ),
    DIFFICULTY: (
        "Analyze Story-Strategos-Legendary Pressure",
        "EnemyHpMultiplier",
        "EnemySpeedMultiplier",
        "EnemyCountMultiplier",
        "pressureScore",
        "READY FOR HUMAN REVIEW",
    ),
    VISUAL: (
        "1920x1080 RU",
        "1920x1080 EN",
        "1376x768 RU",
        "1376x768 EN",
        "Generate Chapter I QA Checklist",
        "Screen.SetResolution",
        "GameLanguage.SetRussian",
    ),
    LANGUAGE: ("SetRussian(bool russian)",),
    DOC: (
        "Step 5",
        "Step 6",
        "Step 7",
        "Step 8",
        "READY TO FREEZE",
        "gameplayFrozen=true",
    ),
}

for path, tokens in contracts.items():
    if not path.exists():
        continue
    text = path.read_text(encoding="utf-8")
    for token in tokens:
        if token not in text:
            errors.append(f"{path.relative_to(ROOT)} missing gameplay acceptance contract token: {token}")

if errors:
    print("Chapter I gameplay acceptance guard FAILED:")
    for error in errors:
        print(f"- {error}")
    sys.exit(1)

print("Chapter I gameplay acceptance guard passed.")
