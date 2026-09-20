using UnityEngine;
using UnityEngine.EventSystems;

public class TowerPlacement : MonoBehaviour
{
    public static TowerPlacement Instance { get; private set; }

    const int PointerHitCapacity = 64;

    public Camera gameCamera;
    public TowerType SelectedBuildType { get; private set; } = TowerType.SpearThrower;
    public Tower SelectedTower { get; private set; }
    public bool BuildModeActive { get; private set; }

    readonly RaycastHit[] pointerHits = new RaycastHit[PointerHitCapacity];
    BuildPoint hoveredPoint;
    TowerRangeIndicator rangeIndicator;
    TowerPlacementPreview placementPreview;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        BuildModeActive = false;
        ClearHoveredPoint();
        placementPreview?.Hide();
        if (rangeIndicator != null) rangeIndicator.Hide();
    }

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
        BuildModeActive = true;
        ClearHoveredPoint();
        rangeIndicator?.Hide();
    }

    public void CancelBuildMode()
    {
        BuildModeActive = false;
        ClearHoveredPoint();
        placementPreview?.Hide();
        if (SelectedTower != null) rangeIndicator?.Show(SelectedTower);
        else rangeIndicator?.Hide();
    }

    public void ClearSelection()
    {
        BuildModeActive = false;
        SelectedTower = null;
        ClearHoveredPoint();
        placementPreview?.Hide();
        rangeIndicator?.Hide();
    }

    public void UpgradeSelected()
    {
        if (SelectedTower == null) return;
        Vector3 position = SelectedTower.transform.position;
        if (!SelectedTower.Upgrade()) return;

        RuntimeEffects.Instance?.PlayBuildSuccessSound(true);
        BuildFeedbackPresentation.Success(position, true);
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

        if (GameInput.SecondaryPressed())
        {
            ClearSelection();
            return;
        }

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
        FindPointerTarget(ray, out Tower hoveredTower, out BuildPoint newPoint);

        if (hoveredPoint != newPoint)
        {
            if (hoveredPoint != null) hoveredPoint.SetHovered(false);
            hoveredPoint = newPoint;
        }

        bool validPlacement = BuildModeActive && hoveredPoint != null && !hoveredPoint.Occupied && CanAffordSelected();
        if (hoveredPoint != null)
            hoveredPoint.SetPlacementState(BuildModeActive, validPlacement);

        if (hoveredTower != null)
        {
            placementPreview?.Hide();
            rangeIndicator.Show(hoveredTower);
        }
        else if (BuildModeActive && hoveredPoint != null && !hoveredPoint.Occupied)
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
            BuildModeActive = false;
            ClearHoveredPoint();
            placementPreview?.Hide();
            SelectedTower = hoveredTower;
            rangeIndicator.Show(SelectedTower);
            return;
        }

        if (!BuildModeActive)
        {
            SelectedTower = null;
            rangeIndicator.Hide();
            return;
        }

        if (hoveredPoint != null && !hoveredPoint.Occupied)
        {
            Vector3 feedbackPosition = hoveredPoint.transform.position + Vector3.up * .2f;
            if (hoveredPoint.TryBuild(SelectedBuildType))
            {
                SelectedTower = hoveredPoint.Tower;
                BuildModeActive = false;
                ClearHoveredPoint();
                placementPreview?.Hide();

                feedbackPosition = SelectedTower.transform.position + Vector3.up * .5f;
                RuntimeEffects.Instance?.PlayBuildSuccessSound(false);
                BuildFeedbackPresentation.Success(feedbackPosition, false);
                rangeIndicator.Show(SelectedTower);
            }
            else
            {
                RuntimeEffects.Instance?.PlayBuildDeniedSound();
                BuildFeedbackPresentation.Denied(feedbackPosition);
            }
        }
    }

    void FindPointerTarget(Ray ray, out Tower tower, out BuildPoint point)
    {
        tower = null;
        point = null;
        float closestDistance = float.PositiveInfinity;
        int hitCount = Physics.RaycastNonAlloc(ray, pointerHits, 250f);

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit hit = pointerHits[i];
            if (hit.collider == null || hit.distance >= closestDistance) continue;

            Tower candidateTower = hit.collider.GetComponentInParent<Tower>();
            BuildPoint candidatePoint = BuildModeActive ? hit.collider.GetComponentInParent<BuildPoint>() : null;
            if (candidateTower == null && candidatePoint == null) continue;

            closestDistance = hit.distance;
            tower = candidateTower;
            point = candidatePoint;
        }
    }

    bool CanAffordSelected()
    {
        return BuildModeActive && GameManager.Instance != null && GameManager.Instance.Money >= TowerFactory.GetCost(SelectedBuildType);
    }

    void ClearHoveredPoint()
    {
        if (hoveredPoint == null) return;
        hoveredPoint.SetHovered(false);
        hoveredPoint = null;
    }

    void OnDisable()
    {
        BuildModeActive = false;
        ClearHoveredPoint();
        placementPreview?.Hide();
        if (rangeIndicator != null) rangeIndicator.Hide();
    }
}
