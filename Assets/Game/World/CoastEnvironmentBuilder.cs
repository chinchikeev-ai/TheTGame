using UnityEngine;

public static class CoastEnvironmentBuilder
{
    static readonly Color Sand = new Color(.63f,.54f,.38f);
    static readonly Color WetSand = new Color(.47f,.43f,.34f);
    static readonly Color Water = new Color(.08f,.24f,.32f);
    static readonly Color DeepWater = new Color(.04f,.12f,.18f);
    static readonly Color Rock = new Color(.30f,.29f,.26f);
    static readonly Color Wood = new Color(.30f,.19f,.10f);
    static readonly Color Cloth = new Color(.62f,.52f,.38f);

    public static void Build()
    {
        if (GameObject.Find("Chapter01_CoastEnvironment") != null) return;
        GameObject root = new GameObject("Chapter01_CoastEnvironment");
        CreateGround(root.transform);
        CreateSea(root.transform);
        CreateShoreline(root.transform);
        CreateGreekLanding(root.transform);
        CreateDunesAndRocks(root.transform);
        CreateTroyBackdrop(root.transform);
    }

    static void CreateGround(Transform parent)
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Sandy Coast";
        ground.transform.SetParent(parent);
        ground.transform.position = new Vector3(2.5f, -0.20f, 0f);
        ground.transform.localScale = new Vector3(30f, .18f, 22f);
        Object.Destroy(ground.GetComponent<Collider>());
        TowerFactory.SetColor(ground, Sand);

