using System;
using NUnit.Framework;

public class GameScreenshotControllerTests
{
    [Test]
    public void ScreenshotFileName_IsTimestampedPng()
    {
        DateTime timestamp = new DateTime(2026, 9, 14, 16, 35, 42, 123);
        string fileName = GameScreenshotController.BuildFileName(timestamp);

        Assert.AreEqual("TheTroyGame_2026-09-14_16-35-42-123.png", fileName);
    }
}
