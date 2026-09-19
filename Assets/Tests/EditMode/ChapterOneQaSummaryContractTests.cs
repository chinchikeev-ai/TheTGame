using System.IO;
using NUnit.Framework;

public class ChapterOneQaSummaryContractTests
{
    const string ValidatorPath = "Assets/Editor/ChapterOneGameplayFreezeValidator.cs";
    const string AutoReporterPath = "Assets/Editor/ChapterOneQaSummaryAutoReporter.cs";

    [Test]
    public void GameplayFreezeValidator_WritesHumanAndMachineQaSummaries()
    {
        Assert.IsTrue(File.Exists(ValidatorPath), $"Missing {ValidatorPath}");
        string source = File.ReadAllText(ValidatorPath);

        StringAssert.Contains("ChapterI_QA_Summary_", source);
        StringAssert.Contains(".md", source);
        StringAssert.Contains(".json", source);
        StringAssert.Contains("canBindAcceptance", source);
        StringAssert.Contains("mainReason", source);
        StringAssert.Contains("nextAction", source);
        StringAssert.Contains("VERDICT:", source);
        StringAssert.Contains("Почему:", source);
        StringAssert.Contains("Что делать:", source);
        StringAssert.Contains("FpsWarningThreshold = 45f", source);
    }

    [Test]
    public void QaSummaryAutoReporter_PersistsPendingReportAcrossPlayModeReload()
    {
        Assert.IsTrue(File.Exists(AutoReporterPath), $"Missing {AutoReporterPath}");
        string source = File.ReadAllText(AutoReporterPath);

        StringAssert.Contains("PlayModeStateChange.ExitingPlayMode", source);
        StringAssert.Contains("PlayModeStateChange.EnteredEditMode", source);
        StringAssert.Contains("SessionState.SetString(PendingReportKey", source);
        StringAssert.Contains("SessionState.GetString(PendingReportKey", source);
        StringAssert.Contains("ChapterOneGameplayFreezeValidator.Check(reportPath, false)", source);
    }
}
