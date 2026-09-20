using UnityEngine;

public sealed class TroyFireLifePresentation : MonoBehaviour
{

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

        AddEmberField(root.transform,new Vector3(gateX-.55f,1.15f,-3.8f),0f,7);
        AddEmberField(root.transform,new Vector3(gateX-.65f,1.05f,3.9f),1.7f,7);
        AddEmberField(root.transform,new Vector3(gateX+5.9f,3.9f,0f),3.1f,6);
    }

    void AddFire(Transform parent,Vector3 pos,float phase,float scale)
    {
        Part(parent,"Troy Fire Bowl",PrimitiveType.Cylinder,pos-new Vector3(0f,.16f,0f),new Vector3(.32f*scale,.10f,.32f*scale),new Color(.50f,.31f,.10f));
        GameObject flame=Part(parent,"Troy Fire",PrimitiveType.Sphere,pos,new Vector3(.24f,.44f,.24f)*scale,new Color(1f,.31f,.035f));
        ChapterOneAmbientMotion fm=flame.AddComponent<ChapterOneAmbientMotion>(); fm.kind=ChapterOneAmbientMotion.MotionKind.Flame; fm.phase=phase;
        GameObject core=Part(parent,"Troy Fire Core",PrimitiveType.Sphere,pos-new Vector3(0f,.05f,0f),new Vector3(.13f,.26f,.13f)*scale,new Color(1f,.72f,.12f));
        ChapterOneAmbientMotion coreMotion=core.AddComponent<ChapterOneAmbientMotion>(); coreMotion.kind=ChapterOneAmbientMotion.MotionKind.Flame; coreMotion.phase=phase+.31f;
        Light l=flame.AddComponent<Light>(); l.type=LightType.Point; l.color=new Color(1f,.45f,.10f); l.range=5.9f*scale; l.intensity=1.65f*scale;

        for(int i=0;i<5;i++)
        {
            Vector3 offset=new Vector3((i%2==0?.07f:-.05f)*i,.58f+.34f*i,(i%3-1)*.06f);
            GameObject puff=Part(parent,"Troy Smoke",PrimitiveType.Sphere,pos+offset,new Vector3(.24f+i*.095f,.17f+i*.06f,.24f+i*.095f)*scale,new Color(.16f,.14f,.12f));
            ChapterOneAmbientMotion sm=puff.AddComponent<ChapterOneAmbientMotion>(); sm.kind=ChapterOneAmbientMotion.MotionKind.Smoke; sm.phase=phase+i*.53f;
        }

        for(int i=0;i<5;i++)
        {
            Vector3 emberPos=pos+new Vector3((i-2)*.055f,.27f+(i%2)*.08f,(i%3-1)*.045f);
            GameObject ember=Part(parent,"Troy Fire Ember",PrimitiveType.Sphere,emberPos,Vector3.one*(.028f+(i%2)*.009f),i%2==0?new Color(1f,.67f,.09f):new Color(1f,.28f,.025f));
            ChapterOneAmbientMotion em=ember.AddComponent<ChapterOneAmbientMotion>(); em.kind=ChapterOneAmbientMotion.MotionKind.Ember; em.phase=phase+i*.58f;
        }
    }

    void AddEmberField(Transform parent,Vector3 pos,float phase,int count)
    {
        GameObject root=new GameObject("Troy Drifting Ember Field");
        root.transform.SetParent(parent,false);
        root.transform.position=pos;
        for(int i=0;i<count;i++)
        {
            float x=(i%3-1)*.34f+(i%2)*.08f;
            float z=(i%4-1.5f)*.28f;
            float y=(i%3)*.22f;
            GameObject ember=Part(root.transform,"Drifting Ember",PrimitiveType.Sphere,new Vector3(x,y,z),Vector3.one*(.022f+(i%3)*.008f),i%2==0?new Color(1f,.60f,.07f):new Color(1f,.25f,.02f));
            ChapterOneAmbientMotion motion=ember.AddComponent<ChapterOneAmbientMotion>();
            motion.kind=ChapterOneAmbientMotion.MotionKind.Ember;
            motion.phase=phase+i*.47f;
        }
    }

    void AddSmokeColumn(Transform parent,Vector3 pos,float phase)
    {
        Color smoke = new Color(.14f,.13f,.12f);
        for(int i=0;i<6;i++)
        {
            float spread=.05f+i*.055f;
            Vector3 offset=new Vector3((i%2==0?spread:-spread),i*.42f,(i%3-1)*.07f);
            GameObject puff=Part(parent,"City Smoke",PrimitiveType.Sphere,pos+offset,new Vector3(.24f+i*.105f,.18f+i*.065f,.24f+i*.105f),smoke*(1f+i*.03f));
            ChapterOneAmbientMotion motion=puff.AddComponent<ChapterOneAmbientMotion>(); motion.kind=ChapterOneAmbientMotion.MotionKind.Smoke; motion.phase=phase+i*.48f;
        }
    }

    void AddWindowGlow(Transform parent,Vector3 pos,float phase)
    {
        GameObject glow=Part(parent,"Troy Window Glow",PrimitiveType.Cube,pos,new Vector3(.035f,.30f,.30f),new Color(1f,.54f,.10f));
        Light l=glow.AddComponent<Light>(); l.type=LightType.Point; l.color=new Color(1f,.39f,.07f); l.range=2.15f; l.intensity=.68f+Mathf.Abs(Mathf.Sin(phase))*.18f;
    }

    void AddWallTorch(Transform parent,Vector3 pos,float phase)
    {
        Part(parent,"Wall Torch Bracket",PrimitiveType.Cylinder,pos,new Vector3(.035f,.30f,.035f),new Color(.31f,.18f,.08f));
        Part(parent,"Wall Torch Cup",PrimitiveType.Cylinder,pos+new Vector3(0f,.28f,0f),new Vector3(.12f,.07f,.12f),new Color(.60f,.36f,.10f));
        GameObject flame=Part(parent,"Wall Torch Flame",PrimitiveType.Sphere,pos+new Vector3(0f,.48f,0f),new Vector3(.14f,.27f,.14f),new Color(1f,.34f,.04f));
        ChapterOneAmbientMotion motion=flame.AddComponent<ChapterOneAmbientMotion>(); motion.kind=ChapterOneAmbientMotion.MotionKind.Flame; motion.phase=phase;
        GameObject ember=Part(parent,"Wall Torch Ember",PrimitiveType.Sphere,pos+new Vector3(.04f,.62f,0f),Vector3.one*.026f,new Color(1f,.62f,.07f));
        ChapterOneAmbientMotion emberMotion=ember.AddComponent<ChapterOneAmbientMotion>(); emberMotion.kind=ChapterOneAmbientMotion.MotionKind.Ember; emberMotion.phase=phase+.37f;
        Light l=flame.AddComponent<Light>(); l.type=LightType.Point; l.color=new Color(1f,.44f,.09f); l.range=3.2f; l.intensity=1.0f;
    }

    GameObject Part(Transform parent,string name,PrimitiveType type,Vector3 pos,Vector3 scale,Color color)
    {
        GameObject go=GameObject.CreatePrimitive(type);
        go.name=name; go.transform.SetParent(parent,false); go.transform.position=pos; go.transform.localScale=scale;
        Collider c=go.GetComponent<Collider>(); if(c!=null) Destroy(c); TowerFactory.SetColor(go,color); return go;
    }
}
