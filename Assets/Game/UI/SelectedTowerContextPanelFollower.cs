using System.Collections;
using UnityEngine;

public sealed class SelectedTowerContextPanelFollower : MonoBehaviour
{
    const float EdgePadding = 22f;
    static SelectedTowerContextPanelFollower instance;

    TowerPlacement placement;
    RectTransform panel;
    RectTransform canvasRect;
    Camera gameCamera;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        if (instance != null) return;
        GameObject host = new GameObject("SelectedTowerContextPanelFollower");
        DontDestroyOnLoad(host);
        instance = host.AddComponent<SelectedTowerContextPanelFollower>();
    }

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
        if (placement == null) placement = FindFirstObjectByType<TowerPlacement>();
        if (placement == null) return false;

        GameObject panelObject = GameObject.Find("SelectedTowerCard");
        if (panelObject == null) return false;
        panel = panelObject.transform as RectTransform;
        if (panel == null) return false;

        Canvas ownerCanvas = panel.GetComponentInParent<Canvas>();
        canvasRect = ownerCanvas != null ? ownerCanvas.transform as RectTransform : null;
        gameCamera = placement.gameCamera != null ? placement.gameCamera : Camera.main;
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

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}
