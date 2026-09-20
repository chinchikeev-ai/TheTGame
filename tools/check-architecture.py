#!/usr/bin/env python3
from pathlib import Path
import re
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
    "Assets/Game/Characters/CharacterWeaponSocketResolver.cs",
    "Assets/Game/Characters/CharacterWeaponSocketResolver.cs.meta",
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

HOT_RUNTIME_METHODS = ("Update", "LateUpdate", "FixedUpdate")
HOT_SCENE_SEARCH_TOKENS = (
    "GameObject.Find(",
    "FindFirstObjectByType<",
    "Object.FindFirstObjectByType<",
)

def method_body(text, method_name):
    match = re.search(r"\bvoid\s+" + re.escape(method_name) + r"\s*\([^)]*\)\s*\{", text)
    if match is None:
        return None

    start = match.end() - 1
    depth = 0
    for index in range(start, len(text)):
        char = text[index]
        if char == "{":
            depth += 1
        elif char == "}":
            depth -= 1
            if depth == 0:
                return text[start:index + 1]
    return text[start:]

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
        'ArrowSocketName = "Socket_ArrowRelease"',
        'SpearSocketName = "Socket_SpearRelease"',
        "CreateReleaseSocket(bow.transform, ArrowSocketName",
        "CreateReleaseSocket(spear.transform, SpearSocketName",
        "HumanBodyBones.RightHand",
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
        "HumanBodyBones.UpperChest",
        "HumanBodyBones.Head",
        "ResolveTorsoBone",
        "ResolveHeadBone",
        "item.transform.SetParent(bone, true)",
    ):
        if token not in armor_text:
            errors.append(f"Chapter I armor candidate contract missing token: {token}")

ANIMATION_BUILDER = ROOT / "Assets" / "Editor" / "ChapterOneCharacterAnimationBuilder.cs"
if ANIMATION_BUILDER.exists():
    animation_text = ANIMATION_BUILDER.read_text(encoding="utf-8")
    for token in (
        "static readonly string[] SpearRoleTokens",
        "static readonly string[] BowDrawTokens",
        "result.Sort(CompareClips);",
        'string.Equals(profileName, "hector", StringComparison.Ordinal)',
        'AddAction(controller, machine, idleState, "Poke"',
        'AddAction(controller, machine, idleState, "Block"',
        "var excluded = new HashSet<AnimationClip>();",
        "PickBestAction(clips, BowRoleTokens, BowReleaseTokens",
        'ReportBinding(profileName, "Draw"',
        'ReportBinding(profileName, "Release"',
        "Dedicated bow clips are still required for production acceptance.",
        "placeholder binding pending authored animation QA",
    ):
        if token not in animation_text:
            errors.append(f"Chapter I animation profile contract missing token: {token}")

PRESENTATION = ROOT / "Assets" / "Game" / "Characters" / "CharacterPresentationState.cs"
if PRESENTATION.exists():
    presentation_text = PRESENTATION.read_text(encoding="utf-8")
    for token in (
        'PlaySpearAttack() => Trigger("Poke", "Attack")',
        'PlaySpearAttack(Action impact) => PlayTimedAttack("Poke", "Attack", impact, .48f, .34f)',
        'PlayAbilityR(Action impact) => PlayTimedAttack("AbilityR", "Attack", impact, .52f, .38f)',
        "public void PlayBowShot(Action impact, float redrawDelay = .18f)",
        "InvokeAtAnimationPhase",
        "IsStateAtOrBeyondPhase",
        "RedrawBowAfterImpact",
        "animator.GetCurrentAnimatorStateInfo(0)",
        "animator.GetNextAnimatorStateInfo(0)",
        "if (impactRoutine != null)",
    ):
        if token not in presentation_text:
            errors.append(f"Chapter I character impact-timing contract missing token: {token}")

