using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class CombatHudLayoutUxTests
{
    [UnityTest]
    public IEnumerator SpeedControls_AreCenteredInsideWaveStatus()
    {
        yield return null;
        yield return null;

        Transform waveStatus = FindSceneTransform("WaveStatus");
        Assert.NotNull(waveStatus, "Modern combat HUD must create WaveStatus.");

        Transform speedDown = waveStatus.Find("SpeedPrevious");
        Transform speedUp = waveStatus.Find("SpeedNext");
        Transform speedValue = waveStatus.Find("SpeedValue");
        Assert.NotNull(speedDown, "Left speed arrow must live inside WaveStatus.");
        Assert.NotNull(speedUp, "Right speed arrow must live inside WaveStatus.");
        Assert.NotNull(speedValue, "Current combat speed must be displayed inside WaveStatus.");

        RectTransform downRect = speedDown as RectTransform;
        RectTransform upRect = speedUp as RectTransform;
        RectTransform speedRect = speedValue as RectTransform;
        Assert.NotNull(downRect);
        Assert.NotNull(upRect);
        Assert.NotNull(speedRect);
        Assert.Less(downRect.anchoredPosition.x, 0f);
        Assert.AreEqual(0f, speedRect.anchoredPosition.x, .01f, "Speed value should be centered between arrows.");
        Assert.Greater(upRect.anchoredPosition.x, 0f);
        Assert.AreEqual(Mathf.Abs(downRect.anchoredPosition.x), Mathf.Abs(upRect.anchoredPosition.x), .01f, "Speed arrows should be symmetric around center.");
    }

    [UnityTest]
    public IEnumerator WaveStatus_HasCenteredDynamicProgressBar()
    {
        yield return null;
        yield return null;

        Transform waveStatus = FindSceneTransform("WaveStatus");
        Assert.NotNull(waveStatus);
        Transform progress = waveStatus.Find("WaveProgress");
        Assert.NotNull(progress, "WaveStatus must expose a live wave progress bar.");

        RectTransform progressRect = progress as RectTransform;
        Assert.NotNull(progressRect);
        Assert.AreEqual(0f, progressRect.anchoredPosition.x, .01f, "Wave progress must stay centered on screen.");

        Image fill = progress.Find("Fill")?.GetComponent<Image>();
        Assert.NotNull(fill);
        Assert.AreEqual(Image.Type.Filled, fill.type);
        Assert.AreEqual(Image.FillMethod.Horizontal, fill.fillMethod);
    }

    [UnityTest]
    public IEnumerator TopLeftResources_SeparateGoldAndGateHealthProgress()
    {
        yield return null;
        yield return null;

        Transform resources = FindSceneTransform("TopResources");
        Assert.NotNull(resources, "Top-left resources block must exist.");
        Assert.NotNull(resources.Find("CoinIcon"), "Gold must remain the primary top-left resource.");

        Transform gateProgress = resources.Find("GateHealthProgress");
        Assert.NotNull(gateProgress, "Gate health progress bar must be directly under gold.");
        Assert.NotNull(gateProgress.Find("Fill")?.GetComponent<Image>());
    }

    [UnityTest]
    public IEnumerator DivineGiftChoice_ContainsFourGodOptions()
    {
        yield return null;
        yield return null;

        Transform overlay = FindSceneTransform("DivineGiftChoiceOverlay");
        Assert.NotNull(overlay, "Combat HUD must create the divine gift chooser.");
        Assert.NotNull(FindChildRecursive(overlay, "Gift_Ares"));
        Assert.NotNull(FindChildRecursive(overlay, "Gift_Athena"));
        Assert.NotNull(FindChildRecursive(overlay, "Gift_Apollo"));
        Assert.NotNull(FindChildRecursive(overlay, "Gift_Poseidon"));
    }

    static Transform FindSceneTransform(string name)
    {
        return Resources.FindObjectsOfTypeAll<Transform>()
            .FirstOrDefault(t => t != null && t.name == name && t.gameObject.scene.IsValid());
    }

    static Transform FindChildRecursive(Transform root, string name)
    {
        if (root == null) return null;
        Transform[] children = root.GetComponentsInChildren<Transform>(true);
        return children.FirstOrDefault(t => t != null && t.name == name);
    }
}
