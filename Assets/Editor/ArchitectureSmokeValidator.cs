#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ArchitectureSmokeValidator
{
    static ArchitectureSmokeValidator()
    {
        EditorApplication.delayCall += RunOnEditorLoad;
    }

    [MenuItem("TheTroyGame/Validation/Run Architecture Smoke Checks")]
    public static void Run()
    {
        int errors = 0;

        ChapterData chapter = Resources.Load<ChapterData>("Chapters/Chapter01_Landing");
        if (chapter == null)
        {
            Debug.LogError("[SMOKE] Missing Chapter01_Landing ChapterData asset.");
            errors++;
        }
        else
        {
            if (chapter.chapterNumber != 1) { Debug.LogError("[SMOKE] Chapter I number must be 1."); errors++; }
            if (chapter.combatEvents != 5) { Debug.LogError("[SMOKE] Chapter I must have 5 combat events."); errors++; }
            if (chapter.unlockChapter < 2) { Debug.LogError("[SMOKE] Chapter I must unlock Chapter II."); errors++; }
        }

        foreach (TowerType type in Enum.GetValues(typeof(TowerType)))
        {
            TowerData data = BalanceCatalog.GetTower(type);
            if (data == null) { Debug.LogError($"[SMOKE] Missing TowerData for {type}."); errors++; continue; }
            if (data.cost <= 0) { Debug.LogError($"[SMOKE] Invalid cost for {type}."); errors++; }
            if (data.damage < 0f) { Debug.LogError($"[SMOKE] Invalid damage for {type}."); errors++; }
        }

        foreach (EnemyArchetype type in Enum.GetValues(typeof(EnemyArchetype)))
        {
            EnemyData data = BalanceCatalog.GetEnemy(type);
            if (data == null) { Debug.LogError($"[SMOKE] Missing EnemyData for {type}."); errors++; continue; }
            if (data.hpMultiplier <= 0f) { Debug.LogError($"[SMOKE] Invalid HP multiplier for {type}."); errors++; }
            if (data.speedMultiplier <= 0f) { Debug.LogError($"[SMOKE] Invalid speed multiplier for {type}."); errors++; }
        }

        for (int wave = 1; wave <= 5; wave++)
        {
            WaveData data = BalanceCatalog.GetWave(wave, 5);
            if (data == null) { Debug.LogError($"[SMOKE] Missing WaveData for wave {wave}."); errors++; continue; }
            if (data.enemyCount <= 0) { Debug.LogError($"[SMOKE] Wave {wave} has no enemies."); errors++; }
            if (data.spawnInterval <= 0f) { Debug.LogError($"[SMOKE] Wave {wave} has invalid spawn interval."); errors++; }
        }

        EconomyController economy = new EconomyController(300);
        if (!economy.TrySpend(100) || economy.Money != 200 || economy.GoldSpent != 100)
        {
            Debug.LogError("[SMOKE] EconomyController spend contract failed.");
            errors++;
        }
        economy.AddIncome(50);
        economy.AddRefund(25);
        if (economy.Money != 275 || economy.GoldEarned != 50)
        {
            Debug.LogError("[SMOKE] EconomyController income/refund accounting failed.");
            errors++;
        }

        ChapterScoreInput scoreInput = new ChapterScoreInput
        {
            kills = 10,
            leaks = 0,
            gateHealth = 20,
            goldEarned = 500,
            goldSpent = 300,
            runTimeSeconds = 720f,
            targetDurationSeconds = 720f,
            difficulty = CampaignDifficulty.Strategos
        };
        if (ScoreController.Calculate(scoreInput) <= 0)
        {
            Debug.LogError("[SMOKE] ScoreController returned invalid score.");
            errors++;
        }

        if (errors == 0) Debug.Log("[SMOKE] Architecture checks passed.");
        else Debug.LogError($"[SMOKE] Architecture checks failed: {errors} error(s).");
    }

    static void RunOnEditorLoad()
    {
        // Keep editor startup non-blocking: validate only after script compilation settles.
        if (!EditorApplication.isCompiling) Run();
    }
}
#endif
