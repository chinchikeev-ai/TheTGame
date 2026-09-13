using UnityEngine;

public sealed class HeavyEnemyGroundVfx : MonoBehaviour
{
    Enemy enemy;
    float nextDust;

    public static void Attach(Enemy target)
    {
        if (target == null || target.GetComponent<HeavyEnemyGroundVfx>() != null) return;
        EnemyArchetype type = target.Archetype;
        if (type != EnemyArchetype.HeavyHoplite && type != EnemyArchetype.ShieldBearer && type != EnemyArchetype.BatteringRam && type != EnemyArchetype.Boss) return;
        target.gameObject.AddComponent<HeavyEnemyGroundVfx>();
    }

    void Awake() => enemy = GetComponent<Enemy>();

    void Update()
    {
        if (enemy == null || !enemy.IsAlive || Time.time < nextDust) return;
        nextDust = Time.time + (enemy.Archetype == EnemyArchetype.Boss ? .28f : .42f);
        SpawnDust();
    }

    void SpawnDust()
    {
        Vector3 p = transform.position + new Vector3(Random.Range(-.16f,.16f),.04f,Random.Range(-.16f,.16f));
        GameObject dust = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dust.name = "Heavy Step Dust";
        dust.transform.position = p;
        float scale = enemy.Archetype == EnemyArchetype.Boss ? .22f : .15f;
        dust.transform.localScale = new Vector3(scale,.06f,scale);
        Collider c = dust.GetComponent<Collider>();
        if (c != null) Destroy(c);
        TowerFactory.SetColor(dust,new Color(.50f,.43f,.32f));
        Destroy(dust,.24f);
    }
}

public sealed class HeavyEnemyGroundVfxBinder : MonoBehaviour
{
    float nextScan;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<HeavyEnemyGroundVfxBinder>() == null)
            new GameObject("HeavyEnemyGroundVfxBinder").AddComponent<HeavyEnemyGroundVfxBinder>();
    }

    void Update()
    {
        if (Time.time < nextScan) return;
        nextScan = Time.time + .65f;
        foreach (Enemy enemy in EnemyRegistry.All) HeavyEnemyGroundVfx.Attach(enemy);
    }
}
