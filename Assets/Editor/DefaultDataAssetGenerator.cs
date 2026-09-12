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
        EditorApplication.delayCall += EnsureAssets;
    }

    [MenuItem("TheTroyGame/Data/Create or Refresh Default Assets")]
    public static void EnsureAssets()
    {
        EnsureFolder("Assets/Resources");
        EnsureFolder(Root);
        EnsureFolder(Root + "/Towers");
        EnsureFolder(Root + "/Enemies");
        EnsureFolder(Root + "/Waves");

        foreach (TowerType type in System.Enum.GetValues(typeof(TowerType))) CreateTower(type);
        foreach (EnemyArchetype type in System.Enum.GetValues(typeof(EnemyArchetype))) CreateEnemy(type);
        for (int wave = 1; wave <= 5; wave++) CreateWave(wave);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void CreateTower(TowerType type)
    {
        string path = $"{Root}/Towers/{type}.asset";
        TowerData source = BalanceCatalog.GetTowerRuntimeDefault(type);
        TowerData asset = AssetDatabase.LoadAssetAtPath<TowerData>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<TowerData>();
            AssetDatabase.CreateAsset(asset, path);
        }
        EditorUtility.CopySerialized(source, asset);
        EditorUtility.SetDirty(asset);
    }

    static void CreateEnemy(EnemyArchetype type)
    {
        string path = $"{Root}/Enemies/{type}.asset";
        EnemyData source = BalanceCatalog.GetEnemyRuntimeDefault(type);
        EnemyData asset = AssetDatabase.LoadAssetAtPath<EnemyData>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<EnemyData>();
            AssetDatabase.CreateAsset(asset, path);
        }
        EditorUtility.CopySerialized(source, asset);
        EditorUtility.SetDirty(asset);
    }

    static void CreateWave(int wave)
    {
        string path = $"{Root}/Waves/Wave_{wave:00}.asset";
        WaveData source = BalanceCatalog.GetWaveRuntimeDefault(wave, 5);
        WaveData asset = AssetDatabase.LoadAssetAtPath<WaveData>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<WaveData>();
            AssetDatabase.CreateAsset(asset, path);
        }
        EditorUtility.CopySerialized(source, asset);
        EditorUtility.SetDirty(asset);
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
