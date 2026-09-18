using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class HectorInputDriver : MonoBehaviour
{
    const float HectorScreenSelectRadius = 72f;

    readonly List<RaycastResult> uiHits = new List<RaycastResult>(16);
    HectorController hector;
    Camera gameCamera;

    public void Initialize(HectorController controller, Camera camera = null)
    {
        hector = controller;
        gameCamera = camera;
    }

    void Awake()
    {
        if (hector == null) hector = GetComponent<HectorController>();
    }

    void Update()
    {
        if (hector == null || hector.IsDowned || GameManager.Instance == null || GameManager.Instance.GameEnded) return;

        Camera cam = gameCamera != null ? gameCamera : Camera.main;
        if (cam == null) return;

        bool pointerOverUi = IsPointerOverInteractiveUi(GameInput.PointerPosition);

        if (!pointerOverUi && GameInput.PrimaryPressed())
            UpdateSelection(cam);

        if (!pointerOverUi && GameInput.SecondaryPressed())
            TryMoveAtScreenPoint(GameInput.PointerPosition);

        if (!hector.Selected || !hector.CanAcceptCombatCommand) return;
        if (GameInput.Ability1Pressed()) hector.UseWarCry();
        if (GameInput.Ability2Pressed()) hector.UseShieldWall();
        if (GameInput.Ability3Pressed()) hector.UseSpearThrow();
        if (GameInput.UltimatePressed()) hector.UseUltimate();
    }

    public bool TrySelectAtScreenPoint(Vector2 screenPoint)
    {
        Camera cam = gameCamera != null ? gameCamera : Camera.main;
        if (cam == null || hector == null || hector.IsDowned) return false;
        return ApplySelectionRay(cam.ScreenPointToRay(screenPoint));
    }

    public bool TryMoveAtScreenPoint(Vector2 screenPoint)
    {
        Camera cam = gameCamera != null ? gameCamera : Camera.main;
        if (cam == null || hector == null || hector.IsDowned || !hector.Selected || !hector.CanMove) return false;

        Ray ray = cam.ScreenPointToRay(screenPoint);
        Plane movementPlane = new Plane(Vector3.up, hector.transform.position);
        if (!movementPlane.Raycast(ray, out float enter) || enter < 0f) return false;

        Vector3 requested = ray.GetPoint(enter);
        Vector3 destination = hector.ConstrainMoveDestination(requested);
        hector.MoveTo(destination);
        RuntimeFileLogger.Event("HECTOR_INPUT", $"Move command to {destination}.");
        return true;
    }

    void UpdateSelection(Camera cam)
    {
        Vector2 pointer = GameInput.PointerPosition;
        if (ApplySelectionRay(cam.ScreenPointToRay(pointer))) return;

        Vector3 hectorScreen = cam.WorldToScreenPoint(hector.transform.position + Vector3.up * .9f);
        if (hectorScreen.z <= 0f) return;

        float scale = Mathf.Max(.75f, Screen.height / 1080f);
        if (Vector2.Distance(pointer, new Vector2(hectorScreen.x, hectorScreen.y)) <= HectorScreenSelectRadius * scale)
        {
            hector.SetSelected(true);
            RuntimeFileLogger.Event("HECTOR_INPUT", "Selected by screen proximity fallback.");
        }
    }

    bool IsPointerOverInteractiveUi(Vector2 screenPoint)
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null) return false;

        PointerEventData pointer = new PointerEventData(eventSystem) { position = screenPoint };
        uiHits.Clear();
        eventSystem.RaycastAll(pointer, uiHits);

        for (int i = 0; i < uiHits.Count; i++)
        {
            GameObject target = uiHits[i].gameObject;
            if (target == null) continue;
            Selectable selectable = target.GetComponentInParent<Selectable>();
            if (selectable != null && selectable.IsActive() && selectable.IsInteractable())
                return true;
        }

        return false;
    }

    bool ApplySelectionRay(Ray ray)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray, 200f, ~0, QueryTriggerInteraction.Collide);
        bool selected = false;
        float nearestHectorDistance = float.PositiveInfinity;

        for (int i = 0; i < hits.Length; i++)
        {
            HectorController hitHector = hits[i].collider != null
                ? hits[i].collider.GetComponentInParent<HectorController>()
                : null;
            if (hitHector != hector || hits[i].distance >= nearestHectorDistance) continue;
            nearestHectorDistance = hits[i].distance;
            selected = true;
        }

        bool changed = hector.Selected != selected;
        hector.SetSelected(selected);
        if (changed)
            RuntimeFileLogger.Event("HECTOR_INPUT", selected ? "Selected by pointer." : "Deselected by pointer.");
        return selected;
    }
}
