using UnityEngine;

// Lightweight procedural deformation for the single Chapter I ocean mesh.
// It animates geometry only; there are no colliders and no gameplay hooks.
public sealed class ChapterOneSeaSurfaceMotion : MonoBehaviour
{
    Mesh mesh;
    Vector3[] baseVertices;
    Vector3[] workingVertices;
    float nextUpdate;

    public void Initialize(Mesh target)
    {
        mesh = target;
        if (mesh == null) return;
        baseVertices = mesh.vertices;
        workingVertices = new Vector3[baseVertices.Length];
        System.Array.Copy(baseVertices, workingVertices, baseVertices.Length);
        mesh.MarkDynamic();
    }

    void Update()
    {
        if (mesh == null || baseVertices == null || baseVertices.Length == 0) return;
        if (Time.time < nextUpdate) return;
        nextUpdate = Time.time + (1f / 30f);

        float time = Time.time;
        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 source = baseVertices[i];
            float shoreline = CoastEnvironmentBuilder.ShorelineX(source.z) - .10f;
            float openWater = Mathf.Clamp01((shoreline - source.x) / 9f);
            float amplitude = Mathf.Lerp(.006f, .045f, openWater);

            float broad = Mathf.Sin(source.z * .48f + source.x * .13f + time * .72f) * amplitude;
            float cross = Mathf.Sin(source.z * 1.08f - source.x * .19f + time * 1.08f) * amplitude * .34f;
            float ripple = Mathf.Cos(source.z * .76f + source.x * .31f - time * .51f) * amplitude * .18f;

            workingVertices[i] = new Vector3(source.x, source.y + broad + cross + ripple, source.z);
        }

        mesh.vertices = workingVertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
