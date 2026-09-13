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
        AddGuard(root.transform, new Vector3(gateX + .45f, 2.82f, -7.5f));
        AddGuard(root.transform, new Vector3(gateX + .48f, 2.82f, -5.2f));
        AddGuard(root.transform, new Vector3(gateX + .55f, 3.10f, -2.9f));
        AddGuard(root.transform, new Vector3(gateX + .55f, 3.10f, 2.9f));
        AddGuard(root.transform, new Vector3(gateX + .48f, 2.82f, 5.2f));
        AddGuard(root.transform, new Vector3(gateX + .45f, 2.82f, 7.5f));
        AddStandard(root.transform, new Vector3(gateX + .32f, 3.18f, -7.0f));
        AddStandard(root.transform, new Vector3(gateX + .32f, 3.18f, 7.0f));
        AddBeacon(root.transform, new Vector3(gateX + 1.30f, 3.38f, -8.9f));
        AddBeacon(root.transform, new Vector3(gateX + 1.30f, 3.38f, 8.9f));
    }

    static void BuildWallStructure(Transform parent, float gateX)
    {
        Color stone = new Color(.58f,.47f,.29f);
        Color warmStone = new Color(.64f,.50f,.29f);
        Color crown = new Color(.72f,.59f,.35f);
        Color darkStone = new Color(.39f,.31f,.21f);
        Color shadow = new Color(.25f,.21f,.17f);
        float wallX = gateX + 1.35f;

        for (int side = -1; side <= 1; side += 2)
        {
            float z = side * 7.15f;
            Part(parent,"Troy Outer Wall Base",PrimitiveType.Cube,new Vector3(wallX+.18f,.38f,z),new Vector3(1.28f,.76f,4.70f),darkStone);
            Part(parent,"Troy Outer Wall",PrimitiveType.Cube,new Vector3(wallX,1.45f,z),new Vector3(.92f,2.55f,4.55f),stone);
            Part(parent,"Outer Wall Walk",PrimitiveType.Cube,new Vector3(wallX-.04f,2.76f,z),new Vector3(1.08f,.16f,4.72f),crown*.88f);

            for (int i = -4; i <= 4; i += 2)
                Part(parent,"Outer Wall Merlon",PrimitiveType.Cube,new Vector3(wallX-.52f,3.07f,z+i*.52f),new Vector3(.30f,.50f,.34f),crown);

            for (int i = -1; i <= 1; i++)
            {
                float buttressZ = z + i * 1.72f;
                Part(parent,"Outer Wall Buttress",PrimitiveType.Cube,new Vector3(wallX-.58f,1.02f,buttressZ),new Vector3(.38f,1.75f,.58f),warmStone*.82f);
                Part(parent,"Outer Buttress Cap",PrimitiveType.Cube,new Vector3(wallX-.60f,1.94f,buttressZ),new Vector3(.48f,.15f,.68f),crown*.86f);
            }

            Part(parent,"Outer Wall Slit",PrimitiveType.Cube,new Vector3(wallX-.48f,1.62f,z-.92f),new Vector3(.035f,.32f,.10f),shadow);
            Part(parent,"Outer Wall Slit",PrimitiveType.Cube,new Vector3(wallX-.48f,1.62f,z+.92f),new Vector3(.035f,.32f,.10f),shadow);
        }

        for (int side = -1; side <= 1; side += 2)
        {
            float z = side * 9.15f;
            Part(parent,"Outer Wall Tower Base",PrimitiveType.Cylinder,new Vector3(wallX+.05f,.32f,z),new Vector3(1.20f,.22f,1.20f),darkStone);
            Part(parent,"Outer Wall Tower",PrimitiveType.Cylinder,new Vector3(wallX,1.62f,z),new Vector3(1.08f,1.48f,1.08f),warmStone);
            Part(parent,"Outer Tower Crown",PrimitiveType.Cylinder,new Vector3(wallX,3.17f,z),new Vector3(1.22f,.14f,1.22f),crown);

            for (int i = 0; i < 6; i++)
            {
                float a = i * Mathf.PI * 2f / 6f;
                Vector3 p = new Vector3(wallX,3.46f,z) + new Vector3(Mathf.Cos(a)*.90f,0f,Mathf.Sin(a)*.90f);
                Part(parent,"Outer Tower Merlon",PrimitiveType.Cube,p,new Vector3(.30f,.46f,.30f),crown);
            }

            Part(parent,"Outer Tower Slit",PrimitiveType.Cube,new Vector3(wallX-1.08f,1.72f,z-.28f),new Vector3(.035f,.36f,.10f),shadow);
            Part(parent,"Outer Tower Slit",PrimitiveType.Cube,new Vector3(wallX-1.08f,1.72f,z+.28f),new Vector3(.035f,.36f,.10f),shadow);
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
        Part(parent,"Royal Standard Pole",PrimitiveType.Cylinder,position,new Vector3(.04f,.95f,.04f),new Color(.30f,.17f,.07f));
        GameObject banner = Part(parent,"Royal Banner",PrimitiveType.Cube,position+new Vector3(-.04f,.50f,0f),new Vector3(.06f,.54f,.54f),new Color(.55f,.06f,.04f));
        Part(parent,"Royal Gold Stripe",PrimitiveType.Cube,position+new Vector3(-.075f,.50f,0f),new Vector3(.025f,.08f,.56f),new Color(.88f,.62f,.14f));
        Part(parent,"Royal Banner Tail",PrimitiveType.Cube,position+new Vector3(-.045f,.17f,.18f),new Vector3(.055f,.18f,.18f),new Color(.45f,.045f,.035f));
        banner.AddComponent<ChapterOneAmbientMotion>().kind = ChapterOneAmbientMotion.MotionKind.Banner;
    }

    static void AddBeacon(Transform parent, Vector3 position)
    {
        Part(parent,"Beacon Stone",PrimitiveType.Cylinder,position,new Vector3(.46f,.20f,.46f),new Color(.48f,.38f,.24f));
        Part(parent,"Beacon Bronze Bowl",PrimitiveType.Cylinder,position+new Vector3(0f,.19f,0f),new Vector3(.34f,.11f,.34f),new Color(.65f,.40f,.12f));
        GameObject flame = Part(parent,"Beacon Flame",PrimitiveType.Sphere,position+new Vector3(0f,.52f,0f),new Vector3(.23f,.42f,.23f),new Color(1f,.34f,.05f));
        flame.AddComponent<ChapterOneAmbientMotion>().kind = ChapterOneAmbientMotion.MotionKind.Flame;
        Light light = flame.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f,.46f,.12f);
        light.range = 5.5f;
        light.intensity = 1.9f;
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
