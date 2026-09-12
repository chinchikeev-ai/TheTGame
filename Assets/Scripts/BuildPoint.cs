using UnityEngine;

public class BuildPoint : MonoBehaviour
{
    public bool Occupied { get; private set; }
    public Tower Tower { get; private set; }

    Renderer marker;
    readonly Color idleColor = new Color(0.16f, 0.30f, 0.17f);
    readonly Color hoverColor = new Color(0.30f, 0.72f, 0.32f);
    readonly Color occupiedColor = new Color(0.18f, 0.18f, 0.18f);

    public void Initialize()
    {
        GameObject markerObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        markerObj.name = "BuildCell";
        markerObj.transform.SetParent(transform);
        markerObj.transform.localPosition = new Vector3(0f, 0.04f, 0f);
        markerObj.transform.localScale = new Vector3(MapBuilder.CellSize * 0.90f, 0.08f, MapBuilder.CellSize * 0.90f);
        marker = markerObj.GetComponent<Renderer>();
        TowerFactory.SetColor(markerObj, idleColor);
    }

    public bool TryBuild(TowerType type)
    {
        if (Occupied || GameManager.Instance == null) return false;
        int cost = TowerFactory.GetCost(type);
        if (!GameManager.Instance.SpendMoney(cost)) return false;

        GameObject towerObj = TowerFactory.CreateTower(transform.position + Vector3.up * 0.5f, type);
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

    public void SetHovered(bool hovered)
    {
        if (marker == null) return;
        SetMarkerColor(Occupied ? occupiedColor : hovered ? hoverColor : idleColor);
    }

    public void RefreshVisual()
    {
        if (marker == null) return;
        SetMarkerColor(Occupied ? occupiedColor : idleColor);
    }

    void SetMarkerColor(Color color)
    {
        Material material = marker.material;
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color")) material.SetColor("_Color", color);
    }
}
