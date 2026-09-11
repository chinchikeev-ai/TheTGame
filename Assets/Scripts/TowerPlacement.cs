using System.Linq;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    public Camera gameCamera;
    public int towerCost = 100;
    public float minTowerDistance = 2.6f;
    public float roadClearance = 1.35f;

    GameObject ghost;
    bool currentValid;
    Vector3 currentPosition;

    void Start()
    {
        ghost = TowerFactory.CreateGhost();
        ghost.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameEnded)
        {
            if (ghost != null) ghost.SetActive(false);
            return;
        }

        if (gameCamera == null) gameCamera = Camera.main;
        UpdatePreview();

        if (Input.GetMouseButtonDown(0) && currentValid)
        {
            if (GameManager.Instance.SpendMoney(towerCost))
                TowerFactory.CreateTower(currentPosition);
        }
    }

    void UpdatePreview()
    {
        if (gameCamera == null || ghost == null) return;

        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 250f).OrderBy(h => h.distance).ToArray();
        RaycastHit? groundHit = null;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.name == "Ground")
            {
                groundHit = hit;
                break;
            }
        }

        if (!groundHit.HasValue)
        {
            ghost.SetActive(false);
            currentValid = false;
            return;
        }

        currentPosition = groundHit.Value.point;
        currentPosition.y = 0.5f;
        ghost.SetActive(true);
        ghost.transform.position = currentPosition;

        currentValid = IsValidPosition(currentPosition) && GameManager.Instance.Money >= towerCost;
        TowerFactory.SetGhostValidity(ghost, currentValid);
    }

    bool IsValidPosition(Vector3 p)
    {
        foreach (Tower tower in FindObjectsByType<Tower>(FindObjectsSortMode.None))
        {
            if (Vector3.Distance(tower.transform.position, p) < minTowerDistance)
                return false;
        }

        foreach (GameObject road in GameObject.FindGameObjectsWithTag("Respawn"))
        {
            Collider col = road.GetComponent<Collider>();
            if (col == null) continue;
            Vector3 closest = col.ClosestPoint(p);
            closest.y = p.y;
            if (Vector3.Distance(closest, p) < roadClearance)
                return false;
        }

        GameObject baseObj = GameObject.Find("Base");
        if (baseObj != null && Vector3.Distance(baseObj.transform.position, p) < 2.8f)
            return false;

        return true;
    }
}
