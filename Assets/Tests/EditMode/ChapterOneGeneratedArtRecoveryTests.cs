using System.IO;
using NUnit.Framework;

namespace TheTroyGame.Tests
{
    public class ChapterOneGeneratedArtRecoveryTests
    {
        const string AutoBuilderPath = "Assets/Editor/CartoonCharacterAutoBuilder.cs";
        const string CharacterBuilderPath = "Assets/Editor/CartoonCharacterPrefabBuilder.cs";
        const string AnimationBuilderPath = "Assets/Editor/ChapterOneCharacterAnimationBuilder.cs";
        const string MaterialRepairPath = "Assets/Editor/ChapterOneUrpMaterialRepair.cs";
        const string MaterialAdapterPath = "Assets/Game/World/CharacterUrpMaterialAdapter.cs";
        const string HeroFactoryPath = "Assets/Game/Heroes/HeroVisualFactory.cs";
        const string EnemyFactoryPath = "Assets/Game/Enemies/EnemyVisualFactory.cs";
        const string TowerFactoryPath = "Assets/Game/Towers/TowerFactory.cs";
        const string TowerBinderPath = "Assets/Game/Towers/TowerProductionArtBinder.cs";

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

        [Test]
        public void AutoBuilder_RepairsAnimatorBindingsEvenWhenControllersAlreadyExist()
        {
            string source = File.ReadAllText(AutoBuilderPath);
            StringAssert.Contains("ChapterOneCharacterAnimationBuilder.RepairControllerAssignments(false)", source);
            StringAssert.Contains("ChapterOneCharacterAnimationBuilder.CollectAnimatorBindingProblems()", source);
        }

        [Test]
        public void AnimationBuilder_CreatesMissingAnimatorInsteadOfSkippingPrefab()
        {
            Assert.IsTrue(File.Exists(AnimationBuilderPath), "Missing Chapter I animation builder.");
            string source = File.ReadAllText(AnimationBuilderPath);

            StringAssert.DoesNotContain("if (animator == null) continue;", source);
            StringAssert.Contains("EnsureAnimator(root, path, out prefabChanged)", source);
            StringAssert.Contains("host.AddComponent<Animator>()", source);
            StringAssert.Contains("root.transform.Find(\"Visual\")", source);
        }

        [Test]
        public void AnimationBuilder_RecoversAvatarAndKeepsRootMotionDisabled()
        {
            string source = File.ReadAllText(AnimationBuilderPath);

            StringAssert.Contains("FindImportedAvatar(root)", source);
            StringAssert.Contains("AssetDatabase.GetAssetPath(renderer.sharedMesh)", source);
            StringAssert.Contains("avatar.isValid", source);
            StringAssert.Contains("animator.applyRootMotion = false", source);
        }

        [Test]
        public void AnimationBuilder_ExposesRepairAndValidationForExistingPrefabs()
        {
            string source = File.ReadAllText(AnimationBuilderPath);

            StringAssert.Contains("Repair Chapter I Animator Bindings", source);
            StringAssert.Contains("public static int RepairControllerAssignments", source);
            StringAssert.Contains("public static List<string> CollectAnimatorBindingProblems", source);
            StringAssert.Contains("runtimeAnimatorController == null", source);
        }

        [Test]
        public void RuntimeCharacterFactories_ApplyUrpMaterialRecoveryAfterAssembly()
        {
            Assert.IsTrue(File.Exists(MaterialAdapterPath), "Missing runtime character material adapter.");
            string adapter = File.ReadAllText(MaterialAdapterPath);
            StringAssert.Contains("public static int ApplyTo(GameObject root)", adapter);
            StringAssert.Contains("ApplyNow(true)", adapter);

            StringAssert.Contains("CharacterUrpMaterialAdapter.ApplyTo(result)", File.ReadAllText(HeroFactoryPath));
            StringAssert.Contains("CharacterUrpMaterialAdapter.ApplyTo(instance)", File.ReadAllText(EnemyFactoryPath));
            StringAssert.Contains("CharacterUrpMaterialAdapter.ApplyTo(root)", File.ReadAllText(TowerFactoryPath));
            StringAssert.Contains("CharacterUrpMaterialAdapter.ApplyTo(crew)", File.ReadAllText(TowerBinderPath));
        }

        [Test]
        public void RuntimeCharacterMaterialRecovery_ForcesStableTemplateAndRepeatsAfterAssembly()
        {
            string adapter = File.ReadAllText(MaterialAdapterPath);

            StringAssert.Contains("const int DeferredPassCount = 2", adapter);
            StringAssert.Contains("void LateUpdate()", adapter);
            StringAssert.Contains("ApplyNow(true)", adapter);
            StringAssert.Contains("forceStableCharacterMaterial", adapter);
            StringAssert.Contains("source == baseTemplate", adapter);
            StringAssert.Contains("new Material(baseTemplate)", adapter);
            StringAssert.Contains("ConvertedSuffix", adapter);
        }

        [Test]
        public void RuntimeCharacterMaterialRecovery_FillsNullRendererMaterialSlots()
        {
            string adapter = File.ReadAllText(MaterialAdapterPath);

            StringAssert.Contains("RequiredMaterialSlots(renderer)", adapter);
            StringAssert.Contains("if (source == null)", adapter);
            StringAssert.Contains("GetMissingSlotMaterial(baseTemplate, renderer, slot)", adapter);
            StringAssert.Contains("ResolveMissingSlotColor(renderer, identity)", adapter);
        }

        [Test]
        public void CharacterBuilder_DoesNotPersistKayKitShadersIntoGeneratedPrefabs()
        {
            Assert.IsTrue(File.Exists(CharacterBuilderPath), "Missing character prefab builder.");
            string builder = File.ReadAllText(CharacterBuilderPath);
            StringAssert.DoesNotContain("new Material(source)", builder);
            StringAssert.Contains("ChapterOneUrpMaterialRepair.StabilizeMaterials(root, prefabName, tint)", builder);
        }

        [Test]
        public void MaterialRepair_BakesProjectOwnedMaterialsAndValidatesEveryRendererSlot()
        {
            Assert.IsTrue(File.Exists(MaterialRepairPath), "Missing Chapter I URP material repair.");
            string repair = File.ReadAllText(MaterialRepairPath);

            StringAssert.Contains("Assets/Game/Art/Characters/Resources/TroyProduction/Materials", repair);
            StringAssert.Contains("Assets/Resources/RuntimeColorMaterial.mat", repair);
            StringAssert.Contains("AssetDatabase.CreateAsset(stable, assetPath)", repair);
            StringAssert.Contains("renderer.sharedMaterials = materials", repair);
            StringAssert.Contains("uses external/non-stabilized material", repair);
            StringAssert.Contains("does not use RuntimeColorMaterial shader", repair);
            StringAssert.Contains("SanitizeColor(ReadColor(source))", repair);
            StringAssert.Contains("source != null ? SanitizeColor(ReadColor(source)) : ResolveMissingSlotColor(renderer, identity)", repair);
            StringAssert.Contains("RequiredMaterialSlots(renderer)", repair);
        }
    }
}
