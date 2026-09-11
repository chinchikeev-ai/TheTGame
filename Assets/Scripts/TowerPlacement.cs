using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    public Camera gameCamera;
    public LayerMask groundMask = ~0;
    public int towerCost = 100;
    public float minTowerDistance = 2.5f;

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameEnded) return;
        if (!Input.GetMouseButtonDown(0)) return;
        if (gameCamera == null) gameCamera = Camera.main;

        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 200f, groundMask)) return;
        if (hit.collider.gameObject.name != "Ground") return;

        Vector3 p = hit.point;
        p.y = 0.5f;

        foreach (Tower tower in FindObjectsByType<Tower>(FindObjectsSortMode.None))
        {
            if (Vector3.Distance(tower.transform.position, p) < minTowerDistance)
                return;
        }

        if (!GameManager.Instance.SpendMoney(towerCost)) return;
        CreateTower(p);
    }

    void CreateTower(Vector3 position)
    {
        GameObject root = new GameObject("Tower");
        root.transform.position = position;

        GameObject baseObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        baseObj.transform.SetParent(root.transform);
        baseObj.transform.localPosition = Vector3.zero;
        baseObj.transform.localScale = new Vector3(0.75f, 0.35f, 0.75f);

        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
        head.name = "Head";
        head.transform.SetParent(root.transform);
        head.transform.localPosition = new Vector3(0f, 0.75f, 0f);
        head.transform.localScale = new Vector3(0.6f, 0.35f, 1.25f);

        Tower tower = root.AddComponent<Tower>();
        tower.head = head.transform;
    }
}
