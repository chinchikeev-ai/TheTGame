using NUnit.Framework;

public class LocalizationContractTests
{
    bool previousRussian;

    [SetUp]
    public void SetUp()
    {
        previousRussian = GameLanguage.Russian;
    }

    [TearDown]
    public void TearDown()
    {
        GameLanguage.SetRussian(previousRussian);
    }

    [Test]
    public void RussianCombatLabels_DoNotExposeRawEnglishEnums()
    {
        GameLanguage.SetRussian(true);

        Assert.AreEqual("ТЯЖЁЛЫЙ ГОПЛИТ", CombatUiLabels.ArchetypeLabel(EnemyArchetype.HeavyHoplite));
        Assert.AreEqual("ЩИТОНОСЕЦ", CombatUiLabels.ArchetypeLabel(EnemyArchetype.ShieldBearer));
        Assert.AreEqual("МЕНЕЛАЙ", CombatUiLabels.EnemyName(EnemyArchetype.Boss));
        Assert.AreEqual("СИЛЬНЕЙШИЙ", CombatHudTowerCatalog.TargetPriorityLabel(TargetPriority.Strongest));
        StringAssert.Contains("ДАЛЬНИЙ", CombatHudTowerCatalog.TowerTags(TowerType.MachineGun));
        Assert.AreEqual("ЛЕГЕНДАРНАЯ", DifficultyRules.Label(CampaignDifficulty.Legendary));
    }

    [Test]
    public void EnglishCombatLabels_RemainCanonical()
    {
        GameLanguage.SetRussian(false);

        Assert.AreEqual("HEAVY HOPLITE", CombatUiLabels.ArchetypeLabel(EnemyArchetype.HeavyHoplite));
        Assert.AreEqual("MENELAUS", CombatUiLabels.EnemyName(EnemyArchetype.Boss));
        Assert.AreEqual("STRONGEST", CombatHudTowerCatalog.TargetPriorityLabel(TargetPriority.Strongest));
        StringAssert.Contains("RANGED", CombatHudTowerCatalog.TowerTags(TowerType.MachineGun));
        Assert.AreEqual("LEGENDARY", DifficultyRules.Label(CampaignDifficulty.Legendary));
    }
}
