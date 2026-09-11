using UnityEngine;

public class BuildPoint : MonoBehaviour
{
    public bool Occupied { get; private set; }
    public Tower Tower { get; private set; }

    Renderer marker;
    readonly Color idleColor = new Color(0.16f, 0.55f, 0.26f, 1f);
    readonly Color hoverColor = new Color(0.40f, 0.95f, 0.48f, 1f);
    readonly Color occupiedColor = new Color(0.18f, 0.18f, 0.18f, 1f);

    public void Initialize()
    {
        GameObject markerObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        markerObj.name = "BuildCell";
        markerObj.transform.SetParent(transform);
        markerObj.transform.localPosition = new Vector3(0f, 0.02f, 0f);
        markerObj.transform.localScale = new Vector3(MapBuilder.CellSize * 0.82f, 0.18f, MapBuilder.CellSize * 0.82f);
        marker = markerObj.GetComponent<Renderer>();
        TowerFactory.SetColor(markerObj, idleColor);
    }

    public bool TryBuild(TowerType type)
    {
        if (Occupied || GameManager.Instance == null) return false;
        int cost = TowerFactory.GetCost(type);
        if (!GameManager.Instance.SpendMoney(cost)) return false;

        GameObject towerObj = TowerFactory.CreateTower(transform.position + Vector3.up * 0.28f, type);
        Tower = towerObj.GetComponent<Tower>();
        Occupied = Tower != null;
        if (Tower != null) Tower.OwnerPoint = this;
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
        marker.material.color = Occupied ? occupiedColor : (hovered ? hoverColor : idleColor);
    }

    public void RefreshVisual()
    {
        if (marker == null) return;
        marker.material.color = Occupied ? occupiedColor : idleColor;
    }
}
