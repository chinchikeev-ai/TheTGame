using System.Linq;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class TowerPlacement : MonoBehaviour
{
    public Camera gameCamera;
    public int towerCost = 100;

    BuildPoint hoveredPoint;
    TowerRangeIndicator rangeIndicator;

    void Start()
    {
        rangeIndicator = FindFirstObjectByType<TowerRangeIndicator>();
        if (rangeIndicator == null)
            rangeIndicator = new GameObject("TowerRangeIndicatorController").AddComponent<TowerRangeIndicator>();
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameEnded) return;
        if (gameCamera == null) gameCamera = Camera.main;
        if (gameCamera == null) return;

        Vector2 pointer = ReadPointerPosition();
        Ray ray = gameCamera.ScreenPointToRay(pointer);
        RaycastHit[] hits = Physics.RaycastAll(ray, 250f).OrderBy(h => h.distance).ToArray();

        BuildPoint newPoint = null;
        Tower hoveredTower = null;

        foreach (RaycastHit hit in hits)
        {
            if (hoveredTower == null)
                hoveredTower = hit.collider.GetComponentInParent<Tower>();
            if (newPoint == null)
                newPoint = hit.collider.GetComponentInParent<BuildPoint>();
            if (hoveredTower != null || newPoint != null) break;
        }

        if (hoveredPoint != newPoint)
        {
            if (hoveredPoint != null) hoveredPoint.SetHovered(false);
            hoveredPoint = newPoint;
            if (hoveredPoint != null) hoveredPoint.SetHovered(true);
        }

        if (hoveredTower != null) rangeIndicator.Show(hoveredTower);
        else rangeIndicator.Hide();

        if (ReadPrimaryClick() && hoveredPoint != null && !hoveredPoint.Occupied)
            hoveredPoint.TryBuild(towerCost);
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
