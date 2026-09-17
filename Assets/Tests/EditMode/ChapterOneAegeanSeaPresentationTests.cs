using System.IO;
using NUnit.Framework;

namespace TheTroyGame.Tests
{
    public class ChapterOneAegeanSeaPresentationTests
    {
        const string SeaPresentation = "Assets/Game/World/ChapterOneAegeanSeaPresentation.cs";

        [Test]
        public void SeaPresentation_UsesDepthSeparatedAegeanPalette()
        {
            string source = File.ReadAllText(SeaPresentation);
            StringAssert.Contains("Deep Cobalt Water", source);
            StringAssert.Contains("Open Azure Water", source);
            StringAssert.Contains("Turquoise Shore Water", source);
            StringAssert.Contains("Lagoon Light Patch", source);
        }

        [Test]
        public void SeaPresentation_AddsAnimatedSurfAndReadableWaveDetail()
        {
            string source = File.ReadAllText(SeaPresentation);
            StringAssert.Contains("Aegean Breaking Foam", source);
            StringAssert.Contains("Open Water Wave Cap", source);
            StringAssert.Contains("Aegean Current Ribbon", source);
            StringAssert.Contains("Aegean Sun Glint", source);
            StringAssert.Contains("MotionKind.Surf", source);
            StringAssert.Contains("MotionKind.Swell", source);
        }

        [Test]
        public void SeaPresentation_RemainsPresentationOnly()
        {
            string source = File.ReadAllText(SeaPresentation);
            StringAssert.Contains("Destroy(collider)", source);
            StringAssert.DoesNotContain("BuildPoint", source);
            StringAssert.DoesNotContain("Route_A", source);
            StringAssert.DoesNotContain("Route_B", source);
        }
    }
}