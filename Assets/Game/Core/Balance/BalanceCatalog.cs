using System;
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

    [Obsolete("WaveData is legacy compatibility data. Runtime encounter composition now comes from ChapterData.encounters / EncounterData.")]
    public static WaveData GetWave(int wave, int maxWaves = 5)
    {
        WaveData authored = Resources.Load<WaveData>($"Data/Waves/Wave_{wave:00}");
        if (authored == null)
            throw new InvalidOperationException($"Missing legacy WaveData: Resources/Data/Waves/Wave_{wave:00}.");
        return authored;
    }

    // Bootstrap defaults exist ONLY to create a missing authored asset in Editor tooling.
    // Runtime gameplay never uses these values as a fallback.
    public static TowerData GetTowerRuntimeDefault(TowerType type)
    {
        switch (type)
        {
            case TowerType.Cannon: return MakeTower(type, "Ballista", 220, 72f, 6.8f, .75f, 10f, 2.2f, 1f, 0f);
            case TowerType.Slow: return MakeTower(type, "Priests of Apollo", 160, 9f, 5.3f, 1.5f, 13f, 0f, .55f, 1.6f);
            case TowerType.SpearThrower: return MakeTower(type, "Spear Wall", 145, 30f, 2.25f, 1.7f, 0f, 0f, 1f, 0f);
            case TowerType.FireTower: return MakeTower(type, "Fire Tower", 240, 34f, 4.8f, 1.05f, 11f, 2.8f, 1f, 0f);
            case TowerType.TrojanGuard: return MakeTower(type, "Trojan Guard", 130, 42f, 2.25f, 1.25f, 15f, 0f, 1f, 0f);
            default: return MakeTower(type, "Archer Tower", 100, 16f, 5.8f, 4.2f, 18f, 0f, 1f, 0f);
        }
    }

    public static EnemyData GetEnemyRuntimeDefault(EnemyArchetype type)
    {
        switch (type)
        {
            case EnemyArchetype.Runner: return MakeEnemy(type, "runner", "Greek Runner", .65f, 1.65f, 18, 1, .70f, 0f, 0f, 0f, new Color(.83f,.65f,.20f));
            case EnemyArchetype.HeavyHoplite: return MakeEnemy(type, "heavy_hoplite", "Heavy Hoplite", 3.2f, .68f, 58, 2, 1.15f, .28f, .08f, 0f, new Color(.35f,.12f,.08f));
            case EnemyArchetype.ShieldBearer: return MakeEnemy(type, "shield_bearer", "Shield Bearer", 2.1f, .82f, 42, 2, 1f, .14f, .55f, 0f, new Color(.30f,.38f,.48f));
            case EnemyArchetype.Archer: return MakeEnemy(type, "greek_archer", "Greek Archer", .9f, .9f, 30, 1, .80f, 0f, 0f, 5.2f, new Color(.42f,.25f,.12f));
            case EnemyArchetype.BatteringRam: return MakeEnemy(type, "battering_ram", "Battering Ram", 5.5f, .42f, 110, 7, 1.45f, .38f, .15f, 0f, new Color(.25f,.18f,.10f));
            case EnemyArchetype.Boss: return MakeEnemy(type, "menelaus", "Menelaus", 15f, .68f, 500, 6, 1.65f, .32f, .20f, 0f, new Color(.55f,.05f,.08f));
            default: return MakeEnemy(type, "greek_infantry", "Greek Infantry", 1f, 1f, 20, 1, .85f, 0f, 0f, 0f, new Color(.65f,.32f,.18f));
        }
    }

    [Obsolete("Legacy WaveData generator only. New chapters author EncounterData instead.")]
    public static WaveData GetWaveRuntimeDefault(int wave, int maxWaves = 5)
    {
        WaveData d = ScriptableObject.CreateInstance<WaveData>();
        d.waveNumber = wave;
        d.enemyCount = 8 + (wave - 1) * 4 + (wave == maxWaves ? 1 : 0);
        d.hpMultiplier = 1f + (wave - 1) * .30f;
        d.speedMultiplier = 1f + (wave - 1) * .04f;
        d.heavyEvery = wave >= 3 ? 4 : 0;
        d.hasBoss = wave == maxWaves;

        float[] target = { 60f, 80f, 100f, 120f, 200f };
        float[] cadence = { 4.6f, 4.25f, 4.0f, 3.75f, 4.15f };
        float[] prep = { 30f, 20f, 20f, 20f, 25f };
        int i = Mathf.Clamp(wave - 1, 0, target.Length - 1);
        d.targetDuration = target[i];
        d.spawnInterval = cadence[i];
        d.preparationTime = prep[i];
        return d;
    }

    static void BuildTowers()
    {
        foreach (TowerType type in Enum.GetValues(typeof(TowerType)))
        {
            string resourceName = GetTowerResourceName(type);
            TowerData authored = Resources.Load<TowerData>($"Data/Towers/{resourceName}");
            if (authored == null)
                throw new InvalidOperationException($"Missing authored TowerData: Resources/Data/Towers/{resourceName} for TowerType.{type}.");
            towers[type] = authored;
        }
    }

    // TowerType names are serialized legacy identifiers. Asset filenames use player-facing canonical terminology.
    public static string GetTowerResourceName(TowerType type)
    {
        switch (type)
        {
            case TowerType.MachineGun: return "ArcherTower";
            case TowerType.Cannon: return "Ballista";
            case TowerType.Slow: return "PriestsOfApollo";
            case TowerType.SpearThrower: return "SpearWall";
            case TowerType.FireTower: return "FireTower";
            case TowerType.TrojanGuard: return "TrojanGuard";
            default: throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported TowerType");
        }
    }

    static void BuildEnemies()
    {
        foreach (EnemyArchetype type in Enum.GetValues(typeof(EnemyArchetype)))
        {
            EnemyData authored = Resources.Load<EnemyData>($"Data/Enemies/{type}");
            if (authored == null)
                throw new InvalidOperationException($"Missing authored EnemyData: Resources/Data/Enemies/{type}.");
            enemies[type] = authored;
        }
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
