using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public sealed class PreMapArtworkBindingTests
{
    [Test]
    public void GraphicStartOpensDifficultyInsteadOfStartingRun()
    {
        var root = new GameObject("TestChapterSelection", typeof(RectTransform), typeof(Image));
        var difficulty = new GameObject("TestDifficulty");
        float timeScale = Time.timeScale;
        try
        {
            int starts = 0;
            var art = root.AddComponent<ChapterSelectionArtwork>();
            art.Build(() => starts++, () => {}, chapter => chapter == 1, true);
            var flow = root.AddComponent<PreMapPatronSelectionPresentation>();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            typeof(PreMapPatronSelectionPresentation).GetField("difficultyOverlay", flags).SetValue(flow, difficulty);
            difficulty.SetActive(false);
            Assert.IsTrue((bool)typeof(PreMapPatronSelectionPresentation).GetMethod("BindChapterAction", flags).Invoke(flow, new object[] { root.transform }));
            art.StartButton.onClick.Invoke();
            Assert.AreEqual(0, starts);
            Assert.IsTrue(difficulty.activeSelf);
            Assert.AreEqual(0, Time.timeScale);
        }
        finally
        {
            Time.timeScale = timeScale;
            Object.DestroyImmediate(root);
            Object.DestroyImmediate(difficulty);
        }
    }
}
