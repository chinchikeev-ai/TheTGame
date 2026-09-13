using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public sealed class ChapterOnePlaythroughReporter : MonoBehaviour
{
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

    GameManager game;
    EnemySpawner spawner;
    readonly PlaythroughReport report = new PlaythroughReport();

    int activeWave;
    float waveStartUnscaled;
    int startKills;
    int startLeaks;
    int startGoldEarned;
    int startGoldSpent;
    int startMoney;
    int startGateHp;
    int maxAlive;
    int frameCount;
    float observedSeconds;
    bool reportWritten;

    void Start()
    {
        game = GameManager.Instance;
        spawner = FindFirstObjectByType<EnemySpawner>();
        report.sessionId = Guid.NewGuid().ToString("N");
        report.createdUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
        report.language = GameLanguage.Code;
        report.resolution = $"{Screen.width}x{Screen.height}";
        report.difficulty = CampaignSave.Difficulty.ToString();
        RuntimeFileLogger.Event("RC_REPORT", $"Chapter I playthrough reporter armed. session={report.sessionId}");
    }

    void Update()
    {
        if (game == null) game = GameManager.Instance;
        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();
        if (game == null || spawner == null || reportWritten) return;

        if (game.RunTime > 0f)
        {
            frameCount++;
            observedSeconds += Time.unscaledDeltaTime;
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

    void BeginWave(int wave)
    {
        activeWave = wave;
        waveStartUnscaled = Time.unscaledTime;
        startKills = game.Kills;
        startLeaks = game.Leaks;
        startGoldEarned = game.GoldEarned;
        startGoldSpent = game.GoldSpent;
        startMoney = game.Money;
        startGateHp = game.BaseHealth;
        maxAlive = EnemyRegistry.AliveCount;
        RuntimeFileLogger.Event("RC_REPORT", $"Wave snapshot started wave={wave}, gold={startMoney}, gateHP={startGateHp}, preparedEnemies={spawner.NextWaveEnemyCount}");
    }

    void FinishWave(bool completed)
    {
        if (activeWave <= 0) return;

        float actual = Mathf.Max(0f, Time.unscaledTime - waveStartUnscaled);
        float target = spawner.TargetWaveDuration;
        WaveReport wave = new WaveReport
        {
            wave = activeWave,
            preparedEnemies = spawner.NextWaveEnemyCount,
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
            completed = completed && !game.GameEnded
        };
        report.waves.Add(wave);
        RuntimeFileLogger.Event("RC_REPORT", $"Wave snapshot wave={wave.wave}, actual={wave.actualDurationSeconds:0.0}s, target={wave.targetDurationSeconds:0.0}s, kills={wave.kills}, leaks={wave.leaks}, goldEarned={wave.goldEarned}, goldSpent={wave.goldSpent}, money={wave.moneyStart}->{wave.moneyEnd}, gateHP={wave.gateHpStart}->{wave.gateHpEnd}, maxAlive={wave.maxAliveEnemies}, completed={wave.completed}");
        activeWave = 0;
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

            File.WriteAllText(LastReportPath, JsonUtility.ToJson(report, true), new UTF8Encoding(false));
            File.WriteAllText(LastCsvPath, BuildCsv(), new UTF8Encoding(false));

            RuntimeFileLogger.Event("RC_REPORT", $"Saved JSON={LastReportPath}");
            RuntimeFileLogger.Event("RC_REPORT", $"Saved CSV={LastCsvPath}");
            RuntimeFileLogger.Event("RC_REPORT", $"Summary result={report.result}, time={report.actualDurationSeconds:0.0}s, pacing={report.pacingVerdict}, score={report.finalScore}, kills={report.kills}, leaks={report.leaks}, goldEarned={report.goldEarned}, goldSpent={report.goldSpent}, finalMoney={report.finalMoney}, gateHP={report.gateHp}/{report.gateHpMax}, averageFPS={report.averageFps:0.0}");
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

    static string F(float value) => value.ToString("0.###", CultureInfo.InvariantCulture);
}
