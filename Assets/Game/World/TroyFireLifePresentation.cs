using UnityEngine;

public sealed class TroyFireLifePresentation : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<TroyFireLifePresentation>() == null)
            new GameObject("TroyFireLifePresentation").AddComponent<TroyFireLifePresentation>();
    }

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.MapNumber != 1) return;
        if (GameObject.Find("Chapter01_TroyFireLife") != null) return;
        GameObject root = new GameObject("Chapter01_TroyFireLife");
        float gateX = MapBuilder.CellToWorld(new Vector2Int(17,6)).x;

        Vector3[] fires = {
            new Vector3(gateX+5.9f,4.40f,-6.8f), new Vector3(gateX+5.9f,4.40f,6.8f),
            new Vector3(gateX+8.1f,5.55f,-3.45f), new Vector3(gateX+8.1f,5.55f,3.45f),
            new Vector3(gateX+9.0f,5.72f,0f)
        };
        for(int i=0;i<fires.Length;i++) AddFire(root.transform,fires[i],i*.73f,i<2?1f:1.15f);

        Vector3[] windows = {
            new Vector3(gateX+2.95f,1.15f,-5.8f), new Vector3(gateX+2.95f,1.20f,-3.5f),
            new Vector3(gateX+3.10f,1.15f,-1.3f), new Vector3(gateX+3.05f,1.28f,1.2f),
            new Vector3(gateX+2.95f,1.15f,3.7f), new Vector3(gateX+3.00f,1.22f,5.8f),
            new Vector3(gateX+4.45f,1.52f,-2.8f), new Vector3(gateX+4.50f,1.58f,2.8f)
        };
        for(int i=0;i<windows.Length;i++) AddWindowGlow(root.transform,windows[i],i*.35f);

        for(int i=-2;i<=2;i++)
            AddWallTorch(root.transform,new Vector3(gateX-.92f,2.15f,i*1.72f),i*.41f);

        AddSmokeColumn(root.transform,new Vector3(gateX+7.55f,4.7f,-5.0f),.3f);
        AddSmokeColumn(root.transform,new Vector3(gateX+8.25f,5.2f,5.1f),1.4f);
        AddSmokeColumn(root.transform,new Vector3(gateX+9.45f,6.0f,1.9f),2.1f);
    }

    void AddFire(Transform parent,Vector3 pos,float phase,float scale)
    {
        Part(parent,"Troy Fire Bowl",PrimitiveType.Cylinder,pos-new Vector3(0f,.16f,0f),new Vector3(.32f*scale,.10f,.32f*scale),new Color(.46f,.29f,.10f));
        GameObject flame=Part(parent,"Troy Fire",PrimitiveType.Sphere,pos,new Vector3(.22f,.40f,.22f)*scale,new Color(1f,.30f,.04f));
        ChapterOneAmbientMotion fm=flame.AddComponent<ChapterOneAmbientMotion>(); fm.kind=ChapterOneAmbientMotion.MotionKind.Flame; fm.phase=phase;
        Light l=flame.AddComponent<Light>(); l.type=LightType.Point; l.color=new Color(1f,.43f,.10f); l.range=5.5f*scale; l.intensity=1.5f*scale;
        for(int i=0;i<4;i++)
        {
            Vector3 offset=new Vector3((i%2==0?.07f:-.05f)*i,.58f+.36f*i,(i%3-1)*.06f);
            GameObject puff=Part(parent,"Troy Smoke",PrimitiveType.Sphere,pos+offset,new Vector3(.24f+i*.09f,.17f+i*.06f,.24f+i*.09f)*scale,new Color(.16f,.14f,.12f));
            ChapterOneAmbientMotion sm=puff.AddComponent<ChapterOneAmbientMotion>(); sm.kind=ChapterOneAmbientMotion.MotionKind.Smoke; sm.phase=phase+i*.53f;
        }
    }

    void AddSmokeColumn(Transform parent,Vector3 pos,float phase)
    {
        Color smoke = new Color(.14f,.13f,.12f);
        for(int i=0;i<5;i++)
        {
            float spread=.05f+i*.05f;
            Vector3 offset=new Vector3((i%2==0?spread:-spread),i*.42f,(i%3-1)*.07f);
            GameObject puff=Part(parent,"City Smoke",PrimitiveType.Sphere,pos+offset,new Vector3(.24f+i*.10f,.18f+i*.06f,.24f+i*.10f),smoke*(1f+i*.03f));
            ChapterOneAmbientMotion motion=puff.AddComponent<ChapterOneAmbientMotion>(); motion.kind=ChapterOneAmbientMotion.MotionKind.Smoke; motion.phase=phase+i*.48f;
        }
    }

    void AddWindowGlow(Transform parent,Vector3 pos,float phase)
    {
        GameObject glow=Part(parent,"Troy Window Glow",PrimitiveType.Cube,pos,new Vector3(.035f,.30f,.30f),new Color(1f,.51f,.10f));
        Light l=glow.AddComponent<Light>(); l.type=LightType.Point; l.color=new Color(1f,.38f,.07f); l.range=2.0f; l.intensity=.62f+Mathf.Abs(Mathf.Sin(phase))*.18f;
    }

    void AddWallTorch(Transform parent,Vector3 pos,float phase)
    {
        Part(parent,"Wall Torch Bracket",PrimitiveType.Cylinder,pos,new Vector3(.035f,.30f,.035f),new Color(.31f,.18f,.08f));
        Part(parent,"Wall Torch Cup",PrimitiveType.Cylinder,pos+new Vector3(0f,.28f,0f),new Vector3(.12f,.07f,.12f),new Color(.60f,.36f,.10f));
        GameObject flame=Part(parent,"Wall Torch Flame",PrimitiveType.Sphere,pos+new Vector3(0f,.48f,0f),new Vector3(.14f,.27f,.14f),new Color(1f,.34f,.04f));
        ChapterOneAmbientMotion motion=flame.AddComponent<ChapterOneAmbientMotion>(); motion.kind=ChapterOneAmbientMotion.MotionKind.Flame; motion.phase=phase;
        Light l=flame.AddComponent<Light>(); l.type=LightType.Point; l.color=new Color(1f,.42f,.09f); l.range=3.0f; l.intensity=.9f;
    }

    GameObject Part(Transform parent,string name,PrimitiveType type,Vector3 pos,Vector3 scale,Color color)
    {
        GameObject go=GameObject.CreatePrimitive(type);
        go.name=name; go.transform.SetParent(parent,false); go.transform.position=pos; go.transform.localScale=scale;
        Collider c=go.GetComponent<Collider>(); if(c!=null) Destroy(c); TowerFactory.SetColor(go,color); return go;
    }
}
