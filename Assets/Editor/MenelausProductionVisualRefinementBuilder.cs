using System;
using UnityEditor;
using UnityEngine;

public static class MenelausProductionVisualRefinementBuilder
{
    const string CharacterRoot = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters";
    const string BossPrefabPath = CharacterRoot + "/Greek/Enemy_Boss.prefab";
    const string HeroPrefabPath = CharacterRoot + "/Heroes/Hero_Menelaus.prefab";
    const string RefinementPrefix = "MenelausRefinement_";

    static readonly Color Bronze = new Color(.70f, .47f, .16f);
    static readonly Color BrightBronze = new Color(.84f, .61f, .22f);
    static readonly Color Ivory = new Color(.80f, .76f, .65f);
    static readonly Color Cobalt = new Color(.12f, .24f, .52f);
    static readonly Color DeepBlue = new Color(.07f, .13f, .30f);
    static readonly Color Leather = new Color(.23f, .12f, .055f);

    [MenuItem("The Troy Game/Characters/Refine Menelaus Production Silhouette")]
    public static void Build()
    {
        ApplyIfAvailable(true);
    }

    public static bool ApplyIfAvailable(bool logIfMissing = false)
    {
        bool changed = false;
        changed |= ApplyToPrefab(BossPrefabPath, logIfMissing);
        changed |= ApplyToPrefab(HeroPrefabPath, logIfMissing);
        return changed;
    }

