using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;

public class ArchitectureContractTests
{
    [Test]
    public void ChapterOne_HasCanonicalContract()
    {
        ChapterData chapter = Resources.Load<ChapterData>("Chapters/Chapter01_Landing");
        Assert.NotNull(chapter);
        Assert.AreEqual(1, chapter.chapterNumber);
        Assert.AreEqual(5, chapter.combatEvents);
        Assert.AreEqual(5, chapter.EncounterCount);
        Assert.AreEqual(ChapterOneRuntimeInstaller.ProfileId, chapter.runtimeProfile);
        Assert.GreaterOrEqual(chapter.unlockChapter, 2);
    }

    [Test]
    public void ChapterOne_EncounterComposition_IsAuthored()
    {
        ChapterData chapter = Resources.Load<ChapterData>("Chapters/Chapter01_Landing");
        Assert.NotNull(chapter);

        int[] expectedCounts = { 8, 12, 16, 20, 25 };
        for (int encounterNumber = 1; encounterNumber <= 5; encounterNumber++)
        {
            EncounterData encounter = chapter.GetEncounter(encounterNumber);
            Assert.NotNull(encounter, $"Encounter {encounterNumber}");
            Assert.AreEqual(encounterNumber, encounter.encounterNumber);
            Assert.AreEqual(expectedCounts[encounterNumber - 1], encounter.BaseEnemyCount, $"Encounter {encounterNumber}");
            Assert.NotNull(encounter.spawnGroups);
            Assert.Greater(encounter.spawnGroups.Length, 0);
            if (encounterNumber < 5) Assert.IsFalse(encounter.HasBoss, $"Encounter {encounterNumber} must not contain a boss.");
        }
    }

    [Test]
    public void ChapterOne_AutoStartPacing_IsElevenToThirteenMinutes()
    {
        ChapterData chapter = Resources.Load<ChapterData>("Chapters/Chapter01_Landing");
        Assert.NotNull(chapter);

        float totalSeconds = 0f;
        for (int encounterNumber = 1; encounterNumber <= chapter.EncounterCount; encounterNumber++)
        {
            EncounterData encounter = chapter.GetEncounter(encounterNumber);
            Assert.NotNull(encounter, $"Encounter {encounterNumber}");
            totalSeconds += encounter.preparationTime + encounter.targetDuration;
        }

        Assert.GreaterOrEqual(totalSeconds, 11f * 60f, $"Chapter I auto-start target is too short: {totalSeconds:0}s");
        Assert.LessOrEqual(totalSeconds, 13f * 60f, $"Chapter I auto-start target is too long: {totalSeconds:0}s");
    }

    [Test]
    public void ChapterOne_FinalEncounter_IsMenelausBossEncounter()
    {
        ChapterData chapter = Resources.Load<ChapterData>("Chapters/Chapter01_Landing");
        Assert.NotNull(chapter);
        EncounterData finalEncounter = chapter.GetEncounter(5);
        EnemyData boss = BalanceCatalog.GetEnemy(EnemyArchetype.Boss);

        Assert.NotNull(finalEncounter);
        Assert.IsTrue(finalEncounter.HasBoss);
        Assert.IsTrue(finalEncounter.Contains(EnemyArchetype.Boss));
        Assert.NotNull(boss);
        Assert.AreEqual("menelaus", boss.id);

        bool menelausBehavior = false;
        foreach (EncounterSpawnGroup group in finalEncounter.spawnGroups)
            if (group != null && string.Equals(group.behaviorId, "menelaus", StringComparison.OrdinalIgnoreCase)) menelausBehavior = true;
        Assert.IsTrue(menelausBehavior, "Final encounter must explicitly bind the Menelaus runtime behavior.");
    }

