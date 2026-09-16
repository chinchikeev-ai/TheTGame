using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class RuntimeVisualAudit
{
    const int MaxDetailedSlotsPerReport = 32;

    static readonly HashSet<string> Reported = new HashSet<string>();
    static readonly HashSet<string> DetailedReported = new HashSet<string>();
    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");
    static readonly MaterialPropertyBlock PropertyBlock = new MaterialPropertyBlock();

    public static void Report(string role, string source, string detail)
    {
        string key = role + "|" + source + "|" + detail;
        if (!Reported.Add(key)) return;
        RuntimeFileLogger.Event("ART_SOURCE", role + " source=" + source + " detail=" + detail);
    }

    public static void ReportDetailed(string role, string source, string detail, GameObject root)
    {
        Report(role, source, detail);
        if (root == null) return;

        string detailKey = role + "|" + source;
        if (!DetailedReported.Add(detailKey)) return;

        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        string pipeline = GraphicsSettings.currentRenderPipeline != null
            ? GraphicsSettings.currentRenderPipeline.GetType().Name
            : "BuiltIn";

        RuntimeFileLogger.Event(
            "ART_RENDER",
            "role=" + role +
            " source=" + source +
            " root=" + root.name +
            " pipeline=" + pipeline +
            " renderers=" + renderers.Length +
            " detail=" + detail);

        int slotCount = 0;
        foreach (Renderer renderer in renderers)
        {
            if (renderer == null) continue;
            string meshName = ResolveMeshName(renderer);
            string blockInfo = DescribePropertyBlock(renderer);
            Material[] materials = renderer.sharedMaterials;

            if (materials == null || materials.Length == 0)
            {
                RuntimeFileLogger.Event(
                    "ART_MATERIAL",
                    "role=" + role +
                    " renderer=" + TransformPath(root.transform, renderer.transform) +
                    " type=" + renderer.GetType().Name +
                    " mesh=" + meshName +
                    " material=<none> " + blockInfo);
                continue;
            }

            for (int i = 0; i < materials.Length && slotCount < MaxDetailedSlotsPerReport; i++, slotCount++)
            {
                Material material = materials[i];
                Shader shader = material != null ? material.shader : null;
                Texture texture = ReadBaseTexture(material);
                Color color = ReadBaseColor(material);

                RuntimeFileLogger.Event(
                    "ART_MATERIAL",
                    "role=" + role +
                    " renderer=" + TransformPath(root.transform, renderer.transform) +
                    " type=" + renderer.GetType().Name +
                    " mesh=" + meshName +
                    " slot=" + i +
                    " material=" + (material != null ? material.name : "<null>") +
                    " shader=" + (shader != null ? shader.name : "<null>") +
                    " shaderSupported=" + (shader != null && shader.isSupported) +
                    " texture=" + (texture != null ? texture.name : "<none>") +
                    " color=" + FormatColor(color) +
                    " " + blockInfo);
            }

            if (slotCount >= MaxDetailedSlotsPerReport) break;
        }

        if (slotCount >= MaxDetailedSlotsPerReport)
            RuntimeFileLogger.Event("ART_RENDER", "role=" + role + " detailedMaterialSlotsTruncatedAt=" + MaxDetailedSlotsPerReport);
    }

    static string ResolveMeshName(Renderer renderer)
    {
        SkinnedMeshRenderer skinned = renderer as SkinnedMeshRenderer;
        if (skinned != null && skinned.sharedMesh != null) return skinned.sharedMesh.name;

        MeshFilter filter = renderer.GetComponent<MeshFilter>();
        return filter != null && filter.sharedMesh != null ? filter.sharedMesh.name : "<none>";
    }

    static string DescribePropertyBlock(Renderer renderer)
    {
        PropertyBlock.Clear();
        renderer.GetPropertyBlock(PropertyBlock);
        if (PropertyBlock.isEmpty) return "propertyBlock=empty";

        Color baseColor = PropertyBlock.GetColor(BaseColorId);
        Color color = PropertyBlock.GetColor(ColorId);
        return "propertyBlockBase=" + FormatColor(baseColor) + " propertyBlockColor=" + FormatColor(color);
    }

    static Texture ReadBaseTexture(Material material)
    {
        if (material == null) return null;
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

    static Color ReadBaseColor(Material material)
    {
        if (material == null) return Color.clear;
        if (material.HasProperty("_BaseColor")) return material.GetColor("_BaseColor");
        if (material.HasProperty("_Color")) return material.GetColor("_Color");
        return Color.clear;
    }

    static string TransformPath(Transform root, Transform target)
    {
        if (target == null) return "<null>";
        if (root == target) return root.name;

        var names = new List<string>();
        Transform current = target;
        while (current != null)
        {
            names.Add(current.name);
            if (current == root) break;
            current = current.parent;
        }
        names.Reverse();
        return string.Join("/", names.ToArray());
    }

    static string FormatColor(Color color)
    {
        return $"({color.r:F3},{color.g:F3},{color.b:F3},{color.a:F3})";
    }
}
