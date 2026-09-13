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
            new Vector3(gateX+5.8f,4.75f,-4f), new Vector3(gateX+5.8f,4.75f,0f), new Vector3(gateX+5.8f,4.75f,4f),
            new Vector3(gateX+6.3f,5.15f,-2.6f), new Vector3(gateX+6.3f,5.15f,2.6f)
        };
        for(int i=0;i<fires.Length;i++) AddFire(root.transform,fires[i],i*.7f);

        for(int i=-3;i<=3;i+=2)
            AddWindowGlow(root.transform,new Vector3(gateX+3.0f,1.2f,i*2.2f));

        for(int i=-1;i<=1;i++)
            AddWallTorch(root.transform,new Vector3(gateX-.9f,2.0f,i*2.4f));
    }

    void AddFire(Transform parent,Vector3 pos,float phase)
    {
        GameObject flame=Part(parent,"Troy Fire",PrimitiveType.Sphere,pos,new Vector3(.22f,.40f,.22f),new Color(1f,.30f,.04f));
        ChapterOneAmbientMotion fm=flame.AddComponent<ChapterOneAmbientMotion>(); fm.kind=ChapterOneAmbientMotion.MotionKind.Flame; fm.phase=phase;
        Light l=flame.AddComponent<Light>(); l.type=LightType.Point; l.color=new Color(1f,.43f,.10f); l.range=5f; l.intensity=1.4f;
        for(int i=0;i<3;i++)
        {
            GameObject puff=Part(parent,"Troy Smoke",PrimitiveType.Sphere,pos+new Vector3(.05f*i,.55f+.34f*i,(i%2==0?.05f:-.04f)),new Vector3(.25f+i*.08f,.18f+i*.06f,.25f+i*.08f),new Color(.16f,.14f,.12f));
            ChapterOneAmbientMotion sm=puff.AddComponent<ChapterOneAmbientMotion>(); sm.kind=ChapterOneAmbientMotion.MotionKind.Smoke; sm.phase=phase+i*.6f;
        }
    }

    void AddWindowGlow(Transform parent,Vector3 pos)
    {
        GameObject glow=Part(parent,"Troy Window Glow",PrimitiveType.Cube,pos,new Vector3(.035f,.28f,.34f),new Color(1f,.54f,.12f));
        Light l=glow.AddComponent<Light>(); l.type=LightType.Point; l.color=new Color(1f,.40f,.08f); l.range=2.2f; l.intensity=.75f;
    }

    void AddWallTorch(Transform parent,Vector3 pos)
    {
        Part(parent,"Wall Torch Bracket",PrimitiveType.Cylinder,pos,new Vector3(.035f,.30f,.035f),new Color(.31f,.18f,.08f));
        GameObject flame=Part(parent,"Wall Torch Flame",PrimitiveType.Sphere,pos+new Vector3(0f,.38f,0f),new Vector3(.14f,.26f,.14f),new Color(1f,.34f,.04f));
        flame.AddComponent<ChapterOneAmbientMotion>().kind=ChapterOneAmbientMotion.MotionKind.Flame;
    }

    GameObject Part(Transform parent,string name,PrimitiveType type,Vector3 pos,Vector3 scale,Color color)
    {
        GameObject go=GameObject.CreatePrimitive(type);
        go.name=name; go.transform.SetParent(parent,false); go.transform.position=pos; go.transform.localScale=scale;
        Collider c=go.GetComponent<Collider>(); if(c!=null) Destroy(c); TowerFactory.SetColor(go,color); return go;
    }
}
