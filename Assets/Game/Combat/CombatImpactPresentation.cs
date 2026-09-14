using UnityEngine;

public static class CombatImpactPresentation
{
    public static void ProjectileHit(Vector3 point, TowerType sourceType)
    {
        switch (sourceType)
        {
            case TowerType.Cannon:
                ImpactBeat(point, new Color(1f,.34f,.05f), .95f, true);
                Burst(point, new Color(1f,.34f,.05f), 4, .22f, .72f);
                GroundPulse(point, new Color(1f,.26f,.03f), 1.5f, .20f);
                break;
            case TowerType.FireTower:
                ImpactBeat(point, new Color(1f,.24f,.02f), .78f, true);
                Burst(point, new Color(1f,.18f,.02f), 4, .16f, .52f);
                GroundPulse(point, new Color(1f,.42f,.04f), 1.2f, .18f);
                break;
            case TowerType.SpearThrower:
                ImpactBeat(point, new Color(.96f,.76f,.34f), .48f);
                Burst(point, new Color(.92f,.70f,.27f), 2, .10f, .34f);
                break;
            case TowerType.Slow:
                ImpactBeat(point, new Color(.28f,.78f,1f), .52f);
                Burst(point, new Color(.22f,.72f,1f), 3, .12f, .42f);
                break;
            default:
                ImpactBeat(point, new Color(1f,.82f,.32f), .36f);
                Burst(point, new Color(1f,.76f,.20f), 1, .07f, .22f);
                break;
        }
    }

    public static void ShotFlash(Vector3 point, TowerType sourceType)
    {
        Color color = new Color(1f,.68f,.18f);
        float size = .28f;
        switch (sourceType)
        {
            case TowerType.Cannon: color = new Color(1f,.42f,.08f); size = .48f; break;
            case TowerType.Slow: color = new Color(.30f,.70f,1f); size = .34f; break;
            case TowerType.SpearThrower: color = new Color(.92f,.78f,.36f); size = .24f; break;
            case TowerType.FireTower: color = new Color(1f,.18f,.02f); size = .52f; break;
            case TowerType.TrojanGuard: color = new Color(.76f,.38f,.12f); size = .26f; break;
        }

        CombatVfxPool.Spawn(
            PrimitiveType.Sphere,
            point,
            Vector3.one * size,
            Vector3.one * (size * .20f),
            color,
            .07f,
            Vector3.zero);
    }

    public static void Pulse(Vector3 point, Color color, float radius, float duration)
    {
        GroundPulse(point, color, radius, duration);
    }

    public static void GenericHit(Vector3 point, bool heavy)
    {
        float size = heavy ? .75f : .34f;
        Color color = heavy ? new Color(1f,.24f,.05f) : new Color(1f,.62f,.18f);
        ImpactBeat(point, color, heavy ? .90f : .42f, heavy);
        CombatVfxPool.Spawn(
            PrimitiveType.Sphere,
            point,
            Vector3.one * .10f,
            Vector3.one * size,
            color,
            .18f,
            Vector3.up * .06f);
    }

    public static void GenericDeath(Vector3 point, bool boss)
    {
        float size = boss ? 2.2f : .88f;
        Color color = boss ? new Color(.88f,.06f,.06f) : new Color(1f,.72f,.18f);
        if (boss) ImpactBeat(point + Vector3.up * .12f, color, 1.35f, true);
        CombatVfxPool.Spawn(
            PrimitiveType.Sphere,
            point,
            Vector3.one * .10f,
            Vector3.one * size,
            color,
            .18f,
            Vector3.up * .06f);

        if (boss) GroundPulse(point, new Color(.95f,.62f,.12f), 3.8f, .5f);
    }

    public static void MeleeHit(Vector3 point, TowerType sourceType)
    {
        Color color = sourceType == TowerType.TrojanGuard
            ? new Color(.86f,.42f,.13f)
            : new Color(.92f,.76f,.32f);
        ImpactBeat(point, color, .44f);
        Burst(point, color, 2, .085f, .20f);
    }

