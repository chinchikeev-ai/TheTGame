using System.IO;
using NUnit.Framework;

namespace TheTroyGame.Tests
{
    public class HectorProductionVisualPipelineTests
    {
        const string BuilderPath = "Assets/Editor/HectorProductionVisualRefinementBuilder.cs";
        const string AutoBuilderPath = "Assets/Editor/CartoonCharacterAutoBuilder.cs";

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
        public void AutoBuilder_AppliesHectorVisualRefinementBeforeAnimationBinding()
        {
            Assert.IsTrue(File.Exists(AutoBuilderPath), "Missing Chapter I art auto builder.");
            string source = File.ReadAllText(AutoBuilderPath);

            int visual = source.IndexOf("HectorProductionVisualRefinementBuilder.ApplyIfAvailable()", System.StringComparison.Ordinal);
            int animation = source.IndexOf("HectorProductionAnimationBinder.ApplyIfAvailable()", System.StringComparison.Ordinal);
            Assert.GreaterOrEqual(visual, 0, "Auto builder does not call Hector visual refinement.");
            Assert.Greater(animation, visual, "Hector animation binding should run after the visual refinement pass.");
        }
    }
}
