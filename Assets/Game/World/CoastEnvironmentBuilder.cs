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
        CreateFireIdentity(root.transform);
    }

    static void CreateGround(Transform parent)
    {
        GameObject ground=Primitive(parent,"Sandy Coast",PrimitiveType.Cube,new Vector3(2.5f,-.20f,0f),new Vector3(30f,.18f,22f),Sand);
        GameObject wet=Primitive(parent,"Wet Shore",PrimitiveType.Cube,new Vector3(-12.9f,-.11f,0f),new Vector3(2.3f,.08f,22f),WetSand);
        Primitive(parent,"Dry Shore Highlight",PrimitiveType.Cube,new Vector3(-10.8f,-.08f,0f),new Vector3(1.2f,.025f,22f),new Color(.70f,.61f,.43f));
    }

    static void CreateSea(Transform parent)
    {
        Primitive(parent,"Aegean Sea",PrimitiveType.Cube,new Vector3(-21f,-.28f,0f),new Vector3(14f,.12f,25f),Water);
        Primitive(parent,"Deep Sea",PrimitiveType.Cube,new Vector3(-29f,-.34f,0f),new Vector3(8f,.10f,25f),DeepWater);
        for(int z=-10;z<=10;z+=4)
            Primitive(parent,"Sea Glint",PrimitiveType.Cube,new Vector3(-18.0f,-.19f,z),new Vector3(4.6f,.012f,.11f),new Color(.20f,.46f,.52f));
    }

    static void CreateShoreline(Transform parent)
    {
        for(int i=-10;i<=10;i+=2)
        {
            float z=i*1.02f;
            Primitive(parent,"Foam",PrimitiveType.Cube,new Vector3(-14.0f+(i%4)*.06f,-.02f,z),new Vector3(.16f,.025f,1.35f),new Color(.82f,.85f,.80f));
            Primitive(parent,"Foam Secondary",PrimitiveType.Cube,new Vector3(-13.65f-((i+2)%4)*.04f,-.025f,z+.42f),new Vector3(.10f,.018f,.90f),new Color(.68f,.76f,.73f));
        }
    }

    static void CreateGreekLanding(Transform parent)
    {
        CreateShip(parent,new Vector3(-19.0f,.10f,5.8f),-8f);
        CreateShip(parent,new Vector3(-20.4f,.10f,.5f),5f);
        CreateShip(parent,new Vector3(-18.5f,.10f,-5.8f),-4f);
        CreateTent(parent,new Vector3(-11.7f,.30f,6.4f));
        CreateTent(parent,new Vector3(-10.9f,.30f,-6.0f));
        CreateTent(parent,new Vector3(-9.6f,.30f,8.0f));

        for(int i=0;i<8;i++)
        {
            float z=-7.5f+i*2.1f;
            Primitive(parent,"Landing Spear",PrimitiveType.Cylinder,new Vector3(-11.2f+(i%2)*.5f,.65f,z),new Vector3(.035f,.65f,.035f),Wood);
        }
    }

    static void CreateShip(Transform parent,Vector3 position,float yaw)
    {
        GameObject root=new GameObject("Greek Landing Ship");
        root.transform.SetParent(parent);
        root.transform.position=position;
        root.transform.rotation=Quaternion.Euler(0f,yaw,0f);

        Primitive(root.transform,"Hull",PrimitiveType.Cube,new Vector3(0f,.15f,0f),new Vector3(3.5f,.35f,1.0f),Wood);
        Primitive(root.transform,"Prow",PrimitiveType.Cube,new Vector3(1.95f,.42f,0f),new Vector3(.55f,.18f,.72f),new Color(.38f,.22f,.10f),Quaternion.Euler(0f,0f,-18f));
        Primitive(root.transform,"Mast",PrimitiveType.Cylinder,new Vector3(0f,1.6f,0f),new Vector3(.06f,1.5f,.06f),Wood);
        Primitive(root.transform,"Sail",PrimitiveType.Cube,new Vector3(0f,1.75f,0f),new Vector3(.10f,1.4f,1.45f),Cloth);
        Primitive(root.transform,"SailStripe",PrimitiveType.Cube,new Vector3(-.065f,1.75f,0f),new Vector3(.03f,.25f,1.48f),new Color(.56f,.12f,.09f));

        for(int side=-1;side<=1;side+=2)
            for(int i=-2;i<=2;i++)
                Primitive(root.transform,"Oar",PrimitiveType.Cylinder,new Vector3(i*.58f,.15f,side*.76f),new Vector3(.025f,.72f,.025f),new Color(.48f,.30f,.13f),Quaternion.Euler(72f,0f,0f));
    }

    static void CreateTent(Transform parent,Vector3 position)
    {
        Primitive(parent,"Greek Camp Tent",PrimitiveType.Cylinder,position,new Vector3(.9f,.45f,.9f),new Color(.50f,.34f,.22f));
        Primitive(parent,"Tent Standard",PrimitiveType.Cylinder,position+new Vector3(0f,.70f,0f),new Vector3(.025f,.42f,.025f),Wood);
    }

    static void CreateDunesAndRocks(Transform parent)
    {
        Vector3[] rocks={new Vector3(-8f,.2f,9f),new Vector3(-6.5f,.15f,-8.2f),new Vector3(1f,.15f,9.2f),new Vector3(4f,.18f,-8.8f),new Vector3(9f,.2f,8.6f),new Vector3(11f,.15f,-7.8f)};
        foreach(Vector3 p in rocks)
        {
            GameObject rock=Primitive(parent,"Coast Rock",PrimitiveType.Sphere,p,new Vector3(1.1f,.45f,.8f),Rock);
            rock.transform.rotation=Quaternion.Euler(0f,p.z*9f,0f);
        }
    }

    static void CreateTroyBackdrop(Transform parent)
    {
        float gateX=MapBuilder.CellToWorld(new Vector2Int(17,6)).x;
        float wallX=gateX+1.35f;
        Primitive(parent,"Troy Outer Wall",PrimitiveType.Cube,new Vector3(wallX,1.0f,0f),new Vector3(.9f,2.2f,18f),new Color(.58f,.47f,.29f));

        for(int i=-4;i<=4;i+=2)
        {
            Primitive(parent,"Troy Wall Tower",PrimitiveType.Cylinder,new Vector3(wallX-.05f,1.25f,i*2.0f),new Vector3(.95f,1.25f,.95f),new Color(.64f,.52f,.31f));
            Primitive(parent,"Wall Crown",PrimitiveType.Cylinder,new Vector3(wallX-.05f,2.55f,i*2.0f),new Vector3(1.08f,.10f,1.08f),new Color(.72f,.59f,.35f));
        }

        CreateTrojanStandard(parent,new Vector3(gateX+.70f,2.65f,2.2f));
        CreateTrojanStandard(parent,new Vector3(gateX+.70f,2.65f,-2.2f));
    }

    static void CreateFireIdentity(Transform parent)
    {
        float gateX = MapBuilder.CellToWorld(new Vector2Int(17,6)).x;
        CreateBrazier(parent, new Vector3(gateX + .30f, .15f, 3.95f), 1.15f);
        CreateBrazier(parent, new Vector3(gateX + .30f, .15f, -3.95f), 1.15f);
        CreateBrazier(parent, new Vector3(gateX - 1.10f, .08f, 1.25f), .82f);
        CreateBrazier(parent, new Vector3(gateX - 1.10f, .08f, -1.25f), .82f);

        CreateSmokeColumn(parent, new Vector3(gateX + .45f, 2.95f, 4.05f), 1.05f);
        CreateSmokeColumn(parent, new Vector3(gateX + .45f, 2.95f, -4.05f), .95f);

        CreateCampfire(parent, new Vector3(-10.7f, .08f, 6.9f));
        CreateCampfire(parent, new Vector3(-9.8f, .08f, -5.4f));
    }

    static void CreateTrojanStandard(Transform parent,Vector3 position)
    {
        Primitive(parent,"Trojan Standard",PrimitiveType.Cylinder,position,new Vector3(.045f,.9f,.045f),Wood);
        Primitive(parent,"Trojan Banner",PrimitiveType.Cube,position+new Vector3(0f,.45f,0f),new Vector3(.08f,.55f,.42f),new Color(.52f,.08f,.06f));
        Primitive(parent,"Banner Gold",PrimitiveType.Cube,position+new Vector3(-.05f,.45f,0f),new Vector3(.02f,.08f,.44f),new Color(.88f,.63f,.14f));
    }

    static void CreateBrazier(Transform parent, Vector3 position, float scale)
    {
        GameObject root = new GameObject("Trojan Fire Brazier");
        root.transform.SetParent(parent, false);
        root.transform.localPosition = position;

        Primitive(root.transform,"Bronze Bowl",PrimitiveType.Cylinder,new Vector3(0f,.32f,0f),new Vector3(.32f*scale,.12f*scale,.32f*scale),new Color(.48f,.31f,.12f));
        Primitive(root.transform,"Coal Bed",PrimitiveType.Cylinder,new Vector3(0f,.43f,0f),new Vector3(.25f*scale,.035f*scale,.25f*scale),new Color(.09f,.045f,.025f));
        Primitive(root.transform,"Flame Core",PrimitiveType.Sphere,new Vector3(0f,.66f,0f),new Vector3(.22f*scale,.38f*scale,.22f*scale),new Color(1f,.24f,.025f));
        Primitive(root.transform,"Flame Gold",PrimitiveType.Sphere,new Vector3(.03f,.75f,-.03f),new Vector3(.13f*scale,.27f*scale,.13f*scale),new Color(1f,.70f,.12f));

        Light light = root.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f,.48f,.14f);
        light.range = 4.8f * scale;
        light.intensity = 1.7f * scale;
    }

    static void CreateCampfire(Transform parent, Vector3 position)
    {
        GameObject root = new GameObject("Greek Campfire");
        root.transform.SetParent(parent, false);
        root.transform.localPosition = position;
        Primitive(root.transform,"Firewood A",PrimitiveType.Cylinder,new Vector3(0f,.18f,0f),new Vector3(.045f,.42f,.045f),Wood,Quaternion.Euler(80f,22f,0f));
        Primitive(root.transform,"Firewood B",PrimitiveType.Cylinder,new Vector3(0f,.18f,0f),new Vector3(.045f,.42f,.045f),Wood,Quaternion.Euler(80f,-28f,0f));
        Primitive(root.transform,"Camp Flame",PrimitiveType.Sphere,new Vector3(0f,.40f,0f),new Vector3(.16f,.26f,.16f),new Color(1f,.36f,.06f));
    }

    static void CreateSmokeColumn(Transform parent, Vector3 position, float scale)
    {
        for (int i = 0; i < 4; i++)
        {
            Vector3 offset = new Vector3(i * .10f, i * .34f, (i % 2 == 0 ? .08f : -.06f));
            GameObject puff = Primitive(parent,"Trojan Smoke",PrimitiveType.Sphere,position + offset,new Vector3((.34f + i * .09f) * scale,(.22f + i * .08f) * scale,(.34f + i * .09f) * scale),new Color(.18f,.16f,.14f));
            puff.transform.rotation = Quaternion.Euler(0f, i * 37f, 0f);
        }
    }

    static GameObject Primitive(Transform parent,string name,PrimitiveType type,Vector3 position,Vector3 scale,Color color,Quaternion? rotation=null)
    {
        GameObject go=GameObject.CreatePrimitive(type);
        go.name=name;
        go.transform.SetParent(parent,false);
        go.transform.localPosition=position;
        go.transform.localScale=scale;
        if(rotation.HasValue) go.transform.localRotation=rotation.Value;
        Object.Destroy(go.GetComponent<Collider>());
        TowerFactory.SetColor(go,color);
        return go;
    }
}
