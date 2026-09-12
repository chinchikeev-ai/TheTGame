using UnityEngine;

public static class EnemyVisualFactory
{
    const string GreekResourceRoot = "TroyCharacters/Factions/Greek/";
    const string LegacyResourceRoot = "TroyCharacters/";

    public static GameObject CreateEnemyObject(EnemyData data)
    {
        EnemyArchetype archetype = data != null ? data.archetype : EnemyArchetype.Infantry;
        string prefabName = GetPrefabName(archetype);
        GameObject prefab = Resources.Load<GameObject>(GreekResourceRoot + prefabName);
        if (prefab == null)
            prefab = Resources.Load<GameObject>(LegacyResourceRoot + prefabName);

        GameObject instance;
        if (prefab != null)
        {
            instance = Object.Instantiate(prefab);
            EnsureGameplayCollider(instance);
        }
        else
        {
            Color fallbackColor = data != null ? data.color : new Color(.48f,.58f,.72f);
            instance = RuntimeWarriorVisualFactory.CreateEnemyFallback(archetype, fallbackColor);
        }

        instance.name = data != null ? data.displayName : "Enemy";
        EnemyMotionAnimator.Attach(instance, archetype);
        return instance;
    }

    public static string GetPrefabName(EnemyArchetype archetype)
    {
        switch (archetype)
        {
            case EnemyArchetype.Runner: return "Enemy_Runner";
            case EnemyArchetype.HeavyHoplite: return "Enemy_HeavyHoplite";
            case EnemyArchetype.ShieldBearer: return "Enemy_ShieldBearer";
            case EnemyArchetype.Archer: return "Enemy_Archer";
            case EnemyArchetype.BatteringRam: return "Enemy_BatteringRam";
            case EnemyArchetype.Boss: return "Enemy_Boss";
            default: return "Enemy_Infantry";
        }
    }

    static void EnsureGameplayCollider(GameObject instance)
    {
        if (instance.GetComponentInChildren<Collider>() != null) return;
        CapsuleCollider collider = instance.AddComponent<CapsuleCollider>();
        collider.center = new Vector3(0f, 0.9f, 0f);
        collider.height = 1.8f;
        collider.radius = 0.35f;
    }
}
