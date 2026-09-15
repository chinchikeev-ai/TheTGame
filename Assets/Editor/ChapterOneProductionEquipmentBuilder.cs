using System;
using System.Collections.Generic;
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
    const string HectorPrefabPath = HeroRoot + "Hero_Hector.prefab";
    const string SourceBowName = "SourceBow_Quaternius_MedievalWeapons";
    const string SourceSpearName = "SourceSpear_Quaternius_MedievalWeapons";
    const string ArrowSocketName = "Socket_ArrowRelease";
    const string SpearSocketName = "Socket_SpearRelease";

    static readonly string[] SpearBearerPaths =
    {
        GreekRoot + "Enemy_Infantry.prefab",
        GreekRoot + "Enemy_HeavyHoplite.prefab",
        GreekRoot + "Enemy_ShieldBearer.prefab",
        TrojanRoot + "Trojan_Infantry.prefab",
        TrojanRoot + "Trojan_Guard.prefab",
        HectorPrefabPath
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
        if (UpgradeArcher(GreekArcherPath, bowSource, .72f)) upgradedArchers++;
        if (UpgradeArcher(TrojanArcherPath, bowSource, .70f)) upgradedArchers++;

        int upgradedSpearBearers = 0;
        foreach (string path in SpearBearerPaths)
            if (UpgradeSpearBearer(path, spearSource)) upgradedSpearBearers++;

        ChapterOneShieldCandidateBuilder.Build();
        ChapterOneArmorCandidateBuilder.Build();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Chapter I equipment pass upgraded " + upgradedArchers + " archer prefab(s), " + upgradedSpearBearers + " spear-bearer prefab(s), added weapon release sockets, and applied authored Late Bronze Age shield/armor candidates. Weapons are attached to resolved rig hands and normalized against body height; gameplay-camera QA is still required.");
    }

    public static int ApplyExistingWeaponGeometryIfSourcesAvailable(bool logSummary = false)
    {
        int upgraded = 0;
        GameObject spearSource = ChapterOneSpearSourceInstaller.LoadSpear();
        if (spearSource != null)
        {
            foreach (string path in SpearBearerPaths)
                if (UpgradeSpearBearer(path, spearSource)) upgraded++;
        }

        GameObject bowSource = ChapterOneWeaponSourceInstaller.LoadBow();
        if (bowSource != null)
        {
            if (UpgradeArcher(GreekArcherPath, bowSource, .72f)) upgraded++;
            if (UpgradeArcher(TrojanArcherPath, bowSource, .70f)) upgraded++;
        }

        if (upgraded > 0)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        if (logSummary)
            Debug.Log("Chapter I offline weapon geometry recovery updated " + upgraded + " prefab(s) using already imported pinned sources.");
        return upgraded;
    }

    public static bool ApplyHectorSpearIfSourceAvailable(bool logIfMissing = false)
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(HectorPrefabPath) == null)
        {
            if (logIfMissing) Debug.LogWarning("Hector spear recovery skipped because prefab is missing: " + HectorPrefabPath);
            return false;
        }

        GameObject spearSource = ChapterOneSpearSourceInstaller.LoadSpear();
        if (spearSource == null)
        {
            if (logIfMissing)
                Debug.LogWarning("Hector production spear source is not imported. Keeping the generated procedural spear. Run 'The Troy Game/Characters/Install CC0 Chapter I Spear' or the full equipment upgrade command, then rebuild/validate Hector. Expected source: " + ChapterOneSpearSourceInstaller.GetAssetPath());
            return false;
        }

        bool upgraded = UpgradeSpearBearer(HectorPrefabPath, spearSource);
        if (upgraded)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        return upgraded;
    }

    static bool UpgradeArcher(string prefabPath, GameObject bowSource, float bodyHeightRatio)
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
            Transform rightHand = FindRightHand(root);
            if (rightHand == null)
            {
                Debug.LogWarning("Chapter I bow pass cannot resolve right-hand rig bone; refusing root-space fallback: " + prefabPath);
                return false;
            }

            RemovePreviousSourceBow(root);
            RemoveProceduralBow(root);

            GameObject bow = InstantiateSource(bowSource, SourceBowName, rightHand);
            bow.transform.localPosition = new Vector3(.03f, .02f, .05f);
            bow.transform.localRotation = Quaternion.Euler(4f, 8f, 88f);
            bow.transform.localScale = Vector3.one;
            NormalizeLongestDimension(bow, MeasureBodyHeight(root) * bodyHeightRatio);
            CreateReleaseSocket(bow.transform, ArrowSocketName, new Vector3(0f, 0f, .18f));
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
            Transform rightHand = FindRightHand(root);
            if (rightHand == null)
            {
                Debug.LogWarning("Chapter I spear pass cannot resolve right-hand rig bone; refusing root-space fallback: " + prefabPath);
                return false;
            }

            Transform procedural = FindProceduralSpear(root);
            Transform previousSource = FindByName(root, SourceSpearName);
            Transform poseSource = procedural != null ? procedural : previousSource;

            Vector3 localPosition = new Vector3(0f, -.05f, .08f);
            Quaternion localRotation = Quaternion.Euler(90f, 0f, 0f);
            if (poseSource != null && poseSource.parent == rightHand)
            {
                localPosition = poseSource.localPosition;
                localRotation = poseSource.localRotation;
            }

            RemovePreviousSourceSpear(root);
            RemoveProceduralSpear(root);

            GameObject spear = InstantiateSource(spearSource, SourceSpearName, rightHand);
            spear.transform.localPosition = localPosition;
            spear.transform.localRotation = localRotation;
            spear.transform.localScale = Vector3.one;
            float bodyHeight = MeasureBodyHeight(root);
            float ratio = string.Equals(prefabPath, HectorPrefabPath, StringComparison.Ordinal) ? 1.42f : 1.28f;
            NormalizeLongestDimension(spear, bodyHeight * ratio);
            CreateReleaseSocket(spear.transform, SpearSocketName, new Vector3(0f, 0f, .10f));
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

    static void CreateReleaseSocket(Transform weaponRoot, string socketName, Vector3 localPosition)
    {
        Transform existing = null;
        foreach (Transform child in weaponRoot)
        {
            if (!string.Equals(child.name, socketName, StringComparison.Ordinal)) continue;
            existing = child;
            break;
        }

        Transform socket = existing;
        if (socket == null)
        {
            GameObject marker = new GameObject(socketName);
            socket = marker.transform;
            socket.SetParent(weaponRoot, false);
        }

        socket.localPosition = localPosition;
        socket.localRotation = Quaternion.identity;
        socket.localScale = Vector3.one;
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
        Animator animator = root.GetComponentInChildren<Animator>(true);
        if (animator != null && animator.avatar != null && animator.avatar.isHuman)
        {
            Transform humanoidHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            if (humanoidHand != null) return humanoidHand;
        }

        return FindRigBone(root, new[] { "righthand", "handr", "rhand" });
    }

    static Transform FindRigBone(GameObject root, string[] hints)
    {
        var bones = new HashSet<Transform>();
        foreach (SkinnedMeshRenderer renderer in root.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            if (renderer == null) continue;
            foreach (Transform bone in renderer.bones)
                if (bone != null) bones.Add(bone);
        }

        foreach (string hint in hints)
        {
            string wanted = Normalize(hint);
            foreach (Transform bone in bones)
            {
                string candidate = Normalize(bone.name);
                if (candidate == wanted || candidate.EndsWith(wanted, StringComparison.Ordinal)) return bone;
            }
        }

        foreach (string hint in hints)
        {
            string wanted = Normalize(hint);
            foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
            {
                if (transform == null || transform == root.transform) continue;
                string candidate = Normalize(transform.name);
                if (candidate == wanted || candidate.EndsWith(wanted, StringComparison.Ordinal)) return transform;
            }
        }
        return null;
    }

    static float MeasureBodyHeight(GameObject root)
    {
        Transform visual = root.transform.Find("Visual");
        GameObject target = visual != null ? visual.gameObject : root;
        SkinnedMeshRenderer[] renderers = target.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        bool hasBounds = false;
        Bounds bounds = default;
        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            if (renderer == null) continue;
            if (!hasBounds)
            {
                bounds = renderer.bounds;
                hasBounds = true;
            }
            else bounds.Encapsulate(renderer.bounds);
        }
        return hasBounds ? Mathf.Max(bounds.size.y, 1f) : 1.8f;
    }

    static void NormalizeLongestDimension(GameObject item, float targetSize)
    {
        Renderer[] renderers = item.GetComponentsInChildren<Renderer>(true);
        bool hasBounds = false;
        Bounds bounds = default;
        foreach (Renderer renderer in renderers)
        {
            if (renderer == null) continue;
            if (!hasBounds)
            {
                bounds = renderer.bounds;
                hasBounds = true;
            }
            else bounds.Encapsulate(renderer.bounds);
        }
        if (!hasBounds) return;
        float current = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
        if (current <= .0001f) return;
        item.transform.localScale *= targetSize / current;
    }

    static string Normalize(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        char[] buffer = new char[value.Length];
        int count = 0;
        foreach (char c in value)
            if (char.IsLetterOrDigit(c)) buffer[count++] = char.ToLowerInvariant(c);
        return new string(buffer, 0, count);
    }
}
