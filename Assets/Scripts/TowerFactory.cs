using UnityEngine;

public static class TowerFactory
{
    public static int GetCost(TowerType type) => BalanceCatalog.GetTower(type).cost;
    public static string GetDisplayName(TowerType type) => BalanceCatalog.GetTower(type).displayName;

    public static GameObject CreateTower(Vector3 position, TowerType type = TowerType.MachineGun, string name = null)
    {
        TowerData data = BalanceCatalog.GetTower(type);
        GameObject root = new GameObject(name ?? data.displayName);
        root.transform.position = position;

        GameObject baseObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        baseObj.name = "Base";
        baseObj.transform.SetParent(root.transform);
        baseObj.transform.localPosition = Vector3.zero;
        baseObj.transform.localScale = new Vector3(0.75f, 0.35f, 0.75f);

        GameObject head = GameObject.CreatePrimitive(type == TowerType.Cannon ? PrimitiveType.Sphere : PrimitiveType.Cube);
        head.name = "Head";
        head.transform.SetParent(root.transform);
        head.transform.localPosition = new Vector3(0f, 0.75f, 0f);
        head.transform.localScale = type == TowerType.Cannon ? new Vector3(0.72f, 0.55f, 0.72f) : new Vector3(0.58f, 0.34f, 0.82f);

        GameObject barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        barrel.name = "Barrel";
        barrel.transform.SetParent(head.transform);
        barrel.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        barrel.transform.localPosition = new Vector3(0f, 0f, type == TowerType.Cannon ? 0.8f : 0.9f);
        barrel.transform.localScale = type == TowerType.Cannon ? new Vector3(0.18f, 0.65f, 0.18f) : new Vector3(0.12f, 0.55f, 0.12f);

        Color accent = type == TowerType.Cannon ? new Color(0.58f, 0.23f, 0.12f) : type == TowerType.Slow ? new Color(0.55f, 0.48f, 0.82f) : new Color(0.55f, 0.38f, 0.20f);
        SetColor(baseObj, new Color(0.28f, 0.24f, 0.18f));
        SetColor(head, accent);
        SetColor(barrel, type == TowerType.Slow ? new Color(0.72f, 0.68f, 0.92f) : new Color(0.20f, 0.16f, 0.12f));

        if (type == TowerType.Slow)
        {
            GameObject orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            orb.name = "ApolloCore";
            orb.transform.SetParent(head.transform);
            orb.transform.localPosition = new Vector3(0f, 0.6f, 0f);
            orb.transform.localScale = Vector3.one * 0.32f;
            Object.Destroy(orb.GetComponent<Collider>());
            SetColor(orb, new Color(0.95f, 0.72f, 0.20f));
        }

        GameObject muzzle = new GameObject("Muzzle");
        muzzle.transform.SetParent(head.transform);
        muzzle.transform.localPosition = new Vector3(0f, 0f, type == TowerType.Cannon ? 1.6f : 1.5f);

        Tower tower = root.AddComponent<Tower>();
        tower.head = head.transform;
        tower.muzzle = muzzle.transform;
        tower.Configure(type, data.cost);
        return root;
    }

    public static void SetColor(GameObject obj, Color color)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return;

        // Keep the primitive's material/shader so the player build cannot lose a
        // dynamically found shader during stripping. Cloning through .material is
        // enough for per-object color without Shader.Find().
        Material material = renderer.material;
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color")) material.SetColor("_Color", color);
    }
}
