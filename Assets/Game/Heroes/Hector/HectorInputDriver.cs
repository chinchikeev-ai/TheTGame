using UnityEngine;
using UnityEngine.EventSystems;

public sealed class HectorInputDriver : MonoBehaviour
{
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

        bool pointerOverUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if (!pointerOverUi && GameInput.PrimaryPressed())
            UpdateSelection(cam);

        if (GameInput.SecondaryPressed())
        {
            if (hector.Selected)
            {
                hector.SetSelected(false);
                RuntimeFileLogger.Event("HECTOR_INPUT", "Deselected by secondary pointer action.");
            }
            return;
        }

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

    void UpdateSelection(Camera cam)
    {
        ApplySelectionRay(cam.ScreenPointToRay(GameInput.PointerPosition));
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
