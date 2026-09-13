#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class CampaignModelAuditValidator
{
    public enum AuditStatus { Done, Candidate, Missing, Broken }
    enum ColliderPolicy { Ignore, None }

    [Serializable]
    public sealed class AuditRow
    {
        public string name;
        public string category;
        public string path;
        public string status;
        public string details;
        public int renderers;
        public int colliders;
        public bool animator;
    }

    [Serializable]
    public sealed class AuditReport
    {
        public string generatedUtc;
        public int total;
        public int done;
        public int candidate;
        public int missing;
        public int broken;
        public AuditRow[] rows;
    }

    sealed class Spec
    {
        public readonly string name;
        public readonly string category;
        public readonly string path;
        public readonly bool animator;
        public readonly bool resources;
        public readonly ColliderPolicy colliders;
        public readonly bool finalAccepted;

        public Spec(string name,string category,string path,bool animator=false,bool resources=true,ColliderPolicy colliders=ColliderPolicy.Ignore,bool finalAccepted=false)
        {
            this.name=name; this.category=category; this.path=path; this.animator=animator;
            this.resources=resources; this.colliders=colliders; this.finalAccepted=finalAccepted;
        }
    }

    const string CharacterRoot = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters";
    const string Greek = CharacterRoot + "/Greek/";
    const string Trojan = CharacterRoot + "/Trojan/";
    const string Heroes = CharacterRoot + "/Heroes/";
    const string Mythic = CharacterRoot + "/Mythic/";
    const string Vehicles = "Assets/Game/Art/Vehicles/Resources/TroyProduction/Vehicles/";
    const string Siege = "Assets/Game/Art/Vehicles/Resources/TroyProduction/Siege/";
    const string Props = "Assets/Game/Art/Props/Resources/TroyProduction/Props/";
    const string Env = "Assets/Game/Art/Environment/Resources/TroyProduction/Environment/";
    const string ReportDir = "Logs/Validation";

    static readonly Spec[] Specs =
    {
        C("Greek Infantry",Greek+"Enemy_Infantry.prefab"), C("Greek Runner",Greek+"Enemy_Runner.prefab"),
        C("Greek Heavy Hoplite",Greek+"Enemy_HeavyHoplite.prefab"), C("Greek Shield Bearer",Greek+"Enemy_ShieldBearer.prefab"),
        C("Greek Archer",Greek+"Enemy_Archer.prefab"), C("Menelaus enemy boss",Greek+"Enemy_Boss.prefab"),
        C("Greek Spearman",Greek+"Enemy_Spearman.prefab"), C("Greek Scout",Greek+"Enemy_Scout.prefab"),
        C("Greek Light Swordsman",Greek+"Enemy_LightSwordsman.prefab"), C("Greek Hoplite",Greek+"Enemy_Hoplite.prefab"),
        C("Myrmidon",Greek+"Enemy_Myrmidon.prefab"), C("Myrmidon Veteran",Greek+"Enemy_MyrmidonVeteran.prefab"),
        C("Sapper",Greek+"Enemy_Sapper.prefab"), C("Greek Captain",Greek+"Enemy_GreekCaptain.prefab"),
        C("Hero Companion",Greek+"Enemy_HeroCompanion.prefab"), C("Ram Crew",Greek+"Enemy_RamCrew.prefab"),

        C("Trojan Infantry",Trojan+"Trojan_Infantry.prefab"), C("Trojan Guard",Trojan+"Trojan_Guard.prefab"),
        C("Trojan Archer",Trojan+"Trojan_Archer.prefab"), C("Priest of Apollo",Trojan+"Trojan_PriestApollo.prefab"),
        C("Fire Keeper",Trojan+"Trojan_FireKeeper.prefab"), C("Ballista Crew",Trojan+"Trojan_BallistaCrew.prefab"),
        C("Trojan Civilian",Trojan+"Trojan_Civilian.prefab"), C("Civilian Worker",Trojan+"Trojan_Civilian_Worker.prefab"),
        C("Civilian Elder",Trojan+"Trojan_Civilian_Elder.prefab"), C("Civilian Young",Trojan+"Trojan_Civilian_Young.prefab"),

        C("Hector",Heroes+"Hero_Hector.prefab"), C("Menelaus hero representation",Heroes+"Hero_Menelaus.prefab"),
        C("Achilles",Heroes+"Hero_Achilles.prefab"), C("Ajax",Heroes+"Hero_Ajax.prefab"), C("Odysseus",Heroes+"Hero_Odysseus.prefab"),
        C("Cyclops",Mythic+"Mythic_Cyclops.prefab"),

        P("Chariot",Vehicles+"Vehicle_Chariot.prefab"), P("Battering Ram",Siege+"Siege_BatteringRam.prefab"),
        P("Siege Tower",Siege+"Siege_SiegeTower.prefab"), P("Trojan Horse",Props+"Prop_TrojanHorse.prefab"),

        E("Plains Road",Env+"Chapter02_Plains/Env_PlainsRoadSegment.prefab"),
        E("Plains Junction",Env+"Chapter02_Plains/Env_PlainsRoadJunction.prefab"),
        E("Chariot Roadside Set",Env+"Chapter02_Plains/Env_ChariotRoadsideSet.prefab"),
        E("Siege Wall Breach",Env+"Chapter03_Siege/Env_SiegeWallBreach.prefab"),
        E("Siege Staging Set",Env+"Chapter03_Siege/Env_SiegeStagingSet.prefab"),
        E("Damaged Outer Defense",Env+"Chapter04_DamagedDefenses/Env_DamagedOuterDefense.prefab"),
        E("Destroyed Build Node",Env+"Chapter05_GreatAssault/Env_DestroyedBuildNode.prefab"),
        E("Great Assault Wall State",Env+"Chapter05_GreatAssault/Env_GreatAssaultWallState.prefab"),
        E("Abandoned Greek Camp",Env+"Chapter06_GreekCamp/Env_AbandonedGreekCamp.prefab"),
        E("Trojan Horse Plaza",Env+"Chapter06_GreekCamp/Env_TrojanHorsePlaza.prefab"),
        E("Troy Interior House",Env+"Chapter07_TroyInterior/Env_TroyInteriorHouse.prefab"),
        E("Troy Street Module",Env+"Chapter07_TroyInterior/Env_TroyStreetModule.prefab"),
        E("Burning Troy House",Env+"Chapter07_TroyInterior/Env_BurningTroyHouse.prefab"),
        E("Collapsed Troy House",Env+"Chapter07_TroyInterior/Env_CollapsedTroyHouse.prefab"),
        E("Evacuation Street",Env+"Chapter07_TroyInterior/Env_EvacuationStreet.prefab")
    };

    static Spec C(string n,string p) => new Spec(n,"Character",p,true,true,ColliderPolicy.Ignore);
    static Spec P(string n,string p) => new Spec(n,"Vehicle / Prop",p,false,true,ColliderPolicy.None);
    static Spec E(string n,string p) => new Spec(n,"Environment",p,false,true,ColliderPolicy.None);

    [MenuItem("TheTroyGame/Validation/Audit Campaign Models")]
    public static void RunMenu() => Run(true);

    [MenuItem("TheTroyGame/Validation/Build Candidates + Audit Campaign Models")]
    public static void BuildAndRunMenu()
    {
        ModelGapClosureBuilder.BuildAll();
        Run(true);
    }

    public static AuditReport Run(bool log)
    {
        List<AuditRow> rows = new List<AuditRow>(Specs.Length + 8);
        foreach (Spec spec in Specs) rows.Add(AuditPrefab(spec));
        AddPipelineChecks(rows);
        AuditReport report = BuildReport(rows);
        WriteReport(report);
        if (log) LogReport(report);
        return report;
    }

    public static void BuildAndRunBatchmode()
    {
        ModelGapClosureBuilder.BuildAll();
        AuditReport report = Run(true);
        EditorApplication.Exit(report.missing == 0 && report.broken == 0 ? 0 : 1);
    }

    static AuditRow AuditPrefab(Spec spec)
    {
        AuditRow row = NewRow(spec);
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(spec.path);
        if (prefab == null)
        {
            row.status = AuditStatus.Missing.ToString().ToUpperInvariant();
            row.details = "Prefab not generated/imported.";
            return row;
        }

        List<string> faults = new List<string>();
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);
        Collider[] colliders = prefab.GetComponentsInChildren<Collider>(true);
        Animator animator = prefab.GetComponentInChildren<Animator>(true);
        row.renderers = renderers.Length;
        row.colliders = colliders.Length;
        row.animator = animator != null && animator.runtimeAnimatorController != null;

        if (renderers.Length == 0) faults.Add("no renderer");
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;
            if (materials == null || materials.Length == 0) { faults.Add("renderer without material"); break; }
            foreach (Material material in materials) if (material == null) { faults.Add("missing material"); break; }
            if (faults.Contains("missing material")) break;
        }
        if (spec.animator && !row.animator) faults.Add("Animator/controller missing");
        if (spec.resources && spec.path.IndexOf("/Resources/",StringComparison.Ordinal) < 0) faults.Add("not under Resources");
        if (!spec.path.StartsWith("Assets/Game/Art/",StringComparison.Ordinal)) faults.Add("outside production art root");
        if (spec.colliders == ColliderPolicy.None && colliders.Length > 0) faults.Add("decorative prefab has collider(s)");

        if (faults.Count > 0)
        {
            row.status = AuditStatus.Broken.ToString().ToUpperInvariant();
            row.details = string.Join("; ",faults);
        }
        else
        {
            row.status = (spec.finalAccepted ? AuditStatus.Done : AuditStatus.Candidate).ToString().ToUpperInvariant();
            row.details = spec.finalAccepted ? "Accepted production asset." : "Structurally valid candidate; final visual QA still required.";
        }
        return row;
    }

    static AuditRow NewRow(Spec spec) => new AuditRow { name=spec.name, category=spec.category, path=spec.path };

    static void AddPipelineChecks(List<AuditRow> rows)
    {
        SourceCheck(rows,"Tower-Unit production binder","Pipeline","Assets/Game/Towers/TowerProductionArtBinder.cs","UpgradeVisual_L","Production crew + L2/L3 visual binding");
        SourceCheck(rows,"Tower art director","Pipeline","Assets/Game/Towers/TowerArtDirector.cs","TrojanProductionRoot","Production-first Tower-Unit art path");
        SourceCheck(rows,"Full candidate build entrypoint","Pipeline","Assets/Editor/ModelGapClosureBuilder.cs","CampaignChariotHorseBuilder.Build()","One-shot campaign candidate build");
        SourceCheck(rows,"Animated horse source installer","Pipeline","Assets/Editor/CampaignHorseSourceInstaller.cs","ExpectedGitBlobSha","Pinned horse source integrity verification");
        SourceCheck(rows,"Chariot horse builder","Pipeline","Assets/Editor/CampaignChariotHorseBuilder.cs","ChariotHorse.controller","Animated chariot-horse integration");
        SourceCheck(rows,"Environment candidate builder","Pipeline","Assets/Editor/CampaignEnvironmentCandidateBuilder.cs","BuildEvacuationStreet","Chapter II-VII environment coverage");
    }

    static void SourceCheck(List<AuditRow> rows,string name,string category,string path,string token,string description)
    {
        AuditRow row = new AuditRow { name=name, category=category, path=path };
        if (!File.Exists(path))
        {
            row.status = AuditStatus.Missing.ToString().ToUpperInvariant();
            row.details = "Required source file missing.";
        }
        else if (!File.ReadAllText(path).Contains(token))
        {
            row.status = AuditStatus.Broken.ToString().ToUpperInvariant();
            row.details = "Required contract token missing: " + token;
        }
        else
        {
            row.status = AuditStatus.Candidate.ToString().ToUpperInvariant();
            row.details = description;
        }
        rows.Add(row);
    }

    static AuditReport BuildReport(List<AuditRow> rows)
    {
        AuditReport report = new AuditReport { generatedUtc=DateTime.UtcNow.ToString("O"), rows=rows.ToArray(), total=rows.Count };
        foreach (AuditRow row in rows)
        {
            if (row.status == "DONE") report.done++;
            else if (row.status == "CANDIDATE") report.candidate++;
            else if (row.status == "MISSING") report.missing++;
            else if (row.status == "BROKEN") report.broken++;
        }
        return report;
    }

    static void WriteReport(AuditReport report)
    {
        Directory.CreateDirectory(ReportDir);
        File.WriteAllText(Path.Combine(ReportDir,"CampaignModelAudit.json"),JsonUtility.ToJson(report,true));
        StringBuilder md = new StringBuilder();
        md.AppendLine("# Campaign Model Audit").AppendLine();
        md.AppendLine($"Generated: `{report.generatedUtc}`");
        md.AppendLine($"Total: **{report.total}** | DONE: **{report.done}** | CANDIDATE: **{report.candidate}** | MISSING: **{report.missing}** | BROKEN: **{report.broken}**").AppendLine();
        md.AppendLine("| Status | Category | Asset | Details |").AppendLine("|---|---|---|---|");
        foreach (AuditRow row in report.rows) md.AppendLine($"| {row.status} | {row.category} | `{row.name}` | {row.details} |");
        File.WriteAllText(Path.Combine(ReportDir,"CampaignModelAudit.md"),md.ToString());
        AssetDatabase.Refresh();
    }

    static void LogReport(AuditReport report)
    {
        string summary = $"[CAMPAIGN MODEL AUDIT] total={report.total} done={report.done} candidate={report.candidate} missing={report.missing} broken={report.broken}. Reports: {ReportDir}";
        if (report.missing > 0 || report.broken > 0) Debug.LogError(summary); else Debug.Log(summary);
        foreach (AuditRow row in report.rows)
            if (row.status == "MISSING" || row.status == "BROKEN") Debug.LogError($"[CAMPAIGN MODEL AUDIT] {row.status}: {row.name} - {row.details} ({row.path})");
    }
}
#endif
