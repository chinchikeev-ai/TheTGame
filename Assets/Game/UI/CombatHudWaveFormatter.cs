// Compatibility shim for callers that have not migrated to CombatHudEncounterFormatter yet.
public static class CombatHudWaveFormatter
{
    public static string BuildPreview(EnemySpawner spawner) => CombatHudEncounterFormatter.BuildPreview(spawner);
}
