using UnityEngine;
using UnityEngine.EventSystems;

public sealed class HectorInputDriver : MonoBehaviour
{
    HectorController hector;
    Camera gameCamera;
    readonly Plane battlefieldPlane = new Plane(Vector3.up, Vector3.zero);

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

        if (!hector.Selected) return;

        if (!pointerOverUi && hector.CanMove && GameInput.SecondaryPressed())
            MoveSelectedHector(cam);

        if (!hector.CanAcceptCombatCommand) return;
        if (GameInput.Ability1Pressed()) hector.UseWarCry();
        if (GameInput.Ability2Pressed()) hector.UseShieldWall();
        if (GameInput.Ability3Pressed()) hector.UseSpearThrow();
        if (GameInput.UltimatePressed()) hector.UseUltimate();
    }

    void UpdateSelection(Camera cam)
    {
        Ray ray = cam.ScreenPointToRay(GameInput.PointerPosition);
        bool selected = Physics.Raycast(ray, out RaycastHit hit, 200f) &&
                        hit.collider.GetComponentInParent<HectorController>() == hector;
        hector.SetSelected(selected);
    }

    void MoveSelectedHector(Camera cam)
    {
        Ray ray = cam.ScreenPointToRay(GameInput.PointerPosition);
        if (!battlefieldPlane.Raycast(ray, out float enter)) return;
        hector.MoveTo(ray.GetPoint(enter));
    }
}
