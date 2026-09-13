using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class CartoonCharacterAutoBuilder
{
    const string SourceRoot = "Assets/ThirdParty/KayKitAdventurers";
    const string ProbePrefab = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters/Greek/Enemy_Infantry.prefab";
    const string AnimationProfileProbe = "Assets/Game/Art/Characters/Animation/ChapterOne_Hector.controller";
    const string SessionKey = "TheTroyGame.CartoonCharacters.AutoBuildAttempted";

    static CartoonCharacterAutoBuilder()
    {
        EditorApplication.delayCall += TryBuild;
    }

    static void TryBuild()
    {
        if (SessionState.GetBool(SessionKey, false)) return;
        SessionState.SetBool(SessionKey, true);

        if (!AssetDatabase.IsValidFolder(SourceRoot))
        {
            Debug.LogWarning("Troy characters: KayKit submodule is not checked out. Using runtime fallback visuals until production candidates can be generated.");
            return;
        }

        if (AssetDatabase.LoadAssetAtPath<GameObject>(ProbePrefab) == null)
        {
            Debug.Log("Troy characters: KayKit source found, generating Chapter I production candidates automatically.");
            CartoonCharacterPrefabBuilder.BuildAll();
        }

        if (AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(AnimationProfileProbe) == null)
        {
            Debug.Log("Troy characters: building role-specific Chapter I animation profiles from imported KayKit clips.");
            ChapterOneCharacterAnimationBuilder.BuildAll();
        }
    }
}
