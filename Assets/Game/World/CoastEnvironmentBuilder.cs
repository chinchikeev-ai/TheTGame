using UnityEngine;

// Static coast geometry only. Dynamic shore motion belongs to ChapterOneShoreLife;
// Troy wall/gate/city/fire have dedicated presentation owners.
public static class CoastEnvironmentBuilder
{
    static readonly Color Sand = new Color(.70f,.58f,.37f);
    static readonly Color LightSand = new Color(.82f,.69f,.44f);
    static readonly Color WetSand = new Color(.47f,.41f,.30f);
    static readonly Color DryTransition = new Color(.60f,.50f,.32f);
    static readonly Color ShallowWater = new Color(.09f,.38f,.46f);
    static readonly Color ShelfWater = new Color(.075f,.33f,.43f);
    static readonly Color Water = new Color(.055f,.27f,.39f);
    static readonly Color DeepWater = new Color(.03f,.13f,.24f);
    static readonly Color Rock = new Color(.35f,.34f,.30f);
    static readonly Color PebbleLight = new Color(.48f,.46f,.39f);
    static readonly Color PebbleDark = new Color(.29f,.29f,.27f);
    static readonly Color Wood = new Color(.31f,.18f,.075f);
    static readonly Color GreekCloth = new Color(.29f,.39f,.56f);
    static readonly Color GreekDark = new Color(.16f,.25f,.40f);
    static readonly Color Bronze = new Color(.66f,.43f,.15f);
    static readonly Color Terracotta = new Color(.56f,.27f,.14f);

    public static float ShorelineX(float z)
    {
        return -13.05f
            + Mathf.Sin(z * .43f) * .30f
            + Mathf.Sin(z * .91f + 1.15f) * .13f;
    }

    public static Transform Build()
    {
        GameObject root = new GameObject("Chapter01_CoastEnvironment");
        CreateGround(root.transform);
        CreateSea(root.transform);
        CreateShorelineBands(root.transform);
        CreateBeachSurfaceDetails(root.transform);
        CreateGreekLanding(root.transform);
        CreateGreekCampLandmarks(root.transform);
        CreateDunesAndRocks(root.transform);
        CreateWashedDebris(root.transform);
        return root.transform;
    }

    static void CreateGround(Transform parent)
    {
        Primitive(parent,"Sandy Coast Base",PrimitiveType.Cube,new Vector3(2f,-.20f,0f),new Vector3(30f,.18f,22f),Sand);

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

        Vector3[] sunBleachedMounds =
        {
            new Vector3(-9.0f,-.025f,5.55f), new Vector3(-8.35f,-.025f,-5.35f),
            new Vector3(-6.55f,-.025f,7.55f), new Vector3(-6.1f,-.025f,-7.15f)
        };
        for(int i=0;i<sunBleachedMounds.Length;i++)
            Primitive(parent,"Sun Bleached Sand Mound",PrimitiveType.Sphere,sunBleachedMounds[i],
                new Vector3(1.35f+(i%2)*.25f,.09f,.72f+(i%3)*.10f),LightSand*(.98f+(i%2)*.025f),Quaternion.Euler(0f,i*37f-12f,0f));
    }

    static void CreateSea(Transform parent)
    {
        Primitive(parent,"Aegean Shallows",PrimitiveType.Cube,new Vector3(-15.6f,-.24f,0f),new Vector3(4.8f,.10f,25f),ShallowWater);
        Primitive(parent,"Aegean Mid Water",PrimitiveType.Cube,new Vector3(-21.9f,-.29f,0f),new Vector3(7.9f,.11f,25f),Water);
        Primitive(parent,"Deep Aegean Sea",PrimitiveType.Cube,new Vector3(-29.8f,-.34f,0f),new Vector3(8f,.10f,25f),DeepWater);

        CreateShoreBand(parent,"Aegean Shelf Transition",-5.55f,-2.40f,-.181f,ShelfWater,.17f,.65f);
        CreateShoreBand(parent,"Aegean Mid Shelf",-8.65f,-5.25f,-.224f,Water*1.08f,.16f,1.45f);

        float[] sandbarZ = { -7.8f,-3.7f,.2f,4.3f,8.2f };
        for (int i = 0; i < sandbarZ.Length; i++)
        {
            Primitive(parent,"Shallow Water Variation",PrimitiveType.Sphere,
                new Vector3(-14.55f,-.175f,sandbarZ[i]),
                new Vector3(1.75f,.025f,1.4f + (i % 2) * .45f),
                new Color(.13f,.43f,.48f),
                Quaternion.Euler(0f,-8f + i * 4f,0f));
        }

        float[] shoalZ = { -8.8f,-5.4f,-1.9f,1.9f,5.4f,8.7f };
        for(int i=0;i<shoalZ.Length;i++)
        {
            float z=shoalZ[i];
            float x=ShorelineX(z)-1.35f-(i%3)*.31f;
            Primitive(parent,"Submerged Shoal",PrimitiveType.Sphere,new Vector3(x,-.149f,z),
                new Vector3(.58f+(i%2)*.16f,.008f,.92f+(i%3)*.20f),
                ShallowWater*(1.08f+(i%2)*.035f),Quaternion.Euler(0f,-12f+i*9f,0f));
        }
    }

