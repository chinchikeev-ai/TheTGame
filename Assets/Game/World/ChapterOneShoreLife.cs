using UnityEngine;

public sealed class ChapterOneShoreLife : MonoBehaviour
{
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
        BuildCampfires(root.transform);
    }

    void BuildFoam(Transform parent)
    {
        for (int i = -9; i <= 9; i += 2)
        {
            GameObject foam = Primitive(parent,"Animated Shore Foam",PrimitiveType.Cube,new Vector3(-13.85f,-.005f,i*1.05f),new Vector3(.18f,.018f,1.25f),new Color(.84f,.88f,.84f));
            ChapterOneAmbientMotion motion = foam.AddComponent<ChapterOneAmbientMotion>();
            motion.kind = ChapterOneAmbientMotion.MotionKind.Sea;
            motion.phase = i * .41f;
        }
    }

    void BuildSeaGlints(Transform parent)
    {
        for (int i = -8; i <= 8; i += 4)
        {
            GameObject glint = Primitive(parent,"Moving Sea Glint",PrimitiveType.Cube,new Vector3(-17.4f,-.18f,i),new Vector3(3.6f,.012f,.10f),new Color(.24f,.50f,.56f));
            ChapterOneAmbientMotion motion = glint.AddComponent<ChapterOneAmbientMotion>();
            motion.kind = ChapterOneAmbientMotion.MotionKind.Sea;
            motion.phase = 1.7f + i * .23f;
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
        for(int i=0;i<2;i++) Primitive(root.transform,"Log",PrimitiveType.Cylinder,new Vector3(0f,.12f,0f),new Vector3(.04f,.38f,.04f),new Color(.18f,.10f,.05f),Quaternion.Euler(82f,i==0?35f:-35f,0f));
        GameObject flame = Primitive(root.transform,"Flame",PrimitiveType.Sphere,new Vector3(0f,.34f,0f),new Vector3(.16f,.26f,.16f),new Color(1f,.34f,.05f));
        ChapterOneAmbientMotion motion = flame.AddComponent<ChapterOneAmbientMotion>();
        motion.kind = ChapterOneAmbientMotion.MotionKind.Flame;
        Light light = root.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f,.42f,.11f);
        light.range = 3.2f;
        light.intensity = 1.2f;
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
