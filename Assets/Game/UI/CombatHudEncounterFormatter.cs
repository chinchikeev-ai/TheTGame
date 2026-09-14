public static class CombatHudEncounterFormatter
{
    public static string BuildPreview(EnemySpawner spawner)
    {
        if (spawner == null) return "";

        string preview = GameLanguage.T("NEXT: ", "ДАЛЕЕ: ");
        bool any = false;
        any |= Append(ref preview, GameLanguage.T("INF", "ПЕХ"), EncounterRuntime.NextEncounterInfantryCount(spawner), any);
        any |= Append(ref preview, GameLanguage.T("RUN", "БЕГ"), EncounterRuntime.NextEncounterRunnerCount(spawner), any);
        any |= Append(ref preview, GameLanguage.T("HEAVY", "ТЯЖ"), EncounterRuntime.NextEncounterHeavyCount(spawner), any);
        any |= Append(ref preview, GameLanguage.T("SHIELD", "ЩИТ"), EncounterRuntime.NextEncounterShieldCount(spawner), any);
        any |= Append(ref preview, GameLanguage.T("ARCHER", "ЛУК"), EncounterRuntime.NextEncounterArcherCount(spawner), any);
        string bossName = EncounterRuntime.NextEncounterBossDisplayName(spawner);
        string bossLabel = BossLabel(bossName);
        any |= Append(ref preview, bossLabel, EncounterRuntime.NextEncounterBossCount(spawner), any);
        return any ? preview : GameLanguage.T("NEXT ENCOUNTER DATA PREPARING", "ПОДГОТОВКА ДАННЫХ БОЯ");
    }

    public static string BossLabel(string bossName)
    {
        if (string.IsNullOrWhiteSpace(bossName))
            return GameLanguage.T("BOSS", "БОСС");

        if (string.Equals(bossName.Trim(), "Menelaus", System.StringComparison.OrdinalIgnoreCase))
            return GameLanguage.T("MENELAUS", "МЕНЕЛАЙ");

        return bossName.ToUpperInvariant();
    }

    static bool Append(ref string text, string label, int count, bool alreadyHas)
    {
        if (count <= 0) return false;
        if (alreadyHas) text += "   •   ";
        text += $"{label} ×{count}";
        return true;
    }
}
