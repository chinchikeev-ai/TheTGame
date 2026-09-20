using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public sealed class ChapterOnePlaythroughReporter : MonoBehaviour
{
    public const int CurrentReportSchemaVersion = 3;

    [Serializable]
    public sealed class WaveReport
    {
        public int wave;
        public int preparedEnemies;
        public float targetDurationSeconds;
        public float actualDurationSeconds;
        public float durationDeltaSeconds;
        public int kills;
        public int leaks;
        public int goldEarned;
        public int goldSpent;
        public int moneyStart;
        public int moneyEnd;
        public int gateHpStart;
        public int gateHpEnd;
        public int maxAliveEnemies;
        public bool completed;
    }

    [Serializable]
    public sealed class PlaythroughReport
    {
        public int schemaVersion = CurrentReportSchemaVersion;
        public string sessionId;
        public string createdUtc;
        public string result;
        public string difficulty;
        public string language;
        public string resolution;
        public int map;
        public float targetDurationMinutes;
        public float actualDurationSeconds;
        public string pacingVerdict;
        public bool nonOneXSpeedUsed;
        public float maxCombatSpeed = 1f;
        public bool pauseUsed;
        public int finalScore;
        public int kills;
        public int leaks;
        public int goldEarned;
        public int goldSpent;
        public int finalMoney;
        public int towersBuilt;
        public int towersSold;
        public int gateHp;
        public int gateHpMax;
        public bool menelausDefeated;
        public bool menelausBreached;
        public float averageFps;
        public List<WaveReport> waves = new List<WaveReport>();
    }

    public static string LastReportPath { get; private set; }
    public static string LastCsvPath { get; private set; }
    public static string LastSummaryPath { get; private set; }

    GameManager game;
    EnemySpawner spawner;
    readonly PlaythroughReport report = new PlaythroughReport();

    int activeWave;
    float waveStartUnscaled;
    int startPreparedEnemies;
    float startTargetDuration;
    int startKills;
    int startLeaks;
    int startGoldEarned;
    int startGoldSpent;
    int startMoney;
    int startGateHp;
    int maxAlive;
    int frameCount;
    float observedSeconds;
    float nextCheckpointAt = 30f;
    bool reportWritten;

    void Start()
    {
        game = GameManager.Instance;
        spawner = EnemySpawner.Instance;
        report.schemaVersion = CurrentReportSchemaVersion;
        report.sessionId = Guid.NewGuid().ToString("N");
        report.createdUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
        report.language = GameLanguage.Code;
        report.resolution = $"{Screen.width}x{Screen.height}";
        report.difficulty = CampaignSave.Difficulty.ToString();
        report.maxCombatSpeed = CombatControlsUI.CurrentSpeed;
        RuntimeFileLogger.Event("RC_REPORT", $"Chapter I playthrough reporter armed. session={report.sessionId}, schema={report.schemaVersion}");
    }

    void Update()
    {
        if (game == null) game = GameManager.Instance;
        if (spawner == null) spawner = EnemySpawner.Instance;
        if (game == null || spawner == null || reportWritten) return;

        if (game.RunTime > 0f)
        {
            float combatSpeed = CombatControlsUI.CurrentSpeed;
            report.maxCombatSpeed = Mathf.Max(report.maxCombatSpeed, combatSpeed);
            if (Mathf.Abs(combatSpeed - 1f) > .01f) report.nonOneXSpeedUsed = true;
            if (!game.GameEnded && Time.timeScale <= .001f) report.pauseUsed = true;

            frameCount++;
            observedSeconds += Time.unscaledDeltaTime;

            if (game.RunTime >= nextCheckpointAt)
            {
                WriteCheckpoint();
                nextCheckpointAt += 30f;
            }
        }

        if (spawner.WaveActive)
        {
            if (activeWave != spawner.CurrentWave)
            {
                if (activeWave > 0) FinishWave(false);
                BeginWave(spawner.CurrentWave);
            }
            maxAlive = Mathf.Max(maxAlive, EnemyRegistry.AliveCount);
        }
        else if (activeWave > 0)
        {
            FinishWave(true);
        }

        if (game.GameEnded)
        {
            if (activeWave > 0) FinishWave(false);
            WriteReport();
        }
    }

    void WriteCheckpoint()
    {
        RuntimeFileLogger.Event(
            "RC_CHECKPOINT",
            $"t={game.RunTime:0.0}s, wave={game.CurrentWave}/{game.MaxWaves}, waveActive={spawner.WaveActive}, alive={EnemyRegistry.AliveCount}, gold={game.Money}, earned={game.GoldEarned}, spent={game.GoldSpent}, gateHP={game.BaseHealth}/{game.MaxBaseHealth}, kills={game.Kills}, leaks={game.Leaks}, built={game.TowersBuilt}, sold={game.TowersSold}, bossDefeated={game.BossDefeated}, bossBreached={game.BossBreached}, combatSpeed={CombatControlsUI.CurrentSpeed:0.##}x, pauseUsed={report.pauseUsed}, pacing={game.PacingVerdict()}");
    }

    void BeginWave(int wave)
    {
        activeWave = wave;
        waveStartUnscaled = Time.unscaledTime;
        startPreparedEnemies = spawner.NextWaveEnemyCount;
        startTargetDuration = spawner.TargetWaveDuration;
        startKills = game.Kills;
        startLeaks = game.Leaks;
        startGoldEarned = game.GoldEarned;
        startGoldSpent = game.GoldSpent;
        startMoney = game.Money;
        startGateHp = game.BaseHealth;
        maxAlive = EnemyRegistry.AliveCount;
        RuntimeFileLogger.Event("RC_REPORT", $"Encounter snapshot started encounter={wave}, gold={startMoney}, gateHP={startGateHp}, preparedEnemies={startPreparedEnemies}, target={startTargetDuration:0.0}s");
    }

    void FinishWave(bool completed)
    {
        if (activeWave <= 0) return;

        float actual = Mathf.Max(0f, Time.unscaledTime - waveStartUnscaled);
        float target = startTargetDuration;
        WaveReport wave = new WaveReport
        {
            wave = activeWave,
            preparedEnemies = startPreparedEnemies,
            targetDurationSeconds = target,
            actualDurationSeconds = actual,
            durationDeltaSeconds = actual - target,
            kills = game.Kills - startKills,
            leaks = game.Leaks - startLeaks,
            goldEarned = game.GoldEarned - startGoldEarned,
            goldSpent = game.GoldSpent - startGoldSpent,
            moneyStart = startMoney,
            moneyEnd = game.Money,
            gateHpStart = startGateHp,
            gateHpEnd = game.BaseHealth,
            maxAliveEnemies = maxAlive,
            completed = completed
        };
        report.waves.Add(wave);
        RuntimeFileLogger.Event("RC_REPORT", $"Encounter snapshot encounter={wave.wave}, actual={wave.actualDurationSeconds:0.0}s, target={wave.targetDurationSeconds:0.0}s, kills={wave.kills}, leaks={wave.leaks}, goldEarned={wave.goldEarned}, goldSpent={wave.goldSpent}, money={wave.moneyStart}->{wave.moneyEnd}, gateHP={wave.gateHpStart}->{wave.gateHpEnd}, maxAlive={wave.maxAliveEnemies}, completed={wave.completed}");
        activeWave = 0;
        startPreparedEnemies = 0;
        startTargetDuration = 0f;
    }

    void WriteReport()
    {
        if (reportWritten) return;
        reportWritten = true;

        report.result = game.EndMessage;
        report.map = game.MapNumber;
        report.targetDurationMinutes = game.Chapter != null ? game.Chapter.targetDurationMinutes : 12f;
        report.actualDurationSeconds = game.RunTime;
        report.pacingVerdict = game.PacingVerdict();
        report.finalScore = game.FinalScore;
        report.kills = game.Kills;
        report.leaks = game.Leaks;
        report.goldEarned = game.GoldEarned;
        report.goldSpent = game.GoldSpent;
        report.finalMoney = game.Money;
        report.towersBuilt = game.TowersBuilt;
        report.towersSold = game.TowersSold;
        report.gateHp = game.BaseHealth;
        report.gateHpMax = game.MaxBaseHealth;
        report.menelausDefeated = game.BossDefeated;
        report.menelausBreached = game.BossBreached;
        report.averageFps = observedSeconds > .01f ? frameCount / observedSeconds : 0f;

        try
        {
            string directory = Path.Combine(Application.persistentDataPath, "Logs");
            Directory.CreateDirectory(directory);
            string stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
            LastReportPath = Path.Combine(directory, $"ChapterI_Playthrough_{stamp}.json");
            LastCsvPath = Path.Combine(directory, $"ChapterI_Waves_{stamp}.csv");
            LastSummaryPath = Path.Combine(directory, $"ChapterI_QA_Summary_{stamp}.md");

            File.WriteAllText(LastReportPath, JsonUtility.ToJson(report, true), new UTF8Encoding(false));
            File.WriteAllText(LastCsvPath, BuildCsv(), new UTF8Encoding(false));
            File.WriteAllText(LastSummaryPath, BuildQaSummary(), new UTF8Encoding(false));

            RuntimeFileLogger.Event("RC_REPORT", $"Saved JSON={LastReportPath}");
            RuntimeFileLogger.Event("RC_REPORT", $"Saved CSV={LastCsvPath}");
            RuntimeFileLogger.Event("RC_REPORT", $"Saved QA summary={LastSummaryPath}");
            RuntimeFileLogger.Event("RC_REPORT", $"Summary result={report.result}, time={report.actualDurationSeconds:0.0}s, pacing={report.pacingVerdict}, speedMax={report.maxCombatSpeed:0.##}x, non1x={report.nonOneXSpeedUsed}, pauseUsed={report.pauseUsed}, score={report.finalScore}, kills={report.kills}, leaks={report.leaks}, goldEarned={report.goldEarned}, goldSpent={report.goldSpent}, finalMoney={report.finalMoney}, gateHP={report.gateHp}/{report.gateHpMax}, averageFPS={report.averageFps:0.0}");
        }
        catch (Exception ex)
        {
            RuntimeFileLogger.Event("RC_REPORT", $"Failed to save report: {ex.GetType().Name}: {ex.Message}");
            Debug.LogException(ex);
        }
    }

    string BuildCsv()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("wave,prepared_enemies,target_sec,actual_sec,delta_sec,kills,leaks,gold_earned,gold_spent,money_start,money_end,gate_hp_start,gate_hp_end,max_alive,completed");
        foreach (WaveReport wave in report.waves)
        {
            sb.Append(wave.wave).Append(',')
                .Append(wave.preparedEnemies).Append(',')
                .Append(F(wave.targetDurationSeconds)).Append(',')
                .Append(F(wave.actualDurationSeconds)).Append(',')
                .Append(F(wave.durationDeltaSeconds)).Append(',')
                .Append(wave.kills).Append(',')
                .Append(wave.leaks).Append(',')
                .Append(wave.goldEarned).Append(',')
                .Append(wave.goldSpent).Append(',')
                .Append(wave.moneyStart).Append(',')
                .Append(wave.moneyEnd).Append(',')
                .Append(wave.gateHpStart).Append(',')
                .Append(wave.gateHpEnd).Append(',')
                .Append(wave.maxAliveEnemies).Append(',')
                .Append(wave.completed ? "true" : "false").AppendLine();
        }
        return sb.ToString();
    }

    string BuildQaSummary()
    {
        List<string> blockers = BuildAcceptanceBlockers();
        List<string> warnings = BuildReviewWarnings();
        bool canBind = blockers.Count == 0;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("# Chapter I QA Summary");
        sb.AppendLine();
        sb.AppendLine($"- Verdict: **{(canBind ? "READY FOR HUMAN ACCEPTANCE" : "BLOCKED")}**");
        sb.AppendLine($"- Can bind to `CHAPTER_I_GAMEPLAY_ACCEPTANCE.json`: **{canBind.ToString().ToLowerInvariant()}**");
        sb.AppendLine($"- Session: `{report.sessionId}`");
        sb.AppendLine($"- Difficulty: **{report.difficulty}**");
        sb.AppendLine($"- Result: **{report.result}**");
        sb.AppendLine($"- Duration: **{FormatTime(report.actualDurationSeconds)}** (required `11:00-13:00`)");
        sb.AppendLine($"- Speed: max **{report.maxCombatSpeed:0.##}x**, non-1x used **{report.nonOneXSpeedUsed}**");
        sb.AppendLine($"- Pause used: **{report.pauseUsed}**");
        sb.AppendLine($"- Menelaus defeated / breached: **{report.menelausDefeated} / {report.menelausBreached}**");
        sb.AppendLine($"- Gate: **{report.gateHp}/{report.gateHpMax}**");
        sb.AppendLine($"- Kills / leaks: **{report.kills} / {report.leaks}**");
        sb.AppendLine($"- Average FPS: **{report.averageFps:0.0}**");
        sb.AppendLine();

        if (blockers.Count > 0)
        {
            sb.AppendLine("## Why It Cannot Be Accepted Yet");
            sb.AppendLine();
            for (int i = 0; i < blockers.Count; i++) sb.AppendLine($"- {blockers[i]}");
            sb.AppendLine();
        }
        else
        {
            sb.AppendLine("## Hard Gates");
            sb.AppendLine();
            sb.AppendLine("All hard Story baseline gates passed. Review any WARN items below before human acceptance.");
            sb.AppendLine();
        }

        sb.AppendLine("## Warnings To Review");
        sb.AppendLine();
        if (warnings.Count == 0) sb.AppendLine("- No automatic WARN findings in this summary.");
        else for (int i = 0; i < warnings.Count; i++) sb.AppendLine($"- {warnings[i]}");
        sb.AppendLine();

        sb.AppendLine("## Next Action");
        sb.AppendLine();
        if (!canBind)
        {
            sb.AppendLine("- Repeat Chapter I on `Story`.");
            sb.AppendLine("- Keep combat speed at `1x` for the whole run.");
            sb.AppendLine("- Do not pause after the chapter starts.");
            sb.AppendLine("- Aim for a real run duration between `11:00` and `13:00`.");
            sb.AppendLine("- After victory, use `TheTroyGame > Validation > Gameplay Acceptance > Prepare Latest Story Candidate`.");
        }
        else
        {
            sb.AppendLine("- Open Unity and run `TheTroyGame > Validation > Gameplay Acceptance > Prepare Latest Story Candidate`.");
            sb.AppendLine("- Review the generated `ChapterI_WARN_Review_<session>.md` before setting human acceptance fields.");
        }
        sb.AppendLine();

        sb.AppendLine("## Encounter Table");
        sb.AppendLine();
        sb.AppendLine("| Encounter | Target | Actual | Delta | Prepared | Peak alive | Kills | Leaks | Money | Gate | Complete |");
        sb.AppendLine("|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|:---:|");
        foreach (WaveReport wave in report.waves)
        {
            sb.AppendLine($"| {wave.wave} | {FormatTime(wave.targetDurationSeconds)} | {FormatTime(wave.actualDurationSeconds)} | {wave.durationDeltaSeconds:+0;-0;0}s | {wave.preparedEnemies} | {wave.maxAliveEnemies} | {wave.kills} | {wave.leaks} | {wave.moneyStart}->{wave.moneyEnd} | {wave.gateHpStart}->{wave.gateHpEnd} | {(wave.completed ? "yes" : "no")} |");
        }
        return sb.ToString();
    }

    List<string> BuildAcceptanceBlockers()
    {
        List<string> blockers = new List<string>();
        if (report.schemaVersion < CurrentReportSchemaVersion)
            blockers.Add($"Telemetry schema is old: `{report.schemaVersion}`, required `{CurrentReportSchemaVersion}`.");
        if (report.map != 1) blockers.Add($"Report is for map `{report.map}`, expected Chapter I.");
        if (!string.Equals(report.difficulty, "Story", StringComparison.OrdinalIgnoreCase))
            blockers.Add($"Difficulty is `{report.difficulty}`, expected `Story`.");
        if (report.nonOneXSpeedUsed || report.maxCombatSpeed > 1.01f)
            blockers.Add($"Combat speed was not locked to 1x: max `{report.maxCombatSpeed:0.##}x`, nonOneXSpeedUsed=`{report.nonOneXSpeedUsed}`.");
        if (report.pauseUsed)
            blockers.Add("Pause was used after the run started.");
        if (!string.Equals(report.result, "VICTORY", StringComparison.OrdinalIgnoreCase))
            blockers.Add($"Run ended with `{report.result}`, expected `VICTORY`.");
        if (!report.menelausDefeated) blockers.Add("Menelaus was not defeated.");
        if (report.menelausBreached) blockers.Add("Menelaus completed a gate breach.");
        if (report.actualDurationSeconds < 11f * 60f || report.actualDurationSeconds > 13f * 60f)
            blockers.Add($"Total duration `{FormatTime(report.actualDurationSeconds)}` is outside `11:00-13:00`.");
        if (report.gateHp <= 0) blockers.Add("Gate HP ended at zero.");

        if (report.waves == null || report.waves.Count != 5)
        {
            blockers.Add($"Expected 5 completed encounter snapshots, found `{report.waves?.Count ?? 0}`.");
            return blockers;
        }

        bool[] seen = new bool[6];
        for (int i = 0; i < report.waves.Count; i++)
        {
            WaveReport wave = report.waves[i];
            if (wave.wave >= 1 && wave.wave <= 5) seen[wave.wave] = true;
            if (!wave.completed) blockers.Add($"Encounter `{wave.wave}` snapshot is incomplete.");
            float target = Mathf.Max(1f, wave.targetDurationSeconds);
            float deviation = Mathf.Abs(wave.actualDurationSeconds - target) / target;
            if (deviation >= .40f)
                blockers.Add($"Encounter `{wave.wave}` duration deviation is `{deviation:P0}`, over the 40% hard limit.");
        }
        for (int encounter = 1; encounter <= 5; encounter++)
            if (!seen[encounter]) blockers.Add($"Encounter `{encounter}` snapshot is missing.");

        return blockers;
    }

    List<string> BuildReviewWarnings()
    {
        List<string> warnings = new List<string>();
        if (report.averageFps > 0f && report.averageFps < 45f)
            warnings.Add($"Average FPS is low: `{report.averageFps:0.0}`.");
        if (report.gateHpMax > 0)
        {
            float gateRatio = report.gateHp / (float)report.gateHpMax;
            if (report.gateHp > 0 && gateRatio < .25f) warnings.Add($"Gate survival is very low: `{report.gateHp}/{report.gateHpMax}`.");
            if (gateRatio > .90f && report.leaks == 0) warnings.Add($"Story pressure may be too forgiving: gate `{report.gateHp}/{report.gateHpMax}`, leaks `0`.");
        }
        if (report.waves != null)
        {
            for (int i = 0; i < report.waves.Count; i++)
            {
                WaveReport wave = report.waves[i];
                float target = Mathf.Max(1f, wave.targetDurationSeconds);
                float deviation = Mathf.Abs(wave.actualDurationSeconds - target) / target;
                if (deviation >= .25f && deviation < .40f)
                    warnings.Add($"Encounter `{wave.wave}` pacing differs by `{deviation:P0}` from target.");
            }
        }
        return warnings;
    }

    static string FormatTime(float seconds)
    {
        int total = Mathf.Max(0, Mathf.RoundToInt(seconds));
        return $"{total / 60:00}:{total % 60:00}";
    }

    static string F(float value) => value.ToString("0.###", CultureInfo.InvariantCulture);
}
