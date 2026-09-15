using System.IO;
using NUnit.Framework;

namespace TheTroyGame.Tests
{
    public class HectorProductionAnimationPipelineTests
    {
        const string BinderPath = "Assets/Editor/HectorProductionAnimationBinder.cs";
        const string AutoBuilderPath = "Assets/Editor/CartoonCharacterAutoBuilder.cs";

        [Test]
        public void Binder_CoversEveryHectorCombatAnimationState()
        {
            Assert.IsTrue(File.Exists(BinderPath), "Missing Hector production animation binder.");
            string source = File.ReadAllText(BinderPath);

            StringAssert.Contains("\"Poke\"", source);
            StringAssert.Contains("\"Block\"", source);
            StringAssert.Contains("\"AbilityQ\"", source);
            StringAssert.Contains("\"AbilityE\"", source);
            StringAssert.Contains("\"ShieldHold\"", source);
            StringAssert.Contains("\"AbilityR\"", source);
            StringAssert.Contains("\"AbilityF\"", source);
            StringAssert.Contains("\"Downed\"", source);
        }

        [Test]
        public void Binder_PrioritizesThrowHeavyBlockCheerAndPersistentDownedSemantics()
        {
            Assert.IsTrue(File.Exists(BinderPath), "Missing Hector production animation binder.");
            string source = File.ReadAllText(BinderPath);

            StringAssert.Contains("\"cheer\"", source);
            StringAssert.Contains("\"block\"", source);
            StringAssert.Contains("\"throw\"", source);
            StringAssert.Contains("\"heavy attack\"", source);
            StringAssert.Contains("\"laying down idle\"", source);
            StringAssert.Contains("/Knight.fbx", source);
        }

        [Test]
        public void AutoBuilder_RebuildsMissingHectorControllerAndRunsBinder()
        {
            Assert.IsTrue(File.Exists(AutoBuilderPath), "Missing Chapter I art auto builder.");
            string source = File.ReadAllText(AutoBuilderPath);

            StringAssert.Contains("ChapterOne_Hector.controller", source);
            StringAssert.Contains("HectorProductionAnimationBinder.ApplyIfAvailable()", source);
        }
    }
}
