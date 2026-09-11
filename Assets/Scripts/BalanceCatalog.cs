using System.Collections.Generic;
using UnityEngine;

public static class BalanceCatalog
{
    static readonly Dictionary<TowerType, TowerData> towers = new Dictionary<TowerType, TowerData>();
    static EnemyData normal;
    static EnemyData heavy;
    static EnemyData boss;

    public static TowerData GetTower(TowerType type)
    {
        if (towers.Count == 0) BuildTowers();
        return towers[type];
    }

    public static EnemyData NormalEnemy => normal ?? (normal = MakeEnemy("greek_infantry", "Greek Infantry", 1f, 1f, 20, 1, 1f, new Color(0.65f,0.32f,0.18f)));
    public static EnemyData HeavyEnemy => heavy ?? (heavy = MakeEnemy("heavy_hoplite", "Heavy Hoplite", 2.7f, 0.82f, 55, 2, 1.25f, new Color(0.35f,0.12f,0.08f)));
    public static EnemyData BossEnemy => boss ?? (boss = MakeEnemy("menelaus", "Menelaus", 7f, 0.72f, 350, 5, 1.8f, new Color(0.55f,0.05f,0.08f)));

    public static WaveData GetWave(int wave, int maxWaves = 7)
    {
        WaveData d = ScriptableObject.CreateInstance<WaveData>();
        d.waveNumber = wave;
        d.enemyCount = 5 + (wave - 1) * 2 + (wave == maxWaves ? 1 : 0);
        d.hpMultiplier = 1f + (wave - 1) * 0.32f;
        d.speedMultiplier = 1f + (wave - 1) * 0.045f;
        d.heavyEvery = wave >= 6 ? 3 : wave >= 3 ? 4 : 0;
        d.hasBoss = wave == maxWaves;
        return d;
    }

    static void BuildTowers()
    {
        towers[TowerType.MachineGun] = MakeTower(TowerType.MachineGun, "Archer Tower", 100, 16f, 5.8f, 4.2f, 18f, 0f, 1f, 0f);
        towers[TowerType.Cannon] = MakeTower(TowerType.Cannon, "Ballista", 220, 72f, 6.8f, 0.75f, 10f, 2.2f, 1f, 0f);
        towers[TowerType.Slow] = MakeTower(TowerType.Slow, "Priests of Apollo", 160, 9f, 5.3f, 1.5f, 13f, 0f, 0.55f, 1.6f);
    }

    static TowerData MakeTower(TowerType type, string displayName, int cost, float damage, float range, float rate, float projectileSpeed, float splash, float slow, float slowDuration)
    {
        TowerData d = ScriptableObject.CreateInstance<TowerData>();
        d.type = type; d.displayName = displayName; d.cost = cost; d.damage = damage; d.range = range;
        d.attacksPerSecond = rate; d.projectileSpeed = projectileSpeed; d.splashRadius = splash;
        d.slowMultiplier = slow; d.slowDuration = slowDuration; d.sellRatio = 0.65f;
        return d;
    }

    static EnemyData MakeEnemy(string id, string displayName, float hp, float speed, int reward, int baseDamage, float scale, Color color)
    {
        EnemyData d = ScriptableObject.CreateInstance<EnemyData>();
        d.id = id; d.displayName = displayName; d.hpMultiplier = hp; d.speedMultiplier = speed;
        d.reward = reward; d.baseDamage = baseDamage; d.scale = scale; d.color = color;
        return d;
    }
}
