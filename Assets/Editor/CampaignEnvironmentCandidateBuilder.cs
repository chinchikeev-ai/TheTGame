using System.IO;
using UnityEditor;
using UnityEngine;

public static class CampaignEnvironmentCandidateBuilder
{
    const string Root = "Assets/Game/Art/Environment/Resources/TroyProduction/Environment";
    const string Chapter02Root = Root + "/Chapter02_Plains";
    const string Chapter03Root = Root + "/Chapter03_Siege";
    const string Chapter04Root = Root + "/Chapter04_DamagedDefenses";
    const string Chapter05Root = Root + "/Chapter05_GreatAssault";
    const string Chapter06Root = Root + "/Chapter06_GreekCamp";
    const string Chapter07Root = Root + "/Chapter07_TroyInterior";

    [MenuItem("The Troy Game/Characters/Build Campaign Environment Candidates")]
    public static void BuildAll()
    {
        EnsureFolder(Chapter02Root);
        EnsureFolder(Chapter03Root);
        EnsureFolder(Chapter04Root);
        EnsureFolder(Chapter05Root);
        EnsureFolder(Chapter06Root);
        EnsureFolder(Chapter07Root);

        BuildPlainsRoadSegment();
        BuildPlainsJunction();
        BuildChariotRoadsideSet();
        BuildSiegeWallBreach();
        BuildSiegeStagingSet();
        BuildDamagedOuterDefense();
        BuildDestroyedBuildNode();
        BuildGreatAssaultWallState();
        BuildAbandonedGreekCamp();
        BuildTrojanHorsePlaza();
        BuildTroyInteriorHouse();
        BuildTroyStreetModule();
        BuildBurningHouse();
        BuildCollapsedHouse();
        BuildEvacuationStreet();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Built modular campaign environment candidates for Chapters II-VII.");
    }

    static void BuildPlainsRoadSegment()
    {
        GameObject root = New("Env_PlainsRoadSegment");
        Color earth = new Color(.42f,.32f,.20f);
        Color dryGrass = new Color(.45f,.46f,.24f);
        Color green = new Color(.30f,.38f,.20f);
        Part(root.transform,"Ground",PrimitiveType.Cube,new Vector3(0f,-.08f,0f),new Vector3(6.0f,.16f,4.0f),dryGrass);
        Part(root.transform,"Worn Road",PrimitiveType.Cube,new Vector3(0f,.015f,0f),new Vector3(6.0f,.045f,1.25f),earth);
        for(int i=-4;i<=4;i++)
        {
            float x=i*.64f;
            Part(root.transform,"Wheel Rut",PrimitiveType.Cube,new Vector3(x,.045f,-.30f),new Vector3(.44f,.025f,.08f),earth*.78f);
            Part(root.transform,"Wheel Rut",PrimitiveType.Cube,new Vector3(x,.045f,.30f),new Vector3(.44f,.025f,.08f),earth*.78f);
        }
        AddGrass(root.transform,new Vector3(-2.2f,.10f,-1.20f),green,1.0f);
        AddGrass(root.transform,new Vector3(2.0f,.10f,1.30f),green*.92f,.9f);
        AddRockCluster(root.transform,new Vector3(-1.3f,.04f,1.42f),new Color(.45f,.40f,.31f),.65f);
        Save(root,Chapter02Root);
    }

    static void BuildPlainsJunction()
    {
        GameObject root=New("Env_PlainsRoadJunction");
        Color grass=new Color(.42f,.45f,.24f); Color earth=new Color(.42f,.31f,.19f);
        Part(root.transform,"Ground",PrimitiveType.Cube,new Vector3(0f,-.08f,0f),new Vector3(6.0f,.16f,6.0f),grass);
        Part(root.transform,"Main Road",PrimitiveType.Cube,new Vector3(0f,.015f,0f),new Vector3(6.0f,.045f,1.2f),earth);
        GameObject branch=Part(root.transform,"Branch Road",PrimitiveType.Cube,new Vector3(.8f,.025f,1.05f),new Vector3(3.9f,.05f,1.10f),earth*.96f,Quaternion.Euler(0f,-34f,0f));
        branch.name="Branch Road";
        AddRoadMarker(root.transform,new Vector3(-1.75f,.28f,1.55f));
        AddRockCluster(root.transform,new Vector3(2.3f,.04f,-1.7f),new Color(.46f,.40f,.31f),.8f);
        AddGrass(root.transform,new Vector3(-2.0f,.10f,-1.55f),new Color(.28f,.36f,.19f),1.15f);
        Save(root,Chapter02Root);
    }

