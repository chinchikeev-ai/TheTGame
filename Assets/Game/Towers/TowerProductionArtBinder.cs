using UnityEngine;

public sealed class TowerProductionArtBinder : MonoBehaviour
{
    const string Root = "TroyProduction/Characters/Trojan/";
    Tower tower;
    int appliedLevel;

    void Start()
    {
        tower = GetComponent<Tower>();
        if (tower == null) return;
        ReplaceCrew();
        ApplyLevel();
    }

    void LateUpdate()
    {
        if (tower != null && tower.Level != appliedLevel) ApplyLevel();
    }

    void ReplaceCrew()
    {
        Transform art = transform.Find("ArtEnhancement");
        if (art == null) return;
        if (tower.Type == TowerType.Cannon && Has("Trojan_BallistaCrew"))
        {
            HideNamed(art,"TowerCrew_Trojan_Infantry");
            string engineer = Has("Trojan_BallistaCrew_Engineer") ? "Trojan_BallistaCrew_Engineer" : "Trojan_BallistaCrew";
            string loader = Has("Trojan_BallistaCrew_Loader") ? "Trojan_BallistaCrew_Loader" : "Trojan_BallistaCrew";
            AddCrew(art,engineer,"A",new Vector3(-.58f,.35f,-.40f),.58f,18f);
            AddCrew(art,loader,"B",new Vector3(.58f,.35f,-.34f),.58f,-16f);
        }
        if (tower.Type == TowerType.Slow && Has("Trojan_PriestApollo"))
        {
            HideNamed(art,"PriestOfApollo");
            AddCrew(art,"Trojan_PriestApollo","Left",new Vector3(-.24f,.35f,.08f),.56f,-10f);
            AddCrew(art,"Trojan_PriestApollo","Right",new Vector3(.24f,.35f,-.02f),.56f,10f);
        }
        if (tower.Type == TowerType.FireTower && Has("Trojan_FireKeeper"))
        {
            HideNamed(art,"FireKeeper");
            AddCrew(art,"Trojan_FireKeeper","A",new Vector3(.52f,.26f,-.20f),.58f,0f);
        }

        GetComponent<TowerCrewAnimationBridge>()?.RefreshCrew();
    }

    void ApplyLevel()
    {
        if (tower.Level >= 2) Layer(2);
        if (tower.Level >= 3) Layer(3);
        appliedLevel = tower.Level;
    }

    void Layer(int level)
    {
        string n = "UpgradeVisual_L" + level;
        if (transform.Find(n) != null) return;
        GameObject layer = new GameObject(n);
        layer.transform.SetParent(transform,false);
        Color c = level == 2 ? new Color(.74f,.50f,.18f) : new Color(.90f,.69f,.24f);
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = level == 2 ? "Reinforced Base Ring" : "Elite Base Ring";
        ring.transform.SetParent(layer.transform,false);
        ring.transform.localPosition = new Vector3(0f,level == 2 ? .18f : .27f,0f);
        ring.transform.localScale = new Vector3(level == 2 ? .84f : .91f,.06f,level == 2 ? .84f : .91f);
        Collider col = ring.GetComponent<Collider>();
        if (col != null) Destroy(col);
        TowerFactory.SetColor(ring,c);
    }

    static bool Has(string prefabName) => Resources.Load<GameObject>(Root + prefabName) != null;

    static void HideNamed(Transform root,string objectName)
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            if (child != root && child.name == objectName) child.gameObject.SetActive(false);
    }

    static void AddCrew(Transform parent,string prefabName,string slot,Vector3 p,float scale,float yaw)
    {
        GameObject prefab = Resources.Load<GameObject>(Root + prefabName);
        string instanceName = "TowerCrew_" + prefabName + "_" + slot;
        if (prefab == null || parent.Find(instanceName) != null) return;
        GameObject crew = Instantiate(prefab,parent);
        crew.name = instanceName;
        crew.transform.localPosition = p;
        crew.transform.localRotation = Quaternion.Euler(0f,yaw,0f);
        crew.transform.localScale = Vector3.one * scale;
        foreach (Collider col in crew.GetComponentsInChildren<Collider>(true)) { col.enabled = false; Destroy(col); }
        foreach (Rigidbody body in crew.GetComponentsInChildren<Rigidbody>(true)) { body.isKinematic = true; body.detectCollisions = false; }
    }
}
