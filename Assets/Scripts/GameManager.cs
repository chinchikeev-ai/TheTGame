using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int Money => economy != null ? economy.Money : 0;
    public int BaseHealth { get; private set; }
    public int MaxBaseHealth { get; private set; }
    public int CurrentWave { get; set; } = 0;
    public int MaxWaves { get; set; } = 5;
    public int MapNumber { get; private set; } = 1;
    public bool GameEnded { get; private set; }
    public string EndMessage { get; private set; } = "";
    public ChapterData Chapter { get; private set; }

    public int Kills { get; private set; }
    public int Leaks { get; private set; }
    public int GoldEarned => economy != null ? economy.GoldEarned : 0;
    public int GoldSpent => economy != null ? economy.GoldSpent : 0;
    public int TowersBuilt { get; private set; }
    public int TowersSold { get; private set; }
    public float RunTime => GameEnded ? finalRunTime : runStarted ? Mathf.Max(0f, Time.unscaledTime - runStartTime) : 0f;
    public float MagicCooldownRemaining => Mathf.Max(0f, magicReadyAt - Time.unscaledTime);
    public bool GiftAvailable => giftWave != CurrentWave;
    public int FinalScore { get; private set; }

    EconomyController economy;
    float runStartTime;
    float finalRunTime;
    float magicReadyAt;
    int giftWave = -1;
    bool runStarted;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        CampaignDifficulty difficulty = CampaignController.Instance != null
            ? CampaignController.Instance.Difficulty
            : CampaignSave.Difficulty;
        economy = new EconomyController(DifficultyRules.StartingGold(difficulty));
        MaxBaseHealth = DifficultyRules.StartingGateHealth(difficulty);
        BaseHealth = MaxBaseHealth;

        Chapter = ChapterController.Instance != null
            ? ChapterController.Instance.ActiveChapter
            : Resources.Load<ChapterData>("Chapters/Chapter01_Landing");

        if (Chapter != null)
        {
            MapNumber = Chapter.chapterNumber;
            MaxWaves = Chapter.combatEvents;
        }
        RuntimeFileLogger.Event("GAME", $"GameManager ready. Map={MapNumber}, startGold={Money}, gateHP={BaseHealth}/{MaxBaseHealth}, maxWaves={MaxWaves}, chapter={(Chapter != null ? Chapter.chapterId : "runtime")}, difficulty={difficulty}");
    }

    public void BeginRun()
    {
        if (runStarted) return;
        runStarted = true;
        runStartTime = Time.unscaledTime;
        RuntimeFileLogger.Event("RUN", $"Map {MapNumber} started. maxWaves={MaxWaves}, gold={Money}, gateHP={BaseHealth}/{MaxBaseHealth}, difficulty={CampaignSave.Difficulty}");
    }

    public void AddMoney(int amount) => economy?.AddIncome(amount);
    public void RefundMoney(int amount) => economy?.AddRefund(amount);

    public bool SpendMoney(int amount)
    {
        if (GameEnded || economy == null) return false;
        return economy.TrySpend(amount);
    }

    public void RecordKill() => Kills++;
    public void RecordLeak() => Leaks++;
    public void RecordTowerBuilt() => TowersBuilt++;
    public void RecordTowerSold() => TowersSold++;

    public int RewardFor(int baseReward)
    {
        return Mathf.Max(1, Mathf.RoundToInt(baseReward * DifficultyRules.RewardMultiplier(CampaignSave.Difficulty)));
    }

    public void DamageBase(int damage)
    {
        if (GameEnded) return;
        BaseHealth = Mathf.Max(0, BaseHealth - damage);
        RuntimeFileLogger.Event("GATE", $"Damage={damage}, remainingHP={BaseHealth}/{MaxBaseHealth}");
        if (BaseHealth <= 0) LoseGame();
    }

    public void HealBase(int amount)
    {
        if (GameEnded) return;
        BaseHealth = Mathf.Min(MaxBaseHealth, BaseHealth + Mathf.Max(0, amount));
    }

    public bool UseMagic()
    {
        if (GameEnded || Time.unscaledTime < magicReadyAt || EnemyRegistry.AliveCount == 0) return false;
        magicReadyAt = Time.unscaledTime + 30f;
        List<Enemy> enemies = new List<Enemy>(EnemyRegistry.All);
        RuntimeFileLogger.Event("MAGIC", $"Used on wave={CurrentWave}, targets={enemies.Count}");
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;
            enemy.TakeDamage(120f);
            enemy.ApplySlow(.50f, 5f);
        }
        return true;
    }

    public bool UseGift()
    {
        if (GameEnded || CurrentWave <= 0 || giftWave == CurrentWave) return false;
        giftWave = CurrentWave;
        AddMoney(100);
        HealBase(2);
        RuntimeFileLogger.Event("GIFT", $"Used on wave={CurrentWave}, gold={Money}, gateHP={BaseHealth}/{MaxBaseHealth}");
        return true;
    }

    public void WinGame()
    {
        if (GameEnded) return;
        FinalizeRun();
        GameEnded = true;
        EndMessage = "VICTORY";
        FinalScore = CalculateScore();

        if (Chapter != null)
        {
            if (CampaignController.Instance != null)
                CampaignController.Instance.CompleteChapter(Chapter, FinalScore, finalRunTime, BaseHealth);
            else
                CampaignSave.RecordChapterResult(Chapter.chapterNumber, FinalScore, finalRunTime, BaseHealth, Chapter.unlockChapter);
        }

        RuntimeFileLogger.Event("RESULT", $"VICTORY map={MapNumber}, waves={CurrentWave}/{MaxWaves}, score={FinalScore}, time={finalRunTime:0.0}s, pacing={PacingVerdict()}, difficulty={CampaignSave.Difficulty}, kills={Kills}, leaks={Leaks}, goldEarned={GoldEarned}, goldSpent={GoldSpent}, built={TowersBuilt}, sold={TowersSold}, gateHP={BaseHealth}/{MaxBaseHealth}");
        GameStateController.Instance?.SetState(GameState.Victory);
    }

    void LoseGame()
    {
        if (GameEnded) return;
        FinalizeRun();
        GameEnded = true;
        BaseHealth = 0;
        EndMessage = "GAME OVER";
        FinalScore = CalculateScore();
        RuntimeFileLogger.Event("RESULT", $"DEFEAT map={MapNumber}, waves={CurrentWave}/{MaxWaves}, score={FinalScore}, time={finalRunTime:0.0}s, pacing={PacingVerdict()}, difficulty={CampaignSave.Difficulty}, kills={Kills}, leaks={Leaks}, goldEarned={GoldEarned}, goldSpent={GoldSpent}, built={TowersBuilt}, sold={TowersSold}");
        GameStateController.Instance?.SetState(GameState.Defeat);
    }

    public string PacingVerdict()
    {
        if (RunTime <= 0f) return "NO_DATA";
        float minutes = RunTime / 60f;
        if (minutes < 11f) return "TOO_FAST";
        if (minutes > 13f) return "TOO_SLOW";
        return "TARGET_11_13_MIN";
    }

    int CalculateScore()
    {
        ChapterScoreInput input = new ChapterScoreInput
        {
            kills = Kills,
            leaks = Leaks,
            gateHealth = BaseHealth,
            goldEarned = GoldEarned,
            goldSpent = GoldSpent,
            runTimeSeconds = RunTime,
            targetDurationSeconds = Chapter != null ? Chapter.targetDurationMinutes * 60f : 0f,
            difficulty = CampaignSave.Difficulty
        };
        return ScoreController.Calculate(input);
    }

    void FinalizeRun()
    {
        finalRunTime = runStarted ? Mathf.Max(0f, Time.unscaledTime - runStartTime) : 0f;
    }
}
