using System;
using UnityEditor;
using UnityEngine;

public static class HectorProductionVisualRefinementBuilder
{
    const string HectorPrefabPath = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters/Heroes/Hero_Hector.prefab";
    const string RefinementPrefix = "HectorRefinement_";

    static readonly Color Bronze = new Color(.70f, .47f, .16f);
    static readonly Color Gold = new Color(.88f, .64f, .20f);
    static readonly Color TrojanRed = new Color(.50f, .045f, .035f);
    static readonly Color DarkRed = new Color(.30f, .025f, .022f);
    static readonly Color Leather = new Color(.24f, .11f, .045f);

    [MenuItem("The Troy Game/Characters/Refine Hector Production Silhouette")]
    public static void Build()
    {
        ApplyIfAvailable(true);
    }

    public static bool ApplyIfAvailable(bool logIfMissing = false)
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(HectorPrefabPath) == null)
        {
            if (logIfMissing)
                Debug.LogWarning("Hector silhouette refinement skipped because prefab is missing: " + HectorPrefabPath);
            return false;
        }

        GameObject root = PrefabUtility.LoadPrefabContents(HectorPrefabPath);
        try
        {
            RemovePreviousRefinement(root);
            RemoveLegacyRootSpaceSilhouetteParts(root);

            Animator animator = root.GetComponentInChildren<Animator>(true);
            Transform chest = ResolveBone(animator, HumanBodyBones.UpperChest) ?? ResolveBone(animator, HumanBodyBones.Chest) ?? FindByHints(root, "upperchest", "chest", "spine2", "spine02");
            Transform hips = ResolveBone(animator, HumanBodyBones.Hips) ?? FindByHints(root, "hips", "pelvis");
            Transform head = ResolveBone(animator, HumanBodyBones.Head) ?? FindByHints(root, "head");
            Transform leftUpperArm = ResolveBone(animator, HumanBodyBones.LeftUpperArm) ?? FindByHints(root, "leftupperarm", "upperarm_l", "arm_l");
            Transform rightUpperArm = ResolveBone(animator, HumanBodyBones.RightUpperArm) ?? FindByHints(root, "rightupperarm", "upperarm_r", "arm_r");
            Transform leftLowerLeg = ResolveBone(animator, HumanBodyBones.LeftLowerLeg) ?? FindByHints(root, "leftlowerleg", "calf_l", "shin_l");
            Transform rightLowerLeg = ResolveBone(animator, HumanBodyBones.RightLowerLeg) ?? FindByHints(root, "rightlowerleg", "calf_r", "shin_r");

            AddCommanderMantle(root, chest);
            AddHeroCape(root, chest);
            AddHeroShoulders(root, leftUpperArm, rightUpperArm);
            AddHeroWaist(root, hips);
            AddGreaves(root, leftLowerLeg, rightLowerLeg);
            ReinforceCrest(root, head);

            PrefabUtility.SaveAsPrefabAsset(root, HectorPrefabPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Hector production silhouette refinement applied: rig-following cape, commander mantle, oversized bronze shoulders, pteruges, greaves and reinforced crest. Gameplay root/collider/navigation remain unchanged; visual QA is still required.");
            return true;
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    static void AddCommanderMantle(GameObject root, Transform chest)
    {
        CreateRigFollowingPart(root, chest, "HectorRefinement_Mantle", PrimitiveType.Cube,
            new Vector3(0f, 1.31f, -.015f), new Vector3(.55f, .12f, .30f), Bronze);
        CreateRigFollowingPart(root, chest, "HectorRefinement_BreastGold", PrimitiveType.Cube,
            new Vector3(0f, 1.23f, -.235f), new Vector3(.24f, .075f, .025f), Gold);
        CreateRigFollowingPart(root, chest, "HectorRefinement_CapeClaspLeft", PrimitiveType.Sphere,
            new Vector3(-.26f, 1.34f, -.19f), new Vector3(.075f, .075f, .045f), Gold);
        CreateRigFollowingPart(root, chest, "HectorRefinement_CapeClaspRight", PrimitiveType.Sphere,
            new Vector3(.26f, 1.34f, -.19f), new Vector3(.075f, .075f, .045f), Gold);
    }

    static void AddHeroCape(GameObject root, Transform chest)
    {
        CreateRigFollowingPart(root, chest, "HectorRefinement_CapeCenter", PrimitiveType.Cube,
            new Vector3(0f, 1.00f, .24f), new Vector3(.48f, .62f, .035f), TrojanRed, Quaternion.Euler(-10f, 0f, 0f));
        CreateRigFollowingPart(root, chest, "HectorRefinement_CapeLeft", PrimitiveType.Cube,
            new Vector3(-.30f, .99f, .23f), new Vector3(.17f, .58f, .032f), DarkRed, Quaternion.Euler(-8f, 0f, 7f));
        CreateRigFollowingPart(root, chest, "HectorRefinement_CapeRight", PrimitiveType.Cube,
            new Vector3(.30f, .99f, .23f), new Vector3(.17f, .58f, .032f), DarkRed, Quaternion.Euler(-8f, 0f, -7f));
        CreateRigFollowingPart(root, chest, "HectorRefinement_CapeGoldEdge", PrimitiveType.Cube,
            new Vector3(0f, .40f, .19f), new Vector3(.47f, .035f, .024f), Gold, Quaternion.Euler(-12f, 0f, 0f));
    }

    static void AddHeroShoulders(GameObject root, Transform leftArm, Transform rightArm)
    {
        CreateRigFollowingPart(root, leftArm, "HectorRefinement_PauldronLeft", PrimitiveType.Sphere,
            new Vector3(-.39f, 1.31f, 0f), new Vector3(.20f, .13f, .20f), Bronze);
        CreateRigFollowingPart(root, rightArm, "HectorRefinement_PauldronRight", PrimitiveType.Sphere,
            new Vector3(.39f, 1.31f, 0f), new Vector3(.20f, .13f, .20f), Bronze);
        CreateRigFollowingPart(root, leftArm, "HectorRefinement_PauldronGoldLeft", PrimitiveType.Cube,
            new Vector3(-.43f, 1.30f, -.15f), new Vector3(.16f, .045f, .045f), Gold, Quaternion.Euler(0f, 0f, 14f));
        CreateRigFollowingPart(root, rightArm, "HectorRefinement_PauldronGoldRight", PrimitiveType.Cube,
            new Vector3(.43f, 1.30f, -.15f), new Vector3(.16f, .045f, .045f), Gold, Quaternion.Euler(0f, 0f, -14f));
    }

    static void AddHeroWaist(GameObject root, Transform hips)
    {
        CreateRigFollowingPart(root, hips, "HectorRefinement_HeroBelt", PrimitiveType.Cube,
            new Vector3(0f, .82f, 0f), new Vector3(.42f, .075f, .23f), Leather);
        CreateRigFollowingPart(root, hips, "HectorRefinement_BeltGold", PrimitiveType.Cube,
            new Vector3(0f, .82f, -.235f), new Vector3(.12f, .09f, .025f), Gold);

        for (int i = -2; i <= 2; i++)
        {
            float x = i * .12f;
            CreateRigFollowingPart(root, hips, "HectorRefinement_Pteruge_" + (i + 2), PrimitiveType.Cube,
                new Vector3(x, .62f, -.02f), new Vector3(.09f, .24f, .16f), i % 2 == 0 ? TrojanRed : DarkRed,
                Quaternion.Euler(0f, 0f, -i * 3f));
        }
    }

    static void AddGreaves(GameObject root, Transform leftLeg, Transform rightLeg)
    {
        CreateRigFollowingPart(root, leftLeg, "HectorRefinement_GreaveLeft", PrimitiveType.Cube,
            new Vector3(-.16f, .34f, -.08f), new Vector3(.11f, .24f, .075f), Bronze, Quaternion.Euler(-4f, 0f, 0f));
        CreateRigFollowingPart(root, rightLeg, "HectorRefinement_GreaveRight", PrimitiveType.Cube,
            new Vector3(.16f, .34f, -.08f), new Vector3(.11f, .24f, .075f), Bronze, Quaternion.Euler(-4f, 0f, 0f));
    }

    static void ReinforceCrest(GameObject root, Transform head)
    {
        Transform oldCrest = FindByName(root, "HeroCrest");
        if (oldCrest != null)
        {
            oldCrest.localScale = new Vector3(.14f, .44f, .08f);
            TowerFactory.SetColor(oldCrest.gameObject, TrojanRed);
        }

        CreateRigFollowingPart(root, head, "HectorRefinement_CrestGoldBase", PrimitiveType.Cube,
            new Vector3(0f, 1.82f, 0f), new Vector3(.16f, .055f, .13f), Gold);
    }

    static GameObject CreateRigFollowingPart(GameObject root, Transform targetBone, string name, PrimitiveType type,
        Vector3 rootLocalPosition, Vector3 rootLocalScale, Color color, Quaternion? rootLocalRotation = null)
    {
        GameObject part = GameObject.CreatePrimitive(type);
        part.name = name;
        part.transform.SetParent(root.transform, false);
        part.transform.localPosition = rootLocalPosition;
        part.transform.localRotation = rootLocalRotation ?? Quaternion.identity;
        part.transform.localScale = rootLocalScale;

        Collider collider = part.GetComponent<Collider>();
        if (collider != null) UnityEngine.Object.DestroyImmediate(collider);
        TowerFactory.SetColor(part, color);

        if (targetBone != null)
            part.transform.SetParent(targetBone, true);
        else
            Debug.LogWarning("Hector silhouette refinement could not resolve rig bone for " + name + "; keeping the visual in root-space for QA.");

        return part;
    }

    static Transform ResolveBone(Animator animator, HumanBodyBones bone)
    {
        if (animator == null || animator.avatar == null || !animator.avatar.isHuman) return null;
        return animator.GetBoneTransform(bone);
    }

    static Transform FindByHints(GameObject root, params string[] hints)
    {
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        foreach (string hint in hints)
        {
            string wanted = Normalize(hint);
            foreach (Transform transform in all)
            {
                if (transform == null || transform == root.transform) continue;
                string candidate = Normalize(transform.name);
                if (candidate == wanted || candidate.EndsWith(wanted, StringComparison.Ordinal)) return transform;
            }
        }
        return null;
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

    static void RemoveLegacyRootSpaceSilhouetteParts(GameObject root)
    {
        RemoveByName(root, "HeroCape");

        Transform kit = FindByName(root, "LateBronzeAgeKit");
        if (kit == null) return;
        RemoveDirectChild(kit, "LeatherBelt");
        RemoveDirectChild(kit, "FactionCloth");
        RemoveDirectChild(kit, "LeftShoulderBronze");
        RemoveDirectChild(kit, "RightShoulderBronze");
    }

    static void RemovePreviousRefinement(GameObject root)
    {
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        for (int i = all.Length - 1; i >= 0; i--)
        {
            Transform transform = all[i];
            if (transform == null || transform.gameObject == root) continue;
            if (transform.name.StartsWith(RefinementPrefix, StringComparison.Ordinal))
                UnityEngine.Object.DestroyImmediate(transform.gameObject);
        }
    }

    static Transform FindByName(GameObject root, string name)
    {
        foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
            if (transform != null && string.Equals(transform.name, name, StringComparison.Ordinal)) return transform;
        return null;
    }

    static void RemoveByName(GameObject root, string name)
    {
        Transform transform = FindByName(root, name);
        if (transform != null && transform.gameObject != root)
            UnityEngine.Object.DestroyImmediate(transform.gameObject);
    }

    static void RemoveDirectChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (!string.Equals(child.name, name, StringComparison.Ordinal)) continue;
            UnityEngine.Object.DestroyImmediate(child.gameObject);
            return;
        }
    }
}
