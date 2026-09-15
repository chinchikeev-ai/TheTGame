using System.IO;
using NUnit.Framework;

namespace TheTroyGame.Tests
{
    public class UnifiedEditorMenuCompatibilityTests
    {
        const string CompatibilityPath = "Assets/Editor/UnifiedEditorMenuCompatibility.cs";

        [Test]
        public void UnifiedMenu_UsesSingleCanonicalRoot()
        {
            Assert.IsTrue(File.Exists(CompatibilityPath), "Missing unified editor menu compatibility layer.");
            string source = File.ReadAllText(CompatibilityPath);

            StringAssert.Contains("const string Root = \"The Troy Game/\"", source);
            StringAssert.DoesNotContain("Tools/TheTroyGame/", source);
            StringAssert.DoesNotContain("Tools/The Troy Game/", source);
        }

        [Test]
        public void UnifiedMenu_RemovesLegacyTopLevelEntries()
        {
            Assert.IsTrue(File.Exists(CompatibilityPath), "Missing unified editor menu compatibility layer.");
            string source = File.ReadAllText(CompatibilityPath);

            StringAssert.Contains("LegacyMenuPaths", source);
            StringAssert.Contains("RemoveMenuItem", source);
            StringAssert.Contains("remove.Invoke(null, new object[] { \"TheTroyGame\" })", source);
        }

        [Test]
        public void UnifiedMenu_ExposesValidationDataAndArtUnderCanonicalRoot()
        {
            Assert.IsTrue(File.Exists(CompatibilityPath), "Missing unified editor menu compatibility layer.");
            string source = File.ReadAllText(CompatibilityPath);

            StringAssert.Contains("Validation/Run Architecture Smoke Checks", source);
            StringAssert.Contains("Data/Create Missing Default Assets", source);
            StringAssert.Contains("Validation/Audit Chapter I Art Freeze", source);
            StringAssert.Contains("Validation/Audit Campaign Models", source);
        }
    }
}
