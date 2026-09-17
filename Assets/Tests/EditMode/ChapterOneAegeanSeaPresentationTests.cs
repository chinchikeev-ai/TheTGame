using System.IO;
using NUnit.Framework;

namespace TheTroyGame.Tests
{
    public class ChapterOneAegeanSeaPresentationTests
    {
        const string SeaPresentation = "Assets/Game/World/ChapterOneAegeanSeaPresentation.cs";
        const string CoastClosure = "Assets/Game/World/ChapterOneCoastEdgeClosure.cs";

        [Test]
        public void SeaPresentation_UsesSingleContinuousOceanSurface()
        {
            string source = File.ReadAllText(SeaPresentation);
            StringAssert.Contains("Aegean Continuous Ocean", source);
            StringAssert.Contains("BuildContinuousOcean", source);
            StringAssert.DoesNotContain("Aegean Deep Transition", source);
            StringAssert.DoesNotContain("Aegean Blue Shelf", source);
            StringAssert.DoesNotContain("Aegean Turquoise Shelf", source);
            StringAssert.DoesNotContain("Aegean Lagoon Edge", source);
            StringAssert.DoesNotContain("Aegean Nearshore Light", source);
        }

        [Test]
        public void SeaPresentation_DisablesAllLegacyBaseSeaSlabs()
        {
            string source = File.ReadAllText(SeaPresentation);
            StringAssert.Contains("Deep Aegean Sea", source);
            StringAssert.Contains("Aegean Mid Water", source);
            StringAssert.Contains("Aegean Shallows", source);
            StringAssert.Contains("Coast Side Sea Extension", source);
            StringAssert.Contains("renderer.enabled = false", source);
        }

        [Test]
        public void SeaPresentation_AddsOnlyFineSurfCurrentsWaveletsAndGlints()
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
        public void SeaPresentation_DoesNotReintroducePrimitiveWaterBlobsOrGameplayGeometry()
        {
            string source = File.ReadAllText(SeaPresentation);
            StringAssert.Contains("CreateMeshObject", source);
            StringAssert.DoesNotContain("GameObject.CreatePrimitive", source);
            StringAssert.DoesNotContain("BuildPoint", source);
            StringAssert.DoesNotContain("Route_A", source);
            StringAssert.DoesNotContain("Route_B", source);
        }

        [Test]
        public void CoastClosure_KeepsLandExtensionOffOpenWater()
        {
            string source = File.ReadAllText(CoastClosure);
            StringAssert.Contains("new Vector3(5.2f, -.155f, z)", source);
            StringAssert.DoesNotContain("new Vector3(-1.5f, -.155f, z), new Vector3(45f", source);
        }
    }
}
