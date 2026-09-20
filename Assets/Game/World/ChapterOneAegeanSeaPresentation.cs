using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Presentation-only sea pass for Chapter I.
// The sea is one continuous mesh from deep water to the shoreline. There are no
// large depth-color bands or separate base slabs, because those produced visible
// vertical cuts from the real tactical camera. Depth is now suggested only by
// restrained wave/current accents and the shoreline surf.
public sealed class ChapterOneAegeanSeaPresentation : MonoBehaviour
{
    Transform coastRoot;
    ChapterOneShoreLife shoreLife;
    ChapterOneCoastEdgeClosure edgeClosure;
    public Transform Root { get; private set; }

    public void Initialize(Transform coast, ChapterOneShoreLife shore, ChapterOneCoastEdgeClosure edge)
    {
        coastRoot = coast;
        shoreLife = shore;
        edgeClosure = edge;
    }

    static readonly Color OceanBase = new Color(.025f, .335f, .60f);
    static readonly Color Undertow = new Color(.035f, .42f, .60f);
    static readonly Color WaveBlue = new Color(.34f, .73f, .81f);
    static readonly Color Foam = new Color(.96f, .985f, .96f);
    static readonly Color ThinFoam = new Color(.77f, .92f, .91f);
    static readonly Color SunGlint = new Color(.86f, .96f, .94f);

    static readonly HashSet<string> LegacyWaterObjects = new HashSet<string>
    {
        "Aegean Sea Base",
        "Deep Aegean Sea",
        "Aegean Mid Water",
        "Aegean Shallows",
        "Aegean Shelf Transition",
        "Aegean Mid Shelf",
        "Shore Shallow Gradient Band",
        "Shallow Water Variation",
        "Submerged Shoal",
        "Shallow Swell",
        "Shore Foam Fleck",
        "Breaking Shore Foam",
        "Shore Backwash",
        "Moving Sea Glint",
        "Sea Spray Wisp",
        "Greek Ship Wake",
        "Coast Side Sea Extension"
    };

    IEnumerator Start()
    {
        if (coastRoot == null || (GameManager.Instance != null && GameManager.Instance.MapNumber != 1))
        {
            Destroy(gameObject);
            yield break;
        }

        // Shore-life and edge-closure build during Start. Wait for their explicit
        // owner roots instead of rediscovering them by scene object name.
        for (int i = 0; i < 8 && ((shoreLife != null && shoreLife.Root == null) || (edgeClosure != null && edgeClosure.Root == null)); i++)
            yield return null;

        DisableLegacySeaRenderers(coastRoot, shoreLife != null ? shoreLife.Root : null, edgeClosure != null ? edgeClosure.Root : null);

        GameObject root = new GameObject("Chapter01_AegeanSeaPresentation");
        Root = root.transform;
        BuildContinuousOcean(root.transform);
        BuildOpenWaterCurrents(root.transform);
        BuildWavelets(root.transform);
        BuildShoreBreakers(root.transform);
        BuildSunGlints(root.transform);
    }

    static void DisableLegacySeaRenderers(Transform coastRoot, Transform shoreLifeRoot, Transform edgeClosureRoot)
    {
        DisableLegacySeaRenderersUnder(coastRoot);
        DisableLegacySeaRenderersUnder(shoreLifeRoot);
        DisableLegacySeaRenderersUnder(edgeClosureRoot);
    }

