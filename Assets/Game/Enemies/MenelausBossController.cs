using UnityEngine;

public class MenelausBossController : MonoBehaviour
{
    public float auraRadius = 5.5f;
    public float auraSpeedMultiplier = 1.20f;
    public float auraDamageMultiplier = 1.35f;
    public float reinforcementInterval = 20f;
    public int reinforcementCount = 3;

    Enemy self;
    EnemySpawner spawner;
    float nextAuraPulse;
    float nextReinforcement;

    void Start()
    {
        self = GetComponent<Enemy>();
        spawner = FindFirstObjectByType<EnemySpawner>();
        nextReinforcement = Time.time + 8f;
        RuntimeFileLogger.Event("BOSS", "Menelaus entered battle");
    }

    void Update()
    {
        if (self == null || self.Health <= 0f || GameManager.Instance == null || GameManager.Instance.GameEnded) return;

        if (Time.time >= nextAuraPulse)
        {
            nextAuraPulse = Time.time + 0.5f;
            foreach (Enemy enemy in EnemyRegistry.All)
            {
                if (enemy == null || enemy == self) continue;
                if ((enemy.transform.position - transform.position).sqrMagnitude <= auraRadius * auraRadius)
                    enemy.ApplyCommanderAura(auraSpeedMultiplier, auraDamageMultiplier, 0.8f);
            }
        }

        if (Time.time >= nextReinforcement)
        {
            nextReinforcement = Time.time + reinforcementInterval;
            if (spawner != null)
            {
                spawner.SpawnMenelausReinforcements(reinforcementCount);
                RuntimeFileLogger.Event("BOSS", $"Menelaus called reinforcements count={reinforcementCount}");
            }
        }
    }

    void OnDestroy()
    {
        if (Application.isPlaying)
            RuntimeFileLogger.Event("BOSS", "Menelaus defeated");
    }
}
