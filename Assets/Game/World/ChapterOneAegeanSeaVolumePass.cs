using System.Collections;
using UnityEngine;

// Second-stage presentation pass for Chapter I sea. This adds visible water volume
// at tactical camera distance: raised breaker bodies, foam wrapping shore boulders,
// and readable V-shaped wakes behind the Greek landing ships. Gameplay geometry,
// routes, build cells and colliders are intentionally untouched.
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
        float[] zValues = { -9.1f, -7.0f, -4.7f, -2.4f, .1f, 2.6f, 5.0f, 7.2f, 9.25f };
        for (int i = 0; i < zValues.Length; i++)
        {
            float z = zValues[i];
            float shore = CoastEnvironmentBuilder.ShorelineX(z);
            float phase = .45f + i * .47f;

            // The body is intentionally tall enough to read as an actual wave ridge,
            // while remaining far below unit silhouettes and tactical telegraphs.
            GameObject body = Part(parent, "Raised Aegean Breaker Body", PrimitiveType.Sphere,
                new Vector3(shore - 1.12f, -.118f, z),
                new Vector3(.24f + (i % 3) * .025f, .075f, .78f + (i % 4) * .14f),
                i % 2 == 0 ? WaveFace : WaveLit,
                Quaternion.Euler(0f, -8f + (i % 5) * 4f, 0f));
            AddSwellMotion(body, phase);

            GameObject crest = Part(parent, "Raised Aegean White Crest", PrimitiveType.Sphere,
                new Vector3(shore - .94f, -.052f, z + .03f),
                new Vector3(.070f + (i % 2) * .012f, .024f, .70f + (i % 4) * .13f),
                Foam,
                Quaternion.Euler(0f, -10f + (i % 5) * 4f, 0f));
            AddSurfMotion(crest, phase + .22f);

            if ((i & 1) == 0)
            {
                GameObject trailingFoam = Part(parent, "Breaker Trailing Foam", PrimitiveType.Sphere,
                    new Vector3(shore - .62f, -.010f, z - .08f),
                    new Vector3(.055f, .009f, .46f + (i % 3) * .10f),
                    ThinFoam,
                    Quaternion.Euler(0f, -14f + i * 3f, 0f));
                AddSurfMotion(trailingFoam, phase + .48f);
            }
        }
    }

    static void BuildRockWash(Transform parent)
    {
        float[] shoreRockZ = { -9.1f, -6.7f, 6.9f, 9.0f };
        for (int i = 0; i < shoreRockZ.Length; i++)
        {
            float z = shoreRockZ[i];
            float x = CoastEnvironmentBuilder.ShorelineX(z) + 1.25f + (i % 2) * .35f;

            GameObject root = new GameObject("Shore Boulder Foam Wrap");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = new Vector3(x, 0f, z);

            for (int j = 0; j < 5; j++)
            {
                float angle = (-115f + j * 56f) * Mathf.Deg2Rad;
                float radiusX = .42f + (j % 2) * .09f;
                float radiusZ = .34f + ((j + 1) % 2) * .08f;
                Vector3 local = new Vector3(Mathf.Cos(angle) * radiusX, .012f, Mathf.Sin(angle) * radiusZ);
                GameObject foam = Part(root.transform, "Boulder Wash Foam", PrimitiveType.Sphere,
                    local,
                    new Vector3(.12f + (j % 3) * .025f, .010f, .22f + (j % 2) * .08f),
                    j == 2 ? Foam : ThinFoam,
                    Quaternion.Euler(0f, j * 37f + i * 11f, 0f));
                AddSurfMotion(foam, 1.4f + i * .8f + j * .24f);
            }

            GameObject wash = Part(root.transform, "Boulder Backwash", PrimitiveType.Sphere,
                new Vector3(-.34f, -.016f, 0f),
                new Vector3(.34f, .007f, .52f),
                WaveLit * .88f,
                Quaternion.Euler(0f, 4f - i * 3f, 0f));
            AddSeaMotion(wash, 3.2f + i * .51f);
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
            wakeRoot.transform.localPosition = new Vector3(ships[i].x, -.145f, ships[i].z);
            wakeRoot.transform.localRotation = Quaternion.Euler(0f, yaw[i], 0f);

            // Ship local +X is the prow, so the wake stretches into local -X.
            for (int j = 0; j < 4; j++)
            {
                float x = -1.65f - j * .72f;
                GameObject churn = Part(wakeRoot.transform, "Ship Wake Churn", PrimitiveType.Sphere,
                    new Vector3(x, .004f, Mathf.Sin(j * 1.3f) * .035f),
                    new Vector3(.52f + j * .10f, .014f, .23f + j * .055f),
                    j == 0 ? Foam * .93f : ThinFoam * (.92f - j * .035f),
                    Quaternion.Euler(0f, j % 2 == 0 ? -2f : 2f, 0f));
                AddSeaMotion(churn, .8f + i * .9f + j * .22f);
            }

            for (int side = -1; side <= 1; side += 2)
            {
                GameObject arm = Part(wakeRoot.transform, "Ship Wake V Arm", PrimitiveType.Sphere,
                    new Vector3(-2.45f, -.002f, side * .52f),
                    new Vector3(1.85f, .010f, .075f),
                    ThinFoam,
                    Quaternion.Euler(0f, side * 9f, 0f));
                AddSwellMotion(arm, 2.1f + i * .7f + side * .12f);

                GameObject blueEdge = Part(wakeRoot.transform, "Ship Wake Blue Edge", PrimitiveType.Sphere,
                    new Vector3(-2.15f, -.010f, side * .34f),
                    new Vector3(1.55f, .007f, .045f),
                    WakeBlue * .90f,
                    Quaternion.Euler(0f, side * 8f, 0f));
                AddSeaMotion(blueEdge, 3.3f + i * .6f + side * .16f);
            }
        }
    }

    static GameObject Part(Transform parent, string name, PrimitiveType type, Vector3 position,
        Vector3 scale, Color color, Quaternion? rotation = null)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = position;
        go.transform.localScale = scale;
        if (rotation.HasValue) go.transform.localRotation = rotation.Value;
        Collider collider = go.GetComponent<Collider>();
        if (collider != null) Object.Destroy(collider);
        TowerFactory.SetColor(go, color);
        return go;
    }

    static void AddSeaMotion(GameObject go, float phase)
    {
        ChapterOneAmbientMotion motion = go.AddComponent<ChapterOneAmbientMotion>();
        motion.kind = ChapterOneAmbientMotion.MotionKind.Sea;
        motion.phase = phase;
    }

    static void AddSurfMotion(GameObject go, float phase)
    {
        ChapterOneAmbientMotion motion = go.AddComponent<ChapterOneAmbientMotion>();
        motion.kind = ChapterOneAmbientMotion.MotionKind.Surf;
        motion.phase = phase;
    }

    static void AddSwellMotion(GameObject go, float phase)
    {
        ChapterOneAmbientMotion motion = go.AddComponent<ChapterOneAmbientMotion>();
        motion.kind = ChapterOneAmbientMotion.MotionKind.Swell;
        motion.phase = phase;
    }
}
