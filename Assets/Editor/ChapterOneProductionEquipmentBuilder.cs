using System;
using UnityEditor;
using UnityEngine;

public static class ChapterOneProductionEquipmentBuilder
{
    const string GreekArcherPath = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters/Greek/Enemy_Archer.prefab";
    const string TrojanArcherPath = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters/Trojan/Trojan_Archer.prefab";
    const string SourceBowName = "SourceBow_Quaternius_MedievalWeapons";

    [MenuItem("The Troy Game/Characters/Upgrade Chapter I Equipment From CC0 Sources")]
    public static void Build()
    {
        ChapterOneWeaponSourceInstaller.Install();
        GameObject bowSource = ChapterOneWeaponSourceInstaller.LoadBow();
        if (bowSource == null)
            throw new InvalidOperationException("Pinned Chapter I bow source failed to import: " + ChapterOneWeaponSourceInstaller.GetAssetPath());

        int upgraded = 0;
        if (UpgradeArcher(GreekArcherPath, bowSource, .95f)) upgraded++;
        if (UpgradeArcher(TrojanArcherPath, bowSource, .92f)) upgraded++;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Chapter I CC0 equipment pass upgraded " + upgraded + " archer prefab(s). Spear and Bronze Age kit remain procedural candidates until authored replacements are integrated.");
    }

    static bool UpgradeArcher(string prefabPath, GameObject bowSource, float scale)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogWarning("Chapter I equipment pass skipped missing prefab: " + prefabPath);
            return false;
        }

        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RemovePreviousSourceBow(root);
            RemoveProceduralBow(root);

            Transform parent = FindRightHand(root) ?? root.transform;
            GameObject bow = PrefabUtility.InstantiatePrefab(bowSource) as GameObject;
            if (bow == null) bow = UnityEngine.Object.Instantiate(bowSource);
            if (bow == null) throw new InvalidOperationException("Unable to instantiate pinned Chapter I bow source.");

            bow.name = SourceBowName;
            bow.transform.SetParent(parent, false);
            bow.transform.localPosition = new Vector3(.03f, .02f, .05f);
            bow.transform.localRotation = Quaternion.Euler(4f, 8f, 88f);
            bow.transform.localScale = Vector3.one * scale;

            foreach (Collider collider in bow.GetComponentsInChildren<Collider>(true))
                UnityEngine.Object.DestroyImmediate(collider);

            if (bow.GetComponentInChildren<Renderer>(true) == null)
                throw new InvalidOperationException("Pinned Chapter I bow imported without a Renderer.");

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            return true;
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    static void RemovePreviousSourceBow(GameObject root)
    {
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform transform in all)
        {
            if (transform == null || transform.gameObject == root) continue;
            if (string.Equals(transform.name, SourceBowName, StringComparison.Ordinal))
                UnityEngine.Object.DestroyImmediate(transform.gameObject);
        }
    }

    static void RemoveProceduralBow(GameObject root)
    {
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform transform in all)
        {
            if (transform == null || !string.Equals(transform.name, "Bow", StringComparison.Ordinal)) continue;
            if (HasChild(transform, "UpperLimb") && HasChild(transform, "LowerLimb") && HasChild(transform, "BowString"))
            {
                UnityEngine.Object.DestroyImmediate(transform.gameObject);
                return;
            }
        }
    }

    static bool HasChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
            if (string.Equals(child.name, name, StringComparison.Ordinal)) return true;
        return false;
    }

    static Transform FindRightHand(GameObject root)
    {
        string[] hints = { "righthand", "right_hand", "hand_r", "mixamorig:righthand", "hand.r" };
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        foreach (string hint in hints)
        {
            string wanted = hint.ToLowerInvariant();
            foreach (Transform transform in all)
                if (transform.name.ToLowerInvariant().Contains(wanted)) return transform;
        }
        return null;
    }
}
