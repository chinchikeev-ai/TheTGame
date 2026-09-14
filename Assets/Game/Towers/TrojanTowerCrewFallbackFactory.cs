using UnityEngine;

// Presentation-only fallback silhouettes used when generated/production Trojan
// character prefabs are unavailable in the current checkout/build.
public static class TrojanTowerCrewFallbackFactory
{
    static readonly Color Skin = new Color(.78f, .57f, .39f);
    static readonly Color Red = new Color(.58f, .065f, .04f);
    static readonly Color DarkRed = new Color(.34f, .035f, .025f);
    static readonly Color Bronze = new Color(.76f, .50f, .16f);
    static readonly Color DarkBronze = new Color(.40f, .24f, .08f);
    static readonly Color Leather = new Color(.23f, .12f, .055f);
    static readonly Color Wood = new Color(.31f, .18f, .075f);

    public static GameObject Create(string prefabName)
    {
        switch (prefabName)
        {
            case "Trojan_Archer": return BuildArcher();
            case "Trojan_Guard": return BuildGuard();
            case "Trojan_Infantry": return BuildSpearman();
            default: return BuildSpearman();
        }
    }

    static GameObject BuildArcher()
    {
        GameObject root = BuildBody("Procedural_Trojan_Archer", .78f, 1.05f, .78f);
        Part(root.transform, "Role_Archer_RedHeadband", PrimitiveType.Cube,
            new Vector3(0f, 1.78f, .01f), new Vector3(.26f, .045f, .25f), Red);
        AddBow(root.transform);
        AddQuiver(root.transform);
        return root;
    }

    static GameObject BuildSpearman()
    {
        GameObject root = BuildBody("Procedural_Trojan_Infantry", .94f, 1f, .94f);
        AddHelmet(root.transform, false);
        AddSpear(root.transform, "Role_Spear", .47f, 1.05f, .02f, 1.14f);
        AddRoundShield(root.transform, "Role_RoundShield", .43f, Red);
        return root;
    }

    static GameObject BuildGuard()
    {
        GameObject root = BuildBody("Procedural_Trojan_Guard", 1.18f, .94f, 1.12f);
        AddHelmet(root.transform, true);
        AddSpear(root.transform, "Role_GuardSpear", .50f, 1.04f, -.02f, 1.18f);
        AddRoundShield(root.transform, "Role_GuardOversizedShield", .61f, DarkRed);
        Part(root.transform, "Role_GuardShoulderL", PrimitiveType.Sphere,
            new Vector3(-.39f, 1.29f, 0f), new Vector3(.20f, .14f, .20f), Bronze);
        Part(root.transform, "Role_GuardShoulderR", PrimitiveType.Sphere,
            new Vector3(.39f, 1.29f, 0f), new Vector3(.20f, .14f, .20f), Bronze);
        return root;
    }

    static GameObject BuildBody(string name, float width, float height, float depth)
    {
        GameObject root = new GameObject(name);
        Part(root.transform, "Body_RedTunic", PrimitiveType.Capsule,
            new Vector3(0f, .98f, 0f), new Vector3(.36f * width, .48f * height, .30f * depth), Red);
        Part(root.transform, "Body_BronzeChest", PrimitiveType.Cube,
            new Vector3(0f, 1.09f, .02f), new Vector3(.40f * width, .27f * height, .27f * depth), Bronze);
        Part(root.transform, "Body_Head", PrimitiveType.Sphere,
            new Vector3(0f, 1.66f * height, 0f), new Vector3(.29f, .29f, .29f), Skin);
        Part(root.transform, "Body_Belt", PrimitiveType.Cube,
            new Vector3(0f, .74f, 0f), new Vector3(.40f * width, .075f, .28f * depth), Leather);
        Part(root.transform, "Body_LegL", PrimitiveType.Cylinder,
            new Vector3(-.14f * width, .28f, 0f), new Vector3(.11f, .31f, .11f), Leather);
        Part(root.transform, "Body_LegR", PrimitiveType.Cylinder,
            new Vector3(.14f * width, .28f, 0f), new Vector3(.11f, .31f, .11f), Leather);
        return root;
    }

