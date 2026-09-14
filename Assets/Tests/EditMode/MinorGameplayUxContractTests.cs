using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class MinorGameplayUxContractTests
{
    [Test]
    public void ChapterOne_FirstEncounter_HasThirtySecondPreparation()
    {
        ChapterData chapter = Resources.Load<ChapterData>("Chapters/Chapter01_Landing");
        Assert.NotNull(chapter);
        EncounterData encounter = chapter.GetEncounter(1);
        Assert.NotNull(encounter);
        Assert.AreEqual(30f, encounter.preparationTime, .01f);
    }

    [Test]
    public void DivineGift_CanBeSelectedOnlyOnceAndOnlyBeforeMapStarts()
    {
        GameObject host = new GameObject("GiftContractTest");
        GameManager manager = host.AddComponent<GameManager>();
        try
        {
            int startingGold = manager.Money;
            Assert.IsTrue(manager.GiftAvailable);
            Assert.IsTrue(manager.UseGift(DivineGiftType.Apollo));
            Assert.IsTrue(manager.GiftSelected);
            Assert.AreEqual(DivineGiftType.Apollo, manager.SelectedGift);
            Assert.AreEqual(startingGold + 50, manager.Money);
            Assert.IsFalse(manager.GiftAvailable);
            Assert.IsFalse(manager.UseGift(DivineGiftType.Ares), "A second patron selection on the same map must be rejected.");
            Assert.AreEqual(DivineGiftType.Apollo, manager.SelectedGift);

            manager.BeginRun();
            Assert.IsFalse(manager.GiftAvailable, "Patron selection must never reopen after the map run begins.");
        }
        finally
        {
            Object.DestroyImmediate(host);
        }
    }

    [Test]
    public void EconomyAndEnemyCounts_UseRequestedGlobalRatios()
    {
        Assert.AreEqual(190, DifficultyRules.StartingGold(CampaignDifficulty.Story));
        Assert.AreEqual(150, DifficultyRules.StartingGold(CampaignDifficulty.Strategos));
        Assert.AreEqual(120, DifficultyRules.StartingGold(CampaignDifficulty.Legendary));

        Assert.AreEqual(1.35f, DifficultyRules.EnemyCountMultiplier(CampaignDifficulty.Story), .0001f);
        Assert.AreEqual(1.50f, DifficultyRules.EnemyCountMultiplier(CampaignDifficulty.Strategos), .0001f);
        Assert.AreEqual(1.725f, DifficultyRules.EnemyCountMultiplier(CampaignDifficulty.Legendary), .0001f);

        Assert.AreEqual(.575f, DifficultyRules.RewardMultiplier(CampaignDifficulty.Story), .0001f);
        Assert.AreEqual(.50f, DifficultyRules.RewardMultiplier(CampaignDifficulty.Strategos), .0001f);
        Assert.AreEqual(.45f, DifficultyRules.RewardMultiplier(CampaignDifficulty.Legendary), .0001f);
    }

    [Test]
    public void HectorRouteNavigator_ProjectsOffRoadCommandsOntoRoute()
    {
        GameObject root = new GameObject("RouteTest");
        try
        {
            Transform a0 = Point(root.transform, "A0", new Vector3(0f, .6f, 0f));
            Transform a1 = Point(root.transform, "A1", new Vector3(5f, .6f, 0f));
            Transform a2 = Point(root.transform, "A2", new Vector3(10f, .6f, 0f));
            Transform[][] routes = { new[] { a0, a1, a2 } };

            Vector3 projected = HectorRouteNavigator.ProjectToNearestRoute(new Vector3(4f, .6f, 7f), routes, .6f);
            Assert.AreEqual(4f, projected.x, .01f);
            Assert.AreEqual(0f, projected.z, .01f);

            List<Vector3> path = new List<Vector3>();
            Assert.IsTrue(HectorRouteNavigator.BuildPath(a0.position, new Vector3(9f, .6f, 6f), routes, .6f, path));
            Assert.Greater(path.Count, 0);
            Assert.AreEqual(0f, path[path.Count - 1].z, .01f);
        }
        finally
        {
            Object.DestroyImmediate(root);
        }
    }

    [Test]
    public void HectorRouteNavigator_GateStartUsesRoadPointBeforeGate()
    {
        GameObject root = new GameObject("GateRouteTest");
        try
        {
            Transform p0 = Point(root.transform, "P0", new Vector3(0f, .6f, 0f));
            Transform p1 = Point(root.transform, "UnderGate", new Vector3(5f, .6f, 0f));
            Transform p2 = Point(root.transform, "Gate", new Vector3(6.5f, .6f, 0f));
            Transform[][] routes = { new[] { p0, p1, p2 } };

            Vector3 start = HectorRouteNavigator.GateStart(routes, .6f);
            Assert.AreEqual(p1.position.x, start.x, .01f);
            Assert.AreEqual(p1.position.z, start.z, .01f);
        }
        finally
        {
            Object.DestroyImmediate(root);
        }
    }

    static Transform Point(Transform parent, string name, Vector3 position)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.position = position;
        return go.transform;
    }
}
