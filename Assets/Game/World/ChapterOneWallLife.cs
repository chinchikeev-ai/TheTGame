using UnityEngine;

public static class ChapterOneWallLife
{
    public static void Build()
    {
        if (GameObject.Find("Chapter01_WallLife") != null) return;
        GameObject root = new GameObject("Chapter01_WallLife");
        float gateX = MapBuilder.CellToWorld(new Vector2Int(17, 6)).x;

        AddGuard(root.transform, new Vector3(gateX + .55f, 2.45f, -5.8f), 180f);
        AddGuard(root.transform, new Vector3(gateX + .55f, 2.45f, -2.9f), 180f);
        AddGuard(root.transform, new Vector3(gateX + .55f, 2.45f, 2.9f), 180f);
        AddGuard(root.transform, new Vector3(gateX + .55f, 2.45f, 5.8f), 180f);

        AddStandard(root.transform, new Vector3(gateX + .65f, 3.0f, -6.9f));
        AddStandard(root.transform, new Vector3(gateX + .65f, 3.0f, 6.9f));
        AddBeacon(root.transform, new Vector3(gateX + 2.7f, 3.25f, -3.4f));
        AddBeacon(root.transform, new Vector3(gateX + 2.7f, 3.25f, 3.4f));
    }

    static void AddGuard(Transform parent, Vector3 position, float yaw)
    {
        GameObject guard = RuntimeWarriorVisualFactory.CreateEnemyFallback(EnemyArchetype.ShieldBearer, new Color(.64f, .22f, .12f));
        guard.name = "Trojan Wall Guard";
        guard.transform.SetParent(parent, false);
        guard.transform.position = position;
        guard.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        guard.transform.localScale *= .82f;
        foreach (Collider collider in guard.GetComponentsInChildren<Collider>(true)) Object.Destroy(collider);
        EnemyMotionAnimator.Attach(guard, EnemyArchetype.ShieldBearer);
    }

    static void AddStandard(Transform parent, Vector3 position)
    {
        Part(parent, "Royal Standard Pole", PrimitiveType.Cylinder, position, new Vector3(.04f, .9f, .04f), new Color(.30f, .17f, .07f));
        GameObject banner = Part(parent, "Royal Banner", PrimitiveType.Cube, position + new Vector3(-.02f, .48f, 0f), new Vector3(.06f, .48f, .48f), new Color(.55f, .06f, .04f));
        Part(parent, "Royal Gold Stripe", PrimitiveType.Cube, position + new Vector3(-.055f, .48f, 0f), new Vector3(.025f, .08f, .50f), new Color(.88f, .62f, .14f));
        banner.AddComponent<ChapterOneAmbientMotion>().kind = ChapterOneAmbientMotion.MotionKind.Banner;
    }

    static void AddBeacon(Transform parent, Vector3 position)
    {
        Part(parent, "Beacon Stone", PrimitiveType.Cylinder, position, new Vector3(.42f, .18f, .42f), new Color(.48f, .38f, .24f));
        GameObject flame = Part(parent, "Beacon Flame", PrimitiveType.Sphere, position + new Vector3(0f, .38f, 0f), new Vector3(.22f, .38f, .22f), new Color(1f, .34f, .05f));
        flame.AddComponent<ChapterOneAmbientMotion>().kind = ChapterOneAmbientMotion.MotionKind.Flame;
        Light light = flame.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, .46f, .12f);
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
