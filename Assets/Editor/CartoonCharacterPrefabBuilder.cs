using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class CartoonCharacterPrefabBuilder
{
    const string ThirdPartyRoot = "Assets/ThirdParty/KayKitAdventurers";
    const string OutputRoot = "Assets/Resources/TroyCharacters";

    static readonly Dictionary<EnemyArchetype, string[]> Hints = new Dictionary<EnemyArchetype, string[]>
    {
        { EnemyArchetype.Infantry, new[] { "knight", "barbarian", "rogue" } },
        { EnemyArchetype.Runner, new[] { "rogue", "ranger", "barbarian" } },
        { EnemyArchetype.HeavyHoplite, new[] { "knight", "warrior", "barbarian" } },
        { EnemyArchetype.ShieldBearer, new[] { "knight", "warrior", "barbarian" } },
        { EnemyArchetype.Archer, new[] { "ranger", "rogue", "archer" } },
        { EnemyArchetype.Boss, new[] { "knight", "barbarian", "warrior" } },
    };

    [MenuItem("The Troy Game/Characters/Build Cartoon Enemy Prefabs")]
    public static void Build()
    {
        if (!AssetDatabase.IsValidFolder(ThirdPartyRoot))
        {
            Debug.LogError("KayKit submodule is missing. Run: git submodule update --init --recursive");
            return;
        }

        EnsureOutputFolder();
        List<GameObject> candidates = FindCharacterModels();
        if (candidates.Count == 0)
        {
            Debug.LogError("No character models found in KayKit submodule. Ensure the submodule contents are checked out.");
            return;
        }

        foreach (EnemyArchetype archetype in System.Enum.GetValues(typeof(EnemyArchetype)))
        {
            if (archetype == EnemyArchetype.BatteringRam)
                continue;

            GameObject source = PickSource(candidates, archetype);
            if (source == null)
                continue;

            GameObject root = new GameObject(EnemyVisualFactory.GetPrefabName(archetype));
            GameObject visual = PrefabUtility.InstantiatePrefab(source) as GameObject;
            if (visual == null)
                visual = Object.Instantiate(source);

            visual.name = "Visual";
            visual.transform.SetParent(root.transform, false);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one;

            CapsuleCollider collider = root.AddComponent<CapsuleCollider>();
            collider.center = new Vector3(0f, 0.9f, 0f);
            collider.height = archetype == EnemyArchetype.Boss ? 2.2f : 1.8f;
            collider.radius = archetype == EnemyArchetype.Boss ? 0.45f : 0.35f;

            string path = Path.Combine(OutputRoot, root.name + ".prefab").Replace('\\', '/');
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Cartoon enemy prefabs built into Assets/Resources/TroyCharacters.");
    }

    static void EnsureOutputFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        if (!AssetDatabase.IsValidFolder(OutputRoot))
            AssetDatabase.CreateFolder("Assets/Resources", "TroyCharacters");
    }

    static List<GameObject> FindCharacterModels()
    {
        string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] { ThirdPartyRoot });
        var result = new List<GameObject>();
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.EndsWith(".fbx") && !path.EndsWith(".prefab"))
                continue;

            GameObject candidate = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (candidate == null)
                continue;

            bool hasSkinned = candidate.GetComponentInChildren<SkinnedMeshRenderer>(true) != null;
            bool hasAnimator = candidate.GetComponentInChildren<Animator>(true) != null;
            if (hasSkinned || hasAnimator)
                result.Add(candidate);
        }
        return result;
    }

    static GameObject PickSource(List<GameObject> candidates, EnemyArchetype archetype)
    {
        if (Hints.TryGetValue(archetype, out string[] hints))
        {
            foreach (string hint in hints)
            {
                GameObject match = candidates.Find(x => x.name.ToLowerInvariant().Contains(hint));
                if (match != null)
                    return match;
            }
        }
        return candidates[0];
    }
}
