using System.Collections;
using UnityEngine;

// Presentation-only sea pass for Chapter I. Gameplay/pathing remain untouched.
// The goal is a bright Aegean read at the real tactical camera: cobalt open water,
// turquoise shallows, broken white surf, broad currents and restrained sun glints.
public sealed class ChapterOneAegeanSeaPresentation : MonoBehaviour
{
    static readonly Color DeepCobalt = new Color(.025f, .19f, .43f);
    static readonly Color OpenAzure = new Color(.025f, .36f, .62f);
    static readonly Color ShelfBlue = new Color(.035f, .48f, .66f);
    static readonly Color Turquoise = new Color(.035f, .58f, .67f);
    static readonly Color Lagoon = new Color(.08f, .69f, .70f);
    static readonly Color Reef = new Color(.035f, .43f, .50f);
    static readonly Color WaveBlue = new Color(.30f, .74f, .80f);
    static readonly Color Foam = new Color(.94f, .98f, .95f);
    static readonly Color ThinFoam = new Color(.70f, .90f, .90f);
    static readonly Color SunGlint = new Color(.78f, .94f, .93f);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<ChapterOneAegeanSeaPresentation>() == null)
            new GameObject("ChapterOneAegeanSeaPresentation").AddComponent<ChapterOneAegeanSeaPresentation>();
    }

    IEnumerator Start()
    {
        GameObject coast = null;
        for (int i = 0; i < 12 && coast == null; i++)
        {
            coast = GameObject.Find("Chapter01_CoastEnvironment");
            if (coast == null) yield return null;
        }

        if (coast == null || (GameManager.Instance != null && GameManager.Instance.MapNumber != 1))
        {
            Destroy(gameObject);
            yield break;
        }

        if (GameObject.Find("Chapter01_AegeanSeaPresentation") != null)
        {
            Destroy(gameObject);
            yield break;
        }

        GameObject root = new GameObject("Chapter01_AegeanSeaPresentation");
        BuildDepthColor(root.transform);
        BuildTurquoiseShallows(root.transform);
        BuildCurrentLines(root.transform);
        BuildOpenWaterWaveCaps(root.transform);
        BuildShoreBreakers(root.transform);
        BuildSunGlints(root.transform);
    }

    static void BuildDepthColor(Transform parent)
    {
        // Thin presentation slabs sit above the existing collision-free water geometry.
        // They deliberately keep the original coast builder as structural fallback.
        Part(parent, "Deep Cobalt Water", PrimitiveType.Cube,
            new Vector3(-29.8f, -.285f, 0f), new Vector3(8.0f, .008f, 25f), DeepCobalt);
        Part(parent, "Open Azure Water", PrimitiveType.Cube,
            new Vector3(-21.9f, -.230f, 0f), new Vector3(7.9f, .008f, 25f), OpenAzure);
        Part(parent, "Turquoise Shore Water", PrimitiveType.Cube,
            new Vector3(-15.6f, -.177f, 0f), new Vector3(4.8f, .008f, 25f), Turquoise);

        // Broad, organic transition patches prevent the three depth zones from reading
        // as flat rectangular color blocks from the gameplay camera.
        float[] transitionZ = { -9.2f, -6.2f, -3.0f, .2f, 3.4f, 6.5f, 9.4f };
        for (int i = 0; i < transitionZ.Length; i++)
        {
            float z = transitionZ[i];
            Part(parent, "Azure Shelf Blend", PrimitiveType.Sphere,
                new Vector3(-18.15f + Mathf.Sin(i * 1.37f) * .22f, -.171f, z),
                new Vector3(1.15f + (i % 3) * .18f, .008f, 1.45f + (i % 2) * .32f),
                i % 2 == 0 ? ShelfBlue : Turquoise * 1.03f,
                Quaternion.Euler(0f, -9f + i * 4f, 0f));
        }
    }

    static void BuildTurquoiseShallows(Transform parent)
    {
        float[] zValues = { -9.1f, -6.8f, -4.3f, -1.5f, 1.2f, 3.9f, 6.5f, 9.0f };
        for (int i = 0; i < zValues.Length; i++)
        {
            float z = zValues[i];
            float shore = CoastEnvironmentBuilder.ShorelineX(z);
            float x = shore - 1.72f - (i % 3) * .34f;

            Part(parent, "Lagoon Light Patch", PrimitiveType.Sphere,
                new Vector3(x, -.160f, z),
                new Vector3(.72f + (i % 2) * .28f, .006f, 1.00f + (i % 3) * .24f),
                Lagoon * (.92f + (i % 2) * .05f),
                Quaternion.Euler(0f, -16f + i * 7f, 0f));

            if ((i & 1) == 0)
            {
                Part(parent, "Submerged Reef Tint", PrimitiveType.Sphere,
                    new Vector3(x - .42f, -.156f, z + .34f),
                    new Vector3(.36f + (i % 3) * .08f, .004f, .58f + (i % 2) * .14f),
                    Reef * 1.02f,
                    Quaternion.Euler(0f, 11f + i * 9f, 0f));
            }
        }
    }

    static void BuildCurrentLines(Transform parent)
    {
        for (int band = 0; band < 3; band++)
        {
            float baseZ = -6.2f + band * 6.1f;
            for (int i = 0; i < 6; i++)
            {
                float x = -25.1f + i * 1.65f;
                float z = baseZ + Mathf.Sin(i * .92f + band) * .52f;
                GameObject current = Part(parent, "Aegean Current Ribbon", PrimitiveType.Sphere,
                    new Vector3(x, -.218f + band * .012f, z),
                    new Vector3(.62f + (i % 3) * .16f, .004f, .045f + (i % 2) * .018f),
                    WaveBlue * (.72f + band * .055f),
                    Quaternion.Euler(0f, -11f + i * 3.8f, 0f));
                AddSeaMotion(current, .3f + band * 1.7f + i * .29f);
            }
        }
    }

    static void BuildOpenWaterWaveCaps(Transform parent)
    {
        for (int i = 0; i < 22; i++)
        {
            int column = i % 5;
            int row = i / 5;
            float x = -16.5f - column * 2.55f - (row % 2) * .42f;
            float z = -9.0f + ((i * 3.17f) % 18.0f);
            float width = .42f + (i % 4) * .15f;
            Color color = i % 5 == 0 ? ThinFoam : WaveBlue;
            GameObject cap = Part(parent, "Open Water Wave Cap", PrimitiveType.Sphere,
                new Vector3(x, -.158f - column * .022f, z),
                new Vector3(width, .004f, .035f + (i % 3) * .012f),
                color * (.82f + (i % 3) * .045f),
                Quaternion.Euler(0f, -13f + (i % 7) * 4f, 0f));
            AddSwellMotion(cap, 1.2f + i * .23f);
        }
    }

    static void BuildShoreBreakers(Transform parent)
    {
        BuildBrokenSurfLine(parent, -.36f, -9.6f, 13, 1.58f, .22f, .90f, Foam, .2f);
        BuildBrokenSurfLine(parent, -.88f, -8.9f, 11, 1.78f, .13f, .72f, ThinFoam, 1.1f);

        // Small bright curls make the shoreline read as water rather than a blue floor.
        for (int i = 0; i < 9; i++)
        {
            float z = -8.4f + i * 2.10f;
            float x = CoastEnvironmentBuilder.ShorelineX(z) - .18f;
            GameObject curl = Part(parent, "Bright Surf Curl", PrimitiveType.Sphere,
                new Vector3(x, .006f, z),
                new Vector3(.10f + (i % 2) * .04f, .006f, .26f + (i % 3) * .09f),
                Foam,
                Quaternion.Euler(0f, -17f + i * 6f, 0f));
            AddSurfMotion(curl, 2.0f + i * .37f);
        }
    }

    static void BuildBrokenSurfLine(Transform parent, float shoreOffset, float startZ, int count,
        float spacing, float width, float length, Color color, float phase)
    {
        for (int i = 0; i < count; i++)
        {
            if (i > 0 && i < count - 1 && i % 6 == 4) continue;
            float z = startZ + i * spacing;
            float x = CoastEnvironmentBuilder.ShorelineX(z) + shoreOffset + Mathf.Sin(i * 1.17f) * .10f;
            GameObject foam = Part(parent, "Aegean Breaking Foam", PrimitiveType.Sphere,
                new Vector3(x, .004f, z),
                new Vector3(width * (.86f + (i % 3) * .08f), .007f, length * (.78f + (i % 4) * .08f)),
                color * (.92f + (i % 2) * .055f),
                Quaternion.Euler(0f, -12f + (i % 5) * 5f, 0f));
            AddSurfMotion(foam, phase + i * .31f);
        }
    }

    static void BuildSunGlints(Transform parent)
    {
        for (int i = 0; i < 14; i++)
        {
            float x = -16.8f - (i % 5) * 2.45f;
            float z = -8.8f + ((i * 4.13f) % 17.6f);
            GameObject glint = Part(parent, "Aegean Sun Glint", PrimitiveType.Sphere,
                new Vector3(x, -.150f - (i % 5) * .025f, z),
                new Vector3(.24f + (i % 4) * .14f, .003f, .018f + (i % 2) * .012f),
                SunGlint * (.82f + (i % 3) * .07f),
                Quaternion.Euler(0f, -8f + (i % 6) * 3f, 0f));
            AddSeaMotion(glint, 3.0f + i * .19f);
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
        if (collider != null) Destroy(collider);
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