using System.IO;
using NUnit.Framework;

namespace TheTroyGame.Tests
{
    public class BuildStampRuntimeVisualAuditTests
    {
        const string BuildCommandPath = "Assets/Editor/BuildPlayerCommand.cs";
        const string BuildOverlayPath = "Assets/Game/Core/BuildVersionOverlay.cs";
        const string RuntimeAuditPath = "Assets/Game/Characters/RuntimeVisualAudit.cs";
        const string HeroFactoryPath = "Assets/Game/Heroes/HeroVisualFactory.cs";
        const string EnemyFactoryPath = "Assets/Game/Enemies/EnemyVisualFactory.cs";

        [Test]
        public void WindowsBuild_WritesRuntimeStampWithoutProjectSettingsMutation()
        {
            string source = File.ReadAllText(BuildCommandPath);
            StringAssert.Contains("build_version.stamp", source);
            StringAssert.Contains("BuildVersionInfo.ComposeStampedVersion", source);
            StringAssert.Contains("TheTGame_VisibleMap_Data", source.Replace("PlayerName + \"_Data\"", "TheTGame_VisibleMap_Data"));
            StringAssert.DoesNotContain("PlayerSettings.bundleVersion =", source);
        }

        [Test]
        public void RuntimeBuildBadge_UsesActualCompactBuildIdentity()
        {
            Assert.IsTrue(File.Exists(BuildOverlayPath), "Missing runtime build version overlay.");
            string source = File.ReadAllText(BuildOverlayPath);
            StringAssert.Contains("RuntimeInitializeOnLoadMethod", source);
            StringAssert.Contains("BuildVersionInfo.CompactMenuBadge", source);
            StringAssert.DoesNotContain("PRE-ALPHA", source);
        }

        [Test]
        public void RuntimeVisualAudit_RecordsActualRendererMaterialState()
        {
            string source = File.ReadAllText(RuntimeAuditPath);
            StringAssert.Contains("ReportDetailed", source);
            StringAssert.Contains("ART_RENDER", source);
            StringAssert.Contains("ART_MATERIAL", source);
            StringAssert.Contains("shader.isSupported", source);
            StringAssert.Contains("GetPropertyBlock", source);
            StringAssert.Contains("SkinnedMeshRenderer", source);
            StringAssert.Contains("sharedMesh", source);
        }

        [Test]
        public void HeroAndEnemyFactories_AuditAfterFinalMaterialPass()
        {
            string hero = File.ReadAllText(HeroFactoryPath);
            string enemy = File.ReadAllText(EnemyFactoryPath);

            int heroRepair = hero.IndexOf("CharacterUrpMaterialAdapter.ApplyTo(result)", System.StringComparison.Ordinal);
            int heroAudit = hero.IndexOf("RuntimeVisualAudit.ReportDetailed", System.StringComparison.Ordinal);
            Assert.Greater(heroRepair, -1);
            Assert.Greater(heroAudit, heroRepair);

            int enemyRepair = enemy.IndexOf("CharacterUrpMaterialAdapter.ApplyTo(instance)", System.StringComparison.Ordinal);
            int enemyAudit = enemy.IndexOf("RuntimeVisualAudit.ReportDetailed", System.StringComparison.Ordinal);
            Assert.Greater(enemyRepair, -1);
            Assert.Greater(enemyAudit, enemyRepair);
        }
    }
}
