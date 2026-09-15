using UnityEngine;

public static class EnemyVisualFactory
{
    const string ProductionRoot = "TroyProduction/Characters/Greek/";
    const string GeneratedRoot = "TroyCharacters/Factions/Greek/";

    public static GameObject CreateEnemyObject(EnemyData data)
    {
        EnemyArchetype archetype = data != null ? data.archetype : EnemyArchetype.Infantry;
        string prefabName = GetPrefabName(archetype);
        string source;
        string detail;

        GameObject prefab = Resources.Load<GameObject>(ProductionRoot + prefabName);
        if (prefab != null)
        {
            source = "PRODUCTION_RESOURCE";
            detail = ProductionRoot + prefabName;
        }
        else
        {
            prefab = Resources.Load<GameObject>(GeneratedRoot + prefabName);
            if (prefab != null)
            {
                source = "GENERATED_RESOURCE";
                detail = GeneratedRoot + prefabName;
            }
            else
            {
                source = "PROCEDURAL_FALLBACK";
                detail = "RuntimeWarriorVisualFactory";
            }
        }

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
        if (archetype == EnemyArchetype.Boss)
            HeroSignatureArt.Enhance(instance, TroyHeroId.Menelaus);
        EnemyMotionAnimator.Attach(instance, archetype);
        int repairedMaterials = CharacterUrpMaterialAdapter.ApplyTo(instance);
        if (repairedMaterials > 0)
            detail += "; urpMaterialRepair=" + repairedMaterials;
        RuntimeVisualAudit.Report("Enemy:" + archetype, source, detail);
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
        collider.center = new Vector3(0f, .9f, 0f);
        collider.height = 1.8f;
        collider.radius = .35f;
    }
}
