#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;

[InitializeOnLoad]
public static class UnifiedEditorMenuCompatibility
{
    const string Root = "The Troy Game/";
    const string LegacyRoot = "TheTroyGame/";

    static readonly string[] LegacyMenuPaths =
    {
        "TheTroyGame/Validation/Run Architecture Smoke Checks",
        "TheTroyGame/Data/Create Missing Default Assets",
        "TheTroyGame/Validation/Validate Chapter I Release Candidate",
        "TheTroyGame/Validation/Audit Chapter I Art Freeze",
        "TheTroyGame/Validation/Build Chapter I Candidates + Audit Art Freeze",
        "TheTroyGame/Validation/Gameplay Acceptance/Prepare Latest Story Candidate",
        "TheTroyGame/Validation/Gameplay Acceptance/Check Final Chapter I Acceptance",
        "TheTroyGame/Validation/Gameplay Acceptance/Open Acceptance Manifest",
        "TheTroyGame/Validation/Check Chapter I Gameplay Freeze Readiness",
        "TheTroyGame/Validation/Analyze Latest Chapter I Playthrough",
        "TheTroyGame/Validation/Open Chapter I Playthrough Logs",
        "TheTroyGame/Validation/Gameplay Acceptance/Analyze Story-Strategos-Legendary Pressure",
        "TheTroyGame/Validation/Visual Fit/1920x1080 RU",
        "TheTroyGame/Validation/Visual Fit/1920x1080 EN",
        "TheTroyGame/Validation/Visual Fit/1376x768 RU",
        "TheTroyGame/Validation/Visual Fit/1376x768 EN",
        "TheTroyGame/Validation/Visual Fit/Generate Chapter I QA Checklist",
        "TheTroyGame/Validation/Visual Fit/Check Visual-Fit Acceptance",
        "TheTroyGame/Validation/Audit Campaign Models",
        "TheTroyGame/Validation/Build Candidates + Audit Campaign Models"
    };

    static UnifiedEditorMenuCompatibility()
    {
        EditorApplication.delayCall += RemoveLegacyMenuEntries;
    }

    static void RemoveLegacyMenuEntries()
    {
        Type menuType = typeof(Editor).Assembly.GetType("UnityEditor.Menu");
        MethodInfo remove = menuType?.GetMethod(
            "RemoveMenuItem",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            new[] { typeof(string) },
            null);
        if (remove == null) return;

        foreach (string path in LegacyMenuPaths)
            remove.Invoke(null, new object[] { path });

        // Some Unity versions retain an empty parent until explicitly removed.
        remove.Invoke(null, new object[] { "TheTroyGame" });
    }

    [MenuItem(Root + "Validation/Run Architecture Smoke Checks")]
    static void RunArchitectureSmokeChecks() => ArchitectureSmokeValidator.Run();

    [MenuItem(Root + "Data/Create Missing Default Assets")]
    static void CreateMissingDefaultAssets() => DefaultDataAssetGenerator.EnsureMissingAssets();

    [MenuItem(Root + "Validation/Validate Chapter I Release Candidate")]
    static void ValidateChapterOneReleaseCandidate() => ChapterOneReleaseValidator.Run();

    [MenuItem(Root + "Validation/Audit Chapter I Art Freeze")]
    static void AuditChapterOneArtFreeze() => ChapterOneArtFreezeValidator.RunMenu();

    [MenuItem(Root + "Validation/Build Chapter I Candidates + Audit Art Freeze")]
    static void BuildAndAuditChapterOneArtFreeze() => ChapterOneArtFreezeValidator.BuildAndRunMenu();

    [MenuItem(Root + "Validation/Gameplay Acceptance/Prepare Latest Story Candidate")]
    static void PrepareLatestStoryCandidate() => ChapterOneGameplayAcceptanceValidator.PrepareLatestStoryCandidate();

    [MenuItem(Root + "Validation/Gameplay Acceptance/Check Final Chapter I Acceptance")]
    static void CheckFinalChapterOneAcceptance() => ChapterOneGameplayAcceptanceValidator.CheckFinalAcceptance();

    [MenuItem(Root + "Validation/Gameplay Acceptance/Open Acceptance Manifest")]
    static void OpenGameplayAcceptanceManifest() => ChapterOneGameplayAcceptanceValidator.OpenManifest();

    [MenuItem(Root + "Validation/Check Chapter I Gameplay Freeze Readiness")]
    static void CheckChapterOneGameplayFreezeReadiness() => ChapterOneGameplayFreezeValidator.CheckLatest();

    [MenuItem(Root + "Validation/Analyze Latest Chapter I Playthrough")]
    static void AnalyzeLatestChapterOnePlaythrough() => ChapterOnePlaythroughAnalyzer.AnalyzeLatest();

    [MenuItem(Root + "Validation/Open Chapter I Playthrough Logs")]
    static void OpenChapterOnePlaythroughLogs() => ChapterOnePlaythroughAnalyzer.OpenLogsFolder();

    [MenuItem(Root + "Validation/Gameplay Acceptance/Analyze Story-Strategos-Legendary Pressure")]
    static void AnalyzeDifficultyPressure() => ChapterOneDifficultyPressureAnalyzer.AnalyzeLatestDifficultySet();

    [MenuItem(Root + "Validation/Visual Fit/1920x1080 RU")]
    static void VisualFit1920Ru() => ChapterOneVisualFitQaTool.Set1920Ru();

    [MenuItem(Root + "Validation/Visual Fit/1920x1080 EN")]
    static void VisualFit1920En() => ChapterOneVisualFitQaTool.Set1920En();

    [MenuItem(Root + "Validation/Visual Fit/1376x768 RU")]
    static void VisualFit1376Ru() => ChapterOneVisualFitQaTool.Set1376Ru();

    [MenuItem(Root + "Validation/Visual Fit/1376x768 EN")]
    static void VisualFit1376En() => ChapterOneVisualFitQaTool.Set1376En();

    [MenuItem(Root + "Validation/Visual Fit/Generate Chapter I QA Checklist")]
    static void GenerateVisualFitChecklist() => ChapterOneVisualFitQaTool.GenerateChecklist();

    [MenuItem(Root + "Validation/Visual Fit/Check Visual-Fit Acceptance")]
    static void CheckVisualFitAcceptance() => ChapterOneVisualFitQaTool.CheckAcceptance();

    [MenuItem(Root + "Validation/Audit Campaign Models")]
    static void AuditCampaignModels() => CampaignModelAuditValidator.RunMenu();

    [MenuItem(Root + "Validation/Build Candidates + Audit Campaign Models")]
    static void BuildAndAuditCampaignModels() => CampaignModelAuditValidator.BuildAndRunMenu();
}
#endif