    static void BuildChariotRoadsideSet()
    {
        GameObject root=New("Env_ChariotRoadsideSet");
        Color stone=new Color(.55f,.48f,.34f); Color wood=new Color(.31f,.18f,.08f);
        Part(root.transform,"Milestone",PrimitiveType.Cylinder,new Vector3(-.85f,.55f,0f),new Vector3(.25f,.55f,.25f),stone);
        Part(root.transform,"Broken Cart Axle",PrimitiveType.Cylinder,new Vector3(.32f,.24f,.20f),new Vector3(.07f,.70f,.07f),wood,Quaternion.Euler(90f,0f,18f));
        Part(root.transform,"Road Wheel",PrimitiveType.Cylinder,new Vector3(.82f,.35f,-.10f),new Vector3(.36f,.08f,.36f),wood,Quaternion.Euler(90f,0f,0f));
        Part(root.transform,"Bronze Hub",PrimitiveType.Cylinder,new Vector3(.82f,.35f,-.19f),new Vector3(.11f,.10f,.11f),new Color(.65f,.44f,.17f),Quaternion.Euler(90f,0f,0f));
        AddGrass(root.transform,new Vector3(.18f,.10f,-.72f),new Color(.30f,.37f,.18f),.8f);
        Save(root,Chapter02Root);
    }

    static void BuildSiegeWallBreach()
    {
        GameObject root=New("Env_SiegeWallBreach");
        Color stone=new Color(.60f,.50f,.32f); Color dark=new Color(.39f,.33f,.24f);
        Part(root.transform,"Wall Left",PrimitiveType.Cube,new Vector3(0f,1.55f,-2.15f),new Vector3(1.05f,3.10f,2.10f),stone);
        Part(root.transform,"Wall Right",PrimitiveType.Cube,new Vector3(0f,1.55f,2.15f),new Vector3(1.05f,3.10f,2.10f),stone);
        Part(root.transform,"Broken Crown L",PrimitiveType.Cube,new Vector3(-.08f,2.95f,-.80f),new Vector3(1.18f,.65f,.72f),stone*.90f,Quaternion.Euler(0f,0f,-8f));
        Part(root.transform,"Broken Crown R",PrimitiveType.Cube,new Vector3(-.06f,2.90f,.82f),new Vector3(1.15f,.52f,.65f),stone*.86f,Quaternion.Euler(0f,0f,11f));
        AddRubble(root.transform,new Vector3(-.45f,.10f,0f),dark,10,1.25f);
        Save(root,Chapter03Root);
    }

    static void BuildSiegeStagingSet()
    {
        GameObject root=New("Env_SiegeStagingSet");
        Color wood=new Color(.30f,.17f,.07f); Color bronze=new Color(.64f,.43f,.16f); Color hide=new Color(.46f,.34f,.22f);
        Part(root.transform,"Ladder",PrimitiveType.Cube,new Vector3(-.60f,.76f,0f),new Vector3(.10f,1.50f,.12f),wood,Quaternion.Euler(0f,0f,-18f));
        Part(root.transform,"Ladder",PrimitiveType.Cube,new Vector3(-.18f,.76f,0f),new Vector3(.10f,1.50f,.12f),wood,Quaternion.Euler(0f,0f,-18f));
        for(int i=0;i<5;i++) Part(root.transform,"Ladder Rung",PrimitiveType.Cube,new Vector3(-.39f,.16f+i*.30f,0f),new Vector3(.45f,.06f,.10f),wood,Quaternion.Euler(0f,0f,-18f));
        Part(root.transform,"Siege Shield",PrimitiveType.Cube,new Vector3(.72f,.62f,.15f),new Vector3(.70f,1.12f,.12f),hide,Quaternion.Euler(0f,-10f,0f));
        Part(root.transform,"Tool Bundle",PrimitiveType.Cylinder,new Vector3(.15f,.28f,-.45f),new Vector3(.10f,.38f,.10f),bronze,Quaternion.Euler(8f,0f,14f));
        Save(root,Chapter03Root);
    }

