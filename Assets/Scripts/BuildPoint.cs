using UnityEngine;

public class BuildPoint : MonoBehaviour
{
    public bool Occupied { get; private set; }
    public Tower Tower { get; private set; }

    Renderer marker;
    Color idleColor = new Color(0.18f, 0.65f, 0.28f, 0.65f);
    Color hoverColor = new Color(0.35f, 1f, 0.5f, 0.95f);
    Color occupiedColor = new Color(0.22f, 0.22f, 0.22f, 0.55f);

    public void Initialize()
    {
        GameObject markerObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        markerObj.name = "BuildMarker";
        markerObj.transform.SetParent(transform);
        markerObj.transform.localPosition = new Vector3(0f, 0.05f, 0f);
        markerObj.transform.localScale = new Vector3(0.9f, 0.06f, 0.9f);
        marker = markerObj.GetComponent<Renderer>();
        TowerFactory.SetColor(markerObj, idleColor);
    }

    public bool TryBuild(int cost)
    {
        if (Occupied || GameManager.Instance == null) return false;
        if (!GameManager.Instance.SpendMoney(cost)) return false;

        GameObject towerObj = TowerFactory.CreateTower(transform.position + Vector3.up * 0.5f);
        Tower = towerObj.GetComponent<Tower>();
        Occupied = Tower != null;
        RefreshVisual();
        return Occupied;
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
