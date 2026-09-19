using UnityEngine;
using UnityEngine.UI;

// UI stencil geometry keeps atlas marker crops circular without modifying source art.
public sealed class ChapterNodeMask : MaskableGraphic
{
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect r = rectTransform.rect;
        Vector2 center = r.center;
        vh.AddVert(center, Color.white, Vector2.zero);
        const int segments = 64;
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            vh.AddVert(center + new Vector2(Mathf.Cos(angle) * r.width * .5f, Mathf.Sin(angle) * r.height * .5f), Color.white, Vector2.zero);
            if (i > 0) vh.AddTriangle(0, i, i + 1);
        }
    }
}
