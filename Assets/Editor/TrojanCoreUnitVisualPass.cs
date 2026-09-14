using System;
using UnityEditor;
using UnityEngine;

// Presentation-only post-process for Chapter I Trojan production candidates.
// It deliberately changes child visuals rather than root colliders/gameplay transforms.
public static class TrojanCoreUnitVisualPass
{
    const string Root = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters";
    const string TrojanRoot = Root + "/Trojan/";
    const string HeroRoot = Root + "/Heroes/";
    const string MarkerName = "TrojanUnitVisualIdentityPass";

    [MenuItem("The Troy Game/Characters/Apply Trojan Unit Visual Identity Pass")]
    public static void ApplyAll()
    {
        Apply(TrojanRoot + "Trojan_Infantry.prefab", ConfigureInfantry);
        Apply(TrojanRoot + "Trojan_Archer.prefab", ConfigureArcher);
        Apply(TrojanRoot + "Trojan_Guard.prefab", ConfigureGuard);
        Apply(HeroRoot + "Hero_Hector.prefab", ConfigureHector);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Applied Trojan core-unit visual identity pass. Root gameplay colliders and public combat behaviour were not changed.");
    }

    static void Apply(string path, Action<GameObject, Transform> configure)
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) == null)
        {
            Debug.LogWarning("Trojan visual identity pass skipped missing prefab: " + path);
            return;
        }

        GameObject root = PrefabUtility.LoadPrefabContents(path);
        try
        {
            RemoveNamed(root.transform, MarkerName);
            Transform marker = new GameObject(MarkerName).transform;
            marker.SetParent(root.transform, false);
            configure(root, marker);
            PrefabUtility.SaveAsPrefabAsset(root, path);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    static void ConfigureInfantry(GameObject root, Transform marker)
    {
        SetVisualScale(root, new Vector3(1.00f, 1.00f, 1.00f));
        SetKitScale(root, new Vector3(1.00f, 1.00f, 1.00f));

        Color leather = new Color(.26f, .13f, .065f);
        Color bronze = new Color(.69f, .43f, .15f);
        Part(marker, "Infantry Brow", PrimitiveType.Cube, new Vector3(0f, 1.67f, .235f), new Vector3(.18f, .028f, .025f), leather);
        Part(marker, "Infantry Nose", PrimitiveType.Cube, new Vector3(0f, 1.62f, .255f), new Vector3(.035f, .075f, .035f), bronze * .72f);
    }

    static void ConfigureArcher(GameObject root, Transform marker)
    {
        // Long, light silhouette. Collider remains on the unchanged root.
        SetVisualScale(root, new Vector3(.88f, 1.03f, .88f));
        SetKitScale(root, new Vector3(.91f, 1.00f, .91f));

        Color dark = new Color(.20f, .12f, .07f);
        Color cloth = new Color(.58f, .12f, .055f);
        Part(marker, "Archer Narrow Brow", PrimitiveType.Cube, new Vector3(0f, 1.69f, .235f), new Vector3(.14f, .022f, .022f), dark);
        Part(marker, "Archer Headband", PrimitiveType.Cube, new Vector3(0f, 1.77f, .03f), new Vector3(.23f, .035f, .24f), cloth);
    }

    static void ConfigureGuard(GameObject root, Transform marker)
    {
        // Broad and slightly squat so he cannot read as a scaled copy of infantry.
        SetVisualScale(root, new Vector3(1.18f, .98f, 1.14f));
        SetKitScale(root, new Vector3(1.12f, 1.00f, 1.10f));

        Color beard = new Color(.18f, .095f, .05f);
        Color bronze = new Color(.72f, .47f, .15f);
        Part(marker, "Guard Heavy Beard", PrimitiveType.Sphere, new Vector3(0f, 1.53f, .235f), new Vector3(.20f, .16f, .07f), beard);
        Part(marker, "Guard Brow", PrimitiveType.Cube, new Vector3(0f, 1.67f, .255f), new Vector3(.23f, .04f, .025f), beard);
        Part(marker, "Guard Neck Guard", PrimitiveType.Cube, new Vector3(0f, 1.43f, .01f), new Vector3(.31f, .09f, .25f), bronze * .82f);
    }

    static void ConfigureHector(GameObject root, Transform marker)
    {
        SetVisualScale(root, new Vector3(1.20f, 1.20f, 1.20f));
        SetKitScale(root, new Vector3(1.08f, 1.04f, 1.08f));

        Color bronze = new Color(.82f, .57f, .18f);
        Color dark = new Color(.20f, .10f, .055f);
        Part(marker, "Hector Hero Brow", PrimitiveType.Cube, new Vector3(0f, 1.69f, .265f), new Vector3(.20f, .03f, .025f), dark);
        Part(marker, "Hector Hero Beard", PrimitiveType.Sphere, new Vector3(0f, 1.54f, .245f), new Vector3(.16f, .12f, .055f), dark);
        Part(marker, "Hector Hero Gorget", PrimitiveType.Cube, new Vector3(0f, 1.43f, .03f), new Vector3(.32f, .08f, .25f), bronze);
    }

    static void SetVisualScale(GameObject root, Vector3 scale)
    {
        Transform visual = root.transform.Find("Visual");
        if (visual != null) visual.localScale = scale;
    }

    static void SetKitScale(GameObject root, Vector3 scale)
    {
        Transform kit = root.transform.Find("LateBronzeAgeKit");
        if (kit != null) kit.localScale = scale;
    }

    static void RemoveNamed(Transform root, string objectName)
    {
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in all)
        {
            if (child == null || child == root) continue;
            if (string.Equals(child.name, objectName, StringComparison.Ordinal))
            {
                UnityEngine.Object.DestroyImmediate(child.gameObject);
                return;
            }
        }
    }

    static GameObject Part(Transform parent, string name, PrimitiveType type, Vector3 position, Vector3 scale, Color color)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = position;
        go.transform.localScale = scale;
        Collider collider = go.GetComponent<Collider>();
        if (collider != null) UnityEngine.Object.DestroyImmediate(collider);
        TowerFactory.SetColor(go, color);
        return go;
    }
}
