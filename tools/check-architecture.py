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
    "Assets/Editor/ChapterOneWeaponSourceInstaller.cs",
    "Assets/Editor/ChapterOneWeaponSourceInstaller.cs.meta",
    "Assets/Editor/ChapterOneSpearSourceInstaller.cs",
    "Assets/Editor/ChapterOneSpearSourceInstaller.cs.meta",
    "Assets/Editor/ChapterOneProductionEquipmentBuilder.cs",
    "Assets/Editor/ChapterOneProductionEquipmentBuilder.cs.meta",
    "Assets/Editor/ChapterOneArmorCandidateBuilder.cs",
    "Assets/Editor/ChapterOneArmorCandidateBuilder.cs.meta",
    "Assets/Game/Art/Characters/Equipment/DendraCuirassCandidate.obj",
    "Assets/Game/Art/Characters/Equipment/DendraCuirassCandidate.obj.meta",
    "Assets/Game/Art/Characters/Equipment/BoarTuskHelmetCandidate.obj",
    "Assets/Game/Art/Characters/Equipment/BoarTuskHelmetCandidate.obj.meta",
    "Assets/Game/Art/PRODUCTION_ACCEPTANCE.json",
    "Assets/Game/Art/PRODUCTION_ACCEPTANCE.json.meta",
    "Assets/Game/Art/CHAPTER_I_FREEZE_ACCEPTANCE.json",
    "Assets/Game/Art/CHAPTER_I_FREEZE_ACCEPTANCE.json.meta",
    "docs/CAMPAIGN_MODEL_AUDIT.md",
    "docs/CHAPTER_I_ART_FREEZE.md",
    "docs/third_party/QUATERNIUS_MEDIEVAL_WEAPONS.md",
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
        "ChapterOneProductionEquipmentBuilder.Build();",
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

WEAPON_SOURCE = ROOT / "Assets" / "Editor" / "ChapterOneWeaponSourceInstaller.cs"
if WEAPON_SOURCE.exists():
    source_text = WEAPON_SOURCE.read_text(encoding="utf-8")
    for token in (
        "jameskane05/three-game/571253b6098f269956f0137125d0920cc9cda4da",
        'ExpectedGitBlobSha = "a836e493f3dbebc75a619e5aa99118f34f700718"',
        "ExpectedSize = 28540",
        "Quaternius/MedievalWeapons/Bow.fbx",
        "GitBlobSha(bytes)",
    ):
        if token not in source_text:
            errors.append(f"Chapter I bow source contract missing token: {token}")

SPEAR_SOURCE = ROOT / "Assets" / "Editor" / "ChapterOneSpearSourceInstaller.cs"
if SPEAR_SOURCE.exists():
    source_text = SPEAR_SOURCE.read_text(encoding="utf-8")
    for token in (
        "DevHuang1/embervale-godot/aa6608e466c377ef588a09eca993f89c0c92470e",
        'ExpectedGitBlobSha = "47ccecfadd1779ae687a94d780b81a8934855b7d"',
        "ExpectedSize = 38556",
        "Quaternius/MedievalWeapons/Spear.fbx",
        "GitBlobSha(bytes)",
    ):
        if token not in source_text:
            errors.append(f"Chapter I spear source contract missing token: {token}")

EQUIPMENT = ROOT / "Assets" / "Editor" / "ChapterOneProductionEquipmentBuilder.cs"
if EQUIPMENT.exists():
    equipment_text = EQUIPMENT.read_text(encoding="utf-8")
    for token in (
        "ChapterOneWeaponSourceInstaller.Install();",
        "ChapterOneWeaponSourceInstaller.LoadBow()",
        "ChapterOneSpearSourceInstaller.Install();",
        "ChapterOneSpearSourceInstaller.LoadSpear()",
        "Enemy_Archer.prefab",
        "Trojan_Archer.prefab",
        "Enemy_Infantry.prefab",
        "Enemy_HeavyHoplite.prefab",
        "Enemy_ShieldBearer.prefab",
        "Trojan_Infantry.prefab",
        "Trojan_Guard.prefab",
        "Hero_Hector.prefab",
        "SourceBow_Quaternius_MedievalWeapons",
        "SourceSpear_Quaternius_MedievalWeapons",
        "RemoveProceduralBow",
        "FindProceduralSpear",
        'HasChild(transform, "Shaft")',
        'HasChild(transform, "BronzeTip")',
        "ChapterOneArmorCandidateBuilder.Build();",
    ):
        if token not in equipment_text:
            errors.append(f"Chapter I production equipment contract missing token: {token}")

ARMOR = ROOT / "Assets" / "Editor" / "ChapterOneArmorCandidateBuilder.cs"
if ARMOR.exists():
    armor_text = ARMOR.read_text(encoding="utf-8")
    for token in (
        "DendraCuirassCandidate.obj",
        "BoarTuskHelmetCandidate.obj",
        "SourceArmor_DendraCandidate",
        "SourceHelmet_BoarTuskCandidate",
        "Enemy_Infantry.prefab",
        "Enemy_HeavyHoplite.prefab",
        "Enemy_ShieldBearer.prefab",
        "Enemy_Boss.prefab",
        "Trojan_Infantry.prefab",
        "Trojan_Guard.prefab",
        "Hero_Hector.prefab",
        "Hero_Menelaus.prefab",
        '"BronzeCuirass"',
        '"BronzeHelmet"',
        '"HelmetCheekLeft"',
        '"HelmetCheekRight"',
    ):
        if token not in armor_text:
            errors.append(f"Chapter I armor candidate contract missing token: {token}")

MODEL_GAP = ROOT / "Assets" / "Editor" / "ModelGapClosureBuilder.cs"
if MODEL_GAP.exists() and "ChapterOneProductionEquipmentBuilder.Build();" not in MODEL_GAP.read_text(encoding="utf-8"):
    errors.append("full campaign model build must include the Chapter I production equipment source pass")

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
