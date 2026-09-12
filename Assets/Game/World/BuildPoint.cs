using UnityEngine;

public class BuildPoint : MonoBehaviour
{
    public bool Occupied { get; private set; }
    public Tower Tower { get; private set; }

    Renderer marker;
    readonly Color idleColor = new Color(.48f,.40f,.24f);
    readonly Color hoverColor = new Color(.30f,.72f,.32f);
    readonly Color occupiedColor = new Color(.20f,.20f,.18f);

    public void Initialize()
    {
        BoxCollider hitbox = gameObject.AddComponent<BoxCollider>();
        hitbox.center = new Vector3(0f, .04f, 0f);
        hitbox.size = new Vector3(MapBuilder.CellSize * .92f, .12f, MapBuilder.CellSize * .92f);

        GameObject markerObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        markerObj.name = "BuildMarker";
        markerObj.transform.SetParent(transform);
        markerObj.transform.localPosition = new Vector3(0f, .015f, 0f);
        markerObj.transform.localScale = new Vector3(.20f, .015f, .20f);
        Object.Destroy(markerObj.GetComponent<Collider>());
        marker = markerObj.GetComponent<Renderer>();
        TowerFactory.SetColor(markerObj, idleColor);
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

    public void SetHovered(bool hovered)
    {
        if (marker == null) return;
        SetMarkerColor(Occupied ? occupiedColor : hovered ? hoverColor : idleColor);
        marker.transform.localScale = hovered && !Occupied ? new Vector3(.34f,.02f,.34f) : new Vector3(.20f,.015f,.20f);
    }

    public void RefreshVisual()
    {
        if (marker == null) return;
        SetMarkerColor(Occupied ? occupiedColor : idleColor);
        marker.transform.localScale = Occupied ? new Vector3(.14f,.015f,.14f) : new Vector3(.20f,.015f,.20f);
    }

    void SetMarkerColor(Color color)
    {
        Material material = marker.material;
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color")) material.SetColor("_Color", color);
    }
}
