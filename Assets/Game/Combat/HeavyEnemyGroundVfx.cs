using UnityEngine;

public sealed class HeavyEnemyGroundVfx : MonoBehaviour
{
    Enemy enemy;
    float nextDust;
    Vector3 lastPosition;

    public static void Attach(Enemy target)
    {
        if (target == null || target.GetComponent<HeavyEnemyGroundVfx>() != null) return;
        EnemyArchetype type = target.Archetype;
        if (type != EnemyArchetype.HeavyHoplite &&
            type != EnemyArchetype.ShieldBearer &&
            type != EnemyArchetype.BatteringRam &&
            type != EnemyArchetype.Boss) return;

        target.gameObject.AddComponent<HeavyEnemyGroundVfx>();
    }

    void Awake()
    {
        enemy = GetComponent<Enemy>();
        lastPosition = transform.position;
    }

    void Update()
    {
        if (enemy == null || !enemy.IsAlive) return;

        Vector3 current = transform.position;
        Vector3 delta = current - lastPosition;
        delta.y = 0f;
        if (delta.sqrMagnitude < .0025f) return;

        lastPosition = current;
        if (Time.time < nextDust) return;

        nextDust = Time.time + (enemy.Archetype == EnemyArchetype.Boss ? .28f : .42f);
        SpawnDust();
    }

    void SpawnDust()
    {
        float scale = enemy.Archetype == EnemyArchetype.Boss ? .22f : .15f;
        Vector3 position = transform.position + new Vector3(Random.Range(-.16f,.16f), .04f, Random.Range(-.16f,.16f));
        Vector3 startScale = new Vector3(scale, .06f, scale);
        Vector3 endScale = new Vector3(scale * 1.55f, .018f, scale * 1.55f);
        Vector3 drift = new Vector3(Random.Range(-.08f,.08f), .06f, Random.Range(-.08f,.08f));

        CombatVfxPool.Spawn(
            PrimitiveType.Sphere,
            position,
            startScale,
            endScale,
            new Color(.50f,.43f,.32f),
            .24f,
            drift);
    }
}
