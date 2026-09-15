using System;
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
        public float scaleMultiplier;
        public bool horseEmblem;

        public ShieldTarget(string prefabPath, bool figureEight, float scaleMultiplier, bool horseEmblem = false)
        {
            this.prefabPath = prefabPath;
            this.figureEight = figureEight;
            this.scaleMultiplier = scaleMultiplier;
            this.horseEmblem = horseEmblem;
        }
    }

    static readonly ShieldTarget[] Targets =
    {
        new ShieldTarget(GreekRoot + "Enemy_Infantry.prefab", false, 1.00f),
        new ShieldTarget(GreekRoot + "Enemy_ShieldBearer.prefab", false, 1.08f),
        new ShieldTarget(GreekRoot + "Enemy_HeavyHoplite.prefab", true, .94f),
        new ShieldTarget(GreekRoot + "Enemy_Boss.prefab", false, 1.08f),
        new ShieldTarget(TrojanRoot + "Trojan_Infantry.prefab", false, 1.00f),
        new ShieldTarget(TrojanRoot + "Trojan_Guard.prefab", true, 1.00f),
        new ShieldTarget(HectorPrefabPath, false, 1.10f, true),
        new ShieldTarget(HeroRoot + "Hero_Menelaus.prefab", false, 1.05f)
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
        Debug.Log("Chapter I Late Bronze Age shield pass upgraded " + upgraded + " prefab(s). These authored static meshes remain generated/candidate art until real Unity visual QA and explicit production acceptance.");
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
            Transform previousSource = FindByName(root, SourceShieldName);
            Transform oldShield = FindGeneratedShield(root);
            Transform poseSource = oldShield != null ? oldShield : previousSource;
            if (poseSource == null)
            {
                Debug.LogWarning("Shield pass found no recognized shield pose: " + target.prefabPath);
                return false;
            }

            Transform parent = poseSource.parent ?? root.transform;
            Vector3 localPosition = poseSource.localPosition;
            Quaternion localRotation = poseSource.localRotation;
            // Do not multiply an already-authored source shield every time the pass is re-run.
            Vector3 localScale = previousSource != null && oldShield == null
                ? poseSource.localScale
                : poseSource.localScale * target.scaleMultiplier;

            RemoveByName(root, SourceShieldName);
            RemoveGeneratedShield(root);

            GameObject shield = PrefabUtility.InstantiatePrefab(shieldSource) as GameObject;
            if (shield == null) shield = UnityEngine.Object.Instantiate(shieldSource);
            if (shield == null) throw new InvalidOperationException("Unable to instantiate authored shield candidate.");

            shield.name = SourceShieldName;
            shield.transform.SetParent(parent, false);
            shield.transform.localPosition = localPosition;
            shield.transform.localRotation = localRotation;
            shield.transform.localScale = localScale;

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
