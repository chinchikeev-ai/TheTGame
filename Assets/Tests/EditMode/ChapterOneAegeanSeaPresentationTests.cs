using System.IO;
using NUnit.Framework;

namespace TheTroyGame.Tests
{
    public class ChapterOneAegeanSeaPresentationTests
    {
        const string SeaPresentation = "Assets/Game/World/ChapterOneAegeanSeaPresentation.cs";
        const string CoastClosure = "Assets/Game/World/ChapterOneCoastEdgeClosure.cs";

        [Test]
        public void SeaPresentation_UsesOneBaseOceanWithCoastFollowingDepthBands()
        {
            string source = File.ReadAllText(SeaPresentation);
            StringAssert.Contains("OceanBase", source);
            StringAssert.Contains("Aegean Deep Transition", source);
            StringAssert.Contains("Aegean Blue Shelf", source);
            StringAssert.Contains("Aegean Turquoise Shelf", source);
            StringAssert.Contains("Aegean Lagoon Edge", source);
            StringAssert.Contains("Aegean Nearshore Light", source);
        }

        [Test]
        public void SeaPresentation_AddsAnimatedSurfCurrentsWaveletsAndGlints()
        {
            string source = File.ReadAllText(SeaPresentation);
            StringAssert.Contains("Aegean Breaking Foam", source);
            StringAssert.Contains("Aegean Small Wave", source);
            StringAssert.Contains("Aegean Current Ribbon", source);
            StringAssert.Contains("Aegean Sun Glint", source);
            StringAssert.Contains("MotionKind.Surf", source);
            StringAssert.Contains("MotionKind.Sea", source);
        }

        [Test]
        public void SeaPresentation_DoesNotReintroducePrimitiveWaterBlobs()
        {
            string source = File.ReadAllText(SeaPresentation);
            StringAssert.Contains("CreateMeshObject", source);
            StringAssert.DoesNotContain("GameObject.CreatePrimitive", source);
            StringAssert.DoesNotContain("BuildPoint", source);
            StringAssert.DoesNotContain("Route_A", source);
            StringAssert.DoesNotContain("Route_B", source);
        }

        [Test]
        public void CoastClosure_SeparatesSeaAndLandInsteadOfCoveringSeaWithSand()
        {
            string source = File.ReadAllText(CoastClosure);
            StringAssert.Contains("Coast Side Sea Extension", source);
            StringAssert.Contains("new Vector3(-25.5f, -.31f, z)", source);
            StringAssert.Contains("new Vector3(5.2f, -.155f, z)", source);
            StringAssert.DoesNotContain("new Vector3(-1.5f, -.155f, z), new Vector3(45f", source);
        }
    }
}
