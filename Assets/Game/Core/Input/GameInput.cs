using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public static class GameInput
{
    public static Vector2 PointerPosition
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
            return Input.mousePosition;
#endif
        }
    }

    public static Vector2 CameraMove
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current == null) return Vector2.zero;
            float x = (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1f : 0f)
                    - (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? 1f : 0f);
            float y = (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1f : 0f)
                    - (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? 1f : 0f);
            return new Vector2(x, y).normalized;
#else
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
#endif
        }
    }

    public static float ScrollDelta
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current == null ? 0f : Mouse.current.scroll.ReadValue().y / 120f;
#else
            return Input.mouseScrollDelta.y;
#endif
        }
    }

    public static bool PrimaryPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    public static bool SecondaryPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(1);
#endif
    }

    public static bool PausePressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }

    public static bool Ability1Pressed() => KeyPressed(KeyCode.Q);
    public static bool Ability2Pressed() => KeyPressed(KeyCode.E);
    public static bool Ability3Pressed() => KeyPressed(KeyCode.R);
    public static bool UltimatePressed() => KeyPressed(KeyCode.F);

    static bool KeyPressed(KeyCode legacyKey)
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current == null) return false;
        switch (legacyKey)
        {
            case KeyCode.Q: return Keyboard.current.qKey.wasPressedThisFrame;
            case KeyCode.E: return Keyboard.current.eKey.wasPressedThisFrame;
            case KeyCode.R: return Keyboard.current.rKey.wasPressedThisFrame;
            case KeyCode.F: return Keyboard.current.fKey.wasPressedThisFrame;
            default: return false;
        }
#else
        return Input.GetKeyDown(legacyKey);
#endif
    }
}
