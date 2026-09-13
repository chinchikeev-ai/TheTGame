#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class ChapterOneArtFreezeValidator
{
    [Serializable]
    sealed class ProductionAcceptance
    {
        public string[] acceptedPaths;
    }

    [Serializable]
    sealed class FreezeAcceptance
    {
        public bool playModeVisualQa;
        public bool englishFramingQa;
        public bool russianFramingQa;
        public bool towerUnitVisualQa;
        public bool environmentVisualQa;
        public bool animationVisualQa;
        public string referenceResolution;
        public string notes;
    }

    [Serializable]
    public sealed class FreezeRow
    {
        public string status;
        public string category;
        public string name;
        public string path;
        public string details;
    }

    [Serializable]
    public sealed class FreezeReport
    {
        public string generatedUtc;
        public bool readyForFreeze;
        public int total;
        public int pass;
        public int blocked;
        public int broken;
        public FreezeRow[] rows;
    }

    const string CharacterRoot = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters";
    const string Greek = CharacterRoot + "/Greek/";
    const string Trojan = CharacterRoot + "/Trojan/";
    const string Heroes = CharacterRoot + "/Heroes/";
    const string ProductionAcceptancePath = "Assets/Game/Art/PRODUCTION_ACCEPTANCE.json";
    const string FreezeAcceptancePath = "Assets/Game/Art/CHAPTER_I_FREEZE_ACCEPTANCE.json";
    const string AnimationControllerPath = "Assets/Game/Art/Characters/Animation/ChapterOneCharacter.controller";
    const string ReportDir = "Logs/Validation";

    static readonly string[] RequiredCharacterPaths =
    {
        Greek + "Enemy_Infantry.prefab",
        Greek + "Enemy_Runner.prefab",
        Greek + "Enemy_HeavyHoplite.prefab",
        Greek + "Enemy_ShieldBearer.prefab",
        Greek + "Enemy_Archer.prefab",
        Greek + "Enemy_Boss.prefab",
        Heroes + "Hero_Hector.prefab",
        Trojan + "Trojan_Infantry.prefab",
        Trojan + "Trojan_Guard.prefab",
        Trojan + "Trojan_Archer.prefab",
        Trojan + "Trojan_PriestApollo.prefab",
        Trojan + "Trojan_FireKeeper.prefab",
        Trojan + "Trojan_BallistaCrew.prefab"
    };

    static readonly string[] RequiredTowerDataPaths =
    {
        "Assets/Resources/Data/Towers/MachineGun.asset",
        "Assets/Resources/Data/Towers/Cannon.asset",
        "Assets/Resources/Data/Towers/Slow.asset",
        "Assets/Resources/Data/Towers/SpearThrower.asset",
        "Assets/Resources/Data/Towers/FireTower.asset",
        "Assets/Resources/Data/Towers/TrojanGuard.asset"
    };

    [MenuItem("TheTroyGame/Validation/Audit Chapter I Art Freeze")]
    public static void RunMenu() => Run(true);

    [MenuItem("TheTroyGame/Validation/Build Chapter I Candidates + Audit Art Freeze")]
    public static void BuildAndRunMenu()
    {
        CartoonCharacterPrefabBuilder.BuildAll();
        MythicAndSupportArtCandidateBuilder.BuildAll();
        ChapterOneCharacterAnimationBuilder.BuildAll();
        Run(true);
    }

    public static FreezeReport Run(bool log)
    {
        var rows = new List<FreezeRow>();
        HashSet<string> accepted = LoadProductionAcceptance(rows);
        FreezeAcceptance freeze = LoadFreezeAcceptance(rows);

        ValidateChapterReleaseContract(rows);
        ValidateRuntimeRosterContract(rows);
        ValidateCharacters(rows, accepted);
        ValidateAnimationController(rows);
        ValidateTowerUnits(rows);
        ValidateEnvironment(rows);
        ValidateHumanQa(rows, freeze);

        FreezeReport report = BuildReport(rows);
        WriteReport(report);
        if (log) LogReport(report);
        return report;
    }

    public static void RunBatchmode()
    {
        FreezeReport report = Run(true);
        EditorApplication.Exit(report.readyForFreeze ? 0 : 1);
    }

    static HashSet<string> LoadProductionAcceptance(List<FreezeRow> rows)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        if (!File.Exists(ProductionAcceptancePath))
        {
            Broken(rows,"Acceptance","Production acceptance manifest",ProductionAcceptancePath,"Manifest missing; no asset can be frozen as production-ready.");
            return result;
        }

        try
        {
            ProductionAcceptance data = JsonUtility.FromJson<ProductionAcceptance>(File.ReadAllText(ProductionAcceptancePath));
            if (data != null && data.acceptedPaths != null)
                foreach (string path in data.acceptedPaths)
                    if (!string.IsNullOrWhiteSpace(path)) result.Add(path.Trim());
            Pass(rows,"Acceptance","Production acceptance manifest",ProductionAcceptancePath,$"Loaded {result.Count} explicitly accepted production asset path(s).");
        }
        catch (Exception ex)
        {
            Broken(rows,"Acceptance","Production acceptance manifest",ProductionAcceptancePath,"Invalid JSON: " + ex.Message);
        }
        return result;
    }

    static FreezeAcceptance LoadFreezeAcceptance(List<FreezeRow> rows)
    {
        if (!File.Exists(FreezeAcceptancePath))
        {
            Broken(rows,"Acceptance","Chapter I freeze acceptance",FreezeAcceptancePath,"Freeze acceptance manifest missing.");
            return null;
        }

        try
        {
            FreezeAcceptance data = JsonUtility.FromJson<FreezeAcceptance>(File.ReadAllText(FreezeAcceptancePath));
            if (data == null)
            {
                Broken(rows,"Acceptance","Chapter I freeze acceptance",FreezeAcceptancePath,"Freeze acceptance JSON could not be parsed.");
                return null;
            }
            Pass(rows,"Acceptance","Chapter I freeze acceptance",FreezeAcceptancePath,"Freeze QA manifest loaded.");
            return data;
        }
        catch (Exception ex)
        {
            Broken(rows,"Acceptance","Chapter I freeze acceptance",FreezeAcceptancePath,"Invalid JSON: " + ex.Message);
            return null;
        }
    }

    static void ValidateChapterReleaseContract(List<FreezeRow> rows)
    {
        int errors;
        try { errors = ChapterOneReleaseValidator.Validate(false); }
        catch (Exception ex)
        {
            Broken(rows,"Release contract","Chapter I release validator","Assets/Editor/ChapterOneReleaseValidator.cs","Validator threw: " + ex.Message);
            return;
        }

        if (errors == 0) Pass(rows,"Release contract","Chapter I release validator","Assets/Editor/ChapterOneReleaseValidator.cs","Existing Chapter I RC contract is intact.");
        else Broken(rows,"Release contract","Chapter I release validator","Assets/Editor/ChapterOneReleaseValidator.cs",$"Existing release validator reports {errors} error(s).");
    }

    static void ValidateRuntimeRosterContract(List<FreezeRow> rows)
    {
        string path = "Assets/Game/Core/Balance/BalanceCatalog.cs";
        SourceContainsAll(rows,"Runtime roster","Chapter I enemy roster mapping",path,new[]
        {
            "EnemyArchetype.Infantry", "EnemyArchetype.Runner", "EnemyArchetype.HeavyHoplite",
            "EnemyArchetype.ShieldBearer", "EnemyArchetype.Archer", "EnemyArchetype.Boss"
        });

        path = "Assets/Game/Towers/TowerType.cs";
        SourceContainsAll(rows,"Runtime roster","Six Chapter I defense types",path,new[]
        {
            "MachineGun", "Cannon", "Slow", "SpearThrower", "FireTower", "TrojanGuard"
        });
    }

    static void ValidateCharacters(List<FreezeRow> rows, HashSet<string> accepted)
    {
        foreach (string path in RequiredCharacterPaths)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                Broken(rows,"Character",name,path,"Required Chapter I production prefab is missing. Build/import the candidate before freeze review.");
                continue;
            }

            var faults = new List<string>();
            Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0) faults.Add("no renderer");
            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.sharedMaterials;
                if (materials == null || materials.Length == 0) { faults.Add("renderer without material"); break; }
                bool missing = false;
                foreach (Material material in materials) if (material == null) { missing = true; break; }
                if (missing) { faults.Add("missing material"); break; }
            }

            Animator animator = prefab.GetComponentInChildren<Animator>(true);
            if (animator == null || animator.runtimeAnimatorController == null) faults.Add("Animator/controller missing");
            if (prefab.GetComponentInChildren<CharacterVisualIdentity>(true) == null) faults.Add("CharacterVisualIdentity missing");
            if (path.IndexOf("/Resources/",StringComparison.Ordinal) < 0) faults.Add("not under Resources");

            if (faults.Count > 0)
            {
                Broken(rows,"Character",name,path,string.Join("; ",faults));
                continue;
            }

            if (!accepted.Contains(path))
                Blocked(rows,"Character",name,path,"Structurally valid but not explicitly accepted in PRODUCTION_ACCEPTANCE.json after gameplay-camera visual QA.");
            else
                Pass(rows,"Character",name,path,"Production asset is structurally valid and explicitly accepted.");
        }
    }

    static void ValidateAnimationController(List<FreezeRow> rows)
    {
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(AnimationControllerPath);
        if (controller == null)
        {
            Broken(rows,"Animation","Chapter I character controller",AnimationControllerPath,"AnimatorController is missing. Run the Chapter I animation builder/import pipeline.");
            return;
        }

        var expected = new Dictionary<string,AnimatorControllerParameterType>(StringComparer.Ordinal)
        {
            { "Speed", AnimatorControllerParameterType.Float },
            { "Attack", AnimatorControllerParameterType.Trigger },
            { "Hit", AnimatorControllerParameterType.Trigger },
            { "Die", AnimatorControllerParameterType.Trigger },
            { "IsDowned", AnimatorControllerParameterType.Bool }
        };
        var actual = new Dictionary<string,AnimatorControllerParameterType>(StringComparer.Ordinal);
        foreach (AnimatorControllerParameter parameter in controller.parameters) actual[parameter.name] = parameter.type;

        var faults = new List<string>();
        foreach (var pair in expected)
        {
            AnimatorControllerParameterType type;
            if (!actual.TryGetValue(pair.Key,out type)) faults.Add("missing " + pair.Key);
            else if (type != pair.Value) faults.Add(pair.Key + " has wrong type");
        }

        if (faults.Count > 0) Broken(rows,"Animation","Chapter I character controller",AnimationControllerPath,string.Join("; ",faults));
        else Pass(rows,"Animation","Chapter I character controller",AnimationControllerPath,"Shared controller has the required Speed/Attack/Hit/Die/IsDowned contract.");

        SourceContainsAll(rows,"Animation","Animation candidate builder","Assets/Editor/ChapterOneCharacterAnimationBuilder.cs",new[]
        {
            "ChapterOneCharacter.controller", "AnimatorControllerParameterType.Float", "AnimatorControllerParameterType.Trigger", "AnimatorControllerParameterType.Bool"
        });
    }

    static void ValidateTowerUnits(List<FreezeRow> rows)
    {
        foreach (string path in RequiredTowerDataPaths)
        {
            TowerData data = AssetDatabase.LoadAssetAtPath<TowerData>(path);
            string name = Path.GetFileNameWithoutExtension(path);
            if (data == null) Broken(rows,"Tower-Unit",name,path,"Required authored TowerData is missing or invalid.");
            else Pass(rows,"Tower-Unit",name,path,"Authored TowerData exists.");
        }

        SourceContainsAll(rows,"Tower-Unit","Tower visual ownership","Assets/Game/Towers/TowerArtDirector.cs",new[]
        {
            "TrojanProductionRoot", "TowerType.MachineGun", "TowerType.Cannon", "TowerType.Slow",
            "TowerType.SpearThrower", "TowerType.FireTower", "TowerType.TrojanGuard", "Trojan_Archer", "Trojan_Guard"
        });
        SourceContainsAll(rows,"Tower-Unit","Production support binder","Assets/Game/Towers/TowerProductionArtBinder.cs",new[]
        {
            "Trojan_BallistaCrew", "Trojan_PriestApollo", "Trojan_FireKeeper", "UpgradeVisual_L2", "UpgradeVisual_L3"
        });
        SourceContainsAll(rows,"Tower-Unit","Tower factory binding","Assets/Game/Towers/TowerFactory.cs",new[]
        {
            "TowerArtDirector.Enhance", "AddComponent<TowerProductionArtBinder>"
        });
    }

    static void ValidateEnvironment(List<FreezeRow> rows)
    {
        SourceContainsAll(rows,"Environment","Coast presentation","Assets/Game/World/CoastEnvironmentBuilder.cs",new[] { "CoastEnvironmentBuilder" });
        SourceContainsAll(rows,"Environment","Greek landing ship","Assets/Game/World/GreekLandingShipVisualFactory.cs",new[] { "GreekLandingShipVisualFactory" });
        SourceContainsAll(rows,"Environment","Landing presentation","Assets/Game/World/LandingPresentation.cs",new[] { "LandingPresentation", "ProductionGreekRoot" });
        SourceContainsAll(rows,"Environment","Troy gate hero silhouette","Assets/Game/World/TroyGateHeroBuilder.cs",new[] { "TroyGateHeroBuilder" });
        SourceContainsAll(rows,"Environment","Troy gate damage states","Assets/Game/World/TroyGateDamagePresentation.cs",new[] { "TroyGateDamagePresentation" });
        SourceContainsAll(rows,"Environment","Troy skyline","Assets/Game/World/TroyCityBackdropPresentation.cs",new[] { "TroyCityBackdropPresentation" });
        SourceContainsAll(rows,"Environment","Atmosphere","Assets/Game/World/ChapterOneAtmosphereController.cs",new[] { "ChapterOneAtmosphereController" });
        SourceContainsAll(rows,"Environment","Wall life","Assets/Game/World/ChapterOneWallLife.cs",new[] { "ChapterOneWallLife" });
        SourceContainsAll(rows,"Environment","Battlefield details","Assets/Game/World/ChapterOneBattlefieldDetails.cs",new[] { "ChapterOneBattlefieldDetails" });
        SourceContainsAll(rows,"Environment","Shore life","Assets/Game/World/ChapterOneShoreLife.cs",new[] { "ChapterOneShoreLife" });
        SourceContainsAll(rows,"Environment","Cinematic camera","Assets/Game/World/ChapterOneCinematicCamera.cs",new[] { "ChapterOneCinematicCamera" });
    }

    static void ValidateHumanQa(List<FreezeRow> rows, FreezeAcceptance qa)
    {
        if (qa == null) return;
        Qa(rows,"Play Mode 16:9 visual QA",qa.playModeVisualQa,"Real Chapter I Play Mode visual pass has not been accepted.");
        Qa(rows,"English framing QA",qa.englishFramingQa,"EN framing/readability has not been accepted.");
        Qa(rows,"Russian framing QA",qa.russianFramingQa,"RU framing/readability has not been accepted.");
        Qa(rows,"Tower-Unit visual QA",qa.towerUnitVisualQa,"All six Tower-Units and L1/L2/L3 readability have not been accepted.");
        Qa(rows,"Environment visual QA",qa.environmentVisualQa,"Coast, ships, Troy gate and skyline have not been accepted from gameplay camera.");
        Qa(rows,"Animation visual QA",qa.animationVisualQa,"Character combat/hero animation presentation has not been accepted.");

        if (string.Equals(qa.referenceResolution,"1920x1080",StringComparison.OrdinalIgnoreCase))
            Pass(rows,"Human QA","Reference resolution",FreezeAcceptancePath,"Reference QA resolution is 1920x1080.");
        else
            Blocked(rows,"Human QA","Reference resolution",FreezeAcceptancePath,"Freeze requires referenceResolution=1920x1080.");
    }

    static void Qa(List<FreezeRow> rows,string name,bool value,string blockedMessage)
    {
        if (value) Pass(rows,"Human QA",name,FreezeAcceptancePath,"Explicitly accepted after real visual QA.");
        else Blocked(rows,"Human QA",name,FreezeAcceptancePath,blockedMessage);
    }

    static void SourceContainsAll(List<FreezeRow> rows,string category,string name,string path,string[] tokens)
    {
        if (!File.Exists(path))
        {
            Broken(rows,category,name,path,"Required source file missing.");
            return;
        }
        string source = File.ReadAllText(path);
        var missing = new List<string>();
        foreach (string token in tokens) if (!source.Contains(token)) missing.Add(token);
        if (missing.Count > 0) Broken(rows,category,name,path,"Missing contract token(s): " + string.Join(", ",missing));
        else Pass(rows,category,name,path,"Required production-art contract is present.");
    }

    static FreezeReport BuildReport(List<FreezeRow> rows)
    {
        var report = new FreezeReport { generatedUtc=DateTime.UtcNow.ToString("O"), rows=rows.ToArray(), total=rows.Count };
        foreach (FreezeRow row in rows)
        {
            if (row.status == "PASS") report.pass++;
            else if (row.status == "BLOCKED") report.blocked++;
            else if (row.status == "BROKEN") report.broken++;
        }
        report.readyForFreeze = report.blocked == 0 && report.broken == 0;
        return report;
    }

    static void WriteReport(FreezeReport report)
    {
        Directory.CreateDirectory(ReportDir);
        File.WriteAllText(Path.Combine(ReportDir,"ChapterOneArtFreeze.json"),JsonUtility.ToJson(report,true));
        var md = new StringBuilder();
        md.AppendLine("# Chapter I Production Art Freeze").AppendLine();
        md.AppendLine($"Generated: `{report.generatedUtc}`");
        md.AppendLine($"Ready for freeze: **{report.readyForFreeze}** | PASS: **{report.pass}** | BLOCKED: **{report.blocked}** | BROKEN: **{report.broken}**").AppendLine();
        md.AppendLine("| Status | Category | Requirement | Details |").AppendLine("|---|---|---|---|");
        foreach (FreezeRow row in report.rows) md.AppendLine($"| {row.status} | {row.category} | `{row.name}` | {row.details} |");
        File.WriteAllText(Path.Combine(ReportDir,"ChapterOneArtFreeze.md"),md.ToString());
    }

    static void LogReport(FreezeReport report)
    {
        string summary = $"[CHAPTER I ART FREEZE] ready={report.readyForFreeze} pass={report.pass} blocked={report.blocked} broken={report.broken}. Reports: {ReportDir}";
        if (report.readyForFreeze) Debug.Log(summary); else Debug.LogWarning(summary);
        foreach (FreezeRow row in report.rows)
            if (row.status != "PASS") Debug.LogWarning($"[CHAPTER I ART FREEZE] {row.status}: {row.name} - {row.details}");
    }

    static void Pass(List<FreezeRow> rows,string category,string name,string path,string details) => Add(rows,"PASS",category,name,path,details);
    static void Blocked(List<FreezeRow> rows,string category,string name,string path,string details) => Add(rows,"BLOCKED",category,name,path,details);
    static void Broken(List<FreezeRow> rows,string category,string name,string path,string details) => Add(rows,"BROKEN",category,name,path,details);
    static void Add(List<FreezeRow> rows,string status,string category,string name,string path,string details)
    {
        rows.Add(new FreezeRow { status=status, category=category, name=name, path=path, details=details });
    }
}
#endif
