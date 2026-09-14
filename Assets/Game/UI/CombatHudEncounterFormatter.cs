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
        string bossLabel = string.IsNullOrWhiteSpace(bossName)
            ? GameLanguage.T("BOSS", "БОСС")
            : bossName.ToUpperInvariant();
        any |= Append(ref preview, bossLabel, EncounterRuntime.NextEncounterBossCount(spawner), any);
        return any ? preview : GameLanguage.T("NEXT ENCOUNTER DATA PREPARING", "ПОДГОТОВКА ДАННЫХ БОЯ");
    }

    static bool Append(ref string text, string label, int count, bool alreadyHas)
    {
        if (count <= 0) return false;
        if (alreadyHas) text += "   •   ";
        text += $"{label} ×{count}";
        return true;
    }
}
