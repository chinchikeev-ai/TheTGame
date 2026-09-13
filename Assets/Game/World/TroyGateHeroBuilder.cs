using UnityEngine;

public static class TroyGateHeroBuilder
{
    public static void Build()
    {
        if (GameObject.Find("Chapter01_TroyGateHero") != null) return;
        GameObject root = new GameObject("Chapter01_TroyGateHero");
        float gateX = MapBuilder.CellToWorld(new Vector2Int(17, 6)).x;
        Color sandstone = new Color(.63f, .51f, .31f);
        Color paleStone = new Color(.72f, .61f, .40f);
        Color darkStone = new Color(.42f, .34f, .22f);
        Color shadowStone = new Color(.30f, .25f, .18f);
        Color bronze = new Color(.72f, .48f, .16f);
        Color wood = new Color(.28f, .15f, .07f);
        Color trojanRed = new Color(.52f, .06f, .04f);

        // Main Scaean-gate mass: layered instead of a single prototype block.
        Part(root.transform, "Gatehouse Core", PrimitiveType.Cube, new Vector3(gateX + .72f, 1.48f, 0f), new Vector3(1.72f, 2.95f, 5.45f), sandstone);
        Part(root.transform, "Gatehouse Upper Terrace", PrimitiveType.Cube, new Vector3(gateX + .56f, 2.82f, 0f), new Vector3(1.95f, .42f, 5.85f), paleStone);
        Part(root.transform, "Wall Walk", PrimitiveType.Cube, new Vector3(gateX + 1.05f, 3.06f, 0f), new Vector3(1.62f, .18f, 6.00f), darkStone);

        // Flanking wall wings make Troy read as a fortified city, not a freestanding gate prop.
        for (int side = -1; side <= 1; side += 2)
        {
            float wingZ = side * 6.55f;
            Part(root.transform, "Troy Wall Wing", PrimitiveType.Cube, new Vector3(gateX + .82f, 1.24f, wingZ), new Vector3(1.45f, 2.48f, 5.10f), sandstone * .95f);
            Part(root.transform, "Wall Parapet", PrimitiveType.Cube, new Vector3(gateX + .62f, 2.58f, wingZ), new Vector3(1.66f, .30f, 5.20f), paleStone);

            for (int i = -4; i <= 4; i++)
            {
                Vector3 merlon = new Vector3(gateX - .02f, 2.95f, wingZ + i * .54f);
                Part(root.transform, "Wall Crenellation", PrimitiveType.Cube, merlon, new Vector3(.36f, .54f, .34f), paleStone);
            }

            for (int i = -1; i <= 1; i++)
            {
                float buttressZ = wingZ + i * 1.75f;
                Part(root.transform, "Wall Buttress", PrimitiveType.Cube, new Vector3(gateX - .05f, .86f, buttressZ), new Vector3(.52f, 1.72f, .56f), darkStone * 1.04f);
                Part(root.transform, "Buttress Cap", PrimitiveType.Cube, new Vector3(gateX - .08f, 1.77f, buttressZ), new Vector3(.62f, .18f, .66f), paleStone);
            }
        }

        // Deep gate recess and massive timber doors.
        Part(root.transform, "Gate Recess", PrimitiveType.Cube, new Vector3(gateX - .20f, .92f, 0f), new Vector3(.36f, 1.84f, 1.96f), shadowStone);
        Part(root.transform, "Gate Left Jamb", PrimitiveType.Cube, new Vector3(gateX - .43f, .95f, -1.10f), new Vector3(.38f, 1.95f, .40f), paleStone);
        Part(root.transform, "Gate Right Jamb", PrimitiveType.Cube, new Vector3(gateX - .43f, .95f, 1.10f), new Vector3(.38f, 1.95f, .40f), paleStone);
        Part(root.transform, "Gate Lintel", PrimitiveType.Cube, new Vector3(gateX - .42f, 1.92f, 0f), new Vector3(.40f, .34f, 2.58f), paleStone);
        Part(root.transform, "Gate Doors", PrimitiveType.Cube, new Vector3(gateX - .47f, .83f, 0f), new Vector3(.20f, 1.65f, 1.68f), wood);

        for (int z = -1; z <= 1; z += 2)
            Part(root.transform, "Door Bronze Band", PrimitiveType.Cube, new Vector3(gateX - .59f, .83f, z * .44f), new Vector3(.05f, 1.55f, .07f), bronze);
        for (int y = 0; y < 4; y++)
            Part(root.transform, "Gate Crossbar", PrimitiveType.Cube, new Vector3(gateX - .60f, .30f + y * .35f, 0f), new Vector3(.05f, .07f, 1.65f), bronze * .92f);

        // Stone courses on the front face add scale at the gameplay camera distance.
        for (int row = 0; row < 5; row++)
        {
            float y = .38f + row * .51f;
            Part(root.transform, "Gate Masonry Course", PrimitiveType.Cube, new Vector3(gateX - .18f, y, -2.12f), new Vector3(.08f, .055f, 1.14f), darkStone * 1.02f);
            Part(root.transform, "Gate Masonry Course", PrimitiveType.Cube, new Vector3(gateX - .18f, y, 2.12f), new Vector3(.08f, .055f, 1.14f), darkStone * 1.02f);
        }

        // Twin hero towers.
        for (int side = -1; side <= 1; side += 2)
        {
            float z = side * 3.58f;
            Part(root.transform, "Hero Gate Tower", PrimitiveType.Cylinder, new Vector3(gateX + .42f, 1.68f, z), new Vector3(1.40f, 1.68f, 1.40f), sandstone * .97f);
            Part(root.transform, "Tower Lower Ring", PrimitiveType.Cylinder, new Vector3(gateX + .42f, .34f, z), new Vector3(1.48f, .16f, 1.48f), darkStone);
            Part(root.transform, "Tower Crown", PrimitiveType.Cylinder, new Vector3(gateX + .42f, 3.40f, z), new Vector3(1.57f, .18f, 1.57f), paleStone);

            for (int i = 0; i < 8; i++)
            {
                float a = i * Mathf.PI * 2f / 8f;
                Vector3 p = new Vector3(gateX + .42f, 3.69f, z) + new Vector3(Mathf.Cos(a) * 1.20f, 0f, Mathf.Sin(a) * 1.20f);
                Part(root.transform, "Tower Crenellation", PrimitiveType.Cube, p, new Vector3(.34f, .50f, .34f), paleStone);
            }

            // Readable dark arrow slits on the attacking face.
            Part(root.transform, "Tower Arrow Slit", PrimitiveType.Cube, new Vector3(gateX - .98f, 1.72f, z - .34f), new Vector3(.035f, .34f, .10f), shadowStone * .55f);
            Part(root.transform, "Tower Arrow Slit", PrimitiveType.Cube, new Vector3(gateX - .98f, 1.72f, z + .34f), new Vector3(.035f, .34f, .10f), shadowStone * .55f);
            AddBanner(root.transform, new Vector3(gateX - 1.00f, 2.18f, z), trojanRed, bronze);
        }

        for (int i = -4; i <= 4; i++)
        {
            if (i == 0) continue;
            Part(root.transform, "Gatehouse Crenellation", PrimitiveType.Cube, new Vector3(gateX - .10f, 3.19f, i * .58f), new Vector3(.36f, .56f, .36f), paleStone);
        }

        // Heroic bronze relief above the doors.
        GameObject emblem = Part(root.transform, "Lion Emblem Back", PrimitiveType.Cylinder, new Vector3(gateX - .40f, 2.28f, 0f), new Vector3(.58f, .08f, .58f), bronze);
        emblem.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        Part(root.transform, "Lion Emblem Body", PrimitiveType.Cube, new Vector3(gateX - .50f, 2.27f, 0f), new Vector3(.08f, .30f, .44f), trojanRed);
        Part(root.transform, "Lion Emblem Head", PrimitiveType.Sphere, new Vector3(gateX - .57f, 2.46f, 0f), new Vector3(.12f, .16f, .16f), trojanRed);

        // Gate braziers remain focal points on the Troy side.
        for (int side = -1; side <= 1; side += 2)
        {
            Vector3 p = new Vector3(gateX - .82f, 1.15f, side * 2.30f);
            Part(root.transform, "Gate Brazier Pedestal", PrimitiveType.Cube, p - new Vector3(0f,.42f,0f), new Vector3(.42f,.72f,.42f), darkStone);
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
