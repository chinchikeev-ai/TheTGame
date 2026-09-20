using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class EnemyInspectorPresentation : MonoBehaviour
{
    const int HitCapacity = 64;
    static readonly string[] BlockingMenuNames = { "MainMenu", "LevelSelect", "Settings", "PauseMenu", "EndMenu", "ConfirmationModal" };

    readonly RaycastHit[] hits = new RaycastHit[HitCapacity];
    Camera gameCamera;
    Canvas canvas;
    Canvas menuCanvas;
    GameObject panel;
    Image portrait;
    Text title;
    Text stats;
    Enemy selected;

    void Start()
    {
        gameCamera = Camera.main;
        BindMenuCanvas();
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

        if (menuCanvas == null) BindMenuCanvas();
        if (IsMenuBlockingCombat())
        {
            if (panel != null) panel.SetActive(false);
            return;
        }

        if (gameCamera == null) gameCamera = Camera.main;
        if (gameCamera == null) return;

        bool overUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if (GameInput.PrimaryPressed() && !overUi) SelectAtPointer();

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
        portrait.sprite = EnemyPortrait(selected.Archetype);
        title.text = CombatUiLabels.EnemyName(selected.Archetype) + "  •  " + CombatUiLabels.ArchetypeLabel(selected.Archetype);
        string blocked = selected.IsBlockedByGuard
            ? GameLanguage.T("BLOCKED BY TROJAN GUARD", "ЗАБЛОКИРОВАН ТРОЯНСКОЙ СТРАЖЕЙ")
            : GameLanguage.T("ADVANCING", "ПРОДВИГАЕТСЯ");
        stats.text =
            $"{GameLanguage.T("HP", "ЗДОРОВЬЕ")}  {Mathf.CeilToInt(selected.Health)} / {Mathf.CeilToInt(selected.maxHealth)}\n" +
            $"{GameLanguage.T("SPEED", "СКОРОСТЬ")}  {selected.speed:0.00}\n" +
            $"{GameLanguage.T("ARMOR", "БРОНЯ")}  {selected.Armor * 100f:0}%\n" +
            $"{GameLanguage.T("ARROW RESIST", "ЗАЩИТА ОТ СТРЕЛ")}  {selected.ArrowResistance * 100f:0}%\n" +
            $"{GameLanguage.T("GATE DAMAGE", "УРОН ВОРОТАМ")}  {selected.baseDamage}\n" +
            $"{GameLanguage.T("BOUNTY", "НАГРАДА")}  {GameManager.Instance.RewardFor(selected.reward)}\n" +
            $"{GameLanguage.T("ROUTE", "МАРШРУТ")}  {selected.RouteProgress * 100f:0}%\n" + blocked;
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
        image.sprite = TroyHudArt.Panel();
        image.type = Image.Type.Sliced;
        image.color = Color.white;
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1f, .5f);
        rt.anchoredPosition = new Vector2(-24f, 8f);
        rt.sizeDelta = new Vector2(350f, 300f);

        MakeImage(panel.transform, "EnemyPortraitFrame", new Vector2(-115f, 86f), new Vector2(108f, 108f), TroyHudArt.Panel());
        portrait = MakeImage(panel.transform, "EnemyPortrait", new Vector2(-115f, 86f), new Vector2(88f, 88f), TroyHudArt.Icon("enemy"));
        title = MakeText(panel.transform, GameLanguage.T("ENEMY", "ПРОТИВНИК"), new Vector2(43f, 111f), new Vector2(205f, 40f), 18, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(1f, .69f, .24f, 1f));
        MakeText(panel.transform, GameLanguage.T("THREAT PROFILE", "ПРОФИЛЬ УГРОЗЫ"), new Vector2(43f, 80f), new Vector2(205f, 24f), 10, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(.73f, .66f, .58f, 1f));
        stats = MakeText(panel.transform, "", new Vector2(0f, -55f), new Vector2(296f, 190f), 14, FontStyle.Bold, TextAnchor.UpperLeft, new Color(.94f, .85f, .72f, 1f));
        panel.SetActive(false);
    }

    void BindMenuCanvas()
    {
        menuCanvas = GameMenuController.Instance != null ? GameMenuController.Instance.MenuCanvas : null;
    }

    bool IsMenuBlockingCombat()
    {
        if (menuCanvas == null) return false;
        for (int i = 0; i < BlockingMenuNames.Length; i++)
        {
            Transform screen = menuCanvas.transform.Find(BlockingMenuNames[i]);
            if (screen != null && screen.gameObject.activeInHierarchy) return true;
        }
        return false;
    }

    static Sprite EnemyPortrait(EnemyArchetype archetype)
    {
        switch (archetype)
        {
            case EnemyArchetype.Runner: return TroyHudArt.Enemy("runner");
            case EnemyArchetype.HeavyHoplite: return TroyHudArt.Enemy("heavy");
            case EnemyArchetype.ShieldBearer: return TroyHudArt.Enemy("shield");
            case EnemyArchetype.Archer: return TroyHudArt.Enemy("archer");
            case EnemyArchetype.Boss: return TroyHudArt.Enemy("boss");
            default: return TroyHudArt.Enemy("infantry");
        }
    }

    Image MakeImage(Transform parent, string name, Vector2 pos, Vector2 size, Sprite sprite)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.sprite = sprite;
        image.raycastTarget = false;
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return image;
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
