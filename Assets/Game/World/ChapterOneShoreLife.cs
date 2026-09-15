using UnityEngine;

public sealed class ChapterOneShoreLife : MonoBehaviour
{
    static readonly Color Foam = new Color(.91f,.94f,.89f);
    static readonly Color ThinFoam = new Color(.70f,.84f,.82f);
    static readonly Color Backwash = new Color(.43f,.57f,.55f);
    static readonly Color SwellBlue = new Color(.17f,.47f,.53f);
    static readonly Color SeaGlint = new Color(.24f,.56f,.64f);
    static readonly Color SeaMist = new Color(.54f,.72f,.73f);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<ChapterOneShoreLife>() == null)
            new GameObject("ChapterOneShoreLife").AddComponent<ChapterOneShoreLife>();
    }

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.MapNumber != 1) return;
        if (GameObject.Find("Chapter01_ShoreLife") != null) return;
        GameObject root = new GameObject("Chapter01_ShoreLife");
        BuildShallowSwells(root.transform);
        BuildFoam(root.transform);
        BuildBackwash(root.transform);
        BuildSeaGlints(root.transform);
        BuildShipWakes(root.transform);
        BuildSeaMist(root.transform);
        BuildCampfires(root.transform);
    }

    void BuildShallowSwells(Transform parent)
    {
        for (int i = 0; i < 14; i++)
        {
            float z = -9.5f + i * 1.46f;
            float shoreOffset = -1.72f - (i % 3) * .64f;
            float x = CoastEnvironmentBuilder.ShorelineX(z) + shoreOffset + Mathf.Sin(i * 1.21f) * .11f;
            GameObject swell = Primitive(parent,"Shallow Swell",PrimitiveType.Sphere,
                new Vector3(x,-.148f,z),
                new Vector3(.085f + (i % 2) * .025f,.008f,.62f + (i % 4) * .15f),
                SwellBlue * (.88f + (i % 3) * .045f),
                Quaternion.Euler(0f,-7f + (i % 5) * 3.5f,0f));
            AddSwellMotion(swell,.35f+i*.41f);
        }
    }

    void BuildFoam(Transform parent)
    {
        BuildBrokenSurfRibbon(parent,-.42f,-9.5f,13,1.55f,.25f,1.22f,Foam,0f,5);
        BuildBrokenSurfRibbon(parent,-.92f,-8.8f,11,1.78f,.16f,1.00f,ThinFoam,.8f,4);
        BuildBrokenSurfRibbon(parent,-1.38f,-8.2f,9,2.02f,.11f,.78f,Foam*.86f,1.65f,3);

        for (int i = 0; i < 11; i++)
        {
            float z = -9.2f + i * 1.82f;
            float x = CoastEnvironmentBuilder.ShorelineX(z) - .24f + ((i * 7) % 5) * .06f;
            GameObject fleck = Primitive(parent,"Shore Foam Fleck",PrimitiveType.Sphere,
                new Vector3(x,.002f,z),
                new Vector3(.09f + (i % 3) * .035f,.008f,.23f + (i % 2) * .11f),
                Foam * (.86f + (i % 2) * .09f),
                Quaternion.Euler(0f,-18f + i * 11f,0f));
            AddSurfMotion(fleck,2.4f + i * .31f);
        }
    }

    void BuildBrokenSurfRibbon(Transform parent,float shoreOffset,float startZ,int count,float spacing,float width,float length,Color color,float phaseOffset,int gapSeed)
    {
        for (int i = 0; i < count; i++)
        {
            if (i > 0 && i < count-1 && ((i + gapSeed) % 6 == 0 || (gapSeed < 5 && (i + gapSeed) % 8 == 3))) continue;

            float z = startZ + i * spacing;
            float lateralNoise = Mathf.Sin(i * 1.37f + phaseOffset) * .13f;
            float segmentLength = length * (.72f + (i % 4) * .11f);
            float segmentWidth = width * (.82f + ((i * 3 + gapSeed) % 4) * .07f);
            float x = CoastEnvironmentBuilder.ShorelineX(z) + shoreOffset + lateralNoise;
            GameObject foam = Primitive(parent,"Breaking Shore Foam",PrimitiveType.Sphere,
                new Vector3(x,.002f,z),
                new Vector3(segmentWidth,.010f,segmentLength),
                color * (.90f + (i % 3) * .035f),
                Quaternion.Euler(0f,-12f + ((i * 7 + gapSeed) % 7) * 4f,0f));
            AddSurfMotion(foam,phaseOffset + i * .37f);
        }
    }

    void BuildBackwash(Transform parent)
    {
        for(int i=0;i<9;i++)
        {
            float z=-8.7f+i*2.15f;
            float x=CoastEnvironmentBuilder.ShorelineX(z)+.30f+(i%3)*.09f;
            GameObject wash=Primitive(parent,"Shore Backwash",PrimitiveType.Sphere,
                new Vector3(x,-.020f,z),
                new Vector3(.16f,.006f,.58f+(i%3)*.14f),
                Backwash*(.88f+(i%2)*.06f),
                Quaternion.Euler(0f,-10f+i*5f,0f));
            AddSurfMotion(wash,1.1f+i*.53f);
        }
    }

    void BuildSeaGlints(Transform parent)
    {
        for (int i = 0; i < 10; i++)
        {
            float x = -15.7f - (i % 4) * 2.35f - (i/4)*.35f;
            float z = -8.9f + (i * 3.47f) % 17.8f;
            float length = .95f + (i % 5) * .39f;
            GameObject glint = Primitive(parent,"Moving Sea Glint",PrimitiveType.Sphere,
                new Vector3(x,-.165f,z),
                new Vector3(length,.008f,.045f + (i % 2) * .022f),
                SeaGlint * (.80f + (i % 4) * .055f),
                Quaternion.Euler(0f,-8f + (i % 5) * 3.5f,0f));
            AddSeaMotion(glint,1.4f + i * .29f);
        }
    }

    void BuildShipWakes(Transform parent)
    {
        Vector3[] wakes =
        {
            new Vector3(-17.8f,-.145f,5.7f),
            new Vector3(-18.9f,-.145f,.5f),
            new Vector3(-17.4f,-.145f,-5.7f)
        };
        float[] yaw = { -8f,5f,-4f };

        for (int i = 0; i < wakes.Length; i++)
        {
            GameObject wakeRoot = new GameObject("Greek Ship Wake");
            wakeRoot.transform.SetParent(parent,false);
            wakeRoot.transform.localPosition = wakes[i];
            wakeRoot.transform.localRotation = Quaternion.Euler(0f,yaw[i],0f);

            GameObject left = Primitive(wakeRoot.transform,"Ship Wake Arm",PrimitiveType.Sphere,
                new Vector3(-.72f,0f,.16f),new Vector3(1.38f,.010f,.060f),ThinFoam,
                Quaternion.Euler(0f,-7f,0f));
            GameObject right = Primitive(wakeRoot.transform,"Ship Wake Arm",PrimitiveType.Sphere,
                new Vector3(-.72f,-.004f,-.16f),new Vector3(1.30f,.009f,.052f),ThinFoam*.90f,
                Quaternion.Euler(0f,7f,0f));
            GameObject churn = Primitive(wakeRoot.transform,"Ship Wake Turbulence",PrimitiveType.Sphere,
                new Vector3(-.18f,.004f,0f),new Vector3(.58f,.012f,.22f),Foam*.82f,
                Quaternion.Euler(0f,3f-i*2f,0f));
            AddSwellMotion(left,.7f+i*.8f);
            AddSwellMotion(right,1.1f+i*.71f);
            AddSeaMotion(churn,1.45f+i*.63f);
        }
    }

    void BuildSeaMist(Transform parent)
    {
        float[] shoreZ = { -7.2f,-2.8f,1.7f,5.9f };
        for(int i=0;i<shoreZ.Length;i++)
        {
            float z=shoreZ[i];
            Vector3 position=new Vector3(CoastEnvironmentBuilder.ShorelineX(z)-.42f,.12f+(i%2)*.03f,z);
            GameObject wisp=Primitive(parent,"Sea Spray Wisp",PrimitiveType.Sphere,position,
                new Vector3(.48f+(i%3)*.12f,.045f,.92f+(i%2)*.26f),SeaMist*(.82f+(i%2)*.07f),Quaternion.Euler(0f,-8f+i*7f,0f));
            ChapterOneAmbientMotion motion=wisp.AddComponent<ChapterOneAmbientMotion>();
            motion.kind=ChapterOneAmbientMotion.MotionKind.Dust;
            motion.phase=.4f+i*.73f;
        }

        Vector3[] outerWisps =
        {
            new Vector3(-15.1f,.08f,-5.0f),
            new Vector3(-15.25f,.09f,4.2f)
        };
        for(int i=0;i<outerWisps.Length;i++)
        {
            GameObject wisp=Primitive(parent,"Sea Spray Wisp",PrimitiveType.Sphere,outerWisps[i],
                new Vector3(.60f,.045f,1.05f),SeaMist*.84f,Quaternion.Euler(0f,10f-i*13f,0f));
            ChapterOneAmbientMotion motion=wisp.AddComponent<ChapterOneAmbientMotion>();
            motion.kind=ChapterOneAmbientMotion.MotionKind.Dust;
            motion.phase=3.8f+i*.73f;
        }
    }

    void BuildCampfires(Transform parent)
    {
        CreateCampfire(parent,new Vector3(-11.3f,.05f,4.9f));
        CreateCampfire(parent,new Vector3(-10.1f,.05f,-3.7f));
    }

    void CreateCampfire(Transform parent,Vector3 pos)
    {
        GameObject root = new GameObject("Beach Campfire Detail");
        root.transform.SetParent(parent,false);
        root.transform.localPosition = pos;

        for(int i=0;i<3;i++)
            Primitive(root.transform,"Log",PrimitiveType.Cylinder,new Vector3(0f,.10f,0f),new Vector3(.035f,.34f,.035f),new Color(.18f,.10f,.05f),Quaternion.Euler(82f,i*60f+25f,0f));

        GameObject flame = Primitive(root.transform,"Flame",PrimitiveType.Sphere,new Vector3(0f,.32f,0f),new Vector3(.15f,.25f,.15f),new Color(1f,.38f,.045f));
        ChapterOneAmbientMotion flameMotion = flame.AddComponent<ChapterOneAmbientMotion>();
        flameMotion.kind = ChapterOneAmbientMotion.MotionKind.Flame;

        for (int i = 0; i < 2; i++)
        {
            GameObject smoke = Primitive(root.transform,"Campfire Smoke",PrimitiveType.Sphere,
                new Vector3(i==0?-.03f:.05f,.58f+i*.20f,0f),
                Vector3.one*(.13f+i*.05f),
                new Color(.24f,.23f,.21f));
            ChapterOneAmbientMotion smokeMotion = smoke.AddComponent<ChapterOneAmbientMotion>();
            smokeMotion.kind = ChapterOneAmbientMotion.MotionKind.Smoke;
            smokeMotion.phase = .6f+i*1.2f;
        }

        for(int i=0;i<4;i++)
        {
            GameObject ember=Primitive(root.transform,"Campfire Ember",PrimitiveType.Sphere,
                new Vector3((i-1.5f)*.045f,.42f,(i%2==0?.05f:-.04f)),Vector3.one*(.025f+(i%2)*.008f),
                i%2==0?new Color(1f,.56f,.08f):new Color(1f,.30f,.03f));
            ChapterOneAmbientMotion emberMotion=ember.AddComponent<ChapterOneAmbientMotion>();
            emberMotion.kind=ChapterOneAmbientMotion.MotionKind.Ember;
            emberMotion.phase=i*.61f;
        }

        Light light = root.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f,.46f,.12f);
        light.range = 3.4f;
        light.intensity = 1.25f;
    }

    static void AddSeaMotion(GameObject go,float phase)
    {
        ChapterOneAmbientMotion motion = go.AddComponent<ChapterOneAmbientMotion>();
        motion.kind = ChapterOneAmbientMotion.MotionKind.Sea;
        motion.phase = phase;
    }

    static void AddSurfMotion(GameObject go,float phase)
    {
        ChapterOneAmbientMotion motion = go.AddComponent<ChapterOneAmbientMotion>();
        motion.kind = ChapterOneAmbientMotion.MotionKind.Surf;
        motion.phase = phase;
    }

    static void AddSwellMotion(GameObject go,float phase)
    {
        ChapterOneAmbientMotion motion = go.AddComponent<ChapterOneAmbientMotion>();
        motion.kind = ChapterOneAmbientMotion.MotionKind.Swell;
        motion.phase = phase;
    }

    GameObject Primitive(Transform parent,string name,PrimitiveType type,Vector3 position,Vector3 scale,Color color,Quaternion? rotation=null)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent,false);
        go.transform.localPosition = position;
        go.transform.localScale = scale;
        if(rotation.HasValue) go.transform.localRotation = rotation.Value;
        Collider c = go.GetComponent<Collider>();
        if(c != null) Destroy(c);
        TowerFactory.SetColor(go,color);
        return go;
    }
}
