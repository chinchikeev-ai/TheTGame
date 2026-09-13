using UnityEngine;

// Sole owner of visible tower geometry and tower-unit crews.
public static class TowerArtDirector
{
    const string TrojanProductionRoot = "TroyProduction/Characters/Trojan/";
    const string TrojanGeneratedRoot = "TroyCharacters/Factions/Trojan/";

    public static void Enhance(GameObject root, TowerType type)
    {
        if (root == null || root.transform.Find("ArtEnhancement") != null) return;

        Transform head = root.transform.Find("Head");
        if (head == null)
        {
            GameObject headObject = new GameObject("Head");
            headObject.transform.SetParent(root.transform, false);
            headObject.transform.localPosition = new Vector3(0f,.72f,0f);
            head = headObject.transform;
        }

        GameObject art = new GameObject("ArtEnhancement");
        art.transform.SetParent(root.transform,false);

        Color wood = new Color(.27f,.15f,.07f);
        Color lightWood = new Color(.43f,.28f,.13f);
        Color bronze = new Color(.68f,.46f,.18f);
        Color red = new Color(.56f,.08f,.055f);
        Color stone = new Color(.48f,.40f,.28f);

        Part(art.transform,"Tower Base",PrimitiveType.Cylinder,Vector3.zero,new Vector3(.78f,.28f,.78f),stone*.72f);

        switch (type)
        {
            case TowerType.MachineGun:
                AddPost(art.transform,new Vector3(-.42f,.62f,-.30f),wood);
                AddPost(art.transform,new Vector3(.42f,.62f,-.30f),wood);
                AddPost(art.transform,new Vector3(-.42f,.62f,.30f),wood);
                AddPost(art.transform,new Vector3(.42f,.62f,.30f),wood);
                Part(art.transform,"Wooden Platform",PrimitiveType.Cube,new Vector3(0f,.95f,0f),new Vector3(1.05f,.12f,.90f),lightWood);
                Part(art.transform,"Red Canopy",PrimitiveType.Cylinder,new Vector3(0f,1.38f,0f),new Vector3(.72f,.16f,.72f),red).transform.rotation=Quaternion.Euler(0f,30f,0f);
                Part(art.transform,"Arrow Bundle",PrimitiveType.Cylinder,new Vector3(.48f,1.12f,.18f),new Vector3(.10f,.42f,.10f),wood).transform.rotation=Quaternion.Euler(10f,0f,-12f);
                Part(head,"Archer Bow",PrimitiveType.Cube,new Vector3(0f,.18f,.62f),new Vector3(.80f,.06f,.08f),bronze);
                AddProductionCrew(head,"Trojan_Archer",new Vector3(-.24f,.24f,-.02f),.60f,8f);
                AddProductionCrew(head,"Trojan_Archer",new Vector3(.24f,.24f,-.10f),.60f,-8f);
                break;

            case TowerType.Cannon:
                Part(art.transform,"Ballista Deck",PrimitiveType.Cube,new Vector3(0f,.55f,0f),new Vector3(1.15f,.14f,1.0f),wood);
                for (int s=-1;s<=1;s+=2)
                    Part(art.transform,"Ballista Support",PrimitiveType.Cube,new Vector3(s*.48f,.40f,0f),new Vector3(.12f,.62f,.14f),lightWood);
                Part(head,"Counterweight",PrimitiveType.Cube,new Vector3(0f,-.34f,-.36f),new Vector3(.38f,.38f,.38f),stone);
                Part(head,"Ballista Rail",PrimitiveType.Cube,new Vector3(0f,.33f,.18f),new Vector3(.16f,.12f,1.15f),lightWood);
                Part(head,"Ballista Bow",PrimitiveType.Cube,new Vector3(0f,.33f,.78f),new Vector3(1.18f,.08f,.10f),bronze);
                Part(head,"Ballista Bolt",PrimitiveType.Cylinder,new Vector3(0f,.35f,.86f),new Vector3(.035f,.62f,.035f),bronze,Quaternion.Euler(90f,0f,0f));
                AddProductionCrew(art.transform,"Trojan_Infantry",new Vector3(-.58f,.35f,-.40f),.58f,18f);
                break;

            case TowerType.Slow:
                for (int i=0;i<4;i++)
                {
                    float a=i*Mathf.PI*.5f;
                    Part(art.transform,"Shrine Column",PrimitiveType.Cylinder,new Vector3(Mathf.Cos(a)*.46f,.58f,Mathf.Sin(a)*.46f),new Vector3(.09f,.55f,.09f),new Color(.80f,.71f,.52f));
                }
                Part(art.transform,"Shrine Roof",PrimitiveType.Cylinder,new Vector3(0f,1.18f,0f),new Vector3(.72f,.11f,.72f),bronze).transform.rotation=Quaternion.Euler(0f,30f,0f);
                Part(art.transform,"Apollo Disc",PrimitiveType.Cylinder,new Vector3(0f,1.48f,0f),new Vector3(.28f,.05f,.28f),new Color(.94f,.72f,.20f)).transform.rotation=Quaternion.Euler(90f,0f,0f);
                Part(head,"Apollo Focus",PrimitiveType.Sphere,new Vector3(0f,.36f,.48f),Vector3.one*.22f,new Color(.98f,.72f,.18f));
                AddPriest(art.transform,new Vector3(-.22f,.35f,.08f),-8f);
                AddPriest(art.transform,new Vector3(.22f,.35f,-.02f),8f);
                break;

            case TowerType.SpearThrower:
                Part(art.transform,"Spear Rack",PrimitiveType.Cube,new Vector3(-.52f,.55f,-.05f),new Vector3(.18f,.80f,.42f),wood);
                for (int i=0;i<4;i++)
                {
                    GameObject spear=Part(art.transform,"Rack Spear",PrimitiveType.Cylinder,new Vector3(-.52f,.72f,-.30f+i*.20f),new Vector3(.025f,.72f,.025f),bronze);
                    spear.transform.rotation=Quaternion.Euler(0f,0f,-8f);
                }
                Part(art.transform,"Spear Crest",PrimitiveType.Cube,new Vector3(.28f,1.18f,0f),new Vector3(.12f,.34f,.42f),red);
                Part(head,"Aimed Spear",PrimitiveType.Cylinder,new Vector3(0f,.20f,.78f),new Vector3(.035f,.72f,.035f),bronze,Quaternion.Euler(90f,0f,0f));
                AddProductionCrew(art.transform,"Trojan_Infantry",new Vector3(-.20f,.30f,-.10f),.56f,5f);
                AddProductionCrew(art.transform,"Trojan_Infantry",new Vector3(.28f,.30f,-.12f),.56f,-5f);
                break;

            case TowerType.FireTower:
                Part(art.transform,"Fire Stone Base",PrimitiveType.Cylinder,new Vector3(0f,.30f,0f),new Vector3(.70f,.25f,.70f),stone);
                Part(art.transform,"Fire Bowl",PrimitiveType.Cylinder,new Vector3(0f,.86f,0f),new Vector3(.42f,.12f,.42f),bronze);
                GameObject flame=Part(art.transform,"Fire Core",PrimitiveType.Sphere,new Vector3(0f,1.08f,0f),new Vector3(.28f,.46f,.28f),new Color(1f,.32f,.04f));
                flame.AddComponent<ChapterOneAmbientMotion>().kind=ChapterOneAmbientMotion.MotionKind.Flame;
                AddFireKeeper(art.transform,new Vector3(.52f,.26f,-.20f));
                break;

            case TowerType.TrojanGuard:
                Part(art.transform,"Guard Platform",PrimitiveType.Cube,new Vector3(0f,.20f,0f),new Vector3(1.15f,.20f,1.0f),stone);
                AddStandard(art.transform,new Vector3(-.54f,1.05f,-.18f),red,bronze);
                AddStandard(art.transform,new Vector3(.54f,1.05f,-.18f),red,bronze);
                Part(art.transform,"Shield Rack",PrimitiveType.Cube,new Vector3(0f,.65f,.46f),new Vector3(.72f,.55f,.10f),red);
                AddProductionCrew(art.transform,"Trojan_Guard",new Vector3(-.26f,.26f,-.06f),.62f,5f);
                AddProductionCrew(art.transform,"Trojan_Guard",new Vector3(.26f,.26f,-.06f),.62f,-5f);
                break;
        }
    }

