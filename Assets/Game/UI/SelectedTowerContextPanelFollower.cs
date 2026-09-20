using System.Collections;
using UnityEngine;

public sealed class SelectedTowerContextPanelFollower : MonoBehaviour
{
    const float EdgePadding = 22f;
    TowerPlacement placement;
    RectTransform panel;
    RectTransform canvasRect;
    Camera gameCamera;

    IEnumerator Start()
    {
        while (true)
        {
            if (TryBind()) yield break;
            yield return null;
        }
    }

    bool TryBind()
    {
        if (placement == null) placement = TowerPlacement.Instance;
        if (placement == null) return false;

        ModernCombatHud hud = ModernCombatHud.Instance;
        panel = hud != null ? hud.SelectedCardRect : null;
        if (panel == null) return false;

        Canvas ownerCanvas = panel.GetComponentInParent<Canvas>();
        canvasRect = ownerCanvas != null ? ownerCanvas.transform as RectTransform : null;
        gameCamera = hud.GameplayCamera != null ? hud.GameplayCamera : (placement.gameCamera != null ? placement.gameCamera : Camera.main);
        return canvasRect != null && gameCamera != null;
    }

    void LateUpdate()
    {
        if (placement == null || panel == null || canvasRect == null || gameCamera == null) return;
        Tower tower = placement.SelectedTower;
        if (tower == null || !panel.gameObject.activeInHierarchy) return;

        Vector3 screenPoint = gameCamera.WorldToScreenPoint(tower.transform.position + Vector3.up * 1.35f);
        if (screenPoint.z <= 0f) return;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out Vector2 localPoint)) return;

        Vector2 offset = new Vector2(205f, 72f);
        if (screenPoint.x > Screen.width * .68f) offset.x = -205f;
        if (screenPoint.y > Screen.height * .72f) offset.y = -105f;

        Vector2 desired = localPoint + offset;
        Vector2 half = panel.rect.size * .5f;
        Rect bounds = canvasRect.rect;
        desired.x = Mathf.Clamp(desired.x, bounds.xMin + half.x + EdgePadding, bounds.xMax - half.x - EdgePadding);
        desired.y = Mathf.Clamp(desired.y, bounds.yMin + half.y + EdgePadding, bounds.yMax - half.y - EdgePadding);

        panel.anchorMin = panel.anchorMax = panel.pivot = new Vector2(.5f, .5f);
        panel.anchoredPosition = desired;
    }
}