    static void AddHelmet(Transform root, bool heavy)
    {
        Part(root, heavy ? "Role_GuardHelmet" : "Role_InfantryHelmet", PrimitiveType.Sphere,
            new Vector3(0f, 1.76f, 0f), new Vector3(heavy ? .33f : .30f, .18f, heavy ? .33f : .30f), Bronze);
        Part(root, heavy ? "Role_GuardCrest" : "Role_InfantryCrest", PrimitiveType.Cube,
            new Vector3(0f, 1.98f, -.01f), new Vector3(heavy ? .14f : .11f, .25f, .34f), Red);
    }

    static void AddSpear(Transform root, string name, float x, float y, float z, float length)
    {
        GameObject spear = Part(root, name, PrimitiveType.Cylinder,
            new Vector3(x, y, z), new Vector3(.032f, length, .032f), Wood,
            Quaternion.Euler(7f, 0f, -9f));
        Part(spear.transform, name + "_BronzeTip", PrimitiveType.Cube,
            new Vector3(0f, 1.04f, 0f), new Vector3(.08f, .16f, .035f), Bronze,
            Quaternion.Euler(0f, 0f, 45f));
    }

    static void AddRoundShield(Transform root, string name, float radius, Color face)
    {
        GameObject shield = Part(root, name, PrimitiveType.Cylinder,
            new Vector3(-.48f, 1.02f, .13f), new Vector3(radius, .065f, radius), face,
            Quaternion.Euler(90f, 0f, 0f));
        Part(shield.transform, name + "_BronzeBoss", PrimitiveType.Sphere,
            new Vector3(0f, .08f, 0f), new Vector3(.22f, .10f, .22f), Bronze);
    }

    static void AddBow(Transform root)
    {
        Transform bow = new GameObject("Role_Bow").transform;
        bow.SetParent(root, false);
        bow.localPosition = new Vector3(.42f, 1.08f, .08f);

        GameObject upper = Part(bow, "Role_Bow_Upper", PrimitiveType.Cylinder,
            new Vector3(0f, .25f, 0f), new Vector3(.03f, .34f, .03f), Wood);
        upper.transform.localRotation = Quaternion.Euler(0f, 0f, 28f);
        GameObject lower = Part(bow, "Role_Bow_Lower", PrimitiveType.Cylinder,
            new Vector3(0f, -.25f, 0f), new Vector3(.03f, .34f, .03f), Wood);
        lower.transform.localRotation = Quaternion.Euler(0f, 0f, -28f);
        Part(bow, "Role_Bow_String", PrimitiveType.Cylinder,
            new Vector3(-.12f, 0f, 0f), new Vector3(.008f, .58f, .008f), new Color(.82f, .74f, .56f));
    }

    static void AddQuiver(Transform root)
    {
        GameObject quiver = Part(root, "Role_Quiver", PrimitiveType.Cylinder,
            new Vector3(-.24f, 1.02f, -.23f), new Vector3(.11f, .37f, .11f), Leather,
            Quaternion.Euler(15f, 0f, -18f));
        for (int i = -1; i <= 1; i++)
            Part(quiver.transform, "Role_Arrow_" + i, PrimitiveType.Cylinder,
                new Vector3(i * .06f, .40f, 0f), new Vector3(.012f, .18f, .012f), Bronze);
    }

    static GameObject Part(Transform parent, string name, PrimitiveType type, Vector3 position, Vector3 scale, Color color, Quaternion? rotation = null)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = position;
        go.transform.localScale = scale;
        if (rotation.HasValue) go.transform.localRotation = rotation.Value;

        Collider collider = go.GetComponent<Collider>();
        if (collider != null)
        {
            if (Application.isPlaying) Object.Destroy(collider);
            else Object.DestroyImmediate(collider);
        }

        TowerFactory.SetColor(go, color);
        return go;
    }
}