    static void CreateShorelineBands(Transform parent)
    {
        CreateShoreBand(parent,"Shore Shallow Gradient Band",-2.55f,-.10f,-.168f,ShallowWater*1.08f,.10f,.3f);
        CreateShoreBand(parent,"Shore Wet Band",-.14f,1.34f,-.073f,WetSand,.10f,1.2f);
        CreateShoreBand(parent,"Shore Dry Sand Band",1.12f,4.55f,-.082f,LightSand*.98f,.20f,2.0f);
        CreateShoreBand(parent,"Shore Land Transition Band",4.20f,7.25f,-.094f,DryTransition,.18f,2.8f);
    }

    static void CreateShoreBand(Transform parent,string name,float leftOffset,float rightOffset,float y,Color color,float edgeWave,float phase)
    {
        const int segments = 30;
        const float minZ = -11.0f;
        const float maxZ = 11.0f;
        Vector3[] vertices = new Vector3[(segments + 1) * 2];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[segments * 6];

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float z = Mathf.Lerp(minZ,maxZ,t);
            float shore = ShorelineX(z);
            float left = shore + leftOffset + Mathf.Sin(z*.57f+phase)*edgeWave*.18f;
            float right = shore + rightOffset + Mathf.Sin(z*.36f+phase)*edgeWave;
            float microHeight = Mathf.Sin(z*.73f+phase)*.004f;
            vertices[i*2] = new Vector3(left,y+microHeight,z);
            vertices[i*2+1] = new Vector3(right,y-microHeight*.35f,z);
            uvs[i*2] = new Vector2(0f,t);
            uvs[i*2+1] = new Vector2(1f,t);
        }

        for (int i = 0; i < segments; i++)
        {
            int v = i * 2;
            int q = i * 6;
            triangles[q] = v;
            triangles[q+1] = v+2;
            triangles[q+2] = v+1;
            triangles[q+3] = v+1;
            triangles[q+4] = v+2;
            triangles[q+5] = v+3;
        }

