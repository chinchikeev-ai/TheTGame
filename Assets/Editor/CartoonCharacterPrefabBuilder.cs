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
        BuildTrojanReferenceUnits(candidates);
        BuildHero(candidates, TroyHeroId.Hector, new[] { "knight", "barbarian", "warrior" }, new Color(.72f, .48f, .12f));
        BuildHero(candidates, TroyHeroId.Achilles, new[] { "barbarian", "knight", "warrior" }, new Color(.80f, .65f, .22f));
        BuildHero(candidates, TroyHeroId.Menelaus, new[] { "knight", "warrior", "barbarian" }, new Color(.40f, .55f, .78f));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Built faction-specific cartoon prefabs with modular silhouettes and hero equipment.");
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

            float scale = archetype == EnemyArchetype.Boss ? 1.22f : archetype == EnemyArchetype.Runner ? .94f : 1f;
            Color tint = archetype == EnemyArchetype.Boss ? new Color(.32f, .45f, .72f) : new Color(.46f, .57f, .72f);
            GameObject root = CreateCharacterRoot(source, EnemyVisualFactory.GetPrefabName(archetype), tint, scale,
                archetype == EnemyArchetype.Boss ? 2.25f : 1.8f,
                archetype == EnemyArchetype.Boss ? .46f : .35f);

            CharacterVisualIdentity identity = root.AddComponent<CharacterVisualIdentity>();
            identity.faction = TroyFaction.Greek;
            identity.role = RoleFor(archetype);
            identity.characterId = archetype.ToString();

            switch (archetype)
            {
                case EnemyArchetype.Runner:
                    AttachAccessory(root, "dagger", Hand(root, true), Vector3.zero, Quaternion.identity, .9f);
                    identity.primaryWeapon = "dagger";
                    break;
                case EnemyArchetype.HeavyHoplite:
                    AttachAccessory(root, "sword_2handed", Hand(root, true), Vector3.zero, Quaternion.identity, 1f);
                    AttachAccessory(root, "shield_badge", Hand(root, false), Vector3.zero, Quaternion.Euler(0f, 90f, 0f), 1.08f);
                    identity.primaryWeapon = "two-handed sword";
                    identity.offhandItem = "badge shield";
                    break;
                case EnemyArchetype.ShieldBearer:
                    AttachProceduralSpear(root, Hand(root, true), 1.15f);
                    AttachAccessory(root, "shield_round", Hand(root, false), Vector3.zero, Quaternion.Euler(0f, 90f, 0f), 1.12f);
                    identity.primaryWeapon = "spear";
                    identity.offhandItem = "round shield";
                    break;
                case EnemyArchetype.Archer:
                    AttachAccessory(root, "crossbow_2handed", Hand(root, true), Vector3.zero, Quaternion.identity, .95f);
                    AttachAccessory(root, "quiver", FindTorso(root), new Vector3(-.18f, .08f, -.12f), Quaternion.Euler(0f, 15f, 12f), .9f);
                    identity.primaryWeapon = "ranged weapon";
                    identity.offhandItem = "quiver";
                    break;
                case EnemyArchetype.Boss:
                    AttachAccessory(root, "sword_2handed_color", Hand(root, true), Vector3.zero, Quaternion.identity, 1.08f);
                    AttachAccessory(root, "shield_spikes_color", Hand(root, false), Vector3.zero, Quaternion.Euler(0f, 90f, 0f), 1.18f);
                    AddHeroCrest(root, new Color(.34f, .48f, .78f));
                    identity.role = TroyVisualRole.Commander;
                    identity.primaryWeapon = "hero sword";
                    identity.offhandItem = "spiked shield";
                    break;
                default:
                    AttachProceduralSpear(root, Hand(root, true), 1.05f);
                    AttachAccessory(root, "shield_round_color", Hand(root, false), Vector3.zero, Quaternion.Euler(0f, 90f, 0f), 1f);
                    identity.primaryWeapon = "spear";
                    identity.offhandItem = "round shield";
                    break;
            }

            SaveRoot(root, GreekOutputRoot);
        }
    }

    static void BuildTrojanReferenceUnits(List<GameObject> candidates)
    {
        GameObject infantry = PickByHints(candidates, new[] { "knight", "barbarian", "warrior" });
        GameObject archer = PickByHints(candidates, new[] { "ranger", "rogue", "archer" });
        if (infantry != null)
        {
            GameObject trojanInfantry = CreateCharacterRoot(infantry, "Trojan_Infantry", new Color(.72f, .38f, .18f), 1f, 1.8f, .35f);
            AddIdentity(trojanInfantry, TroyFaction.Trojan, TroyVisualRole.Infantry, "Trojan_Infantry", "spear", "round shield");
            AttachProceduralSpear(trojanInfantry, Hand(trojanInfantry, true), 1.05f);
            AttachAccessory(trojanInfantry, "shield_round_barbarian", Hand(trojanInfantry, false), Vector3.zero, Quaternion.Euler(0f, 90f, 0f), 1.02f);
            SaveRoot(trojanInfantry, TrojanOutputRoot);

            GameObject trojanGuard = CreateCharacterRoot(infantry, "Trojan_Guard", new Color(.82f, .56f, .20f), 1.08f, 1.92f, .39f);
            AddIdentity(trojanGuard, TroyFaction.Trojan, TroyVisualRole.ShieldBearer, "Trojan_Guard", "sword", "square shield");
            AttachAccessory(trojanGuard, "sword_1handed", Hand(trojanGuard, true), Vector3.zero, Quaternion.identity, 1f);
            AttachAccessory(trojanGuard, "shield_square_color", Hand(trojanGuard, false), Vector3.zero, Quaternion.Euler(0f, 90f, 0f), 1.12f);
            AddHeroCrest(trojanGuard, new Color(.75f, .22f, .12f));
            SaveRoot(trojanGuard, TrojanOutputRoot);
        }
        if (archer != null)
        {
            GameObject trojanArcher = CreateCharacterRoot(archer, "Trojan_Archer", new Color(.66f, .34f, .16f), .98f, 1.8f, .35f);
            AddIdentity(trojanArcher, TroyFaction.Trojan, TroyVisualRole.Archer, "Trojan_Archer", "ranged weapon", "quiver");
            AttachAccessory(trojanArcher, "crossbow_2handed", Hand(trojanArcher, true), Vector3.zero, Quaternion.identity, .92f);
            AttachAccessory(trojanArcher, "quiver", FindTorso(trojanArcher), new Vector3(-.18f, .08f, -.12f), Quaternion.Euler(0f, 15f, 12f), .9f);
            SaveRoot(trojanArcher, TrojanOutputRoot);
        }
    }

    static void BuildHero(List<GameObject> candidates, TroyHeroId heroId, string[] hints, Color tint)
    {
        GameObject source = PickByHints(candidates, hints);
        if (source == null) return;

        float scale = heroId == TroyHeroId.Achilles ? 1.22f : heroId == TroyHeroId.Menelaus ? 1.17f : 1.19f;
        GameObject root = CreateCharacterRoot(source, HeroVisualFactory.GetPrefabName(heroId), tint, scale, 2.12f, .44f);
        TroyFaction faction = heroId == TroyHeroId.Hector ? TroyFaction.Trojan : TroyFaction.Greek;
        CharacterVisualIdentity identity = AddIdentity(root, faction, TroyVisualRole.Hero, heroId.ToString(), "", "");

        switch (heroId)
        {
            case TroyHeroId.Hector:
                AttachProceduralSpear(root, Hand(root, true), 1.20f);
                AttachAccessory(root, "shield_square_color", Hand(root, false), Vector3.zero, Quaternion.Euler(0f, 90f, 0f), 1.12f);
                AddHeroCrest(root, new Color(.72f, .18f, .12f));
                identity.primaryWeapon = "long spear";
                identity.offhandItem = "Trojan shield";
                break;
            case TroyHeroId.Achilles:
                AttachAccessory(root, "sword_1handed", Hand(root, true), Vector3.zero, Quaternion.identity, 1.08f);
                AttachAccessory(root, "shield_round_color", Hand(root, false), Vector3.zero, Quaternion.Euler(0f, 90f, 0f), 1.08f);
                AddHeroCrest(root, new Color(.86f, .63f, .18f));
                identity.primaryWeapon = "hero sword";
                identity.offhandItem = "round shield";
                break;
            case TroyHeroId.Menelaus:
                AttachAccessory(root, "sword_2handed", Hand(root, true), Vector3.zero, Quaternion.identity, 1.05f);
                AttachAccessory(root, "shield_badge_color", Hand(root, false), Vector3.zero, Quaternion.Euler(0f, 90f, 0f), 1.10f);
                AddHeroCrest(root, new Color(.28f, .42f, .72f));
                identity.primaryWeapon = "command sword";
                identity.offhandItem = "royal shield";
                break;
        }

        SaveRoot(root, HeroOutputRoot);
    }

    static GameObject CreateCharacterRoot(GameObject source, string prefabName, Color tint, float visualScale, float colliderHeight, float colliderRadius)
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
        return root;
    }

    static CharacterVisualIdentity AddIdentity(GameObject root, TroyFaction faction, TroyVisualRole role, string id, string primary, string offhand)
    {
        CharacterVisualIdentity identity = root.AddComponent<CharacterVisualIdentity>();
        identity.faction = faction;
        identity.role = role;
        identity.characterId = id;
        identity.primaryWeapon = primary;
        identity.offhandItem = offhand;
        return identity;
    }

    static TroyVisualRole RoleFor(EnemyArchetype archetype)
    {
        switch (archetype)
        {
            case EnemyArchetype.Runner: return TroyVisualRole.Runner;
            case EnemyArchetype.HeavyHoplite: return TroyVisualRole.Heavy;
            case EnemyArchetype.ShieldBearer: return TroyVisualRole.ShieldBearer;
            case EnemyArchetype.Archer: return TroyVisualRole.Archer;
            case EnemyArchetype.Boss: return TroyVisualRole.Commander;
            default: return TroyVisualRole.Infantry;
        }
    }

    static void AttachProceduralSpear(GameObject root, Transform parent, float scale)
    {
        if (parent == null) parent = root.transform;
        GameObject spear = new GameObject("Spear");
        spear.transform.SetParent(parent, false);
        spear.transform.localPosition = new Vector3(0f, -.05f, .08f);
        spear.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        spear.transform.localScale = Vector3.one * scale;

        GameObject shaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        shaft.name = "Shaft";
        shaft.transform.SetParent(spear.transform, false);
        shaft.transform.localScale = new Vector3(.025f, .72f, .025f);
        DestroyCollider(shaft);
        TowerFactory.SetColor(shaft, new Color(.34f, .20f, .10f));

        GameObject tip = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tip.name = "BronzeTip";
        tip.transform.SetParent(spear.transform, false);
        tip.transform.localPosition = new Vector3(0f, .78f, 0f);
        tip.transform.localScale = new Vector3(.06f, .16f, .025f);
        tip.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
        DestroyCollider(tip);
        TowerFactory.SetColor(tip, new Color(.72f, .50f, .20f));
    }

    static void AddHeroCrest(GameObject root, Color color)
    {
        Transform head = FindTransform(root, new[] { "head", "mixamorig:head", "headtop_end" });
        if (head == null) head = root.transform;
        GameObject crest = GameObject.CreatePrimitive(PrimitiveType.Cube);
        crest.name = "HeroCrest";
        crest.transform.SetParent(head, false);
        crest.transform.localPosition = new Vector3(0f, .22f, 0f);
        crest.transform.localScale = new Vector3(.10f, .34f, .06f);
        DestroyCollider(crest);
        TowerFactory.SetColor(crest, color);
    }

    static void AttachAccessory(GameObject root, string nameHint, Transform parent, Vector3 localPosition, Quaternion localRotation, float scale)
    {
        GameObject asset = FindAccessory(nameHint);
        if (asset == null) return;
        if (parent == null) parent = root.transform;

        GameObject item = PrefabUtility.InstantiatePrefab(asset) as GameObject;
        if (item == null) item = Object.Instantiate(asset);
        item.name = "Gear_" + nameHint;
        item.transform.SetParent(parent, false);
        item.transform.localPosition = localPosition;
        item.transform.localRotation = localRotation;
        item.transform.localScale = Vector3.one * scale;
        foreach (Collider collider in item.GetComponentsInChildren<Collider>(true)) Object.DestroyImmediate(collider);
    }

    static GameObject FindAccessory(string nameHint)
    {
        string[] guids = AssetDatabase.FindAssets(nameHint + " t:GameObject", new[] { ThirdPartyRoot });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.EndsWith(".fbx") && !path.EndsWith(".prefab")) continue;
            string lower = Path.GetFileNameWithoutExtension(path).ToLowerInvariant();
            if (!lower.Contains(nameHint.ToLowerInvariant())) continue;
            GameObject item = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (item != null) return item;
        }
        return null;
    }

    static Transform Hand(GameObject root, bool right)
    {
        string[] hints = right
            ? new[] { "righthand", "right_hand", "hand_r", "mixamorig:righthand", "hand.r" }
            : new[] { "lefthand", "left_hand", "hand_l", "mixamorig:lefthand", "hand.l" };
        return FindTransform(root, hints) ?? root.transform;
    }

    static Transform FindTorso(GameObject root)
    {
        return FindTransform(root, new[] { "spine2", "spine_02", "chest", "upperchest", "mixamorig:spine2" }) ?? root.transform;
    }

    static Transform FindTransform(GameObject root, string[] hints)
    {
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        foreach (string hint in hints)
        {
            string wanted = hint.ToLowerInvariant();
            foreach (Transform transform in all)
                if (transform.name.ToLowerInvariant().Contains(wanted)) return transform;
        }
        return null;
    }

    static void DestroyCollider(GameObject obj)
    {
        Collider collider = obj.GetComponent<Collider>();
        if (collider != null) Object.DestroyImmediate(collider);
    }

    static void SaveRoot(GameObject root, string outputRoot)
    {
        string path = Path.Combine(outputRoot, root.name + ".prefab").Replace('\\', '/');
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
