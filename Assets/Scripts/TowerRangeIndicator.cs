using UnityEngine;

public class TowerRangeIndicator : MonoBehaviour
{
    GameObject ring;
    Tower selected;

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
        selected = tower;
        if (tower == null)
        {
            ring.SetActive(false);
            return;
        }
        ring.SetActive(true);
        ring.transform.position = tower.transform.position + new Vector3(0f, 0.03f, 0f);
        ring.transform.localScale = new Vector3(tower.range * 2f, 0.025f, tower.range * 2f);
    }

    public void Hide()
    {
        selected = null;
        if (ring != null) ring.SetActive(false);
    }

    void LateUpdate()
    {
        if (selected != null && ring != null && ring.activeSelf)
            ring.transform.position = selected.transform.position + new Vector3(0f, 0.03f, 0f);
    }
}
