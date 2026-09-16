using System.Collections;
using UnityEngine;

// Presentation-only closure for the north/south edges of the Chapter I coast.
// The authored shoreline remains untouched; these slabs only hide the finite sea
// planes where the gameplay camera can see beyond the dressed beach corridor.
public sealed class ChapterOneCoastEdgeClosure : MonoBehaviour
{
    static readonly Color Sand = new Color(.70f,.58f,.37f);
    static readonly Color LightSand = new Color(.82f,.69f,.44f);
    static readonly Color Rock = new Color(.35f,.34f,.30f);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<ChapterOneCoastEdgeClosure>() == null)
            new GameObject("ChapterOneCoastEdgeClosure").AddComponent<ChapterOneCoastEdgeClosure>();
    }

    IEnumerator Start()
    {
        yield return null;
        yield return null;

        if (GameManager.Instance != null && GameManager.Instance.MapNumber != 1)
        {
            Destroy(gameObject);
            yield break;
        }

        if (GameObject.Find("Chapter01_CoastEdgeClosure") != null)
        {
            Destroy(gameObject);
            yield break;
        }

        GameObject root = new GameObject("Chapter01_CoastEdgeClosure");
        BuildEdge(root.transform, -17.4f, -1f);
        BuildEdge(root.transform, 17.4f, 1f);
    }

    static void BuildEdge(Transform parent, float z, float side)
    {
        Part(parent, "Coast Side Sand Extension", PrimitiveType.Cube,
            new Vector3(-1.5f, -.155f, z), new Vector3(45f, .16f, 12.8f), Sand);

        Part(parent, "Coast Side Light Sand", PrimitiveType.Cube,
            new Vector3(-7.0f, -.070f, z - side * 4.8f), new Vector3(16f, .025f, 3.2f), LightSand * .94f);

        for (int i = 0; i < 7; i++)
        {
            float x = -11.5f + i * 4.1f;
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
