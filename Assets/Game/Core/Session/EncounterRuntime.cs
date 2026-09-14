public static class EncounterRuntime
{
    public static int CurrentEncounter => GameManager.Instance != null ? GameManager.Instance.CurrentWave : 0;
    public static int MaxEncounters => GameManager.Instance != null ? GameManager.Instance.MaxWaves : 0;

    public static bool EncounterActive(EnemySpawner spawner) => spawner != null && spawner.WaveActive;
    public static bool BetweenEncounters(EnemySpawner spawner) =>
        spawner != null &&
        !spawner.WaveActive &&
        spawner.CurrentWave > 0 &&
        (GameManager.Instance == null || !GameManager.Instance.GameEnded);
    public static bool WaitingForEncounterStart(EnemySpawner spawner) => spawner != null && spawner.WaitingForManualStart;
    public static bool FirstEncounterPreparationLocked(EnemySpawner spawner) => spawner != null && spawner.FirstEncounterPreparationLocked;

    public static int CurrentEncounter(EnemySpawner spawner) => spawner != null ? spawner.CurrentWave : 0;
    public static int NextEncounterEnemyCount(EnemySpawner spawner) => spawner != null ? spawner.NextWaveEnemyCount : 0;
    public static float NextEncounterHpMultiplier(EnemySpawner spawner) => spawner != null ? spawner.NextWaveHpMultiplier : 1f;
    public static float NextEncounterSpeedMultiplier(EnemySpawner spawner) => spawner != null ? spawner.NextWaveSpeedMultiplier : 1f;
    public static float InterEncounterCountdown(EnemySpawner spawner) => spawner != null ? spawner.InterWaveCountdown : 0f;
    public static float TargetEncounterDuration(EnemySpawner spawner) => spawner != null ? spawner.TargetWaveDuration : 0f;
    public static float CurrentEncounterElapsed(EnemySpawner spawner) => spawner != null ? spawner.CurrentWaveElapsed : 0f;
    public static int CurrentEncounterTotalEnemies(EnemySpawner spawner) => spawner != null ? spawner.CurrentWaveTotalEnemies : 0;
    public static int CurrentEncounterSpawnedEnemies(EnemySpawner spawner) => spawner != null ? spawner.CurrentWaveSpawnedEnemies : 0;
    public static int CurrentEncounterResolvedEnemies(EnemySpawner spawner) => spawner != null ? spawner.CurrentWaveResolvedEnemies : 0;
    public static float CurrentEncounterProgress(EnemySpawner spawner) => spawner != null ? spawner.CurrentWaveProgress : 0f;

    public static bool NextEncounterHasHeavy(EnemySpawner spawner) => spawner != null && spawner.NextWaveHasHeavy;
    public static bool NextEncounterHasBoss(EnemySpawner spawner) => spawner != null && spawner.NextWaveHasBoss;
    public static string NextEncounterBossDisplayName(EnemySpawner spawner) => spawner != null ? spawner.NextWaveBossDisplayName : "";
    public static int NextEncounterInfantryCount(EnemySpawner spawner) => spawner != null ? spawner.NextWaveInfantryCount : 0;
    public static int NextEncounterRunnerCount(EnemySpawner spawner) => spawner != null ? spawner.NextWaveRunnerCount : 0;
    public static int NextEncounterHeavyCount(EnemySpawner spawner) => spawner != null ? spawner.NextWaveHeavyCount : 0;
    public static int NextEncounterShieldCount(EnemySpawner spawner) => spawner != null ? spawner.NextWaveShieldCount : 0;
    public static int NextEncounterArcherCount(EnemySpawner spawner) => spawner != null ? spawner.NextWaveArcherCount : 0;
    public static int NextEncounterBossCount(EnemySpawner spawner) => spawner != null ? spawner.NextWaveBossCount : 0;

    public static void StartEncounterNow(EnemySpawner spawner)
    {
        if (spawner != null) spawner.StartWaveNow();
    }
}
