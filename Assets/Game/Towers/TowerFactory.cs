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

        Tower tower = root.AddComponent<Tower>();
        tower.head = headObject.transform;
        tower.muzzle = muzzle.transform;
        tower.Configure(type, data.cost);
        return root;
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
