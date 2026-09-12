using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 12f;
    public float zoomSpeed = 2.2f;
    public float minOrthoSize = 7f;
    public float maxOrthoSize = 13f;
    public Vector2 xBounds = new Vector2(-4f, 4f);
    public Vector2 zBounds = new Vector2(-2f, 2f);
    public float edgeSize = 18f;
    public bool edgePan = true;

    Camera cam;
    Vector3 targetPosition;
    float targetOrthoSize;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetPosition = transform.position;
        targetOrthoSize = cam != null ? cam.orthographicSize : 10f;
    }

    void Update()
    {
        if (cam == null) cam = GetComponent<Camera>();

        Vector2 move = GameInput.CameraMove;
        if (edgePan) move += ReadEdgePan();
        targetPosition += new Vector3(move.x, 0f, move.y) * panSpeed * Time.unscaledDeltaTime;

        targetOrthoSize = Mathf.Clamp(targetOrthoSize - GameInput.ScrollDelta * zoomSpeed, minOrthoSize, maxOrthoSize);
        if (cam != null && cam.orthographic)
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetOrthoSize, 10f * Time.unscaledDeltaTime);

        targetPosition.x = Mathf.Clamp(targetPosition.x, xBounds.x, xBounds.y);
        targetPosition.z = Mathf.Clamp(targetPosition.z, zBounds.x, zBounds.y);
        transform.position = Vector3.Lerp(transform.position, targetPosition, 10f * Time.unscaledDeltaTime);
    }

    Vector2 ReadEdgePan()
    {
        Vector2 p = GameInput.PointerPosition;
        Vector2 m = Vector2.zero;
        if (p.x <= edgeSize) m.x -= 1f;
        if (p.x >= Screen.width - edgeSize) m.x += 1f;
        if (p.y <= edgeSize) m.y -= 1f;
        if (p.y >= Screen.height - edgeSize) m.y += 1f;
        return m;
    }
}