        GameObject go = new GameObject(name);
        go.transform.SetParent(parent,false);
        Mesh mesh = new Mesh { name = name + " Mesh" };
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        go.AddComponent<MeshRenderer>();
        TowerFactory.SetColor(go,color);
    }

    static void CreateBeachSurfaceDetails(Transform parent)
    {
        float[] pebbleZ = { -8.6f,-6.4f,-1.8f,1.7f,6.3f,8.55f };
        for(int c=0;c<pebbleZ.Length;c++)
        {
            float z = pebbleZ[c];
            float x = ShorelineX(z) + 1.65f + (c%2)*.38f;
            CreatePebbleCluster(parent,new Vector3(x,-.012f,z),c);
        }

        Vector3[] berms =
        {
            new Vector3(-10.0f,-.018f,9.05f), new Vector3(-8.6f,-.018f,7.65f),
            new Vector3(-9.3f,-.018f,-8.95f), new Vector3(-7.5f,-.018f,-7.65f),
            new Vector3(-6.0f,-.018f,9.25f), new Vector3(-5.5f,-.018f,-9.15f)
        };
        for(int i=0;i<berms.Length;i++)
        {
            Primitive(parent,"Low Shore Berm",PrimitiveType.Sphere,berms[i],
                new Vector3(1.55f+(i%2)*.28f,.12f,.66f+(i%3)*.10f),
                LightSand*(.92f+(i%3)*.025f),Quaternion.Euler(0f,-18f+i*29f,0f));
        }
    }

    static void CreatePebbleCluster(Transform parent,Vector3 center,int seed)
    {
        GameObject root = new GameObject("Pebble Cluster");
        root.transform.SetParent(parent,false);
        root.transform.localPosition = center;
        for(int i=0;i<7;i++)
        {
            float angle = (seed*41f+i*53f)*Mathf.Deg2Rad;
            float radius = .18f + (i%4)*.12f;
            Vector3 p = new Vector3(Mathf.Cos(angle)*radius,.025f,Mathf.Sin(angle)*radius*.78f);
            Vector3 scale = new Vector3(.10f+(i%3)*.045f,.035f+(i%2)*.018f,.08f+((i+1)%3)*.040f);
            Primitive(root.transform,"Beach Pebble",PrimitiveType.Sphere,p,scale,i%2==0?PebbleLight:PebbleDark,Quaternion.Euler(0f,seed*23f+i*37f,0f));
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
            Primitive(spear.transform,"Bronze Spear Tip",PrimitiveType.Sphere,new Vector3(0f,1.0f,0f),new Vector3(2.1f,.16f,2.1f),Bronze);
        }
    }

    static void CreateGreekCampLandmarks(Transform parent)
    {
        CreateCommandAwning(parent,new Vector3(-7.75f,.03f,6.55f),-8f);
        CreateShieldRack(parent,new Vector3(-8.25f,.03f,-6.85f),10f);
        CreateLandingRamp(parent,new Vector3(-11.45f,-.015f,.05f),4f);
        CreateBaggageLine(parent,new Vector3(-8.25f,.02f,-8.15f),-7f);
        CreateBaggageLine(parent,new Vector3(-7.75f,.02f,8.45f),9f);
    }

    static void CreateCommandAwning(Transform parent,Vector3 position,float yaw)
    {
        GameObject root=new GameObject("Greek Command Awning");
        root.transform.SetParent(parent,false);
        root.transform.localPosition=position;
        root.transform.localRotation=Quaternion.Euler(0f,yaw,0f);

        float[] xs={-.72f,.72f};
        float[] zs={-.48f,.48f};
        for(int xi=0;xi<xs.Length;xi++)
            for(int zi=0;zi<zs.Length;zi++)
                Primitive(root.transform,"Awning Pole",PrimitiveType.Cylinder,new Vector3(xs[xi],.62f,zs[zi]),new Vector3(.025f,.62f,.025f),Wood);

        Primitive(root.transform,"Blue Command Canopy",PrimitiveType.Cube,new Vector3(0f,1.18f,0f),new Vector3(1.65f,.055f,1.18f),GreekCloth,Quaternion.Euler(0f,0f,-2f));
        Primitive(root.transform,"Canopy Dark Border",PrimitiveType.Cube,new Vector3(0f,1.145f,-.57f),new Vector3(1.68f,.07f,.07f),GreekDark);
        Primitive(root.transform,"Canopy Bronze Stripe",PrimitiveType.Cube,new Vector3(0f,1.225f,.02f),new Vector3(.16f,.025f,1.20f),Bronze);
        Primitive(root.transform,"Command Table",PrimitiveType.Cube,new Vector3(0f,.34f,0f),new Vector3(.90f,.09f,.55f),Wood*1.12f);
        Primitive(root.transform,"Map Weight",PrimitiveType.Sphere,new Vector3(.23f,.42f,.08f),new Vector3(.10f,.06f,.10f),Bronze*.92f);
    }

    static void CreateShieldRack(Transform parent,Vector3 position,float yaw)
    {
        GameObject root=new GameObject("Greek Shield Rack");
        root.transform.SetParent(parent,false);
        root.transform.localPosition=position;
        root.transform.localRotation=Quaternion.Euler(0f,yaw,0f);

        Primitive(root.transform,"Rack Beam",PrimitiveType.Cube,new Vector3(0f,.62f,0f),new Vector3(1.65f,.08f,.10f),Wood);
        Primitive(root.transform,"Rack Leg",PrimitiveType.Cylinder,new Vector3(-.68f,.35f,0f),new Vector3(.035f,.35f,.035f),Wood,Quaternion.Euler(0f,0f,8f));
        Primitive(root.transform,"Rack Leg",PrimitiveType.Cylinder,new Vector3(.68f,.35f,0f),new Vector3(.035f,.35f,.035f),Wood,Quaternion.Euler(0f,0f,-8f));
        for(int i=0;i<3;i++)
        {
            GameObject shield=Primitive(root.transform,"Racked Greek Shield",PrimitiveType.Cylinder,new Vector3((i-1)*.48f,.52f,-.09f),new Vector3(.26f,.035f,.26f),i==1?GreekCloth:GreekDark,Quaternion.Euler(90f,0f,0f));
            Primitive(shield.transform,"Bronze Shield Boss",PrimitiveType.Sphere,new Vector3(0f,.08f,0f),new Vector3(.12f,.045f,.12f),Bronze);
        }
    }

    static void CreateLandingRamp(Transform parent,Vector3 position,float yaw)
    {
        GameObject root=new GameObject("Improvised Landing Ramp");
        root.transform.SetParent(parent,false);
        root.transform.localPosition=position;
        root.transform.localRotation=Quaternion.Euler(0f,yaw,0f);
        for(int i=0;i<6;i++)
        {
            float z=(i-2.5f)*.28f;
            Primitive(root.transform,"Wet Landing Plank",PrimitiveType.Cube,new Vector3(0f,.025f,z),new Vector3(1.55f,.055f,.22f),Wood*(.84f+(i%3)*.07f),Quaternion.Euler((i%2==0?1f:-1f),0f,(i-2)*.6f));
        }
        Primitive(root.transform,"Ramp Rope",PrimitiveType.Cylinder,new Vector3(-.61f,.08f,0f),new Vector3(.018f,.78f,.018f),new Color(.33f,.27f,.18f),Quaternion.Euler(90f,0f,0f));
        Primitive(root.transform,"Ramp Rope",PrimitiveType.Cylinder,new Vector3(.61f,.08f,0f),new Vector3(.018f,.78f,.018f),new Color(.33f,.27f,.18f),Quaternion.Euler(90f,0f,0f));
    }

    static void CreateBaggageLine(Transform parent,Vector3 position,float yaw)
    {
        GameObject root=new GameObject("Greek Baggage Stack");
        root.transform.SetParent(parent,false);
        root.transform.localPosition=position;
        root.transform.localRotation=Quaternion.Euler(0f,yaw,0f);
        Primitive(root.transform,"Baggage Crate",PrimitiveType.Cube,new Vector3(-.42f,.18f,.05f),new Vector3(.54f,.36f,.48f),Wood*.94f);
        Primitive(root.transform,"Baggage Crate",PrimitiveType.Cube,new Vector3(.12f,.14f,.18f),new Vector3(.42f,.28f,.42f),Wood*1.12f,Quaternion.Euler(0f,15f,0f));
        Primitive(root.transform,"Rolled Cloth",PrimitiveType.Cylinder,new Vector3(.52f,.16f,-.12f),new Vector3(.14f,.34f,.14f),GreekCloth,Quaternion.Euler(90f,0f,0f));
        Primitive(root.transform,"Amphora",PrimitiveType.Sphere,new Vector3(.72f,.20f,.25f),new Vector3(.17f,.25f,.17f),Terracotta);
    }

    static void CreateTent(Transform parent, Vector3 position, float yaw)
    {
        GameObject root = new GameObject("Greek A-Frame Tent");
        root.transform.SetParent(parent,false);
        root.transform.localPosition = position;
        root.transform.localRotation = Quaternion.Euler(0f,yaw,0f);

        Primitive(root.transform,"Tent Ground Cloth",PrimitiveType.Cube,new Vector3(0f,.045f,0f),new Vector3(1.45f,.05f,1.25f),GreekDark * .92f);
        Primitive(root.transform,"Tent Left Roof",PrimitiveType.Cube,new Vector3(-.31f,.42f,0f),new Vector3(.78f,.065f,1.22f),GreekCloth,Quaternion.Euler(0f,0f,-31f));
        Primitive(root.transform,"Tent Right Roof",PrimitiveType.Cube,new Vector3(.31f,.42f,0f),new Vector3(.78f,.065f,1.22f),GreekCloth * .91f,Quaternion.Euler(0f,0f,31f));
        Primitive(root.transform,"Tent Ridge Pole",PrimitiveType.Cylinder,new Vector3(0f,.70f,0f),new Vector3(.025f,.67f,.025f),Wood,Quaternion.Euler(90f,0f,0f));
        Primitive(root.transform,"Tent Front Pole",PrimitiveType.Cylinder,new Vector3(0f,.35f,-.62f),new Vector3(.022f,.35f,.022f),Wood);
        Primitive(root.transform,"Tent Rear Pole",PrimitiveType.Cylinder,new Vector3(0f,.35f,.62f),new Vector3(.022f,.35f,.022f),Wood);
        Primitive(root.transform,"Tent Bronze Mark",PrimitiveType.Cube,new Vector3(-.34f,.44f,-.63f),new Vector3(.13f,.17f,.015f),Bronze,Quaternion.Euler(0f,0f,-31f));
    }

    static void CreateSupplyCluster(Transform parent, Vector3 position, float yaw)
    {
        GameObject root = new GameObject("Greek Landing Supplies");
        root.transform.SetParent(parent,false);
        root.transform.localPosition = position;
        root.transform.localRotation = Quaternion.Euler(0f,yaw,0f);

        Primitive(root.transform,"Supply Crate",PrimitiveType.Cube,new Vector3(-.32f,.18f,0f),new Vector3(.48f,.36f,.46f),Wood);
        Primitive(root.transform,"Supply Crate",PrimitiveType.Cube,new Vector3(.18f,.14f,.28f),new Vector3(.38f,.28f,.38f),Wood * 1.14f,Quaternion.Euler(0f,17f,0f));
        Primitive(root.transform,"Amphora Body",PrimitiveType.Sphere,new Vector3(.38f,.22f,-.26f),new Vector3(.20f,.28f,.20f),Terracotta);
        Primitive(root.transform,"Amphora Neck",PrimitiveType.Cylinder,new Vector3(.38f,.43f,-.26f),new Vector3(.075f,.12f,.075f),Terracotta*1.08f);
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

        float[] shoreRockZ = { -9.1f,-6.7f,6.9f,9.0f };
        for(int i=0;i<shoreRockZ.Length;i++)
        {
            float z=shoreRockZ[i];
            float x=ShorelineX(z)+1.25f+(i%2)*.35f;
            Primitive(parent,"Shore Boulder",PrimitiveType.Sphere,new Vector3(x,.10f,z),
                new Vector3(.48f+(i%2)*.17f,.22f+(i%3)*.05f,.40f+((i+1)%2)*.14f),
                Rock*(.95f+(i%2)*.04f),Quaternion.Euler(0f,17f+i*41f,0f));
        }
    }

    static void CreateWashedDebris(Transform parent)
    {
        float[] driftZ = { -8.0f,-5.7f,-1.2f,2.1f,5.8f,8.35f };
        for(int i=0;i<driftZ.Length;i++)
        {
            float z=driftZ[i];
            GameObject root=new GameObject("Washed Driftwood Cluster");
            root.transform.SetParent(parent,false);
            root.transform.localPosition=new Vector3(ShorelineX(z)+.35f,.015f,z);
            root.transform.localRotation=Quaternion.Euler(0f,-24f+i*23f,0f);

            Primitive(root.transform,"Driftwood",PrimitiveType.Cylinder,new Vector3(0f,.055f,0f),
                new Vector3(.035f,.52f,.035f),Wood*(.72f+(i%3)*.08f),Quaternion.Euler(88f,0f,12f));
            if((i&1)==0)
                Primitive(root.transform,"Wreck Plank",PrimitiveType.Cube,new Vector3(.28f,.035f,.17f),
                    new Vector3(.52f,.055f,.13f),Wood*.84f,Quaternion.Euler(0f,18f,3f));
            Primitive(root.transform,"Washed Stone",PrimitiveType.Sphere,new Vector3(-.24f,.035f,-.18f),
                new Vector3(.16f,.07f,.12f),i%2==0?PebbleDark:PebbleLight,Quaternion.Euler(0f,i*37f,0f));
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