    static void AddProductionCrew(Transform parent,string prefabName,Vector3 localPosition,float scale,float yaw)
    {
        GameObject prefab=Resources.Load<GameObject>(TrojanProductionRoot+prefabName);
        if(prefab==null) prefab=Resources.Load<GameObject>(TrojanGeneratedRoot+prefabName);
        if(prefab==null) return;
        GameObject crew=Object.Instantiate(prefab,parent);
        crew.name="TowerCrew_"+prefabName;
        crew.transform.localPosition=localPosition;
        crew.transform.localRotation=Quaternion.Euler(0f,yaw,0f);
        crew.transform.localScale=Vector3.one*scale;
        foreach(Collider collider in crew.GetComponentsInChildren<Collider>(true))
        {
            collider.enabled=false;
            Object.Destroy(collider);
        }
    }

    static void AddPriest(Transform parent,Vector3 localPosition,float yaw)
    {
        GameObject root=new GameObject("PriestOfApollo");
        root.transform.SetParent(parent,false);
        root.transform.localPosition=localPosition;
        root.transform.localRotation=Quaternion.Euler(0f,yaw,0f);
        Part(root.transform,"Robe",PrimitiveType.Cylinder,new Vector3(0f,.34f,0f),new Vector3(.18f,.34f,.18f),new Color(.88f,.78f,.58f));
        Part(root.transform,"Head",PrimitiveType.Sphere,new Vector3(0f,.78f,0f),Vector3.one*.17f,new Color(.72f,.52f,.34f));
        Part(root.transform,"SunStaff",PrimitiveType.Cylinder,new Vector3(.22f,.48f,.02f),new Vector3(.025f,.48f,.025f),new Color(.72f,.48f,.18f));
    }

