using UnityEngine;

// Static coast geometry only. Dynamic shore motion belongs to ChapterOneShoreLife;
// Troy wall/gate/city/fire have dedicated presentation owners.
public static class CoastEnvironmentBuilder
{
    static readonly Color Sand = new Color(.63f,.54f,.38f);
    static readonly Color LightSand = new Color(.72f,.62f,.43f);
    static readonly Color WetSand = new Color(.43f,.39f,.31f);
    static readonly Color ShallowWater = new Color(.10f,.31f,.37f);
    static readonly Color Water = new Color(.065f,.21f,.29f);
    static readonly Color DeepWater = new Color(.035f,.10f,.17f);
    static readonly Color Rock = new Color(.30f,.29f,.26f);
    static readonly Color Wood = new Color(.29f,.17f,.08f);
    static readonly Color GreekCloth = new Color(.43f,.46f,.52f);

    public static void Build()
    {
        if (GameObject.Find("Chapter01_CoastEnvironment") != null) return;
        GameObject root = new GameObject("Chapter01_CoastEnvironment");
        CreateGround(root.transform);
        CreateSea(root.transform);
        CreateGreekLanding(root.transform);
        CreateDunesAndRocks(root.transform);
    }

    static void CreateGround(Transform parent)
    {
        Primitive(parent,"Sandy Coast Base",PrimitiveType.Cube,new Vector3(2f,-.20f,0f),new Vector3(30f,.18f,22f),Sand);
        Primitive(parent,"Wet Shore Base",PrimitiveType.Cube,new Vector3(-12.65f,-.11f,0f),new Vector3(2.05f,.075f,22f),WetSand);

        float[] wetZ = { -9.1f,-6.7f,-4.2f,-1.8f,.7f,3.2f,5.7f,8.4f };
        for (int i = 0; i < wetZ.Length; i++)
        {
            float x = -12.15f + (i % 3 - 1) * .18f;
            float length = 1.65f + (i % 4) * .18f;
            Primitive(parent,"Irregular Wet Sand",PrimitiveType.Sphere,
                new Vector3(x,-.055f,wetZ[i]),
                new Vector3(1.55f,.055f,length),
                i % 2 == 0 ? WetSand : WetSand * 1.06f,
                Quaternion.Euler(0f,(i % 2 == 0 ? 8f : -11f),0f));
        }

        Vector3[] dryTongues =
        {
            new Vector3(-10.75f,-.045f,-7.7f), new Vector3(-10.45f,-.045f,-3.4f),
            new Vector3(-10.85f,-.045f,1.1f), new Vector3(-10.35f,-.045f,5.2f),
            new Vector3(-10.8f,-.045f,8.4f)
        };
        for (int i = 0; i < dryTongues.Length; i++)
        {
            Primitive(parent,"Dry Sand Tongue",PrimitiveType.Sphere,dryTongues[i],
                new Vector3(1.75f,.035f,1.55f + (i % 2) * .45f),
                LightSand * (.96f + i * .008f),
                Quaternion.Euler(0f,-14f + i * 7f,0f));
        }
    }

    static void CreateSea(Transform parent)
    {
        Primitive(parent,"Aegean Shallows",PrimitiveType.Cube,new Vector3(-15.6f,-.24f,0f),new Vector3(4.8f,.10f,25f),ShallowWater);
        Primitive(parent,"Aegean Mid Water",PrimitiveType.Cube,new Vector3(-21.9f,-.29f,0f),new Vector3(7.9f,.11f,25f),Water);
        Primitive(parent,"Deep Aegean Sea",PrimitiveType.Cube,new Vector3(-29.8f,-.34f,0f),new Vector3(8f,.10f,25f),DeepWater);

        float[] sandbarZ = { -7.8f,-3.7f,.2f,4.3f,8.2f };
        for (int i = 0; i < sandbarZ.Length; i++)
        {
            Primitive(parent,"Shallow Water Variation",PrimitiveType.Sphere,
                new Vector3(-14.55f,-.175f,sandbarZ[i]),
                new Vector3(1.75f,.025f,1.4f + (i % 2) * .45f),
                new Color(.12f,.35f,.39f),
                Quaternion.Euler(0f,-8f + i * 4f,0f));
        }
    }

    static void CreateGreekLanding(Transform parent)
    {
        GreekLandingShipVisualFactory.Create(parent,new Vector3(-19.0f,.10f,5.8f),"Greek Landing Ship",-8f);
        GreekLandingShipVisualFactory.Create(parent,new Vector3(-20.4f,.10f,.5f),"Greek Landing Ship",5f);
        GreekLandingShipVisualFactory.Create(parent,new Vector3(-18.5f,.10f,-5.8f),"Greek Landing Ship",-4f);

        CreateTent(parent,new Vector3(-10.8f,.02f,6.6f),-7f);
        CreateTent(parent,new Vector3(-10.1f,.02f,-6.2f),9f);
        CreateTent(parent,new Vector3(-8.9f,.02f,8.1f),4f);
        CreateSupplyCluster(parent,new Vector3(-10.2f,.02f,2.2f),12f);
        CreateSupplyCluster(parent,new Vector3(-9.6f,.02f,-2.0f),-9f);

        for (int i = 0; i < 8; i++)
        {
            float z = -7.5f + i * 2.1f;
            GameObject spear = Primitive(parent,"Landing Spear",PrimitiveType.Cylinder,
                new Vector3(-11.0f+(i%2)*.42f,.61f,z),
                new Vector3(.028f,.61f,.028f),Wood,
                Quaternion.Euler(i % 2 == 0 ? 5f : -7f,0f,i % 2 == 0 ? 5f : -6f));
            Primitive(spear.transform,"Bronze Spear Tip",PrimitiveType.Sphere,new Vector3(0f,1.0f,0f),new Vector3(2.1f,.16f,2.1f),new Color(.58f,.38f,.15f));
        }
    }

