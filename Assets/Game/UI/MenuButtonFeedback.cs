using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class MenuButtonFeedback : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler,
    ISelectHandler, IDeselectHandler
{
    const float HoverScale = 1.035f;
    const float PressScale = 0.985f;
    const float Speed = 14f;

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
        wantedScale = baseScale;
        if (targetGraphic != null) baseColor = targetGraphic.color;
    }

    void OnEnable()
    {
        if (rect != null)
        {
            rect.localScale = baseScale == Vector3.zero ? Vector3.one : baseScale;
            wantedScale = rect.localScale;
        }
        ApplyHighlight(false);
    }

    void Update()
    {
        if (rect == null) return;
        rect.localScale = Vector3.Lerp(rect.localScale, wantedScale, 1f - Mathf.Exp(-Speed * Time.unscaledDeltaTime));
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
            float multiplier = highlighted ? 1.08f : 1f;
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
