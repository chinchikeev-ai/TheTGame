using System.IO;
using NUnit.Framework;

public class CampaignSaveTests
{
    string testRoot;

    [SetUp]
    public void SetUp()
    {
        testRoot = Path.Combine(Path.GetTempPath(), "TheTroyGame_SaveTests", System.Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(testRoot);
        CampaignSave.ConfigureStorageForTests(testRoot);
        CampaignSave.ResetProgress();
    }

    [TearDown]
    public void TearDown()
    {
        CampaignSave.ClearStorageOverrideForTests();
        if (Directory.Exists(testRoot)) Directory.Delete(testRoot, true);
    }

    [Test]
    public void Save_RoundTrip_PreservesCampaignState()
    {
        CampaignSave.SetDifficulty(CampaignDifficulty.Legendary);
        CampaignSave.RecordChapterResult(1, 12345, 712f, 14, 2);
        CampaignSave.SetNarrativeChoice("test.choice", "alpha");
        CampaignSave.SetModifier("test.modifier", 2.5f);
        CampaignSave.SetFinalResult(50000, 120, 3, 8, 7200f, "A", "troy_burns");

        CampaignSave.Reload();

        Assert.AreEqual(CampaignDifficulty.Legendary, CampaignSave.Difficulty);
        Assert.IsTrue(CampaignSave.IsUnlocked(2));
        Assert.AreEqual(12345, CampaignSave.GetBestScore(1));
        Assert.AreEqual("alpha", CampaignSave.GetNarrativeChoice("test.choice"));
        Assert.AreEqual(2.5f, CampaignSave.GetModifier("test.modifier"), .001f);
        Assert.IsTrue(CampaignSave.FinalResult.completed);
        Assert.AreEqual(50000, CampaignSave.FinalResult.totalScore);
    }

    [Test]
    public void CorruptPrimary_LoadsBackup()
    {
        CampaignSave.RecordChapterResult(1, 1000, 700f, 10, 2);
        CampaignSave.SetDifficulty(CampaignDifficulty.Strategos); // creates backup of prior valid save

        File.WriteAllText(CampaignSave.CurrentSavePath, "{this-is-not-json");
        CampaignSave.ResetRuntimeCacheForTests();

        Assert.DoesNotThrow(() => CampaignSave.Reload());
        Assert.IsTrue(CampaignSave.IsUnlocked(2));
        Assert.AreEqual(1000, CampaignSave.GetBestScore(1));
    }

    [Test]
    public void ResetProgress_ReturnsCanonicalDefaults()
    {
        CampaignSave.RecordChapterResult(1, 9999, 600f, 20, 2);
        CampaignSave.SetDifficulty(CampaignDifficulty.Legendary);

        CampaignSave.ResetProgress();

        Assert.AreEqual(1, CampaignSave.UnlockedChapter);
        Assert.AreEqual(CampaignDifficulty.Story, CampaignSave.Difficulty);
        Assert.AreEqual(0, CampaignSave.GetBestScore(1));
        Assert.IsFalse(CampaignSave.FinalResult.completed);
    }

    [Test]
    public void ChapterSixChoices_RoundTripIntoChapterSevenModifiers()
    {
        CampaignSave.SaveHorseChapterChoices(true, false, true, 4, 7.5f);
        CampaignSave.Reload();

        Assert.AreEqual("1", CampaignSave.GetNarrativeChoice("chapter6.reinforced_inner_city"));
        Assert.AreEqual("0", CampaignSave.GetNarrativeChoice("chapter6.repaired_gate"));
        Assert.AreEqual("1", CampaignSave.GetNarrativeChoice("chapter6.prepared_fire_defense"));
        Assert.AreEqual(4f, CampaignSave.GetModifier("chapter7.evacuation_readiness"));
        Assert.AreEqual(7.5f, CampaignSave.GetModifier("chapter7.internal_spawn_delay_bonus"));
    }
}
