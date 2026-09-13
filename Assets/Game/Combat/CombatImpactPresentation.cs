using UnityEngine;

public static class CombatImpactPresentation
{
    public static void ProjectileHit(Vector3 point, TowerType sourceType)
    {
        switch (sourceType)
        {
            case TowerType.Cannon:
                Burst(point, new Color(1f,.34f,.05f), 4, .22f, .72f);
                RuntimeEffects.Instance?.PlayHeroPulse(point, new Color(1f,.26f,.03f), 1.5f, .20f);
                break;
            case TowerType.FireTower:
                Burst(point, new Color(1f,.18f,.02f), 4, .16f, .52f);
                RuntimeEffects.Instance?.PlayHeroPulse(point, new Color(1f,.42f,.04f), 1.2f, .18f);
                break;
            case TowerType.SpearThrower:
                Burst(point, new Color(.92f,.70f,.27f), 2, .10f, .34f);
                break;
            case TowerType.Slow:
                Burst(point, new Color(.22f,.72f,1f), 3, .12f, .42f);
                break;
            default:
                Burst(point, new Color(1f,.76f,.20f), 1, .07f, .22f);
                break;
        }
    }

    public static void BurnStatus(Vector3 point)
    {
        for (int i = 0; i < 2; i++)
        {
            float size = Random.Range(.07f, .115f);
            Vector3 position = point + new Vector3(Random.Range(-.18f,.18f), Random.Range(.25f,.72f), Random.Range(-.18f,.18f));
            Color color = i == 0 ? new Color(1f,.18f,.015f) : new Color(1f,.58f,.05f);
            CombatVfxPool.Spawn(
                PrimitiveType.Sphere,
                position,
                new Vector3(size, size * 1.65f, size),
                new Vector3(size * .18f, size * .32f, size * .18f),
                color,
                .28f,
                new Vector3(Random.Range(-.08f,.08f), .42f, Random.Range(-.08f,.08f)));
        }
    }

    public static void EnemyDeath(Vector3 point, EnemyArchetype archetype)
    {
        bool heavy = archetype == EnemyArchetype.HeavyHoplite || archetype == EnemyArchetype.ShieldBearer || archetype == EnemyArchetype.BatteringRam;
        bool boss = archetype == EnemyArchetype.Boss;
        if (!heavy && !boss) return;

        Color color = boss ? new Color(.78f,.08f,.03f) : new Color(.55f,.43f,.29f);
        Burst(point + Vector3.up*.15f, color, boss ? 7 : 4, boss ? .24f : .15f, boss ? .85f : .52f);
    }

    static void Burst(Vector3 point, Color color, int count, float size, float spread)
    {
        for (int i = 0; i < count; i++)
        {
            PrimitiveType shape = i % 2 == 0 ? PrimitiveType.Sphere : PrimitiveType.Cube;
            Vector3 offset = new Vector3(
                Random.Range(-spread, spread),
                Random.Range(.04f, .35f),
                Random.Range(-spread, spread));
            float particleSize = Random.Range(size * .55f, size);
            float lifetime = .27f + i * .025f;
            Vector3 drift = new Vector3(
                Random.Range(-.14f,.14f),
                Random.Range(.18f,.42f),
                Random.Range(-.14f,.14f));

            CombatVfxPool.Spawn(
                shape,
                point + offset,
                Vector3.one * particleSize,
                Vector3.one * (particleSize * .16f),
                color,
                lifetime,
                drift);
        }
    }
}