        GameObject wet = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wet.name = "Wet Shore";
        wet.transform.SetParent(parent);
        wet.transform.position = new Vector3(-12.9f, -0.11f, 0f);
        wet.transform.localScale = new Vector3(2.3f, .08f, 22f);
        Object.Destroy(wet.GetComponent<Collider>());
        TowerFactory.SetColor(wet, WetSand);
    }

    static void CreateSea(Transform parent)
    {
        GameObject sea = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sea.name = "Aegean Sea";
        sea.transform.SetParent(parent);
        sea.transform.position = new Vector3(-21f, -0.28f, 0f);
        sea.transform.localScale = new Vector3(14f, .12f, 25f);
        Object.Destroy(sea.GetComponent<Collider>());
        TowerFactory.SetColor(sea, Water);

        GameObject deep = GameObject.CreatePrimitive(PrimitiveType.Cube);
        deep.name = "Deep Sea";
        deep.transform.SetParent(parent);
        deep.transform.position = new Vector3(-29f, -0.34f, 0f);
        deep.transform.localScale = new Vector3(8f, .10f, 25f);
        Object.Destroy(deep.GetComponent<Collider>());
        TowerFactory.SetColor(deep, DeepWater);
    }

    static void CreateShoreline(Transform parent)
    {
        for (int i = -9; i <= 9; i += 2)
        {
            GameObject foam = GameObject.CreatePrimitive(PrimitiveType.Cube);
            foam.name = "Foam";
            foam.transform.SetParent(parent);
            foam.transform.position = new Vector3(-14.0f + (i % 4) * .08f, -0.02f, i * 1.05f);
            foam.transform.localScale = new Vector3(.14f, .025f, 1.55f);
            Object.Destroy(foam.GetComponent<Collider>());
            TowerFactory.SetColor(foam, new Color(.78f,.82f,.78f));
        }
    }

    static void CreateGreekLanding(Transform parent)
    {
        CreateShip(parent, new Vector3(-19.0f, .10f, 5.8f), -8f);
        CreateShip(parent, new Vector3(-20.4f, .10f, .5f), 5f);
        CreateShip(parent, new Vector3(-18.5f, .10f, -5.8f), -4f);
        CreateTent(parent, new Vector3(-11.7f, .30f, 6.4f));
        CreateTent(parent, new Vector3(-10.9f, .30f, -6.0f));
        CreateTent(parent, new Vector3(-9.6f, .30f, 8.0f));

        for (int i = 0; i < 8; i++)
        {
            float z = -7.5f + i * 2.1f;
            GameObject spear = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            spear.name = "Landing Spear";
            spear.transform.SetParent(parent);
            spear.transform.position = new Vector3(-11.2f + (i % 2) * .5f, .65f, z);
            spear.transform.localScale = new Vector3(.035f, .65f, .035f);
            Object.Destroy(spear.GetComponent<Collider>());
            TowerFactory.SetColor(spear, Wood);
        }
    }

    static void CreateShip(Transform parent, Vector3 position, float yaw)
    {
        GameObject root = new GameObject("Greek Landing Ship");
        root.transform.SetParent(parent);
        root.transform.position = position;
        root.transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        GameObject hull = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hull.transform.SetParent(root.transform, false);
        hull.transform.localScale = new Vector3(3.5f, .35f, 1.0f);
        hull.transform.localPosition = new Vector3(0f, .15f, 0f);
        Object.Destroy(hull.GetComponent<Collider>());
        TowerFactory.SetColor(hull, Wood);

        GameObject mast = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        mast.transform.SetParent(root.transform, false);
        mast.transform.localScale = new Vector3(.06f, 1.5f, .06f);
        mast.transform.localPosition = new Vector3(0f, 1.6f, 0f);
        Object.Destroy(mast.GetComponent<Collider>());
        TowerFactory.SetColor(mast, Wood);

        GameObject sail = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sail.transform.SetParent(root.transform, false);
        sail.transform.localScale = new Vector3(.10f, 1.4f, 1.45f);
        sail.transform.localPosition = new Vector3(0f, 1.75f, 0f);
        Object.Destroy(sail.GetComponent<Collider>());
        TowerFactory.SetColor(sail, Cloth);
    }

    static void CreateTent(Transform parent, Vector3 position)
    {
        GameObject tent = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        tent.name = "Greek Camp Tent";
        tent.transform.SetParent(parent);
        tent.transform.position = position;
        tent.transform.localScale = new Vector3(.9f, .45f, .9f);
        Object.Destroy(tent.GetComponent<Collider>());
        TowerFactory.SetColor(tent, new Color(.50f,.34f,.22f));
    }

    static void CreateDunesAndRocks(Transform parent)
    {
        Vector3[] rocks = {
            new Vector3(-8f,.2f,9f), new Vector3(-6.5f,.15f,-8.2f), new Vector3(1f,.15f,9.2f),
            new Vector3(4f,.18f,-8.8f), new Vector3(9f,.2f,8.6f), new Vector3(11f,.15f,-7.8f)
        };
        foreach (Vector3 p in rocks)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.name = "Coast Rock";
            rock.transform.SetParent(parent);
            rock.transform.position = p;
            rock.transform.localScale = new Vector3(1.1f, .45f, .8f);
            Object.Destroy(rock.GetComponent<Collider>());
            TowerFactory.SetColor(rock, Rock);
        }
    }

    static void CreateTroyBackdrop(Transform parent)
    {
        float gateX = MapBuilder.CellToWorld(new Vector2Int(17, 6)).x;
        float wallX = gateX + 1.35f;

        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Troy Outer Wall";
        wall.transform.SetParent(parent);
        wall.transform.position = new Vector3(wallX, 1.0f, 0f);
        wall.transform.localScale = new Vector3(.9f, 2.2f, 18f);
        Object.Destroy(wall.GetComponent<Collider>());
        TowerFactory.SetColor(wall, new Color(.58f,.47f,.29f));

        for (int i = -4; i <= 4; i += 2)
        {
            GameObject tower = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tower.name = "Troy Wall Tower";
            tower.transform.SetParent(parent);
            tower.transform.position = new Vector3(wallX - .05f, 1.25f, i * 2.0f);
            tower.transform.localScale = new Vector3(.95f, 1.25f, .95f);
            Object.Destroy(tower.GetComponent<Collider>());
            TowerFactory.SetColor(tower, new Color(.64f,.52f,.31f));
        }

        CreateTrojanStandard(parent, new Vector3(gateX + .70f, 2.65f, 2.2f));
        CreateTrojanStandard(parent, new Vector3(gateX + .70f, 2.65f, -2.2f));
    }

    static void CreateTrojanStandard(Transform parent, Vector3 position)
    {
        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.name = "Trojan Standard";
        pole.transform.SetParent(parent);
        pole.transform.position = position;
        pole.transform.localScale = new Vector3(.045f, .9f, .045f);
        Object.Destroy(pole.GetComponent<Collider>());
        TowerFactory.SetColor(pole, Wood);

        GameObject banner = GameObject.CreatePrimitive(PrimitiveType.Cube);
        banner.name = "Trojan Banner";
        banner.transform.SetParent(parent);
        banner.transform.position = position + new Vector3(0f, .45f, 0f);
        banner.transform.localScale = new Vector3(.08f, .55f, .42f);
        Object.Destroy(banner.GetComponent<Collider>());
        TowerFactory.SetColor(banner, new Color(.52f, .08f, .06f));
    }
}
