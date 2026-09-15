using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace TheTroyGame.Tests
{
    public class UnifiedEditorMenuCompatibilityTests
    {
        const string EditorRoot = "Assets/Editor";
        const string CompatibilityPath = "Assets/Editor/UnifiedEditorMenuCompatibility.cs";
        static readonly Regex LiteralMenuItem = new Regex("\\[MenuItem\\(\\\"([^\\\"]+)\\\"", RegexOptions.Compiled);

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
        public void ProjectMenuItems_DoNotUseToolsLegacyRoots()
        {
            foreach ((string file, string path) in ReadLiteralMenuItems())
            {
                Assert.IsFalse(path.StartsWith("Tools/TheTroyGame/", StringComparison.Ordinal),
                    $"Legacy Tools/TheTroyGame menu path found in {file}: {path}");
                Assert.IsFalse(path.StartsWith("Tools/The Troy Game/", StringComparison.Ordinal),
                    $"Legacy Tools/The Troy Game menu path found in {file}: {path}");
            }
        }

        [Test]
        public void EveryRemainingLegacyTopLevelMenu_IsCoveredByCompatibilityLayer()
        {
            Assert.IsTrue(File.Exists(CompatibilityPath), "Missing unified editor menu compatibility layer.");
            string compatibility = File.ReadAllText(CompatibilityPath);
            List<string> uncovered = new List<string>();

            foreach ((string file, string path) in ReadLiteralMenuItems())
            {
                if (!path.StartsWith("TheTroyGame/", StringComparison.Ordinal)) continue;
                if (file.Replace('\\', '/').EndsWith("/UnifiedEditorMenuCompatibility.cs", StringComparison.Ordinal)) continue;
                if (!compatibility.Contains("\"" + path + "\"", StringComparison.Ordinal))
                    uncovered.Add(file + " :: " + path);
            }

            Assert.IsEmpty(uncovered,
                "Legacy TheTroyGame MenuItem declarations must be migrated or explicitly covered by UnifiedEditorMenuCompatibility:\n" +
                string.Join("\n", uncovered));
        }

        [Test]
        public void UnifiedMenu_RemovesLegacyTopLevelEntries()
        {
            string source = File.ReadAllText(CompatibilityPath);
            StringAssert.Contains("LegacyMenuPaths", source);
            StringAssert.Contains("RemoveMenuItem", source);
            StringAssert.Contains("remove.Invoke(null, new object[] { \"TheTroyGame\" })", source);
        }

        [Test]
        public void UnifiedMenu_ExposesValidationDataAndArtUnderCanonicalRoot()
        {
            string source = File.ReadAllText(CompatibilityPath);
            StringAssert.Contains("Validation/Run Architecture Smoke Checks", source);
            StringAssert.Contains("Data/Create Missing Default Assets", source);
            StringAssert.Contains("Validation/Audit Chapter I Art Freeze", source);
            StringAssert.Contains("Validation/Audit Campaign Models", source);
        }

        static IEnumerable<(string file, string path)> ReadLiteralMenuItems()
        {
            Assert.IsTrue(Directory.Exists(EditorRoot), "Assets/Editor is missing.");
            return Directory.GetFiles(EditorRoot, "*.cs", SearchOption.AllDirectories)
                .OrderBy(path => path, StringComparer.Ordinal)
                .SelectMany(file => LiteralMenuItem.Matches(File.ReadAllText(file))
                    .Cast<Match>()
                    .Select(match => (file, match.Groups[1].Value)));
        }
    }
}
