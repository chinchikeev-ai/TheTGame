using System.Collections.Generic;
using UnityEngine;

public static class BalanceCatalog
{
    static readonly Dictionary<TowerType, TowerData> towers = new Dictionary<TowerType, TowerData>();
    static readonly Dictionary<EnemyArchetype, EnemyData> enemies = new Dictionary<EnemyArchetype, EnemyData>();

    public static TowerData GetTower(TowerType type)
    {
        if (towers.Count == 0) BuildTowers();
        return towers[type];
    }

    public static EnemyData GetEnemy(EnemyArchetype type)
    {
        if (enemies.Count == 0) BuildEnemies();
        return enemies[type];
    }

    public static EnemyData GetEnemyForWave(int wave, int index, int count, bool boss)
    {
        if (boss) return GetEnemy(EnemyArchetype.Boss);
        if (wave >= 6 && index % 7 == 5) return GetEnemy(EnemyArchetype.BatteringRam);
        if (wave >= 4 && index % 5 == 3) return GetEnemy(EnemyArchetype.ShieldBearer);
        if (wave >= 3 && index % 6 == 2) return GetEnemy(EnemyArchetype.Archer);
        if (wave >= 3 && index % 4 == 3) return GetEnemy(EnemyArchetype.HeavyHoplite);
        if (wave >= 2 && index % 4 == 1) return GetEnemy(EnemyArchetype.Runner);
        return GetEnemy(EnemyArchetype.Infantry);
    }

    public static WaveData GetWave(int wave, int maxWaves = 7)
    {
        WaveData d = ScriptableObject.CreateInstance<WaveData>();
        d.waveNumber = wave;
        d.enemyCount = 8 + (wave - 1) * 3 + (wave == maxWaves ? 1 : 0);
        d.hpMultiplier = 1f + (wave - 1) * 0.26f;
        d.speedMultiplier = 1f + (wave - 1) * 0.035f;
        d.heavyEvery = wave >= 6 ? 3 : wave >= 3 ? 4 : 0;
        d.hasBoss = wave == maxWaves;
        float[] target = { 60f, 75f, 90f, 100f, 110f, 120f, 180f };
        float[] cadence = { 4.5f, 4.2f, 3.9f, 3.6f, 3.4f, 3.2f, 4.0f };
        int i = Mathf.Clamp(wave - 1, 0, target.Length - 1);
        d.targetDuration = target[i];
        d.spawnInterval = cadence[i];
        d.preparationTime = wave == 1 ? 35f : wave >= 6 ? 25f : 20f;
        return d;
    }

    static void BuildTowers()
    {
        towers[TowerType.MachineGun] = MakeTower(TowerType.MachineGun, "Archer Tower", 100, 16f, 5.8f, 4.2f, 18f, 0f, 1f, 0f);
        towers[TowerType.Cannon] = MakeTower(TowerType.Cannon, "Ballista", 220, 72f, 6.8f, 0.75f, 10f, 2.2f, 1f, 0f);
        towers[TowerType.Slow] = MakeTower(TowerType.Slow, "Priests of Apollo", 160, 9f, 5.3f, 1.5f, 13f, 0f, 0.55f, 1.6f);
    }

    static void BuildEnemies()
    {
        enemies[EnemyArchetype.Infantry] = MakeEnemy(EnemyArchetype.Infantry, "greek_infantry", "Greek Infantry", 1f, 1f, 20, 1, 0.85f, 0f, 0f, 0f, new Color(.65f,.32f,.18f));
        enemies[EnemyArchetype.Runner] = MakeEnemy(EnemyArchetype.Runner, "runner", "Greek Runner", .65f, 1.65f, 18, 1, .70f, 0f, 0f, 0f, new Color(.83f,.65f,.20f));
        enemies[EnemyArchetype.HeavyHoplite] = MakeEnemy(EnemyArchetype.HeavyHoplite, "heavy_hoplite", "Heavy Hoplite", 3.2f, .68f, 58, 2, 1.15f, .28f, .08f, 0f, new Color(.35f,.12f,.08f));
        enemies[EnemyArchetype.ShieldBearer] = MakeEnemy(EnemyArchetype.ShieldBearer, "shield_bearer", "Shield Bearer", 2.1f, .82f, 42, 2, 1.0f, .14f, .55f, 0f, new Color(.30f,.38f,.48f));
        enemies[EnemyArchetype.Archer] = MakeEnemy(EnemyArchetype.Archer, "greek_archer", "Greek Archer", .9f, .9f, 30, 1, .80f, 0f, 0f, 5.2f, new Color(.42f,.25f,.12f));
        enemies[EnemyArchetype.BatteringRam] = MakeEnemy(EnemyArchetype.BatteringRam, "battering_ram", "Battering Ram", 5.5f, .42f, 110, 7, 1.45f, .38f, .15f, 0f, new Color(.25f,.18f,.10f));
        enemies[EnemyArchetype.Boss] = MakeEnemy(EnemyArchetype.Boss, "menelaus", "Menelaus", 7f, .72f, 350, 5, 1.55f, .22f, .12f, 0f, new Color(.55f,.05f,.08f));
    }

    static TowerData MakeTower(TowerType type, string displayName, int cost, float damage, float range, float rate, float projectileSpeed, float splash, float slow, float slowDuration)
    {
        TowerData d = ScriptableObject.CreateInstance<TowerData>();
        d.type = type; d.displayName = displayName; d.cost = cost; d.damage = damage; d.range = range;
        d.attacksPerSecond = rate; d.projectileSpeed = projectileSpeed; d.splashRadius = splash;
        d.slowMultiplier = slow; d.slowDuration = slowDuration; d.sellRatio = .65f;
        return d;
    }

    static EnemyData MakeEnemy(EnemyArchetype archetype, string id, string displayName, float hp, float speed, int reward, int baseDamage, float scale, float armor, float arrowResistance, float attackRange, Color color)
    {
        EnemyData d = ScriptableObject.CreateInstance<EnemyData>();
        d.archetype = archetype; d.id = id; d.displayName = displayName; d.hpMultiplier = hp; d.speedMultiplier = speed;
        d.reward = reward; d.baseDamage = baseDamage; d.scale = scale; d.armor = armor; d.arrowResistance = arrowResistance;
        d.attackRange = attackRange; d.attackInterval = 2.2f; d.color = color;
        return d;
    }
}
