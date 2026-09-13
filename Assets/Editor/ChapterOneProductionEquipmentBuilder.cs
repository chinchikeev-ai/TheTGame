using System;
using UnityEditor;
using UnityEngine;

public static class ChapterOneProductionEquipmentBuilder
{
    const string CharacterRoot = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters";
    const string GreekRoot = CharacterRoot + "/Greek/";
    const string TrojanRoot = CharacterRoot + "/Trojan/";
    const string HeroRoot = CharacterRoot + "/Heroes/";

    const string GreekArcherPath = GreekRoot + "Enemy_Archer.prefab";
    const string TrojanArcherPath = TrojanRoot + "Trojan_Archer.prefab";
    const string SourceBowName = "SourceBow_Quaternius_MedievalWeapons";
    const string SourceSpearName = "SourceSpear_Quaternius_MedievalWeapons";

    static readonly string[] SpearBearerPaths =
    {
        GreekRoot + "Enemy_Infantry.prefab",
        GreekRoot + "Enemy_HeavyHoplite.prefab",
        GreekRoot + "Enemy_ShieldBearer.prefab",
        TrojanRoot + "Trojan_Infantry.prefab",
        TrojanRoot + "Trojan_Guard.prefab",
        HeroRoot + "Hero_Hector.prefab"
    };

    [MenuItem("The Troy Game/Characters/Upgrade Chapter I Equipment From CC0 Sources")]
    public static void Build()
    {
        ChapterOneWeaponSourceInstaller.Install();
        GameObject bowSource = ChapterOneWeaponSourceInstaller.LoadBow();
        if (bowSource == null)
            throw new InvalidOperationException("Pinned Chapter I bow source failed to import: " + ChapterOneWeaponSourceInstaller.GetAssetPath());

        ChapterOneSpearSourceInstaller.Install();
        GameObject spearSource = ChapterOneSpearSourceInstaller.LoadSpear();
        if (spearSource == null)
            throw new InvalidOperationException("Pinned Chapter I spear source failed to import: " + ChapterOneSpearSourceInstaller.GetAssetPath());

        int upgradedArchers = 0;
        if (UpgradeArcher(GreekArcherPath, bowSource, .95f)) upgradedArchers++;
        if (UpgradeArcher(TrojanArcherPath, bowSource, .92f)) upgradedArchers++;

        int upgradedSpearBearers = 0;
        foreach (string path in SpearBearerPaths)
            if (UpgradeSpearBearer(path, spearSource)) upgradedSpearBearers++;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Chapter I CC0 equipment pass upgraded " + upgradedArchers + " archer prefab(s) and " + upgradedSpearBearers + " spear-bearer prefab(s). Bronze Age armor kit remains procedural candidate geometry; real Unity gameplay-camera QA is still required before production acceptance.");
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
            GameObject bow = InstantiateSource(bowSource, SourceBowName, parent);
            bow.transform.localPosition = new Vector3(.03f, .02f, .05f);
            bow.transform.localRotation = Quaternion.Euler(4f, 8f, 88f);
            bow.transform.localScale = Vector3.one * scale;
            ValidateDecorativeSource(bow, "bow");

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            return true;
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    static bool UpgradeSpearBearer(string prefabPath, GameObject spearSource)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogWarning("Chapter I spear pass skipped missing prefab: " + prefabPath);
            return false;
        }

        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            Transform procedural = FindProceduralSpear(root);
            Transform previousSource = FindByName(root, SourceSpearName);
            Transform poseSource = procedural != null ? procedural : previousSource;
            if (poseSource == null)
            {
                Debug.LogWarning("Chapter I spear pass found no recognized spear pose in prefab: " + prefabPath);
                return false;
            }

            Transform parent = poseSource.parent ?? root.transform;
            Vector3 localPosition = poseSource.localPosition;
            Quaternion localRotation = poseSource.localRotation;
            Vector3 localScale = poseSource.localScale;

            RemovePreviousSourceSpear(root);
            RemoveProceduralSpear(root);

            GameObject spear = InstantiateSource(spearSource, SourceSpearName, parent);
            spear.transform.localPosition = localPosition;
            spear.transform.localRotation = localRotation;
            spear.transform.localScale = localScale;
            ValidateDecorativeSource(spear, "spear");

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            return true;
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    static GameObject InstantiateSource(GameObject source, string sourceName, Transform parent)
    {
        GameObject item = PrefabUtility.InstantiatePrefab(source) as GameObject;
        if (item == null) item = UnityEngine.Object.Instantiate(source);
        if (item == null) throw new InvalidOperationException("Unable to instantiate pinned Chapter I equipment source: " + sourceName);

        item.name = sourceName;
        item.transform.SetParent(parent, false);
        return item;
    }

    static void ValidateDecorativeSource(GameObject item, string label)
    {
        foreach (Collider collider in item.GetComponentsInChildren<Collider>(true))
            UnityEngine.Object.DestroyImmediate(collider);

        if (item.GetComponentInChildren<Renderer>(true) == null)
            throw new InvalidOperationException("Pinned Chapter I " + label + " imported without a Renderer.");
    }

    static void RemovePreviousSourceBow(GameObject root)
    {
        RemoveByName(root, SourceBowName);
    }

    static void RemovePreviousSourceSpear(GameObject root)
    {
        RemoveByName(root, SourceSpearName);
    }

    static void RemoveByName(GameObject root, string objectName)
    {
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform transform in all)
        {
            if (transform == null || transform.gameObject == root) continue;
            if (string.Equals(transform.name, objectName, StringComparison.Ordinal))
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

    static Transform FindProceduralSpear(GameObject root)
    {
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform transform in all)
        {
            if (transform == null || !string.Equals(transform.name, "Spear", StringComparison.Ordinal)) continue;
            if (HasChild(transform, "Shaft") && HasChild(transform, "BronzeTip")) return transform;
        }
        return null;
    }

    static void RemoveProceduralSpear(GameObject root)
    {
        Transform spear = FindProceduralSpear(root);
        if (spear != null) UnityEngine.Object.DestroyImmediate(spear.gameObject);
    }

    static Transform FindByName(GameObject root, string objectName)
    {
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform transform in all)
            if (transform != null && string.Equals(transform.name, objectName, StringComparison.Ordinal)) return transform;
        return null;
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
