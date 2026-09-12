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
