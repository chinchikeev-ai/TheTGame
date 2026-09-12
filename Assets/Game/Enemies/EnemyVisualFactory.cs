using UnityEngine;

public static class EnemyVisualFactory
{
    const string GreekResourceRoot = "TroyCharacters/Factions/Greek/";
    const string LegacyResourceRoot = "TroyCharacters/";

    public static GameObject CreateEnemyObject(EnemyData data)
    {
        string prefabName = GetPrefabName(data != null ? data.archetype : EnemyArchetype.Infantry);
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
            PrimitiveType primitive = data != null && data.archetype == EnemyArchetype.BatteringRam
                ? PrimitiveType.Cube
                : PrimitiveType.Capsule;
            instance = GameObject.CreatePrimitive(primitive);
            if (data != null) TowerFactory.SetColor(instance, data.color);
        }

        instance.name = data != null ? data.displayName : "Enemy";
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
