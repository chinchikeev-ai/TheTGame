#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class ChapterOneDifficultyPressureAnalyzer
{
    sealed class RunMetrics
    {
        public string path;
        public ChapterOnePlaythroughReporter.PlaythroughReport report;
        public int preparedEnemies;
        public float averagePeakRatio;
        public float leakRate;
        public float gateLossRatio;
        public float durationRatio;
        public float pressureScore;
        public List<string> blockers = new List<string>();
    }

    [MenuItem("TheTroyGame/Validation/Gameplay Acceptance/Analyze Story-Strategos-Legendary Pressure")]
    public static void AnalyzeLatestDifficultySet()
    {
        string logsDirectory = Path.Combine(Application.persistentDataPath, "Logs");
        Directory.CreateDirectory(logsDirectory);

        RunMetrics story = LoadLatest(logsDirectory, "Story");
        RunMetrics strategos = LoadLatest(logsDirectory, "Strategos");
        RunMetrics legendary = LoadLatest(logsDirectory, "Legendary");

        List<string> findings = new List<string>();
        CollectEligibility("Story", story, findings);
        CollectEligibility("Strategos", strategos, findings);
        CollectEligibility("Legendary", legendary, findings);

        if (story != null && strategos != null && legendary != null)
        {
            if (story.preparedEnemies > strategos.preparedEnemies)
                findings.Add($"Prepared enemy count is not monotonic Story->Strategos: {story.preparedEnemies}>{strategos.preparedEnemies}.");
            if (strategos.preparedEnemies > legendary.preparedEnemies)
                findings.Add($"Prepared enemy count is not monotonic Strategos->Legendary: {strategos.preparedEnemies}>{legendary.preparedEnemies}.");

            const float ObservedTolerance = 10f;
            if (strategos.pressureScore + ObservedTolerance < story.pressureScore)
                findings.Add($"Observed Strategos pressure ({strategos.pressureScore:0.0}) is materially below Story ({story.pressureScore:0.0}). Review player behavior and tuning before acceptance.");
            if (legendary.pressureScore + ObservedTolerance < strategos.pressureScore)
                findings.Add($"Observed Legendary pressure ({legendary.pressureScore:0.0}) is materially below Strategos ({strategos.pressureScore:0.0}). Review player behavior and tuning before acceptance.");
        }

        string outputPath = WriteReport(logsDirectory, story, strategos, legendary, findings);
        bool eligibleSet = story != null && strategos != null && legendary != null &&
                           story.blockers.Count == 0 && strategos.blockers.Count == 0 && legendary.blockers.Count == 0;

        if (!eligibleSet)
            Debug.LogError($"[CHAPTER I DIFFICULTY] BLOCKED - one or more difficulty runs are missing/ineligible. Report={outputPath}");
        else if (findings.Count > 0)
            Debug.LogWarning($"[CHAPTER I DIFFICULTY] REVIEW REQUIRED - {findings.Count} pressure finding(s). Report={outputPath}");
        else
            Debug.Log($"[CHAPTER I DIFFICULTY] READY FOR HUMAN REVIEW - three eligible runs found and no automatic pressure inversion detected. Report={outputPath}");

        EditorUtility.RevealInFinder(outputPath);
    }

    static RunMetrics LoadLatest(string directory, string difficulty)
    {
        foreach (string path in Directory.GetFiles(directory, "ChapterI_Playthrough_*.json").OrderByDescending(File.GetLastWriteTimeUtc))
        {
            try
            {
                ChapterOnePlaythroughReporter.PlaythroughReport report =
                    JsonUtility.FromJson<ChapterOnePlaythroughReporter.PlaythroughReport>(File.ReadAllText(path));
                if (report == null || !string.Equals(report.difficulty, difficulty, StringComparison.OrdinalIgnoreCase)) continue;
                return BuildMetrics(path, report);
            }
            catch (Exception)
            {
                // Historical malformed reports are ignored while locating the latest usable report for the requested difficulty.
            }
        }
        return null;
    }

    static RunMetrics BuildMetrics(string path, ChapterOnePlaythroughReporter.PlaythroughReport report)
    {
        RunMetrics metrics = new RunMetrics { path = path, report = report };

        if (report.schemaVersion < ChapterOnePlaythroughReporter.CurrentReportSchemaVersion)
            metrics.blockers.Add($"schemaVersion={report.schemaVersion}, required={ChapterOnePlaythroughReporter.CurrentReportSchemaVersion}");
        if (report.map != 1) metrics.blockers.Add($"map={report.map}, expected Chapter I");
        if (report.nonOneXSpeedUsed || report.maxCombatSpeed > 1.01f) metrics.blockers.Add("run was not kept at 1x");
        if (report.pauseUsed) metrics.blockers.Add("run used pause");
        if (!string.Equals(report.result, "VICTORY", StringComparison.OrdinalIgnoreCase)) metrics.blockers.Add($"result={report.result}, expected VICTORY");
        if (!report.menelausDefeated || report.menelausBreached) metrics.blockers.Add("Menelaus outcome is not a clean defeat-before-breach");
        if (report.waves == null || report.waves.Count != 5) metrics.blockers.Add($"encounter snapshots={report.waves?.Count ?? 0}, expected 5");
        else if (report.waves.Any(w => !w.completed)) metrics.blockers.Add("one or more encounter snapshots are incomplete");

        if (report.waves != null && report.waves.Count > 0)
        {
            metrics.preparedEnemies = report.waves.Sum(w => Mathf.Max(0, w.preparedEnemies));
            metrics.averagePeakRatio = report.waves.Average(w => w.preparedEnemies > 0 ? Mathf.Clamp01(w.maxAliveEnemies / (float)w.preparedEnemies) : 0f);
        }

        int observedEnemies = Mathf.Max(1, report.kills + report.leaks);
        metrics.leakRate = Mathf.Clamp01(report.leaks / (float)observedEnemies);
        metrics.gateLossRatio = report.gateHpMax > 0 ? Mathf.Clamp01((report.gateHpMax - report.gateHp) / (float)report.gateHpMax) : 1f;
        float targetSeconds = Mathf.Max(1f, report.targetDurationMinutes * 60f);
        metrics.durationRatio = report.actualDurationSeconds / targetSeconds;

        float congestion = metrics.averagePeakRatio;
        float overtimePressure = Mathf.Clamp01((metrics.durationRatio - .85f) / .45f);
        metrics.pressureScore = 100f * (
            metrics.gateLossRatio * .35f +
            metrics.leakRate * .30f +
            congestion * .25f +
            overtimePressure * .10f);

        return metrics;
    }

    static void CollectEligibility(string label, RunMetrics metrics, List<string> findings)
    {
        if (metrics == null)
        {
            findings.Add($"Missing {label} telemetry report.");
            return;
        }
        foreach (string blocker in metrics.blockers)
            findings.Add($"{label} run ineligible: {blocker}.");
    }

    static string WriteReport(string directory, RunMetrics story, RunMetrics strategos, RunMetrics legendary, List<string> findings)
    {
        string stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
        string outputPath = Path.Combine(directory, $"ChapterI_DifficultyPressure_{stamp}.md");
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("# Chapter I Difficulty Pressure Review");
        sb.AppendLine();
        sb.AppendLine("This report compares the latest eligible-looking 1x/no-pause Story, Strategos and Legendary runs. The observed pressure score is diagnostic only; human review remains authoritative because player decisions differ between runs.");
        sb.AppendLine();
        sb.AppendLine("## Authored difficulty curve");
        sb.AppendLine();
        sb.AppendLine("| Difficulty | Start gold | Gate HP | Enemy HP | Enemy speed | Enemy count | Rewards |");
        sb.AppendLine("|---|---:|---:|---:|---:|---:|---:|");
        AppendAuthoredRow(sb, CampaignDifficulty.Story);
        AppendAuthoredRow(sb, CampaignDifficulty.Strategos);
        AppendAuthoredRow(sb, CampaignDifficulty.Legendary);
        sb.AppendLine();
        sb.AppendLine("## Latest telemetry");
        sb.AppendLine();
        sb.AppendLine("| Difficulty | Result | Time | Prepared | Peak ratio | Leaks | Gate | Pressure | FPS | Eligibility |");
        sb.AppendLine("|---|---|---:|---:|---:|---:|---:|---:|---:|---|");
        AppendRunRow(sb, "Story", story);
        AppendRunRow(sb, "Strategos", strategos);
        AppendRunRow(sb, "Legendary", legendary);
        sb.AppendLine();
        sb.AppendLine("## Automatic findings");
        sb.AppendLine();
        if (findings.Count == 0) sb.AppendLine("- No automatic blockers or material pressure inversions detected.");
        else foreach (string finding in findings) sb.AppendLine($"- {finding}");
        sb.AppendLine();
        sb.AppendLine("## Human acceptance");
        sb.AppendLine();
        sb.AppendLine("After reviewing actual play feel, encounter-local spikes, economy recovery and Menelaus pressure, record the decision in `Assets/Game/QA/CHAPTER_I_GAMEPLAY_ACCEPTANCE.json` under `difficultyPressure`. Do not mark Strategos/Legendary reviewed merely because the automatic score is monotonic.");
        File.WriteAllText(outputPath, sb.ToString(), new UTF8Encoding(false));
        return outputPath;
    }

    static void AppendAuthoredRow(StringBuilder sb, CampaignDifficulty difficulty)
    {
        sb.AppendLine($"| {difficulty} | {DifficultyRules.StartingGold(difficulty)} | {DifficultyRules.StartingGateHealth(difficulty)} | {DifficultyRules.EnemyHpMultiplier(difficulty):0.00}x | {DifficultyRules.EnemySpeedMultiplier(difficulty):0.00}x | {DifficultyRules.EnemyCountMultiplier(difficulty):0.00}x | {DifficultyRules.RewardMultiplier(difficulty):0.00}x |");
    }

    static void AppendRunRow(StringBuilder sb, string label, RunMetrics metrics)
    {
        if (metrics == null)
        {
            sb.AppendLine($"| {label} | MISSING | - | - | - | - | - | - | - | BLOCKED |");
            return;
        }

        ChapterOnePlaythroughReporter.PlaythroughReport r = metrics.report;
        string eligibility = metrics.blockers.Count == 0 ? "eligible" : string.Join("; ", metrics.blockers);
        sb.AppendLine($"| {label} | {r.result} | {FormatTime(r.actualDurationSeconds)} | {metrics.preparedEnemies} | {metrics.averagePeakRatio:P0} | {r.leaks} ({metrics.leakRate:P0}) | {r.gateHp}/{r.gateHpMax} | {metrics.pressureScore:0.0} | {r.averageFps:0.0} | {eligibility} |");
        sb.AppendLine($"<!-- {label} source: {Path.GetFileName(metrics.path)} session={r.sessionId} -->");
    }

    static string FormatTime(float seconds)
    {
        int total = Mathf.Max(0, Mathf.RoundToInt(seconds));
        return $"{total / 60:00}:{total % 60:00}";
    }
}
#endif
