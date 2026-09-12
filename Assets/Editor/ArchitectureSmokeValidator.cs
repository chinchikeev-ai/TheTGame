#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ArchitectureSmokeValidator
{
    static readonly string[] RequiredScripts =
    {
        "Assets/Game/Core/Bootstrap/GameBootstrap.cs",
        "Assets/Game/Core/Session/GameManager.cs",
        "Assets/Game/Core/Input/GameInput.cs",
        "Assets/Game/Core/State/GameStateController.cs",
        "Assets/Game/Core/Logging/RuntimeFileLogger.cs",
        "Assets/Game/Campaign/CampaignController.cs",
        "Assets/Game/Campaign/ChapterController.cs",
        "Assets/Game/Campaign/Persistence/CampaignSave.cs",
        "Assets/Game/Combat/CombatDamage.cs",
        "Assets/Game/Enemies/Enemy.cs",
        "Assets/Game/Enemies/EnemySpawner.cs",
        "Assets/Game/Towers/Tower.cs",
        "Assets/Game/Towers/TowerRegistry.cs",
        "Assets/Game/Heroes/Hector/HectorController.cs",
        "Assets/Game/World/MapBuilder.cs",
        "Assets/Game/UI/GameMenuController.cs"
    };

    static ArchitectureSmokeValidator()
    {
        EditorApplication.delayCall += RunOnEditorLoad;
    }

    [MenuItem("TheTroyGame/Validation/Run Architecture Smoke Checks")]
    public static void Run()
    {
        int errors = 0;

        foreach (string path in RequiredScripts)
            CheckScriptPath(path, ref errors);

        CheckAsset("Assets/Game/TheTroyGame.Runtime.asmdef", ref errors);
        CheckAsset("Assets/Editor/TheTroyGame.Editor.asmdef", ref errors);
        CheckAsset("Assets/Tests/EditMode/TheTroyGame.EditModeTests.asmdef", ref errors);

        if (AssetDatabase.IsValidFolder("Assets/Scripts"))
        {
            Debug.LogError("[SMOKE] Legacy Assets/Scripts folder must not exist. Runtime code belongs under Assets/Game modules.");
            errors++;
        }

        ValidateSourceBoundaries(ref errors);
        ValidateGameData(ref errors);
        ValidatePureControllers(ref errors);

        if (errors == 0) Debug.Log("[SMOKE] Architecture checks passed.");
        else Debug.LogError($"[SMOKE] Architecture checks failed: {errors} error(s).");
    }

    static void ValidateSourceBoundaries(ref int errors)
    {
        string root = Path.GetFullPath("Assets/Game");
        if (!Directory.Exists(root))
        {
            Debug.LogError("[SMOKE] Assets/Game is missing.");
            errors++;
            return;
        }

        foreach (string file in Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories))
        {
            string normalized = file.Replace('\\', '/');
            string text = File.ReadAllText(file);

            bool isGameInput = normalized.EndsWith("/Core/Input/GameInput.cs", StringComparison.Ordinal);
            if (!isGameInput &&
                (text.Contains("Mouse.current") || text.Contains("Keyboard.current") || text.Contains("Input.Get")))
            {
                Debug.LogError($"[SMOKE] Direct input access outside GameInput: {normalized}");
                errors++;
            }

            if (text.Contains("FindObjectsByType<") || text.Contains("FindObjectsOfType<"))
            {
                Debug.LogError($"[SMOKE] Scene-wide gameplay search is forbidden: {normalized}");
                errors++;
            }

            if (normalized.Contains("/Game/UI/") && text.Contains("CampaignSave."))
            {
                Debug.LogError($"[SMOKE] UI must use CampaignController instead of CampaignSave directly: {normalized}");
                errors++;
            }
        }
    }

    static void ValidateGameData(ref int errors)
    {
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
    }

    static void ValidatePureControllers(ref int errors)
    {
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
    }

    static void CheckScriptPath(string path, ref int errors)
    {
        if (AssetDatabase.LoadAssetAtPath<MonoScript>(path) != null) return;
        Debug.LogError($"[SMOKE] Architecture script missing from canonical path: {path}");
        errors++;
    }

    static void CheckAsset(string path, ref int errors)
    {
        if (File.Exists(path)) return;
        Debug.LogError($"[SMOKE] Required architecture asset missing: {path}");
        errors++;
    }

    static void RunOnEditorLoad()
    {
        if (!EditorApplication.isCompiling) Run();
    }
}
#endif
