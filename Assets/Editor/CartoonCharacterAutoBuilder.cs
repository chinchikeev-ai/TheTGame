using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class CartoonCharacterAutoBuilder
{
    const string SourceRoot = "Assets/ThirdParty/KayKitAdventurers";
    const string ProbePrefab = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters/Greek/Enemy_Infantry.prefab";
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

        if (AssetDatabase.LoadAssetAtPath<GameObject>(ProbePrefab) != null) return;

        Debug.Log("Troy characters: KayKit source found, generating Chapter I production candidates automatically.");
        CartoonCharacterPrefabBuilder.BuildAll();
    }
}