    static void BuildDamagedOuterDefense()
    {
        GameObject root=New("Env_DamagedOuterDefense");
        Color stone=new Color(.52f,.43f,.29f); Color dark=new Color(.30f,.25f,.20f); Color charred=new Color(.13f,.10f,.08f);
        Part(root.transform,"Broken Wall",PrimitiveType.Cube,new Vector3(0f,.92f,0f),new Vector3(1.30f,1.84f,3.25f),stone);
        Part(root.transform,"Missing Corner",PrimitiveType.Cube,new Vector3(-.72f,1.58f,1.12f),new Vector3(.30f,.62f,.72f),dark,Quaternion.Euler(0f,0f,18f));
        AddRubble(root.transform,new Vector3(-.80f,.08f,1.55f),dark,8,.85f);
        Part(root.transform,"Charred Beam",PrimitiveType.Cube,new Vector3(-.85f,.78f,-1.35f),new Vector3(.18f,1.55f,.18f),charred,Quaternion.Euler(0f,0f,-32f));
        AddFire(root.transform,new Vector3(-.75f,.28f,-1.28f));
        Save(root,Chapter04Root);
    }

    static void BuildDestroyedBuildNode()
    {
        GameObject root=New("Env_DestroyedBuildNode");
        Color stone=new Color(.46f,.39f,.28f); Color bronze=new Color(.58f,.39f,.15f);
        Part(root.transform,"Broken Platform A",PrimitiveType.Cube,new Vector3(-.35f,.10f,0f),new Vector3(.72f,.20f,1.20f),stone,Quaternion.Euler(0f,6f,-5f));
        Part(root.transform,"Broken Platform B",PrimitiveType.Cube,new Vector3(.40f,.06f,.16f),new Vector3(.62f,.16f,.92f),stone*.85f,Quaternion.Euler(0f,-12f,7f));
        AddRubble(root.transform,new Vector3(.12f,.05f,-.65f),stone*.72f,7,.52f);
        Part(root.transform,"Broken Bronze Marker",PrimitiveType.Cube,new Vector3(-.10f,.26f,.62f),new Vector3(.10f,.45f,.10f),bronze,Quaternion.Euler(0f,0f,24f));
        Save(root,Chapter05Root);
    }

    static void BuildGreatAssaultWallState()
    {
        GameObject root=New("Env_GreatAssaultWallState");
        Color stone=new Color(.49f,.41f,.28f); Color dark=new Color(.28f,.23f,.18f);
        Part(root.transform,"Wall Standing",PrimitiveType.Cube,new Vector3(0f,1.10f,-1.90f),new Vector3(1.10f,2.20f,2.15f),stone);
        Part(root.transform,"Wall Collapsed Slab",PrimitiveType.Cube,new Vector3(-.52f,.40f,1.00f),new Vector3(1.05f,.72f,2.20f),stone*.82f,Quaternion.Euler(0f,8f,24f));
        AddRubble(root.transform,new Vector3(-.72f,.08f,2.10f),dark,12,1.05f);
        AddFire(root.transform,new Vector3(-.95f,.22f,.72f));
        AddFire(root.transform,new Vector3(-.30f,.18f,2.35f));
        Save(root,Chapter05Root);
    }

    static void BuildAbandonedGreekCamp()
    {
        GameObject root=New("Env_AbandonedGreekCamp");
        Color cloth=new Color(.66f,.61f,.49f); Color blue=new Color(.28f,.37f,.52f); Color wood=new Color(.30f,.17f,.07f);
        AddTent(root.transform,new Vector3(-1.25f,0f,-.55f),cloth,blue,1.0f);
        AddTent(root.transform,new Vector3(1.15f,0f,.65f),cloth*.90f,blue*.85f,.85f);
        Part(root.transform,"Abandoned Crate",PrimitiveType.Cube,new Vector3(.15f,.24f,-1.20f),new Vector3(.48f,.48f,.48f),wood);
        Part(root.transform,"Weapon Rack",PrimitiveType.Cube,new Vector3(1.30f,.60f,-1.10f),new Vector3(.12f,1.08f,.65f),wood);
        for(int i=0;i<4;i++) Part(root.transform,"Abandoned Spear",PrimitiveType.Cylinder,new Vector3(1.22f+i*.08f,.82f,-1.10f),new Vector3(.018f,.74f,.018f),new Color(.55f,.37f,.14f),Quaternion.Euler(0f,0f,-7f+i*4f));
        Part(root.transform,"Cold Fire Ring",PrimitiveType.Cylinder,new Vector3(-.10f,.06f,.70f),new Vector3(.42f,.07f,.42f),new Color(.32f,.28f,.22f));
        Save(root,Chapter06Root);
    }

