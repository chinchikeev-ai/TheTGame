using System;
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
        Assert.GreaterOrEqual(chapter.unlockChapter, 2);
    }

    [Test]
    public void ChapterOne_AutoStartPacing_IsElevenToThirteenMinutes()
    {
        float totalSeconds = 0f;
        for (int wave = 1; wave <= 5; wave++)
        {
            WaveData data = BalanceCatalog.GetWave(wave, 5);
            Assert.NotNull(data, $"Wave {wave}");
            totalSeconds += data.preparationTime + data.targetDuration;
        }

        Assert.GreaterOrEqual(totalSeconds, 11f * 60f, $"Chapter I auto-start target is too short: {totalSeconds:0}s");
        Assert.LessOrEqual(totalSeconds, 13f * 60f, $"Chapter I auto-start target is too long: {totalSeconds:0}s");
    }

    [Test]
    public void ChapterOne_FinalWave_IsMenelausBossWave()
    {
        WaveData finalWave = BalanceCatalog.GetWave(5, 5);
        EnemyData boss = BalanceCatalog.GetEnemy(EnemyArchetype.Boss);
        Assert.NotNull(finalWave);
        Assert.IsTrue(finalWave.hasBoss);
        Assert.NotNull(boss);
        Assert.AreEqual("menelaus", boss.id);
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
