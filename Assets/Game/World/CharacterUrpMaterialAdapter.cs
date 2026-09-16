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
    static readonly Dictionary<string, Material> MissingSlotMaterials = new Dictionary<string, Material>();
    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");
    static readonly MaterialPropertyBlock PropertyBlock = new MaterialPropertyBlock();
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

        int convertedCount = SanitizeErrorMagentaPropertyBlock(renderer);
        Material[] sourceMaterials = renderer.sharedMaterials ?? Array.Empty<Material>();
        int requiredSlots = RequiredMaterialSlots(renderer);
        if (requiredSlots > sourceMaterials.Length)
            Array.Resize(ref sourceMaterials, requiredSlots);
        if (sourceMaterials.Length == 0) return convertedCount;

        bool changed = false;
        Material[] convertedMaterials = new Material[sourceMaterials.Length];
        for (int i = 0; i < sourceMaterials.Length; i++)
        {
            Material source = sourceMaterials[i];
            Material converted = ConvertMaterial(source, forceStableCharacterMaterial, renderer, i);
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

    static int RequiredMaterialSlots(Renderer renderer)
    {
        if (renderer is SkinnedMeshRenderer skinned && skinned.sharedMesh != null)
            return Mathf.Max(1, skinned.sharedMesh.subMeshCount);

        MeshFilter filter = renderer.GetComponent<MeshFilter>();
        if (filter != null && filter.sharedMesh != null)
            return Mathf.Max(1, filter.sharedMesh.subMeshCount);

        return renderer.sharedMaterials != null ? renderer.sharedMaterials.Length : 0;
    }

    static int SanitizeErrorMagentaPropertyBlock(Renderer renderer)
    {
        PropertyBlock.Clear();
        renderer.GetPropertyBlock(PropertyBlock);
        if (PropertyBlock.isEmpty) return 0;

        Color baseOverride = PropertyBlock.GetColor(BaseColorId);
        Color colorOverride = PropertyBlock.GetColor(ColorId);
        bool baseMagenta = IsErrorMagenta(baseOverride);
        bool colorMagenta = IsErrorMagenta(colorOverride);
        if (!baseMagenta && !colorMagenta) return 0;

        if (baseMagenta) PropertyBlock.SetColor(BaseColorId, Color.white);
        if (colorMagenta) PropertyBlock.SetColor(ColorId, Color.white);
        renderer.SetPropertyBlock(PropertyBlock);
        return 1;
    }

    static Material ConvertMaterial(Material source, bool forceStableCharacterMaterial, Renderer renderer, int slot)
    {
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

        if (source == null)
            return GetMissingSlotMaterial(baseTemplate, renderer, slot);

        if (source == baseTemplate) return source;
        if (source.shader == baseTemplate.shader && source.name.EndsWith(ConvertedSuffix, StringComparison.Ordinal))
        {
            SanitizeMaterialColor(source);
            return source;
        }

        Shader sourceShader = source.shader;
        if (!forceStableCharacterMaterial && sourceShader != null && sourceShader.isSupported &&
            sourceShader.name.StartsWith(UrpPrefix, StringComparison.Ordinal))
        {
            SanitizeMaterialColor(source);
            return source;
        }

        if (Converted.TryGetValue(source, out Material cached) && cached != null)
        {
            SanitizeMaterialColor(cached);
            return cached;
        }

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

        Color color = SanitizeColor(ReadBaseColor(source));
        ApplyColor(material, color);

        CopyTextureTransform(source, material, "_BaseMap");
        if (!source.HasProperty("_BaseMap")) CopyTextureTransform(source, material, "_MainTex");

        Converted[source] = material;
        return material;
    }

    static Material GetMissingSlotMaterial(Material baseTemplate, Renderer renderer, int slot)
    {
        CharacterVisualIdentity identity = renderer != null ? renderer.GetComponentInParent<CharacterVisualIdentity>() : null;
        string semantic = MaterialSemantic(renderer != null ? renderer.name : string.Empty);
        string faction = identity != null ? identity.faction.ToString() : "Unknown";
        string role = identity != null ? identity.role.ToString() : "Unknown";
        string key = faction + ":" + role + ":" + semantic + ":" + slot;

        if (MissingSlotMaterials.TryGetValue(key, out Material cached) && cached != null)
            return cached;

        Material material = new Material(baseTemplate)
        {
            name = "Missing_" + semantic + "_" + faction + "_M" + slot + ConvertedSuffix,
            hideFlags = HideFlags.DontSave
        };
        ApplyColor(material, ResolveMissingSlotColor(renderer, identity));
        MissingSlotMaterials[key] = material;
        return material;
    }

    static string MaterialSemantic(string rendererName)
    {
        string lower = (rendererName ?? string.Empty).ToLowerInvariant();
        if (lower.Contains("head") || lower.Contains("face")) return "SkinHead";
        if (lower.Contains("arm") || lower.Contains("hand")) return "SkinArm";
        if (lower.Contains("helmet")) return "Helmet";
        if (lower.Contains("cape") || lower.Contains("cloth")) return "Cape";
        if (lower.Contains("shield")) return "Shield";
        if (lower.Contains("sword") || lower.Contains("blade")) return "Steel";
        if (lower.Contains("leg") || lower.Contains("boot") || lower.Contains("foot")) return "Legs";
        return "Body";
    }

    static Color ResolveMissingSlotColor(Renderer renderer, CharacterVisualIdentity identity)
    {
        string semantic = MaterialSemantic(renderer != null ? renderer.name : string.Empty);
        bool trojan = identity == null || identity.faction == TroyFaction.Trojan;

        switch (semantic)
        {
            case "SkinHead":
            case "SkinArm":
                return new Color(.86f, .52f, .32f, 1f);
            case "Helmet":
            case "Shield":
                return new Color(.63f, .40f, .14f, 1f);
            case "Steel":
                return new Color(.58f, .62f, .67f, 1f);
            case "Cape":
                return trojan ? new Color(.55f, .08f, .045f, 1f) : new Color(.18f, .30f, .52f, 1f);
            case "Legs":
                return trojan ? new Color(.28f, .12f, .07f, 1f) : new Color(.20f, .24f, .31f, 1f);
            default:
                return trojan ? new Color(.63f, .30f, .12f, 1f) : new Color(.38f, .50f, .67f, 1f);
        }
    }

    static void SanitizeMaterialColor(Material material)
    {
        if (material == null) return;
        if (material.HasProperty("_BaseColor"))
        {
            Color color = material.GetColor("_BaseColor");
            if (IsErrorMagenta(color)) material.SetColor("_BaseColor", Color.white);
        }
        if (material.HasProperty("_Color"))
        {
            Color color = material.GetColor("_Color");
            if (IsErrorMagenta(color)) material.SetColor("_Color", Color.white);
        }
    }

    static Color SanitizeColor(Color color)
    {
        return IsErrorMagenta(color) ? Color.white : color;
    }

    static bool IsErrorMagenta(Color color)
    {
        return color.a > .5f && color.r >= .90f && color.g <= .12f && color.b >= .90f;
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

    static void ApplyColor(Material material, Color color)
    {
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color")) material.SetColor("_Color", color);
    }

    static void CopyTextureTransform(Material source, Material target, string property)
    {
        if (!source.HasProperty(property) || !target.HasProperty("_BaseMap")) return;
        target.SetTextureScale("_BaseMap", source.GetTextureScale(property));
        target.SetTextureOffset("_BaseMap", source.GetTextureOffset(property));
    }
}