    [Test]
    public void ChapterOne_GameplayAcceptanceManifest_HasAllFinalGates()
    {
        const string path = "Assets/Game/QA/CHAPTER_I_GAMEPLAY_ACCEPTANCE.json";
        Assert.IsTrue(File.Exists(path), $"Missing {path}");
        string json = File.ReadAllText(path);
        StringAssert.Contains("\"sourceSessionId\"", json);
        StringAssert.Contains("\"sourceReportSha256\"", json);
        StringAssert.Contains("\"warningsReviewed\"", json);
        StringAssert.Contains("\"humanAccepted\"", json);
        StringAssert.Contains("\"strategosReviewed\"", json);
        StringAssert.Contains("\"legendaryReviewed\"", json);
        StringAssert.Contains("\"ru1920x1080\"", json);
        StringAssert.Contains("\"en1920x1080\"", json);
        StringAssert.Contains("\"ru1366or1376x768\"", json);
        StringAssert.Contains("\"en1366or1376x768\"", json);
        StringAssert.Contains("\"gameplayFrozen\"", json);
    }

    [Test]
    public void GameLanguage_AllowsDeterministicQaSelection()
    {
        bool wasRussian = GameLanguage.Russian;
        try
        {
            GameLanguage.SetRussian(true);
            Assert.IsTrue(GameLanguage.Russian);
            Assert.AreEqual("RU", GameLanguage.Code);
            GameLanguage.SetRussian(false);
            Assert.IsFalse(GameLanguage.Russian);
            Assert.AreEqual("EN", GameLanguage.Code);
        }
        finally
        {
            GameLanguage.SetRussian(wasRussian);
        }
    }

    [Test]
    public void BattlefieldClamp_KeepsHeroInsideMap()
    {
        Vector3 clamped = MapBuilder.ClampToPlayableArea(new Vector3(999f, 1f, -999f));
        Vector3 min = MapBuilder.CellToWorld(new Vector2Int(0, 0));
        Vector3 max = MapBuilder.CellToWorld(new Vector2Int(MapBuilder.GridWidth - 1, MapBuilder.GridHeight - 1));
        Assert.Greater(clamped.x, min.x);
        Assert.Less(clamped.x, max.x);
        Assert.Greater(clamped.z, min.z);
        Assert.Less(clamped.z, max.z);
        Assert.AreEqual(1f, clamped.y);
    }

    [Test]
    public void EveryTowerType_HasData()
    {
        foreach (TowerType type in Enum.GetValues(typeof(TowerType)))
        {
            TowerData data = BalanceCatalog.GetTower(type);
            Assert.NotNull(data, type.ToString());
            Assert.Greater(data.cost, 0, type.ToString());
        }
    }

    [Test]
    public void EveryEnemyType_HasData()
    {
        foreach (EnemyArchetype type in Enum.GetValues(typeof(EnemyArchetype)))
        {
            EnemyData data = BalanceCatalog.GetEnemy(type);
            Assert.NotNull(data, type.ToString());
            Assert.Greater(data.hpMultiplier, 0f, type.ToString());
        }
    }

    [Test]
    public void Economy_RefundDoesNotCountAsIncome()
    {
        EconomyController economy = new EconomyController(300);
        Assert.IsTrue(economy.TrySpend(100));
        economy.AddIncome(50);
        economy.AddRefund(25);
        Assert.AreEqual(275, economy.Money);
        Assert.AreEqual(50, economy.GoldEarned);
        Assert.AreEqual(100, economy.GoldSpent);
    }

    [Test]
    public void Score_ReturnsPositiveValue()
    {
        ChapterScoreInput input = new ChapterScoreInput
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
        Assert.Greater(ScoreController.Calculate(input), 0);
    }

    [Test]
    public void Difficulty_HasIncreasingCombatPressure()
    {
        Assert.Less(DifficultyRules.EnemyHpMultiplier(CampaignDifficulty.Story), DifficultyRules.EnemyHpMultiplier(CampaignDifficulty.Strategos));
        Assert.Less(DifficultyRules.EnemyHpMultiplier(CampaignDifficulty.Strategos), DifficultyRules.EnemyHpMultiplier(CampaignDifficulty.Legendary));
        Assert.Greater(DifficultyRules.StartingGold(CampaignDifficulty.Story), DifficultyRules.StartingGold(CampaignDifficulty.Legendary));
    }
}
