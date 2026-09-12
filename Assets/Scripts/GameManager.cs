using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int Money { get; private set; } = 300;
    public int BaseHealth { get; private set; } = 20;
    public int CurrentWave { get; set; } = 0;
    public int MaxWaves { get; set; } = 5;
    public int MapNumber { get; private set; } = 1;
    public bool GameEnded { get; private set; }
    public string EndMessage { get; private set; } = "";
    public ChapterData Chapter { get; private set; }

    public int Kills { get; private set; }
    public int Leaks { get; private set; }
    public int GoldEarned { get; private set; }
    public int GoldSpent { get; private set; }
    public int TowersBuilt { get; private set; }
    public int TowersSold { get; private set; }
    public float RunTime => GameEnded ? finalRunTime : runStarted ? Mathf.Max(0f, Time.unscaledTime - runStartTime) : 0f;
    public float MagicCooldownRemaining => Mathf.Max(0f, magicReadyAt - Time.unscaledTime);
    public bool GiftAvailable => giftWave != CurrentWave;
    public int FinalScore { get; private set; }

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
        Chapter = Resources.Load<ChapterData>("Chapters/Chapter01_Landing");
        if (Chapter != null)
        {
            MapNumber = Chapter.chapterNumber;
            MaxWaves = Chapter.combatEvents;
        }
        RuntimeFileLogger.Event("GAME", $"GameManager ready. Map={MapNumber}, startGold={Money}, gateHP={BaseHealth}, maxWaves={MaxWaves}, chapter={(Chapter != null ? Chapter.chapterId : "runtime")}");
    }

    public void BeginRun()
    {
        if (runStarted) return;
        runStarted = true;
        runStartTime = Time.unscaledTime;
        RuntimeFileLogger.Event("RUN", $"Map {MapNumber} started. maxWaves={MaxWaves}, gold={Money}, gateHP={BaseHealth}");
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0) return;
        Money += amount;
        GoldEarned += amount;
    }

    public bool SpendMoney(int amount)
    {
        if (GameEnded || Money < amount) return false;
        Money -= amount;
        GoldSpent += amount;
        return true;
    }

    public void RecordKill() => Kills++;
    public void RecordLeak() => Leaks++;
    public void RecordTowerBuilt() => TowersBuilt++;
    public void RecordTowerSold() => TowersSold++;

    public void DamageBase(int damage)
    {
        if (GameEnded) return;
        BaseHealth = Mathf.Max(0, BaseHealth - damage);
        RuntimeFileLogger.Event("GATE", $"Damage={damage}, remainingHP={BaseHealth}");
        if (BaseHealth <= 0) LoseGame();
    }

    public void HealBase(int amount)
    {
        if (GameEnded) return;
        BaseHealth = Mathf.Min(20, BaseHealth + Mathf.Max(0, amount));
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
        RuntimeFileLogger.Event("GIFT", $"Used on wave={CurrentWave}, gold={Money}, gateHP={BaseHealth}");
        return true;
    }

    public void WinGame()
    {
        if (GameEnded) return;
        FinalizeRun();
        GameEnded = true;
        EndMessage = "VICTORY";
        FinalScore = CalculateScore();
        if (Chapter != null) CampaignSave.CompleteChapter(Chapter.chapterNumber, FinalScore, Chapter.unlockChapter);
        RuntimeFileLogger.Event("RESULT", $"VICTORY map={MapNumber}, waves={CurrentWave}/{MaxWaves}, score={FinalScore}, time={finalRunTime:0.0}s, kills={Kills}, leaks={Leaks}, goldEarned={GoldEarned}, goldSpent={GoldSpent}, built={TowersBuilt}, sold={TowersSold}, gateHP={BaseHealth}");
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
        RuntimeFileLogger.Event("RESULT", $"DEFEAT map={MapNumber}, waves={CurrentWave}/{MaxWaves}, score={FinalScore}, time={finalRunTime:0.0}s, kills={Kills}, leaks={Leaks}, goldEarned={GoldEarned}, goldSpent={GoldSpent}, built={TowersBuilt}, sold={TowersSold}");
        GameStateController.Instance?.SetState(GameState.Defeat);
    }

    int CalculateScore()
    {
        int score = Kills * 100;
        score += BaseHealth * 250;
        score += Mathf.Max(0, 3000 - Leaks * 350);
        score += Mathf.Max(0, GoldEarned - GoldSpent / 2);
        if (Chapter != null && RunTime > 0f)
        {
            float target = Chapter.targetDurationMinutes * 60f;
            score += Mathf.RoundToInt(Mathf.Clamp(target / RunTime, .5f, 1.5f) * 2000f);
        }
        return Mathf.Max(0, score);
    }

    void FinalizeRun()
    {
        finalRunTime = runStarted ? Mathf.Max(0f, Time.unscaledTime - runStartTime) : 0f;
    }
}
