using UnityEngine;

public sealed class ChapterOneBattlefieldDetails : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<ChapterOneBattlefieldDetails>() == null)
            new GameObject("ChapterOneBattlefieldDetails").AddComponent<ChapterOneBattlefieldDetails>();
    }

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.MapNumber != 1) return;
        if (GameObject.Find("Chapter01_BattlefieldDetails") != null) return;
        GameObject root = new GameObject("Chapter01_BattlefieldDetails");
        BuildDebris(root.transform);
        BuildStandards(root.transform);
        BuildRouteMarkers(root.transform);
    }

    void BuildDebris(Transform parent)
    {
        CreateBrokenBoat(parent,new Vector3(-12.4f,.05f,8.4f),-16f);
        CreateBrokenBoat(parent,new Vector3(-11.8f,.04f,-8.2f),13f);
        Vector3[] shields = { new Vector3(-10.7f,.12f,4.6f), new Vector3(-9.2f,.12f,-4.1f), new Vector3(-7.8f,.12f,6.9f), new Vector3(-6.5f,.12f,-6.7f) };
        for(int i=0;i<shields.Length;i++) CreateShield(parent,shields[i],i*31f);
        Vector3[] spears = { new Vector3(-10f,.08f,2.9f), new Vector3(-8.4f,.08f,-2.6f), new Vector3(-5.8f,.08f,5.2f), new Vector3(-3.9f,.08f,-5.8f) };
        for(int i=0;i<spears.Length;i++) Primitive(parent,"Broken Spear",PrimitiveType.Cylinder,spears[i],new Vector3(.028f,.72f,.028f),new Color(.29f,.17f,.08f),Quaternion.Euler(77f,0f,18f+i*19f));
        CreateScorchedPatch(parent,new Vector3(-8.9f,.015f,5.9f),1.2f);
        CreateScorchedPatch(parent,new Vector3(-6.9f,.015f,-5.4f),.9f);
    }

    void BuildStandards(Transform parent)
    {
        CreateStandard(parent,new Vector3(-10.8f,.05f,8.8f),-8f);
        CreateStandard(parent,new Vector3(-9.5f,.05f,-8.5f),9f);
    }

    void BuildRouteMarkers(Transform parent)
    {
        Color rock = new Color(.27f,.26f,.23f);
        for(int lane=-1;lane<=1;lane++)
        {
            float z=lane*4.6f;
            for(int i=0;i<5;i++)
            {
                float x=-8f+i*3.2f;
                Primitive(parent,"Route Edge Stone",PrimitiveType.Sphere,new Vector3(x,.08f,z+2.1f),new Vector3(.36f,.16f,.28f),rock,Quaternion.Euler(0f,i*29f,0f));
                Primitive(parent,"Route Edge Stone",PrimitiveType.Sphere,new Vector3(x+.8f,.07f,z-2f),new Vector3(.28f,.14f,.24f),rock,Quaternion.Euler(0f,i*37f,0f));
            }
        }
    }

    void CreateBrokenBoat(Transform parent,Vector3 pos,float yaw)
    {
        GameObject root=new GameObject("Broken Landing Boat"); root.transform.SetParent(parent,false); root.transform.localPosition=pos; root.transform.localRotation=Quaternion.Euler(0f,yaw,7f);
        Primitive(root.transform,"Hull A",PrimitiveType.Cube,new Vector3(-.7f,.16f,0f),new Vector3(1.35f,.22f,.62f),new Color(.18f,.10f,.05f),Quaternion.Euler(0f,0f,5f));
        Primitive(root.transform,"Hull B",PrimitiveType.Cube,new Vector3(.85f,.10f,.12f),new Vector3(.95f,.18f,.45f),new Color(.29f,.17f,.08f),Quaternion.Euler(0f,22f,-8f));
        Primitive(root.transform,"Snapped Mast",PrimitiveType.Cylinder,new Vector3(.10f,.24f,-.1f),new Vector3(.035f,.78f,.035f),new Color(.29f,.17f,.08f),Quaternion.Euler(71f,12f,0f));
    }

    void CreateShield(Transform parent,Vector3 pos,float yaw)
    {
        GameObject shield=Primitive(parent,"Fallen Greek Shield",PrimitiveType.Cylinder,pos,new Vector3(.34f,.035f,.34f),new Color(.54f,.07f,.035f),Quaternion.Euler(90f,yaw,0f));
        Primitive(shield.transform,"Shield Boss",PrimitiveType.Sphere,new Vector3(0f,.07f,0f),new Vector3(.13f,.05f,.13f),new Color(.52f,.30f,.10f));
    }

    void CreateScorchedPatch(Transform parent,Vector3 pos,float scale)
    {
        Primitive(parent,"Scorched Sand",PrimitiveType.Cylinder,pos,new Vector3(1.15f*scale,.018f,.82f*scale),new Color(.14f,.13f,.12f));
    }

    void CreateStandard(Transform parent,Vector3 pos,float yaw)
    {
        GameObject root=new GameObject("Greek Beach Standard"); root.transform.SetParent(parent,false); root.transform.localPosition=pos; root.transform.localRotation=Quaternion.Euler(0f,yaw,0f);
        Primitive(root.transform,"Pole",PrimitiveType.Cylinder,new Vector3(0f,.85f,0f),new Vector3(.035f,.85f,.035f),new Color(.29f,.17f,.08f));
        GameObject banner=Primitive(root.transform,"Banner",PrimitiveType.Cube,new Vector3(.28f,1.25f,0f),new Vector3(.56f,.40f,.03f),new Color(.54f,.07f,.035f));
        ChapterOneAmbientMotion motion=banner.AddComponent<ChapterOneAmbientMotion>(); motion.kind=ChapterOneAmbientMotion.MotionKind.Banner;
        Primitive(root.transform,"Bronze Stripe",PrimitiveType.Cube,new Vector3(.28f,1.25f,-.035f),new Vector3(.34f,.055f,.015f),new Color(.52f,.30f,.10f));
    }

    GameObject Primitive(Transform parent,string name,PrimitiveType type,Vector3 position,Vector3 scale,Color color,Quaternion? rotation=null)
    {
        GameObject go=GameObject.CreatePrimitive(type); go.name=name; go.transform.SetParent(parent,false); go.transform.localPosition=position; go.transform.localScale=scale; if(rotation.HasValue) go.transform.localRotation=rotation.Value;
        Collider c=go.GetComponent<Collider>(); if(c!=null) Destroy(c); TowerFactory.SetColor(go,color); return go;
    }
}
