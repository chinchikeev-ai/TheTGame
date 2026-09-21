using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public sealed class CombatHudIndependentLayoutTests
{
    const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

    [Test]
    public void MissingSecondaryRootsDoNotBlockCombatHudAndReplacementRebinds()
    {
        var host = new GameObject("LayoutTest");
        var root = new GameObject("Hud", typeof(RectTransform));
        var replacement = new GameObject("Replacement", typeof(RectTransform));
        try
        {
            var hud = host.AddComponent<ModernCombatHud>();
            typeof(ModernCombatHud).GetMethod("Awake", Flags).Invoke(hud, null);
            var compact = host.AddComponent<ChapterOneUiCompactPresentation>();
            var top = new GameObject("TopResources", typeof(RectTransform)).GetComponent<RectTransform>();
            top.SetParent(root.transform, false);
            typeof(ModernCombatHud).GetProperty("HudRoot").SetValue(hud, root.transform);
            var bind = typeof(ChapterOneUiCompactPresentation).GetMethod("TryBindAndApply", Flags);
            Assert.IsFalse((bool)bind.Invoke(compact, null));
            Assert.AreEqual(.8f, top.localScale.x, .001f);
            Assert.AreEqual(new Vector2(16, -16), top.anchoredPosition);
            // Already-bound roots are not rewritten each frame.
            top.anchoredPosition = new Vector2(17, -17);
            bind.Invoke(compact, null);
            Assert.AreEqual(new Vector2(17, -17), top.anchoredPosition);
            var next = new GameObject("TopResources", typeof(RectTransform)).GetComponent<RectTransform>();
            next.SetParent(replacement.transform, false);
            typeof(ModernCombatHud).GetProperty("HudRoot").SetValue(hud, replacement.transform);
            bind.Invoke(compact, null);
            Assert.AreEqual(.8f, next.localScale.x, .001f);
        }
        finally
        {
            Object.DestroyImmediate(host);
            Object.DestroyImmediate(root);
            Object.DestroyImmediate(replacement);
        }
    }
}
