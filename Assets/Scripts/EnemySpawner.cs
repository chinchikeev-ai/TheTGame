using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public Transform[][] paths;
    public int maxWaves = 7;
    public int enemiesPerWave = 5;
    public float spawnInterval = 0.8f;
    public float timeBetweenWaves = 6f;

    public int CurrentWave { get; private set; }
    public int NextWaveEnemyCount { get; private set; }
    public float NextWaveHpMultiplier { get; private set; } = 1f;
    public float NextWaveSpeedMultiplier { get; private set; } = 1f;
    public float InterWaveCountdown { get; private set; }
    public bool WaveActive { get; private set; }
    public bool WaitingForManualStart { get; private set; } = true;
    public bool NextWaveHasHeavy => Mathf.Min(CurrentWave + 1, maxWaves) >= 3;
    public bool NextWaveHasBoss => Mathf.Min(CurrentWave + 1, maxWaves) == maxWaves;

    bool running;
    bool requestStart;

    public void Initialize(Transform[][] newPaths)
    {
        paths = newPaths;
        spawnPoints = new Transform[paths.Length];
        for (int i = 0; i < paths.Length; i++) spawnPoints[i] = paths[i][0];
        GameManager.Instance.MaxWaves = maxWaves;
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

            if (wave == 1)
            {
                while (!requestStart && !GameManager.Instance.GameEnded) yield return null;
            }
            else
            {
                InterWaveCountdown = timeBetweenWaves;
                while (InterWaveCountdown > 0f && !requestStart && !GameManager.Instance.GameEnded)
                {
                    InterWaveCountdown -= Time.deltaTime;
                    yield return null;
                }
            }

            if (GameManager.Instance.GameEnded) yield break;
            requestStart = false;
            WaitingForManualStart = false;
            InterWaveCountdown = 0f;
            CurrentWave = wave;
            GameManager.Instance.CurrentWave = wave;
            WaveActive = true;

            for (int i = 0; i < NextWaveEnemyCount; i++)
            {
                bool boss = wave == maxWaves && i == NextWaveEnemyCount - 1;
                bool heavy = !boss && wave >= 3 && (i % 4 == 3 || (wave >= 6 && i % 3 == 2));
                SpawnEnemy(wave, i, heavy, boss);
                yield return new WaitForSeconds(spawnInterval);
            }

            while (!GameManager.Instance.GameEnded && FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length > 0)
                yield return null;

            WaveActive = false;
        }

        if (!GameManager.Instance.GameEnded) GameManager.Instance.WinGame();
    }

    void PrepareNextWave(int wave)
    {
        NextWaveEnemyCount = enemiesPerWave + (wave - 1) * 2 + (wave == maxWaves ? 1 : 0);
        NextWaveHpMultiplier = 1f + (wave - 1) * 0.32f;
        NextWaveSpeedMultiplier = 1f + (wave - 1) * 0.045f;
    }

    void SpawnEnemy(int wave, int index, bool heavy, bool boss)
    {
        int route = paths != null && paths.Length > 1 ? index % paths.Length : 0;
        Transform[] routePath = paths[route];
        GameObject enemyObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        enemyObj.name = boss ? "Boss" : heavy ? "HeavyEnemy" : "Enemy";
        enemyObj.transform.position = routePath[0].position;
        TowerFactory.SetColor(enemyObj, boss ? new Color(0.55f,0.05f,0.08f) : heavy ? new Color(0.35f,0.12f,0.08f) : new Color(0.65f,0.32f,0.18f));

        Enemy enemy = enemyObj.AddComponent<Enemy>();
        float hp = NextWaveHpMultiplier * (boss ? 7f : heavy ? 2.7f : 1f);
        float speed = NextWaveSpeedMultiplier * (boss ? 0.72f : heavy ? 0.82f : 1f);
        enemy.Init(routePath, hp, speed);
        if (boss) enemy.ConfigureElite(1.8f, 350, 5);
        else if (heavy) enemy.ConfigureElite(1.25f, 55, 2);
    }
}
