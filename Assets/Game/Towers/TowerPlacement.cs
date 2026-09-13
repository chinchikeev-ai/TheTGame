using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerPlacement : MonoBehaviour
{
    public Camera gameCamera;
    public TowerType SelectedBuildType { get; private set; } = TowerType.SpearThrower;
    public Tower SelectedTower { get; private set; }

    BuildPoint hoveredPoint;
    TowerRangeIndicator rangeIndicator;
    TowerPlacementPreview placementPreview;

    void Start()
    {
        rangeIndicator = FindFirstObjectByType<TowerRangeIndicator>();
        if (rangeIndicator == null)
            rangeIndicator = new GameObject("TowerRangeIndicatorController").AddComponent<TowerRangeIndicator>();

        placementPreview = FindFirstObjectByType<TowerPlacementPreview>();
        if (placementPreview == null)
            placementPreview = new GameObject("TowerPlacementPreview").AddComponent<TowerPlacementPreview>();
    }

    public void SelectBuildType(TowerType type)
    {
        SelectedBuildType = type;
        SelectedTower = null;
    }

    public void UpgradeSelected()
    {
        if (SelectedTower == null) return;
        Vector3 position = SelectedTower.transform.position;
        if (SelectedTower.Upgrade())
            RuntimeEffects.Instance?.PlayBuildSuccess(position, true);
    }

    public void SellSelected()
    {
        if (SelectedTower == null) return;
        Tower tower = SelectedTower;
        SelectedTower = null;
        rangeIndicator.Hide();
        tower.Sell();
    }

    public void CycleSelectedPriority()
    {
        if (SelectedTower != null && SelectedTower.Type != TowerType.TrojanGuard)
            SelectedTower.CyclePriority();
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameEnded) return;
        if (gameCamera == null) gameCamera = Camera.main;
        if (gameCamera == null) return;

        Vector2 pointer = GameInput.PointerPosition;
        bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if (overUI)
        {
            ClearHoveredPoint();
            placementPreview?.Hide();
            if (SelectedTower != null) rangeIndicator.Show(SelectedTower); else rangeIndicator.Hide();
            return;
        }

        Ray ray = gameCamera.ScreenPointToRay(pointer);
        RaycastHit[] hits = Physics.RaycastAll(ray, 250f).OrderBy(h => h.distance).ToArray();

        BuildPoint newPoint = null;
        Tower hoveredTower = null;
        foreach (RaycastHit hit in hits)
        {
            if (hoveredTower == null) hoveredTower = hit.collider.GetComponentInParent<Tower>();
            if (newPoint == null) newPoint = hit.collider.GetComponentInParent<BuildPoint>();
            if (hoveredTower != null || newPoint != null) break;
        }

        if (hoveredPoint != newPoint)
        {
            if (hoveredPoint != null) hoveredPoint.SetHovered(false);
            hoveredPoint = newPoint;
        }

        bool validPlacement = hoveredPoint != null && !hoveredPoint.Occupied && CanAffordSelected();
        if (hoveredPoint != null)
            hoveredPoint.SetPlacementState(true, validPlacement);

        if (hoveredTower != null)
        {
            placementPreview?.Hide();
            rangeIndicator.Show(hoveredTower);
        }
        else if (hoveredPoint != null && !hoveredPoint.Occupied)
        {
            placementPreview?.Show(hoveredPoint, SelectedBuildType, validPlacement);
            float previewRange = BalanceCatalog.GetTower(SelectedBuildType).range;
            rangeIndicator.ShowPreview(hoveredPoint.transform.position, previewRange, validPlacement);
        }
        else
        {
            placementPreview?.Hide();
            if (SelectedTower != null) rangeIndicator.Show(SelectedTower); else rangeIndicator.Hide();
        }

        if (!GameInput.PrimaryPressed()) return;

        if (hoveredTower != null)
        {
            SelectedTower = hoveredTower;
            rangeIndicator.Show(SelectedTower);
            return;
        }

        if (hoveredPoint != null && !hoveredPoint.Occupied)
        {
            if (hoveredPoint.TryBuild(SelectedBuildType))
            {
                SelectedTower = hoveredPoint.Tower;
                placementPreview?.Hide();
                RuntimeEffects.Instance?.PlayBuildSuccess(hoveredPoint.transform.position + Vector3.up * .5f, false);
                rangeIndicator.Show(SelectedTower);
            }
            else
            {
                RuntimeEffects.Instance?.PlayBuildDenied(hoveredPoint.transform.position + Vector3.up * .2f);
            }
        }
        else
        {
            SelectedTower = null;
        }
    }

    bool CanAffordSelected()
    {
        return GameManager.Instance != null && GameManager.Instance.Money >= TowerFactory.GetCost(SelectedBuildType);
    }

    void ClearHoveredPoint()
    {
        if (hoveredPoint == null) return;
        hoveredPoint.SetHovered(false);
        hoveredPoint = null;
    }

    void OnDisable()
    {
        ClearHoveredPoint();
        placementPreview?.Hide();
        if (rangeIndicator != null) rangeIndicator.Hide();
    }
}
