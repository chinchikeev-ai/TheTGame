using System.IO;
using NUnit.Framework;

namespace TheTroyGame.Tests
{
    public class ChapterOneGeneratedArtRecoveryTests
    {
        const string AutoBuilderPath = "Assets/Editor/CartoonCharacterAutoBuilder.cs";

        [Test]
        public void AutoBuilder_TracksEveryCoreChapterOneCharacterPrefab()
        {
            Assert.IsTrue(File.Exists(AutoBuilderPath), "Missing Chapter I art auto builder.");
            string source = File.ReadAllText(AutoBuilderPath);

            string[] required =
            {
                "Enemy_Infantry.prefab",
                "Enemy_Runner.prefab",
                "Enemy_HeavyHoplite.prefab",
                "Enemy_ShieldBearer.prefab",
                "Enemy_Archer.prefab",
                "Enemy_Boss.prefab",
                "Trojan_Infantry.prefab",
                "Trojan_Guard.prefab",
                "Trojan_Archer.prefab",
                "Hero_Hector.prefab",
                "Hero_Achilles.prefab",
                "Hero_Menelaus.prefab"
            };

            foreach (string asset in required)
                StringAssert.Contains(asset, source, "Auto builder does not track required core prefab: " + asset);
        }

        [Test]
        public void AutoBuilder_TracksEveryChapterOneSupportPrefab()
        {
            string source = File.ReadAllText(AutoBuilderPath);
            string[] required =
            {
                "Trojan_PriestApollo.prefab",
                "Trojan_FireKeeper.prefab",
                "Trojan_BallistaCrew.prefab",
                "Trojan_BallistaCrew_Engineer.prefab",
                "Trojan_BallistaCrew_Loader.prefab",
                "Mythic_Cyclops.prefab"
            };

            foreach (string asset in required)
                StringAssert.Contains(asset, source, "Auto builder does not track required support prefab: " + asset);
        }

        [Test]
        public void AutoBuilder_TracksAllChapterOneAnimationControllersAndHasValidationCommand()
        {
            string source = File.ReadAllText(AutoBuilderPath);
            string[] required =
            {
                "ChapterOneCharacter.controller",
                "ChapterOne_Spear.controller",
                "ChapterOne_Archer.controller",
                "ChapterOne_Skirmisher.controller",
                "ChapterOne_Hector.controller",
                "ChapterOne_Menelaus.controller",
                "ChapterOne_BallistaCrew.controller",
                "ChapterOne_PriestApollo.controller",
                "ChapterOne_FireKeeper.controller"
            };

            foreach (string asset in required)
                StringAssert.Contains(asset, source, "Auto builder does not track required controller: " + asset);

            StringAssert.Contains("Validate Generated Chapter I Art", source);
            StringAssert.Contains("ReportMissingAfterBuild", source);
        }
    }
}
