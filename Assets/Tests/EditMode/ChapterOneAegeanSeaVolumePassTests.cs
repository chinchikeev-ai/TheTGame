using System.IO;
using NUnit.Framework;

namespace TheTroyGame.Tests
{
    public class ChapterOneAegeanSeaVolumePassTests
    {
        const string VolumePass = "Assets/Game/World/ChapterOneAegeanSeaVolumePass.cs";

        [Test]
        public void VolumePass_AddsRaisedBreakersRockWashAndShipWakes()
        {
            string source = File.ReadAllText(VolumePass);
            StringAssert.Contains("Raised Aegean Breaker Body", source);
            StringAssert.Contains("Raised Aegean White Crest", source);
            StringAssert.Contains("Boulder Wash Foam", source);
            StringAssert.Contains("Greek Landing Ship Hero Wake", source);
            StringAssert.Contains("Ship Wake V Arm", source);
        }

        [Test]
        public void VolumePass_UsesContinuousMeshRibbons()
        {
            string source = File.ReadAllText(VolumePass);
            StringAssert.Contains("CreatePolylineRibbon", source);
            StringAssert.Contains("CreateCoastRibbon", source);
            StringAssert.Contains("CreateArcRibbon", source);
            StringAssert.Contains("new Mesh", source);
            StringAssert.DoesNotContain("GameObject.CreatePrimitive", source);
        }

        [Test]
        public void VolumePass_UsesExistingAmbientMotionSystem()
        {
            string source = File.ReadAllText(VolumePass);
            StringAssert.Contains("MotionKind.Sea", source);
            StringAssert.Contains("MotionKind.Surf", source);
            StringAssert.Contains("MotionKind.Swell", source);
        }

        [Test]
        public void VolumePass_RemainsPresentationOnly()
        {
            string source = File.ReadAllText(VolumePass);
            StringAssert.DoesNotContain("BuildPoint", source);
            StringAssert.DoesNotContain("Route_A", source);
            StringAssert.DoesNotContain("Route_B", source);
            StringAssert.DoesNotContain("Collider", source);
        }
    }
}
