using UnityEngine;

public sealed class ChapterOneBattlefieldDetails : MonoBehaviour
{
    static readonly Color GreekBlue = new Color(.31f,.39f,.49f);
    static readonly Color Bronze = new Color(.52f,.32f,.12f);
    static readonly Color Wood = new Color(.29f,.17f,.08f);
    static readonly Color DarkSand = new Color(.41f,.34f,.24f);
    static readonly Color Rock = new Color(.27f,.26f,.23f);

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

        Vector3[] shields =
        {
            new Vector3(-10.7f,.12f,4.6f), new Vector3(-9.2f,.12f,-4.1f),
            new Vector3(-7.8f,.12f,6.9f), new Vector3(-6.5f,.12f,-6.7f),
            new Vector3(-4.8f,.11f,2.3f), new Vector3(-3.6f,.11f,-2.2f)
        };
        for(int i=0;i<shields.Length;i++) CreateShield(parent,shields[i],i*31f);

        Vector3[] spears =
        {
            new Vector3(-10f,.08f,2.9f), new Vector3(-8.4f,.08f,-2.6f),
            new Vector3(-5.8f,.08f,5.2f), new Vector3(-3.9f,.08f,-5.8f),
            new Vector3(-1.7f,.08f,2.4f)
        };
        for(int i=0;i<spears.Length;i++)
            Primitive(parent,"Broken Spear",PrimitiveType.Cylinder,spears[i],new Vector3(.025f,.62f+(i%2)*.12f,.025f),Wood,Quaternion.Euler(76f+i%3*4f,0f,16f+i*23f));

