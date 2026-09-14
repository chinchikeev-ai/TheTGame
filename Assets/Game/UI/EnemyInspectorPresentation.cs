using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class EnemyInspectorPresentation : MonoBehaviour
{
    const int HitCapacity = 64;

    readonly RaycastHit[] hits = new RaycastHit[HitCapacity];
    Camera gameCamera;
    Canvas canvas;
    GameObject panel;
    Text title;
    Text stats;
    Enemy selected;

    void Start()
    {
        gameCamera = Camera.main;
        Build();
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || gm.GameEnded)
        {
            ClearSelection();
            return;
        }

        if (gameCamera == null) gameCamera = Camera.main;
        if (gameCamera == null) return;

        bool overUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if (GameInput.PrimaryPressed() && !overUi)
            SelectAtPointer();

        if (selected == null || !selected.IsAlive)
        {
            ClearSelection();
            return;
        }

        Refresh();
    }

    void SelectAtPointer()
    {
        Ray ray = gameCamera.ScreenPointToRay(GameInput.PointerPosition);
        int count = Physics.RaycastNonAlloc(ray, hits, 250f);
        Enemy closest = null;
        float closestDistance = float.PositiveInfinity;

        for (int i = 0; i < count; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.collider == null || hit.distance >= closestDistance) continue;
            Enemy candidate = hit.collider.GetComponentInParent<Enemy>();
            if (candidate == null || !candidate.IsAlive) continue;
            closest = candidate;
            closestDistance = hit.distance;
        }

        selected = closest;
        if (panel != null) panel.SetActive(selected != null);
        if (selected != null)
        {
            RuntimeFileLogger.Event("ENEMY_INSPECT", $"Selected {selected.name} archetype={selected.Archetype}");
            Refresh();
        }
    }

    void Refresh()
    {
        if (selected == null || panel == null) return;
        panel.SetActive(true);
        title.text = selected.name.ToUpperInvariant() + "  •  " + selected.Archetype;

        string blocked = selected.IsBlockedByGuard
            ? GameLanguage.T("BLOCKED BY TROJAN GUARD", "ЗАБЛОКИРОВАН ТРОЯНСКОЙ СТРАЖЕЙ")
            : GameLanguage.T("ADVANCING", "ПРОДВИГАЕТСЯ");
        stats.text =
            $"HP  {Mathf.CeilToInt(selected.Health)} / {Mathf.CeilToInt(selected.maxHealth)}\n" +
            $"{GameLanguage.T("SPEED", "СКОРОСТЬ")}  {selected.speed:0.00}\n" +
            $"{GameLanguage.T("ARMOR", "БРОНЯ")}  {selected.Armor * 100f:0}%\n" +
            $"{GameLanguage.T("ARROW RESIST", "ЗАЩИТА ОТ СТРЕЛ")}  {selected.ArrowResistance * 100f:0}%\n" +
            $"{GameLanguage.T("GATE DAMAGE", "УРОН ВОРОТАМ")}  {selected.baseDamage}\n" +
            $"{GameLanguage.T("BOUNTY", "НАГРАДА")}  {GameManager.Instance.RewardFor(selected.reward)}\n" +
            $"{GameLanguage.T("ROUTE", "МАРШРУТ")}  {selected.RouteProgress * 100f:0}%\n" +
            blocked;
    }

    void ClearSelection()
    {
        selected = null;
        if (panel != null) panel.SetActive(false);
    }

    void Build()
    {
        GameObject canvasObject = new GameObject("EnemyInspectorCanvas");
        canvasObject.transform.SetParent(transform, false);
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 84;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;
        canvasObject.AddComponent<GraphicRaycaster>();

        panel = new GameObject("EnemyInspectorPanel");
        panel.transform.SetParent(canvasObject.transform, false);
        Image image = panel.AddComponent<Image>();
        image.color = new Color(.035f, .02f, .014f, .96f);
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1f, .5f);
        rt.anchoredPosition = new Vector2(-24f, -20f);
        rt.sizeDelta = new Vector2(360f, 330f);
        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(.72f, .31f, .10f, .72f);
        outline.effectDistance = new Vector2(2f, -2f);

        title = MakeText(panel.transform, "ENEMY", new Vector2(0, 132), new Vector2(320, 46), 20, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(1f, .69f, .24f, 1f));
        MakeText(panel.transform, GameLanguage.T("ENEMY CHARACTERISTICS", "ХАРАКТЕРИСТИКИ ПРОТИВНИКА"), new Vector2(0, 95), new Vector2(320, 26), 11, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(.73f, .66f, .58f, 1f));
        stats = MakeText(panel.transform, "", new Vector2(0, -30), new Vector2(300, 220), 15, FontStyle.Bold, TextAnchor.UpperLeft, new Color(.94f, .85f, .72f, 1f));
        panel.SetActive(false);
    }

    Text MakeText(Transform parent, string value, Vector2 pos, Vector2 size, int fontSize, FontStyle style, TextAnchor anchor, Color color)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = color;
        text.alignment = anchor;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.raycastTarget = false;
        RectTransform rt = text.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return text;
    }
}
