using System;
using UnityEditor;
using UnityEngine;

public static class ChapterOneArmorCandidateBuilder
{
    const string CharacterRoot = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters";
    const string GreekRoot = CharacterRoot + "/Greek/";
    const string TrojanRoot = CharacterRoot + "/Trojan/";
    const string HeroRoot = CharacterRoot + "/Heroes/";
    const string CuirassPath = "Assets/Game/Art/Characters/Equipment/DendraCuirassCandidate.obj";
    const string HelmetPath = "Assets/Game/Art/Characters/Equipment/BoarTuskHelmetCandidate.obj";
    const string SourceCuirassName = "SourceArmor_DendraCandidate";
    const string SourceHelmetName = "SourceHelmet_BoarTuskCandidate";

    static readonly string[] TorsoBoneHints =
    {
        "upperchest", "chest", "spine2", "spine02", "spine1", "spine01", "spine"
    };

    static readonly string[] HeadBoneHints =
    {
        "head", "head01"
    };

    struct ArmorTarget
    {
        public string prefabPath;
        public float cuirassScale;
        public float helmetScale;

        public ArmorTarget(string prefabPath, float cuirassScale, float helmetScale)
        {
            this.prefabPath = prefabPath;
            this.cuirassScale = cuirassScale;
            this.helmetScale = helmetScale;
        }
    }

    static readonly ArmorTarget[] Targets =
    {
        new ArmorTarget(GreekRoot + "Enemy_Infantry.prefab", 1.00f, 1.00f),
        new ArmorTarget(GreekRoot + "Enemy_HeavyHoplite.prefab", 1.12f, 1.06f),
        new ArmorTarget(GreekRoot + "Enemy_ShieldBearer.prefab", 1.08f, 1.04f),
        new ArmorTarget(GreekRoot + "Enemy_Boss.prefab", 1.14f, 1.08f),
        new ArmorTarget(TrojanRoot + "Trojan_Infantry.prefab", 1.00f, 1.00f),
        new ArmorTarget(TrojanRoot + "Trojan_Guard.prefab", 1.10f, 1.06f),
        new ArmorTarget(HeroRoot + "Hero_Hector.prefab", 1.15f, 1.10f),
        new ArmorTarget(HeroRoot + "Hero_Menelaus.prefab", 1.12f, 1.08f)
    };

    [MenuItem("The Troy Game/Characters/Bind Late Bronze Age Armor Candidates")]
    public static void Build()
    {
        GameObject cuirass = AssetDatabase.LoadAssetAtPath<GameObject>(CuirassPath);
        GameObject helmet = AssetDatabase.LoadAssetAtPath<GameObject>(HelmetPath);
        if (cuirass == null) throw new InvalidOperationException("Authored Dendra-inspired cuirass candidate is missing: " + CuirassPath);
        if (helmet == null) throw new InvalidOperationException("Authored boar-tusk helmet candidate is missing: " + HelmetPath);

        int upgraded = 0;
        foreach (ArmorTarget target in Targets)
            if (Upgrade(target, cuirass, helmet)) upgraded++;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Chapter I Late Bronze Age armor pass upgraded " + upgraded + " prefab(s). Armor now rigidly follows resolved torso/head bones where available, but meshes are still unskinned production candidates pending real Unity animation-clearance, material and gameplay-camera QA.");
    }

