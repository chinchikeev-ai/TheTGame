using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ChapterOneUrpMaterialRepair
{
    const string CharacterRoot = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters";
    const string MaterialRoot = "Assets/Game/Art/Characters/Resources/TroyProduction/Materials";
    const string RuntimeMaterialPath = "Assets/Resources/RuntimeColorMaterial.mat";

    [MenuItem("The Troy Game/Characters/Repair Chapter I URP Materials")]
    public static void RepairMenu()
    {
        RepairAll(true);
    }

    public static int RepairAll(bool logSummary = true)
    {
        if (!AssetDatabase.IsValidFolder(CharacterRoot)) return 0;
        EnsureFolder(MaterialRoot);

        int repairedPrefabs = 0;
        int repairedMaterials = 0;
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { CharacterRoot });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                bool changed = false;
                if (root.GetComponent<CharacterUrpMaterialAdapter>() == null)
                {
                    root.AddComponent<CharacterUrpMaterialAdapter>();
                    changed = true;
                }

                int materialChanges = StabilizeMaterials(root, root.name, null);
                if (materialChanges > 0)
                {
                    repairedMaterials += materialChanges;
                    changed = true;
                }

                if (!changed) continue;
                PrefabUtility.SaveAsPrefabAsset(root, path);
                repairedPrefabs++;
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        if (repairedPrefabs > 0 || repairedMaterials > 0)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        if (logSummary)
            Debug.Log("Chapter I URP material repair stabilized " + repairedMaterials + " renderer material slot(s) across " + repairedPrefabs + " prefab(s). Materials are now project-owned URP assets; runtime adapter remains as a safety net.");

        return repairedPrefabs;
    }

    public static int StabilizeMaterials(GameObject root, string prefabName, Color? tint)
    {
        if (root == null) return 0;
        Material template = AssetDatabase.LoadAssetAtPath<Material>(RuntimeMaterialPath);
        if (template == null || template.shader == null)
        {
            Debug.LogError("Chapter I material stabilization cannot find valid template: " + RuntimeMaterialPath);
            return 0;
        }

        EnsureFolder(MaterialRoot);
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        int changed = 0;
        for (int rendererIndex = 0; rendererIndex < renderers.Length; rendererIndex++)
        {
            Renderer renderer = renderers[rendererIndex];
            if (renderer == null) continue;

            Material[] materials = renderer.sharedMaterials;
            bool rendererChanged = false;
            for (int slot = 0; slot < materials.Length; slot++)
            {
                Material source = materials[slot];
                if (source == null) continue;

                string assetPath = MaterialPath(prefabName, rendererIndex, slot);
                Material stable = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                Texture texture = ReadTexture(source);
                Vector2 scale = ReadTextureScale(source);
                Vector2 offset = ReadTextureOffset(source);
                Color color = SanitizeColor(ReadColor(source));
                if (tint.HasValue) color = Color.Lerp(color, tint.Value, .32f);

                if (stable == null)
                {
                    stable = new Material(template) { name = Path.GetFileNameWithoutExtension(assetPath) };
                    AssetDatabase.CreateAsset(stable, assetPath);
                }
                else if (stable.shader != template.shader)
                {
                    stable.shader = template.shader;
                }

                ApplyTexture(stable, texture, scale, offset);
                ApplyColor(stable, color);
                EditorUtility.SetDirty(stable);

                if (materials[slot] == stable) continue;
                materials[slot] = stable;
                rendererChanged = true;
                changed++;
            }

            if (rendererChanged)
                renderer.sharedMaterials = materials;
        }

        return changed;
    }

    public static List<string> CollectProblems()
    {
        var problems = new List<string>();
        if (!AssetDatabase.IsValidFolder(CharacterRoot))
        {
            problems.Add("Missing character root: " + CharacterRoot);
            return problems;
        }

        Material template = AssetDatabase.LoadAssetAtPath<Material>(RuntimeMaterialPath);
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { CharacterRoot });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                if (root.GetComponent<CharacterUrpMaterialAdapter>() == null)
                    problems.Add(path + ": missing CharacterUrpMaterialAdapter.");

                foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
                {
                    Material[] materials = renderer.sharedMaterials;
                    for (int i = 0; i < materials.Length; i++)
                    {
                        Material material = materials[i];
                        if (material == null)
                        {
                            problems.Add(path + ": renderer " + renderer.name + " has missing material at slot " + i + ".");
                            continue;
                        }

                        string materialPath = AssetDatabase.GetAssetPath(material);
                        if (!materialPath.StartsWith(MaterialRoot + "/", StringComparison.Ordinal))
                            problems.Add(path + ": renderer " + renderer.name + " uses external/non-stabilized material " + material.name + ".");
                        if (template != null && material.shader != template.shader)
                            problems.Add(path + ": renderer " + renderer.name + " material " + material.name + " does not use RuntimeColorMaterial shader.");
                    }
                }
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
        return problems;
    }

    static string MaterialPath(string prefabName, int rendererIndex, int slot)
    {
        string safe = SanitizeFileName(string.IsNullOrEmpty(prefabName) ? "Character" : prefabName);
        return MaterialRoot + "/" + safe + "_R" + rendererIndex + "_M" + slot + ".mat";
    }

    static string SanitizeFileName(string value)
    {
        foreach (char invalid in Path.GetInvalidFileNameChars()) value = value.Replace(invalid, '_');
        return value.Replace('/', '_').Replace('\\', '_').Replace(':', '_');
    }

    static Texture ReadTexture(Material material)
    {
        if (material.HasProperty("_BaseMap"))
        {
            Texture texture = material.GetTexture("_BaseMap");
            if (texture != null) return texture;
        }
        if (material.HasProperty("_MainTex"))
        {
            Texture texture = material.GetTexture("_MainTex");
            if (texture != null) return texture;
        }
        return material.mainTexture;
    }

    static Vector2 ReadTextureScale(Material material)
    {
        if (material.HasProperty("_BaseMap")) return material.GetTextureScale("_BaseMap");
        if (material.HasProperty("_MainTex")) return material.GetTextureScale("_MainTex");
        return Vector2.one;
    }

    static Vector2 ReadTextureOffset(Material material)
    {
        if (material.HasProperty("_BaseMap")) return material.GetTextureOffset("_BaseMap");
        if (material.HasProperty("_MainTex")) return material.GetTextureOffset("_MainTex");
        return Vector2.zero;
    }

    static Color ReadColor(Material material)
    {
        if (material.HasProperty("_BaseColor")) return material.GetColor("_BaseColor");
        if (material.HasProperty("_Color")) return material.GetColor("_Color");
        return Color.white;
    }

    static Color SanitizeColor(Color color)
    {
        // Bright Unity-error magenta must never be propagated into the stabilized material.
        if (color.r >= .85f && color.g <= .25f && color.b >= .85f) return Color.white;
        return color;
    }

    static void ApplyTexture(Material material, Texture texture, Vector2 scale, Vector2 offset)
    {
        if (material.HasProperty("_BaseMap"))
        {
            material.SetTexture("_BaseMap", texture);
            material.SetTextureScale("_BaseMap", scale);
            material.SetTextureOffset("_BaseMap", offset);
        }
        if (material.HasProperty("_MainTex"))
        {
            material.SetTexture("_MainTex", texture);
            material.SetTextureScale("_MainTex", scale);
            material.SetTextureOffset("_MainTex", offset);
        }
    }

    static void ApplyColor(Material material, Color color)
    {
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color")) material.SetColor("_Color", color);
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
