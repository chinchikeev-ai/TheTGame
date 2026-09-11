using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform[] waypoints;
    public int maxWaves = 5;
    public int enemiesPerWave = 5;
    public float spawnInterval = 0.8f;
    public float timeBetweenWaves = 3f;

    bool running;

    public void Begin()
    {
        if (!running) StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        running = true;
        GameManager.Instance.MaxWaves = maxWaves;

        for (int wave = 1; wave <= maxWaves; wave++)
        {
            if (GameManager.Instance.GameEnded) yield break;
            GameManager.Instance.CurrentWave = wave;

            int count = enemiesPerWave + (wave - 1) * 2;
            float hpMul = 1f + (wave - 1) * 0.28f;
            float speedMul = 1f + (wave - 1) * 0.05f;

            for (int i = 0; i < count; i++)
            {
                SpawnEnemy(hpMul, speedMul);
                yield return new WaitForSeconds(spawnInterval);
            }

            while (!GameManager.Instance.GameEnded && FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length > 0)
                yield return null;

            if (wave < maxWaves)
                yield return new WaitForSeconds(timeBetweenWaves);
        }

        if (!GameManager.Instance.GameEnded)
            GameManager.Instance.WinGame();
    }

    void SpawnEnemy(float hpMul, float speedMul)
    {
        GameObject enemyObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        enemyObj.name = "Enemy";
        enemyObj.transform.position = spawnPoint.position;
        enemyObj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        Enemy enemy = enemyObj.AddComponent<Enemy>();
        enemy.Init(waypoints, hpMul, speedMul);
    }
}
