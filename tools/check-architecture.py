#!/usr/bin/env python3
from pathlib import Path
import sys

ROOT = Path(__file__).resolve().parents[1]
GAME = ROOT / "Assets" / "Game"

REQUIRED = [
    "Assets/Game/Core/Bootstrap/GameBootstrap.cs",
    "Assets/Game/Core/Input/GameInput.cs",
    "Assets/Game/Core/Session/GameManager.cs",
    "Assets/Game/Campaign/CampaignController.cs",
    "Assets/Game/Campaign/ChapterController.cs",
    "Assets/Game/Campaign/Persistence/CampaignSave.cs",
    "Assets/Game/Combat/CombatDamage.cs",
    "Assets/Game/Towers/Tower.cs",
    "Assets/Game/Enemies/Enemy.cs",
    "Assets/Game/Heroes/Hector/HectorController.cs",
    "Assets/Game/UI/GameMenuController.cs",
    "Assets/Game/TheTroyGame.Runtime.asmdef",
    "Assets/Editor/TheTroyGame.Editor.asmdef",
    "Assets/Editor/CampaignModelAuditValidator.cs",
    "Assets/Editor/CampaignModelAuditValidator.cs.meta",
    "Assets/Editor/CampaignModelAuditCommand.cs",
    "Assets/Editor/CampaignModelAuditCommand.cs.meta",
    "Assets/Editor/ModelGapClosureBuilder.cs",
    "Assets/Editor/ChapterOneArtFreezeValidator.cs",
    "Assets/Editor/ChapterOneArtFreezeValidator.cs.meta",
    "Assets/Game/Art/PRODUCTION_ACCEPTANCE.json",
    "Assets/Game/Art/PRODUCTION_ACCEPTANCE.json.meta",
    "Assets/Game/Art/CHAPTER_I_FREEZE_ACCEPTANCE.json",
    "Assets/Game/Art/CHAPTER_I_FREEZE_ACCEPTANCE.json.meta",
    "docs/CAMPAIGN_MODEL_AUDIT.md",
    "docs/CHAPTER_I_ART_FREEZE.md",
    "tools/audit-models.sh",
    "tools/audit-models.ps1",
    "tools/audit-chapter1-art.sh",
    "tools/audit-chapter1-art.ps1",
    "Assets/Tests/EditMode/TheTroyGame.EditModeTests.asmdef",
    "Assets/Tests/PlayMode/TheTroyGame.PlayModeTests.asmdef",
]

errors = []
for rel in REQUIRED:
    if not (ROOT / rel).exists():
        errors.append(f"missing required path: {rel}")

AUDIT = ROOT / "Assets" / "Editor" / "CampaignModelAuditValidator.cs"
if AUDIT.exists():
    audit_text = AUDIT.read_text(encoding="utf-8")
    for token in (
        "AuditStatus { Done, Candidate, Missing, Broken }",
        "Build Candidates + Audit Campaign Models",
        "BuildAndRunBatchmode",
        "CampaignModelAudit.json",
        "CampaignModelAudit.md",
        "ModelGapClosureBuilder.BuildAll()",
        "PRODUCTION_ACCEPTANCE.json",
        "accepted.Contains(spec.path)",
    ):
        if token not in audit_text:
            errors.append(f"campaign model audit contract missing token: {token}")

FREEZE = ROOT / "Assets" / "Editor" / "ChapterOneArtFreezeValidator.cs"
if FREEZE.exists():
    freeze_text = FREEZE.read_text(encoding="utf-8")
    for token in (
        "Audit Chapter I Art Freeze",
        "Build Chapter I Candidates + Audit Art Freeze",
        "PRODUCTION_ACCEPTANCE.json",
        "CHAPTER_I_FREEZE_ACCEPTANCE.json",
        "ChapterOneReleaseValidator.Validate(false)",
        "readyForFreeze = report.blocked == 0 && report.broken == 0",
        "ChapterOneArtFreeze.json",
        "ChapterOneArtFreeze.md",
        "playModeVisualQa",
        "englishFramingQa",
        "russianFramingQa",
        "towerUnitVisualQa",
        "environmentVisualQa",
        "animationVisualQa",
        "AnimatorControllerParameterType.Trigger",
        "UpgradeVisual_L2",
        "UpgradeVisual_L3",
        'Add(rows,"BLOCKED"',
        'Add(rows,"BROKEN"',
    ):
        if token not in freeze_text:
            errors.append(f"Chapter I art freeze contract missing token: {token}")

if (ROOT / "Assets" / "Scripts").exists():
    errors.append("legacy Assets/Scripts must not exist")

if GAME.exists():
    for path in GAME.rglob("*.cs"):
        text = path.read_text(encoding="utf-8")
        rel = path.relative_to(ROOT).as_posix()
        is_input = rel.endswith("Core/Input/GameInput.cs")
        if not is_input and any(token in text for token in ("Mouse.current", "Keyboard.current", "Input.Get")):
            errors.append(f"direct input outside GameInput: {rel}")
        if "FindObjectsByType<" in text or "FindObjectsOfType<" in text:
            errors.append(f"scene-wide gameplay search: {rel}")
        if "/UI/" in f"/{rel}" and "CampaignSave." in text:
            errors.append(f"UI accesses CampaignSave directly: {rel}")
        if "namespace TheTroyGame" in text:
            errors.append(f"namespace migration is not active; keep global namespace: {rel}")

if errors:
    print("ARCHITECTURE CHECK FAILED")
    for error in errors:
        print(f" - {error}")
    sys.exit(1)

print("ARCHITECTURE CHECK PASSED")
