#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class ChapterOnePlaythroughAnalyzer
{
    const float MinChapterSeconds = 11f * 60f;
    const float MaxChapterSeconds = 13f * 60f;
    const float WaveWarningRatio = .25f;
    const float WaveFailureRatio = .40f;

    enum Severity
    {
        Pass,
        Warning,
        Failure
    }

    sealed class Finding
    {
        public Severity severity;
        public string area;
        public string message;
        public string action;
    }

    [MenuItem("TheTroyGame/Validation/Analyze Latest Chapter I Playthrough")]
    public static void AnalyzeLatest()
    {
        string logsDirectory = Path.Combine(Application.persistentDataPath, "Logs");
        string reportPath = FindLatestReport(logsDirectory);
        if (string.IsNullOrEmpty(reportPath))
        {
            Debug.LogWarning($"[CHAPTER I RC ANALYZER] No ChapterI_Playthrough_*.json found in {logsDirectory}. Complete one Chapter I run at 1x first.");
            return;
        }

        Analyze(reportPath, true);
    }

    [MenuItem("TheTroyGame/Validation/Open Chapter I Playthrough Logs")]
    public static void OpenLogsFolder()
    {
        string logsDirectory = Path.Combine(Application.persistentDataPath, "Logs");
        Directory.CreateDirectory(logsDirectory);
        EditorUtility.RevealInFinder(logsDirectory);
    }

    public static string Analyze(string reportPath, bool revealOutput)
    {
        if (string.IsNullOrWhiteSpace(reportPath) || !File.Exists(reportPath))
            throw new FileNotFoundException("Chapter I playthrough report not found.", reportPath);

        ChapterOnePlaythroughReporter.PlaythroughReport report = JsonUtility.FromJson<ChapterOnePlaythroughReporter.PlaythroughReport>(File.ReadAllText(reportPath));
        if (report == null)
            throw new InvalidDataException($"Could not parse Chapter I playthrough report: {reportPath}");

        List<Finding> findings = Evaluate(report);
        string output = BuildMarkdown(reportPath, report, findings);

        string directory = Path.GetDirectoryName(reportPath) ?? Path.Combine(Application.persistentDataPath, "Logs");
        string stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
        string outputPath = Path.Combine(directory, $"ChapterI_RC_Analysis_{stamp}.md");
        File.WriteAllText(outputPath, output, new UTF8Encoding(false));

        int failures = findings.Count(f => f.severity == Severity.Failure);
        int warnings = findings.Count(f => f.severity == Severity.Warning);
        string verdict = failures > 0 ? "FAIL" : warnings > 0 ? "WARN" : "PASS";
        string summary = $"[CHAPTER I RC ANALYZER] {verdict} | failures={failures}, warnings={warnings} | source={Path.GetFileName(reportPath)} | analysis={outputPath}";

        if (failures > 0) Debug.LogError(summary);
        else if (warnings > 0) Debug.LogWarning(summary);
        else Debug.Log(summary);

        foreach (Finding finding in findings.Where(f => f.severity != Severity.Pass))
        {
            string line = $"[CHAPTER I RC][{finding.severity.ToString().ToUpperInvariant()}][{finding.area}] {finding.message} ACTION: {finding.action}";
            if (finding.severity == Severity.Failure) Debug.LogError(line);
            else Debug.LogWarning(line);
        }

        if (revealOutput) EditorUtility.RevealInFinder(outputPath);
        return outputPath;
    }

    static List<Finding> Evaluate(ChapterOnePlaythroughReporter.PlaythroughReport report)
    {
        List<Finding> findings = new List<Finding>();

        bool victory = string.Equals(report.result, "VICTORY", StringComparison.OrdinalIgnoreCase);
        Add(findings, victory ? Severity.Pass : Severity.Failure, "RESULT",
            victory ? "Chapter I ended in victory." : $"Chapter I result was '{report.result}'.",
            victory ? "No action required." : "Do not tune the baseline from a failed run. Review the losing wave, gate damage and economy, then repeat a clean 1x run.");

        bool bossOk = report.menelausDefeated && !report.menelausBreached;
        Add(findings, bossOk ? Severity.Pass : Severity.Failure, "MENELAUS",
            bossOk ? "Menelaus was defeated before a gate breach." : $"menelausDefeated={report.menelausDefeated}, menelausBreached={report.menelausBreached}.",
            bossOk ? "No action required." : "Inspect Wave 5 first. If pressure is excessive, reduce reinforcement pressure before lowering the boss identity; if trivial, increase boss/reinforcement pressure rather than stretching earlier waves.");

        float duration = report.actualDurationSeconds;
        Severity pacingSeverity = duration < MinChapterSeconds || duration > MaxChapterSeconds ? Severity.Failure : Severity.Pass;
        Add(findings, pacingSeverity, "PACING",
            $"Total run time {FormatTime(duration)}; target is 11:00-13:00. Reporter verdict={report.pacingVerdict}.",
            duration < MinChapterSeconds
                ? "Increase meaningful combat duration in the fastest waves; prefer enemy pressure/spawn cadence tuning over idle preparation time."
                : duration > MaxChapterSeconds
                    ? "Shorten the slowest waves; reduce HP congestion or tighten spawn cadence before cutting tactical preparation."
                    : "Total pacing is inside the Chapter I contract.");

        if (report.waves == null || report.waves.Count < 5)
        {
            Add(findings, Severity.Failure, "WAVES", $"Only {report.waves?.Count ?? 0}/5 wave snapshots were recorded.",
                "Repeat a complete Chapter I 1x run and verify the reporter remains active through the result screen.");
        }
        else
        {
            foreach (ChapterOnePlaythroughReporter.WaveReport wave in report.waves.OrderBy(w => w.wave))
                EvaluateWave(wave, findings);
        }

        int startMoney = report.waves != null && report.waves.Count > 0 ? report.waves.OrderBy(w => w.wave).First().moneyStart : 0;
        int totalAvailable = Mathf.Max(1, startMoney + report.goldEarned);
        float spendRatio = Mathf.Clamp01(report.goldSpent / (float)totalAvailable);
        Severity economySeverity = spendRatio < .45f || spendRatio > .92f ? Severity.Warning : Severity.Pass;
        string economyAction = spendRatio < .45f
            ? "Player is retaining too much purchasing power. Check tower prices/rewards and whether useful upgrade choices arrive early enough."
            : spendRatio > .92f
                ? "Economy is close to starvation. Check whether the player can recover from one imperfect purchase before reducing rewards."
                : "Economy utilization is in a healthy first-pass band.";
        Add(findings, economySeverity, "ECONOMY",
            $"Starting money≈{startMoney}, earned={report.goldEarned}, spent={report.goldSpent}, final={report.finalMoney}, spend ratio={spendRatio:P0}.", economyAction);

        float gateRatio = report.gateHpMax > 0 ? report.gateHp / (float)report.gateHpMax : 0f;
        Severity gateSeverity = report.gateHp <= 0 ? Severity.Failure : gateRatio < .25f || (gateRatio > .90f && report.leaks == 0) ? Severity.Warning : Severity.Pass;
        string gateAction = report.gateHp <= 0
            ? "Baseline failed: identify the first wave that causes irreversible gate loss before changing global difficulty."
            : gateRatio < .25f
                ? "Victory is too close to collapse for a Story baseline. Reduce the dominant leak source or smooth the wave causing the largest gate-HP delta."
                : gateRatio > .90f && report.leaks == 0
                    ? "Defense may be too forgiving. Increase tactical pressure in the weakest-pressure wave rather than globally inflating HP."
                    : "Gate survival is in a useful tuning band.";
        Add(findings, gateSeverity, "GATE",
            $"Gate ended at {report.gateHp}/{report.gateHpMax} ({gateRatio:P0}); leaks={report.leaks}.", gateAction);

        if (report.leaks > 0)
        {
            float leakRate = report.kills + report.leaks > 0 ? report.leaks / (float)(report.kills + report.leaks) : 0f;
            Severity leakSeverity = leakRate > .12f ? Severity.Warning : Severity.Pass;
            Add(findings, leakSeverity, "LEAKS", $"Leaks={report.leaks}; observed kill+leak share={leakRate:P0}.",
                leakSeverity == Severity.Warning ? "Find the wave with the largest leak count and tune that archetype/cadence locally." : "Leak pressure is present without dominating the run.");
        }

        Severity fpsSeverity = report.averageFps > 0f && report.averageFps < 45f ? Severity.Warning : Severity.Pass;
        Add(findings, fpsSeverity, "PERFORMANCE", $"Average observed FPS={report.averageFps:0.0} at {report.resolution}.",
            fpsSeverity == Severity.Warning ? "Profile the wave with the highest max-alive count before adding more presentation objects or campaign-scale content." : "No first-pass average-FPS warning.");

        if (!string.Equals(report.difficulty, "Story", StringComparison.OrdinalIgnoreCase))
            Add(findings, Severity.Warning, "BASELINE", $"Run difficulty was {report.difficulty}, not Story.", "Freeze the Chapter I baseline from a Story 1x run first; then validate Strategos and Legendary as separate pressure passes.");

        return findings;
    }

    static void EvaluateWave(ChapterOnePlaythroughReporter.WaveReport wave, List<Finding> findings)
    {
        if (!wave.completed)
        {
            Add(findings, Severity.Failure, $"WAVE {wave.wave}", "Wave snapshot was not marked complete.", "Do not tune from this snapshot; repeat a complete run or inspect why the wave/run terminated early.");
            return;
        }

        float target = Mathf.Max(1f, wave.targetDurationSeconds);
        float ratio = (wave.actualDurationSeconds - target) / target;
        float absRatio = Mathf.Abs(ratio);
        Severity durationSeverity = absRatio >= WaveFailureRatio ? Severity.Failure : absRatio >= WaveWarningRatio ? Severity.Warning : Severity.Pass;
        string durationAction = ratio > 0f
            ? "Wave is running long: inspect HP congestion/max-alive first, then spawn interval. Avoid padding/cutting prep time to hide combat pacing."
            : "Wave is resolving early: increase meaningful pressure via composition/cadence before simply adding enemy HP.";
        Add(findings, durationSeverity, $"WAVE {wave.wave} PACING",
            $"Actual={wave.actualDurationSeconds:0.0}s, target={wave.targetDurationSeconds:0.0}s, delta={wave.durationDeltaSeconds:+0.0;-0.0;0.0}s ({ratio:+0%;-0%;0%}).",
            durationSeverity == Severity.Pass ? "Wave duration is inside the first-pass tolerance." : durationAction);

        float pressureRatio = wave.preparedEnemies > 0 ? wave.maxAliveEnemies / (float)wave.preparedEnemies : 0f;
        Severity pressureSeverity = Severity.Pass;
        string pressureAction = "Pressure is in a usable first-pass band.";
        if (wave.preparedEnemies >= 8 && pressureRatio < .20f)
        {
            pressureSeverity = Severity.Warning;
            pressureAction = "Pressure is too serialized. Tighten spawn cadence or mix faster archetypes before increasing raw HP.";
        }
        else if (wave.maxAliveEnemies >= 8 && pressureRatio > .72f)
        {
            pressureSeverity = Severity.Warning;
            pressureAction = "Large backlog forms. Reduce simultaneous congestion or HP before making the wave longer.";
        }
        Add(findings, pressureSeverity, $"WAVE {wave.wave} PRESSURE",
            $"Prepared={wave.preparedEnemies}, peak alive={wave.maxAliveEnemies} ({pressureRatio:P0}), kills={wave.kills}, leaks={wave.leaks}.", pressureAction);

        int gateLoss = Mathf.Max(0, wave.gateHpStart - wave.gateHpEnd);
        if (gateLoss > 0 || wave.leaks > 0)
        {
            Severity defenseSeverity = wave.gateHpEnd <= 0 ? Severity.Failure : Severity.Warning;
            Add(findings, defenseSeverity, $"WAVE {wave.wave} DEFENSE",
                $"Gate {wave.gateHpStart}->{wave.gateHpEnd}, loss={gateLoss}; leaks={wave.leaks}.",
                "Correlate this wave's leak archetypes with tower availability. Prefer local composition/reward tuning over global gate-HP inflation.");
        }

        int waveAvailable = Mathf.Max(1, wave.moneyStart + wave.goldEarned);
        float waveSpendRatio = wave.goldSpent / (float)waveAvailable;
        if (waveSpendRatio < .15f && wave.moneyEnd > wave.moneyStart && wave.wave >= 2)
            Add(findings, Severity.Warning, $"WAVE {wave.wave} ECONOMY",
                $"Money {wave.moneyStart}->{wave.moneyEnd}, earned={wave.goldEarned}, spent={wave.goldSpent}.",
                "Player accumulates resources during this wave. Check whether upgrades/new counters are compelling enough at this point.");
    }

    static string BuildMarkdown(string reportPath, ChapterOnePlaythroughReporter.PlaythroughReport report, List<Finding> findings)
    {
        int failures = findings.Count(f => f.severity == Severity.Failure);
        int warnings = findings.Count(f => f.severity == Severity.Warning);
        string verdict = failures > 0 ? "FAIL" : warnings > 0 ? "WARN" : "PASS";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("# Chapter I Gameplay RC Analysis");
        sb.AppendLine();
        sb.AppendLine($"- Verdict: **{verdict}**");
        sb.AppendLine($"- Source: `{Path.GetFileName(reportPath)}`");
        sb.AppendLine($"- Session: `{report.sessionId}`");
        sb.AppendLine($"- Difficulty: **{report.difficulty}**");
        sb.AppendLine($"- Resolution: **{report.resolution}**");
        sb.AppendLine($"- Result: **{report.result}**");
        sb.AppendLine($"- Duration: **{FormatTime(report.actualDurationSeconds)}** (target 11:00-13:00)");
        sb.AppendLine($"- Score: **{report.finalScore}**");
        sb.AppendLine($"- Gate: **{report.gateHp}/{report.gateHpMax}**");
        sb.AppendLine($"- Kills / leaks: **{report.kills} / {report.leaks}**");
        sb.AppendLine($"- Gold earned / spent / final: **{report.goldEarned} / {report.goldSpent} / {report.finalMoney}**");
        sb.AppendLine($"- Menelaus defeated / breached: **{report.menelausDefeated} / {report.menelausBreached}**");
        sb.AppendLine($"- Average FPS: **{report.averageFps:0.0}**");
        sb.AppendLine();
        sb.AppendLine($"## Findings ({failures} failures, {warnings} warnings)");
        sb.AppendLine();

        foreach (Finding finding in findings)
        {
            string icon = finding.severity == Severity.Failure ? "FAIL" : finding.severity == Severity.Warning ? "WARN" : "PASS";
            sb.AppendLine($"### [{icon}] {finding.area}");
            sb.AppendLine(finding.message);
            sb.AppendLine();
            sb.AppendLine($"**Action:** {finding.action}");
            sb.AppendLine();
        }

        sb.AppendLine("## Wave table");
        sb.AppendLine();
        sb.AppendLine("| Wave | Target | Actual | Delta | Peak alive | Kills | Leaks | Gold + | Gold - | Money | Gate |");
        sb.AppendLine("|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|");
        if (report.waves != null)
        {
            foreach (ChapterOnePlaythroughReporter.WaveReport wave in report.waves.OrderBy(w => w.wave))
            {
                sb.AppendLine($"| {wave.wave} | {wave.targetDurationSeconds:0}s | {wave.actualDurationSeconds:0}s | {wave.durationDeltaSeconds:+0;-0;0}s | {wave.maxAliveEnemies} | {wave.kills} | {wave.leaks} | {wave.goldEarned} | {wave.goldSpent} | {wave.moneyStart}->{wave.moneyEnd} | {wave.gateHpStart}->{wave.gateHpEnd} |");
            }
        }

        sb.AppendLine();
        sb.AppendLine("## RC decision rule");
        sb.AppendLine();
        sb.AppendLine("Freeze Story balance only after a clean 1x victory has no FAIL findings, total pacing is 11-13 minutes, and remaining WARN findings are explicitly accepted or tuned. Then repeat pressure validation on Strategos and Legendary.");
        return sb.ToString();
    }

    static string FindLatestReport(string directory)
    {
        if (!Directory.Exists(directory)) return null;
        return Directory.GetFiles(directory, "ChapterI_Playthrough_*.json", SearchOption.TopDirectoryOnly)
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .FirstOrDefault();
    }

    static void Add(List<Finding> findings, Severity severity, string area, string message, string action)
    {
        findings.Add(new Finding { severity = severity, area = area, message = message, action = action });
    }

    static string FormatTime(float seconds)
    {
        int rounded = Mathf.Max(0, Mathf.RoundToInt(seconds));
        return $"{rounded / 60:00}:{rounded % 60:00}";
    }
}
#endif
