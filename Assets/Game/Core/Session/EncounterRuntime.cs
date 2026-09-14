public static class EncounterRuntime
{
    public static int CurrentEncounter => GameManager.Instance != null ? GameManager.Instance.CurrentWave : 0;
    public static int MaxEncounters => GameManager.Instance != null ? GameManager.Instance.MaxWaves : 0;

    public static bool EncounterActive(EnemySpawner spawner) => spawner != null && spawner.WaveActive;
    public static bool BetweenEncounters(EnemySpawner spawner) =>
        spawner != null && !spawner.WaveActive && spawner.CurrentWave > 0 && !spawner.WaitingForManualStart;

    public static int CurrentEncounter(EnemySpawner spawner) => spawner != null ? spawner.CurrentWave : 0;
    public static int NextEncounterEnemyCount(EnemySpawner spawner) => spawner != null ? spawner.NextWaveEnemyCount : 0;
    public static float InterEncounterCountdown(EnemySpawner spawner) => spawner != null ? spawner.InterWaveCountdown : 0f;
    public static float TargetEncounterDuration(EnemySpawner spawner) => spawner != null ? spawner.TargetWaveDuration : 0f;
    public static float CurrentEncounterElapsed(EnemySpawner spawner) => spawner != null ? spawner.CurrentWaveElapsed : 0f;
    public static float CurrentEncounterProgress(EnemySpawner spawner) => spawner != null ? spawner.CurrentWaveProgress : 0f;
}
