using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform[] waypoints;
    public int maxWaves = 7;
    public int enemiesPerWave = 5;
    public float spawnInterval = 0.8f;
    public float timeBetweenWaves = 4f;

    public int CurrentWave { get; private set; }
    public int NextWaveEnemyCount { get; private set; }
    public float NextWaveHpMultiplier { get; private set; } = 1f;
    public float NextWaveSpeedMultiplier { get; private set; } = 1f;
    public float InterWaveCountdown { get; private set; }
    public bool WaveActive { get; private set; }
    public bool NextWaveHasHeavy { get; private set; }
    public bool NextWaveHasBoss { get; private set; }

    bool running;

    public void Begin()
    {
        if (!running) StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        running = true;
        GameManager.Instance.MaxWaves = maxWaves;
        PrepareNextWave(1);

        for (int wave = 1; wave <= maxWaves; wave++)
        {
            if (GameManager.Instance.GameEnded) yield break;

            CurrentWave = wave;
            GameManager.Instance.CurrentWave = wave;
            PrepareNextWave(wave);
            WaveActive = true;
            InterWaveCountdown = 0f;

            int regularCount = enemiesPerWave + (wave - 1) * 2;
            float hpMul = NextWaveHpMultiplier;
            float speedMul = NextWaveSpeedMultiplier;

            for (int i = 0; i < regularCount; i++)
            {
                bool heavy = wave >= 3 && i > 0 && i % 4 == 0;
                SpawnEnemy(hpMul, speedMul, wave, heavy, false);
                yield return new WaitForSeconds(spawnInterval);
            }

            if (wave == maxWaves)
            {
                yield return new WaitForSeconds(1.2f);
                SpawnEnemy(hpMul, speedMul, wave, false, true);
            }

            while (!GameManager.Instance.GameEnded && FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length > 0)
                yield return null;

            WaveActive = false;
            if (wave >= maxWaves) break;

            PrepareNextWave(wave + 1);
            InterWaveCountdown = timeBetweenWaves;
            while (InterWaveCountdown > 0f && !GameManager.Instance.GameEnded)
            {
                InterWaveCountdown -= Time.deltaTime;
                yield return null;
            }
            InterWaveCountdown = 0f;
        }

        if (!GameManager.Instance.GameEnded)
            GameManager.Instance.WinGame();
    }

    void PrepareNextWave(int wave)
    {
        int regular = enemiesPerWave + (wave - 1) * 2;
        NextWaveHasHeavy = wave >= 3;
        NextWaveHasBoss = wave == maxWaves;
        NextWaveEnemyCount = regular + (NextWaveHasBoss ? 1 : 0);
        NextWaveHpMultiplier = 1f + (wave - 1) * 0.32f;
        NextWaveSpeedMultiplier = 1f + (wave - 1) * 0.045f;
    }

    void SpawnEnemy(float hpMul, float speedMul, int wave, bool heavy, bool boss)
    {
        GameObject enemyObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        enemyObj.transform.position = spawnPoint.position;
        Enemy enemy = enemyObj.AddComponent<Enemy>();

        if (boss)
        {
            enemyObj.name = "Boss";
            TowerFactory.SetColor(enemyObj, new Color(0.42f, 0.08f, 0.42f));
            enemy.Init(waypoints, hpMul * 8f, speedMul * 0.62f);
            enemy.ConfigureElite(1.75f, 300, 5);
        }
        else if (heavy)
        {
            enemyObj.name = "HeavyEnemy";
            TowerFactory.SetColor(enemyObj, new Color(0.48f, 0.12f, 0.10f));
            enemy.Init(waypoints, hpMul * 2.25f, speedMul * 0.76f);
            enemy.ConfigureElite(1.28f, 45, 2);
        }
        else
        {
            enemyObj.name = "Enemy";
            TowerFactory.SetColor(enemyObj, wave >= 5 ? new Color(0.62f, 0.16f, 0.12f) : new Color(0.65f, 0.32f, 0.18f));
            enemy.Init(waypoints, hpMul, speedMul);
        }
    }
}