    public static void HeroHit(Vector3 point, bool heavy)
    {
        Color color = heavy ? new Color(.92f,.30f,.08f) : new Color(1f,.72f,.24f);
        ImpactBeat(point, color, heavy ? .82f : .46f, heavy);
        Burst(
            point,
            color,
            heavy ? 3 : 1,
            heavy ? .14f : .08f,
            heavy ? .34f : .18f);
    }

    public static void GateHit(Vector3 point, bool heavy)
    {
        Color color = heavy ? new Color(.84f,.28f,.06f) : new Color(.92f,.58f,.18f);
        ImpactBeat(point + Vector3.up * .12f, color, heavy ? 1.05f : .50f, heavy);
        Burst(point + Vector3.up * .12f, color, heavy ? 4 : 2, heavy ? .15f : .09f, heavy ? .48f : .24f);
        if (heavy) GroundPulse(point, new Color(.72f,.30f,.08f), 1.35f, .24f);
    }

    public static void EnemyBreach(Vector3 point, EnemyArchetype archetype)
    {
        bool heavy = archetype == EnemyArchetype.HeavyHoplite ||
                     archetype == EnemyArchetype.ShieldBearer ||
                     archetype == EnemyArchetype.BatteringRam;
        Color color = heavy ? new Color(.54f,.40f,.27f) : new Color(.74f,.56f,.32f);
        if (heavy) ImpactBeat(point + Vector3.up * .12f, color, .78f, true);
        Burst(
            point + Vector3.up * .12f,
            color,
            heavy ? 3 : 1,
            heavy ? .14f : .08f,
            heavy ? .42f : .20f);
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

        Color color = boss
            ? new Color(.78f,.08f,.03f)
            : heavy
                ? new Color(.55f,.43f,.29f)
                : new Color(1f,.72f,.18f);
        int count = boss ? 7 : heavy ? 4 : 2;
        float size = boss ? .24f : heavy ? .15f : .10f;
        float spread = boss ? .85f : heavy ? .52f : .30f;

        if (boss) ImpactBeat(point + Vector3.up * .20f, color, 1.55f, true);
        else if (heavy) ImpactBeat(point + Vector3.up * .15f, color, .82f, true);
        Burst(point + Vector3.up * .15f, color, count, size, spread);
        if (boss) GroundPulse(point, new Color(.95f,.62f,.12f), 3.8f, .5f);
    }

    static void ImpactBeat(Vector3 point, Color color, float size, bool heavy = false)
    {
        float coreSize = size * (heavy ? .42f : .34f);
        CombatVfxPool.Spawn(
            PrimitiveType.Sphere,
            point + Vector3.up * .10f,
            Vector3.one * (coreSize * .24f),
            Vector3.one * coreSize,
            color,
            heavy ? .14f : .10f,
            Vector3.up * .035f);

        int shardCount = heavy ? 4 : 2;
        float angleOffset = Random.Range(0f, 90f);
        for (int i = 0; i < shardCount; i++)
        {
            float angle = angleOffset + 360f * i / shardCount + Random.Range(-14f, 14f);
            Vector3 direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            Vector3 startScale = new Vector3(size * .075f, size * .045f, size * .34f);
            Vector3 endScale = new Vector3(size * .018f, size * .018f, size * .07f);
            CombatVfxPool.Spawn(
                PrimitiveType.Cube,
                point + Vector3.up * .14f + direction * (size * .08f),
                startScale,
                endScale,
                color,
                heavy ? .20f : .14f,
                direction * (size * (heavy ? .58f : .40f)) + Vector3.up * .08f);
        }
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

    static void GroundPulse(Vector3 point, Color color, float radius, float duration)
    {
        CombatVfxPool.Spawn(
            PrimitiveType.Cylinder,
            point + Vector3.up * .045f,
            new Vector3(.22f,.018f,.22f),
            new Vector3(radius,.018f,radius),
            color,
            duration,
            Vector3.zero);
    }
}