        CreateScorchedPatch(parent,new Vector3(-8.9f,.015f,5.9f),1.2f);
        CreateScorchedPatch(parent,new Vector3(-6.9f,.015f,-5.4f),.9f);
        CreateScorchedPatch(parent,new Vector3(-2.6f,.015f,2.8f),.65f);
        CreateArrowBundle(parent,new Vector3(-7.2f,.06f,2.8f),18f);
        CreateArrowBundle(parent,new Vector3(-5.4f,.06f,-2.9f),-13f);
    }

    void BuildStandards(Transform parent)
    {
        CreateStandard(parent,new Vector3(-10.8f,.05f,8.8f),-8f);
        CreateStandard(parent,new Vector3(-9.5f,.05f,-8.5f),9f);
        CreateStandard(parent,new Vector3(-7.4f,.05f,7.8f),5f,.78f);
    }

    void BuildRouteMarkers(Transform parent)
    {
        Vector3[] edgeStones =
        {
            new Vector3(-10.8f,.07f,5.0f), new Vector3(-8.1f,.06f,2.55f),
            new Vector3(-5.9f,.07f,4.95f), new Vector3(-4.2f,.06f,2.05f),
            new Vector3(-1.2f,.06f,2.0f), new Vector3(2.5f,.07f,-1.15f),
            new Vector3(5.7f,.06f,2.0f), new Vector3(-10.2f,.07f,-5.0f),
            new Vector3(-7.4f,.06f,-2.45f), new Vector3(-4.8f,.07f,-4.95f),
            new Vector3(-2.0f,.06f,-2.0f), new Vector3(1.1f,.07f,-2.0f),
            new Vector3(4.1f,.06f,1.65f), new Vector3(7.0f,.07f,-1.2f)
        };
        for(int i=0;i<edgeStones.Length;i++)
        {
            Vector3 scale = new Vector3(.22f+(i%3)*.07f,.09f+(i%2)*.035f,.18f+((i+1)%3)*.06f);
            Primitive(parent,"Irregular Route Edge Stone",PrimitiveType.Sphere,edgeStones[i],scale,Rock*(.91f+(i%4)*.025f),Quaternion.Euler(0f,i*43f,0f));
        }

        CreateFootprintTrail(parent,new Vector3(-11.6f,.015f,3.15f),new Vector3(-5.7f,.015f,3.45f),7,0f);
        CreateFootprintTrail(parent,new Vector3(-11.3f,.015f,-3.15f),new Vector3(-5.8f,.015f,-3.45f),7,.55f);
        CreateFootprintTrail(parent,new Vector3(-4.9f,.015f,.45f),new Vector3(4.2f,.015f,.45f),9,1.1f);

        Vector3[] trampled =
        {
            new Vector3(-10.2f,.012f,3.75f), new Vector3(-8.0f,.012f,-3.75f),
            new Vector3(-5.15f,.012f,2.7f), new Vector3(-5.0f,.012f,-2.5f),
            new Vector3(-1.5f,.012f,.5f), new Vector3(2.8f,.012f,-.5f),
            new Vector3(6.0f,.012f,.35f)
        };
        for(int i=0;i<trampled.Length;i++)
            Primitive(parent,"Trampled Earth Patch",PrimitiveType.Sphere,trampled[i],new Vector3(.58f+(i%2)*.20f,.012f,.34f+(i%3)*.10f),DarkSand*(.90f+(i%3)*.035f),Quaternion.Euler(0f,i*29f-17f,0f));
    }

    void CreateFootprintTrail(Transform parent,Vector3 start,Vector3 end,int count,float phase)
    {
        for(int i=0;i<count;i++)
        {
            float t=(i+.5f)/count;
            Vector3 center=Vector3.Lerp(start,end,t);
            Vector3 dir=(end-start).normalized;
            Vector3 side=new Vector3(-dir.z,0f,dir.x);
            float stagger=(i%2==0?-.12f:.12f)+Mathf.Sin(i*1.7f+phase)*.035f;
            Vector3 p=center+side*stagger;
            float yaw=Mathf.Atan2(dir.x,dir.z)*Mathf.Rad2Deg+(i%2==0?-7f:7f);
            Primitive(parent,"Footprint",PrimitiveType.Sphere,p,new Vector3(.075f,.008f,.15f),DarkSand*.78f,Quaternion.Euler(0f,yaw,0f));
        }
    }

    void CreateBrokenBoat(Transform parent,Vector3 pos,float yaw)
    {
        GameObject root=new GameObject("Broken Landing Boat");
        root.transform.SetParent(parent,false);
        root.transform.localPosition=pos;
        root.transform.localRotation=Quaternion.Euler(0f,yaw,7f);
        Primitive(root.transform,"Hull A",PrimitiveType.Cube,new Vector3(-.7f,.16f,0f),new Vector3(1.35f,.22f,.62f),new Color(.18f,.10f,.05f),Quaternion.Euler(0f,0f,5f));
        Primitive(root.transform,"Hull B",PrimitiveType.Cube,new Vector3(.85f,.10f,.12f),new Vector3(.95f,.18f,.45f),Wood,Quaternion.Euler(0f,22f,-8f));
        Primitive(root.transform,"Snapped Mast",PrimitiveType.Cylinder,new Vector3(.10f,.24f,-.1f),new Vector3(.035f,.78f,.035f),Wood,Quaternion.Euler(71f,12f,0f));
        Primitive(root.transform,"Broken Plank",PrimitiveType.Cube,new Vector3(.10f,.10f,-.62f),new Vector3(.82f,.045f,.12f),Wood*1.18f,Quaternion.Euler(0f,-24f,8f));
        Primitive(root.transform,"Broken Plank",PrimitiveType.Cube,new Vector3(-.30f,.08f,.58f),new Vector3(.64f,.04f,.10f),Wood*.92f,Quaternion.Euler(0f,31f,-5f));
    }

    void CreateShield(Transform parent,Vector3 pos,float yaw)
    {
        GameObject shield=Primitive(parent,"Fallen Greek Shield",PrimitiveType.Cylinder,pos,new Vector3(.32f,.03f,.32f),GreekBlue,Quaternion.Euler(90f,yaw,0f));
        Primitive(shield.transform,"Shield Rim",PrimitiveType.Cylinder,new Vector3(0f,.01f,0f),new Vector3(1.08f,.55f,1.08f),Bronze*.85f);
        Primitive(shield.transform,"Shield Boss",PrimitiveType.Sphere,new Vector3(0f,.075f,0f),new Vector3(.13f,.05f,.13f),Bronze);
    }

    void CreateArrowBundle(Transform parent,Vector3 pos,float yaw)
    {
        GameObject root=new GameObject("Discarded Arrow Bundle");
        root.transform.SetParent(parent,false);
        root.transform.localPosition=pos;
        root.transform.localRotation=Quaternion.Euler(0f,yaw,0f);
        for(int i=0;i<5;i++)
        {
            Vector3 p=new Vector3((i-2)*.035f,.06f,(i%2)*.035f);
            Primitive(root.transform,"Arrow Shaft",PrimitiveType.Cylinder,p,new Vector3(.009f,.40f,.009f),Wood,Quaternion.Euler(74f,0f,7f+i*2f));
        }
    }

    void CreateScorchedPatch(Transform parent,Vector3 pos,float scale)
    {
        Primitive(parent,"Scorched Sand",PrimitiveType.Cylinder,pos,new Vector3(1.15f*scale,.018f,.82f*scale),new Color(.14f,.13f,.12f));
    }

    void CreateStandard(Transform parent,Vector3 pos,float yaw,float scale=1f)
    {
        GameObject root=new GameObject("Greek Beach Standard");
        root.transform.SetParent(parent,false);
        root.transform.localPosition=pos;
        root.transform.localRotation=Quaternion.Euler(0f,yaw,0f);
        root.transform.localScale=Vector3.one*scale;
        Primitive(root.transform,"Pole",PrimitiveType.Cylinder,new Vector3(0f,.85f,0f),new Vector3(.032f,.85f,.032f),Wood);
        GameObject banner=Primitive(root.transform,"Banner",PrimitiveType.Cube,new Vector3(.27f,1.25f,0f),new Vector3(.54f,.38f,.025f),GreekBlue);
        ChapterOneAmbientMotion motion=banner.AddComponent<ChapterOneAmbientMotion>();
        motion.kind=ChapterOneAmbientMotion.MotionKind.Banner;
        Primitive(root.transform,"Bronze Stripe",PrimitiveType.Cube,new Vector3(.27f,1.25f,-.030f),new Vector3(.32f,.05f,.012f),Bronze);
        Primitive(root.transform,"Torn Banner Tail",PrimitiveType.Cube,new Vector3(.48f,1.05f,0f),new Vector3(.18f,.13f,.022f),GreekBlue*.82f,Quaternion.Euler(0f,0f,-13f));
    }

    GameObject Primitive(Transform parent,string name,PrimitiveType type,Vector3 position,Vector3 scale,Color color,Quaternion? rotation=null)
    {
        GameObject go=GameObject.CreatePrimitive(type);
        go.name=name;
        go.transform.SetParent(parent,false);
        go.transform.localPosition=position;
        go.transform.localScale=scale;
        if(rotation.HasValue) go.transform.localRotation=rotation.Value;
        Collider c=go.GetComponent<Collider>();
        if(c!=null) Destroy(c);
        TowerFactory.SetColor(go,color);
        return go;
    }
}
