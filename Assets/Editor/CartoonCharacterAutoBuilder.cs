using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class CartoonCharacterAutoBuilder
{
    const string SourceRoot = "Assets/ThirdParty/KayKitAdventurers";
    const string CharacterRoot = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters";
    const string AnimationRoot = "Assets/Game/Art/Characters/Animation";

    static readonly string[] CoreCharacterPrefabs =
    {
        CharacterRoot + "/Greek/Enemy_Infantry.prefab",
        CharacterRoot + "/Greek/Enemy_Runner.prefab",
        CharacterRoot + "/Greek/Enemy_HeavyHoplite.prefab",
        CharacterRoot + "/Greek/Enemy_ShieldBearer.prefab",
        CharacterRoot + "/Greek/Enemy_Archer.prefab",
        CharacterRoot + "/Greek/Enemy_Boss.prefab",
        CharacterRoot + "/Trojan/Trojan_Infantry.prefab",
        CharacterRoot + "/Trojan/Trojan_Guard.prefab",
        CharacterRoot + "/Trojan/Trojan_Archer.prefab",
        CharacterRoot + "/Heroes/Hero_Hector.prefab",
        CharacterRoot + "/Heroes/Hero_Achilles.prefab",
        CharacterRoot + "/Heroes/Hero_Menelaus.prefab"
    };

    static readonly string[] SupportCharacterPrefabs =
    {
        CharacterRoot + "/Trojan/Trojan_PriestApollo.prefab",
        CharacterRoot + "/Trojan/Trojan_FireKeeper.prefab",
        CharacterRoot + "/Trojan/Trojan_BallistaCrew.prefab",
        CharacterRoot + "/Trojan/Trojan_BallistaCrew_Engineer.prefab",
        CharacterRoot + "/Trojan/Trojan_BallistaCrew_Loader.prefab",
        CharacterRoot + "/Mythic/Mythic_Cyclops.prefab"
    };

    static readonly string[] AnimationControllers =
    {
        AnimationRoot + "/ChapterOneCharacter.controller",
        AnimationRoot + "/ChapterOne_Spear.controller",
        AnimationRoot + "/ChapterOne_Archer.controller",
        AnimationRoot + "/ChapterOne_Skirmisher.controller",
        AnimationRoot + "/ChapterOne_Hector.controller",
        AnimationRoot + "/ChapterOne_Menelaus.controller",
        AnimationRoot + "/ChapterOne_BallistaCrew.controller",
        AnimationRoot + "/ChapterOne_PriestApollo.controller",
        AnimationRoot + "/ChapterOne_FireKeeper.controller"
    };

    [MenuItem("Tools/TheTroyGame/Art/Build Missing Chapter I Art")]
    public static void BuildMissing()
    {
        if (!AssetDatabase.IsValidFolder(SourceRoot))
        {
            Debug.LogWarning("Troy characters: KayKit submodule is not checked out. No art candidates were generated.");
            return;
        }

        bool changed = false;

        List<string> missingCore = FindMissing<GameObject>(CoreCharacterPrefabs);
        if (missingCore.Count > 0)
        {
            Debug.LogWarning("Troy characters: missing core Chapter I prefab(s):\n" + string.Join("\n", missingCore) +
                             "\nRebuilding the core candidate set.");
            CartoonCharacterPrefabBuilder.BuildAll();
            changed = true;
        }

        List<string> missingSupport = FindMissing<GameObject>(SupportCharacterPrefabs);
        if (missingSupport.Count > 0)
        {
            Debug.LogWarning("Troy characters: missing support Chapter I prefab(s):\n" + string.Join("\n", missingSupport) +
                             "\nRebuilding the support candidate set.");
            MythicAndSupportArtCandidateBuilder.BuildAll();
            changed = true;
        }

        List<string> missingAnimations = FindMissing<RuntimeAnimatorController>(AnimationControllers);
        if (missingAnimations.Count > 0)
        {
            Debug.LogWarning("Troy characters: missing Chapter I animation controller(s):\n" + string.Join("\n", missingAnimations) +
                             "\nRebuilding all role-specific animation profiles.");
            ChapterOneCharacterAnimationBuilder.BuildAll();
            changed = true;
        }

        if (HectorProductionVisualRefinementBuilder.ApplyIfAvailable())
        {
            Debug.Log("Troy characters: applied Hector-specific rig-following production silhouette refinement.");
            changed = true;
        }

        if (HectorProductionAnimationBinder.ApplyIfAvailable())
        {
            Debug.Log("Troy characters: applied Hector-specific KayKit production animation candidates.");
            changed = true;
        }

        ReportMissingAfterBuild();

        if (!changed)
            Debug.Log("Troy characters: Chapter I generated art candidates are already present. Nothing to build.");
    }

    [MenuItem("Tools/TheTroyGame/Art/Validate Generated Chapter I Art")]
    public static void ValidateGeneratedArt()
    {
        ReportMissingAfterBuild();
    }

    static List<string> FindMissing<T>(string[] paths) where T : Object
    {
        var missing = new List<string>();
        foreach (string path in paths)
            if (AssetDatabase.LoadAssetAtPath<T>(path) == null) missing.Add(path);
        return missing;
    }

    static void ReportMissingAfterBuild()
    {
        var missing = new List<string>();
        missing.AddRange(FindMissing<GameObject>(CoreCharacterPrefabs));
        missing.AddRange(FindMissing<GameObject>(SupportCharacterPrefabs));
        missing.AddRange(FindMissing<RuntimeAnimatorController>(AnimationControllers));

        if (missing.Count == 0)
        {
            Debug.Log("Troy generated-art validation: all required Chapter I character prefabs and animation controllers are present.");
            return;
        }

        Debug.LogError("Troy generated-art validation: " + missing.Count + " required Chapter I asset(s) are still missing:\n" +
                       string.Join("\n", missing));
    }
}
