using UnityEngine;

public class TowerRangeIndicator : MonoBehaviour
{
    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");

    readonly MaterialPropertyBlock colorBlock = new MaterialPropertyBlock();
    GameObject ring;
    Renderer ringRenderer;
    Tower selected;
    bool previewMode;

    void Awake()
    {
        ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "TowerRangeIndicator";
        Destroy(ring.GetComponent<Collider>());
        ring.transform.localScale = Vector3.zero;
        ringRenderer = ring.GetComponent<Renderer>();
        TowerFactory.SetColor(ring, new Color(.20f, .75f, 1f, .28f));
        ring.SetActive(false);
    }

    public void Show(Tower tower)
    {
        previewMode = false;
        selected = tower;
        if (tower == null)
        {
            ring.SetActive(false);
            return;
        }

        ApplyColor(new Color(.20f, .75f, 1f, .28f));
        ring.SetActive(true);
        ring.transform.position = tower.transform.position + new Vector3(0f, .03f, 0f);
        ring.transform.localScale = new Vector3(tower.range * 2f, .025f, tower.range * 2f);
    }

    public void ShowPreview(Vector3 position, float range, bool valid)
    {
        selected = null;
        previewMode = true;
        ring.SetActive(true);
        ApplyColor(valid ? new Color(.18f, .86f, .34f, .26f) : new Color(.92f, .16f, .10f, .30f));
        ring.transform.position = position + new Vector3(0f, .03f, 0f);
        ring.transform.localScale = new Vector3(range * 2f, .025f, range * 2f);
    }

    public void Hide()
    {
        selected = null;
        previewMode = false;
        if (ring != null) ring.SetActive(false);
    }

    void LateUpdate()
    {
        if (!previewMode && selected != null && ring != null && ring.activeSelf)
            ring.transform.position = selected.transform.position + new Vector3(0f, .03f, 0f);
    }

    void ApplyColor(Color color)
    {
        if (ringRenderer == null) return;
        colorBlock.Clear();
        colorBlock.SetColor(BaseColorId, color);
        colorBlock.SetColor(ColorId, color);
        ringRenderer.SetPropertyBlock(colorBlock);
    }

    void OnDestroy()
    {
        if (ring != null) Destroy(ring);
    }
}