    static void BuildTrojanHorsePlaza()
    {
        GameObject root=New("Env_TrojanHorsePlaza");
        Color stone=new Color(.58f,.49f,.33f); Color red=new Color(.52f,.06f,.04f); Color bronze=new Color(.67f,.45f,.17f);
        Part(root.transform,"Plaza",PrimitiveType.Cylinder,new Vector3(0f,.04f,0f),new Vector3(2.70f,.08f,2.70f),stone);
        for(int i=0;i<4;i++)
        {
            float a=i*Mathf.PI*.5f;
            Vector3 p=new Vector3(Mathf.Cos(a)*2.15f,.72f,Mathf.Sin(a)*2.15f);
            Part(root.transform,"Ceremonial Pole",PrimitiveType.Cylinder,p,new Vector3(.035f,.70f,.035f),new Color(.29f,.17f,.08f));
            Part(root.transform,"Trojan Cloth",PrimitiveType.Cube,p+new Vector3(.14f,.25f,0f),new Vector3(.28f,.38f,.035f),red);
            Part(root.transform,"Bronze Trim",PrimitiveType.Cube,p+new Vector3(.14f,.48f,0f),new Vector3(.30f,.045f,.04f),bronze);
        }
        Save(root,Chapter06Root);
    }

    static void BuildTroyInteriorHouse()
    {
        GameObject root=New("Env_TroyInteriorHouse");
        Color plaster=new Color(.68f,.56f,.38f); Color stone=new Color(.48f,.40f,.29f); Color roof=new Color(.48f,.17f,.09f); Color wood=new Color(.29f,.16f,.07f);
        Part(root.transform,"House Core",PrimitiveType.Cube,new Vector3(0f,1.20f,0f),new Vector3(2.20f,2.40f,2.00f),plaster);
        Part(root.transform,"Stone Base",PrimitiveType.Cube,new Vector3(0f,.24f,0f),new Vector3(2.34f,.48f,2.12f),stone);
        Part(root.transform,"Door",PrimitiveType.Cube,new Vector3(-1.13f,.72f,0f),new Vector3(.10f,1.18f,.66f),wood);
        Part(root.transform,"Window",PrimitiveType.Cube,new Vector3(-1.14f,1.55f,-.55f),new Vector3(.06f,.38f,.38f),new Color(.20f,.18f,.15f));
        Part(root.transform,"Window",PrimitiveType.Cube,new Vector3(-1.14f,1.55f,.55f),new Vector3(.06f,.38f,.38f),new Color(.20f,.18f,.15f));
        GameObject roofObj=Part(root.transform,"Roof",PrimitiveType.Cylinder,new Vector3(0f,2.62f,0f),new Vector3(1.45f,.28f,1.35f),roof);
        roofObj.transform.localRotation=Quaternion.Euler(0f,30f,0f);
        Save(root,Chapter07Root);
    }

    static void BuildTroyStreetModule()
    {
        GameObject root=New("Env_TroyStreetModule");
        Color stone=new Color(.55f,.47f,.34f); Color dark=new Color(.40f,.34f,.26f); Color red=new Color(.50f,.06f,.04f);
        Part(root.transform,"Street",PrimitiveType.Cube,new Vector3(0f,-.04f,0f),new Vector3(6.0f,.08f,2.10f),stone);
        for(int i=-5;i<=5;i++) Part(root.transform,"Paving Stone",PrimitiveType.Cube,new Vector3(i*.52f,.025f,(i%2==0?-.45f:.45f)),new Vector3(.42f,.05f,.58f),dark*.94f,Quaternion.Euler(0f,(i%3-1)*5f,0f));
        Part(root.transform,"Street Brazier",PrimitiveType.Cylinder,new Vector3(-2.05f,.35f,.82f),new Vector3(.25f,.35f,.25f),dark);
        AddFire(root.transform,new Vector3(-2.05f,.72f,.82f));
        Part(root.transform,"Street Banner",PrimitiveType.Cube,new Vector3(2.10f,.92f,-.92f),new Vector3(.05f,.52f,.34f),red);
        Save(root,Chapter07Root);
    }

