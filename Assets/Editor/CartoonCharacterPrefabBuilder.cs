using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class CartoonCharacterPrefabBuilder
{
    const string ThirdPartyRoot = "Assets/ThirdParty/KayKitAdventurers";
    const string BaseOutputRoot = "Assets/Resources/TroyCharacters";
    const string GreekOutputRoot = BaseOutputRoot + "/Factions/Greek";
    const string TrojanOutputRoot = BaseOutputRoot + "/Factions/Trojan";
    const string HeroOutputRoot = BaseOutputRoot + "/Heroes";

    static readonly Dictionary<EnemyArchetype, string[]> Hints = new Dictionary<EnemyArchetype, string[]>
    {
        { EnemyArchetype.Infantry, new[] { "knight", "barbarian", "rogue" } },
        { EnemyArchetype.Runner, new[] { "rogue", "ranger", "barbarian" } },
        { EnemyArchetype.HeavyHoplite, new[] { "knight", "warrior", "barbarian" } },
        { EnemyArchetype.ShieldBearer, new[] { "knight", "warrior", "barbarian" } },
        { EnemyArchetype.Archer, new[] { "ranger", "rogue", "archer" } },
        { EnemyArchetype.Boss, new[] { "knight", "barbarian", "warrior" } },
    };

    [MenuItem("The Troy Game/Characters/Build All Cartoon Prefabs")]
    public static void BuildAll()
    {
        if (!ValidateSource()) return;
        EnsureFolders();

        List<GameObject> candidates = FindCharacterModels();
        if (candidates.Count == 0)
        {
            Debug.LogError("No character models found in KayKit submodule. Ensure the submodule contents are checked out.");
            return;
        }

        BuildGreekEnemies(candidates);
        BuildHero(candidates, TroyHeroId.Hector, new[] { "knight", "barbarian", "warrior" }, new Color(.72f, .48f, .12f));
        BuildHero(candidates, TroyHeroId.Achilles, new[] { "barbarian", "knight", "warrior" }, new Color(.80f, .65f, .22f));
        BuildHero(candidates, TroyHeroId.Menelaus, new[] { "knight", "warrior", "barbarian" }, new Color(.40f, .55f, .78f));

        BuildTrojanReferenceUnits(candidates);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Built Greek enemy, Trojan reference and hero cartoon prefabs.");
    }

    [MenuItem("The Troy Game/Characters/Build Cartoon Enemy Prefabs")]
    public static void BuildEnemiesOnly()
    {
        if (!ValidateSource()) return;
        EnsureFolders();
        List<GameObject> candidates = FindCharacterModels();
        if (candidates.Count == 0) return;
        BuildGreekEnemies(candidates);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static bool ValidateSource()
    {
        if (AssetDatabase.IsValidFolder(ThirdPartyRoot)) return true;
        Debug.LogError("KayKit submodule is missing. Run: git submodule update --init --recursive");
        return false;
    }

    static void BuildGreekEnemies(List<GameObject> candidates)
    {
        foreach (EnemyArchetype archetype in System.Enum.GetValues(typeof(EnemyArchetype)))
        {
            if (archetype == EnemyArchetype.BatteringRam) continue;
            GameObject source = PickSource(candidates, archetype);
            if (source == null) continue;

            float scale = archetype == EnemyArchetype.Boss ? 1.18f : 1f;
            Color tint = archetype == EnemyArchetype.Boss
                ? new Color(.34f, .48f, .72f)
                : new Color(.48f, .58f, .72f);

            SaveCharacterPrefab(source, EnemyVisualFactory.GetPrefabName(archetype), GreekOutputRoot, tint, scale,
                archetype == EnemyArchetype.Boss ? 2.2f : 1.8f,
                archetype == EnemyArchetype.Boss ? .45f : .35f);
        }
    }

    static void BuildTrojanReferenceUnits(List<GameObject> candidates)
    {
        GameObject infantry = PickByHints(candidates, new[] { "knight", "barbarian", "warrior" });
        GameObject archer = PickByHints(candidates, new[] { "ranger", "rogue", "archer" });
        if (infantry != null)
        {
            SaveCharacterPrefab(infantry, "Trojan_Infantry", TrojanOutputRoot, new Color(.72f, .40f, .20f), 1f, 1.8f, .35f);
            SaveCharacterPrefab(infantry, "Trojan_Guard", TrojanOutputRoot, new Color(.82f, .58f, .22f), 1.05f, 1.9f, .38f);
        }
        if (archer != null)
            SaveCharacterPrefab(archer, "Trojan_Archer", TrojanOutputRoot, new Color(.68f, .36f, .18f), .98f, 1.8f, .35f);
    }

    static void BuildHero(List<GameObject> candidates, TroyHeroId heroId, string[] hints, Color tint)
    {
        GameObject source = PickByHints(candidates, hints);
        if (source == null) return;
        float scale = heroId == TroyHeroId.Hector ? 1.12f : 1.16f;
        SaveCharacterPrefab(source, HeroVisualFactory.GetPrefabName(heroId), HeroOutputRoot, tint, scale, 2.05f, .42f);
    }

    static void SaveCharacterPrefab(GameObject source, string prefabName, string outputRoot, Color tint, float visualScale, float colliderHeight, float colliderRadius)
    {
        GameObject root = new GameObject(prefabName);
        GameObject visual = PrefabUtility.InstantiatePrefab(source) as GameObject;
        if (visual == null) visual = Object.Instantiate(source);

        visual.name = "Visual";
        visual.transform.SetParent(root.transform, false);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.identity;
        visual.transform.localScale = Vector3.one * visualScale;

        ApplyTint(visual, tint);

        CapsuleCollider collider = root.AddComponent<CapsuleCollider>();
        collider.center = new Vector3(0f, colliderHeight * .5f, 0f);
        collider.height = colliderHeight;
        collider.radius = colliderRadius;

        string path = Path.Combine(outputRoot, prefabName + ".prefab").Replace('\\', '/');
        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
    }

    static void ApplyTint(GameObject root, Color tint)
    {
        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            Material[] materials = renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
            {
                Material source = materials[i];
                if (source == null) continue;
                Material clone = new Material(source) { name = source.name + "_TroyVariant" };
                if (clone.HasProperty("_BaseColor")) clone.SetColor("_BaseColor", Color.Lerp(clone.GetColor("_BaseColor"), tint, .32f));
                if (clone.HasProperty("_Color")) clone.SetColor("_Color", Color.Lerp(clone.GetColor("_Color"), tint, .32f));
                materials[i] = clone;
            }
            renderer.sharedMaterials = materials;
        }
    }

    static void EnsureFolders()
    {
        EnsureFolder("Assets/Resources");
        EnsureFolder(BaseOutputRoot);
        EnsureFolder(BaseOutputRoot + "/Factions");
        EnsureFolder(GreekOutputRoot);
        EnsureFolder(TrojanOutputRoot);
        EnsureFolder(HeroOutputRoot);
    }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = Path.GetDirectoryName(path).Replace('\\', '/');
        string name = Path.GetFileName(path);
        if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, name);
    }

    static List<GameObject> FindCharacterModels()
    {
        string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] { ThirdPartyRoot });
        var result = new List<GameObject>();
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.EndsWith(".fbx") && !path.EndsWith(".prefab")) continue;

            GameObject candidate = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (candidate == null) continue;

            bool hasSkinned = candidate.GetComponentInChildren<SkinnedMeshRenderer>(true) != null;
            bool hasAnimator = candidate.GetComponentInChildren<Animator>(true) != null;
            if (hasSkinned || hasAnimator) result.Add(candidate);
        }
        return result;
    }

    static GameObject PickSource(List<GameObject> candidates, EnemyArchetype archetype)
    {
        return Hints.TryGetValue(archetype, out string[] hints) ? PickByHints(candidates, hints) : candidates[0];
    }

    static GameObject PickByHints(List<GameObject> candidates, string[] hints)
    {
        foreach (string hint in hints)
        {
            GameObject match = candidates.Find(x => x.name.ToLowerInvariant().Contains(hint));
            if (match != null) return match;
        }
        return candidates.Count > 0 ? candidates[0] : null;
    }
}
