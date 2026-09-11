using UnityEngine;

public static class TowerFactory
{
    public static GameObject CreateTower(Vector3 position, string name = "Tower")
    {
        GameObject root = new GameObject(name);
        root.transform.position = position;

        GameObject baseObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        baseObj.name = "Base";
        baseObj.transform.SetParent(root.transform);
        baseObj.transform.localPosition = Vector3.zero;
        baseObj.transform.localScale = new Vector3(0.75f, 0.35f, 0.75f);
        SetColor(baseObj, new Color(0.18f, 0.22f, 0.27f));

        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
        head.name = "Head";
        head.transform.SetParent(root.transform);
        head.transform.localPosition = new Vector3(0f, 0.75f, 0f);
        head.transform.localScale = new Vector3(0.58f, 0.34f, 0.82f);
        SetColor(head, new Color(0.32f, 0.38f, 0.46f));

        GameObject barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        barrel.name = "Barrel";
        barrel.transform.SetParent(head.transform);
        barrel.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        barrel.transform.localPosition = new Vector3(0f, 0f, 0.9f);
        barrel.transform.localScale = new Vector3(0.12f, 0.55f, 0.12f);
        SetColor(barrel, new Color(0.12f, 0.14f, 0.16f));

        GameObject muzzle = new GameObject("Muzzle");
        muzzle.transform.SetParent(head.transform);
        muzzle.transform.localPosition = new Vector3(0f, 0f, 1.5f);

        Tower tower = root.AddComponent<Tower>();
        tower.head = head.transform;
        tower.muzzle = muzzle.transform;
        return root;
    }

    public static GameObject CreateGhost()
    {
        GameObject root = CreateTower(Vector3.zero, "TowerPreview");
        Tower tower = root.GetComponent<Tower>();
        if (tower != null) Object.Destroy(tower);

        foreach (Collider c in root.GetComponentsInChildren<Collider>())
            Object.Destroy(c);

        SetGhostColor(root, new Color(0.15f, 0.95f, 0.35f, 0.42f));
        return root;
    }

    public static void SetGhostValidity(GameObject ghost, bool valid)
    {
        if (ghost == null) return;
        Color c = valid ? new Color(0.15f, 0.95f, 0.35f, 0.42f) : new Color(1f, 0.18f, 0.15f, 0.42f);
        SetGhostColor(ghost, c);
    }

    static void SetGhostColor(GameObject root, Color color)
    {
        foreach (Renderer r in root.GetComponentsInChildren<Renderer>())
        {
            Material m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (m.shader == null) m = new Material(Shader.Find("Standard"));
            m.color = color;
            r.material = m;
        }
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
