using System;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class BuildButtonHoverRelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Action onEnter;
    Action onExit;

    public void Initialize(Action enter, Action exit)
    {
        onEnter = enter;
        onExit = exit;
    }

    public void OnPointerEnter(PointerEventData eventData) => onEnter?.Invoke();
    public void OnPointerExit(PointerEventData eventData) => onExit?.Invoke();

    void OnDisable() => onExit?.Invoke();
}
