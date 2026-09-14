#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class DefaultDataAssetGenerator
{
    const string Root = "Assets/Resources/Data";

    static DefaultDataAssetGenerator()
    {
        // Low-level catalog assets may be created automatically for a fresh checkout.
        // Existing authored assets are NEVER overwritten here.
        // Chapter encounters are intentionally not regenerated: their GUID-backed references
        // and spawn plans are authored campaign content and must fail validation if missing.
        EditorApplication.delayCall += EnsureMissingAssets;
    }

    [MenuItem("TheTroyGame/Data/Create Missing Default Assets")]
    public static void EnsureMissingAssets()
    {
        EnsureFolder("Assets/Resources");
        EnsureFolder(Root);
        EnsureFolder(Root + "/Towers");
        EnsureFolder(Root + "/Enemies");
        EnsureFolder(Root + "/Encounters");

        foreach (TowerType type in System.Enum.GetValues(typeof(TowerType))) CreateTowerIfMissing(type);
        foreach (EnemyArchetype type in System.Enum.GetValues(typeof(EnemyArchetype))) CreateEnemyIfMissing(type);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void CreateTowerIfMissing(TowerType type)
    {
        string path = $"{Root}/Towers/{type}.asset";
        if (AssetDatabase.LoadAssetAtPath<TowerData>(path) != null) return;

        TowerData source = BalanceCatalog.GetTowerRuntimeDefault(type);
        TowerData asset = ScriptableObject.CreateInstance<TowerData>();
        EditorUtility.CopySerialized(source, asset);
        AssetDatabase.CreateAsset(asset, path);
        Object.DestroyImmediate(source);
        Debug.Log($"[DATA] Created missing TowerData: {path}");
    }

    static void CreateEnemyIfMissing(EnemyArchetype type)
    {
        string path = $"{Root}/Enemies/{type}.asset";
        if (AssetDatabase.LoadAssetAtPath<EnemyData>(path) != null) return;

        EnemyData source = BalanceCatalog.GetEnemyRuntimeDefault(type);
        EnemyData asset = ScriptableObject.CreateInstance<EnemyData>();
        EditorUtility.CopySerialized(source, asset);
        AssetDatabase.CreateAsset(asset, path);
        Object.DestroyImmediate(source);
        Debug.Log($"[DATA] Created missing EnemyData: {path}");
    }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = Path.GetDirectoryName(path).Replace('\\', '/');
        string name = Path.GetFileName(path);
        if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, name);
    }
}
#endif
