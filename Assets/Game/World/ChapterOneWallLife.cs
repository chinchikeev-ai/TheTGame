using UnityEngine;

// Sole owner of the Chapter I outer wall and wall-life dressing.
// The central gatehouse itself belongs to TroyGateHeroBuilder.
public static class ChapterOneWallLife
{
    const string TrojanProductionRoot = "TroyProduction/Characters/Trojan/";
    const string TrojanGeneratedRoot = "TroyCharacters/Factions/Trojan/";

    public static void Build()
    {
        if (GameObject.Find("Chapter01_WallLife") != null) return;
        GameObject root = new GameObject("Chapter01_WallLife");
        float gateX = MapBuilder.CellToWorld(new Vector2Int(17, 6)).x;

        BuildWallStructure(root.transform, gateX);
        AddGuard(root.transform, new Vector3(gateX + .55f, 2.45f, -5.8f));
        AddGuard(root.transform, new Vector3(gateX + .55f, 2.45f, -2.9f));
        AddGuard(root.transform, new Vector3(gateX + .55f, 2.45f, 2.9f));
        AddGuard(root.transform, new Vector3(gateX + .55f, 2.45f, 5.8f));
        AddStandard(root.transform, new Vector3(gateX + .65f, 3.0f, -7.0f));
        AddStandard(root.transform, new Vector3(gateX + .65f, 3.0f, 7.0f));
        AddBeacon(root.transform, new Vector3(gateX + 2.7f, 3.25f, -3.4f));
        AddBeacon(root.transform, new Vector3(gateX + 2.7f, 3.25f, 3.4f));
    }

    static void BuildWallStructure(Transform parent, float gateX)
    {
        Color stone = new Color(.58f,.47f,.29f);
        Color crown = new Color(.72f,.59f,.35f);
        float wallX = gateX + 1.35f;

        // Keep a clear central opening for the hero gate instead of drawing a second wall through it.
        Part(parent,"Troy Outer Wall North",PrimitiveType.Cube,new Vector3(wallX,1.05f,7.15f),new Vector3(.9f,2.2f,4.45f),stone);
        Part(parent,"Troy Outer Wall South",PrimitiveType.Cube,new Vector3(wallX,1.05f,-7.15f),new Vector3(.9f,2.2f,4.45f),stone);

        for (int side = -1; side <= 1; side += 2)
        {
            float z = side * 8.95f;
            Part(parent,"Outer Wall Tower",PrimitiveType.Cylinder,new Vector3(wallX-.05f,1.30f,z),new Vector3(1.0f,1.30f,1.0f),stone*1.05f);
            Part(parent,"Outer Tower Crown",PrimitiveType.Cylinder,new Vector3(wallX-.05f,2.68f,z),new Vector3(1.12f,.11f,1.12f),crown);
        }
    }

    static void AddGuard(Transform parent, Vector3 position)
    {
        GameObject prefab = Resources.Load<GameObject>(TrojanProductionRoot + "Trojan_Guard");
        if (prefab == null) prefab = Resources.Load<GameObject>(TrojanGeneratedRoot + "Trojan_Guard");
        GameObject guard = prefab != null
            ? Object.Instantiate(prefab)
            : RuntimeWarriorVisualFactory.CreateEnemyFallback(EnemyArchetype.ShieldBearer, new Color(.64f,.22f,.12f));

        guard.name = "Trojan Wall Guard";
        guard.transform.SetParent(parent, false);
        guard.transform.position = position;
        guard.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        guard.transform.localScale *= .82f;
        foreach (Collider collider in guard.GetComponentsInChildren<Collider>(true)) Object.Destroy(collider);
        EnemyMotionAnimator.Attach(guard, EnemyArchetype.ShieldBearer);
    }

    static void AddStandard(Transform parent, Vector3 position)
    {
        Part(parent,"Royal Standard Pole",PrimitiveType.Cylinder,position,new Vector3(.04f,.9f,.04f),new Color(.30f,.17f,.07f));
        GameObject banner = Part(parent,"Royal Banner",PrimitiveType.Cube,position+new Vector3(-.02f,.48f,0f),new Vector3(.06f,.48f,.48f),new Color(.55f,.06f,.04f));
        Part(parent,"Royal Gold Stripe",PrimitiveType.Cube,position+new Vector3(-.055f,.48f,0f),new Vector3(.025f,.08f,.50f),new Color(.88f,.62f,.14f));
        banner.AddComponent<ChapterOneAmbientMotion>().kind = ChapterOneAmbientMotion.MotionKind.Banner;
    }

    static void AddBeacon(Transform parent, Vector3 position)
    {
        Part(parent,"Beacon Stone",PrimitiveType.Cylinder,position,new Vector3(.42f,.18f,.42f),new Color(.48f,.38f,.24f));
        GameObject flame = Part(parent,"Beacon Flame",PrimitiveType.Sphere,position+new Vector3(0f,.38f,0f),new Vector3(.22f,.38f,.22f),new Color(1f,.34f,.05f));
        flame.AddComponent<ChapterOneAmbientMotion>().kind = ChapterOneAmbientMotion.MotionKind.Flame;
        Light light = flame.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f,.46f,.12f);
        light.range = 5f;
        light.intensity = 1.8f;
    }

    static GameObject Part(Transform parent, string name, PrimitiveType type, Vector3 position, Vector3 scale, Color color)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.position = position;
        go.transform.localScale = scale;
        Collider collider = go.GetComponent<Collider>();
        if (collider != null) Object.Destroy(collider);
        TowerFactory.SetColor(go, color);
        return go;
    }
}
