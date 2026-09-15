using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ChapterOneUrpMaterialRepair
{
    const string CharacterRoot = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters";

    [MenuItem("The Troy Game/Characters/Repair Chapter I URP Materials")]
    public static void RepairMenu()
    {
        RepairAll(true);
    }

    public static int RepairAll(bool logSummary = true)
    {
        if (!AssetDatabase.IsValidFolder(CharacterRoot)) return 0;

        int repaired = 0;
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { CharacterRoot });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                if (root.GetComponent<CharacterUrpMaterialAdapter>() != null) continue;
                root.AddComponent<CharacterUrpMaterialAdapter>();
                PrefabUtility.SaveAsPrefabAsset(root, path);
                repaired++;
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        if (repaired > 0)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        if (logSummary)
            Debug.Log("Chapter I URP material repair added runtime material adapters to " + repaired + " prefab(s). Non-URP source materials will be converted at runtime while preserving base textures/colors.");

        return repaired;
    }

    public static List<string> CollectProblems()
    {
        var problems = new List<string>();
        if (!AssetDatabase.IsValidFolder(CharacterRoot))
        {
            problems.Add("Missing character root: " + CharacterRoot);
            return problems;
        }

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { CharacterRoot });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                if (root.GetComponent<CharacterUrpMaterialAdapter>() == null)
                    problems.Add(path + ": missing CharacterUrpMaterialAdapter.");
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
        return problems;
    }
}
