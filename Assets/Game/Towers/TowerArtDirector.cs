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
        Color brightBronze = new Color(.82f,.57f,.20f);
        Color red = new Color(.56f,.08f,.055f);
        Color darkRed = new Color(.34f,.035f,.025f);
        Color stone = new Color(.48f,.40f,.28f);
        Color cream = new Color(.86f,.78f,.60f);
        Color fire = new Color(1f,.32f,.04f);

        Part(art.transform,"Tower Base",PrimitiveType.Cylinder,Vector3.zero,new Vector3(.78f,.28f,.78f),stone*.72f);

        switch (type)
        {
            case TowerType.MachineGun:
                AddPost(art.transform,new Vector3(-.44f,.64f,-.31f),wood);
                AddPost(art.transform,new Vector3(.44f,.64f,-.31f),wood);
                AddPost(art.transform,new Vector3(-.44f,.64f,.31f),wood);
                AddPost(art.transform,new Vector3(.44f,.64f,.31f),wood);
                Part(art.transform,"Wooden Platform",PrimitiveType.Cube,new Vector3(0f,.96f,0f),new Vector3(1.12f,.12f,.96f),lightWood);
                Part(art.transform,"Archer Parapet",PrimitiveType.Cube,new Vector3(0f,1.15f,.47f),new Vector3(1.08f,.30f,.11f),wood);
                Part(art.transform,"Archer Parapet Left",PrimitiveType.Cube,new Vector3(-.52f,1.15f,.05f),new Vector3(.11f,.30f,.78f),wood);
                Part(art.transform,"Archer Parapet Right",PrimitiveType.Cube,new Vector3(.52f,1.15f,.05f),new Vector3(.11f,.30f,.78f),wood);
                Part(art.transform,"Role_Archer_RedCanopy",PrimitiveType.Cylinder,new Vector3(0f,1.55f,0f),new Vector3(.88f,.18f,.88f),red).transform.rotation=Quaternion.Euler(0f,30f,0f);
                Part(art.transform,"Role_Archer_ArrowBundle",PrimitiveType.Cylinder,new Vector3(.56f,1.20f,.18f),new Vector3(.13f,.50f,.13f),wood).transform.rotation=Quaternion.Euler(10f,0f,-12f);
                Part(art.transform,"Arrow Crate",PrimitiveType.Cube,new Vector3(-.45f,1.10f,-.28f),new Vector3(.34f,.20f,.29f),lightWood);
                Part(art.transform,"Bronze Archer Emblem",PrimitiveType.Cylinder,new Vector3(0f,1.21f,.54f),new Vector3(.16f,.025f,.16f),bronze,Quaternion.Euler(90f,0f,0f));
                AddArcherRoleBow(art.transform,bronze,brightBronze);
                Part(head,"Archer Aimed Bow",PrimitiveType.Cube,new Vector3(0f,.18f,.68f),new Vector3(.92f,.065f,.085f),bronze);
                AddProductionCrew(head,"Trojan_Archer",new Vector3(-.25f,.24f,-.02f),.64f,8f);
                AddProductionCrew(head,"Trojan_Archer",new Vector3(.25f,.24f,-.10f),.64f,-8f);
                break;

            case TowerType.Cannon:
                Part(art.transform,"Ballista Deck",PrimitiveType.Cube,new Vector3(0f,.55f,0f),new Vector3(1.28f,.16f,1.08f),wood);
                for (int s=-1;s<=1;s+=2)
                    Part(art.transform,"Ballista Support",PrimitiveType.Cube,new Vector3(s*.52f,.42f,0f),new Vector3(.14f,.66f,.16f),lightWood);
                Part(head,"Counterweight",PrimitiveType.Cube,new Vector3(0f,-.34f,-.40f),new Vector3(.43f,.43f,.43f),stone);
                Part(head,"Role_Ballista_Rail",PrimitiveType.Cube,new Vector3(0f,.36f,.22f),new Vector3(.19f,.14f,1.38f),lightWood);
                Part(head,"Role_Ballista_MainBow",PrimitiveType.Cube,new Vector3(0f,.37f,.86f),new Vector3(1.58f,.10f,.12f),bronze);
                Part(head,"Role_Ballista_LeftArm",PrimitiveType.Cube,new Vector3(-.63f,.39f,.63f),new Vector3(.12f,.10f,.72f),wood,Quaternion.Euler(0f,-28f,0f));
                Part(head,"Role_Ballista_RightArm",PrimitiveType.Cube,new Vector3(.63f,.39f,.63f),new Vector3(.12f,.10f,.72f),wood,Quaternion.Euler(0f,28f,0f));
                Part(head,"Role_Ballista_Bolt",PrimitiveType.Cylinder,new Vector3(0f,.39f,1.01f),new Vector3(.045f,.82f,.045f),brightBronze,Quaternion.Euler(90f,0f,0f));
                Part(art.transform,"Ballista Bolt Crate",PrimitiveType.Cube,new Vector3(.53f,.72f,-.32f),new Vector3(.38f,.20f,.55f),lightWood);
                AddProductionCrew(art.transform,"Trojan_Infantry",new Vector3(-.62f,.35f,-.42f),.60f,18f);
                break;

            case TowerType.Slow:
                for (int i=0;i<4;i++)
                {
                    float a=i*Mathf.PI*.5f;
                    Part(art.transform,"Shrine Column",PrimitiveType.Cylinder,new Vector3(Mathf.Cos(a)*.48f,.60f,Mathf.Sin(a)*.48f),new Vector3(.10f,.58f,.10f),cream);
                }
                Part(art.transform,"Role_Priest_ShrineRoof",PrimitiveType.Cylinder,new Vector3(0f,1.22f,0f),new Vector3(.78f,.12f,.78f),bronze).transform.rotation=Quaternion.Euler(0f,30f,0f);
                Part(art.transform,"Role_Priest_ApolloDisc",PrimitiveType.Cylinder,new Vector3(0f,1.62f,.02f),new Vector3(.42f,.055f,.42f),new Color(.96f,.74f,.20f)).transform.rotation=Quaternion.Euler(90f,0f,0f);
                AddSunRays(art.transform,new Vector3(0f,1.62f,.02f),brightBronze);
                Part(art.transform,"Role_Priest_SunStaff",PrimitiveType.Cylinder,new Vector3(.58f,.93f,-.12f),new Vector3(.035f,.80f,.035f),brightBronze);
                Part(art.transform,"Role_Priest_StaffDisc",PrimitiveType.Cylinder,new Vector3(.58f,1.68f,-.12f),new Vector3(.22f,.035f,.22f),new Color(.98f,.78f,.24f),Quaternion.Euler(90f,0f,0f));
                Part(head,"Apollo Focus",PrimitiveType.Sphere,new Vector3(0f,.38f,.50f),Vector3.one*.25f,new Color(.98f,.72f,.18f));
                AddPriest(art.transform,new Vector3(-.24f,.35f,.08f),-8f);
                AddPriest(art.transform,new Vector3(.24f,.35f,-.02f),8f);
                break;

            case TowerType.SpearThrower:
                Part(art.transform,"Spear Wall Beam",PrimitiveType.Cube,new Vector3(0f,.48f,.48f),new Vector3(1.18f,.16f,.14f),wood);
                for (int s=-1;s<=1;s++)
                {
                    float x=s*.36f;
                    Part(art.transform,"Role_SpearWall_Shield",PrimitiveType.Cylinder,new Vector3(x,.68f,.58f),new Vector3(.32f,.060f,.42f),s==0?red:bronze,Quaternion.Euler(90f,0f,0f));
                    Part(art.transform,"Role_SpearWall_ForwardSpear",PrimitiveType.Cylinder,new Vector3(x,.76f,1.02f),new Vector3(.033f,.88f,.033f),brightBronze,Quaternion.Euler(90f,0f,0f));
                }
                for (int i=-2;i<=2;i++)
                {
                    GameObject spear=Part(art.transform,"Role_SpearWall_UprightSpear",PrimitiveType.Cylinder,new Vector3(i*.20f,1.02f,-.28f+Mathf.Abs(i)*.035f),new Vector3(.028f,.90f,.028f),bronze);
                    spear.transform.rotation=Quaternion.Euler(0f,0f,i*3f);
                }
                Part(art.transform,"Spear Rack",PrimitiveType.Cube,new Vector3(-.58f,.58f,-.05f),new Vector3(.18f,.86f,.44f),wood);
                Part(art.transform,"Role_SpearWall_RedCrest",PrimitiveType.Cube,new Vector3(.34f,1.28f,-.04f),new Vector3(.15f,.42f,.48f),red);
                Part(head,"Aimed Spear",PrimitiveType.Cylinder,new Vector3(0f,.20f,.88f),new Vector3(.040f,.84f,.040f),brightBronze,Quaternion.Euler(90f,0f,0f));
                AddProductionCrew(art.transform,"Trojan_Infantry",new Vector3(-.22f,.30f,-.10f),.60f,5f);
                AddProductionCrew(art.transform,"Trojan_Infantry",new Vector3(.30f,.30f,-.12f),.60f,-5f);
                break;

            case TowerType.FireTower:
                Part(art.transform,"Fire Stone Base",PrimitiveType.Cylinder,new Vector3(0f,.30f,0f),new Vector3(.76f,.27f,.76f),stone);
                Part(art.transform,"Role_Fire_Bowl",PrimitiveType.Cylinder,new Vector3(0f,.90f,0f),new Vector3(.54f,.15f,.54f),bronze);
                AddFlame(art.transform,"Role_Fire_MainFlame",new Vector3(0f,1.28f,0f),new Vector3(.38f,.70f,.38f),fire);
                AddFlame(art.transform,"Role_Fire_LeftFlame",new Vector3(-.22f,1.20f,.05f),new Vector3(.18f,.46f,.18f),new Color(1f,.55f,.05f));
                AddFlame(art.transform,"Role_Fire_RightFlame",new Vector3(.22f,1.18f,-.04f),new Vector3(.17f,.42f,.17f),new Color(1f,.42f,.03f));
                Part(art.transform,"Role_Fire_PitchRack",PrimitiveType.Cube,new Vector3(-.50f,.60f,-.24f),new Vector3(.28f,.54f,.38f),wood);
                for (int i=0;i<3;i++)
                    Part(art.transform,"Role_Fire_PitchBottle",PrimitiveType.Cylinder,new Vector3(-.50f+i*.10f,.88f,-.22f+i*.04f),new Vector3(.065f,.19f,.065f),darkRed,Quaternion.Euler(i*5f,0f,-8f+i*6f));
                AddFireKeeper(art.transform,new Vector3(.56f,.26f,-.20f));
                break;

            case TowerType.TrojanGuard:
                Part(art.transform,"Guard Platform",PrimitiveType.Cube,new Vector3(0f,.20f,0f),new Vector3(1.20f,.20f,1.05f),stone);
                Part(art.transform,"Guard Bronze Rail",PrimitiveType.Cube,new Vector3(0f,.60f,.50f),new Vector3(1.16f,.10f,.10f),bronze);
                for (int s=-1;s<=1;s++)
                {
                    float x=s*.36f;
                    GameObject shield=Part(art.transform,"Role_Guard_OversizedShield",PrimitiveType.Cylinder,new Vector3(x,.78f,.60f),new Vector3(.40f,.070f,.54f),s==0?darkRed:bronze,Quaternion.Euler(90f,0f,0f));
                    Part(shield.transform,"Role_Guard_ShieldBoss",PrimitiveType.Sphere,new Vector3(0f,.08f,0f),new Vector3(.18f,.10f,.18f),brightBronze);
                }
                AddStandard(art.transform,new Vector3(-.58f,1.10f,-.20f),red,bronze);
                AddStandard(art.transform,new Vector3(.58f,1.10f,-.20f),red,bronze);
                Part(art.transform,"Shield Rack",PrimitiveType.Cube,new Vector3(0f,.68f,-.46f),new Vector3(.80f,.60f,.11f),darkRed);
                AddProductionCrew(art.transform,"Trojan_Guard",new Vector3(-.27f,.26f,-.08f),.68f,5f);
                AddProductionCrew(art.transform,"Trojan_Guard",new Vector3(.27f,.26f,-.08f),.68f,-5f);
                break;
        }
    }

    static void AddArcherRoleBow(Transform parent,Color bronze,Color brightBronze)
    {
        Transform bow=new GameObject("Role_Archer_GiantBow").transform;
        bow.SetParent(parent,false);
        bow.localPosition=new Vector3(0f,1.10f,.60f);
        GameObject left=Part(bow,"Role_Archer_BowLeft",PrimitiveType.Cylinder,new Vector3(-.33f,.18f,0f),new Vector3(.035f,.46f,.035f),bronze,Quaternion.Euler(0f,0f,-32f));
        GameObject right=Part(bow,"Role_Archer_BowRight",PrimitiveType.Cylinder,new Vector3(.33f,.18f,0f),new Vector3(.035f,.46f,.035f),bronze,Quaternion.Euler(0f,0f,32f));
        Part(bow,"Role_Archer_BowString",PrimitiveType.Cylinder,new Vector3(0f,.08f,0f),new Vector3(.010f,.66f,.010f),brightBronze);
        left.transform.localPosition+=new Vector3(0f,.03f,0f);
        right.transform.localPosition+=new Vector3(0f,.03f,0f);
    }

    static void AddSunRays(Transform parent,Vector3 center,Color color)
    {
        for(int i=0;i<4;i++)
        {
            float angle=i*45f;
            Part(parent,"Role_Priest_SunRay",PrimitiveType.Cube,center,new Vector3(.06f,.52f,.055f),color,Quaternion.Euler(0f,0f,angle));
        }
    }

    static void AddFlame(Transform parent,string name,Vector3 position,Vector3 scale,Color color)
    {
        GameObject flame=Part(parent,name,PrimitiveType.Sphere,position,scale,color);
        flame.AddComponent<ChapterOneAmbientMotion>().kind=ChapterOneAmbientMotion.MotionKind.Flame;
    }

    static void AddProductionCrew(Transform parent,string prefabName,Vector3 localPosition,float scale,float yaw)
    {
        string source;
        string detail;
        GameObject prefab=Resources.Load<GameObject>(TrojanProductionRoot+prefabName);
        GameObject crew;

        if(prefab!=null)
        {
            crew=Object.Instantiate(prefab,parent);
            source="PRODUCTION_RESOURCE";
            detail=TrojanProductionRoot+prefabName;
        }
        else
        {
            prefab=Resources.Load<GameObject>(TrojanGeneratedRoot+prefabName);
            if(prefab!=null)
            {
                crew=Object.Instantiate(prefab,parent);
                source="GENERATED_RESOURCE";
                detail=TrojanGeneratedRoot+prefabName;
            }
            else
            {
                crew=TrojanTowerCrewFallbackFactory.Create(prefabName);
                crew.transform.SetParent(parent,false);
                source="PROCEDURAL_FALLBACK";
                detail="TrojanTowerCrewFallbackFactory";
            }
        }

        crew.name="TowerCrew_"+prefabName;
        crew.transform.localPosition=localPosition;
        crew.transform.localRotation=Quaternion.Euler(0f,yaw,0f);
        crew.transform.localScale=Vector3.one*scale;
        foreach(Collider collider in crew.GetComponentsInChildren<Collider>(true))
        {
            collider.enabled=false;
            Object.Destroy(collider);
        }
        RuntimeVisualAudit.Report("TowerCrew:"+prefabName,source,detail);
    }

    static void AddPriest(Transform parent,Vector3 localPosition,float yaw)
    {
        GameObject root=new GameObject("PriestOfApollo");
        root.transform.SetParent(parent,false);
        root.transform.localPosition=localPosition;
        root.transform.localRotation=Quaternion.Euler(0f,yaw,0f);
        Part(root.transform,"Robe",PrimitiveType.Cylinder,new Vector3(0f,.34f,0f),new Vector3(.18f,.34f,.18f),new Color(.88f,.78f,.58f));
        Part(root.transform,"Head",PrimitiveType.Sphere,new Vector3(0f,.78f,0f),Vector3.one*.17f,new Color(.72f,.52f,.34f));
        Part(root.transform,"SunStaff",PrimitiveType.Cylinder,new Vector3(.22f,.52f,.02f),new Vector3(.030f,.56f,.030f),new Color(.78f,.53f,.18f));
        Part(root.transform,"PriestSunDisc",PrimitiveType.Cylinder,new Vector3(.22f,1.06f,.02f),new Vector3(.13f,.025f,.13f),new Color(.96f,.76f,.24f),Quaternion.Euler(90f,0f,0f));
        RuntimeVisualAudit.Report("TowerCrew:Trojan_PriestApollo","PROCEDURAL_FALLBACK","TowerArtDirector.AddPriest");
    }

    static void AddFireKeeper(Transform parent,Vector3 localPosition)
    {
        GameObject root=new GameObject("FireKeeper");
        root.transform.SetParent(parent,false);
        root.transform.localPosition=localPosition;
        root.transform.localRotation=Quaternion.Euler(0f,-12f,0f);
        Part(root.transform,"Tunic",PrimitiveType.Cylinder,new Vector3(0f,.32f,0f),new Vector3(.18f,.32f,.17f),new Color(.48f,.09f,.045f));
        Part(root.transform,"Head",PrimitiveType.Sphere,new Vector3(.02f,.72f,.02f),Vector3.one*.17f,new Color(.72f,.50f,.32f));
        Part(root.transform,"PitchPot",PrimitiveType.Cylinder,new Vector3(-.24f,.28f,.10f),new Vector3(.13f,.21f,.13f),new Color(.17f,.11f,.07f));
        Part(root.transform,"RaisedFireBottle",PrimitiveType.Cylinder,new Vector3(.24f,.72f,.06f),new Vector3(.065f,.22f,.065f),new Color(.22f,.10f,.055f),Quaternion.Euler(0f,0f,-24f));
        RuntimeVisualAudit.Report("TowerCrew:Trojan_FireKeeper","PROCEDURAL_FALLBACK","TowerArtDirector.AddFireKeeper");
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