    static bool ApplyToPrefab(string prefabPath, bool logIfMissing)
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) == null)
        {
            if (logIfMissing)
                Debug.LogWarning("Menelaus silhouette refinement skipped because prefab is missing: " + prefabPath);
            return false;
        }

        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            RemovePreviousRefinement(root);
            RemoveLegacyRootSpaceSilhouetteParts(root);

            Animator animator = root.GetComponentInChildren<Animator>(true);
            Transform head = ResolveBone(animator, HumanBodyBones.Head) ?? FindByHints(root, "head");
            Transform chest = ResolveBone(animator, HumanBodyBones.UpperChest) ?? ResolveBone(animator, HumanBodyBones.Chest) ?? FindByHints(root, "upperchest", "chest", "spine2", "spine02");
            Transform hips = ResolveBone(animator, HumanBodyBones.Hips) ?? FindByHints(root, "hips", "pelvis");
            Transform leftUpperArm = ResolveBone(animator, HumanBodyBones.LeftUpperArm) ?? FindByHints(root, "leftupperarm", "upperarm_l", "arm_l");
            Transform rightUpperArm = ResolveBone(animator, HumanBodyBones.RightUpperArm) ?? FindByHints(root, "rightupperarm", "upperarm_r", "arm_r");
            Transform leftHand = ResolveBone(animator, HumanBodyBones.LeftHand) ?? ResolveBone(animator, HumanBodyBones.LeftLowerArm) ?? FindByHints(root, "lefthand", "hand_l", "leftforearm");

            AddRoyalHeadgear(root, head);
            AddCommanderUpperBody(root, chest, leftUpperArm, rightUpperArm);
            AddCommanderMantle(root, chest);
            AddRoyalSashAndWaist(root, chest, hips);
            AddShieldIdentity(root, leftHand);

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            Debug.Log("Menelaus production silhouette refinement applied to " + prefabPath + ": Mycenaean royal headgear, commander shoulders/pectorals, cobalt mantle/sash and royal shield roundel. Gameplay root/collider/navigation remain unchanged; visual QA is still required.");
            return true;
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    static void AddRoyalHeadgear(GameObject root, Transform head)
    {
        CreateRigFollowingPart(root, head, "MenelausRefinement_RoyalBrowBand", PrimitiveType.Cube,
            new Vector3(0f, 1.72f, -.02f), new Vector3(.25f, .055f, .20f), Bronze);
        CreateRigFollowingPart(root, head, "MenelausRefinement_RoyalBrowGold", PrimitiveType.Cube,
            new Vector3(0f, 1.72f, -.215f), new Vector3(.13f, .045f, .022f), BrightBronze);
        CreateRigFollowingPart(root, head, "MenelausRefinement_CrestBase", PrimitiveType.Cube,
            new Vector3(0f, 1.87f, .01f), new Vector3(.18f, .055f, .14f), BrightBronze);
        CreateRigFollowingPart(root, head, "MenelausRefinement_CobaltCrest", PrimitiveType.Cube,
            new Vector3(0f, 2.04f, .025f), new Vector3(.085f, .25f, .15f), Cobalt, Quaternion.Euler(-7f, 0f, 0f));
        CreateRigFollowingPart(root, head, "MenelausRefinement_CrestTip", PrimitiveType.Cube,
            new Vector3(0f, 2.24f, .06f), new Vector3(.065f, .12f, .12f), DeepBlue, Quaternion.Euler(-16f, 0f, 0f));
        CreateRigFollowingPart(root, head, "MenelausRefinement_CheekGuardLeft", PrimitiveType.Cube,
            new Vector3(-.19f, 1.58f, -.12f), new Vector3(.055f, .16f, .10f), Bronze, Quaternion.Euler(0f, 0f, 8f));
        CreateRigFollowingPart(root, head, "MenelausRefinement_CheekGuardRight", PrimitiveType.Cube,
            new Vector3(.19f, 1.58f, -.12f), new Vector3(.055f, .16f, .10f), Bronze, Quaternion.Euler(0f, 0f, -8f));
    }

    static void AddCommanderUpperBody(GameObject root, Transform chest, Transform leftArm, Transform rightArm)
    {
        CreateRigFollowingPart(root, chest, "MenelausRefinement_CommanderCollar", PrimitiveType.Cube,
            new Vector3(0f, 1.34f, -.01f), new Vector3(.56f, .10f, .28f), Bronze);
        CreateRigFollowingPart(root, chest, "MenelausRefinement_RoyalPectoral", PrimitiveType.Cube,
            new Vector3(0f, 1.22f, -.245f), new Vector3(.31f, .12f, .028f), BrightBronze);
        CreateRigFollowingPart(root, chest, "MenelausRefinement_PectoralCenter", PrimitiveType.Sphere,
            new Vector3(0f, 1.23f, -.285f), new Vector3(.095f, .095f, .035f), Ivory);

        CreateRigFollowingPart(root, leftArm, "MenelausRefinement_PauldronLeft", PrimitiveType.Sphere,
            new Vector3(-.42f, 1.32f, -.015f), new Vector3(.23f, .15f, .22f), Bronze);
        CreateRigFollowingPart(root, rightArm, "MenelausRefinement_PauldronRight", PrimitiveType.Sphere,
            new Vector3(.42f, 1.32f, -.015f), new Vector3(.23f, .15f, .22f), Bronze);
        CreateRigFollowingPart(root, leftArm, "MenelausRefinement_PauldronBandLeft", PrimitiveType.Cube,
            new Vector3(-.46f, 1.29f, -.16f), new Vector3(.17f, .045f, .05f), BrightBronze, Quaternion.Euler(0f, 0f, 13f));
        CreateRigFollowingPart(root, rightArm, "MenelausRefinement_PauldronBandRight", PrimitiveType.Cube,
            new Vector3(.46f, 1.29f, -.16f), new Vector3(.17f, .045f, .05f), BrightBronze, Quaternion.Euler(0f, 0f, -13f));
    }

    static void AddCommanderMantle(GameObject root, Transform chest)
    {
        CreateRigFollowingPart(root, chest, "MenelausRefinement_MantleCenter", PrimitiveType.Cube,
            new Vector3(0f, .98f, .24f), new Vector3(.51f, .58f, .035f), DeepBlue, Quaternion.Euler(-10f, 0f, 0f));
        CreateRigFollowingPart(root, chest, "MenelausRefinement_MantleLeft", PrimitiveType.Cube,
            new Vector3(-.31f, 1.00f, .225f), new Vector3(.17f, .54f, .032f), Cobalt, Quaternion.Euler(-8f, 0f, 7f));
        CreateRigFollowingPart(root, chest, "MenelausRefinement_MantleRight", PrimitiveType.Cube,
            new Vector3(.31f, 1.00f, .225f), new Vector3(.17f, .54f, .032f), Cobalt, Quaternion.Euler(-8f, 0f, -7f));
        CreateRigFollowingPart(root, chest, "MenelausRefinement_MantleGoldEdge", PrimitiveType.Cube,
            new Vector3(0f, .43f, .19f), new Vector3(.50f, .035f, .024f), BrightBronze, Quaternion.Euler(-12f, 0f, 0f));
        CreateRigFollowingPart(root, chest, "MenelausRefinement_ClaspLeft", PrimitiveType.Sphere,
            new Vector3(-.26f, 1.38f, -.20f), new Vector3(.075f, .075f, .045f), BrightBronze);
        CreateRigFollowingPart(root, chest, "MenelausRefinement_ClaspRight", PrimitiveType.Sphere,
            new Vector3(.26f, 1.38f, -.20f), new Vector3(.075f, .075f, .045f), BrightBronze);
    }

    static void AddRoyalSashAndWaist(GameObject root, Transform chest, Transform hips)
    {
        CreateRigFollowingPart(root, chest, "MenelausRefinement_CobaltSash", PrimitiveType.Cube,
            new Vector3(.09f, 1.02f, -.275f), new Vector3(.10f, .54f, .025f), Cobalt, Quaternion.Euler(0f, 0f, -22f));
        CreateRigFollowingPart(root, hips, "MenelausRefinement_RoyalBelt", PrimitiveType.Cube,
            new Vector3(0f, .81f, 0f), new Vector3(.43f, .075f, .23f), Leather);
        CreateRigFollowingPart(root, hips, "MenelausRefinement_BeltBuckle", PrimitiveType.Cube,
            new Vector3(0f, .81f, -.24f), new Vector3(.12f, .10f, .025f), BrightBronze);

        for (int i = -2; i <= 2; i++)
        {
            float x = i * .12f;
            CreateRigFollowingPart(root, hips, "MenelausRefinement_Pteruge_" + (i + 2), PrimitiveType.Cube,
                new Vector3(x, .61f, -.02f), new Vector3(.09f, .23f, .16f), i % 2 == 0 ? Ivory : Cobalt,
                Quaternion.Euler(0f, 0f, -i * 3f));
        }
    }

    static void AddShieldIdentity(GameObject root, Transform leftHand)
    {
        Transform shield = FindByName(root, "SourceShield_LateBronzeAge") ?? FindByHints(root, "shieldbadgecolor", "shieldbadge", "shieldround", "shield");
        Transform anchor = shield ?? leftHand;

        CreateRigFollowingPart(root, anchor, "MenelausRefinement_ShieldRoyalRoundel", PrimitiveType.Sphere,
            new Vector3(-.49f, 1.03f, -.28f), new Vector3(.17f, .17f, .032f), Cobalt);
        CreateRigFollowingPart(root, anchor, "MenelausRefinement_ShieldRosetteCenter", PrimitiveType.Sphere,
            new Vector3(-.49f, 1.03f, -.315f), new Vector3(.085f, .085f, .024f), BrightBronze);
        CreateRigFollowingPart(root, anchor, "MenelausRefinement_ShieldRosetteTop", PrimitiveType.Sphere,
            new Vector3(-.49f, 1.14f, -.315f), new Vector3(.033f, .033f, .020f), Ivory);
        CreateRigFollowingPart(root, anchor, "MenelausRefinement_ShieldRosetteBottom", PrimitiveType.Sphere,
            new Vector3(-.49f, .92f, -.315f), new Vector3(.033f, .033f, .020f), Ivory);
        CreateRigFollowingPart(root, anchor, "MenelausRefinement_ShieldRosetteLeft", PrimitiveType.Sphere,
            new Vector3(-.60f, 1.03f, -.315f), new Vector3(.033f, .033f, .020f), Ivory);
        CreateRigFollowingPart(root, anchor, "MenelausRefinement_ShieldRosetteRight", PrimitiveType.Sphere,
            new Vector3(-.38f, 1.03f, -.315f), new Vector3(.033f, .033f, .020f), Ivory);
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
            Debug.LogWarning("Menelaus silhouette refinement could not resolve rig anchor for " + name + "; keeping the visual in root-space for QA.");

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
                if (candidate == wanted || candidate.EndsWith(wanted, StringComparison.Ordinal) || candidate.Contains(wanted)) return transform;
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
        RemoveByName(root, "HeroCrest");

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
