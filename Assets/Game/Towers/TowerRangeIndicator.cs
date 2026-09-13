using UnityEngine;

public class TowerRangeIndicator : MonoBehaviour
{
    GameObject ring;
    Tower selected;
    bool previewMode;

    void Awake()
    {
        ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "TowerRangeIndicator";
        Destroy(ring.GetComponent<Collider>());
        ring.transform.localScale = Vector3.zero;
        TowerFactory.SetColor(ring, new Color(0.2f, 0.75f, 1f, 0.28f));
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
        ring.transform.position = tower.transform.position + new Vector3(0f, 0.03f, 0f);
        ring.transform.localScale = new Vector3(tower.range * 2f, 0.025f, tower.range * 2f);
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
            ring.transform.position = selected.transform.position + new Vector3(0f, 0.03f, 0f);
    }

    void ApplyColor(Color color)
    {
        if (ring == null) return;
        Renderer renderer = ring.GetComponent<Renderer>();
        if (renderer == null) return;
        Material material = renderer.material;
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color")) material.SetColor("_Color", color);
    }
}
