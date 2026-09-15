using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ChapterOneShieldCandidateBuilder
{
    const string CharacterRoot = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters";
    const string GreekRoot = CharacterRoot + "/Greek/";
    const string TrojanRoot = CharacterRoot + "/Trojan/";
    const string HeroRoot = CharacterRoot + "/Heroes/";
    const string HectorPrefabPath = HeroRoot + "Hero_Hector.prefab";
    const string RoundShieldPath = "Assets/Game/Art/Characters/Equipment/AegeanRoundShield.obj";
    const string FigureEightShieldPath = "Assets/Game/Art/Characters/Equipment/FigureEightTowerShield.obj";
    const string SourceShieldName = "SourceShield_LateBronzeAge";

    struct ShieldTarget
    {
        public string prefabPath;
        public bool figureEight;
        public float bodyHeightRatio;
        public bool horseEmblem;

        public ShieldTarget(string prefabPath, bool figureEight, float bodyHeightRatio, bool horseEmblem = false)
        {
            this.prefabPath = prefabPath;
            this.figureEight = figureEight;
            this.bodyHeightRatio = bodyHeightRatio;
            this.horseEmblem = horseEmblem;
        }
    }

    static readonly ShieldTarget[] Targets =
    {
        new ShieldTarget(GreekRoot + "Enemy_Infantry.prefab", false, .56f),
        new ShieldTarget(GreekRoot + "Enemy_ShieldBearer.prefab", false, .64f),
        new ShieldTarget(GreekRoot + "Enemy_HeavyHoplite.prefab", true, .70f),
        new ShieldTarget(GreekRoot + "Enemy_Boss.prefab", false, .62f),
        new ShieldTarget(TrojanRoot + "Trojan_Infantry.prefab", false, .56f),
        new ShieldTarget(TrojanRoot + "Trojan_Guard.prefab", true, .70f),
        new ShieldTarget(HectorPrefabPath, false, .62f, true),
        new ShieldTarget(HeroRoot + "Hero_Menelaus.prefab", false, .60f)
    };

    [MenuItem("The Troy Game/Characters/Bind Late Bronze Age Shield Candidates")]
    public static void Build()
    {
        GameObject round = AssetDatabase.LoadAssetAtPath<GameObject>(RoundShieldPath);
        GameObject figureEight = AssetDatabase.LoadAssetAtPath<GameObject>(FigureEightShieldPath);
        if (round == null) throw new InvalidOperationException("Authored round shield candidate is missing: " + RoundShieldPath);
        if (figureEight == null) throw new InvalidOperationException("Authored figure-eight shield candidate is missing: " + FigureEightShieldPath);

        int upgraded = 0;
        foreach (ShieldTarget target in Targets)
            if (Upgrade(target, target.figureEight ? figureEight : round)) upgraded++;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Chapter I Late Bronze Age shield pass upgraded " + upgraded + " prefab(s). Shields are bound to resolved left-hand rig bones and normalized against character height; real animation-clearance QA is still required.");
    }

    public static bool ApplyHectorIfAvailable(bool logIfMissing = false)
    {
        GameObject round = AssetDatabase.LoadAssetAtPath<GameObject>(RoundShieldPath);
        if (round == null)
        {
            if (logIfMissing) Debug.LogWarning("Hector shield recovery skipped because authored round shield is missing: " + RoundShieldPath);
            return false;
        }

        if (AssetDatabase.LoadAssetAtPath<GameObject>(HectorPrefabPath) == null)
        {
            if (logIfMissing) Debug.LogWarning("Hector shield recovery skipped because prefab is missing: " + HectorPrefabPath);
            return false;
        }

        foreach (ShieldTarget target in Targets)
        {
            if (!string.Equals(target.prefabPath, HectorPrefabPath, StringComparison.Ordinal)) continue;
            bool upgraded = Upgrade(target, round);
            if (upgraded)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            return upgraded;
        }

        return false;
    }

    static bool Upgrade(ShieldTarget target, GameObject shieldSource)
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(target.prefabPath) == null)
        {
            Debug.LogWarning("Shield pass skipped missing prefab: " + target.prefabPath);
            return false;
        }

        GameObject root = PrefabUtility.LoadPrefabContents(target.prefabPath);
        try
        {
            Transform leftHand = ResolveHand(root, false);
            if (leftHand == null)
            {
                Debug.LogWarning("Shield pass cannot resolve left-hand rig bone; refusing root-space fallback: " + target.prefabPath);
                return false;
            }

            Transform previousSource = FindByName(root, SourceShieldName);
            Transform oldShield = FindGeneratedShield(root);
            Transform poseSource = oldShield != null ? oldShield : previousSource;
            Vector3 localPosition = Vector3.zero;
            Quaternion localRotation = Quaternion.Euler(0f, 90f, 0f);
            if (poseSource != null && poseSource.parent == leftHand)
            {
                localPosition = poseSource.localPosition;
                localRotation = poseSource.localRotation;
            }

            float bodyHeight = MeasureBodyHeight(root);
            if (bodyHeight <= .25f) bodyHeight = 1.8f;

            RemoveByName(root, SourceShieldName);
            RemoveGeneratedShield(root);

            GameObject shield = PrefabUtility.InstantiatePrefab(shieldSource) as GameObject;
            if (shield == null) shield = UnityEngine.Object.Instantiate(shieldSource);
            if (shield == null) throw new InvalidOperationException("Unable to instantiate authored shield candidate.");

            shield.name = SourceShieldName;
            shield.transform.SetParent(leftHand, false);
            shield.transform.localPosition = localPosition;
            shield.transform.localRotation = localRotation;
            shield.transform.localScale = Vector3.one;
            NormalizeLongestDimension(shield, bodyHeight * target.bodyHeightRatio);

            foreach (Collider collider in shield.GetComponentsInChildren<Collider>(true))
                UnityEngine.Object.DestroyImmediate(collider);

            Renderer[] renderers = shield.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
                throw new InvalidOperationException("Authored shield candidate imported without a Renderer.");

            Color tint = target.figureEight ? new Color(.46f, .24f, .09f) : new Color(.64f, .42f, .16f);
            foreach (Renderer renderer in renderers)
                TowerFactory.SetColor(renderer.gameObject, tint);

            if (target.horseEmblem) AddHorseEmblem(shield.transform);

            PrefabUtility.SaveAsPrefabAsset(root, target.prefabPath);
            return true;
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    static Transform ResolveHand(GameObject root, bool right)
    {
        Animator animator = root.GetComponentInChildren<Animator>(true);
        if (animator != null && animator.avatar != null && animator.avatar.isHuman)
        {
            Transform humanoid = animator.GetBoneTransform(right ? HumanBodyBones.RightHand : HumanBodyBones.LeftHand);
            if (humanoid != null) return humanoid;
        }

        return FindRigBone(root, right
            ? new[] { "righthand", "handr", "rhand" }
            : new[] { "lefthand", "handl", "lhand" });
    }

    static Transform FindRigBone(GameObject root, string[] hints)
    {
        var bones = new HashSet<Transform>();
        foreach (SkinnedMeshRenderer renderer in root.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            if (renderer == null) continue;
            foreach (Transform bone in renderer.bones)
                if (bone != null) bones.Add(bone);
        }

        foreach (string hint in hints)
        {
            string wanted = Normalize(hint);
            foreach (Transform bone in bones)
            {
                string candidate = Normalize(bone.name);
                if (candidate == wanted || candidate.EndsWith(wanted, StringComparison.Ordinal)) return bone;
            }
        }

        foreach (string hint in hints)
        {
            string wanted = Normalize(hint);
            foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
            {
                if (transform == null || transform == root.transform) continue;
                string candidate = Normalize(transform.name);
                if (candidate == wanted || candidate.EndsWith(wanted, StringComparison.Ordinal)) return transform;
            }
        }
        return null;
    }

    static float MeasureBodyHeight(GameObject root)
    {
        Transform visual = root.transform.Find("Visual");
        GameObject target = visual != null ? visual.gameObject : root;
        SkinnedMeshRenderer[] renderers = target.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        bool hasBounds = false;
        Bounds bounds = default;
        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            if (renderer == null) continue;
            if (!hasBounds)
            {
                bounds = renderer.bounds;
                hasBounds = true;
            }
            else bounds.Encapsulate(renderer.bounds);
        }
        return hasBounds ? bounds.size.y : 0f;
    }

    static void NormalizeLongestDimension(GameObject item, float targetSize)
    {
        Renderer[] renderers = item.GetComponentsInChildren<Renderer>(true);
        bool hasBounds = false;
        Bounds bounds = default;
        foreach (Renderer renderer in renderers)
        {
            if (renderer == null) continue;
            if (!hasBounds)
            {
                bounds = renderer.bounds;
                hasBounds = true;
            }
            else bounds.Encapsulate(renderer.bounds);
        }
        if (!hasBounds) return;
        float current = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
        if (current <= .0001f) return;
        item.transform.localScale *= targetSize / current;
    }

    static string Normalize(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        char[] buffer = new char[value.Length];
        int count = 0;
        foreach (char c in value)
            if (char.IsLetterOrDigit(c)) buffer[count++] = char.ToLowerInvariant(c);
        return new string(buffer, 0, count);
    }

    static void AddHorseEmblem(Transform shield)
    {
        Color gold = new Color(.86f, .62f, .20f);
        AddEmblemPart(shield, "HectorHorseBody", PrimitiveType.Cube, new Vector3(-.02f, .02f, .184f), new Vector3(.25f, .105f, .022f), gold, Quaternion.Euler(0f, 0f, -7f));
        AddEmblemPart(shield, "HectorHorseNeck", PrimitiveType.Cube, new Vector3(.14f, .11f, .186f), new Vector3(.055f, .16f, .023f), gold, Quaternion.Euler(0f, 0f, -24f));
        AddEmblemPart(shield, "HectorHorseHead", PrimitiveType.Sphere, new Vector3(.20f, .19f, .188f), new Vector3(.085f, .065f, .024f), gold);
        AddEmblemPart(shield, "HectorHorseLegFront", PrimitiveType.Cube, new Vector3(.09f, -.11f, .186f), new Vector3(.045f, .16f, .022f), gold, Quaternion.Euler(0f, 0f, -19f));
        AddEmblemPart(shield, "HectorHorseLegRear", PrimitiveType.Cube, new Vector3(-.13f, -.11f, .186f), new Vector3(.045f, .15f, .022f), gold, Quaternion.Euler(0f, 0f, 18f));
        AddEmblemPart(shield, "HectorHorseTail", PrimitiveType.Cube, new Vector3(-.22f, .07f, .186f), new Vector3(.035f, .13f, .022f), gold, Quaternion.Euler(0f, 0f, 42f));
    }

    static GameObject AddEmblemPart(Transform parent, string name, PrimitiveType type, Vector3 localPosition, Vector3 localScale, Color color, Quaternion? localRotation = null)
    {
        GameObject part = GameObject.CreatePrimitive(type);
        part.name = name;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = localPosition;
        part.transform.localScale = localScale;
        if (localRotation.HasValue) part.transform.localRotation = localRotation.Value;
        Collider collider = part.GetComponent<Collider>();
        if (collider != null) UnityEngine.Object.DestroyImmediate(collider);
        TowerFactory.SetColor(part, color);
        return part;
    }

    static Transform FindGeneratedShield(GameObject root)
    {
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform transform in all)
        {
            if (transform == null) continue;
            string lower = transform.name.ToLowerInvariant();
            if (lower.StartsWith("gear_shield_", StringComparison.Ordinal)) return transform;
        }
        return null;
    }

    static void RemoveGeneratedShield(GameObject root)
    {
        Transform shield = FindGeneratedShield(root);
        if (shield != null) UnityEngine.Object.DestroyImmediate(shield.gameObject);
    }

    static Transform FindByName(GameObject root, string objectName)
    {
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform transform in all)
            if (transform != null && string.Equals(transform.name, objectName, StringComparison.Ordinal)) return transform;
        return null;
    }

    static void RemoveByName(GameObject root, string objectName)
    {
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform transform in all)
        {
            if (transform == null || transform.gameObject == root) continue;
            if (string.Equals(transform.name, objectName, StringComparison.Ordinal))
                UnityEngine.Object.DestroyImmediate(transform.gameObject);
        }
    }
}