    static void DisableLegacySeaRenderersUnder(Transform root)
    {
        if (root == null) return;

        Transform[] descendants = root.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < descendants.Length; i++)
        {
            Transform t = descendants[i];
            if (t == null || !LegacyWaterObjects.Contains(t.name)) continue;
            Renderer renderer = t.GetComponent<Renderer>();
            if (renderer != null) renderer.enabled = false;
        }
    }

    static void BuildContinuousOcean(Transform parent)
    {
        const int segments = 72;
        const float minZ = -27f;
        const float maxZ = 27f;
        const float openSeaX = -40f;
        const float waterY = -.142f;

        Vector3[] vertices = new Vector3[(segments + 1) * 2];
        int[] triangles = new int[segments * 6];

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float z = Mathf.Lerp(minZ, maxZ, t);
            float shoreX = CoastEnvironmentBuilder.ShorelineX(z) - .10f;
            vertices[i * 2] = new Vector3(openSeaX, waterY, z);
            vertices[i * 2 + 1] = new Vector3(shoreX, waterY, z);
        }

        FillStripTriangles(triangles, segments);
        CreateMeshObject(parent, "Aegean Continuous Ocean", vertices, triangles, OceanBase);
    }

    static void BuildOpenWaterCurrents(Transform parent)
    {
        float[] zBands = { -8.0f, -4.2f, -.3f, 3.8f, 7.8f };
        for (int i = 0; i < zBands.Length; i++)
        {
            GameObject current = CreateOpenWaterRibbon(parent,
                "Aegean Current Ribbon",
                -31.5f,
                -16.5f,
                zBands[i],
                -.132f - (i % 2) * .004f,
                .032f + (i % 3) * .008f,
                WaveBlue * (.55f + i * .025f),
                i * .83f);
            AddSeaMotion(current, .5f + i * .64f);
        }
    }

    static void BuildWavelets(Transform parent)
    {
        for (int i = 0; i < 22; i++)
        {
            float x = -16.8f - (i % 6) * 2.20f - (i / 6) * .20f;
            float z = -9.4f + ((i * 3.17f) % 18.8f);
            float length = .32f + (i % 5) * .13f;
            Vector3 a = new Vector3(x, -.126f - (i % 3) * .002f, z);
            Vector3 b = a + new Vector3(length, 0f, .025f + (i % 2) * .035f);
            GameObject wavelet = CreateSegmentRibbon(parent, "Aegean Small Wave", a, b,
                .018f + (i % 3) * .005f,
                i % 6 == 0 ? ThinFoam * .83f : WaveBlue * .72f);
            AddSeaMotion(wavelet, 1.0f + i * .21f);
        }
    }

    static void BuildShoreBreakers(Transform parent)
    {
        // Three very narrow lines define the active surf zone. They are not depth
        // slices: all of them sit within ~1 metre of the real shoreline.
        GameObject undertow = CreateCoastRibbon(parent, "Aegean Undertow Line", -1.02f, -.131f, .060f, Undertow, .10f, .0f);
        GameObject outer = CreateCoastRibbon(parent, "Aegean Breaking Foam", -.66f, -.126f, .070f, ThinFoam, .10f, .4f);
        GameObject main = CreateCoastRibbon(parent, "Aegean Breaking Foam", -.33f, -.120f, .105f, Foam, .12f, 1.2f);
        AddSeaMotion(undertow, .2f);
        AddSurfMotion(outer, .6f);
        AddSurfMotion(main, 1.4f);
    }

    static void BuildSunGlints(Transform parent)
    {
        for (int i = 0; i < 16; i++)
        {
            float x = -16.4f - (i % 5) * 2.35f - (i / 5) * .20f;
            float z = -9.0f + ((i * 4.03f) % 18.0f);
            Vector3 a = new Vector3(x, -.122f, z);
            Vector3 b = a + new Vector3(.24f + (i % 4) * .10f, 0f, .02f + (i % 2) * .025f);
            GameObject glint = CreateSegmentRibbon(parent, "Aegean Sun Glint", a, b,
                .012f + (i % 2) * .006f,
                SunGlint * (.74f + (i % 3) * .06f));
            AddSeaMotion(glint, 3.0f + i * .19f);
        }
    }

    static GameObject CreateCoastRibbon(Transform parent, string name, float shoreOffset, float y,
        float width, Color color, float edgeWave, float phase)
    {
        const int segments = 48;
        Vector3[] centers = new Vector3[segments + 1];
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float z = Mathf.Lerp(-11.0f, 11.0f, t);
            float x = CoastEnvironmentBuilder.ShorelineX(z) + shoreOffset + Mathf.Sin(z * .71f + phase) * edgeWave;
            centers[i] = new Vector3(x, y, z);
        }
        return CreatePolylineRibbon(parent, name, centers, width, color);
    }

    static GameObject CreateOpenWaterRibbon(Transform parent, string name, float startX, float endX,
        float zCenter, float y, float width, Color color, float phase)
    {
        const int points = 16;
        Vector3[] centers = new Vector3[points];
        for (int i = 0; i < points; i++)
        {
            float t = i / (float)(points - 1);
            float x = Mathf.Lerp(startX, endX, t);
            float z = zCenter + Mathf.Sin(t * 5.1f + phase) * .18f + Mathf.Sin(t * 9.4f + phase * .6f) * .045f;
            centers[i] = new Vector3(x, y, z);
        }
        return CreatePolylineRibbon(parent, name, centers, width, color);
    }

    static GameObject CreateSegmentRibbon(Transform parent, string name, Vector3 a, Vector3 b, float width, Color color)
    {
        return CreatePolylineRibbon(parent, name, new[] { a, b }, width, color);
    }

    static GameObject CreatePolylineRibbon(Transform parent, string name, Vector3[] centers, float width, Color color)
    {
        if (centers == null || centers.Length < 2) return null;
        Vector3[] vertices = new Vector3[centers.Length * 2];
        int[] triangles = new int[(centers.Length - 1) * 6];

        for (int i = 0; i < centers.Length; i++)
        {
            Vector3 prev = centers[Mathf.Max(0, i - 1)];
            Vector3 next = centers[Mathf.Min(centers.Length - 1, i + 1)];
            Vector3 tangent = next - prev;
            tangent.y = 0f;
            if (tangent.sqrMagnitude < .0001f) tangent = Vector3.forward;
            tangent.Normalize();
            Vector3 side = new Vector3(-tangent.z, 0f, tangent.x) * (width * .5f);
            vertices[i * 2] = centers[i] - side;
            vertices[i * 2 + 1] = centers[i] + side;
        }

        FillStripTriangles(triangles, centers.Length - 1);
        return CreateMeshObject(parent, name, vertices, triangles, color);
    }

    static void FillStripTriangles(int[] triangles, int segments)
    {
        for (int i = 0; i < segments; i++)
        {
            int v = i * 2;
            int q = i * 6;
            triangles[q] = v;
            triangles[q + 1] = v + 2;
            triangles[q + 2] = v + 1;
            triangles[q + 3] = v + 1;
            triangles[q + 4] = v + 2;
            triangles[q + 5] = v + 3;
        }
    }

    static GameObject CreateMeshObject(Transform parent, string name, Vector3[] vertices, int[] triangles, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Mesh mesh = new Mesh { name = name + " Mesh" };
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        go.AddComponent<MeshRenderer>();
        TowerFactory.SetColor(go, color);
        return go;
    }

    static void AddSeaMotion(GameObject go, float phase)
    {
        if (go == null) return;
        ChapterOneAmbientMotion motion = go.AddComponent<ChapterOneAmbientMotion>();
        motion.kind = ChapterOneAmbientMotion.MotionKind.Sea;
        motion.phase = phase;
    }

    static void AddSurfMotion(GameObject go, float phase)
    {
        if (go == null) return;
        ChapterOneAmbientMotion motion = go.AddComponent<ChapterOneAmbientMotion>();
        motion.kind = ChapterOneAmbientMotion.MotionKind.Surf;
        motion.phase = phase;
    }
}
