using UnityEngine;

// Static coast geometry only. Dynamic shore motion belongs to ChapterOneShoreLife;
// Troy wall/gate/city/fire have dedicated presentation owners.
public static class CoastEnvironmentBuilder
{
    static readonly Color Sand = new Color(.63f,.54f,.38f);
    static readonly Color WetSand = new Color(.47f,.43f,.34f);
    static readonly Color Water = new Color(.08f,.24f,.32f);
    static readonly Color DeepWater = new Color(.04f,.12f,.18f);
    static readonly Color Rock = new Color(.30f,.29f,.26f);
    static readonly Color Wood = new Color(.30f,.19f,.10f);

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
        Primitive(parent,"Sandy Coast",PrimitiveType.Cube,new Vector3(2.5f,-.20f,0f),new Vector3(30f,.18f,22f),Sand);
        Primitive(parent,"Wet Shore",PrimitiveType.Cube,new Vector3(-12.9f,-.11f,0f),new Vector3(2.3f,.08f,22f),WetSand);
        Primitive(parent,"Dry Shore Highlight",PrimitiveType.Cube,new Vector3(-10.8f,-.08f,0f),new Vector3(1.2f,.025f,22f),new Color(.70f,.61f,.43f));
    }

    static void CreateSea(Transform parent)
    {
        Primitive(parent,"Aegean Sea",PrimitiveType.Cube,new Vector3(-21f,-.28f,0f),new Vector3(14f,.12f,25f),Water);
        Primitive(parent,"Deep Sea",PrimitiveType.Cube,new Vector3(-29f,-.34f,0f),new Vector3(8f,.10f,25f),DeepWater);
    }

    static void CreateGreekLanding(Transform parent)
    {
        GreekLandingShipVisualFactory.Create(parent,new Vector3(-19.0f,.10f,5.8f),"Greek Landing Ship",-8f);
        GreekLandingShipVisualFactory.Create(parent,new Vector3(-20.4f,.10f,.5f),"Greek Landing Ship",5f);
        GreekLandingShipVisualFactory.Create(parent,new Vector3(-18.5f,.10f,-5.8f),"Greek Landing Ship",-4f);

        CreateTent(parent,new Vector3(-11.7f,.30f,6.4f));
        CreateTent(parent,new Vector3(-10.9f,.30f,-6.0f));
        CreateTent(parent,new Vector3(-9.6f,.30f,8.0f));

        for (int i = 0; i < 8; i++)
        {
            float z = -7.5f + i * 2.1f;
            Primitive(parent,"Landing Spear",PrimitiveType.Cylinder,new Vector3(-11.2f+(i%2)*.5f,.65f,z),new Vector3(.035f,.65f,.035f),Wood);
        }
    }

    static void CreateTent(Transform parent, Vector3 position)
    {
        Primitive(parent,"Greek Camp Tent",PrimitiveType.Cylinder,position,new Vector3(.9f,.45f,.9f),new Color(.50f,.34f,.22f));
        Primitive(parent,"Tent Standard",PrimitiveType.Cylinder,position+new Vector3(0f,.70f,0f),new Vector3(.025f,.42f,.025f),Wood);
    }

    static void CreateDunesAndRocks(Transform parent)
    {
        Vector3[] rocks =
        {
            new Vector3(-8f,.2f,9f), new Vector3(-6.5f,.15f,-8.2f), new Vector3(1f,.15f,9.2f),
            new Vector3(4f,.18f,-8.8f), new Vector3(9f,.2f,8.6f), new Vector3(11f,.15f,-7.8f)
        };
        foreach (Vector3 p in rocks)
        {
            GameObject rock = Primitive(parent,"Coast Rock",PrimitiveType.Sphere,p,new Vector3(1.1f,.45f,.8f),Rock);
            rock.transform.rotation = Quaternion.Euler(0f,p.z*9f,0f);
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
        Object.Destroy(go.GetComponent<Collider>());
        TowerFactory.SetColor(go,color);
        return go;
    }
}
