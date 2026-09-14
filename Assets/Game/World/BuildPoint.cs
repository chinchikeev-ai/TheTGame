using UnityEngine;

public class BuildPoint : MonoBehaviour
{
    // Idle build sites should remain discoverable without becoming the dominant
    // repeating pattern in the tactical view. Interaction states expand strongly.
    const float IdleCoreScale = .075f;
    const float IdleRimScale = .14f;
    const float HoverCoreScale = .27f;
    const float HoverRimScale = .37f;

    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");
    static readonly Color FoundationEarth = new Color(.38f,.32f,.24f);
    static readonly Color FoundationStone = new Color(.36f,.34f,.29f);
    static readonly Color FoundationStoneLight = new Color(.41f,.38f,.32f);
    static readonly Color IdleColor = new Color(.41f,.35f,.27f);
    static readonly Color RimColor = new Color(.34f,.31f,.26f);
    static readonly Color ValidColor = new Color(.32f,.86f,.38f);
    static readonly Color InvalidColor = new Color(.94f,.22f,.12f);
    static readonly Color OccupiedColor = new Color(.20f,.19f,.16f);
    static readonly Color ValidRimColor = new Color(.13f,.54f,.20f);
    static readonly Color InvalidRimColor = new Color(.56f,.07f,.04f);

    public bool Occupied { get; private set; }
    public Tower Tower { get; private set; }

    MaterialPropertyBlock colorBlock;
    Renderer marker;
    Renderer rim;
    bool hovered;

    public void Initialize()
    {
        BoxCollider hitbox = gameObject.AddComponent<BoxCollider>();
        hitbox.center = new Vector3(0f, .04f, 0f);
        hitbox.size = new Vector3(MapBuilder.CellSize * .92f, .12f, MapBuilder.CellSize * .92f);

        BuildDiegeticFoundation();

        GameObject rimObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rimObj.name = "BuildStateOuterAccent";
        rimObj.transform.SetParent(transform,false);
        rimObj.transform.localPosition = new Vector3(0f, .036f, 0f);
        rimObj.transform.localScale = new Vector3(IdleRimScale, .006f, IdleRimScale);
        Object.Destroy(rimObj.GetComponent<Collider>());
        rim = rimObj.GetComponent<Renderer>();
        TowerFactory.SetColor(rimObj, RimColor);

        GameObject markerObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        markerObj.name = "BuildStateCore";
        markerObj.transform.SetParent(transform,false);
        markerObj.transform.localPosition = new Vector3(0f, .044f, 0f);
        markerObj.transform.localScale = new Vector3(IdleCoreScale, .007f, IdleCoreScale);
        Object.Destroy(markerObj.GetComponent<Collider>());
        marker = markerObj.GetComponent<Renderer>();
        TowerFactory.SetColor(markerObj, IdleColor);
    }

    void BuildDiegeticFoundation()
    {
        GameObject root = new GameObject("Diegetic Build Foundation");
        root.transform.SetParent(transform,false);

        GameObject earth = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        earth.name = "Packed Foundation Earth";
        earth.transform.SetParent(root.transform,false);
        earth.transform.localPosition = new Vector3(0f,.006f,0f);
        earth.transform.localScale = new Vector3(.32f,.014f,.30f);
        Object.Destroy(earth.GetComponent<Collider>());
        TowerFactory.SetColor(earth,FoundationEarth);

        // Two small irregular stones are enough to imply a prepared foundation.
        // The old seven-stone ring read as a bright flower repeated across the map.
        float phase = Mathf.Repeat(Mathf.Abs(transform.position.x * 17f + transform.position.z * 11f),23f);
        for(int i=0;i<2;i++)
        {
            float angle = (phase + i * 163f) * Mathf.Deg2Rad;
            float radius = .27f + i*.035f;
            GameObject slab = GameObject.CreatePrimitive(i==0 ? PrimitiveType.Sphere : PrimitiveType.Cube);
            slab.name = "Weathered Foundation Stone";
            slab.transform.SetParent(root.transform,false);
            slab.transform.localPosition = new Vector3(Mathf.Cos(angle)*radius,.016f,Mathf.Sin(angle)*radius);
            slab.transform.localRotation = Quaternion.Euler(i==0?2f:-2f,phase+i*71f,i==0?-2f:3f);
            slab.transform.localScale = i==0
                ? new Vector3(.17f,.036f,.13f)
                : new Vector3(.20f,.040f,.12f);
            Object.Destroy(slab.GetComponent<Collider>());
            TowerFactory.SetColor(slab,i==0 ? FoundationStone : FoundationStoneLight);
        }
    }

    void Update()
    {
        if (!hovered || marker == null || rim == null || Occupied) return;
        float pulse = 1f + Mathf.Sin(Time.unscaledTime * 7f) * .09f;
        marker.transform.localScale = new Vector3(HoverCoreScale * pulse, .009f, HoverCoreScale * pulse);
        rim.transform.localScale = new Vector3(HoverRimScale * (1f + Mathf.Sin(Time.unscaledTime * 5.5f) * .035f), .008f, HoverRimScale * (1f + Mathf.Sin(Time.unscaledTime * 5.5f) * .035f));
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

        float coreScale = value ? HoverCoreScale : IdleCoreScale;
        float rimScale = value ? HoverRimScale : IdleRimScale;
        marker.transform.localScale = new Vector3(coreScale, value ? .009f : .007f, coreScale);
        rim.transform.localScale = new Vector3(rimScale, value ? .008f : .006f, rimScale);

        Color core = value ? (valid ? ValidColor : InvalidColor) : IdleColor;
        Color edge = value ? (valid ? ValidRimColor : InvalidRimColor) : RimColor;
        SetRendererColor(marker, core);
        SetRendererColor(rim, edge);
    }

    public void RefreshVisual()
    {
        if (marker == null || rim == null) return;
        hovered = false;
        marker.transform.localScale = new Vector3(IdleCoreScale, .007f, IdleCoreScale);
        rim.transform.localScale = new Vector3(IdleRimScale, .006f, IdleRimScale);
        SetRendererColor(marker, Occupied ? OccupiedColor : IdleColor);
        SetRendererColor(rim, Occupied ? OccupiedColor : RimColor);
        marker.gameObject.SetActive(!Occupied);
        rim.gameObject.SetActive(!Occupied);
    }

    void SetRendererColor(Renderer renderer, Color color)
    {
        if (colorBlock == null) colorBlock = new MaterialPropertyBlock();
        colorBlock.Clear();
        colorBlock.SetColor(BaseColorId, color);
        colorBlock.SetColor(ColorId, color);
        renderer.SetPropertyBlock(colorBlock);
    }
}