    static void AddFireKeeper(Transform parent,Vector3 localPosition)
    {
        GameObject root=new GameObject("FireKeeper");
        root.transform.SetParent(parent,false);
        root.transform.localPosition=localPosition;
        Part(root.transform,"Tunic",PrimitiveType.Cylinder,new Vector3(0f,.32f,0f),new Vector3(.17f,.32f,.17f),new Color(.48f,.09f,.045f));
        Part(root.transform,"Head",PrimitiveType.Sphere,new Vector3(0f,.72f,0f),Vector3.one*.16f,new Color(.72f,.50f,.32f));
        Part(root.transform,"PitchPot",PrimitiveType.Cylinder,new Vector3(-.22f,.26f,.10f),new Vector3(.12f,.18f,.12f),new Color(.17f,.11f,.07f));
    }

    static void AddPost(Transform parent,Vector3 p,Color c)
    {
        Part(parent,"Support Post",PrimitiveType.Cylinder,p,new Vector3(.08f,.62f,.08f),c);
    }

    static void AddStandard(Transform parent,Vector3 p,Color cloth,Color trim)
    {
        Part(parent,"Standard Pole",PrimitiveType.Cylinder,p,new Vector3(.025f,.78f,.025f),new Color(.28f,.17f,.08f));
        GameObject banner=Part(parent,"Trojan Banner",PrimitiveType.Cube,p+new Vector3(.12f,.42f,0f),new Vector3(.24f,.38f,.04f),cloth);
        banner.AddComponent<ChapterOneAmbientMotion>().kind=ChapterOneAmbientMotion.MotionKind.Banner;
        Part(parent,"Banner Trim",PrimitiveType.Cube,p+new Vector3(.12f,.68f,0f),new Vector3(.26f,.04f,.05f),trim);
    }

    static GameObject Part(Transform parent,string name,PrimitiveType type,Vector3 localPos,Vector3 localScale,Color color,Quaternion? localRot=null)
    {
        GameObject go=GameObject.CreatePrimitive(type);
        go.name=name;
        go.transform.SetParent(parent,false);
        go.transform.localPosition=localPos;
        go.transform.localScale=localScale;
        if(localRot.HasValue) go.transform.localRotation=localRot.Value;
        Object.Destroy(go.GetComponent<Collider>());
        TowerFactory.SetColor(go,color);
        return go;
    }
}