    static void BuildBurningHouse()
    {
        GameObject root=New("Env_BurningTroyHouse");
        Color plaster=new Color(.48f,.34f,.24f); Color charred=new Color(.12f,.09f,.07f); Color roof=new Color(.28f,.10f,.06f);
        Part(root.transform,"Burning House Core",PrimitiveType.Cube,new Vector3(0f,1.05f,0f),new Vector3(2.10f,2.10f,1.90f),plaster);
        Part(root.transform,"Charred Door",PrimitiveType.Cube,new Vector3(-1.08f,.65f,0f),new Vector3(.10f,1.05f,.64f),charred);
        GameObject roofObj=Part(root.transform,"Burning Roof",PrimitiveType.Cylinder,new Vector3(0f,2.28f,0f),new Vector3(1.35f,.25f,1.25f),roof);
        roofObj.transform.localRotation=Quaternion.Euler(0f,30f,0f);
        AddFire(root.transform,new Vector3(-.55f,2.05f,-.40f));
        AddFire(root.transform,new Vector3(.48f,2.28f,.35f));
        AddFire(root.transform,new Vector3(-.98f,.82f,.45f));
        Save(root,Chapter07Root);
    }

    static void BuildCollapsedHouse()
    {
        GameObject root=New("Env_CollapsedTroyHouse");
        Color stone=new Color(.48f,.39f,.28f); Color plaster=new Color(.59f,.45f,.31f); Color wood=new Color(.20f,.12f,.06f);
        Part(root.transform,"Standing Corner",PrimitiveType.Cube,new Vector3(.80f,.92f,-.65f),new Vector3(.72f,1.84f,.72f),plaster,Quaternion.Euler(0f,-5f,-4f));
        Part(root.transform,"Collapsed Slab",PrimitiveType.Cube,new Vector3(-.35f,.28f,.25f),new Vector3(1.70f,.48f,1.42f),plaster*.82f,Quaternion.Euler(0f,14f,22f));
        Part(root.transform,"Broken Beam",PrimitiveType.Cube,new Vector3(-.75f,.55f,-.52f),new Vector3(.14f,1.85f,.14f),wood,Quaternion.Euler(0f,20f,-54f));
        AddRubble(root.transform,new Vector3(-.55f,.06f,.92f),stone,12,.90f);
        Save(root,Chapter07Root);
    }

    static void BuildEvacuationStreet()
    {
        GameObject root=New("Env_EvacuationStreet");
        Color stone=new Color(.51f,.43f,.31f); Color wood=new Color(.26f,.15f,.07f); Color red=new Color(.51f,.06f,.04f);
        Part(root.transform,"Evacuation Lane",PrimitiveType.Cube,new Vector3(0f,-.03f,0f),new Vector3(6.0f,.06f,1.70f),stone);
        Part(root.transform,"Cart",PrimitiveType.Cube,new Vector3(-1.10f,.38f,.70f),new Vector3(.92f,.52f,.62f),wood);
        for(int side=-1;side<=1;side+=2) Part(root.transform,"Cart Wheel",PrimitiveType.Cylinder,new Vector3(-1.10f,.28f,.70f+side*.38f),new Vector3(.28f,.07f,.28f),wood,Quaternion.Euler(90f,0f,0f));
        Part(root.transform,"Evacuation Standard",PrimitiveType.Cylinder,new Vector3(1.65f,.72f,-.72f),new Vector3(.03f,.72f,.03f),wood);
        Part(root.transform,"Evacuation Cloth",PrimitiveType.Cube,new Vector3(1.82f,1.10f,-.72f),new Vector3(.34f,.30f,.04f),red);
        AddRubble(root.transform,new Vector3(2.42f,.04f,.50f),stone*.75f,5,.48f);
        Save(root,Chapter07Root);
    }

    static GameObject New(string name)
    {
        GameObject root=new GameObject(name);
        root.transform.position=Vector3.zero;
        return root;
    }

