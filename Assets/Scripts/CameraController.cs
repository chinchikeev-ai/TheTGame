using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class CameraController : MonoBehaviour
{
    public float panSpeed = 12f;
    public float zoomSpeed = 8f;
    public float minHeight = 10f;
    public float maxHeight = 24f;
    public Vector2 xBounds = new Vector2(-10f, 10f);
    public Vector2 zBounds = new Vector2(-8f, 8f);
    public float edgeSize = 18f;
    public bool edgePan = true;

    Vector3 targetPosition;

    void Start() => targetPosition = transform.position;

    void Update()
    {
        Vector2 move = ReadMove();
        if (edgePan) move += ReadEdgePan();

        Vector3 right = transform.right; right.y = 0f; right.Normalize();
        Vector3 forward = Vector3.Cross(right, Vector3.up).normalized;
        targetPosition += (right * move.x + forward * move.y) * panSpeed * Time.unscaledDeltaTime;

        float scroll = ReadScroll();
        targetPosition += Vector3.up * (-scroll * zoomSpeed);

        targetPosition.x = Mathf.Clamp(targetPosition.x, xBounds.x, xBounds.y);
        targetPosition.z = Mathf.Clamp(targetPosition.z, zBounds.x, zBounds.y);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minHeight, maxHeight);

        transform.position = Vector3.Lerp(transform.position, targetPosition, 10f * Time.unscaledDeltaTime);
    }

    Vector2 ReadMove()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current == null) return Vector2.zero;
        float x = (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1f : 0f) - (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? 1f : 0f);
        float y = (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1f : 0f) - (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? 1f : 0f);
        return new Vector2(x, y).normalized;
#else
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
#endif
    }

    float ReadScroll()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current == null ? 0f : Mouse.current.scroll.ReadValue().y / 120f;
#else
        return Input.mouseScrollDelta.y;
#endif
    }

    Vector2 ReadEdgePan()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current == null) return Vector2.zero;
        Vector2 p = Mouse.current.position.ReadValue();
#else
        Vector2 p = Input.mousePosition;
#endif
        Vector2 m = Vector2.zero;
        if (p.x <= edgeSize) m.x -= 1f;
        if (p.x >= Screen.width - edgeSize) m.x += 1f;
        if (p.y <= edgeSize) m.y -= 1f;
        if (p.y >= Screen.height - edgeSize) m.y += 1f;
        return m;
    }
}
