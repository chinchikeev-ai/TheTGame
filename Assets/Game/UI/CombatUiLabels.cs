public static class CombatUiLabels
{
    static string L(string en, string ru) => GameLanguage.T(en, ru);

    public static string EnemyArchetype(EnemyArchetype archetype)
    {
        switch (archetype)
        {
            case global::EnemyArchetype.Runner: return L("RUNNER", "БЕГУН");
            case global::EnemyArchetype.HeavyHoplite: return L("HEAVY HOPLITE", "ТЯЖЁЛЫЙ ГОПЛИТ");
            case global::EnemyArchetype.ShieldBearer: return L("SHIELD BEARER", "ЩИТОНОСЕЦ");
            case global::EnemyArchetype.Archer: return L("ARCHER", "ЛУЧНИК");
            case global::EnemyArchetype.BatteringRam: return L("BATTERING RAM", "ТАРАН");
            case global::EnemyArchetype.Boss: return L("BOSS", "БОСС");
            default: return L("INFANTRY", "ПЕХОТА");
        }
    }

    public static string EnemyName(EnemyArchetype archetype)
    {
        switch (archetype)
        {
            case global::EnemyArchetype.Runner: return L("GREEK RUNNER", "ГРЕЧЕСКИЙ БЕГУН");
            case global::EnemyArchetype.HeavyHoplite: return L("HEAVY HOPLITE", "ТЯЖЁЛЫЙ ГОПЛИТ");
            case global::EnemyArchetype.ShieldBearer: return L("SHIELD BEARER", "ЩИТОНОСЕЦ");
            case global::EnemyArchetype.Archer: return L("GREEK ARCHER", "ГРЕЧЕСКИЙ ЛУЧНИК");
            case global::EnemyArchetype.BatteringRam: return L("BATTERING RAM", "ТАРАН");
            case global::EnemyArchetype.Boss: return L("MENELAUS", "МЕНЕЛАЙ");
            default: return L("GREEK INFANTRY", "ГРЕЧЕСКАЯ ПЕХОТА");
        }
    }
}
