using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class CharacterUrpMaterialAdapter : MonoBehaviour
{
    const string RuntimeMaterialResource = "RuntimeColorMaterial";
    const string UrpPrefix = "Universal Render Pipeline/";
    const string ConvertedSuffix = "_URP_Runtime";
    const int DeferredPassCount = 2;

    static readonly Dictionary<Material, Material> Converted = new Dictionary<Material, Material>();
    static Material template;
    static bool loggedMissingTemplate;

    int deferredPasses;

    void Awake()
    {
        deferredPasses = DeferredPassCount;
        ApplyNow(true);
    }

    void LateUpdate()
    {
        if (deferredPasses <= 0) return;
        ApplyNow(true);
        deferredPasses--;
    }

    public static int ApplyTo(GameObject root)
    {
        if (root == null) return 0;
        CharacterUrpMaterialAdapter adapter = root.GetComponent<CharacterUrpMaterialAdapter>();
        if (adapter == null) adapter = root.AddComponent<CharacterUrpMaterialAdapter>();
        adapter.deferredPasses = DeferredPassCount;
        return adapter.ApplyNow(true);
    }

    public int ApplyNow()
    {
        return ApplyNow(true);
    }

    public int ApplyNow(bool forceStableCharacterMaterial)
    {
        int convertedCount = 0;
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
            convertedCount += ConvertRenderer(renderer, forceStableCharacterMaterial);
        return convertedCount;
    }

    static int ConvertRenderer(Renderer renderer, bool forceStableCharacterMaterial)
    {
        if (renderer == null) return 0;

        Material[] sourceMaterials = renderer.sharedMaterials;
        if (sourceMaterials == null || sourceMaterials.Length == 0) return 0;

        int convertedCount = 0;
        bool changed = false;
        Material[] convertedMaterials = new Material[sourceMaterials.Length];
        for (int i = 0; i < sourceMaterials.Length; i++)
        {
            Material source = sourceMaterials[i];
            Material converted = ConvertMaterial(source, forceStableCharacterMaterial);
            convertedMaterials[i] = converted;
            if (converted != source)
            {
                changed = true;
                convertedCount++;
            }
        }

        if (changed)
            renderer.sharedMaterials = convertedMaterials;
        return convertedCount;
    }

    static Material ConvertMaterial(Material source, bool forceStableCharacterMaterial)
    {
        if (source == null) return null;

        Material baseTemplate = GetTemplate();
        if (baseTemplate == null)
        {
            if (!loggedMissingTemplate)
            {
                loggedMissingTemplate = true;
                Debug.LogError("Character URP material adapter cannot find RuntimeColorMaterial or Universal Render Pipeline/Lit. Character materials will remain unchanged.");
            }
            return source;
        }

        if (source == baseTemplate) return source;
        if (source.shader == baseTemplate.shader && source.name.EndsWith(ConvertedSuffix, StringComparison.Ordinal))
            return source;

        Shader sourceShader = source.shader;
        if (!forceStableCharacterMaterial && sourceShader != null && sourceShader.isSupported &&
            sourceShader.name.StartsWith(UrpPrefix, StringComparison.Ordinal))
            return source;

        if (Converted.TryGetValue(source, out Material cached) && cached != null)
            return cached;

        Material material = new Material(baseTemplate)
        {
            name = source.name + ConvertedSuffix,
            hideFlags = HideFlags.DontSave
        };

        Texture texture = ReadBaseTexture(source);
        if (texture != null)
        {
            if (material.HasProperty("_BaseMap")) material.SetTexture("_BaseMap", texture);
            if (material.HasProperty("_MainTex")) material.SetTexture("_MainTex", texture);
        }

        Color color = ReadBaseColor(source);
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color")) material.SetColor("_Color", color);

        CopyTextureTransform(source, material, "_BaseMap");
        if (!source.HasProperty("_BaseMap")) CopyTextureTransform(source, material, "_MainTex");

        Converted[source] = material;
        return material;
    }

    static Material GetTemplate()
    {
        if (template != null && template.shader != null && template.shader.isSupported) return template;
        template = Resources.Load<Material>(RuntimeMaterialResource);
        if (template != null && template.shader != null && template.shader.isSupported) return template;

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader != null && shader.isSupported)
        {
            template = new Material(shader)
            {
                name = "CharacterUrpMaterialAdapter_Fallback",
                hideFlags = HideFlags.DontSave
            };
        }
        return template;
    }

    static Texture ReadBaseTexture(Material source)
    {
        if (source.HasProperty("_BaseMap"))
        {
            Texture texture = source.GetTexture("_BaseMap");
            if (texture != null) return texture;
        }
        if (source.HasProperty("_MainTex"))
        {
            Texture texture = source.GetTexture("_MainTex");
            if (texture != null) return texture;
        }
        return source.mainTexture;
    }

    static Color ReadBaseColor(Material source)
    {
        if (source.HasProperty("_BaseColor")) return source.GetColor("_BaseColor");
        if (source.HasProperty("_Color")) return source.GetColor("_Color");
        return Color.white;
    }

    static void CopyTextureTransform(Material source, Material target, string property)
    {
        if (!source.HasProperty(property) || !target.HasProperty("_BaseMap")) return;
        target.SetTextureScale("_BaseMap", source.GetTextureScale(property));
        target.SetTextureOffset("_BaseMap", source.GetTextureOffset(property));
    }
}
