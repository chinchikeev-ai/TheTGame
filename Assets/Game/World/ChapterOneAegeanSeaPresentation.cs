using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Presentation-only sea pass for Chapter I.
// This version deliberately avoids primitive spheres/cubes for water detail because
// they read as circles and hard rectangular bands from the real tactical camera.
// Water detail is built from flat authored mesh ribbons instead.
public sealed class ChapterOneAegeanSeaPresentation : MonoBehaviour
{
    static readonly Color DeepCobalt = new Color(.025f, .18f, .40f);
    static readonly Color OpenAzure = new Color(.025f, .34f, .60f);
    static readonly Color ShelfBlue = new Color(.035f, .47f, .66f);
    static readonly Color Turquoise = new Color(.035f, .58f, .67f);
    static readonly Color Lagoon = new Color(.09f, .69f, .70f);
    static readonly Color WaveBlue = new Color(.31f, .73f, .80f);
    static readonly Color Foam = new Color(.95f, .985f, .96f);
    static readonly Color ThinFoam = new Color(.73f, .90f, .90f);
    static readonly Color SunGlint = new Color(.82f, .95f, .93f);

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

        // Let ChapterOneShoreLife finish its own runtime construction, then remove
        // the old blob-based water accents that are visibly wrong from gameplay view.
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
        BuildShallowDepthBands(root.transform);
        BuildOpenWaterCurrents(root.transform);
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
        Recolor("Deep Aegean Sea", DeepCobalt);
        Recolor("Aegean Mid Water", OpenAzure);
        Recolor("Aegean Shallows", Turquoise * .93f);
        Recolor("Aegean Shelf Transition", ShelfBlue);
        Recolor("Aegean Mid Shelf", OpenAzure * 1.04f);
        Recolor("Shore Shallow Gradient Band", Lagoon * .92f);
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

    static void BuildShallowDepthBands(Transform parent)
    {
        CreateCoastBand(parent, "Aegean Turquoise Shelf", -4.35f, -1.75f, -.151f, ShelfBlue * .92f, .10f, .3f);
        CreateCoastBand(parent, "Aegean Lagoon Edge", -2.05f, -.74f, -.145f, Lagoon * .86f, .08f, 1.2f);
        CreateCoastBand(parent, "Aegean Nearshore Light", -1.24f, -.34f, -.139f, Turquoise * 1.03f, .06f, 2.1f);
    }

    static void BuildOpenWaterCurrents(Transform parent)
    {
        float[] zBands = { -7.4f, -4.0f, -.6f, 2.8f, 6.2f };
        for (int i = 0; i < zBands.Length; i++)
        {
            GameObject current = CreateOpenWaterRibbon(parent,
                "Aegean Current Ribbon",
                -28.5f,
                -15.6f,
                zBands[i],
                -.138f - (i % 2) * .008f,
                .055f + (i % 3) * .012f,
                WaveBlue * (.70f + i * .035f),
                i * .83f);
            AddSeaMotion(current, .5f + i * .64f);
        }
    }

    static void BuildShoreBreakers(Transform parent)
    {
        GameObject outer = CreateCoastRibbon(parent, "Aegean Breaking Foam", -.96f, -.135f, .115f, ThinFoam, .13f, .2f);
        GameObject main = CreateCoastRibbon(parent, "Aegean Breaking Foam", -.48f, -.126f, .165f, Foam, .15f, 1.1f);
        GameObject inner = CreateCoastRibbon(parent, "Bright Surf Curl", -.18f, -.120f, .075f, Foam * .96f, .11f, 2.0f);
        AddSurfMotion(outer, .4f);
        AddSurfMotion(main, 1.3f);
        AddSurfMotion(inner, 2.2f);
    }

    static void BuildSunGlints(Transform parent)
    {
        for (int i = 0; i < 12; i++)
        {
            float x = -16.4f - (i % 4) * 2.65f - (i / 4) * .35f;
            float z = -8.6f + ((i * 4.07f) % 17.2f);
            Vector3 a = new Vector3(x, -.129f, z);
            Vector3 b = a + new Vector3(.42f + (i % 3) * .18f, 0f, .05f + (i % 2) * .05f);
            GameObject glint = CreateSegmentRibbon(parent, "Aegean Sun Glint", a, b, .025f + (i % 2) * .010f, SunGlint * (.80f + (i % 3) * .065f));
            AddSeaMotion(glint, 3.0f + i * .19f);
        }
    }

    static GameObject CreateCoastBand(Transform parent, string name, float leftOffset, float rightOffset,
        float y, Color color, float edgeWave, float phase)
    {
        const int segments = 34;
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
        const int segments = 34;
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
        const int points = 12;
        Vector3[] centers = new Vector3[points];
        for (int i = 0; i < points; i++)
        {
            float t = i / (float)(points - 1);
            float x = Mathf.Lerp(startX, endX, t);
            float z = zCenter + Mathf.Sin(t * 5.1f + phase) * .22f + Mathf.Sin(t * 9.4f + phase * .6f) * .06f;
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
