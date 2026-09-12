using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class TowerPlacement : MonoBehaviour
{
    public Camera gameCamera;
    public TowerType SelectedBuildType { get; private set; } = TowerType.MachineGun;
    public Tower SelectedTower { get; private set; }

    BuildPoint hoveredPoint;
    TowerRangeIndicator rangeIndicator;

    void Start()
    {
        rangeIndicator = FindFirstObjectByType<TowerRangeIndicator>();
        if (rangeIndicator == null)
            rangeIndicator = new GameObject("TowerRangeIndicatorController").AddComponent<TowerRangeIndicator>();
    }

    public void SelectBuildType(TowerType type)
    {
        SelectedBuildType = type;
        SelectedTower = null;
    }

    public void UpgradeSelected()
    {
        if (SelectedTower != null) SelectedTower.Upgrade();
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

        Vector2 pointer = ReadPointerPosition();
        bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if (overUI)
        {
            if (hoveredPoint != null) { hoveredPoint.SetHovered(false); hoveredPoint = null; }
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
            if (hoveredPoint != null) hoveredPoint.SetHovered(true);
        }

        if (hoveredTower != null) rangeIndicator.Show(hoveredTower);
        else if (SelectedTower != null) rangeIndicator.Show(SelectedTower);
        else rangeIndicator.Hide();

        if (!ReadPrimaryClick()) return;

        if (hoveredTower != null)
        {
            SelectedTower = hoveredTower;
            rangeIndicator.Show(SelectedTower);
            return;
        }

        if (hoveredPoint != null && !hoveredPoint.Occupied)
        {
            if (hoveredPoint.TryBuild(SelectedBuildType))
                SelectedTower = hoveredPoint.Tower;
        }
        else
        {
            SelectedTower = null;
        }
    }

    void OnDisable()
    {
        if (hoveredPoint != null) hoveredPoint.SetHovered(false);
        if (rangeIndicator != null) rangeIndicator.Hide();
    }

    Vector2 ReadPointerPosition()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
        return Input.mousePosition;
#endif
    }

    bool ReadPrimaryClick()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }
}
