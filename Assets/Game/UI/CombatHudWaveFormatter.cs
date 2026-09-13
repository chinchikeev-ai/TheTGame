public static class CombatHudWaveFormatter
{
    public static string BuildPreview(EnemySpawner spawner)
    {
        if (spawner == null) return "";

        string preview = GameLanguage.T("NEXT: ", "ДАЛЕЕ: ");
        bool any = false;
        any |= Append(ref preview, GameLanguage.T("INF", "ПЕХ"), spawner.NextWaveInfantryCount, any);
        any |= Append(ref preview, GameLanguage.T("RUN", "БЕГ"), spawner.NextWaveRunnerCount, any);
        any |= Append(ref preview, GameLanguage.T("HEAVY", "ТЯЖ"), spawner.NextWaveHeavyCount, any);
        any |= Append(ref preview, GameLanguage.T("SHIELD", "ЩИТ"), spawner.NextWaveShieldCount, any);
        any |= Append(ref preview, GameLanguage.T("ARCHER", "ЛУК"), spawner.NextWaveArcherCount, any);
        any |= Append(ref preview, GameLanguage.T("MENELAUS", "МЕНЕЛАЙ"), spawner.NextWaveBossCount, any);
        return any ? preview : GameLanguage.T("NEXT WAVE DATA PREPARING", "ПОДГОТОВКА ДАННЫХ ВОЛНЫ");
    }

    static bool Append(ref string text, string label, int count, bool alreadyHas)
    {
        if (count <= 0) return false;
        if (alreadyHas) text += "   •   ";
        text += $"{label} ×{count}";
        return true;
    }
}
