using UnityEngine;
using UnityEngine.UI;

public sealed class TowerContextActionHud : MonoBehaviour
{
    TowerPlacement placement;
    Canvas canvas;
    RectTransform canvasRect;
    RectTransform root;
    GameObject oldSelectedCard;
    Text title;
    Text stats;
    Button upgradeButton;
    Button sellButton;
    Button priorityButton;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<TowerContextActionHud>() == null)
            new GameObject("TowerContextActionHud").AddComponent<TowerContextActionHud>();
    }

    void Start()
    {
        placement = FindFirstObjectByType<TowerPlacement>();
        Build();
    }

    void Build()
    {
        GameObject canvasObject = new GameObject("TowerContextActionCanvas");
        canvasObject.transform.SetParent(transform, false);
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 90;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;
        canvasObject.AddComponent<GraphicRaycaster>();
        canvasRect = canvasObject.GetComponent<RectTransform>();

        GameObject rootObject = new GameObject("TowerContextActions");
        rootObject.transform.SetParent(canvasObject.transform, false);
        root = rootObject.AddComponent<RectTransform>();
        root.anchorMin = root.anchorMax = root.pivot = new Vector2(.5f, .5f);
        root.sizeDelta = new Vector2(430f, 330f);

        title = Label(root, "Title", new Vector2(0f, 132f), new Vector2(350f, 42f), 20, FontStyle.Bold, new Color(1f, .76f, .30f, 1f));
        stats = Label(root, "Stats", new Vector2(0f, 96f), new Vector2(360f, 34f), 14, FontStyle.Bold, new Color(.96f, .88f, .76f, 1f));

        upgradeButton = ActionButton(root, GameLanguage.T("UPGRADE", "УЛУЧШИТЬ"), new Vector2(-122f, 22f), () => placement?.UpgradeSelected(), true);
        priorityButton = ActionButton(root, GameLanguage.T("TARGET", "ЦЕЛЬ"), new Vector2(122f, 22f), () => placement?.CycleSelectedPriority(), false);
        sellButton = ActionButton(root, GameLanguage.T("SELL", "ПРОДАТЬ"), new Vector2(0f, -92f), () => placement?.SellSelected(), false);

        root.gameObject.SetActive(false);
    }

    void Update()
    {
        if (placement == null) placement = FindFirstObjectByType<TowerPlacement>();
        HideLegacySelectedCard();

        Tower tower = placement != null ? placement.SelectedTower : null;
        if (tower == null || Camera.main == null)
        {
            if (root != null) root.gameObject.SetActive(false);
            return;
        }

        Vector3 screen = Camera.main.WorldToScreenPoint(tower.transform.position + Vector3.up * 1.0f);
        if (screen.z <= 0f)
        {
            root.gameObject.SetActive(false);
            return;
        }

        root.gameObject.SetActive(true);
        PositionAroundTower(screen);

        title.text = $"{tower.DisplayName}   LV {tower.Level}/3";
        stats.text = $"{GameLanguage.T("DMG", "УРОН")} {tower.damage:0}   •   {GameLanguage.T("RNG", "ДАЛЬН")} {tower.range:0.0}   •   {GameLanguage.T("RATE", "СКОР")} {tower.fireRate:0.00}/s";

        upgradeButton.GetComponentInChildren<Text>().text = tower.Level >= 3
            ? GameLanguage.T("MAX LEVEL", "МАКС. УРОВЕНЬ")
            : $"{GameLanguage.T("UPGRADE", "УЛУЧШИТЬ")}\n{tower.UpgradeCost} {GameLanguage.T("GOLD", "ЗОЛОТА")}";
        upgradeButton.interactable = tower.Level < 3 && GameManager.Instance != null && GameManager.Instance.Money >= tower.UpgradeCost;

        sellButton.GetComponentInChildren<Text>().text = $"{GameLanguage.T("SELL", "ПРОДАТЬ")}\n+{tower.SellValue}";
        priorityButton.gameObject.SetActive(tower.Type != TowerType.TrojanGuard);
        if (tower.Type != TowerType.TrojanGuard)
            priorityButton.GetComponentInChildren<Text>().text = $"{GameLanguage.T("TARGET", "ЦЕЛЬ")}\n{tower.Priority}";
    }

    void HideLegacySelectedCard()
    {
        if (oldSelectedCard == null)
            oldSelectedCard = GameObject.Find("SelectedTowerCard");
        if (oldSelectedCard != null && oldSelectedCard.activeSelf)
            oldSelectedCard.SetActive(false);
    }

    void PositionAroundTower(Vector3 screenPoint)
    {
        if (canvasRect == null) return;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out Vector2 local)) return;

        Rect rect = canvasRect.rect;
        const float halfWidth = 215f;
        const float halfHeight = 165f;
        const float sideMargin = 24f;
        const float bottomReserved = 205f;
        const float topReserved = 190f;

        local.x = Mathf.Clamp(local.x, rect.xMin + halfWidth + sideMargin, rect.xMax - halfWidth - sideMargin);
        local.y = Mathf.Clamp(local.y + 24f, rect.yMin + halfHeight + bottomReserved, rect.yMax - halfHeight - topReserved);
        root.anchoredPosition = local;
    }

    Button ActionButton(Transform parent, string text, Vector2 position, UnityEngine.Events.UnityAction action, bool primary)
    {
        GameObject go = new GameObject(text);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = primary ? new Color(.57f, .12f, .05f, .98f) : new Color(.20f, .12f, .07f, .97f);
        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(.82f, .48f, .17f, .75f);
        outline.effectDistance = new Vector2(1.4f, -1.4f);

        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        go.AddComponent<MenuButtonFeedback>();
        go.AddComponent<MenuUiAudioFeedback>();

        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(168f, 62f);

        Label(go.transform, "Text", Vector2.zero, new Vector2(158f, 56f), 14, FontStyle.Bold, new Color(1f, .88f, .66f, 1f));
        button.GetComponentInChildren<Text>().text = text;
        return button;
    }

    Text Label(Transform parent, string name, Vector2 position, Vector2 size, int fontSize, FontStyle style, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.raycastTarget = false;

        RectTransform rt = text.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        return text;
    }
}
