using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class BuildVersionBadgeRecoveryTests
{
    [Test]
    public void MenuArtRebuild_CannotLeaveBoundBadgeHiddenBehindBackground()
    {
        var menu = new GameObject("BadgeRecoveryTestMenu", typeof(RectTransform));
        var host = new GameObject("BadgeRecoveryTestPresenter");
        try
        {
            var presenter = host.AddComponent<MainMenuBuildVersionPresentation>();
            var badge = new GameObject("BuildVersionBadge", typeof(RectTransform));
            badge.transform.SetParent(menu.transform, false);
            badge.SetActive(false);
            var background = new GameObject("ApprovedMainMenu", typeof(RectTransform));
            background.transform.SetParent(menu.transform, false);
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            typeof(MainMenuBuildVersionPresentation).GetField("boundMainMenu", flags).SetValue(presenter, menu);
            typeof(MainMenuBuildVersionPresentation).GetField("badge", flags).SetValue(presenter, badge);
            typeof(MainMenuBuildVersionPresentation).GetMethod("Update", flags).Invoke(presenter, null);
            Assert.IsTrue(badge.activeSelf);
            Assert.AreEqual(menu.transform.childCount - 1, badge.transform.GetSiblingIndex());
            Assert.AreEqual(2, menu.transform.childCount, "Recovery must reuse the existing badge.");
        }
        finally
        {
            Object.DestroyImmediate(host);
            Object.DestroyImmediate(menu);
        }
    }
}
