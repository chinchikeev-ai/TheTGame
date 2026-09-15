using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RuntimeGraphTests
{
    [UnityTest]
    public IEnumerator Bootstrap_CreatesCoreRuntimeGraph()
    {
        yield return null;
        Assert.NotNull(Object.FindFirstObjectByType<GameBootstrap>());
        Assert.NotNull(CampaignController.Instance);
        Assert.NotNull(ChapterController.Instance);
        Assert.NotNull(GameManager.Instance);
        Assert.NotNull(GameStateController.Instance);
    }

    [UnityTest]
    public IEnumerator ChapterOne_IsActiveAndHasFiveEvents()
    {
        yield return null;
        Assert.NotNull(ChapterController.Instance);
        Assert.NotNull(ChapterController.Instance.ActiveChapter);
        Assert.AreEqual(1, ChapterController.Instance.ActiveChapter.chapterNumber);
        Assert.AreEqual(5, ChapterController.Instance.ActiveChapter.combatEvents);
    }

    [UnityTest]
    public IEnumerator ChapterOne_InstallsSingleOpeningPresentationOwners()
    {
        yield return null;
        yield return null;

        Assert.AreEqual(1, Object.FindObjectsByType<LandingPresentation>(FindObjectsSortMode.None).Length, "Chapter I must have one landing presentation owner.");
        Assert.AreEqual(1, Object.FindObjectsByType<ChapterOneCinematicCamera>(FindObjectsSortMode.None).Length, "Chapter I must have one cinematic camera owner.");
        Assert.AreEqual(1, Object.FindObjectsByType<ChapterOneEncounterPresentation>(FindObjectsSortMode.None).Length, "Chapter I must have one encounter presentation owner.");
    }

    [UnityTest]
    public IEnumerator Registries_StartInValidState()
    {
        yield return null;
        Assert.GreaterOrEqual(EnemyRegistry.AliveCount, 0);
        Assert.NotNull(TowerRegistry.All);
    }
}
