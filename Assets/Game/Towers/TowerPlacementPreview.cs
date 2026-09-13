using UnityEngine;

public sealed class TowerPlacementPreview : MonoBehaviour
{
    GameObject ghost;
    TowerType currentType;
    BuildPoint currentPoint;

    public void Show(BuildPoint point, TowerType type, bool valid)
    {
        if (point == null)
        {
            Hide();
            return;
        }

        if (ghost == null || type != currentType)
        {
            Rebuild(type);
        }

        currentPoint = point;
        currentType = type;
        ghost.SetActive(true);
        ghost.transform.position = point.transform.position + Vector3.up * .5f;
        SetGhostState(valid);
    }

    public void Hide()
    {
        currentPoint = null;
        if (ghost != null) ghost.SetActive(false);
    }

    void Rebuild(TowerType type)
    {
        if (ghost != null) Destroy(ghost);
        ghost = TowerFactory.CreateTower(Vector3.zero, type, "PlacementGhost");
        currentType = type;

        Tower tower = ghost.GetComponent<Tower>();
        if (tower != null) Destroy(tower);
        Collider[] colliders = ghost.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < colliders.Length; i++) Destroy(colliders[i]);

        Renderer[] renderers = ghost.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            Material material = renderer.material;
            if (material.HasProperty("_Surface")) material.SetFloat("_Surface", 1f);
            if (material.HasProperty("_BaseColor"))
            {
                Color c = material.GetColor("_BaseColor");
                c.a = .42f;
                material.SetColor("_BaseColor", c);
            }
            if (material.HasProperty("_Color"))
            {
                Color c = material.GetColor("_Color");
                c.a = .42f;
                material.SetColor("_Color", c);
            }
            if (material.HasProperty("_SrcBlend")) material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            if (material.HasProperty("_DstBlend")) material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            if (material.HasProperty("_ZWrite")) material.SetFloat("_ZWrite", 0f);
            material.renderQueue = 3000;
        }

        MonoBehaviour[] behaviours = ghost.GetComponentsInChildren<MonoBehaviour>(true);
        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] is ChapterOneAmbientMotion) behaviours[i].enabled = false;
        }
    }

    void SetGhostState(bool valid)
    {
        if (ghost == null) return;
        Color tint = valid ? new Color(.35f, 1f, .45f, .42f) : new Color(1f, .26f, .18f, .42f);
        Renderer[] renderers = ghost.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Material material = renderers[i].material;
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", tint);
            if (material.HasProperty("_Color")) material.SetColor("_Color", tint);
        }
    }
}
