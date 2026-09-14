using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class MenuButtonFeedback : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler,
    ISelectHandler, IDeselectHandler
{
    const float HoverScale = 1.055f;
    const float PressScale = 0.92f;
    const float ReleaseKickScale = 1.08f;
    const float Speed = 18f;

    RectTransform rect;
    Graphic targetGraphic;
    Vector3 baseScale;
    Vector3 wantedScale;
    Color baseColor;
    bool highlighted;

    void Awake()
    {
        rect = transform as RectTransform;
        targetGraphic = GetComponent<Graphic>();
        baseScale = rect != null ? rect.localScale : transform.localScale;
        if (baseScale == Vector3.zero) baseScale = Vector3.one;
        wantedScale = baseScale;
        if (targetGraphic != null) baseColor = targetGraphic.color;
    }

    void OnEnable()
    {
        if (rect != null)
        {
            rect.localScale = baseScale;
            wantedScale = baseScale;
        }
        ApplyHighlight(false);
    }

    void Update()
    {
        if (rect == null) return;
        float t = 1f - Mathf.Exp(-Speed * Time.unscaledDeltaTime);
        rect.localScale = Vector3.Lerp(rect.localScale, wantedScale, t);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        ApplyHighlight(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ApplyHighlight(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        wantedScale = baseScale * PressScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        if (rect != null) rect.localScale = baseScale * ReleaseKickScale;
        wantedScale = baseScale * (highlighted ? HoverScale : 1f);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!IsInteractable()) return;
        ApplyHighlight(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ApplyHighlight(false);
    }

    void ApplyHighlight(bool value)
    {
        highlighted = value && IsInteractable();
        wantedScale = baseScale * (highlighted ? HoverScale : 1f);

        if (targetGraphic != null)
        {
            float multiplier = highlighted ? 1.10f : 1f;
            targetGraphic.color = new Color(
                Mathf.Clamp01(baseColor.r * multiplier),
                Mathf.Clamp01(baseColor.g * multiplier),
                Mathf.Clamp01(baseColor.b * multiplier),
                baseColor.a);
        }
    }

    bool IsInteractable()
    {
        Button button = GetComponent<Button>();
        return button == null || button.interactable;
    }
}
