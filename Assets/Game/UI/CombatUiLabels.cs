public static class CombatUiLabels
{
    static string L(string en, string ru) => GameLanguage.T(en, ru);

    public static string ArchetypeLabel(EnemyArchetype archetype)
    {
        switch (archetype)
        {
            case EnemyArchetype.Runner: return L("RUNNER", "БЕГУН");
            case EnemyArchetype.HeavyHoplite: return L("HEAVY HOPLITE", "ТЯЖЁЛЫЙ ГОПЛИТ");
            case EnemyArchetype.ShieldBearer: return L("SHIELD BEARER", "ЩИТОНОСЕЦ");
            case EnemyArchetype.Archer: return L("ARCHER", "ЛУЧНИК");
            case EnemyArchetype.BatteringRam: return L("BATTERING RAM", "ТАРАН");
            case EnemyArchetype.Boss: return L("BOSS", "БОСС");
            default: return L("INFANTRY", "ПЕХОТА");
        }
    }

    public static string EnemyName(EnemyArchetype archetype)
    {
        switch (archetype)
        {
            case EnemyArchetype.Runner: return L("GREEK RUNNER", "ГРЕЧЕСКИЙ БЕГУН");
            case EnemyArchetype.HeavyHoplite: return L("HEAVY HOPLITE", "ТЯЖЁЛЫЙ ГОПЛИТ");
            case EnemyArchetype.ShieldBearer: return L("SHIELD BEARER", "ЩИТОНОСЕЦ");
            case EnemyArchetype.Archer: return L("GREEK ARCHER", "ГРЕЧЕСКИЙ ЛУЧНИК");
            case EnemyArchetype.BatteringRam: return L("BATTERING RAM", "ТАРАН");
            case EnemyArchetype.Boss: return L("MENELAUS", "МЕНЕЛАЙ");
            default: return L("GREEK INFANTRY", "ГРЕЧЕСКАЯ ПЕХОТА");
        }
    }
}
