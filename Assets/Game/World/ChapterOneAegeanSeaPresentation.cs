using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Presentation-only sea pass for Chapter I.
// The water is intentionally built as one coherent blue field with narrow,
// shoreline-following depth bands. This avoids the hard vertical stripes and
// oversized teal blocks that were visible from the real tactical camera.
public sealed class ChapterOneAegeanSeaPresentation : MonoBehaviour
{
    static readonly Color OceanBase = new Color(.028f, .32f, .58f);
    static readonly Color OceanDeep = new Color(.024f, .27f, .52f);
    static readonly Color ShelfBlue = new Color(.045f, .43f, .64f);
    static readonly Color Turquoise = new Color(.055f, .54f, .68f);
    static readonly Color Lagoon = new Color(.12f, .64f, .71f);
    static readonly Color Undertow = new Color(.035f, .39f, .57f);
    static readonly Color WaveBlue = new Color(.34f, .72f, .80f);
    static readonly Color Foam = new Color(.96f, .985f, .96f);
    static readonly Color ThinFoam = new Color(.76f, .91f, .90f);
    static readonly Color SunGlint = new Color(.84f, .95f, .93f);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<ChapterOneAegeanSeaPresentation>() == null)
            new GameObject("ChapterOneAegeanSeaPresentation").AddComponent<ChapterOneAegeanSeaPresentation>();
    }

    IEnumerator Start()
    {
        GameObject coast = null;
        for (int i = 0; i < 16 && coast == null; i++)
        {
            coast = GameObject.Find("Chapter01_CoastEnvironment");
            if (coast == null) yield return null;
        }

        if (coast == null || (GameManager.Instance != null && GameManager.Instance.MapNumber != 1))
        {
            Destroy(gameObject);
            yield break;
        }

        yield return null;
        yield return null;

        if (GameObject.Find("Chapter01_AegeanSeaPresentation") != null)
        {
            Destroy(gameObject);
            yield break;
        }

        DisableLegacyBlobWaterDetails();
        TintExistingSeaBase();

        GameObject root = new GameObject("Chapter01_AegeanSeaPresentation");
        BuildDepthBands(root.transform);
        BuildOpenWaterCurrents(root.transform);
        BuildWavelets(root.transform);
        BuildShoreBreakers(root.transform);
        BuildSunGlints(root.transform);
    }

    static void DisableLegacyBlobWaterDetails()
    {
        HashSet<string> names = new HashSet<string>
        {
            "Shallow Water Variation",
            "Submerged Shoal",
            "Shallow Swell",
            "Shore Foam Fleck",
            "Breaking Shore Foam",
            "Shore Backwash",
            "Moving Sea Glint",
            "Sea Spray Wisp",
            "Greek Ship Wake"
        };

        Transform[] all = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++)
        {
            Transform t = all[i];
            if (t == null || !names.Contains(t.name)) continue;
            t.gameObject.SetActive(false);
        }
    }

    static void TintExistingSeaBase()
    {
        // The three structural sea slabs now share one base hue. Depth is expressed
        // by organic shoreline bands instead of visible rectangular color seams.
        Recolor("Deep Aegean Sea", OceanBase);
        Recolor("Aegean Mid Water", OceanBase);
        Recolor("Aegean Shallows", OceanBase);
        Recolor("Aegean Shelf Transition", OceanBase);
        Recolor("Aegean Mid Shelf", OceanBase);
        Recolor("Shore Shallow Gradient Band", Turquoise * .94f);
    }

    static void Recolor(string objectName, Color color)
    {
        Transform[] all = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++)
        {
            Transform t = all[i];
            if (t != null && t.name == objectName)
                TowerFactory.SetColor(t.gameObject, color);
        }
    }

    static void BuildDepthBands(Transform parent)
    {
        // Wide-to-narrow depth progression, always following the coastline.
        CreateCoastBand(parent, "Aegean Deep Transition", -7.2f, -4.65f, -.154f, OceanDeep * 1.02f, .16f, .2f);
        CreateCoastBand(parent, "Aegean Blue Shelf", -5.0f, -2.85f, -.150f, ShelfBlue * .90f, .14f, .8f);
        CreateCoastBand(parent, "Aegean Turquoise Shelf", -3.05f, -1.55f, -.146f, Turquoise * .86f, .11f, 1.4f);
        CreateCoastBand(parent, "Aegean Lagoon Edge", -1.75f, -.68f, -.141f, Turquoise, .085f, 2.0f);
        CreateCoastBand(parent, "Aegean Nearshore Light", -.92f, -.24f, -.136f, Lagoon * .96f, .060f, 2.6f);
    }

    static void BuildOpenWaterCurrents(Transform parent)
    {
        float[] zBands = { -7.5f, -3.7f, .2f, 4.2f, 7.4f };
        for (int i = 0; i < zBands.Length; i++)
        {
            GameObject current = CreateOpenWaterRibbon(parent,
                "Aegean Current Ribbon",
                -30.0f,
                -16.0f,
                zBands[i],
                -.132f - (i % 2) * .006f,
                .045f + (i % 3) * .010f,
                WaveBlue * (.60f + i * .035f),
                i * .83f);
            AddSeaMotion(current, .5f + i * .64f);
        }
    }

    static void BuildWavelets(Transform parent)
    {
        for (int i = 0; i < 18; i++)
        {
            float x = -17.0f - (i % 5) * 2.25f - (i / 5) * .30f;
            float z = -8.8f + ((i * 3.31f) % 17.6f);
            float length = .42f + (i % 4) * .16f;
            Vector3 a = new Vector3(x, -.127f - (i % 4) * .003f, z);
            Vector3 b = a + new Vector3(length, 0f, .04f + (i % 2) * .045f);
            GameObject wavelet = CreateSegmentRibbon(parent, "Aegean Small Wave", a, b,
                .026f + (i % 3) * .007f,
                i % 5 == 0 ? ThinFoam * .86f : WaveBlue * .78f);
            AddSeaMotion(wavelet, 1.0f + i * .21f);
        }
    }

    static void BuildShoreBreakers(Transform parent)
    {
        GameObject undertow = CreateCoastRibbon(parent, "Aegean Undertow Line", -1.18f, -.134f, .095f, Undertow, .12f, .0f);
        GameObject outer = CreateCoastRibbon(parent, "Aegean Breaking Foam", -.83f, -.130f, .095f, ThinFoam, .12f, .4f);
        GameObject main = CreateCoastRibbon(parent, "Aegean Breaking Foam", -.43f, -.123f, .145f, Foam, .14f, 1.2f);
        GameObject inner = CreateCoastRibbon(parent, "Bright Surf Curl", -.15f, -.118f, .060f, Foam * .97f, .09f, 2.0f);
        AddSeaMotion(undertow, .2f);
        AddSurfMotion(outer, .6f);
        AddSurfMotion(main, 1.4f);
        AddSurfMotion(inner, 2.2f);
    }

    static void BuildSunGlints(Transform parent)
    {
        for (int i = 0; i < 14; i++)
        {
            float x = -16.6f - (i % 5) * 2.25f - (i / 5) * .22f;
            float z = -8.7f + ((i * 4.11f) % 17.4f);
            Vector3 a = new Vector3(x, -.124f, z);
            Vector3 b = a + new Vector3(.30f + (i % 4) * .12f, 0f, .03f + (i % 2) * .035f);
            GameObject glint = CreateSegmentRibbon(parent, "Aegean Sun Glint", a, b,
                .018f + (i % 2) * .008f,
                SunGlint * (.76f + (i % 3) * .065f));
            AddSeaMotion(glint, 3.0f + i * .19f);
        }
    }

    static GameObject CreateCoastBand(Transform parent, string name, float leftOffset, float rightOffset,
        float y, Color color, float edgeWave, float phase)
    {
        const int segments = 40;
        Vector3[] vertices = new Vector3[(segments + 1) * 2];
        int[] triangles = new int[segments * 6];

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float z = Mathf.Lerp(-11.0f, 11.0f, t);
            float shore = CoastEnvironmentBuilder.ShorelineX(z);
            float waveA = Mathf.Sin(z * .52f + phase) * edgeWave;
            float waveB = Mathf.Sin(z * .31f + phase * 1.7f) * edgeWave * .55f;
            vertices[i * 2] = new Vector3(shore + leftOffset + waveA, y, z);
            vertices[i * 2 + 1] = new Vector3(shore + rightOffset + waveB, y + .001f, z);
        }

        FillStripTriangles(triangles, segments);
        return CreateMeshObject(parent, name, vertices, triangles, color);
    }

    static GameObject CreateCoastRibbon(Transform parent, string name, float shoreOffset, float y,
        float width, Color color, float edgeWave, float phase)
    {
        const int segments = 40;
        Vector3[] centers = new Vector3[segments + 1];
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float z = Mathf.Lerp(-10.7f, 10.7f, t);
            float x = CoastEnvironmentBuilder.ShorelineX(z) + shoreOffset + Mathf.Sin(z * .71f + phase) * edgeWave;
            centers[i] = new Vector3(x, y, z);
        }
        return CreatePolylineRibbon(parent, name, centers, width, color);
    }

    static GameObject CreateOpenWaterRibbon(Transform parent, string name, float startX, float endX,
        float zCenter, float y, float width, Color color, float phase)
    {
        const int points = 14;
        Vector3[] centers = new Vector3[points];
        for (int i = 0; i < points; i++)
        {
            float t = i / (float)(points - 1);
            float x = Mathf.Lerp(startX, endX, t);
            float z = zCenter + Mathf.Sin(t * 5.1f + phase) * .20f + Mathf.Sin(t * 9.4f + phase * .6f) * .055f;
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
