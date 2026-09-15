using UnityEditor;
using UnityEngine;

public static class CartoonCharacterAutoBuilder
{
    const string SourceRoot = "Assets/ThirdParty/KayKitAdventurers";
    const string ProbePrefab = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters/Greek/Enemy_Infantry.prefab";
    const string SupportProbePrefab = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters/Trojan/Trojan_BallistaCrew.prefab";
    const string AnimationProfileProbe = "Assets/Game/Art/Characters/Animation/ChapterOne_BallistaCrew.controller";
    const string HectorAnimationProfileProbe = "Assets/Game/Art/Characters/Animation/ChapterOne_Hector.controller";

    [MenuItem("Tools/TheTroyGame/Art/Build Missing Chapter I Art")]
    public static void BuildMissing()
    {
        if (!AssetDatabase.IsValidFolder(SourceRoot))
        {
            Debug.LogWarning("Troy characters: KayKit submodule is not checked out. No art candidates were generated.");
            return;
        }

        bool changed = false;

        if (AssetDatabase.LoadAssetAtPath<GameObject>(ProbePrefab) == null)
        {
            Debug.Log("Troy characters: generating missing Chapter I production candidates.");
            CartoonCharacterPrefabBuilder.BuildAll();
            changed = true;
        }

        if (AssetDatabase.LoadAssetAtPath<GameObject>(SupportProbePrefab) == null)
        {
            Debug.Log("Troy characters: generating missing Chapter I Trojan support candidates.");
            MythicAndSupportArtCandidateBuilder.BuildAll();
            changed = true;
        }

        if (AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(AnimationProfileProbe) == null ||
            AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(HectorAnimationProfileProbe) == null)
        {
            Debug.Log("Troy characters: building missing role-specific Chapter I animation profiles.");
            ChapterOneCharacterAnimationBuilder.BuildAll();
            changed = true;
        }

        if (HectorProductionVisualRefinementBuilder.ApplyIfAvailable())
        {
            Debug.Log("Troy characters: applied Hector-specific rig-following production silhouette refinement.");
            changed = true;
        }

        if (MenelausProductionVisualRefinementBuilder.ApplyIfAvailable())
        {
            Debug.Log("Troy characters: applied Menelaus-specific boss/hero production silhouette refinement.");
            changed = true;
        }

        if (HectorProductionAnimationBinder.ApplyIfAvailable())
        {
            Debug.Log("Troy characters: applied Hector-specific KayKit production animation candidates.");
            changed = true;
        }

        if (!changed)
            Debug.Log("Troy characters: Chapter I generated art candidates are already present. Nothing to build.");
    }
}
