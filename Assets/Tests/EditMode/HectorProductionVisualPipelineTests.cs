using System.IO;
using NUnit.Framework;

namespace TheTroyGame.Tests
{
    public class HectorProductionVisualPipelineTests
    {
        const string BuilderPath = "Assets/Editor/HectorProductionVisualRefinementBuilder.cs";
        const string AutoBuilderPath = "Assets/Editor/CartoonCharacterAutoBuilder.cs";
        const string ShieldBuilderPath = "Assets/Editor/ChapterOneShieldCandidateBuilder.cs";
        const string ArmorBuilderPath = "Assets/Editor/ChapterOneArmorCandidateBuilder.cs";
        const string EquipmentBuilderPath = "Assets/Editor/ChapterOneProductionEquipmentBuilder.cs";
        const string ValidatorPath = "Assets/Editor/HectorProductionVisualValidator.cs";

        [Test]
        public void HectorRefinement_UsesRigFollowingHeroSilhouetteParts()
        {
            Assert.IsTrue(File.Exists(BuilderPath), "Missing Hector production visual refinement builder.");
            string source = File.ReadAllText(BuilderPath);

            StringAssert.Contains("HumanBodyBones.UpperChest", source);
            StringAssert.Contains("HumanBodyBones.Hips", source);
            StringAssert.Contains("HumanBodyBones.LeftUpperArm", source);
            StringAssert.Contains("HumanBodyBones.RightUpperArm", source);
            StringAssert.Contains("HumanBodyBones.LeftLowerLeg", source);
            StringAssert.Contains("HumanBodyBones.RightLowerLeg", source);
            StringAssert.Contains("part.transform.SetParent(targetBone, true)", source);
        }

        [Test]
        public void HectorRefinement_CoversCanonicalHeroReadabilityCues()
        {
            Assert.IsTrue(File.Exists(BuilderPath), "Missing Hector production visual refinement builder.");
            string source = File.ReadAllText(BuilderPath);

            StringAssert.Contains("HectorRefinement_CapeCenter", source);
            StringAssert.Contains("HectorRefinement_PauldronLeft", source);
            StringAssert.Contains("HectorRefinement_PauldronRight", source);
            StringAssert.Contains("HectorRefinement_Pteruge_", source);
            StringAssert.Contains("HectorRefinement_GreaveLeft", source);
            StringAssert.Contains("HectorRefinement_GreaveRight", source);
            StringAssert.Contains("HeroCrest", source);
            StringAssert.Contains("TrojanRed", source);
            StringAssert.Contains("Gold", source);
        }

        [Test]
        public void HectorRefinement_RemovesLegacyRootSpaceCapeAndWaistParts()
        {
            Assert.IsTrue(File.Exists(BuilderPath), "Missing Hector production visual refinement builder.");
            string source = File.ReadAllText(BuilderPath);

            StringAssert.Contains("RemoveByName(root, \"HeroCape\")", source);
            StringAssert.Contains("RemoveDirectChild(kit, \"LeatherBelt\")", source);
            StringAssert.Contains("RemoveDirectChild(kit, \"FactionCloth\")", source);
        }

        [Test]
        public void HectorShieldRecovery_UsesRoundShieldHorseEmblemAndIsIdempotent()
        {
            Assert.IsTrue(File.Exists(ShieldBuilderPath), "Missing Chapter I shield candidate builder.");
            string source = File.ReadAllText(ShieldBuilderPath);

            StringAssert.Contains("ApplyHectorIfAvailable", source);
            StringAssert.Contains("AegeanRoundShield.obj", source);
            StringAssert.Contains("SourceShield_LateBronzeAge", source);
            StringAssert.Contains("HectorHorseBody", source);
            StringAssert.Contains("HectorHorseNeck", source);
            StringAssert.Contains("HectorHorseHead", source);
            StringAssert.Contains("HectorHorseLegFront", source);
            StringAssert.Contains("HectorHorseLegRear", source);
            StringAssert.Contains("HectorHorseTail", source);
            StringAssert.Contains("previousSource != null && oldShield == null", source);
        }

        [Test]
        public void HectorArmorRecovery_UsesAuthoredCuirassAndHelmetCandidates()
        {
            Assert.IsTrue(File.Exists(ArmorBuilderPath), "Missing Chapter I armor candidate builder.");
            string source = File.ReadAllText(ArmorBuilderPath);

            StringAssert.Contains("ApplyHectorIfAvailable", source);
            StringAssert.Contains("DendraCuirassCandidate.obj", source);
            StringAssert.Contains("BoarTuskHelmetCandidate.obj", source);
            StringAssert.Contains("SourceArmor_DendraCandidate", source);
            StringAssert.Contains("SourceHelmet_BoarTuskCandidate", source);
            StringAssert.Contains("new ArmorTarget(HectorPrefabPath, 1.15f, 1.10f)", source);
        }

