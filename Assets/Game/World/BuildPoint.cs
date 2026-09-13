using UnityEngine;

public class BuildPoint : MonoBehaviour
{
    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");
    static readonly Color IdleColor = new Color(.78f,.58f,.20f);
    static readonly Color RimColor = new Color(.30f,.22f,.10f);
    static readonly Color ValidColor = new Color(.22f,.82f,.34f);
    static readonly Color InvalidColor = new Color(.90f,.16f,.10f);
    static readonly Color OccupiedColor = new Color(.16f,.16f,.14f);
    static readonly Color ValidRimColor = new Color(.08f,.48f,.16f);
    static readonly Color InvalidRimColor = new Color(.50f,.055f,.035f);

    public bool Occupied { get; private set; }
    public Tower Tower { get; private set; }

    readonly MaterialPropertyBlock colorBlock = new MaterialPropertyBlock();
    Renderer marker;
    Renderer rim;
    bool hovered;

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
        TowerFactory.SetColor(rimObj, RimColor);

        GameObject markerObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        markerObj.name = "BuildMarkerCore";
        markerObj.transform.SetParent(transform);
        markerObj.transform.localPosition = new Vector3(0f, .035f, 0f);
        markerObj.transform.localScale = new Vector3(.29f, .014f, .29f);
        Object.Destroy(markerObj.GetComponent<Collider>());
        marker = markerObj.GetComponent<Renderer>();
        TowerFactory.SetColor(markerObj, IdleColor);
    }

    void Update()
    {
        if (!hovered || marker == null || rim == null || Occupied) return;
        float pulse = 1f + Mathf.Sin(Time.unscaledTime * 7f) * .10f;
        marker.transform.localScale = new Vector3(.29f * pulse, .014f, .29f * pulse);
        rim.transform.localScale = new Vector3(.40f * 1.03f, .012f, .40f * 1.03f);
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
        if (!value) RefreshVisual();
    }

    public void SetPlacementState(bool value, bool valid)
    {
        hovered = value;
        if (marker == null || rim == null) return;
        if (Occupied)
        {
            SetRendererColor(marker, OccupiedColor);
            SetRendererColor(rim, OccupiedColor);
            return;
        }

        Color core = value ? (valid ? ValidColor : InvalidColor) : IdleColor;
        Color edge = value ? (valid ? ValidRimColor : InvalidRimColor) : RimColor;
        SetRendererColor(marker, core);
        SetRendererColor(rim, edge);
    }

    public void RefreshVisual()
    {
        if (marker == null || rim == null) return;
        hovered = false;
        marker.transform.localScale = new Vector3(.29f, .014f, .29f);
        rim.transform.localScale = new Vector3(.40f, .012f, .40f);
        SetRendererColor(marker, Occupied ? OccupiedColor : IdleColor);
        SetRendererColor(rim, Occupied ? OccupiedColor : RimColor);
        marker.gameObject.SetActive(!Occupied);
        rim.gameObject.SetActive(!Occupied);
    }

    void SetRendererColor(Renderer renderer, Color color)
    {
        colorBlock.Clear();
        colorBlock.SetColor(BaseColorId, color);
        colorBlock.SetColor(ColorId, color);
        renderer.SetPropertyBlock(colorBlock);
    }
}
