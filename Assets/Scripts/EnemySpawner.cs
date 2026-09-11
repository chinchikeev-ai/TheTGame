using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform[] waypoints;
    public int maxWaves = 5;
    public int enemiesPerWave = 5;
    public float spawnInterval = 0.8f;
    public float timeBetweenWaves = 4f;

    public int CurrentWave { get; private set; }
    public int NextWaveEnemyCount { get; private set; }
    public float NextWaveHpMultiplier { get; private set; } = 1f;
    public float NextWaveSpeedMultiplier { get; private set; } = 1f;
    public float InterWaveCountdown { get; private set; }
    public bool WaveActive { get; private set; }

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

            int count = NextWaveEnemyCount;
            float hpMul = NextWaveHpMultiplier;
            float speedMul = NextWaveSpeedMultiplier;

            for (int i = 0; i < count; i++)
            {
                SpawnEnemy(hpMul, speedMul, wave);
                yield return new WaitForSeconds(spawnInterval);
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
        NextWaveEnemyCount = enemiesPerWave + (wave - 1) * 2;
        NextWaveHpMultiplier = 1f + (wave - 1) * 0.32f;
        NextWaveSpeedMultiplier = 1f + (wave - 1) * 0.045f;
    }

    void SpawnEnemy(float hpMul, float speedMul, int wave)
    {
        GameObject enemyObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        enemyObj.name = "Enemy";
        enemyObj.transform.position = spawnPoint.position;
        enemyObj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        TowerFactory.SetColor(enemyObj, wave >= 5 ? new Color(0.62f, 0.16f, 0.12f) : new Color(0.65f, 0.32f, 0.18f));
        Enemy enemy = enemyObj.AddComponent<Enemy>();
        enemy.Init(waypoints, hpMul, speedMul);
    }
}
