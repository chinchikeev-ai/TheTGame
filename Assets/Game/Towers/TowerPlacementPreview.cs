using UnityEngine;

public sealed class TowerPlacementPreview : MonoBehaviour
{
    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");

    MaterialPropertyBlock colorBlock;
    GameObject ghost;
    Renderer[] renderers = System.Array.Empty<Renderer>();
    TowerType currentType;
    bool hasState;
    bool lastValid;

    public void Show(BuildPoint point, TowerType type, bool valid)
    {
        if (point == null)
        {
            Hide();
            return;
        }

        if (ghost == null || type != currentType)
            Rebuild(type);

        currentType = type;
        ghost.SetActive(true);
        ghost.transform.position = point.transform.position + Vector3.up * .5f;

        if (!hasState || lastValid != valid)
        {
            SetGhostState(valid);
            hasState = true;
            lastValid = valid;
        }
    }

    public void Hide()
    {
        if (ghost != null) ghost.SetActive(false);
    }

    void Rebuild(TowerType type)
    {
        if (ghost != null) Destroy(ghost);
        ghost = TowerFactory.CreateTower(Vector3.zero, type, "PlacementGhost");
        currentType = type;
        hasState = false;

        MonoBehaviour[] behaviours = ghost.GetComponentsInChildren<MonoBehaviour>(true);
        for (int i = 0; i < behaviours.Length; i++)
            behaviours[i].enabled = false;

        Collider[] colliders = ghost.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = false;

        renderers = ghost.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Material material = renderers[i].material;
            if (material.HasProperty("_Surface")) material.SetFloat("_Surface", 1f);
            if (material.HasProperty("_SrcBlend")) material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            if (material.HasProperty("_DstBlend")) material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            if (material.HasProperty("_ZWrite")) material.SetFloat("_ZWrite", 0f);
            material.renderQueue = 3000;
        }
    }

    void SetGhostState(bool valid)
    {
        if (colorBlock == null) colorBlock = new MaterialPropertyBlock();
        Color tint = valid ? new Color(.35f, 1f, .45f, .42f) : new Color(1f, .26f, .18f, .42f);
        colorBlock.Clear();
        colorBlock.SetColor(BaseColorId, tint);
        colorBlock.SetColor(ColorId, tint);

        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i] != null) renderers[i].SetPropertyBlock(colorBlock);
    }

    void OnDestroy()
    {
        if (ghost != null) Destroy(ghost);
    }
}
