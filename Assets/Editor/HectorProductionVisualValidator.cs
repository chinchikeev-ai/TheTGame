using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class HectorProductionVisualValidator
{
    public const string HectorPrefabPath = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters/Heroes/Hero_Hector.prefab";

    static readonly string[] RequiredTransforms =
    {
        "SourceSpear_Quaternius_MedievalWeapons",
        "Socket_SpearRelease",
        "SourceShield_LateBronzeAge",
        "HectorHorseBody",
        "HectorHorseNeck",
        "HectorHorseHead",
        "HectorHorseLegFront",
        "HectorHorseLegRear",
        "HectorHorseTail",
        "SourceArmor_DendraCandidate",
        "SourceHelmet_BoarTuskCandidate",
        "HeroCrest",
        "HectorRefinement_Mantle",
        "HectorRefinement_BreastGold",
        "HectorRefinement_CapeCenter",
        "HectorRefinement_CapeLeft",
        "HectorRefinement_CapeRight",
        "HectorRefinement_CapeGoldEdge",
        "HectorRefinement_PauldronLeft",
        "HectorRefinement_PauldronRight",
        "HectorRefinement_HeroBelt",
        "HectorRefinement_BeltGold",
        "HectorRefinement_Pteruge_0",
        "HectorRefinement_Pteruge_1",
        "HectorRefinement_Pteruge_2",
        "HectorRefinement_Pteruge_3",
        "HectorRefinement_Pteruge_4",
        "HectorRefinement_GreaveLeft",
        "HectorRefinement_GreaveRight",
        "HectorRefinement_CrestGoldBase"
    };

    [MenuItem("The Troy Game/Characters/Validate Hector Production Visual")]
    public static void ValidateMenu()
    {
        List<string> problems = CollectProblems();
        if (problems.Count == 0)
        {
            Debug.Log("Hector production visual validation: source-level prefab contract is complete. Real Play Mode visual QA, clipping review and gameplay-camera acceptance are still required before DONE.");
            return;
        }

        Debug.LogError("Hector production visual validation: " + problems.Count + " problem(s):\n" + string.Join("\n", problems));
    }

    public static List<string> CollectProblems()
    {
        var problems = new List<string>();
        if (AssetDatabase.LoadAssetAtPath<GameObject>(HectorPrefabPath) == null)
        {
            problems.Add("Missing Hector prefab: " + HectorPrefabPath);
            return problems;
        }

        GameObject root = PrefabUtility.LoadPrefabContents(HectorPrefabPath);
        try
        {
            foreach (string requiredName in RequiredTransforms)
                if (FindByName(root, requiredName) == null)
                    problems.Add("Hector missing required visual element: " + requiredName);

            Animator animator = root.GetComponentInChildren<Animator>(true);
            if (animator == null)
                problems.Add("Hector missing Animator.");
            else if (animator.runtimeAnimatorController == null)
                problems.Add("Hector Animator has no runtime controller.");

            CapsuleCollider rootCollider = root.GetComponent<CapsuleCollider>();
            if (rootCollider == null)
                problems.Add("Hector gameplay root is missing its CapsuleCollider.");

            foreach (Collider collider in root.GetComponentsInChildren<Collider>(true))
            {
                if (collider == rootCollider) continue;
                problems.Add("Hector decorative hierarchy contains unexpected collider: " + GetHierarchyPath(collider.transform, root.transform));
            }
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }

        if (ChapterOneSpearSourceInstaller.LoadSpear() == null)
            problems.Add("Pinned Hector production spear source is not imported: " + ChapterOneSpearSourceInstaller.GetAssetPath());

        return problems;
    }

    static Transform FindByName(GameObject root, string objectName)
    {
        foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
            if (transform != null && string.Equals(transform.name, objectName, StringComparison.Ordinal)) return transform;
        return null;
    }

    static string GetHierarchyPath(Transform transform, Transform root)
    {
        string result = transform.name;
        Transform current = transform.parent;
        while (current != null && current != root)
        {
            result = current.name + "/" + result;
            current = current.parent;
        }
        return root.name + "/" + result;
    }
}
