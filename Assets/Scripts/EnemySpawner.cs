using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public Transform[][] paths;
    public int maxWaves = 7;

    public int CurrentWave { get; private set; }
    public int NextWaveEnemyCount { get; private set; }
    public float NextWaveHpMultiplier { get; private set; } = 1f;
    public float NextWaveSpeedMultiplier { get; private set; } = 1f;
    public float InterWaveCountdown { get; private set; }
    public float TargetWaveDuration { get; private set; }
    public bool WaveActive { get; private set; }
    public bool WaitingForManualStart { get; private set; } = true;
    public bool NextWaveHasHeavy { get; private set; }
    public bool NextWaveHasBoss { get; private set; }

    bool running;
    bool requestStart;
    WaveData preparedWave;

    public void Initialize(Transform[][] newPaths)
    {
        paths = newPaths;
        spawnPoints = new Transform[paths.Length];
        for (int i = 0; i < paths.Length; i++) spawnPoints[i] = paths[i][0];
        GameManager.Instance.MaxWaves = maxWaves;
        EnemyRegistry.Clear();
        PrepareNextWave(1);
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
    }

    IEnumerator GameLoop()
    {
        running = true;
        for (int wave = 1; wave <= maxWaves; wave++)
        {
            PrepareNextWave(wave);
            WaitingForManualStart = true;
            GameStateController.Instance?.SetState(wave == 1 ? GameState.Preparing : GameState.BetweenWaves);

            InterWaveCountdown = preparedWave.preparationTime;
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
            GameStateController.Instance?.SetState(GameState.WaveRunning);

            for (int i = 0; i < preparedWave.enemyCount; i++)
            {
                bool boss = preparedWave.hasBoss && i == preparedWave.enemyCount - 1;
                bool heavy = !boss && preparedWave.heavyEvery > 0 && i > 0 && i % preparedWave.heavyEvery == preparedWave.heavyEvery - 1;
                SpawnEnemy(i, heavy, boss);
                yield return new WaitForSeconds(preparedWave.spawnInterval);
            }

            while (!GameManager.Instance.GameEnded && EnemyRegistry.AliveCount > 0)
                yield return null;

            WaveActive = false;
        }

        if (!GameManager.Instance.GameEnded) GameManager.Instance.WinGame();
    }

    void PrepareNextWave(int wave)
    {
        preparedWave = BalanceCatalog.GetWave(wave, maxWaves);
        NextWaveEnemyCount = preparedWave.enemyCount;
        NextWaveHpMultiplier = preparedWave.hpMultiplier;
        NextWaveSpeedMultiplier = preparedWave.speedMultiplier;
        NextWaveHasHeavy = preparedWave.heavyEvery > 0;
        NextWaveHasBoss = preparedWave.hasBoss;
        TargetWaveDuration = preparedWave.targetDuration;
        InterWaveCountdown = preparedWave.preparationTime;
    }

    void SpawnEnemy(int index, bool heavy, bool boss)
    {
        int route = paths != null && paths.Length > 1 ? index % paths.Length : 0;
        Transform[] routePath = paths[route];
        EnemyData data = boss ? BalanceCatalog.BossEnemy : heavy ? BalanceCatalog.HeavyEnemy : BalanceCatalog.NormalEnemy;

        GameObject enemyObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        enemyObj.name = data.displayName;
        enemyObj.transform.position = routePath[0].position;
        TowerFactory.SetColor(enemyObj, data.color);

        Enemy enemy = enemyObj.AddComponent<Enemy>();
        enemy.InitFromData(routePath, data, preparedWave.hpMultiplier, preparedWave.speedMultiplier);
    }
}
