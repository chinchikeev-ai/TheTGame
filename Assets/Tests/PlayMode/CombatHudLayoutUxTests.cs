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

        Transform waveStatus = Resources.FindObjectsOfTypeAll<Transform>()
            .FirstOrDefault(t => t != null && t.name == "WaveStatus" && t.gameObject.scene.IsValid());
        Assert.NotNull(waveStatus, "Modern combat HUD must create WaveStatus.");

        Transform speedDown = waveStatus.Find("−");
        Transform speedUp = waveStatus.Find("+");
        Assert.NotNull(speedDown, "Speed-down control must live inside WaveStatus.");
        Assert.NotNull(speedUp, "Speed-up control must live inside WaveStatus.");

        Text speed = waveStatus.GetComponentsInChildren<Text>(true)
            .FirstOrDefault(t => t.text != null && t.text.EndsWith("x"));
        Assert.NotNull(speed, "Current combat speed must be displayed inside WaveStatus.");
        Assert.AreSame(waveStatus, speed.transform.parent);

        RectTransform downRect = speedDown as RectTransform;
        RectTransform upRect = speedUp as RectTransform;
        RectTransform speedRect = speed.rectTransform;
        Assert.NotNull(downRect);
        Assert.NotNull(upRect);
        Assert.Less(downRect.anchoredPosition.x, 0f);
        Assert.AreEqual(0f, speedRect.anchoredPosition.x, .01f, "Speed value should be centered in the wave block.");
        Assert.Greater(upRect.anchoredPosition.x, 0f);
        Assert.AreEqual(Mathf.Abs(downRect.anchoredPosition.x), Mathf.Abs(upRect.anchoredPosition.x), .01f, "Speed controls should be symmetric around center.");
    }
}
