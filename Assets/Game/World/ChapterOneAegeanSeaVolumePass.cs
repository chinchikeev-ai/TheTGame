using System.Collections;
using UnityEngine;

// Second-stage presentation pass for Chapter I sea.
// Uses mesh ribbons instead of stretched primitive spheres so the shoreline reads
// as continuous water from the tactical camera rather than as discrete blobs.
public sealed class ChapterOneAegeanSeaVolumePass : MonoBehaviour
{
    static readonly Color WaveFace = new Color(.035f, .50f, .61f);
    static readonly Color WaveLit = new Color(.13f, .67f, .72f);
    static readonly Color Foam = new Color(.96f, .985f, .96f);
    static readonly Color ThinFoam = new Color(.73f, .91f, .90f);
    static readonly Color WakeBlue = new Color(.22f, .68f, .75f);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<ChapterOneAegeanSeaVolumePass>() == null)
            new GameObject("ChapterOneAegeanSeaVolumePass").AddComponent<ChapterOneAegeanSeaVolumePass>();
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

        if (GameObject.Find("Chapter01_AegeanSeaVolumePass") != null)
        {
            Destroy(gameObject);
            yield break;
        }

        GameObject root = new GameObject("Chapter01_AegeanSeaVolumePass");
        BuildRaisedBreakers(root.transform);
        BuildRockWash(root.transform);
        BuildShipWakes(root.transform);
    }

    static void BuildRaisedBreakers(Transform parent)
    {
        GameObject bodyOuter = CreateCoastRibbon(parent, "Raised Aegean Breaker Body", -.88f, -.119f, .22f, WaveFace, .12f, .4f);
        GameObject bodyInner = CreateCoastRibbon(parent, "Raised Aegean Breaker Body", -.62f, -.104f, .15f, WaveLit, .10f, 1.2f);
        GameObject crestOuter = CreateCoastRibbon(parent, "Raised Aegean White Crest", -.72f, -.069f, .065f, ThinFoam, .11f, .8f);
        GameObject crestInner = CreateCoastRibbon(parent, "Raised Aegean White Crest", -.45f, -.057f, .090f, Foam, .13f, 1.7f);
        GameObject trailing = CreateCoastRibbon(parent, "Breaker Trailing Foam", -.24f, -.018f, .050f, ThinFoam, .08f, 2.5f);

        AddSwellMotion(bodyOuter, .4f);
        AddSwellMotion(bodyInner, 1.1f);
        AddSurfMotion(crestOuter, .9f);
        AddSurfMotion(crestInner, 1.6f);
        AddSurfMotion(trailing, 2.3f);
    }

    static void BuildRockWash(Transform parent)
    {
        float[] shoreRockZ = { -9.1f, -6.7f, 6.9f, 9.0f };
        for (int i = 0; i < shoreRockZ.Length; i++)
        {
            float z = shoreRockZ[i];
            float x = CoastEnvironmentBuilder.ShorelineX(z) + 1.25f + (i % 2) * .35f;

            Vector3 center = new Vector3(x, .008f, z);
            GameObject foamArc = CreateArcRibbon(parent, "Boulder Wash Foam", center, .46f, .31f, -145f, 145f, .055f, i % 2 == 0 ? Foam : ThinFoam);
            GameObject backwash = CreateSegmentRibbon(parent, "Boulder Backwash",
                center + new Vector3(-.28f, -.018f, -.28f),
                center + new Vector3(-.62f, -.018f, .28f),
                .075f,
                WaveLit * .86f);
            AddSurfMotion(foamArc, 1.4f + i * .7f);
            AddSeaMotion(backwash, 3.0f + i * .5f);
        }
    }

    static void BuildShipWakes(Transform parent)
    {
        Vector3[] ships =
        {
            new Vector3(-19.0f, .10f, 5.8f),
            new Vector3(-20.4f, .10f, .5f),
            new Vector3(-18.5f, .10f, -5.8f)
        };
        float[] yaw = { -8f, 5f, -4f };

        for (int i = 0; i < ships.Length; i++)
        {
            GameObject wakeRoot = new GameObject("Greek Landing Ship Hero Wake");
            wakeRoot.transform.SetParent(parent, false);
            wakeRoot.transform.localPosition = new Vector3(ships[i].x, -.142f, ships[i].z);
            wakeRoot.transform.localRotation = Quaternion.Euler(0f, yaw[i], 0f);

            Vector3 centerA = new Vector3(-1.0f, 0f, 0f);
            Vector3 centerB = new Vector3(-3.8f, 0f, 0f);
            GameObject center = CreateSegmentRibbon(wakeRoot.transform, "Ship Wake Churn", centerA, centerB, .20f, ThinFoam * .94f);
            AddSeaMotion(center, .8f + i * .7f);

            for (int side = -1; side <= 1; side += 2)
            {
                Vector3 armA = new Vector3(-1.15f, 0f, side * .18f);
                Vector3 armB = new Vector3(-4.25f, 0f, side * 1.18f);
                GameObject arm = CreateSegmentRibbon(wakeRoot.transform, "Ship Wake V Arm", armA, armB, .085f, ThinFoam);
                AddSwellMotion(arm, 2.1f + i * .6f + side * .10f);

                Vector3 blueA = new Vector3(-1.35f, -.006f, side * .12f);
                Vector3 blueB = new Vector3(-3.85f, -.006f, side * .80f);
                GameObject edge = CreateSegmentRibbon(wakeRoot.transform, "Ship Wake Blue Edge", blueA, blueB, .045f, WakeBlue * .88f);
                AddSeaMotion(edge, 3.1f + i * .5f + side * .12f);
            }
        }
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
            float x = CoastEnvironmentBuilder.ShorelineX(z) + shoreOffset + Mathf.Sin(z * .67f + phase) * edgeWave;
            centers[i] = new Vector3(x, y, z);
        }
        return CreatePolylineRibbon(parent, name, centers, width, color);
    }

    static GameObject CreateArcRibbon(Transform parent, string name, Vector3 center, float radiusX,
        float radiusZ, float startDegrees, float endDegrees, float width, Color color)
    {
        const int points = 13;
        Vector3[] centers = new Vector3[points];
        for (int i = 0; i < points; i++)
        {
            float t = i / (float)(points - 1);
            float angle = Mathf.Lerp(startDegrees, endDegrees, t) * Mathf.Deg2Rad;
            centers[i] = center + new Vector3(Mathf.Cos(angle) * radiusX, 0f, Mathf.Sin(angle) * radiusZ);
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

        for (int i = 0; i < centers.Length - 1; i++)
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

    static void AddSwellMotion(GameObject go, float phase)
    {
        if (go == null) return;
        ChapterOneAmbientMotion motion = go.AddComponent<ChapterOneAmbientMotion>();
        motion.kind = ChapterOneAmbientMotion.MotionKind.Swell;
        motion.phase = phase;
    }
}
