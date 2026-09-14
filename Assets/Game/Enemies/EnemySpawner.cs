using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    static readonly WaitForSeconds ReinforcementDelay = new WaitForSeconds(.65f);

    sealed class PreparedSpawn
    {
        public EnemyArchetype archetype;
        public int route;
        public int routeOffset;
        public float startDelay;
        public float spawnInterval;
        public float hpMultiplier = 1f;
        public float speedMultiplier = 1f;
        public string behaviorId;

        public PreparedSpawn Copy()
        {
            return new PreparedSpawn
            {
                archetype = archetype,
                route = route,
                routeOffset = routeOffset,
                startDelay = startDelay,
                spawnInterval = spawnInterval,
                hpMultiplier = hpMultiplier,
                speedMultiplier = speedMultiplier,
                behaviorId = behaviorId
            };
        }
    }

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
    public int CurrentWaveTotalEnemies => currentWaveTotalEnemies;
    public int CurrentWaveSpawnedEnemies => currentWaveSpawnedEnemies;
    public int CurrentWaveResolvedEnemies => Mathf.Max(0, currentWaveSpawnedEnemies - EnemyRegistry.AliveCount);
    public float CurrentWaveProgress
    {
        get
        {
            if (!WaveActive) return CurrentWave > 0 ? 1f : 0f;
            if (currentWaveTotalEnemies <= 0) return 0f;
            return Mathf.Clamp01(CurrentWaveResolvedEnemies / (float)currentWaveTotalEnemies);
        }
    }
    public bool WaveActive { get; private set; }
    public bool WaitingForManualStart { get; private set; } = true;
    public bool NextWaveHasHeavy { get; private set; }
    public bool NextWaveHasBoss { get; private set; }
    public string NextWaveBossDisplayName { get; private set; } = "";

    public int NextWaveInfantryCount { get; private set; }
    public int NextWaveRunnerCount { get; private set; }
    public int NextWaveHeavyCount { get; private set; }
    public int NextWaveShieldCount { get; private set; }
    public int NextWaveArcherCount { get; private set; }
    public int NextWaveBossCount { get; private set; }

    readonly List<PreparedSpawn> preparedPlan = new List<PreparedSpawn>(64);
    readonly List<PreparedSpawn> authoredNonBossPlan = new List<PreparedSpawn>(64);
    readonly List<PreparedSpawn> authoredBossPlan = new List<PreparedSpawn>(4);

    bool running;
    bool requestStart;
    float waveStartedAt;
    float lastWaveDuration;
    EncounterData preparedEncounter;
    int effectiveEnemyCount;
    int currentWaveTotalEnemies;
    int currentWaveSpawnedEnemies;
    float effectiveHpMultiplier = 1f;
    float effectiveSpeedMultiplier = 1f;

    public void Initialize(Transform[][] newPaths)
    {
        paths = newPaths;
        if (paths == null || paths.Length == 0)
            throw new InvalidOperationException("EnemySpawner requires at least one authored runtime path.");

        spawnPoints = new Transform[paths.Length];
        for (int i = 0; i < paths.Length; i++)
            spawnPoints[i] = paths[i] != null && paths[i].Length > 0 ? paths[i][0] : null;

        ChapterData chapter = GameManager.Instance != null ? GameManager.Instance.Chapter : null;
        if (chapter != null && chapter.EncounterCount > 0) maxWaves = chapter.EncounterCount;
        if (GameManager.Instance != null) GameManager.Instance.MaxWaves = maxWaves;

        EnemyRegistry.Clear();
        PrepareNextWave(1);
        RuntimeFileLogger.Event("SPAWNER", $"Initialized routes={paths.Length}, encounters={maxWaves}, difficulty={CampaignSave.Difficulty}");
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
        RuntimeFileLogger.Event("WAVE", $"Manual start requested for encounter={Mathf.Max(1, CurrentWave + 1)}");
    }

    IEnumerator GameLoop()
    {
        running = true;
        for (int wave = 1; wave <= maxWaves; wave++)
        {
            PrepareNextWave(wave);
            WaitingForManualStart = true;
            GameStateController.Instance?.SetState(wave == 1 ? GameState.Preparing : GameState.BetweenWaves);

            float preparationSeconds = preparedEncounter.preparationTime;
            RuntimeFileLogger.Event(
                "WAVE",
                $"Prepared encounter={wave}/{maxWaves}, id={preparedEncounter.encounterId}, enemies={effectiveEnemyCount}, prep={preparationSeconds:0.0}s, target={preparedEncounter.targetDuration:0.0}s, spawnInterval={preparedEncounter.spawnInterval:0.00}s, hpMul={effectiveHpMultiplier:0.00}, speedMul={effectiveSpeedMultiplier:0.00}, boss={NextWaveHasBoss}, difficulty={CampaignSave.Difficulty}");

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
            currentWaveTotalEnemies = preparedPlan.Count;
            currentWaveSpawnedEnemies = 0;
            GameStateController.Instance?.SetState(GameState.WaveRunning);
            RuntimeFileLogger.Event("WAVE", $"Started encounter={wave}/{maxWaves}, id={preparedEncounter.encounterId}, total={currentWaveTotalEnemies}");

            for (int i = 0; i < preparedPlan.Count; i++)
            {
                PreparedSpawn spawn = preparedPlan[i];
                if (spawn.startDelay > 0f) yield return new WaitForSeconds(spawn.startDelay);
                SpawnPreparedEnemy(spawn, i);
                yield return new WaitForSeconds(Mathf.Max(.01f, spawn.spawnInterval));
            }

            while (!GameManager.Instance.GameEnded && EnemyRegistry.AliveCount > 0) yield return null;
            lastWaveDuration = Mathf.Max(0f, Time.time - waveStartedAt);
            WaveActive = false;
            RuntimeFileLogger.Event("WAVE", $"Completed encounter={wave}/{maxWaves}, actualDuration={lastWaveDuration:0.0}s, target={preparedEncounter.targetDuration:0.0}s, killsTotal={GameManager.Instance.Kills}, leaksTotal={GameManager.Instance.Leaks}, gold={GameManager.Instance.Money}, gateHP={GameManager.Instance.BaseHealth}");
        }

        if (!GameManager.Instance.GameEnded) GameManager.Instance.WinGame();
    }

    void PrepareNextWave(int wave)
    {
        ChapterData chapter = GameManager.Instance != null ? GameManager.Instance.Chapter : null;
        preparedEncounter = chapter != null ? chapter.GetEncounter(wave) : null;
        if (preparedEncounter == null)
            throw new InvalidOperationException($"Missing authored EncounterData for encounter {wave}. Chapter={chapter?.chapterId ?? "none"}.");

        CampaignDifficulty difficulty = CampaignSave.Difficulty;
        effectiveHpMultiplier = preparedEncounter.hpMultiplier * DifficultyRules.EnemyHpMultiplier(difficulty);
        effectiveSpeedMultiplier = preparedEncounter.speedMultiplier * DifficultyRules.EnemySpeedMultiplier(difficulty);
        BuildPreparedPlan(DifficultyRules.EnemyCountMultiplier(difficulty));

        effectiveEnemyCount = preparedPlan.Count;
        NextWaveEnemyCount = effectiveEnemyCount;
        NextWaveHpMultiplier = effectiveHpMultiplier;
        NextWaveSpeedMultiplier = effectiveSpeedMultiplier;
        TargetWaveDuration = preparedEncounter.targetDuration;
        InterWaveCountdown = preparedEncounter.preparationTime;
        BuildPreparedWaveComposition();
        NextWaveHasHeavy = NextWaveHeavyCount > 0 || NextWaveShieldCount > 0;
    }

    void BuildPreparedPlan(float countMultiplier)
    {
        preparedPlan.Clear();
        authoredNonBossPlan.Clear();
        authoredBossPlan.Clear();

        EncounterSpawnGroup[] groups = preparedEncounter.spawnGroups;
        if (groups == null || groups.Length == 0)
            throw new InvalidOperationException($"Encounter '{preparedEncounter.encounterId}' has no authored spawn groups.");

        int baseNonBossCount = 0;
        int baseBossCount = 0;

        for (int g = 0; g < groups.Length; g++)
        {
            EncounterSpawnGroup group = groups[g];
            if (group == null || group.pattern == null || group.pattern.Length == 0) continue;

            bool containsBoss = false;
            bool containsNonBoss = false;
            for (int p = 0; p < group.pattern.Length; p++)
            {
                if (group.pattern[p] == EnemyArchetype.Boss) containsBoss = true;
                else containsNonBoss = true;
            }
            if (containsBoss && containsNonBoss)
                throw new InvalidOperationException($"Encounter '{preparedEncounter.encounterId}' spawn group {g} mixes Boss and non-boss archetypes. Author the boss as a separate group.");

            List<PreparedSpawn> destination = containsBoss ? authoredBossPlan : authoredNonBossPlan;
            if (containsBoss) baseBossCount += group.BaseCount;
            else baseNonBossCount += group.BaseCount;

            int repeats = Mathf.Max(1, group.repeats);
            bool firstInGroup = true;
            for (int repeat = 0; repeat < repeats; repeat++)
            {
                for (int p = 0; p < group.pattern.Length; p++)
                {
                    destination.Add(new PreparedSpawn
                    {
                        archetype = group.pattern[p],
                        route = group.route,
                        routeOffset = group.routeOffset,
                        startDelay = firstInGroup ? Mathf.Max(0f, group.startDelay) : 0f,
                        spawnInterval = group.spawnInterval > 0f ? group.spawnInterval : preparedEncounter.spawnInterval,
                        hpMultiplier = Mathf.Max(.01f, group.hpMultiplier),
                        speedMultiplier = Mathf.Max(.01f, group.speedMultiplier),
                        behaviorId = group.behaviorId
                    });
                    firstInGroup = false;
                }
            }
        }

        int baseTotal = baseNonBossCount + baseBossCount;
        if (baseTotal <= 0)
            throw new InvalidOperationException($"Encounter '{preparedEncounter.encounterId}' resolves to zero base enemies.");
        if (baseNonBossCount > 0 && authoredNonBossPlan.Count == 0)
            throw new InvalidOperationException($"Encounter '{preparedEncounter.encounterId}' has a non-boss base count but no authored non-boss pattern.");
        if (baseBossCount > 0 && authoredBossPlan.Count == 0)
            throw new InvalidOperationException($"Encounter '{preparedEncounter.encounterId}' has a boss base count but no authored boss pattern.");

        int targetTotal = Mathf.Max(1, Mathf.RoundToInt(baseTotal * Mathf.Max(.01f, countMultiplier)));
        if (baseBossCount > 0) targetTotal = Mathf.Max(baseBossCount, targetTotal);
        int targetNonBoss = baseNonBossCount > 0 ? Mathf.Max(1, targetTotal - baseBossCount) : 0;

        AppendPrefix(authoredNonBossPlan, targetNonBoss, preparedPlan);
        AppendPrefix(authoredBossPlan, baseBossCount, preparedPlan);
    }

    static void AppendPrefix(List<PreparedSpawn> source, int targetCount, List<PreparedSpawn> destination)
    {
        if (targetCount <= 0) return;
        if (source == null || source.Count == 0)
            throw new InvalidOperationException("Cannot scale an empty authored spawn pattern.");

        for (int i = 0; i < targetCount; i++)
        {
            int sourceIndex = i % source.Count;
            PreparedSpawn copy = source[sourceIndex].Copy();
            if (i >= source.Count) copy.startDelay = 0f;
            destination.Add(copy);
        }
    }

    void BuildPreparedWaveComposition()
    {
        NextWaveInfantryCount = 0;
        NextWaveRunnerCount = 0;
        NextWaveHeavyCount = 0;
        NextWaveShieldCount = 0;
        NextWaveArcherCount = 0;
        NextWaveBossCount = 0;
        NextWaveBossDisplayName = "";

        for (int i = 0; i < preparedPlan.Count; i++)
        {
            EnemyArchetype archetype = preparedPlan[i].archetype;
            switch (archetype)
            {
                case EnemyArchetype.Runner: NextWaveRunnerCount++; break;
                case EnemyArchetype.HeavyHoplite: NextWaveHeavyCount++; break;
                case EnemyArchetype.ShieldBearer: NextWaveShieldCount++; break;
                case EnemyArchetype.Archer: NextWaveArcherCount++; break;
                case EnemyArchetype.Boss:
                    NextWaveBossCount++;
                    if (string.IsNullOrEmpty(NextWaveBossDisplayName))
                        NextWaveBossDisplayName = BalanceCatalog.GetEnemy(archetype).displayName;
                    break;
                default: NextWaveInfantryCount++; break;
            }
        }

        NextWaveHasBoss = NextWaveBossCount > 0;
    }

    void SpawnPreparedEnemy(PreparedSpawn spawn, int globalIndex)
    {
        if (paths == null || paths.Length == 0) return;

        int route = spawn.route;
        if (route < 0)
        {
            route = (globalIndex + spawn.routeOffset) % paths.Length;
            if (route < 0) route += paths.Length;
        }

        EnemyData data = BalanceCatalog.GetEnemy(spawn.archetype);
        SpawnConfiguredEnemy(
            data,
            route,
            effectiveHpMultiplier * spawn.hpMultiplier,
            effectiveSpeedMultiplier * spawn.speedMultiplier,
            spawn.behaviorId);
        currentWaveSpawnedEnemies++;
    }

    void SpawnConfiguredEnemy(EnemyData data, int route, float hpMultiplier, float speedMultiplier, string behaviorId = null)
    {
        if (paths == null || paths.Length == 0 || data == null) return;
        route = Mathf.Clamp(route, 0, paths.Length - 1);
        Transform[] routePath = paths[route];
        if (routePath == null || routePath.Length == 0) return;

        GameObject enemyObj = EnemyVisualFactory.CreateEnemyObject(data);
        enemyObj.transform.position = routePath[0].position;

        Enemy enemy = enemyObj.AddComponent<Enemy>();
        enemy.InitFromData(routePath, data, hpMultiplier, speedMultiplier);
        EnemyRuntimeBehaviorRegistry.Attach(enemyObj, behaviorId);
    }

    public void SpawnReinforcements(int count, EnemyArchetype[] pattern, float hpMultiplier = .85f, float speedMultiplier = 1f)
    {
        if (!WaveActive || GameManager.Instance == null || GameManager.Instance.GameEnded || count <= 0) return;
        if (pattern == null || pattern.Length == 0) pattern = new[] { EnemyArchetype.Infantry };
        StartCoroutine(SpawnReinforcementBurst(count, pattern, hpMultiplier, speedMultiplier));
    }

    IEnumerator SpawnReinforcementBurst(int count, EnemyArchetype[] pattern, float hpMultiplier, float speedMultiplier)
    {
        for (int i = 0; i < count; i++)
        {
            if (GameManager.Instance == null || GameManager.Instance.GameEnded) yield break;
            int route = paths != null && paths.Length > 1 ? i % paths.Length : 0;
            EnemyArchetype archetype = pattern[i % pattern.Length];
            EnemyData data = BalanceCatalog.GetEnemy(archetype);
            currentWaveTotalEnemies++;
            SpawnConfiguredEnemy(
                data,
                route,
                Mathf.Max(.01f, effectiveHpMultiplier * hpMultiplier),
                Mathf.Max(.01f, effectiveSpeedMultiplier * speedMultiplier));
            currentWaveSpawnedEnemies++;
            yield return ReinforcementDelay;
        }
    }
}
