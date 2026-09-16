using System.IO;
using NUnit.Framework;

namespace TheTroyGame.Tests
{
    public class ChapterOneBattlefieldPolishTests
    {
        const string WallLife = "Assets/Game/World/ChapterOneWallLife.cs";
        const string CoastClosure = "Assets/Game/World/ChapterOneCoastEdgeClosure.cs";
        const string CompactUi = "Assets/Game/UI/ChapterOneUiCompactPresentation.cs";
        const string TowerPlacement = "Assets/Game/Towers/TowerPlacement.cs";
        const string HectorInput = "Assets/Game/Heroes/Hector/HectorInputDriver.cs";
        const string EnemyRoot = "Assets/Resources/Data/Enemies/";

        [Test]
        public void WallGuards_ApplyCharacterMaterialRecovery()
        {
            string source = File.ReadAllText(WallLife);
            StringAssert.Contains("CharacterUrpMaterialAdapter.ApplyTo(guard)", source);
        }

        [Test]
        public void CoastClosure_CoversNorthAndSouthWithoutReplacingAuthoredShoreline()
        {
            string source = File.ReadAllText(CoastClosure);
            StringAssert.Contains("Chapter01_CoastEnvironment", source);
            StringAssert.Contains("BuildEdge(root.transform, -17.4f, -1f)", source);
            StringAssert.Contains("BuildEdge(root.transform, 17.4f, 1f)", source);
            StringAssert.Contains("Coast Side Sand Extension", source);
        }

        [Test]
        public void DivinePower_IsForcedToBottomRightAboveDefenseControl()
        {
            string source = File.ReadAllText(CompactUi);
            StringAssert.Contains("DivinePowerActions", source);
            StringAssert.Contains("SetBottomRight(divinePower, new Vector2(-24f, 156f))", source);
            StringAssert.Contains("new Vector2(1f, 0f)", source);
        }

        [Test]
        public void SecondaryPointer_ClearsTowerAndHectorSelection()
        {
            string tower = File.ReadAllText(TowerPlacement);
            StringAssert.Contains("if (GameInput.SecondaryPressed())", tower);
            StringAssert.Contains("ClearSelection();", tower);
            StringAssert.Contains("SelectedTower = null;", tower);

            string hector = File.ReadAllText(HectorInput);
            StringAssert.Contains("if (GameInput.SecondaryPressed())", hector);
            StringAssert.Contains("hector.SetSelected(false);", hector);
            StringAssert.DoesNotContain("MoveSelectedHector", hector);
        }

        [TestCase("Infantry.asset", "hpMultiplier: 1.5")]
        [TestCase("Runner.asset", "hpMultiplier: 0.975")]
        [TestCase("HeavyHoplite.asset", "hpMultiplier: 4.8")]
        [TestCase("ShieldBearer.asset", "hpMultiplier: 3.15")]
        [TestCase("Archer.asset", "hpMultiplier: 1.35")]
        [TestCase("BatteringRam.asset", "hpMultiplier: 8.25")]
        [TestCase("Boss.asset", "hpMultiplier: 22.5")]
        public void EnemyHealth_IsRaisedByFiftyPercent(string assetName, string expected)
        {
            StringAssert.Contains(expected, File.ReadAllText(EnemyRoot + assetName));
        }
    }
}
