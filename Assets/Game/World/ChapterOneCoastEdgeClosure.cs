using System.Collections;
using UnityEngine;

// Presentation-only closure for the north/south edges of the Chapter I coast.
// Sea and land are extended separately so the off-map sand cannot cover the water
// in the real tactical camera.
public sealed class ChapterOneCoastEdgeClosure : MonoBehaviour
{
    Transform coastRoot;
    public Transform Root { get; private set; }

    public void Initialize(Transform coast)
    {
        coastRoot = coast;
    }

    static readonly Color Sand = new Color(.70f,.58f,.37f);
    static readonly Color LightSand = new Color(.82f,.69f,.44f);
    static readonly Color Rock = new Color(.35f,.34f,.30f);
    static readonly Color EdgeSea = new Color(.035f,.34f,.60f);

    IEnumerator Start()
    {
        yield return null;
        yield return null;

        if (coastRoot == null ||
            (GameManager.Instance != null && GameManager.Instance.MapNumber != 1))
        {
            Destroy(gameObject);
            yield break;
        }

        GameObject root = new GameObject("Chapter01_CoastEdgeClosure");
        Root = root.transform;
        BuildEdge(root.transform, -17.4f, -1f);
        BuildEdge(root.transform, 17.4f, 1f);
    }

    static void BuildEdge(Transform parent, float z, float side)
    {
        // Water occupies the western/left side of Chapter I. Keep the sea continuous
        // beyond the authored corridor instead of hiding it with one huge sand slab.
        Part(parent, "Coast Side Sea Extension", PrimitiveType.Cube,
            new Vector3(-25.5f, -.31f, z), new Vector3(25f, .05f, 12.8f), EdgeSea);

        // Land extension begins to the east of the shoreline. Its left edge is around
        // x=-12.6, close to ShorelineX(), so it can no longer sit on top of the sea.
        Part(parent, "Coast Side Sand Extension", PrimitiveType.Cube,
            new Vector3(5.2f, -.155f, z), new Vector3(35.6f, .16f, 12.8f), Sand);

        Part(parent, "Coast Side Light Sand", PrimitiveType.Cube,
            new Vector3(-7.0f, -.070f, z - side * 4.8f), new Vector3(10.6f, .025f, 3.2f), LightSand * .94f);

        // Edge rocks also stay land-side. None may extend into open water.
        for (int i = 0; i < 6; i++)
        {
            float x = -9.4f + i * 4.2f;
            float stagger = (i % 2 == 0 ? .55f : -.35f) * side;
            GameObject rock = Part(parent, "Coast Edge Rock", PrimitiveType.Sphere,
                new Vector3(x, -.01f, z - side * (4.5f + stagger)),
                new Vector3(.75f + (i % 3) * .20f, .24f, .55f + (i % 2) * .18f),
                Rock * (.88f + (i % 3) * .05f));
            rock.transform.rotation = Quaternion.Euler(0f, i * 31f + side * 9f, 0f);
        }
    }

    static GameObject Part(Transform parent, string name, PrimitiveType type, Vector3 position, Vector3 scale, Color color)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.position = position;
        go.transform.localScale = scale;
        Collider collider = go.GetComponent<Collider>();
        if (collider != null) Destroy(collider);
        TowerFactory.SetColor(go, color);
        return go;
    }
}
