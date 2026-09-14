using UnityEngine;

public sealed class HectorCommandFeedback : MonoBehaviour
{
    const float Lifetime = .65f;
    float bornAt;
    LineRenderer ring;
    Material material;

    public static void ShowMove(Vector3 worldPosition)
    {
        GameObject go = new GameObject("HectorMoveCommand");
        go.transform.position = new Vector3(worldPosition.x, .08f, worldPosition.z);
        go.AddComponent<HectorCommandFeedback>().Build();
    }

    void Build()
    {
        bornAt = Time.unscaledTime;
        ring = gameObject.AddComponent<LineRenderer>();
        ring.useWorldSpace = false;
        ring.loop = true;
        ring.positionCount = 33;
        ring.widthMultiplier = .07f;
        ring.numCornerVertices = 3;
        ring.numCapVertices = 2;

        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader != null)
        {
            material = new Material(shader);
            material.color = new Color(1f, .72f, .20f, .95f);
            ring.material = material;
        }

        for (int i = 0; i < ring.positionCount; i++)
        {
            float angle = i / (float)(ring.positionCount - 1) * Mathf.PI * 2f;
            ring.SetPosition(i, new Vector3(Mathf.Cos(angle) * .55f, 0f, Mathf.Sin(angle) * .55f));
        }
    }

    void Update()
    {
        if (ring == null) return;
        float t = Mathf.Clamp01((Time.unscaledTime - bornAt) / Lifetime);
        float scale = Mathf.Lerp(.65f, 1.35f, t);
        transform.localScale = new Vector3(scale, 1f, scale);
        ring.widthMultiplier = Mathf.Lerp(.08f, .025f, t);
        if (material != null)
        {
            Color color = material.color;
            color.a = 1f - t;
            material.color = color;
        }
        if (t >= 1f) Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (material != null) Destroy(material);
    }
}