    static void CreateTent(Transform parent, Vector3 position, float yaw)
    {
        GameObject root = new GameObject("Greek A-Frame Tent");
        root.transform.SetParent(parent,false);
        root.transform.localPosition = position;
        root.transform.localRotation = Quaternion.Euler(0f,yaw,0f);

        Primitive(root.transform,"Tent Ground Cloth",PrimitiveType.Cube,new Vector3(0f,.045f,0f),new Vector3(1.45f,.05f,1.25f),GreekCloth * .74f);
        Primitive(root.transform,"Tent Left Roof",PrimitiveType.Cube,new Vector3(-.31f,.42f,0f),new Vector3(.78f,.065f,1.22f),GreekCloth,Quaternion.Euler(0f,0f,-31f));
        Primitive(root.transform,"Tent Right Roof",PrimitiveType.Cube,new Vector3(.31f,.42f,0f),new Vector3(.78f,.065f,1.22f),GreekCloth * .92f,Quaternion.Euler(0f,0f,31f));
        Primitive(root.transform,"Tent Ridge Pole",PrimitiveType.Cylinder,new Vector3(0f,.70f,0f),new Vector3(.025f,.67f,.025f),Wood,Quaternion.Euler(90f,0f,0f));
        Primitive(root.transform,"Tent Front Pole",PrimitiveType.Cylinder,new Vector3(0f,.35f,-.62f),new Vector3(.022f,.35f,.022f),Wood);
        Primitive(root.transform,"Tent Rear Pole",PrimitiveType.Cylinder,new Vector3(0f,.35f,.62f),new Vector3(.022f,.35f,.022f),Wood);
    }

    static void CreateSupplyCluster(Transform parent, Vector3 position, float yaw)
    {
        GameObject root = new GameObject("Greek Landing Supplies");
        root.transform.SetParent(parent,false);
        root.transform.localPosition = position;
        root.transform.localRotation = Quaternion.Euler(0f,yaw,0f);

        Primitive(root.transform,"Supply Crate",PrimitiveType.Cube,new Vector3(-.32f,.18f,0f),new Vector3(.48f,.36f,.46f),Wood);
        Primitive(root.transform,"Supply Crate",PrimitiveType.Cube,new Vector3(.18f,.14f,.28f),new Vector3(.38f,.28f,.38f),Wood * 1.14f,Quaternion.Euler(0f,17f,0f));
        Primitive(root.transform,"Amphora Body",PrimitiveType.Sphere,new Vector3(.38f,.22f,-.26f),new Vector3(.20f,.28f,.20f),new Color(.48f,.28f,.15f));
        Primitive(root.transform,"Amphora Neck",PrimitiveType.Cylinder,new Vector3(.38f,.43f,-.26f),new Vector3(.075f,.12f,.075f),new Color(.52f,.31f,.17f));
    }

    static void CreateDunesAndRocks(Transform parent)
    {
        Vector3[] dunes =
        {
            new Vector3(-7.8f,-.04f,9.0f), new Vector3(-5.2f,-.04f,-8.8f),
            new Vector3(.8f,-.04f,9.4f), new Vector3(4.6f,-.04f,-9.1f),
            new Vector3(8.7f,-.04f,8.8f), new Vector3(11.3f,-.04f,-8.2f)
        };
        for (int i = 0; i < dunes.Length; i++)
        {
            Primitive(parent,"Low Sand Dune",PrimitiveType.Sphere,dunes[i],
                new Vector3(2.1f + (i % 2) * .55f,.16f,1.0f + (i % 3) * .18f),
                LightSand * (.91f + i * .012f),
                Quaternion.Euler(0f,11f + i * 23f,0f));
        }

        Vector3[] rocks =
        {
            new Vector3(-8.1f,.18f,8.8f), new Vector3(-6.6f,.14f,-8.0f), new Vector3(1.1f,.15f,9.0f),
            new Vector3(4.1f,.17f,-8.6f), new Vector3(9.1f,.19f,8.4f), new Vector3(11.2f,.14f,-7.6f)
        };
        for (int i = 0; i < rocks.Length; i++)
        {
            Vector3 scale = new Vector3(.72f + (i % 3) * .18f,.28f + (i % 2) * .12f,.56f + ((i + 1) % 3) * .14f);
            Primitive(parent,"Coast Rock",PrimitiveType.Sphere,rocks[i],scale,Rock * (.92f + i * .018f),Quaternion.Euler(0f,19f + i * 37f,0f));
        }
    }

    static GameObject Primitive(Transform parent, string name, PrimitiveType type, Vector3 position, Vector3 scale, Color color, Quaternion? rotation = null)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent,false);
        go.transform.localPosition = position;
        go.transform.localScale = scale;
        if (rotation.HasValue) go.transform.localRotation = rotation.Value;
        Collider collider = go.GetComponent<Collider>();
        if (collider != null) Object.Destroy(collider);
        TowerFactory.SetColor(go,color);
        return go;
    }
}