        [Test]
        public void HectorScopedSpearRecovery_DoesNotInstallNetworkSource()
        {
            Assert.IsTrue(File.Exists(EquipmentBuilderPath), "Missing Chapter I production equipment builder.");
            string source = File.ReadAllText(EquipmentBuilderPath);

            int methodStart = source.IndexOf("public static bool ApplyHectorSpearIfSourceAvailable", System.StringComparison.Ordinal);
            int nextMethod = source.IndexOf("static bool UpgradeArcher", methodStart, System.StringComparison.Ordinal);
            Assert.GreaterOrEqual(methodStart, 0, "Missing scoped Hector spear recovery.");
            Assert.Greater(nextMethod, methodStart, "Could not isolate scoped Hector spear recovery body.");

            string methodBody = source.Substring(methodStart, nextMethod - methodStart);
            StringAssert.Contains("ChapterOneSpearSourceInstaller.LoadSpear()", methodBody);
            StringAssert.DoesNotContain("ChapterOneSpearSourceInstaller.Install()", methodBody);
            StringAssert.Contains("UpgradeSpearBearer(HectorPrefabPath, spearSource)", methodBody);
        }

        [Test]
        public void AutoBuilder_RebuildsHectorEquipmentBeforeSilhouetteAndAnimation()
        {
            Assert.IsTrue(File.Exists(AutoBuilderPath), "Missing Chapter I art auto builder.");
            string source = File.ReadAllText(AutoBuilderPath);

            int spear = source.IndexOf("ChapterOneProductionEquipmentBuilder.ApplyHectorSpearIfSourceAvailable", System.StringComparison.Ordinal);
            int shield = source.IndexOf("ChapterOneShieldCandidateBuilder.ApplyHectorIfAvailable", System.StringComparison.Ordinal);
            int armor = source.IndexOf("ChapterOneArmorCandidateBuilder.ApplyHectorIfAvailable", System.StringComparison.Ordinal);
            int visual = source.IndexOf("HectorProductionVisualRefinementBuilder.ApplyIfAvailable", System.StringComparison.Ordinal);
            int animation = source.IndexOf("HectorProductionAnimationBinder.ApplyIfAvailable", System.StringComparison.Ordinal);

            Assert.GreaterOrEqual(spear, 0, "Auto builder does not recover Hector's production spear.");
            Assert.Greater(shield, spear, "Hector shield should run after spear recovery.");
            Assert.Greater(armor, shield, "Hector armor should run after shield recovery.");
            Assert.Greater(visual, armor, "Hector silhouette refinement should run after authored equipment.");
            Assert.Greater(animation, visual, "Hector animation binding should run after visual recovery.");
        }

        [Test]
        public void AutoBuilder_CoreManifestIncludesHectorAndRebuildsMissingCoreSet()
        {
            Assert.IsTrue(File.Exists(AutoBuilderPath), "Missing Chapter I art auto builder.");
            string source = File.ReadAllText(AutoBuilderPath);

            StringAssert.Contains("/Heroes/Hero_Hector.prefab", source);
            StringAssert.Contains("List<string> missingCore = FindMissing<GameObject>(CoreCharacterPrefabs)", source);
            StringAssert.Contains("if (missingCore.Count > 0)", source);
            StringAssert.Contains("CartoonCharacterPrefabBuilder.BuildAll()", source);
        }

        [Test]
        public void HectorValidator_RequiresAllProductionVisualElements()
        {
            Assert.IsTrue(File.Exists(ValidatorPath), "Missing Hector production visual validator.");
            string source = File.ReadAllText(ValidatorPath);

            string[] requirements =
            {
                "SourceSpear_Quaternius_MedievalWeapons",
                "Socket_SpearRelease",
                "SourceShield_LateBronzeAge",
                "HectorHorseBody",
                "SourceArmor_DendraCandidate",
                "SourceHelmet_BoarTuskCandidate",
                "HectorRefinement_CapeCenter",
                "HectorRefinement_PauldronLeft",
                "HectorRefinement_Pteruge_4",
                "HectorRefinement_GreaveLeft",
                "HectorRefinement_CrestGoldBase",
                "runtimeAnimatorController"
            };

            foreach (string requirement in requirements)
                StringAssert.Contains(requirement, source, "Validator does not require: " + requirement);

            string autoBuilder = File.ReadAllText(AutoBuilderPath);
            StringAssert.Contains("HectorProductionVisualValidator.CollectProblems()", autoBuilder);
        }
    }
}
