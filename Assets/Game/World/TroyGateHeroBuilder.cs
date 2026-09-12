using UnityEngine;

public static class TroyGateHeroBuilder
{
    public static void Build()
    {
        if (GameObject.Find("Chapter01_TroyGateHero") != null) return;
        GameObject root = new GameObject("Chapter01_TroyGateHero");
        float gateX = MapBuilder.CellToWorld(new Vector2Int(17, 6)).x;
        Color sandstone = new Color(.63f, .51f, .31f);
        Color darkStone = new Color(.42f, .34f, .22f);
        Color bronze = new Color(.72f, .48f, .16f);
        Color wood = new Color(.28f, .15f, .07f);
        Color trojanRed = new Color(.52f, .06f, .04f);

        Part(root.transform, "Gatehouse Core", PrimitiveType.Cube, new Vector3(gateX + .65f, 1.45f, 0f), new Vector3(1.65f, 2.8f, 5.3f), sandstone);
        Part(root.transform, "Gate Recess", PrimitiveType.Cube, new Vector3(gateX - .18f, .88f, 0f), new Vector3(.30f, 1.75f, 1.85f), darkStone);
        Part(root.transform, "Gate Doors", PrimitiveType.Cube, new Vector3(gateX - .38f, .83f, 0f), new Vector3(.20f, 1.65f, 1.65f), wood);

        for (int z = -1; z <= 1; z += 2)
            Part(root.transform, "Door Bronze Band", PrimitiveType.Cube, new Vector3(gateX - .50f, .83f, z * .44f), new Vector3(.05f, 1.55f, .07f), bronze);
        for (int y = 0; y < 4; y++)
            Part(root.transform, "Gate Crossbar", PrimitiveType.Cube, new Vector3(gateX - .51f, .30f + y * .35f, 0f), new Vector3(.05f, .07f, 1.62f), bronze * .92f);

        for (int side = -1; side <= 1; side += 2)
        {
            float z = side * 3.55f;
            Part(root.transform, "Hero Gate Tower", PrimitiveType.Cylinder, new Vector3(gateX + .40f, 1.65f, z), new Vector3(1.35f, 1.65f, 1.35f), sandstone * .97f);
            Part(root.transform, "Tower Crown", PrimitiveType.Cylinder, new Vector3(gateX + .40f, 3.35f, z), new Vector3(1.52f, .16f, 1.52f), sandstone * 1.08f);
            for (int i = 0; i < 8; i++)
            {
                float a = i * Mathf.PI * 2f / 8f;
                Vector3 p = new Vector3(gateX + .40f, 3.62f, z) + new Vector3(Mathf.Cos(a) * 1.18f, 0f, Mathf.Sin(a) * 1.18f);
                Part(root.transform, "Tower Crenellation", PrimitiveType.Cube, p, new Vector3(.34f, .48f, .34f), sandstone * 1.06f);
            }
            AddBanner(root.transform, new Vector3(gateX - .95f, 2.15f, z), trojanRed, bronze);
        }

        for (int i = -4; i <= 4; i++)
        {
            if (i == 0) continue;
            Part(root.transform, "Gatehouse Crenellation", PrimitiveType.Cube, new Vector3(gateX - .08f, 3.08f, i * .55f), new Vector3(.34f, .54f, .36f), sandstone * 1.05f);
        }

        GameObject emblem = Part(root.transform, "Lion Emblem Back", PrimitiveType.Cylinder, new Vector3(gateX - .30f, 2.06f, 0f), new Vector3(.52f, .08f, .52f), bronze);
        emblem.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        Part(root.transform, "Lion Emblem Mark", PrimitiveType.Cube, new Vector3(gateX - .40f, 2.07f, 0f), new Vector3(.08f, .48f, .14f), trojanRed);

        for (int side = -1; side <= 1; side += 2)
        {
            Vector3 p = new Vector3(gateX - .75f, 1.15f, side * 2.25f);
            Part(root.transform, "Gate Brazier", PrimitiveType.Cylinder, p, new Vector3(.34f, .14f, .34f), bronze * .72f);
            GameObject flame = Part(root.transform, "Gate Flame", PrimitiveType.Sphere, p + new Vector3(0f, .34f, 0f), new Vector3(.22f, .42f, .22f), new Color(1f, .32f, .04f));
            flame.AddComponent<ChapterOneAmbientMotion>().kind = ChapterOneAmbientMotion.MotionKind.Flame;
            Light light = flame.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, .42f, .10f);
            light.range = 5.5f;
            light.intensity = 2f;
        }
    }

    static void AddBanner(Transform parent, Vector3 position, Color cloth, Color trim)
    {
        Part(parent, "Gate Banner Pole", PrimitiveType.Cylinder, position, new Vector3(.045f, .85f, .045f), new Color(.28f, .16f, .07f));
        GameObject banner = Part(parent, "Gate Banner", PrimitiveType.Cube, position + new Vector3(-.08f, .30f, 0f), new Vector3(.08f, .52f, .52f), cloth);
        Part(parent, "Banner Trim", PrimitiveType.Cube, position + new Vector3(-.125f, .30f, 0f), new Vector3(.02f, .07f, .52f), trim);
        banner.AddComponent<ChapterOneAmbientMotion>().kind = ChapterOneAmbientMotion.MotionKind.Banner;
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
