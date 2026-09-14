using UnityEngine;

public enum DivineGiftType
{
    Ares,
    Athena,
    Apollo,
    Poseidon
}

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
    public bool BossDefeated { get; private set; }
    public bool BossBreached { get; private set; }
    public float RunTime => GameEnded ? finalRunTime : runStarted ? Mathf.Max(0f, Time.unscaledTime - runStartTime) : 0f;
    public float MagicCooldownRemaining => Mathf.Max(0f, magicReadyAt - Time.unscaledTime);
    public bool GiftSelected { get; private set; }
    public DivineGiftType SelectedGift { get; private set; }
    public bool GiftAvailable => !runStarted && !GameEnded && CurrentWave == 0 && !GiftSelected;
    public float PlayerDamageMultiplier => GiftSelected && SelectedGift == DivineGiftType.Ares ? 1.10f : 1f;
    public float EnemySpeedGiftMultiplier => GiftSelected && SelectedGift == DivineGiftType.Poseidon ? .90f : 1f;
    public int FinalScore { get; private set; }

    EconomyController economy;
    float runStartTime;
    float finalRunTime;
    float magicReadyAt;
    bool runStarted;
    bool bossAtGateRecorded;

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

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void BeginRun()
    {
        if (runStarted) return;
        if (!GiftSelected)
        {
            RuntimeFileLogger.Event("RUN", $"Map {MapNumber} start rejected: patron god was not selected before the map.");
            return;
        }
        runStarted = true;
        runStartTime = Time.unscaledTime;
        RuntimeFileLogger.Event("RUN", $"Map {MapNumber} started. patron={SelectedGift}, maxWaves={MaxWaves}, gold={Money}, gateHP={BaseHealth}/{MaxBaseHealth}, difficulty={CampaignSave.Difficulty}");
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

    public void RecordBossDefeated()
    {
        if (BossDefeated) return;
        BossDefeated = true;
        RuntimeFileLogger.Event("BOSS", $"Menelaus objective completed at wave={CurrentWave}");
    }

    public void BossReachedGate(int damage)
    {
        if (GameEnded) return;
        if (!bossAtGateRecorded)
        {
            bossAtGateRecorded = true;
            RecordLeak();
            RuntimeFileLogger.Event("BOSS", "Menelaus reached the Trojan gate and started breaking it down");
        }

        BaseHealth = Mathf.Max(0, BaseHealth - Mathf.Max(1, damage));
        RuntimeFileLogger.Event("BOSS", $"Menelaus damaged the Trojan gate. remainingHP={BaseHealth}/{MaxBaseHealth}");
        if (BaseHealth <= 0)
        {
            BossBreached = true;
            RuntimeFileLogger.Event("BOSS", "Menelaus destroyed the Trojan gate");
            LoseGame();
        }
    }

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
        var enemies = new System.Collections.Generic.List<Enemy>(EnemyRegistry.All);
        RuntimeFileLogger.Event("MAGIC", $"Divine Storm used on wave={CurrentWave}, targets={enemies.Count}, damage=120, slow=50%, duration=5s");
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;
            enemy.TakeDamage(120f);
            enemy.ApplySlow(.50f, 5f);
        }
        return true;
    }

    public bool UseGift(DivineGiftType gift)
    {
        if (!GiftAvailable) return false;

        GiftSelected = true;
        SelectedGift = gift;
        switch (gift)
        {
            case DivineGiftType.Ares:
                break;
            case DivineGiftType.Athena:
                MaxBaseHealth += 2;
                BaseHealth += 2;
                break;
            case DivineGiftType.Apollo:
                AddMoney(50);
                break;
            case DivineGiftType.Poseidon:
                break;
        }

        RuntimeFileLogger.Event(
            "GIFT_SELECTED",
            $"god={gift}, map={MapNumber}, beforeRun={!runStarted}, gold={Money}, gateHP={BaseHealth}/{MaxBaseHealth}, playerDamageMul={PlayerDamageMultiplier:0.00}, enemySpeedMul={EnemySpeedGiftMultiplier:0.00}");
        return true;
    }

    // Legacy compatibility for older callers. A legacy gift request now selects Apollo once before the map starts.
    public bool UseGift() => UseGift(DivineGiftType.Apollo);

    public void WinGame()
    {
        if (GameEnded) return;
        if (Chapter != null && Chapter.chapterNumber == 1 && !BossDefeated)
        {
            RuntimeFileLogger.Event("RESULT", "Chapter I victory rejected because Menelaus was not defeated");
            LoseGame();
            return;
        }

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

        RuntimeFileLogger.Event("RESULT", $"VICTORY map={MapNumber}, waves={CurrentWave}/{MaxWaves}, bossDefeated={BossDefeated}, score={FinalScore}, time={finalRunTime:0.0}s, pacing={PacingVerdict()}, difficulty={CampaignSave.Difficulty}, kills={Kills}, leaks={Leaks}, goldEarned={GoldEarned}, goldSpent={GoldSpent}, built={TowersBuilt}, sold={TowersSold}, gateHP={BaseHealth}/{MaxBaseHealth}");
        GameStateController.Instance?.SetState(GameState.Victory);
    }

    void LoseGame()
    {
        if (GameEnded) return;
        FinalizeRun();
        GameEnded = true;
        EndMessage = "GAME OVER";
        FinalScore = CalculateScore();
        RuntimeFileLogger.Event("RESULT", $"DEFEAT map={MapNumber}, waves={CurrentWave}/{MaxWaves}, bossDefeated={BossDefeated}, bossBreached={BossBreached}, score={FinalScore}, time={finalRunTime:0.0}s, pacing={PacingVerdict()}, difficulty={CampaignSave.Difficulty}, kills={Kills}, leaks={Leaks}, goldEarned={GoldEarned}, goldSpent={GoldSpent}, built={TowersBuilt}, sold={TowersSold}, gateHP={BaseHealth}/{MaxBaseHealth}");
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
