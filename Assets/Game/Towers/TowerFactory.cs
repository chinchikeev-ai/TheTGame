using UnityEngine;

public static class TowerFactory
{
    const string RuntimeMaterialResource = "RuntimeColorMaterial";
    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");
    static readonly MaterialPropertyBlock ColorBlock = new MaterialPropertyBlock();
    static Material runtimeMaterialTemplate;

    public static int GetCost(TowerType type) => BalanceCatalog.GetTower(type).cost;
    public static string GetDisplayName(TowerType type) => BalanceCatalog.GetTower(type).displayName;

    public static GameObject CreateTower(Vector3 position, TowerType type = TowerType.MachineGun, string name = null)
    {
        TowerData data = BalanceCatalog.GetTower(type);
        GameObject root = new GameObject(name ?? data.displayName);
        root.transform.position = position;

        // Gameplay anchors only. Visible tower geometry is owned by TowerArtDirector.
        GameObject headObject = new GameObject("Head");
        headObject.transform.SetParent(root.transform, false);
        headObject.transform.localPosition = new Vector3(0f,.72f,0f);

        GameObject muzzle = new GameObject("Muzzle");
        muzzle.transform.SetParent(headObject.transform, false);
        muzzle.transform.localPosition = new Vector3(0f,0f,1.45f);

        TowerArtDirector.Enhance(root, type);

        BoxCollider interaction = root.AddComponent<BoxCollider>();
        interaction.center = new Vector3(0f,.55f,0f);
        interaction.size = new Vector3(1.35f,1.45f,1.35f);

        TowerCrewAnimationBridge crewAnimation = root.AddComponent<TowerCrewAnimationBridge>();
        Tower tower = root.AddComponent<Tower>();
        tower.head = headObject.transform;
        tower.muzzle = muzzle.transform;
        tower.crewAnimation = crewAnimation;
        tower.Configure(type, data.cost);
        root.AddComponent<TowerProductionArtBinder>();
        return root;
    }

    public static void SetColor(GameObject obj, Color color)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return;

        Material material = GetRuntimeMaterialTemplate();
        if (material != null) renderer.sharedMaterial = material;

        ColorBlock.Clear();
        ColorBlock.SetColor(BaseColorId, color);
        ColorBlock.SetColor(ColorId, color);
        renderer.SetPropertyBlock(ColorBlock);
    }

    static Material GetRuntimeMaterialTemplate()
    {
        if (runtimeMaterialTemplate != null) return runtimeMaterialTemplate;
        runtimeMaterialTemplate = Resources.Load<Material>(RuntimeMaterialResource);
        if (runtimeMaterialTemplate != null) return runtimeMaterialTemplate;

        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        if (shader != null) runtimeMaterialTemplate = new Material(shader);
        return runtimeMaterialTemplate;
    }
}
