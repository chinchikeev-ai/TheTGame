using UnityEngine;

public static class TowerFactory
{
    const string RuntimeMaterialResource = "RuntimeColorMaterial";
    static Material runtimeMaterialTemplate;

    public static int GetCost(TowerType type) => BalanceCatalog.GetTower(type).cost;
    public static string GetDisplayName(TowerType type) => BalanceCatalog.GetTower(type).displayName;

    public static GameObject CreateTower(Vector3 position, TowerType type = TowerType.MachineGun, string name = null)
    {
        TowerData data = BalanceCatalog.GetTower(type);
        GameObject root = new GameObject(name ?? data.displayName);
        root.transform.position = position;

        Color stone = new Color(.34f,.28f,.20f);
        Color bronze = new Color(.55f,.36f,.16f);
        Color darkWood = new Color(.24f,.14f,.07f);

        GameObject baseObj = Primitive(root.transform, "Base", PrimitiveType.Cylinder, Vector3.zero, new Vector3(.78f,.28f,.78f), stone);
        GameObject head = Primitive(root.transform, "Head", PrimitiveType.Cube, new Vector3(0f,.72f,0f), new Vector3(.58f,.34f,.70f), bronze);
        GameObject barrel = Primitive(head.transform, "Barrel", PrimitiveType.Cylinder, new Vector3(0f,0f,.82f), new Vector3(.10f,.48f,.10f), darkWood, Quaternion.Euler(90f,0f,0f));

        switch (type)
        {
            case TowerType.MachineGun:
                SetColor(head, new Color(.48f,.31f,.14f));
                Primitive(head.transform,"Bow",PrimitiveType.Cube,new Vector3(0f,.05f,.62f),new Vector3(.80f,.06f,.08f),new Color(.68f,.48f,.22f));
                Primitive(head.transform,"Roof",PrimitiveType.Cylinder,new Vector3(0f,.48f,0f),new Vector3(.58f,.10f,.58f),new Color(.60f,.22f,.12f));
                break;
            case TowerType.Cannon:
                head.transform.localScale = new Vector3(.72f,.42f,.72f);
                SetColor(head,new Color(.48f,.24f,.12f));
                barrel.transform.localScale = new Vector3(.18f,.66f,.18f);
                Primitive(head.transform,"BallistaArmL",PrimitiveType.Cube,new Vector3(-.45f,.05f,.34f),new Vector3(.65f,.08f,.10f),new Color(.72f,.48f,.18f),Quaternion.Euler(0f,18f,0f));
                Primitive(head.transform,"BallistaArmR",PrimitiveType.Cube,new Vector3(.45f,.05f,.34f),new Vector3(.65f,.08f,.10f),new Color(.72f,.48f,.18f),Quaternion.Euler(0f,-18f,0f));
                break;
            case TowerType.Slow:
                SetColor(head,new Color(.54f,.48f,.78f));
                SetColor(barrel,new Color(.72f,.68f,.92f));
                Primitive(head.transform,"ApolloCore",PrimitiveType.Sphere,new Vector3(0f,.55f,0f),Vector3.one*.34f,new Color(.98f,.72f,.18f));
                Primitive(root.transform,"ColumnL",PrimitiveType.Cylinder,new Vector3(-.34f,.45f,0f),new Vector3(.10f,.42f,.10f),new Color(.80f,.72f,.54f));
                Primitive(root.transform,"ColumnR",PrimitiveType.Cylinder,new Vector3(.34f,.45f,0f),new Vector3(.10f,.42f,.10f),new Color(.80f,.72f,.54f));
                break;
            case TowerType.SpearThrower:
                SetColor(head,new Color(.46f,.36f,.16f));
                for (int i=-1;i<=1;i++)
                    Primitive(head.transform,"Spear"+i,PrimitiveType.Cylinder,new Vector3(i*.18f,.10f,.70f),new Vector3(.035f,.68f,.035f),new Color(.72f,.55f,.25f),Quaternion.Euler(90f,0f,0f));
                break;
            case TowerType.FireTower:
                SetColor(head,new Color(.42f,.18f,.10f));
                SetColor(barrel,new Color(.32f,.12f,.07f));
                Primitive(head.transform,"Brazier",PrimitiveType.Cylinder,new Vector3(0f,.48f,0f),new Vector3(.36f,.12f,.36f),new Color(.25f,.20f,.16f));
                GameObject flame = Primitive(head.transform,"FlameCore",PrimitiveType.Sphere,new Vector3(0f,.74f,0f),new Vector3(.30f,.42f,.30f),new Color(1f,.32f,.04f));
                flame.AddComponent<ChapterOneAmbientMotion>().kind = ChapterOneAmbientMotion.MotionKind.Flame;
                break;
            case TowerType.TrojanGuard:
                SetColor(head,new Color(.52f,.18f,.14f));
                barrel.SetActive(false);
                Primitive(root.transform,"ShieldL",PrimitiveType.Cube,new Vector3(-.28f,.64f,.28f),new Vector3(.32f,.52f,.10f),new Color(.72f,.46f,.14f));
                Primitive(root.transform,"ShieldR",PrimitiveType.Cube,new Vector3(.28f,.64f,.28f),new Vector3(.32f,.52f,.10f),new Color(.72f,.46f,.14f));
                Primitive(head.transform,"Crest",PrimitiveType.Cube,new Vector3(0f,.48f,0f),new Vector3(.15f,.30f,.48f),new Color(.68f,.08f,.06f));
                break;
        }

        TowerArtDirector.Enhance(root, type);

        GameObject muzzle = new GameObject("Muzzle");
        muzzle.transform.SetParent(head.transform);
        muzzle.transform.localPosition = new Vector3(0f,0f,1.45f);

        BoxCollider interaction = root.AddComponent<BoxCollider>();
        interaction.center = new Vector3(0f,.55f,0f);
        interaction.size = new Vector3(1.35f,1.45f,1.35f);

        Tower tower = root.AddComponent<Tower>();
        tower.head = head.transform;
        tower.muzzle = muzzle.transform;
        tower.Configure(type, data.cost);
        return root;
    }

    static GameObject Primitive(Transform parent,string name,PrimitiveType type,Vector3 localPos,Vector3 localScale,Color color,Quaternion? localRot=null)
    {
        GameObject go=GameObject.CreatePrimitive(type);
        go.name=name;
        go.transform.SetParent(parent,false);
        go.transform.localPosition=localPos;
        go.transform.localScale=localScale;
        if(localRot.HasValue) go.transform.localRotation=localRot.Value;
        Object.Destroy(go.GetComponent<Collider>());
        SetColor(go,color);
        return go;
    }

    public static void SetColor(GameObject obj, Color color)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return;
        Material material = new Material(GetRuntimeMaterialTemplate());
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color")) material.SetColor("_Color", color);
        renderer.material = material;
    }

    static Material GetRuntimeMaterialTemplate()
    {
        if (runtimeMaterialTemplate != null) return runtimeMaterialTemplate;
        runtimeMaterialTemplate = Resources.Load<Material>(RuntimeMaterialResource);
        if (runtimeMaterialTemplate != null) return runtimeMaterialTemplate;
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        runtimeMaterialTemplate = new Material(shader);
        return runtimeMaterialTemplate;
    }
}
