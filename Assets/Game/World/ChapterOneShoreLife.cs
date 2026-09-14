using UnityEngine;

public sealed class ChapterOneShoreLife : MonoBehaviour
{
    static readonly Color Foam = new Color(.91f,.94f,.89f);
    static readonly Color ThinFoam = new Color(.70f,.84f,.82f);
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
        BuildFoam(root.transform);
        BuildSeaGlints(root.transform);
        BuildShipWakes(root.transform);
        BuildSeaMist(root.transform);
        BuildCampfires(root.transform);
    }

    void BuildFoam(Transform parent)
    {
        BuildFoamRibbon(parent,-13.72f,-9.2f,11,1.82f,.24f,1.18f,Foam,0f);
        BuildFoamRibbon(parent,-14.25f,-8.4f,9,2.08f,.14f,.94f,ThinFoam,.9f);
        BuildFoamRibbon(parent,-13.18f,-7.9f,8,2.22f,.12f,.70f,Foam*.88f,1.8f);

        for (int i = 0; i < 10; i++)
        {
            float z = -9f + i * 1.95f;
            float x = -13.45f + ((i * 7) % 5) * .16f;
            GameObject fleck = Primitive(parent,"Shore Foam Fleck",PrimitiveType.Sphere,
                new Vector3(x,.002f,z),
                new Vector3(.10f + (i % 3) * .035f,.008f,.28f + (i % 2) * .12f),
                Foam * (.88f + (i % 2) * .08f),
                Quaternion.Euler(0f,-18f + i * 11f,0f));
            AddSeaMotion(fleck,2.4f + i * .31f);
        }
    }

    void BuildFoamRibbon(Transform parent,float x,float startZ,int count,float spacing,float width,float length,Color color,float phaseOffset)
    {
        for (int i = 0; i < count; i++)
        {
            float wobble = Mathf.Sin(i * 1.37f + phaseOffset) * .20f;
            float segmentLength = length * (.78f + (i % 4) * .09f);
            GameObject foam = Primitive(parent,"Breaking Shore Foam",PrimitiveType.Sphere,
                new Vector3(x + wobble,.002f,startZ + i * spacing),
                new Vector3(width,.010f,segmentLength),
                color * (.92f + (i % 3) * .035f),
                Quaternion.Euler(0f,-9f + (i % 5) * 4f,0f));
            AddSeaMotion(foam,phaseOffset + i * .37f);
        }
    }

    void BuildSeaGlints(Transform parent)
    {
        for (int i = 0; i < 12; i++)
        {
            float x = -16.1f - (i % 4) * 2.15f;
            float z = -9.1f + (i * 3.15f) % 18.2f;
            float length = 1.15f + (i % 5) * .42f;
            GameObject glint = Primitive(parent,"Moving Sea Glint",PrimitiveType.Sphere,
                new Vector3(x,-.165f,z),
                new Vector3(length,.009f,.055f + (i % 2) * .025f),
                SeaGlint * (.82f + (i % 4) * .06f),
                Quaternion.Euler(0f,-7f + (i % 5) * 3f,0f));
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

        for (int i = 0; i < wakes.Length; i++)
        {
            GameObject wakeA = Primitive(parent,"Moored Ship Wake",PrimitiveType.Sphere,wakes[i],
                new Vector3(1.8f,.010f,.075f),ThinFoam,Quaternion.Euler(0f,-5f+i*6f,0f));
            AddSeaMotion(wakeA,.7f+i*.8f);

            GameObject wakeB = Primitive(parent,"Moored Ship Wake",PrimitiveType.Sphere,
                wakes[i]+new Vector3(-.55f,-.006f,i%2==0?.18f:-.18f),
                new Vector3(1.15f,.008f,.045f),ThinFoam*.80f,Quaternion.Euler(0f,7f-i*5f,0f));
            AddSeaMotion(wakeB,1.3f+i*.65f);
        }
    }

    void BuildSeaMist(Transform parent)
    {
        Vector3[] wisps =
        {
            new Vector3(-13.45f,.16f,-7.2f), new Vector3(-13.65f,.13f,-2.8f),
            new Vector3(-13.38f,.15f,1.7f), new Vector3(-13.62f,.12f,5.9f),
            new Vector3(-15.1f,.08f,-5.0f), new Vector3(-15.25f,.09f,4.2f)
        };
        for(int i=0;i<wisps.Length;i++)
        {
            GameObject wisp=Primitive(parent,"Sea Spray Wisp",PrimitiveType.Sphere,wisps[i],
                new Vector3(.48f+(i%3)*.12f,.045f,.92f+(i%2)*.26f),SeaMist*(.82f+(i%2)*.07f),Quaternion.Euler(0f,-8f+i*7f,0f));
            ChapterOneAmbientMotion motion=wisp.AddComponent<ChapterOneAmbientMotion>();
            motion.kind=ChapterOneAmbientMotion.MotionKind.Dust;
            motion.phase=.4f+i*.73f;
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
