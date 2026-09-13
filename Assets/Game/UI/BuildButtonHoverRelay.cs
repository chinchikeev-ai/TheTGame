using UnityEngine;
using UnityEngine.EventSystems;

public sealed class BuildButtonHoverRelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    System.Action onEnter;
    System.Action onExit;

    public void Initialize(System.Action enter, System.Action exit)
    {
        onEnter = enter;
        onExit = exit;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke();
        BuildTooltipTacticalDecorator.Apply(transform);
    }

    public void OnPointerExit(PointerEventData eventData) => onExit?.Invoke();
    void OnDisable() => onExit?.Invoke();
}