    static bool Upgrade(ArmorTarget target, GameObject cuirassSource, GameObject helmetSource)
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(target.prefabPath) == null)
        {
            Debug.LogWarning("Armor pass skipped missing prefab: " + target.prefabPath);
            return false;
        }

        GameObject root = PrefabUtility.LoadPrefabContents(target.prefabPath);
        try
        {
            RemoveByName(root, SourceCuirassName);
            RemoveByName(root, SourceHelmetName);
            RemoveProceduralArmorParts(root);

            GameObject cuirass = InstantiateSource(cuirassSource, SourceCuirassName, root.transform);
            cuirass.transform.localPosition = new Vector3(0f, 1.10f, .01f);
            cuirass.transform.localRotation = Quaternion.identity;
            cuirass.transform.localScale = Vector3.one * target.cuirassScale;
            PrepareDecorativeMesh(cuirass, new Color(.67f, .44f, .17f), "cuirass");
            Transform torsoBone = ResolveTorsoBone(root);
            AttachRigidlyToBone(cuirass, torsoBone, target.prefabPath, "cuirass");

            GameObject helmet = InstantiateSource(helmetSource, SourceHelmetName, root.transform);
            helmet.transform.localPosition = new Vector3(0f, 1.70f, 0f);
            helmet.transform.localRotation = Quaternion.identity;
            helmet.transform.localScale = Vector3.one * target.helmetScale;
            PrepareDecorativeMesh(helmet, new Color(.76f, .70f, .55f), "helmet");
            Transform headBone = ResolveHeadBone(root);
            AttachRigidlyToBone(helmet, headBone, target.prefabPath, "helmet");

            PrefabUtility.SaveAsPrefabAsset(root, target.prefabPath);
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
        if (item == null) throw new InvalidOperationException("Unable to instantiate authored armor candidate: " + sourceName);
        item.name = sourceName;
        item.transform.SetParent(parent, false);
        return item;
    }

    static void PrepareDecorativeMesh(GameObject item, Color tint, string label)
    {
        foreach (Collider collider in item.GetComponentsInChildren<Collider>(true))
            UnityEngine.Object.DestroyImmediate(collider);

        Renderer[] renderers = item.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0)
            throw new InvalidOperationException("Authored Chapter I " + label + " candidate imported without a Renderer.");

        foreach (Renderer renderer in renderers)
            TowerFactory.SetColor(renderer.gameObject, tint);
    }

    static Transform ResolveTorsoBone(GameObject root)
    {
        Animator animator = root.GetComponentInChildren<Animator>(true);
        Transform humanoidBone = ResolveHumanoidBone(animator, HumanBodyBones.UpperChest);
        if (humanoidBone == null) humanoidBone = ResolveHumanoidBone(animator, HumanBodyBones.Chest);
        if (humanoidBone == null) humanoidBone = ResolveHumanoidBone(animator, HumanBodyBones.Spine);
        return humanoidBone != null ? humanoidBone : FindBoneByHints(root, TorsoBoneHints);
    }

    static Transform ResolveHeadBone(GameObject root)
    {
        Animator animator = root.GetComponentInChildren<Animator>(true);
        Transform humanoidBone = ResolveHumanoidBone(animator, HumanBodyBones.Head);
        return humanoidBone != null ? humanoidBone : FindBoneByHints(root, HeadBoneHints);
    }

    static Transform ResolveHumanoidBone(Animator animator, HumanBodyBones bone)
    {
        if (animator == null || animator.avatar == null || !animator.avatar.isHuman) return null;
        return animator.GetBoneTransform(bone);
    }

    static Transform FindBoneByHints(GameObject root, string[] hints)
    {
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        foreach (string hint in hints)
        {
            foreach (Transform transform in all)
            {
                if (transform == null || transform == root.transform) continue;
                string normalized = NormalizeBoneName(transform.name);
                if (normalized == hint || normalized.EndsWith(hint, StringComparison.Ordinal)) return transform;
            }
        }
        return null;
    }

    static string NormalizeBoneName(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        char[] buffer = new char[value.Length];
        int count = 0;
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            if (!char.IsLetterOrDigit(c)) continue;
            buffer[count++] = char.ToLowerInvariant(c);
        }
        return new string(buffer, 0, count);
    }

    static void AttachRigidlyToBone(GameObject item, Transform bone, string prefabPath, string label)
    {
        if (bone == null)
        {
            Debug.LogWarning("Chapter I " + label + " candidate could not resolve a rig bone for " + prefabPath + "; keeping the candidate in root-space for visual QA.");
            return;
        }

        // Preserve the authored/rest-pose world transform while making the static mesh follow the animated bone.
        // This is a rigid attachment only; it does not claim skinning or deformation quality.
        item.transform.SetParent(bone, true);
    }

    static void RemoveProceduralArmorParts(GameObject root)
    {
        Transform kit = FindByName(root, "LateBronzeAgeKit");
        if (kit == null) return;

        string[] names =
        {
            "BronzeCuirass",
            "LeftShoulderBronze",
            "RightShoulderBronze",
            "BronzeHelmet",
            "HelmetCheekLeft",
            "HelmetCheekRight"
        };

        foreach (string name in names)
        {
            Transform part = FindDirectChild(kit, name);
            if (part != null) UnityEngine.Object.DestroyImmediate(part.gameObject);
        }
    }

    static Transform FindDirectChild(Transform parent, string objectName)
    {
        foreach (Transform child in parent)
            if (string.Equals(child.name, objectName, StringComparison.Ordinal)) return child;
        return null;
    }

    static Transform FindByName(GameObject root, string objectName)
    {
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform transform in all)
            if (transform != null && string.Equals(transform.name, objectName, StringComparison.Ordinal)) return transform;
        return null;
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
}
