using UnityEngine;

public static class TowerFactory
{
    public static int GetCost(TowerType type)
    {
        switch (type)
        {
            case TowerType.Cannon: return 220;
            case TowerType.Slow: return 160;
            default: return 100;
        }
    }

    public static GameObject CreateTower(Vector3 position, TowerType type = TowerType.MachineGun, string name = null)
    {
        GameObject root = new GameObject(name ?? type.ToString());
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

        Color accent = type == TowerType.Cannon ? new Color(0.58f, 0.23f, 0.12f) : type == TowerType.Slow ? new Color(0.15f, 0.48f, 0.72f) : new Color(0.30f, 0.36f, 0.44f);
        SetColor(baseObj, new Color(0.16f, 0.19f, 0.23f));
        SetColor(head, accent);
        SetColor(barrel, type == TowerType.Slow ? new Color(0.18f, 0.72f, 0.92f) : new Color(0.10f, 0.12f, 0.14f));

        if (type == TowerType.Slow)
        {
            GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Torus);
            Object.Destroy(ring);
        }

        GameObject muzzle = new GameObject("Muzzle");
        muzzle.transform.SetParent(head.transform);
        muzzle.transform.localPosition = new Vector3(0f, 0f, type == TowerType.Cannon ? 1.6f : 1.5f);

        Tower tower = root.AddComponent<Tower>();
        tower.head = head.transform;
        tower.muzzle = muzzle.transform;
        tower.Configure(type, GetCost(type));
        return root;
    }

    public static void SetColor(GameObject obj, Color color)
    {
        Renderer r = obj.GetComponent<Renderer>();
        if (r == null) return;
        Material m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (m.shader == null) m = new Material(Shader.Find("Standard"));
        m.color = color;
        r.material = m;
    }
}
