using System;
using NUnit.Framework;
using UnityEngine;

public class GameScreenshotControllerTests
{
    [Test]
    public void ScreenshotFileName_IsTimestampedPng()
    {
        DateTime timestamp = new DateTime(2026, 9, 14, 16, 35, 42, 123);
        string fileName = GameScreenshotController.BuildFileName(timestamp);

        Assert.AreEqual("TheTroyGame_2026-09-14_16-35-42-123.png", fileName);
    }

    [Test]
    public void ScreenshotButton_IsAnchoredRightCenterInsideViewport()
    {
        Assert.AreEqual(new Vector2(1f, .5f), GameScreenshotController.CaptureButtonAnchor);
        Assert.Less(GameScreenshotController.CaptureButtonOffset.x, 0f);
        Assert.AreEqual(0f, GameScreenshotController.CaptureButtonOffset.y);
        Assert.Greater(GameScreenshotController.CaptureButtonSize.x, 0f);
        Assert.Greater(GameScreenshotController.CaptureButtonSize.y, 0f);
    }
}
