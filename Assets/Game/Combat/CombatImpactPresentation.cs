using UnityEngine;

public static class CombatImpactPresentation
{
    public static void ProjectileHit(Vector3 point, TowerType sourceType)
    {
        switch (sourceType)
        {
            case TowerType.Cannon:
                Burst(point, new Color(1f,.34f,.05f), 5, .22f, .85f);
                RuntimeEffects.Instance?.PlayHeroPulse(point, new Color(1f,.26f,.03f), 1.5f, .20f);
                break;
            case TowerType.FireTower:
                Burst(point, new Color(1f,.18f,.02f), 6, .16f, .65f);
                RuntimeEffects.Instance?.PlayHeroPulse(point, new Color(1f,.42f,.04f), 1.2f, .18f);
                break;
            case TowerType.SpearThrower:
                Burst(point, new Color(.92f,.70f,.27f), 3, .10f, .45f);
                break;
            case TowerType.Slow:
                Burst(point, new Color(.22f,.72f,1f), 4, .12f, .55f);
                break;
            default:
                Burst(point, new Color(1f,.76f,.20f), 2, .07f, .32f);
                break;
        }
    }

    public static void EnemyDeath(Vector3 point, EnemyArchetype archetype)
    {
        bool heavy = archetype == EnemyArchetype.HeavyHoplite || archetype == EnemyArchetype.ShieldBearer || archetype == EnemyArchetype.BatteringRam;
        bool boss = archetype == EnemyArchetype.Boss;
        if (!heavy && !boss) return;
        Color color = boss ? new Color(.78f,.08f,.03f) : new Color(.55f,.43f,.29f);
        Burst(point + Vector3.up*.15f, color, boss ? 9 : 5, boss ? .24f : .15f, boss ? 1.0f : .65f);
        if (boss) RuntimeEffects.Instance?.PlayHeroPulse(point, new Color(1f,.18f,.025f), 3.2f, .45f);
    }

    static void Burst(Vector3 point, Color color, int count, float size, float spread)
    {
        for (int i=0;i<count;i++)
        {
            GameObject shard = GameObject.CreatePrimitive(i%2==0 ? PrimitiveType.Sphere : PrimitiveType.Cube);
            shard.name = "Combat Impact";
            shard.transform.position = point + new Vector3(Random.Range(-spread,spread), Random.Range(.04f,.35f), Random.Range(-spread,spread));
            shard.transform.localScale = Vector3.one * Random.Range(size*.55f,size);
            Collider collider = shard.GetComponent<Collider>();
            if (collider != null) Object.Destroy(collider);
            TowerFactory.SetColor(shard,color);
            Object.Destroy(shard,.32f + i*.035f);
        }
    }
}
