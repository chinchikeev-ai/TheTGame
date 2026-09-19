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
    const float FpsWarningThreshold = 45f;

    [Serializable]
    sealed class QaSummaryJson
    {
        public int schemaVersion = 1;
        public string verdict;
        public bool canBindAcceptance;
        public string mainReason;
        public string nextAction;
        public string sourceSessionId;
        public string sourceReportFile;
    }

    [MenuItem("TheTroyGame/Validation/Check Chapter I Gameplay Freeze Readiness")]
    public static void CheckLatest()
    {
        string logsDirectory = Path.Combine(Application.persistentDataPath, "Logs");
        string reportPath = FindLatestReport(logsDirectory);
        if (string.IsNullOrEmpty(reportPath))
        {
            Debug.LogError($"[CHAPTER I GAMEPLAY FREEZE] BLOCKED - no ChapterI_Playthrough_*.json found in {logsDirectory}. Complete one clean Story run at 1x without pausing first.");
            return;
        }

        Check(reportPath, true);
    }

    [MenuItem("TheTroyGame/Validation/Gameplay Acceptance/Generate Latest QA Summary")]
    public static void GenerateLatestQaSummary()
    {
        string logsDirectory = Path.Combine(Application.persistentDataPath, "Logs");
        string reportPath = FindLatestReport(logsDirectory);
        if (string.IsNullOrEmpty(reportPath))
        {
            Debug.LogError($"[CHAPTER I QA SUMMARY] No ChapterI_Playthrough_*.json found in {logsDirectory}.");
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

        bool hasVerifiedRunTelemetry =
            json.Contains("\"schemaVersion\"") &&
            json.Contains("\"nonOneXSpeedUsed\"") &&
            json.Contains("\"maxCombatSpeed\"") &&
            json.Contains("\"pauseUsed\"");

        List<string> blockers = EvaluateBlockers(report, hasVerifiedRunTelemetry);

        string analysisPath = ChapterOnePlaythroughAnalyzer.Analyze(reportPath, false);
        string outputPath = WriteReadinessReport(reportPath, analysisPath, report, blockers);
        string qaSummaryPath = WriteQaSummary(reportPath, report, blockers);

        if (blockers.Count == 0)
        {
            Debug.Log(
                $"[CHAPTER I GAMEPLAY FREEZE] READY FOR HUMAN ACCEPTANCE - source={Path.GetFileName(reportPath)}. " +
                $"QA summary={qaSummaryPath}. Review analyzer WARN findings and the real Play Mode presentation before declaring the baseline frozen.");
        }
        else
        {
            Debug.LogError($"[CHAPTER I GAMEPLAY FREEZE] BLOCKED - {blockers.Count} blocker(s). QA summary={qaSummaryPath}");
            for (int i = 0; i < blockers.Count; i++) Debug.LogError($"[CHAPTER I GAMEPLAY FREEZE] {blockers[i]}");
        }

        if (revealOutput) EditorUtility.RevealInFinder(qaSummaryPath);
        return blockers.Count == 0;
    }

    static List<string> EvaluateBlockers(ChapterOnePlaythroughReporter.PlaythroughReport report, bool hasVerifiedRunTelemetry)
    {
        List<string> blockers = new List<string>();

        if (!hasVerifiedRunTelemetry)
            blockers.Add("Telemetry file does not contain the required 1x/no-pause verification fields. Run Chapter I again with the current reporter.");
        if (report.schemaVersion < ChapterOnePlaythroughReporter.CurrentReportSchemaVersion)
            blockers.Add($"Telemetry schema is too old for freeze acceptance: report={report.schemaVersion}, required={ChapterOnePlaythroughReporter.CurrentReportSchemaVersion}. Run Chapter I again with the current build.");
        if (report.map != 1) blockers.Add($"Report map must be Chapter I; map={report.map}.");
        if (!string.Equals(report.difficulty, "Story", StringComparison.OrdinalIgnoreCase))
            blockers.Add($"Freeze baseline must use Story difficulty; difficulty={report.difficulty}.");
        if (report.nonOneXSpeedUsed || report.maxCombatSpeed > 1.01f)
            blockers.Add($"Freeze baseline must remain at 1x for the full run; nonOneXSpeedUsed={report.nonOneXSpeedUsed}, maxCombatSpeed={report.maxCombatSpeed:0.##}x.");
        if (report.pauseUsed)
            blockers.Add("Freeze baseline must be completed without pausing after the run starts because Chapter I duration uses unscaled wall-clock time.");
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
        sb.AppendLine($"- Pause used: `{report.pauseUsed}`");
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
            sb.AppendLine("All hard gameplay-freeze gates pass, including verified 1x/no-pause telemetry. This does **not** automatically freeze the baseline.");
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

    static string WriteQaSummary(
        string sourcePath,
        ChapterOnePlaythroughReporter.PlaythroughReport report,
        List<string> blockers)
    {
        string directory = Path.GetDirectoryName(sourcePath) ?? Path.Combine(Application.persistentDataPath, "Logs");
        string stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
        string basePath = Path.Combine(directory, $"ChapterI_QA_Summary_{stamp}");
        string markdownPath = basePath + ".md";
        string jsonPath = basePath + ".json";
        bool canBind = blockers.Count == 0;
        bool lowFps = report.averageFps > 0f && report.averageFps < FpsWarningThreshold;
        string verdict = canBind ? "READY_TO_BIND" : "BLOCKED";

        List<string> humanReasons = blockers.Select(HumanizeBlocker).ToList();
        if (humanReasons.Count == 0)
            humanReasons.Add("Жёсткие условия Story baseline выполнены: Chapter I, Story, 1x, без паузы, Victory и допустимый pacing.");
        if (lowFps)
            humanReasons.Add($"Средний FPS {report.averageFps:0.0} при {report.resolution}; это WARN ниже {FpsWarningThreshold:0} FPS и требует проверки производительности.");

        string nextAction = BuildNextAction(report, blockers, lowFps);
        string mainReason = blockers.Count > 0
            ? HumanizeBlocker(blockers[0])
            : lowFps
                ? $"Hard freeze gates pass, but average FPS is {report.averageFps:0.0} and needs performance review"
                : "Hard freeze gates pass; the Story run can be bound to gameplay acceptance";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"VERDICT: {verdict}");
        sb.AppendLine();
        sb.AppendLine("Почему:");
        foreach (string reason in humanReasons) sb.AppendLine($"- {reason}");
        sb.AppendLine();
        sb.AppendLine("Что делать:");
        foreach (string action in nextAction.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            sb.AppendLine($"- {action}");
        sb.AppendLine();
        sb.AppendLine("Данные прохода:");
        sb.AppendLine($"- Session: `{report.sessionId}`");
        sb.AppendLine($"- Source: `{Path.GetFileName(sourcePath)}`");
        sb.AppendLine($"- Difficulty: `{report.difficulty}`");
        sb.AppendLine($"- Speed max: `{report.maxCombatSpeed:0.##}x`");
        sb.AppendLine($"- Pause used: `{report.pauseUsed}`");
        sb.AppendLine($"- Result: `{report.result}`");
        sb.AppendLine($"- Duration: `{FormatTime(report.actualDurationSeconds)}` (норма 11:00-13:00)");
        sb.AppendLine($"- Average FPS: `{report.averageFps:0.0}`");
        sb.AppendLine($"- Resolution: `{report.resolution}`");

        QaSummaryJson json = new QaSummaryJson
        {
            verdict = verdict,
            canBindAcceptance = canBind,
            mainReason = mainReason,
            nextAction = nextAction.Replace("\n", " | "),
            sourceSessionId = report.sessionId,
            sourceReportFile = Path.GetFileName(sourcePath)
        };

        File.WriteAllText(markdownPath, sb.ToString(), new UTF8Encoding(false));
        File.WriteAllText(jsonPath, JsonUtility.ToJson(json, true), new UTF8Encoding(false));
        Debug.Log($"[CHAPTER I QA SUMMARY] {verdict} | md={markdownPath} | json={jsonPath}");
        return markdownPath;
    }

    static string HumanizeBlocker(string blocker)
    {
        if (blocker.StartsWith("Freeze baseline must remain at 1x", StringComparison.Ordinal))
            return "Во время прохода использовалась скорость выше 1x. Для Story baseline разрешена только 1x.";
        if (blocker.StartsWith("Freeze baseline must be completed without pausing", StringComparison.Ordinal))
            return "Во время прохода использовалась пауза. Для Story baseline нужен проход без паузы.";
        if (blocker.StartsWith("Total duration", StringComparison.Ordinal))
            return blocker.Replace("Total duration", "Длительность").Replace("is outside", "вне допустимого диапазона");
        if (blocker.StartsWith("Freeze baseline must use Story difficulty", StringComparison.Ordinal))
            return "Проход сделан не на сложности Story. Freeze baseline принимается только со Story.";
        if (blocker.StartsWith("Run must end in VICTORY", StringComparison.Ordinal))
            return "Проход не завершился Victory. Для acceptance нужен полный успешный проход.";
        if (blocker.StartsWith("Telemetry file does not contain", StringComparison.Ordinal) ||
            blocker.StartsWith("Telemetry schema is too old", StringComparison.Ordinal))
            return "Телеметрия прохода устарела или не содержит проверки 1x/паузы. Нужен новый проход на текущей версии reporter.";
        if (blocker.StartsWith("Exactly 5 completed encounter snapshots", StringComparison.Ordinal))
            return "Телеметрия записала не все 5 encounter'ов Chapter I. Такой проход нельзя принимать.";
        if (blocker.StartsWith("Encounter", StringComparison.Ordinal))
            return "Один из encounter'ов не прошёл hard pacing/telemetry проверку: " + blocker;
        if (blocker.StartsWith("Menelaus", StringComparison.Ordinal))
            return "Проверка финального боя с Менелаем не пройдена: " + blocker;
        if (blocker.StartsWith("Trojan gate ended", StringComparison.Ordinal))
            return "Ворота Трои завершили проход с 0 HP. Такой Story baseline не принимается.";
        if (blocker.StartsWith("Report map must be Chapter I", StringComparison.Ordinal))
            return "Телеметрия относится не к Chapter I.";
        return blocker;
    }

    static string BuildNextAction(
        ChapterOnePlaythroughReporter.PlaythroughReport report,
        List<string> blockers,
        bool lowFps)
    {
        if (blockers.Count == 0)
        {
            string action = "Открыть: TheTroyGame > Validation > Gameplay Acceptance > Prepare Latest Story Candidate.";
            if (lowFps) action += "\nПеред финальным freeze отдельно проверить производительность из-за FPS WARN.";
            action += "\nПосле привязки разобрать analyzer WARN и отметить их решение в acceptance.";
            return action;
        }

        List<string> actions = new List<string>();
        if (report.nonOneXSpeedUsed || report.maxCombatSpeed > 1.01f)
            actions.Add("Повторить Story строго на 1x.");
        if (report.pauseUsed)
            actions.Add("Не ставить игру на паузу после начала прохода.");
        if (!string.Equals(report.result, "VICTORY", StringComparison.OrdinalIgnoreCase))
            actions.Add("Дойти до Victory.");
        if (!string.Equals(report.difficulty, "Story", StringComparison.OrdinalIgnoreCase))
            actions.Add("Выбрать сложность Story.");
        if (report.actualDurationSeconds < MinChapterSeconds || report.actualDurationSeconds > MaxChapterSeconds)
            actions.Add("Проверить pacing/баланс: полный проход должен занимать 11:00-13:00.");
        if (actions.Count == 0)
            actions.Add("Исправить указанные hard blocker'ы и повторить чистый Story-проход на 1x без паузы до Victory.");
        actions.Add("После нового успешного прохода открыть: TheTroyGame > Validation > Gameplay Acceptance > Prepare Latest Story Candidate.");
        return string.Join("\n", actions);
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
