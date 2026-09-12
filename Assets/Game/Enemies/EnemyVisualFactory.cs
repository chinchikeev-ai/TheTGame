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
            PrimitiveType primitive = archetype == EnemyArchetype.BatteringRam ? PrimitiveType.Cube : PrimitiveType.Capsule;
            instance = GameObject.CreatePrimitive(primitive);
            if (data != null) TowerFactory.SetColor(instance, data.color);
        }

        instance.name = data != null ? data.displayName : "Enemy";
        AddArchetypeReadability(instance.transform, archetype);
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

    static void AddArchetypeReadability(Transform root, EnemyArchetype archetype)
    {
        switch (archetype)
        {
            case EnemyArchetype.Runner:
                AddMarker(root,"RunnerPlume",PrimitiveType.Cube,new Vector3(0f,1.75f,0f),new Vector3(.10f,.34f,.38f),new Color(.84f,.72f,.22f));
                break;
            case EnemyArchetype.HeavyHoplite:
                AddMarker(root,"HeavyShield",PrimitiveType.Cube,new Vector3(.42f,.90f,.18f),new Vector3(.48f,.72f,.12f),new Color(.28f,.32f,.38f));
                AddMarker(root,"HeavyCrest",PrimitiveType.Cube,new Vector3(0f,1.86f,0f),new Vector3(.16f,.30f,.55f),new Color(.38f,.08f,.06f));
                break;
            case EnemyArchetype.ShieldBearer:
                AddMarker(root,"Shield",PrimitiveType.Cylinder,new Vector3(.42f,.92f,.20f),new Vector3(.42f,.08f,.42f),new Color(.58f,.42f,.18f),Quaternion.Euler(90f,0f,0f));
                break;
            case EnemyArchetype.Archer:
                AddMarker(root,"Bow",PrimitiveType.Cube,new Vector3(.38f,.92f,.12f),new Vector3(.08f,.68f,.08f),new Color(.50f,.30f,.12f),Quaternion.Euler(0f,0f,20f));
                break;
            case EnemyArchetype.BatteringRam:
                AddMarker(root,"RamHead",PrimitiveType.Cylinder,new Vector3(0f,.55f,.85f),new Vector3(.18f,.42f,.18f),new Color(.32f,.28f,.22f),Quaternion.Euler(90f,0f,0f));
                break;
            case EnemyArchetype.Boss:
                AddMarker(root,"MenelausCrest",PrimitiveType.Cube,new Vector3(0f,2.05f,0f),new Vector3(.22f,.40f,.68f),new Color(.72f,.06f,.05f));
                AddMarker(root,"MenelausCape",PrimitiveType.Cube,new Vector3(0f,1.05f,-.24f),new Vector3(.70f,1.10f,.08f),new Color(.55f,.04f,.04f));
                AddMarker(root,"MenelausGold",PrimitiveType.Cylinder,new Vector3(0f,1.62f,0f),new Vector3(.38f,.06f,.38f),new Color(.90f,.66f,.16f));
                break;
        }
    }

    static GameObject AddMarker(Transform parent,string name,PrimitiveType primitive,Vector3 position,Vector3 scale,Color color,Quaternion? rotation=null)
    {
        GameObject go=GameObject.CreatePrimitive(primitive);
        go.name=name;
        go.transform.SetParent(parent,false);
        go.transform.localPosition=position;
        go.transform.localScale=scale;
        if(rotation.HasValue) go.transform.localRotation=rotation.Value;
        Object.Destroy(go.GetComponent<Collider>());
        TowerFactory.SetColor(go,color);
        return go;
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
