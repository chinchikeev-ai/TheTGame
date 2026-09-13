using UnityEngine;

public static class HectorShieldWallFactory
{
    public static ShieldWallZone Create(Vector3 center, Quaternion rotation)
    {
        GameObject root = new GameObject("Hector Shield Wall");
        root.transform.position = center;
        root.transform.rotation = rotation;

        ShieldWallZone zone = root.AddComponent<ShieldWallZone>();
        for (int i = -1; i <= 1; i++)
            CreateShield(root.transform, i * 1.05f);

        return zone;
    }

    static void CreateShield(Transform parent, float localX)
    {
        GameObject shield = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shield.name = "Shield";
        shield.transform.SetParent(parent, false);
        shield.transform.localPosition = new Vector3(localX, .55f, 0f);
        shield.transform.localScale = new Vector3(.9f, 1.1f, .28f);
        TowerFactory.SetColor(shield, new Color(.70f, .50f, .18f));

        Collider collider = shield.GetComponent<Collider>();
        if (collider != null) Object.Destroy(collider);
    }
}
