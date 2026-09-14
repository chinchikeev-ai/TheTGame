#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class ChapterOneGameplayFreezeValidator
{
    const float MinChapterSeconds = 11f * 60f;
    const float MaxChapterSeconds = 13f * 60f;
    const float MaxWaveDeviationRatio = .40f;

    [MenuItem("TheTroyGame/Validation/Check Chapter I Gameplay Freeze Readiness")]
    public static void CheckLatest()
    {
        string logsDirectory = Path.Combine(Application.persistentDataPath, "Logs");
        string reportPath = FindLatestReport(logsDirectory);
        if (string.IsNullOrEmpty(reportPath))
        {
            Debug.LogError($"[CHAPTER I GAMEPLAY FREEZE] BLOCKED - no ChapterI_Playthrough_*.json found in {logsDirectory}. Complete one clean Story run at 1x first.");
            return;
        }

        Check(reportPath, true);
    }

    public static bool Check(string reportPath, bool revealOutput)
    {
        if (string.IsNullOrWhiteSpace(reportPath) || !File.Exists(reportPath))
            throw new FileNotFoundException("Chapter I playthrough report not found.", reportPath);

        string json = File.ReadAllText(reportPath);
        ChapterOnePlaythroughReporter.PlaythroughReport report =
            JsonUtility.FromJson<ChapterOnePlaythroughReporter.PlaythroughReport>(json);
        if (report == null)
            throw new InvalidDataException($"Could not parse Chapter I playthrough report: {reportPath}");

        bool hasVerifiedSpeedTelemetry =
            json.Contains("\"schemaVersion\"") &&
            json.Contains("\"nonOneXSpeedUsed\"") &&
            json.Contains("\"maxCombatSpeed\"");

        List<string> blockers = EvaluateBlockers(report, hasVerifiedSpeedTelemetry);

        // Always generate the detailed tuning report beside the freeze decision.
        string analysisPath = ChapterOnePlaythroughAnalyzer.Analyze(reportPath, false);
        string outputPath = WriteReadinessReport(reportPath, analysisPath, report, blockers);

        if (blockers.Count == 0)
        {
            Debug.Log(
                $"[CHAPTER I GAMEPLAY FREEZE] READY FOR HUMAN ACCEPTANCE - source={Path.GetFileName(reportPath)}. " +
                "Review analyzer WARN findings and the real Play Mode presentation before declaring the baseline frozen.");
        }
        else
        {
            Debug.LogError($"[CHAPTER I GAMEPLAY FREEZE] BLOCKED - {blockers.Count} blocker(s). Read {outputPath}");
            for (int i = 0; i < blockers.Count; i++) Debug.LogError($"[CHAPTER I GAMEPLAY FREEZE] {blockers[i]}");
        }

        if (revealOutput) EditorUtility.RevealInFinder(outputPath);
        return blockers.Count == 0;
    }

    static List<string> EvaluateBlockers(ChapterOnePlaythroughReporter.PlaythroughReport report, bool hasVerifiedSpeedTelemetry)
    {
        List<string> blockers = new List<string>();

        if (!hasVerifiedSpeedTelemetry)
            blockers.Add("Telemetry file does not contain the required 1x verification fields. Run Chapter I again with the current reporter.");
        if (report.schemaVersion < ChapterOnePlaythroughReporter.CurrentReportSchemaVersion)
            blockers.Add($"Telemetry schema is too old for freeze acceptance: report={report.schemaVersion}, required={ChapterOnePlaythroughReporter.CurrentReportSchemaVersion}. Run Chapter I again with the current build.");
        if (report.map != 1) blockers.Add($"Report map must be Chapter I; map={report.map}.");
        if (!string.Equals(report.difficulty, "Story", StringComparison.OrdinalIgnoreCase))
            blockers.Add($"Freeze baseline must use Story difficulty; difficulty={report.difficulty}.");
        if (report.nonOneXSpeedUsed || report.maxCombatSpeed > 1.01f)
            blockers.Add($"Freeze baseline must remain at 1x for the full run; nonOneXSpeedUsed={report.nonOneXSpeedUsed}, maxCombatSpeed={report.maxCombatSpeed:0.##}x.");
        if (!string.Equals(report.result, "VICTORY", StringComparison.OrdinalIgnoreCase))
            blockers.Add($"Run must end in VICTORY; result={report.result}.");
        if (!report.menelausDefeated) blockers.Add("Menelaus was not recorded as defeated.");
        if (report.menelausBreached) blockers.Add("Menelaus reached a completed gate-breach state.");
        if (report.actualDurationSeconds < MinChapterSeconds || report.actualDurationSeconds > MaxChapterSeconds)
            blockers.Add($"Total duration {FormatTime(report.actualDurationSeconds)} is outside 11:00-13:00.");
        if (report.gateHp <= 0) blockers.Add("Trojan gate ended at zero HP.");

        if (report.waves == null || report.waves.Count != 5)
        {
            blockers.Add($"Exactly 5 completed encounter snapshots are required; recorded={report.waves?.Count ?? 0}.");
        }
        else
        {
            HashSet<int> encounterNumbers = new HashSet<int>();
            foreach (ChapterOnePlaythroughReporter.WaveReport wave in report.waves)
            {
                encounterNumbers.Add(wave.wave);
                if (!wave.completed) blockers.Add($"Encounter {wave.wave} snapshot is incomplete.");

                float target = Mathf.Max(1f, wave.targetDurationSeconds);
                float deviation = Mathf.Abs(wave.actualDurationSeconds - target) / target;
                if (deviation >= MaxWaveDeviationRatio)
                    blockers.Add($"Encounter {wave.wave} duration deviation {deviation:P0} exceeds the 40% hard limit.");
            }

            for (int encounter = 1; encounter <= 5; encounter++)
                if (!encounterNumbers.Contains(encounter)) blockers.Add($"Encounter {encounter} snapshot is missing.");
        }

        return blockers;
    }

    static string WriteReadinessReport(
        string sourcePath,
        string analysisPath,
        ChapterOnePlaythroughReporter.PlaythroughReport report,
        List<string> blockers)
    {
        string directory = Path.GetDirectoryName(sourcePath) ?? Path.Combine(Application.persistentDataPath, "Logs");
        string stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
        string outputPath = Path.Combine(directory, $"ChapterI_GameplayFreeze_Readiness_{stamp}.md");

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("# Chapter I Gameplay Freeze Readiness");
        sb.AppendLine();
        sb.AppendLine($"- Source: `{Path.GetFileName(sourcePath)}`");
        sb.AppendLine($"- Analyzer: `{Path.GetFileName(analysisPath)}`");
        sb.AppendLine($"- Telemetry schema: `{report.schemaVersion}`");
        sb.AppendLine($"- Difficulty: `{report.difficulty}`");
        sb.AppendLine($"- Max combat speed: `{report.maxCombatSpeed:0.##}x`");
        sb.AppendLine($"- Non-1x speed used: `{report.nonOneXSpeedUsed}`");
        sb.AppendLine($"- Result: `{report.result}`");
        sb.AppendLine($"- Duration: `{FormatTime(report.actualDurationSeconds)}`");
        sb.AppendLine($"- Gate: `{report.gateHp}/{report.gateHpMax}`");
        sb.AppendLine($"- Menelaus defeated: `{report.menelausDefeated}`");
        sb.AppendLine($"- Menelaus breached: `{report.menelausBreached}`");
        sb.AppendLine($"- Recorded encounters: `{report.waves?.Count ?? 0}/5`");
        sb.AppendLine();

        if (blockers.Count == 0)
        {
            sb.AppendLine("## Verdict: READY FOR HUMAN ACCEPTANCE");
            sb.AppendLine();
            sb.AppendLine("All hard gameplay-freeze gates pass, including verified 1x telemetry. This does **not** automatically freeze the baseline.");
            sb.AppendLine("Review analyzer WARN findings and perform the required real Play Mode visual/readability inspection before recording human acceptance.");
        }
        else
        {
            sb.AppendLine("## Verdict: BLOCKED");
            sb.AppendLine();
            foreach (string blocker in blockers) sb.AppendLine($"- {blocker}");
        }

        File.WriteAllText(outputPath, sb.ToString(), new UTF8Encoding(false));
        return outputPath;
    }

    static string FindLatestReport(string directory)
    {
        if (!Directory.Exists(directory)) return null;
        return Directory.GetFiles(directory, "ChapterI_Playthrough_*.json")
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .FirstOrDefault();
    }

    static string FormatTime(float seconds)
    {
        int total = Mathf.Max(0, Mathf.RoundToInt(seconds));
        return $"{total / 60:00}:{total % 60:00}";
    }
}
#endif
