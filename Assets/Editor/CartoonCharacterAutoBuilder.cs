using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class CartoonCharacterAutoBuilder
{
    const string SourceRoot = "Assets/ThirdParty/KayKitAdventurers";
    const string ProbePrefab = "Assets/Resources/TroyCharacters/Factions/Greek/Enemy_Infantry.prefab";
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
            Debug.LogWarning("Troy characters: KayKit submodule is not checked out. Using procedural warrior visuals until assets are available.");
            return;
        }

        if (AssetDatabase.LoadAssetAtPath<GameObject>(ProbePrefab) != null) return;

        Debug.Log("Troy characters: KayKit source found, generating character prefabs automatically.");
        CartoonCharacterPrefabBuilder.BuildAll();
    }
}
