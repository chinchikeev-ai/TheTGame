using UnityEngine;

public class BuildPoint : MonoBehaviour
{
    public bool Occupied { get; private set; }
    public Tower Tower { get; private set; }

    Renderer marker;
    Renderer rim;
    bool hovered;
    readonly Color idleColor = new Color(.78f,.58f,.20f);
    readonly Color rimColor = new Color(.30f,.22f,.10f);
    readonly Color hoverColor = new Color(.22f,.82f,.34f);
    readonly Color occupiedColor = new Color(.16f,.16f,.14f);

    public void Initialize()
    {
        BoxCollider hitbox = gameObject.AddComponent<BoxCollider>();
        hitbox.center = new Vector3(0f, .04f, 0f);
        hitbox.size = new Vector3(MapBuilder.CellSize * .92f, .12f, MapBuilder.CellSize * .92f);

        GameObject rimObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rimObj.name = "BuildMarkerRim";
        rimObj.transform.SetParent(transform);
        rimObj.transform.localPosition = new Vector3(0f, .02f, 0f);
        rimObj.transform.localScale = new Vector3(.40f, .012f, .40f);
        Object.Destroy(rimObj.GetComponent<Collider>());
        rim = rimObj.GetComponent<Renderer>();
        TowerFactory.SetColor(rimObj, rimColor);

        GameObject markerObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        markerObj.name = "BuildMarkerCore";
        markerObj.transform.SetParent(transform);
        markerObj.transform.localPosition = new Vector3(0f, .035f, 0f);
        markerObj.transform.localScale = new Vector3(.29f, .014f, .29f);
        Object.Destroy(markerObj.GetComponent<Collider>());
        marker = markerObj.GetComponent<Renderer>();
        TowerFactory.SetColor(markerObj, idleColor);
    }

    void Update()
    {
        if (marker == null || rim == null || Occupied) return;
        float pulse = hovered ? 1f + Mathf.Sin(Time.unscaledTime * 7f) * .08f : 1f;
        marker.transform.localScale = new Vector3(.29f * pulse, .014f, .29f * pulse);
    }

    public bool TryBuild(TowerType type)
    {
        if (Occupied || GameManager.Instance == null) return false;
        int cost = TowerFactory.GetCost(type);
        if (!GameManager.Instance.SpendMoney(cost)) return false;

        GameObject towerObj = TowerFactory.CreateTower(transform.position + Vector3.up * .5f, type);
        Tower = towerObj.GetComponent<Tower>();
        Occupied = Tower != null;
        if (Tower != null)
        {
            Tower.OwnerPoint = this;
            GameManager.Instance.RecordTowerBuilt();
        }
        RefreshVisual();
        return Occupied;
    }

    public void ClearTower(Tower tower)
    {
        if (Tower != tower) return;
        Tower = null;
        Occupied = false;
        RefreshVisual();
    }

    public void SetHovered(bool value)
    {
        hovered = value;
        if (marker == null || rim == null) return;
        SetRendererColor(marker, Occupied ? occupiedColor : hovered ? hoverColor : idleColor);
        SetRendererColor(rim, Occupied ? occupiedColor : hovered ? new Color(.10f,.42f,.16f) : rimColor);
    }

    public void RefreshVisual()
    {
        if (marker == null || rim == null) return;
        hovered = false;
        SetRendererColor(marker, Occupied ? occupiedColor : idleColor);
        SetRendererColor(rim, Occupied ? occupiedColor : rimColor);
        marker.gameObject.SetActive(!Occupied);
        rim.gameObject.SetActive(!Occupied);
    }

    void SetRendererColor(Renderer renderer, Color color)
    {
        Material material = renderer.material;
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color")) material.SetColor("_Color", color);
    }
}
