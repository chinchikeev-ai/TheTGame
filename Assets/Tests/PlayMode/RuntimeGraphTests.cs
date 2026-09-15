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
    public IEnumerator ChapterOne_CoastHasLayeredIrregularShoreline()
    {
        yield return null;
        yield return null;

        GameObject coast = GameObject.Find("Chapter01_CoastEnvironment");
        Assert.NotNull(coast, "Chapter I coast environment must exist.");
        Assert.NotNull(FindNamed(coast.transform,"Shore Shallow Gradient Band"));
        Assert.NotNull(FindNamed(coast.transform,"Shore Wet Band"));
        Assert.NotNull(FindNamed(coast.transform,"Shore Dry Sand Band"));
        Assert.NotNull(FindNamed(coast.transform,"Shore Land Transition Band"));
        Assert.GreaterOrEqual(CountNamed(coast.transform,"Beach Pebble"),28,"Pebble fields should break up the flat beach without becoming gameplay blockers.");
        Assert.GreaterOrEqual(CountNamed(coast.transform,"Driftwood"),4,"Washed-up debris should make the landing edge read as a real coast.");

        float left = CoastEnvironmentBuilder.ShorelineX(-8f);
        float center = CoastEnvironmentBuilder.ShorelineX(0f);
        float right = CoastEnvironmentBuilder.ShorelineX(8f);
        Assert.Greater(Mathf.Abs(left-center)+Mathf.Abs(center-right),.18f,"The authored shoreline must not collapse back to a straight edge.");
    }

    [UnityTest]
    public IEnumerator Registries_StartInValidState()
    {
        yield return null;
        Assert.GreaterOrEqual(EnemyRegistry.AliveCount, 0);
        Assert.NotNull(TowerRegistry.All);
    }

    static Transform FindNamed(Transform root,string objectName)
    {
        Transform[] all=root.GetComponentsInChildren<Transform>(true);
        for(int i=0;i<all.Length;i++) if(all[i].name==objectName) return all[i];
        return null;
    }

    static int CountNamed(Transform root,string objectName)
    {
        int count=0;
        Transform[] all=root.GetComponentsInChildren<Transform>(true);
        for(int i=0;i<all.Length;i++) if(all[i].name==objectName) count++;
        return count;
    }
}