    static void AddTent(Transform parent,Vector3 position,Color cloth,Color accent,float scale)
    {
        GameObject tent=new GameObject("Greek Tent");
        tent.transform.SetParent(parent,false);
        tent.transform.localPosition=position;
        tent.transform.localScale=Vector3.one*scale;
        Part(tent.transform,"Tent Body",PrimitiveType.Cube,new Vector3(0f,.48f,0f),new Vector3(1.30f,.88f,1.10f),cloth,Quaternion.Euler(0f,0f,0f));
        Part(tent.transform,"Tent Ridge",PrimitiveType.Cube,new Vector3(0f,1.00f,0f),new Vector3(1.38f,.16f,.20f),accent);
        Part(tent.transform,"Tent Pole",PrimitiveType.Cylinder,new Vector3(-.68f,.55f,0f),new Vector3(.03f,.56f,.03f),new Color(.31f,.18f,.08f));
        Part(tent.transform,"Tent Pole",PrimitiveType.Cylinder,new Vector3(.68f,.55f,0f),new Vector3(.03f,.56f,.03f),new Color(.31f,.18f,.08f));
    }

    static void AddRubble(Transform parent,Vector3 center,Color color,int count,float radius)
    {
        for(int i=0;i<count;i++)
        {
            float a=i*2.399963f;
            float r=radius*(.28f+.72f*((i%5)/4f));
            Vector3 p=center+new Vector3(Mathf.Cos(a)*r,.08f+(i%3)*.045f,Mathf.Sin(a)*r);
            Vector3 s=new Vector3(.22f+(i%3)*.09f,.14f+(i%2)*.08f,.25f+((i+1)%3)*.07f);
            Part(parent,"Rubble",PrimitiveType.Cube,p,s,color*(.84f+(i%4)*.04f),Quaternion.Euler(i*7f,i*23f,i*11f));
        }
    }

    static void AddFire(Transform parent,Vector3 position)
    {
        GameObject flame=Part(parent,"Fire",PrimitiveType.Sphere,position,new Vector3(.18f,.32f,.18f),new Color(1f,.31f,.035f));
        flame.AddComponent<ChapterOneAmbientMotion>().kind=ChapterOneAmbientMotion.MotionKind.Flame;
    }

    static void AddGrass(Transform parent,Vector3 position,Color color,float scale)
    {
        for(int i=0;i<5;i++)
        {
            GameObject blade=Part(parent,"Grass",PrimitiveType.Cube,position+new Vector3((i-2)*.07f,.10f,(i%2)*.07f),new Vector3(.025f,.24f,.045f)*scale,color*(.90f+i*.02f));
            blade.transform.localRotation=Quaternion.Euler(0f,i*31f,(i-2)*8f);
        }
    }

    static void AddRockCluster(Transform parent,Vector3 position,Color color,float scale)
    {
        for(int i=0;i<4;i++)
        {
            float a=i*Mathf.PI*.5f;
            Part(parent,"Road Rock",PrimitiveType.Sphere,position+new Vector3(Mathf.Cos(a)*.28f,0f,Mathf.Sin(a)*.22f)*scale,new Vector3(.22f,.14f,.28f)*scale,color*(.90f+i*.03f));
        }
    }

    static void AddRoadMarker(Transform parent,Vector3 position)
    {
        Color stone=new Color(.58f,.50f,.36f); Color bronze=new Color(.65f,.44f,.17f);
        Part(parent,"Road Marker",PrimitiveType.Cylinder,position,new Vector3(.20f,.42f,.20f),stone);
        Part(parent,"Road Marker Band",PrimitiveType.Cylinder,position+new Vector3(0f,.40f,0f),new Vector3(.22f,.055f,.22f),bronze);
    }

    static GameObject Part(Transform parent,string name,PrimitiveType type,Vector3 position,Vector3 scale,Color color,Quaternion? rotation=null)
    {
        GameObject go=GameObject.CreatePrimitive(type);
        go.name=name;
        go.transform.SetParent(parent,false);
        go.transform.localPosition=position;
        go.transform.localScale=scale;
        if(rotation.HasValue) go.transform.localRotation=rotation.Value;
        Collider collider=go.GetComponent<Collider>(); if(collider!=null) Object.DestroyImmediate(collider);
        TowerFactory.SetColor(go,color);
        return go;
    }

    static void Save(GameObject root,string folder)
    {
        EnsureFolder(folder);
        string path=Path.Combine(folder,root.name+".prefab").Replace('\\','/');
        PrefabUtility.SaveAsPrefabAsset(root,path);
        Object.DestroyImmediate(root);
    }

    static void EnsureFolder(string path)
    {
        if(AssetDatabase.IsValidFolder(path)) return;
        string parent=Path.GetDirectoryName(path).Replace('\\','/');
        string name=Path.GetFileName(path);
        if(!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent,name);
    }
}
