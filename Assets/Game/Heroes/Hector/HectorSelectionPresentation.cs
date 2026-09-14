using UnityEngine;

public sealed class HectorSelectionPresentation : MonoBehaviour
{
    HectorController hector;
    LineRenderer ring;
    Material material;

    public void Initialize(HectorController controller)
    {
        hector = controller;
        EnsureRing();
        Refresh();
    }

    void Awake()
    {
        if (hector == null) hector = GetComponent<HectorController>();
    }

    void LateUpdate()
    {
        if (hector == null) return;
        Refresh();
        if (ring != null && ring.enabled)
        {
            float pulse = .5f + .5f * Mathf.Sin(Time.unscaledTime * 5f);
            ring.widthMultiplier = Mathf.Lerp(.055f, .085f, pulse);
        }
    }

    void EnsureRing()
    {
        Transform existing = transform.Find("HectorSelectionRing");
        GameObject go;
        if (existing != null)
        {
            go = existing.gameObject;
        }
        else
        {
            go = new GameObject("HectorSelectionRing");
            go.transform.SetParent(transform, false);
        }

        go.transform.localPosition = new Vector3(0f, -.50f, 0f);
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        ring = go.GetComponent<LineRenderer>();
        if (ring == null) ring = go.AddComponent<LineRenderer>();
        ring.useWorldSpace = false;
        ring.loop = true;
        ring.positionCount = 49;
        ring.numCornerVertices = 3;
        ring.numCapVertices = 2;
        ring.widthMultiplier = .07f;

        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader != null && material == null)
        {
            material = new Material(shader);
            material.color = new Color(1f, .72f, .18f, .95f);
            ring.material = material;
        }

        const float radius = .78f;
        for (int i = 0; i < ring.positionCount; i++)
        {
            float angle = i / (float)(ring.positionCount - 1) * Mathf.PI * 2f;
            ring.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius));
        }
    }

    void Refresh()
    {
        if (ring == null) EnsureRing();
        if (ring != null) ring.enabled = hector != null && hector.Selected && !hector.IsDowned;
    }

    void OnDestroy()
    {
        if (material != null) Destroy(material);
    }
}
