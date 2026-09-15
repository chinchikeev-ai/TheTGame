using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class BuildVersionPresentationTests
{
    [UnityTest]
    public IEnumerator MainMenu_ShowsCompactTopLeftBuildIdentityBadge()
    {
        yield return null;
        yield return null;
        yield return null;

        GameObject badge = GameObject.Find("BuildVersionBadge");
        Assert.NotNull(badge, "Main menu must show the active build identity in the top-left corner.");

        RectTransform rect = badge.GetComponent<RectTransform>();
        Assert.NotNull(rect);
        Assert.AreEqual(new Vector2(0f, 1f), rect.anchorMin);
        Assert.AreEqual(new Vector2(0f, 1f), rect.anchorMax);
        Assert.LessOrEqual(rect.sizeDelta.x, 240f, "Build badge should remain compact and not cover the main artwork.");

        Text label = badge.GetComponentInChildren<Text>();
        Assert.NotNull(label);
        StringAssert.StartsWith("v0.6", label.text);
        StringAssert.Contains("·", label.text);
    }
}
