using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    const float FirstWavePreparationSeconds = 30f;
    static readonly WaitForSeconds ReinforcementDelay = new WaitForSeconds(.65f);

    public Transform[] spawnPoints;
    public Transform[][] paths;
    public int maxWaves = 5;

    public int CurrentWave { get; private set; }
    public int NextWaveEnemyCount { get; private set; }
    public float NextWaveHpMultiplier { get; private set; } = 1f;
    public float NextWaveSpeedMultiplier { get; private set; } = 1f;
    public float InterWaveCountdown { get; private set; }
    public float TargetWaveDuration { get; private set; }
    public float CurrentWaveElapsed => WaveActive ? Mathf.Max(0f, Time.time - waveStartedAt) : lastWaveDuration;
    public bool WaveActive { get; private set; }
    public bool WaitingForManualStart { get; private set; } = true;
    public bool NextWaveHasHeavy { get; private set; }
    public bool NextWaveHasBoss { get; private set; }

    public int NextWaveInfantryCount { get; private set; }
    public int NextWaveRunnerCount { get; private set; }
    public int NextWaveHeavyCount { get; private set; }
    public int NextWaveShieldCount { get; private set; }
    public int NextWaveArcherCount { get; private set; }
    public int NextWaveBossCount { get; private set; }

    bool running;
    bool requestStart;
    float waveStartedAt;
    float lastWaveDuration;
    WaveData preparedWave;
    int effectiveEnemyCount;
    float effectiveHpMultiplier = 1f;
    float effectiveSpeedMultiplier = 1f;

    public void Initialize(Transform[][] newPaths)
    {
        paths = newPaths;
        spawnPoints = new Transform[paths.Length];
        for (int i = 0; i < paths.Length; i++) spawnPoints[i] = paths[i][0];
        GameManager.Instance.MaxWaves = maxWaves;
        EnemyRegistry.Clear();
        PrepareNextWave(1);
        RuntimeFileLogger.Event("SPAWNER", $"Initialized routes={paths.Length}, maxWaves={maxWaves}, difficulty={CampaignSave.Difficulty}");
    }

    public void ActivateLevel()
    {
        if (!running) StartCoroutine(GameLoop());
    }

    public void StartWaveNow()
    {
        if (WaveActive || GameManager.Instance == null || GameManager.Instance.GameEnded) return;
        requestStart = true;
        InterWaveCountdown = 0f;
        RuntimeFileLogger.Event("WAVE", $"Manual start requested for wave={Mathf.Max(1, CurrentWave + 1)}");
    }

    IEnumerator GameLoop()
    {
        running = true;
        for (int wave = 1; wave <= maxWaves; wave++)
        {
            PrepareNextWave(wave);
            WaitingForManualStart = true;
            GameStateController.Instance?.SetState(wave == 1 ? GameState.Preparing : GameState.BetweenWaves);

            float preparationSeconds = wave == 1 ? FirstWavePreparationSeconds : preparedWave.preparationTime;
            RuntimeFileLogger.Event("WAVE", $"Prepared wave={wave}/{maxWaves}, enemies={effectiveEnemyCount}, prep={preparationSeconds:0.0}s, target={preparedWave.targetDuration:0.0}s, spawnInterval={preparedWave.spawnInterval:0.00}s, hpMul={effectiveHpMultiplier:0.00}, speedMul={effectiveSpeedMultiplier:0.00}, boss={preparedWave.hasBoss}, difficulty={CampaignSave.Difficulty}");

            InterWaveCountdown = preparationSeconds;
            while (InterWaveCountdown > 0f && !requestStart && !GameManager.Instance.GameEnded)
            {
                InterWaveCountdown -= Time.deltaTime;
                yield return null;
            }

            if (GameManager.Instance.GameEnded) yield break;
            requestStart = false;
            WaitingForManualStart = false;
            InterWaveCountdown = 0f;
            CurrentWave = wave;
            GameManager.Instance.CurrentWave = wave;
            WaveActive = true;
            waveStartedAt = Time.time;
            lastWaveDuration = 0f;
            GameStateController.Instance?.SetState(GameState.WaveRunning);
            RuntimeFileLogger.Event("WAVE", $"Started wave={wave}/{maxWaves}");

            WaitForSeconds spawnDelay = new WaitForSeconds(preparedWave.spawnInterval);
            for (int i = 0; i < effectiveEnemyCount; i++)
            {
                bool boss = preparedWave.hasBoss && i == effectiveEnemyCount - 1;
                SpawnEnemy(wave, i, boss);
                yield return spawnDelay;
            }

            while (!GameManager.Instance.GameEnded && EnemyRegistry.AliveCount > 0) yield return null;
            lastWaveDuration = Mathf.Max(0f, Time.time - waveStartedAt);
            WaveActive = false;
            RuntimeFileLogger.Event("WAVE", $"Completed wave={wave}/{maxWaves}, actualDuration={lastWaveDuration:0.0}s, target={preparedWave.targetDuration:0.0}s, killsTotal={GameManager.Instance.Kills}, leaksTotal={GameManager.Instance.Leaks}, gold={GameManager.Instance.Money}, gateHP={GameManager.Instance.BaseHealth}");
        }

        if (!GameManager.Instance.GameEnded) GameManager.Instance.WinGame();
    }

    void PrepareNextWave(int wave)
    {
        preparedWave = BalanceCatalog.GetWave(wave, maxWaves);
        CampaignDifficulty difficulty = CampaignSave.Difficulty;

        effectiveEnemyCount = Mathf.Max(1, Mathf.RoundToInt(preparedWave.enemyCount * DifficultyRules.EnemyCountMultiplier(difficulty)));
        if (preparedWave.hasBoss) effectiveEnemyCount = Mathf.Max(2, effectiveEnemyCount);
        effectiveHpMultiplier = preparedWave.hpMultiplier * DifficultyRules.EnemyHpMultiplier(difficulty);
        effectiveSpeedMultiplier = preparedWave.speedMultiplier * DifficultyRules.EnemySpeedMultiplier(difficulty);

        NextWaveEnemyCount = effectiveEnemyCount;
        NextWaveHpMultiplier = effectiveHpMultiplier;
        NextWaveSpeedMultiplier = effectiveSpeedMultiplier;
        NextWaveHasBoss = preparedWave.hasBoss;
        TargetWaveDuration = preparedWave.targetDuration;
        InterWaveCountdown = wave == 1 ? FirstWavePreparationSeconds : preparedWave.preparationTime;
        BuildPreparedWaveComposition(wave);
        NextWaveHasHeavy = NextWaveHeavyCount > 0 || NextWaveShieldCount > 0;
    }

    void BuildPreparedWaveComposition(int wave)
    {
        NextWaveInfantryCount = 0;
        NextWaveRunnerCount = 0;
        NextWaveHeavyCount = 0;
        NextWaveShieldCount = 0;
        NextWaveArcherCount = 0;
        NextWaveBossCount = 0;

        for (int i = 0; i < effectiveEnemyCount; i++)
        {
            bool boss = preparedWave.hasBoss && i == effectiveEnemyCount - 1;
            EnemyArchetype archetype = BalanceCatalog.GetEnemyForWave(wave, i, effectiveEnemyCount, boss).archetype;
            switch (archetype)
            {
                case EnemyArchetype.Runner: NextWaveRunnerCount++; break;
                case EnemyArchetype.HeavyHoplite: NextWaveHeavyCount++; break;
                case EnemyArchetype.ShieldBearer: NextWaveShieldCount++; break;
                case EnemyArchetype.Archer: NextWaveArcherCount++; break;
                case EnemyArchetype.Boss: NextWaveBossCount++; break;
                default: NextWaveInfantryCount++; break;
            }
        }
    }

    void SpawnEnemy(int wave, int index, bool boss)
    {
        int route = paths != null && paths.Length > 1 ? index % paths.Length : 0;
        EnemyData data = BalanceCatalog.GetEnemyForWave(wave, index, effectiveEnemyCount, boss);
        SpawnConfiguredEnemy(data, route, effectiveHpMultiplier, effectiveSpeedMultiplier, boss);
    }

    void SpawnConfiguredEnemy(EnemyData data, int route, float hpMultiplier, float speedMultiplier, bool boss)
    {
        if (paths == null || paths.Length == 0) return;
        route = Mathf.Clamp(route, 0, paths.Length - 1);
        Transform[] routePath = paths[route];
        if (routePath == null || routePath.Length == 0) return;

        GameObject enemyObj = EnemyVisualFactory.CreateEnemyObject(data);
        enemyObj.transform.position = routePath[0].position;

        Enemy enemy = enemyObj.AddComponent<Enemy>();
        enemy.InitFromData(routePath, data, hpMultiplier, speedMultiplier);
        if (boss) enemyObj.AddComponent<MenelausBossController>();
    }

    public void SpawnMenelausReinforcements(int count)
    {
        if (!WaveActive || GameManager.Instance == null || GameManager.Instance.GameEnded || count <= 0) return;
        StartCoroutine(SpawnReinforcementBurst(count));
    }

    IEnumerator SpawnReinforcementBurst(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (GameManager.Instance == null || GameManager.Instance.GameEnded) yield break;
            int route = paths != null && paths.Length > 1 ? i % paths.Length : 0;
            EnemyArchetype archetype = i % 3 == 2 ? EnemyArchetype.ShieldBearer : EnemyArchetype.Infantry;
            EnemyData data = BalanceCatalog.GetEnemy(archetype);
            SpawnConfiguredEnemy(data, route, Mathf.Max(1f, effectiveHpMultiplier * .85f), Mathf.Max(1f, effectiveSpeedMultiplier), false);
            yield return ReinforcementDelay;
        }
    }
}
