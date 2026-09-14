#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class ChapterOneGameplayAcceptanceValidator
{
    const string ManifestPath = "Assets/Game/QA/CHAPTER_I_GAMEPLAY_ACCEPTANCE.json";

    [Serializable]
    public sealed class StoryBaselineAcceptance
    {
        public string sourceSessionId;
        public string sourceReportFile;
        public string sourceReportSha256;
        public bool warningsReviewed;
        public string warningsDecisionNotes;
        public bool humanAccepted;
        public string acceptedBy;
        public string acceptedUtc;
    }

    [Serializable]
    public sealed class DifficultyPressureAcceptance
    {
        public bool strategosReviewed;
        public bool legendaryReviewed;
        public string notes;
    }

    [Serializable]
    public sealed class VisualFitAcceptance
    {
        public bool ru1920x1080;
        public bool en1920x1080;
        public bool ru1366or1376x768;
        public bool en1366or1376x768;
        public string notes;
    }

    [Serializable]
    public sealed class GameplayAcceptanceManifest
    {
        public int schemaVersion = 1;
        public StoryBaselineAcceptance storyBaseline = new StoryBaselineAcceptance();
        public DifficultyPressureAcceptance difficultyPressure = new DifficultyPressureAcceptance();
        public VisualFitAcceptance visualFit = new VisualFitAcceptance();
        public bool gameplayFrozen;
        public string freezeNotes;
    }

    [MenuItem("TheTroyGame/Validation/Gameplay Acceptance/Prepare Latest Story Candidate")]
    public static void PrepareLatestStoryCandidate()
    {
        string reportPath = FindLatestReportForDifficulty("Story");
        if (string.IsNullOrEmpty(reportPath))
        {
            Debug.LogError("[CHAPTER I ACCEPTANCE] No Story ChapterI_Playthrough_*.json found. Complete a clean Story 1x/no-pause run first.");
            return;
        }

        if (!ChapterOneGameplayFreezeValidator.Check(reportPath, false))
        {
            Debug.LogError("[CHAPTER I ACCEPTANCE] Latest Story report is not freeze-ready. Resolve hard blockers before binding it to the acceptance manifest.");
            return;
        }

        ChapterOnePlaythroughReporter.PlaythroughReport report = ReadReport(reportPath);
        GameplayAcceptanceManifest manifest = LoadManifest();
        bool changedSource = !string.Equals(manifest.storyBaseline.sourceSessionId, report.sessionId, StringComparison.Ordinal);
        if (changedSource)
        {
            manifest.storyBaseline.warningsReviewed = false;
            manifest.storyBaseline.warningsDecisionNotes = "";
            manifest.storyBaseline.humanAccepted = false;
            manifest.storyBaseline.acceptedBy = "";
            manifest.storyBaseline.acceptedUtc = "";
            manifest.difficultyPressure.strategosReviewed = false;
            manifest.difficultyPressure.legendaryReviewed = false;
            manifest.difficultyPressure.notes = "";
            manifest.visualFit.ru1920x1080 = false;
            manifest.visualFit.en1920x1080 = false;
            manifest.visualFit.ru1366or1376x768 = false;
            manifest.visualFit.en1366or1376x768 = false;
            manifest.visualFit.notes = "";
            manifest.gameplayFrozen = false;
        }

        manifest.storyBaseline.sourceSessionId = report.sessionId;
        manifest.storyBaseline.sourceReportFile = Path.GetFileName(reportPath);
        manifest.storyBaseline.sourceReportSha256 = ComputeSha256(reportPath);
        SaveManifest(manifest);

        string analysisPath = ChapterOnePlaythroughAnalyzer.Analyze(reportPath, false);
        string reviewPath = WriteWarningReviewTemplate(report, analysisPath);
        Debug.Log($"[CHAPTER I ACCEPTANCE] Story candidate bound to manifest. session={report.sessionId}, sha256={manifest.storyBaseline.sourceReportSha256}. WARN review template={reviewPath}");
        EditorUtility.RevealInFinder(reviewPath);
    }

    [MenuItem("TheTroyGame/Validation/Gameplay Acceptance/Check Final Chapter I Acceptance")]
    public static void CheckFinalAcceptance()
    {
        GameplayAcceptanceManifest manifest = LoadManifest();
        List<string> blockers = new List<string>();

        if (manifest.schemaVersion != 1) blockers.Add($"Unsupported gameplay acceptance schemaVersion={manifest.schemaVersion}; expected 1.");
        if (manifest.storyBaseline == null) blockers.Add("storyBaseline section is missing.");
        if (manifest.difficultyPressure == null) blockers.Add("difficultyPressure section is missing.");
        if (manifest.visualFit == null) blockers.Add("visualFit section is missing.");

        if (blockers.Count == 0)
        {
            ValidateStory(manifest.storyBaseline, blockers);
            ValidateDifficulty(manifest.difficultyPressure, blockers);
            ValidateVisualFit(manifest.visualFit, blockers);
        }

        if (manifest.gameplayFrozen && blockers.Count > 0)
            blockers.Insert(0, "gameplayFrozen=true is invalid while prerequisite acceptance gates are incomplete.");

        if (blockers.Count > 0)
        {
            Debug.LogError($"[CHAPTER I ACCEPTANCE] BLOCKED - {blockers.Count} blocker(s).");
            foreach (string blocker in blockers) Debug.LogError($"[CHAPTER I ACCEPTANCE] {blocker}");
            return;
        }

        if (!manifest.gameplayFrozen)
        {
            Debug.Log("[CHAPTER I ACCEPTANCE] READY TO FREEZE - all Story/WARN/difficulty/visual-fit gates are accepted. Set gameplayFrozen=true only as the explicit final human action.");
            return;
        }

        Debug.Log($"[CHAPTER I ACCEPTANCE] PASS - Chapter I gameplay baseline is explicitly frozen. acceptedBy={manifest.storyBaseline.acceptedBy}, acceptedUtc={manifest.storyBaseline.acceptedUtc}, session={manifest.storyBaseline.sourceSessionId}");
    }

    [MenuItem("TheTroyGame/Validation/Gameplay Acceptance/Open Acceptance Manifest")]
    public static void OpenManifest()
    {
        UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(ManifestPath);
        if (asset == null)
        {
            Debug.LogError($"[CHAPTER I ACCEPTANCE] Missing {ManifestPath}");
            return;
        }
        Selection.activeObject = asset;
        EditorGUIUtility.PingObject(asset);
    }

    static void ValidateStory(StoryBaselineAcceptance story, List<string> blockers)
    {
        if (string.IsNullOrWhiteSpace(story.sourceSessionId)) blockers.Add("Story sourceSessionId is not bound.");
        if (string.IsNullOrWhiteSpace(story.sourceReportFile)) blockers.Add("Story sourceReportFile is not bound.");
        if (string.IsNullOrWhiteSpace(story.sourceReportSha256)) blockers.Add("Story sourceReportSha256 is not bound.");
        if (!story.warningsReviewed) blockers.Add("Analyzer WARN findings have not been explicitly reviewed.");
        if (story.warningsReviewed && string.IsNullOrWhiteSpace(story.warningsDecisionNotes)) blockers.Add("WARN review requires warningsDecisionNotes describing fixes/accepted design reasons.");
        if (!story.humanAccepted) blockers.Add("Story baseline has not been explicitly humanAccepted.");
        if (story.humanAccepted && string.IsNullOrWhiteSpace(story.acceptedBy)) blockers.Add("Story human acceptance requires acceptedBy.");
        if (story.humanAccepted && !DateTime.TryParse(story.acceptedUtc, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _)) blockers.Add("Story human acceptance requires acceptedUtc in ISO-8601 format.");

        if (string.IsNullOrWhiteSpace(story.sourceReportFile)) return;
        string localPath = Path.Combine(Application.persistentDataPath, "Logs", story.sourceReportFile);
        if (!File.Exists(localPath))
        {
            blockers.Add($"Bound Story report is not present locally for verification: {localPath}");
            return;
        }

        string actualHash = ComputeSha256(localPath);
        if (!string.Equals(actualHash, story.sourceReportSha256, StringComparison.OrdinalIgnoreCase))
            blockers.Add($"Bound Story report SHA-256 changed: manifest={story.sourceReportSha256}, actual={actualHash}.");

        ChapterOnePlaythroughReporter.PlaythroughReport report = ReadReport(localPath);
        if (!string.Equals(report.sessionId, story.sourceSessionId, StringComparison.Ordinal))
            blockers.Add($"Bound Story report session mismatch: manifest={story.sourceSessionId}, report={report.sessionId}.");
        if (!ChapterOneGameplayFreezeValidator.Check(localPath, false))
            blockers.Add("Bound Story report no longer passes the hard gameplay-freeze validator.");
    }

    static void ValidateDifficulty(DifficultyPressureAcceptance difficulty, List<string> blockers)
    {
        if (!difficulty.strategosReviewed) blockers.Add("Strategos pressure pass has not been reviewed.");
        if (!difficulty.legendaryReviewed) blockers.Add("Legendary pressure pass has not been reviewed.");
        if ((difficulty.strategosReviewed || difficulty.legendaryReviewed) && string.IsNullOrWhiteSpace(difficulty.notes))
            blockers.Add("Difficulty pressure review requires notes describing the Strategos/Legendary result or accepted exceptions.");
    }

    static void ValidateVisualFit(VisualFitAcceptance visual, List<string> blockers)
    {
        if (!visual.ru1920x1080) blockers.Add("Visual-fit QA missing: 1920x1080 RU.");
        if (!visual.en1920x1080) blockers.Add("Visual-fit QA missing: 1920x1080 EN.");
        if (!visual.ru1366or1376x768) blockers.Add("Visual-fit QA missing: 1366x768 or 1376x768 RU.");
        if (!visual.en1366or1376x768) blockers.Add("Visual-fit QA missing: 1366x768 or 1376x768 EN.");
        if ((visual.ru1920x1080 || visual.en1920x1080 || visual.ru1366or1376x768 || visual.en1366or1376x768) && string.IsNullOrWhiteSpace(visual.notes))
            blockers.Add("Visual-fit QA requires notes identifying the tested build/editor state and any accepted visual limitations.");
    }

    static string WriteWarningReviewTemplate(ChapterOnePlaythroughReporter.PlaythroughReport report, string analysisPath)
    {
        string analysis = File.ReadAllText(analysisPath);
        string[] warningHeaders = analysis.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(line => line.StartsWith("### [WARN]", StringComparison.Ordinal))
            .ToArray();

        string directory = Path.GetDirectoryName(analysisPath) ?? Path.Combine(Application.persistentDataPath, "Logs");
        string path = Path.Combine(directory, $"ChapterI_WARN_Review_{report.sessionId}.md");
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("# Chapter I WARN Review");
        sb.AppendLine();
        sb.AppendLine($"- Session: `{report.sessionId}`");
        sb.AppendLine($"- Source analysis: `{Path.GetFileName(analysisPath)}`");
        sb.AppendLine($"- Warnings found: `{warningHeaders.Length}`");
        sb.AppendLine();
        if (warningHeaders.Length == 0)
        {
            sb.AppendLine("No analyzer WARN findings were found. Record this fact in `warningsDecisionNotes` before acceptance.");
        }
        else
        {
            sb.AppendLine("For every warning, either fix the issue and repeat the run, or document the explicit design reason for accepting it:");
            sb.AppendLine();
            foreach (string header in warningHeaders)
                sb.AppendLine($"- [ ] {header.Replace("### [WARN] ", "")} — decision/reason: ");
        }
        sb.AppendLine();
        sb.AppendLine("After review, copy the decisions into `Assets/Game/QA/CHAPTER_I_GAMEPLAY_ACCEPTANCE.json`, set `warningsReviewed=true`, and only then record human Story acceptance.");
        File.WriteAllText(path, sb.ToString(), new UTF8Encoding(false));
        return path;
    }

    static GameplayAcceptanceManifest LoadManifest()
    {
        if (!File.Exists(ManifestPath)) throw new FileNotFoundException("Chapter I gameplay acceptance manifest is missing.", ManifestPath);
        GameplayAcceptanceManifest manifest = JsonUtility.FromJson<GameplayAcceptanceManifest>(File.ReadAllText(ManifestPath));
        if (manifest == null) throw new InvalidDataException($"Could not parse {ManifestPath}");
        manifest.storyBaseline ??= new StoryBaselineAcceptance();
        manifest.difficultyPressure ??= new DifficultyPressureAcceptance();
        manifest.visualFit ??= new VisualFitAcceptance();
        return manifest;
    }

    static void SaveManifest(GameplayAcceptanceManifest manifest)
    {
        File.WriteAllText(ManifestPath, JsonUtility.ToJson(manifest, true) + Environment.NewLine, new UTF8Encoding(false));
        AssetDatabase.ImportAsset(ManifestPath, ImportAssetOptions.ForceUpdate);
    }

    static ChapterOnePlaythroughReporter.PlaythroughReport ReadReport(string path)
    {
        ChapterOnePlaythroughReporter.PlaythroughReport report = JsonUtility.FromJson<ChapterOnePlaythroughReporter.PlaythroughReport>(File.ReadAllText(path));
        if (report == null) throw new InvalidDataException($"Could not parse Chapter I playthrough report: {path}");
        return report;
    }

    static string FindLatestReportForDifficulty(string difficulty)
    {
        string directory = Path.Combine(Application.persistentDataPath, "Logs");
        if (!Directory.Exists(directory)) return null;
        foreach (string path in Directory.GetFiles(directory, "ChapterI_Playthrough_*.json").OrderByDescending(File.GetLastWriteTimeUtc))
        {
            try
            {
                ChapterOnePlaythroughReporter.PlaythroughReport report = ReadReport(path);
                if (string.Equals(report.difficulty, difficulty, StringComparison.OrdinalIgnoreCase)) return path;
            }
            catch (Exception)
            {
                // Ignore malformed historical reports while locating the latest valid report for this difficulty.
            }
        }
        return null;
    }

    static string ComputeSha256(string path)
    {
        using SHA256 sha = SHA256.Create();
        byte[] hash = sha.ComputeHash(File.ReadAllBytes(path));
        StringBuilder sb = new StringBuilder(hash.Length * 2);
        for (int i = 0; i < hash.Length; i++) sb.Append(hash[i].ToString("x2", CultureInfo.InvariantCulture));
        return sb.ToString();
    }
}
#endif
