using UnityEngine;

public class ShieldWallZone : MonoBehaviour
{
    public float duration = 6f;
    public float radius = 3.2f;
    public float slowMultiplier = 0.20f;

    float expiresAt;

    void Start()
    {
        expiresAt = Time.time + duration;
    }

    void Update()
    {
        if (Time.time >= expiresAt)
        {
            Destroy(gameObject);
            return;
        }

        foreach (Enemy enemy in EnemyRegistry.All)
        {
            if (enemy == null) continue;
            if ((enemy.transform.position - transform.position).sqrMagnitude <= radius * radius)
                enemy.ApplySlow(slowMultiplier, 0.35f);
        }
    }
}