SOCKETS = ROOT / "Assets" / "Game" / "Characters" / "CharacterWeaponSocketResolver.cs"
if SOCKETS.exists():
    socket_text = SOCKETS.read_text(encoding="utf-8")
    for token in (
        'ArrowSocketName = "Socket_ArrowRelease"',
        'SpearSocketName = "Socket_SpearRelease"',
        'SourceBowName = "SourceBow_Quaternius_MedievalWeapons"',
        'SourceSpearName = "SourceSpear_Quaternius_MedievalWeapons"',
        "public Vector3 ArrowReleasePoint()",
        "public Vector3 SpearReleasePoint()",
        "animator.GetBoneTransform(HumanBodyBones.RightHand)",
        "return transform.TransformPoint(new Vector3(.18f, .95f, .28f));",
        "return transform.TransformPoint(new Vector3(.22f, 1.10f, .42f));",
    ):
        if token not in socket_text:
            errors.append(f"Chapter I weapon-socket contract missing token: {token}")

ENEMY = ROOT / "Assets" / "Game" / "Enemies" / "Enemy.cs"
if ENEMY.exists():
    enemy_text = ENEMY.read_text(encoding="utf-8")
    for token in (
        "if (Archetype == EnemyArchetype.Archer) presentation.PrepareBow();",
        "CharacterWeaponSocketResolver weaponSockets;",
        "weaponSockets.Refresh();",
        "weaponSockets.ArrowReleasePoint()",
        "void PlayCombatAttack(Action impact)",
        "case EnemyArchetype.Infantry:",
        "case EnemyArchetype.HeavyHoplite:",
        "case EnemyArchetype.ShieldBearer:",
        "presentation.PlaySpearAttack(impact);",
        "case EnemyArchetype.Archer:",
        "presentation.PlayBowShot(impact);",
        "PlayCombatAttack(() =>",
        "if (!IsAlive || targetGuard == null || !targetGuard.IsAlive || blockingGuard != targetGuard) return;",
        "if (!IsAlive || hector == null || hector.IsDowned) return;",
    ):
        if token not in enemy_text:
            errors.append(f"Chapter I enemy impact-timing contract missing token: {token}")

HECTOR_PRESENTATION = ROOT / "Assets" / "Game" / "Heroes" / "Hector" / "HectorPresentationBridge.cs"
if HECTOR_PRESENTATION.exists():
    hector_presentation_text = HECTOR_PRESENTATION.read_text(encoding="utf-8")
    for token in (
        "public void PlayAttackImpact(Vector3 point, Action impact)",
        "characterPresentation.PlaySpearAttack(() =>",
        "public void PlaySpearImpact(Vector3 point, Action impact)",
        "characterPresentation.PlayAbilityR(() =>",
        "CharacterWeaponSocketResolver weaponSockets;",
        "weaponSockets.Refresh();",
        "weaponSockets.SpearReleasePoint()",
    ):
        if token not in hector_presentation_text:
            errors.append(f"Hector animation-phase impact contract missing token: {token}")

HECTOR = ROOT / "Assets" / "Game" / "Heroes" / "Hector" / "HectorController.cs"
if HECTOR.exists():
    hector_text = HECTOR.read_text(encoding="utf-8")
    for token in (
        "presentation.PlayAttackImpact(impactPoint, () =>",
        "presentation.PlaySpearImpact(impactPoint, () =>",
        "if (IsDowned || best == null || !best.IsAlive) return;",
        "if (IsDowned || target == null || !target.IsAlive) return;",
    ):
        if token not in hector_text:
            errors.append(f"Hector gameplay impact-timing contract missing token: {token}")

TROJAN_GUARD = ROOT / "Assets" / "Game" / "Towers" / "TrojanGuardSquad.cs"
if TROJAN_GUARD.exists():
    guard_text = TROJAN_GUARD.read_text(encoding="utf-8")
    for token in (
        "presentation.PlaySpearAttack(() => ApplyAttackImpact(attackTarget, attackDamage));",
        "void ApplyAttackImpact(Enemy attackTarget, float attackDamage)",
        "!blockedEnemies.Contains(attackTarget)",
    ):
        if token not in guard_text:
            errors.append(f"Trojan Guard animation-phase impact contract missing token: {token}")

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
        for method_name in HOT_RUNTIME_METHODS:
            body = method_body(text, method_name)
            if body is not None and any(token in body for token in HOT_SCENE_SEARCH_TOKENS):
                errors.append(f"hot-path scene search in {method_name}: {rel}")
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
