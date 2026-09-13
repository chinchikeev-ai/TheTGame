using UnityEngine;
using UnityEngine.EventSystems;

public sealed class BuildButtonHoverRelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    System.Action onEnter;
    System.Action onExit;
    TowerType towerType;

    public void Initialize(TowerType type, System.Action enter, System.Action exit)
    {
        towerType = type;
        onEnter = enter;
        onExit = exit;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke();
        BuildTooltipTacticalDecorator.Apply(transform, towerType);
    }

    public void OnPointerExit(PointerEventData eventData) => onExit?.Invoke();
    void OnDisable() => onExit?.Invoke();
}
