using UnityEngine;
using UnityEngine.UI;

public sealed class CampaignMapPresentation : MonoBehaviour
{
    Canvas canvas;

    string L(string en, string ru) => GameLanguage.T(en, ru);
    CampaignController Campaign => CampaignController.Instance;

    void Start()
    {
        canvas = FindMenuCanvas();
        if (canvas == null) return;
        Build();
    }

    public void Refresh()
    {
        if (canvas == null) canvas = FindMenuCanvas();
        if (canvas == null) return;
        Build();
    }

    Canvas FindMenuCanvas()
    {
        return GameMenuController.Instance != null ? GameMenuController.Instance.MenuCanvas : null;
    }

    void Build()
    {
        Transform levelSelect = canvas.transform.Find("LevelSelect");
        if (levelSelect == null) return;
        if (levelSelect.GetComponent<ChapterSelectionArtwork>() != null)
        {
            Transform legacy = levelSelect.Find("CampaignMapLayer");
            if (legacy != null)
            {
                legacy.gameObject.SetActive(false);
                Destroy(legacy.gameObject);
            }
            return;
        }

        Transform levelCard = levelSelect.Find("LevelCard");
        if (levelCard != null) RecomposeLevelCard(levelCard);

        Transform old = levelSelect.Find("CampaignMapLayer");
        if (old != null) Destroy(old.gameObject);

        GameObject layer = new GameObject("CampaignMapLayer");
        layer.transform.SetParent(levelSelect, false);
        RectTransform root = layer.AddComponent<RectTransform>();
        root.anchorMin = Vector2.zero;
        root.anchorMax = Vector2.one;
        root.offsetMin = root.offsetMax = Vector2.zero;
        root.SetAsFirstSibling();

        Image shade = layer.AddComponent<Image>();
        shade.color = new Color(.035f, .014f, .006f, .36f);
        shade.raycastTarget = false;

        GameObject mapField = MakePanel(
            layer.transform,
            "MapField",
            new Vector2(-330f, 0f),
            new Vector2(1040f, 760f),
            new Color(.13f, .070f, .030f, .93f),
            new Color(.96f, .52f, .13f, .62f));

        BuildMapBackdrop(mapField.transform);

        Vector2[] points =
        {
            new Vector2(-338f, -178f),
            new Vector2(-218f, -88f),
            new Vector2(-72f, 18f),
            new Vector2(  86f, 118f),
            new Vector2( 245f,  86f),
            new Vector2( 160f, -72f),
            new Vector2( 310f,-170f)
        };

        int unlockedChapter = Campaign != null ? Mathf.Clamp(Campaign.UnlockedChapter, 1, 7) : 1;
        for (int i = 0; i < points.Length - 1; i++)
            MakeRoute(mapField.transform, points[i], points[i + 1], i + 1 < unlockedChapter);

        for (int i = 0; i < points.Length; i++)
            MakeNode(mapField.transform, i + 1, points[i], i + 1 == unlockedChapter);

        MakeMapHeader(mapField.transform);
        MakeProgressHeader(mapField.transform);
        MakeMapLegend(mapField.transform);
        MakeCurrentObjective(mapField.transform, unlockedChapter);

        RuntimeFileLogger.Event("MENU", "Campaign map rebuilt as Trojan war route");
    }

    void BuildMapBackdrop(Transform parent)
    {
        // Sea / coast / plains / Troy are intentionally stylised UI shapes rather than generic fantasy art.
        AddRect(parent, "Sea", new Vector2(-390f, -10f), new Vector2(230f, 690f), new Color(.075f, .22f, .30f, .92f));
        AddRect(parent, "SeaHighlight", new Vector2(-305f, -10f), new Vector2(9f, 690f), new Color(.22f, .56f, .63f, .62f));
        AddRect(parent, "Beach", new Vector2(-275f, -10f), new Vector2(70f, 690f), new Color(.64f, .46f, .24f, .88f));
        AddRect(parent, "Plains", new Vector2(75f, -10f), new Vector2(625f, 690f), new Color(.31f, .23f, .11f, .74f));

        // A few terrain bands make the route read like a hand-drawn campaign board.
        AddRect(parent, "HillBandA", new Vector2(-65f, 155f), new Vector2(390f, 95f), new Color(.22f, .18f, .08f, .44f), 7f);
        AddRect(parent, "HillBandB", new Vector2(80f, -165f), new Vector2(430f, 92f), new Color(.18f, .14f, .07f, .46f), -8f);
        AddRect(parent, "RoadDust", new Vector2(55f, 5f), new Vector2(520f, 25f), new Color(.70f, .49f, .22f, .20f), 11f);

        MakeGreekShip(parent, new Vector2(-412f, 175f), 1f);
        MakeGreekShip(parent, new Vector2(-395f, 40f), .84f);
        MakeGreekShip(parent, new Vector2(-420f, -110f), .72f);

        MakeTroy(parent);

        AddMapLabel(parent, L("AEGEAN SEA", "ЭГЕЙСКОЕ МОРЕ"), new Vector2(-405f, 300f), 14, new Color(.65f, .88f, .92f, .82f), 210f);
        AddMapLabel(parent, L("THE BEACH", "БЕРЕГ"), new Vector2(-270f, 300f), 13, new Color(1f, .82f, .49f, .90f), 130f);
        AddMapLabel(parent, L("PLAINS OF TROY", "РАВНИНЫ ТРОИ"), new Vector2(35f, 300f), 13, new Color(.90f, .76f, .50f, .78f), 260f);
    }

    void MakeGreekShip(Transform parent, Vector2 pos, float scale)
    {
        GameObject root = new GameObject("GreekShip");
        root.transform.SetParent(parent, false);
        RectTransform rr = root.AddComponent<RectTransform>();
        rr.anchorMin = rr.anchorMax = rr.pivot = new Vector2(.5f, .5f);
        rr.anchoredPosition = pos;
        rr.sizeDelta = new Vector2(100f * scale, 70f * scale);
        rr.localRotation = Quaternion.Euler(0f, 0f, -7f);

        AddRect(root.transform, "Hull", new Vector2(0f, -13f * scale), new Vector2(82f * scale, 18f * scale), new Color(.29f, .105f, .035f, .96f));
        AddRect(root.transform, "Mast", new Vector2(0f, 7f * scale), new Vector2(5f * scale, 50f * scale), new Color(.37f, .16f, .055f, 1f));
        AddRect(root.transform, "Sail", new Vector2(18f * scale, 9f * scale), new Vector2(38f * scale, 34f * scale), new Color(.82f, .69f, .49f, .92f), -8f);
        AddRect(root.transform, "RedMark", new Vector2(18f * scale, 9f * scale), new Vector2(5f * scale, 28f * scale), new Color(.50f, .08f, .025f, .90f), -8f);
    }

    void MakeTroy(Transform parent)
    {
        GameObject city = new GameObject("Troy");
        city.transform.SetParent(parent, false);
        RectTransform rr = city.AddComponent<RectTransform>();
        rr.anchorMin = rr.anchorMax = rr.pivot = new Vector2(.5f, .5f);
        rr.anchoredPosition = new Vector2(385f, -15f);
        rr.sizeDelta = new Vector2(210f, 420f);

        AddRect(city.transform, "Wall", new Vector2(0f, -35f), new Vector2(170f, 255f), new Color(.55f, .31f, .11f, .96f));
        AddRect(city.transform, "WallLight", new Vector2(-55f, -35f), new Vector2(18f, 255f), new Color(.84f, .54f, .21f, .34f));
        AddRect(city.transform, "TowerTop", new Vector2(0f, 112f), new Vector2(190f, 34f), new Color(.68f, .37f, .10f, 1f));
        AddRect(city.transform, "Gate", new Vector2(0f, -115f), new Vector2(64f, 92f), new Color(.115f, .045f, .018f, 1f));
        AddRect(city.transform, "GateGlow", new Vector2(0f, -107f), new Vector2(46f, 60f), new Color(.82f, .25f, .055f, .35f));
        AddRect(city.transform, "BannerL", new Vector2(-72f, 65f), new Vector2(18f, 62f), new Color(.58f, .08f, .025f, .96f));
        AddRect(city.transform, "BannerR", new Vector2(72f, 65f), new Vector2(18f, 62f), new Color(.58f, .08f, .025f, .96f));

        Text label = AddMapLabel(city.transform, L("TROY", "ТРОЯ"), new Vector2(0f, 168f), 24, new Color(1f, .62f, .16f, 1f), 190f);
        Shadow shadow = label.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(.10f, .02f, .003f, .95f);
        shadow.effectDistance = new Vector2(3f, -3f);
    }

    void RecomposeLevelCard(Transform levelCard)
    {
        RectTransform card = levelCard as RectTransform;
        if (card == null) return;
        card.anchorMin = card.anchorMax = card.pivot = new Vector2(.5f, .5f);
        card.anchoredPosition = new Vector2(560f, 0f);
        card.sizeDelta = new Vector2(600f, 760f);

        Image image = levelCard.GetComponent<Image>();
        if (image != null) image.color = new Color(.075f, .030f, .012f, .985f);

        Outline outline = levelCard.GetComponent<Outline>();
        if (outline == null) outline = levelCard.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(.96f, .52f, .13f, .78f);
        outline.effectDistance = new Vector2(3f, -3f);

        Text[] texts = levelCard.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            RectTransform rt = texts[i].rectTransform;
            string value = texts[i].text ?? string.Empty;
            if (value.Contains("CHAPTER SELECT") || value.Contains("ВЫБОР ГЛАВЫ"))
            {
                texts[i].color = new Color(1f, .67f, .17f, 1f);
                texts[i].fontSize = 43;
                rt.anchoredPosition = new Vector2(0, 300);
                rt.sizeDelta = new Vector2(530, 72);
            }
            else if (value.Contains("Choose where") || value.Contains("Выберите этап"))
            {
                texts[i].text = L("Choose the next battle on the road to Troy", "Выберите следующую битву на пути к Трое");
                texts[i].color = new Color(.86f, .73f, .58f, .96f);
                rt.anchoredPosition = new Vector2(0, 242);
                rt.sizeDelta = new Vector2(510, 52);
            }
        }

        Button[] buttons = levelCard.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
        {
            RectTransform rt = buttons[i].transform as RectTransform;
            if (rt == null) continue;
            Text label = buttons[i].GetComponentInChildren<Text>(true);
            if (label == null) continue;

            if (label.text.Contains("THE LANDING") || label.text.Contains("ВЫСАДКА"))
            {
                rt.anchoredPosition = new Vector2(0, 95);
                rt.sizeDelta = new Vector2(480, 82);
            }
            else if (label.text.Contains("ROAD TO TROY") || label.text.Contains("ДОРОГА К ТРОЕ"))
            {
                rt.anchoredPosition = new Vector2(0, -8);
                rt.sizeDelta = new Vector2(480, 74);
            }
            else if (label.text == "BACK" || label.text == "НАЗАД")
            {
                rt.anchoredPosition = new Vector2(0, -305);
                rt.sizeDelta = new Vector2(250, 58);
            }
        }
    }

    void MakeRoute(Transform parent, Vector2 a, Vector2 b, bool active)
    {
        Vector2 delta = b - a;
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        GameObject shadow = AddRect(parent, "RouteShadow", (a + b) * .5f, new Vector2(delta.magnitude, 12f), new Color(.075f, .030f, .010f, .82f), angle);
        shadow.transform.SetAsFirstSibling();

        AddRect(
            parent,
            active ? "RouteActive" : "RouteLocked",
            (a + b) * .5f,
            new Vector2(delta.magnitude, active ? 6f : 4f),
            active ? new Color(1f, .43f, .08f, .88f) : new Color(.31f, .24f, .17f, .72f),
            angle);

        // Dashes make the route feel like a campaign board rather than a plain UI line.
        int dashCount = Mathf.Max(2, Mathf.RoundToInt(delta.magnitude / 46f));
        for (int i = 1; i < dashCount; i++)
        {
            float t = i / (float)dashCount;
            Vector2 p = Vector2.Lerp(a, b, t);
            AddRect(parent, "RouteDash", p, new Vector2(8f, 8f), active ? new Color(1f, .73f, .27f, .86f) : new Color(.44f, .36f, .27f, .60f), 45f);
        }
    }

    void MakeNode(Transform parent, int chapter, Vector2 pos, bool current)
    {
        bool unlocked = Campaign != null && Campaign.IsChapterUnlocked(chapter);
        ChapterProgress progress = Campaign != null ? Campaign.GetProgress(chapter) : null;
        bool completed = progress != null && progress.completed;

        if (current && unlocked)
        {
            GameObject halo = AddRect(parent, "CurrentHalo_" + chapter, pos, new Vector2(108f, 108f), new Color(1f, .46f, .08f, .20f), 45f);
            halo.AddComponent<CampaignMapPulse>();
        }

        GameObject go = new GameObject("ChapterNode_" + chapter);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = completed
            ? new Color(.70f, .20f, .045f, .98f)
            : unlocked ? new Color(.43f, .20f, .065f, .98f)
            : new Color(.105f, .078f, .060f, .95f);

        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        float nodeSize = chapter == 1 ? 82f : 70f;
        rt.sizeDelta = new Vector2(nodeSize, nodeSize);
        rt.localRotation = Quaternion.Euler(0f, 0f, 45f);
        image.raycastTarget = false;

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = unlocked ? new Color(1f, .61f, .16f, .88f) : new Color(.30f, .23f, .17f, .56f);
        outline.effectDistance = new Vector2(3f, -3f);

        GameObject badge = new GameObject("Badge");
        badge.transform.SetParent(go.transform, false);
        RectTransform badgeRect = badge.AddComponent<RectTransform>();
        badgeRect.anchorMin = badgeRect.anchorMax = badgeRect.pivot = new Vector2(.5f, .5f);
        badgeRect.sizeDelta = new Vector2(nodeSize, nodeSize);
        badgeRect.localRotation = Quaternion.Euler(0f, 0f, -45f);

        Text numeral = MakeText(badge.transform, ToRoman(chapter), new Vector2(0f, 2f), chapter == 1 ? 28 : 22,
            unlocked ? new Color(1f, .88f, .58f, 1f) : new Color(.53f, .47f, .40f, 1f));
        numeral.rectTransform.sizeDelta = new Vector2(80f, 44f);

        Vector2 textOffset = chapter == 7 ? new Vector2(-10f, -59f) : new Vector2(0f, -57f);
        Text titleText = MakeText(parent, ChapterName(chapter), pos + textOffset, 11,
            unlocked ? new Color(.97f, .84f, .64f, 1f) : new Color(.53f, .47f, .40f, .92f));
        titleText.rectTransform.sizeDelta = new Vector2(chapter == 4 ? 175f : 145f, 34f);

        string state;
        if (completed)
            state = progress.bestScore > 0 ? L($"BEST {progress.bestScore:N0}", $"ЛУЧШИЙ {progress.bestScore:N0}") : L("COMPLETED", "ПРОЙДЕНО");
        else if (unlocked)
            state = current ? L("NEXT BATTLE", "СЛЕДУЮЩИЙ БОЙ") : L("AVAILABLE", "ДОСТУПНО");
        else
            state = L("LOCKED", "ЗАКРЫТО");

        Text stateText = MakeText(parent, state, pos + new Vector2(0f, -79f), 9,
            completed ? new Color(1f, .49f, .14f, .96f)
                      : current && unlocked ? new Color(1f, .70f, .22f, .98f)
                      : new Color(.66f, .58f, .49f, .86f));
        stateText.rectTransform.sizeDelta = new Vector2(150f, 22f);
    }

    void MakeMapHeader(Transform parent)
    {
        Text text = MakeText(parent, L("THE WAR FOR TROY", "ВОЙНА ЗА ТРОЮ"), new Vector2(0f, 338f), 27,
            new Color(1f, .68f, .20f, 1f));
        text.rectTransform.sizeDelta = new Vector2(560f, 48f);

        Shadow shadow = text.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(.08f, .018f, .003f, .95f);
        shadow.effectDistance = new Vector2(3f, -3f);
    }

    void MakeProgressHeader(Transform parent)
    {
        int unlocked = Campaign != null ? Mathf.Clamp(Campaign.UnlockedChapter, 1, 7) : 1;
        int completed = 0;
        if (Campaign != null)
        {
            for (int i = 1; i <= 7; i++)
            {
                ChapterProgress progress = Campaign.GetProgress(i);
                if (progress != null && progress.completed) completed++;
            }
        }

        Text text = MakeText(parent,
            L($"CAMPAIGN  •  CHAPTER {unlocked}/7  •  COMPLETED {completed}/7", $"КАМПАНИЯ  •  ГЛАВА {unlocked}/7  •  ПРОЙДЕНО {completed}/7"),
            new Vector2(0f, 304f), 12, new Color(.86f, .73f, .57f, .92f));
        text.rectTransform.sizeDelta = new Vector2(620f, 30f);
    }

    void MakeMapLegend(Transform parent)
    {
        GameObject legend = MakePanel(parent, "Legend", new Vector2(-335f, -302f), new Vector2(265f, 68f),
            new Color(.055f, .025f, .011f, .86f), new Color(.55f, .30f, .11f, .56f));
        AddMapLabel(legend.transform,
            L("GREEK LANDING  →  TROY", "ВЫСАДКА ГРЕКОВ  →  ТРОЯ"),
            new Vector2(0f, 12f), 11, new Color(.93f, .78f, .57f, .94f), 240f);
        AddMapLabel(legend.transform,
            L("Follow the campaign route", "Следуйте по маршруту кампании"),
            new Vector2(0f, -13f), 10, new Color(.68f, .61f, .54f, .88f), 240f);
    }

    void MakeCurrentObjective(Transform parent, int chapter)
    {
        string objective;
        switch (chapter)
        {
            case 1: objective = L("STOP THE LANDING", "ОСТАНОВИТЬ ВЫСАДКУ"); break;
            case 2: objective = L("HOLD THE ROAD TO TROY", "УДЕРЖАТЬ ДОРОГУ К ТРОЕ"); break;
            case 3: objective = L("DEFEND THE GATES", "ЗАЩИТИТЬ ВРАТА"); break;
            case 4: objective = L("FACE THE GREEK HEROES", "ВСТРЕТИТЬ ГЕРОЕВ ГРЕЦИИ"); break;
            case 5: objective = L("SURVIVE THE GREAT ASSAULT", "ПЕРЕЖИТЬ ВЕЛИКИЙ ШТУРМ"); break;
            case 6: objective = L("WATCH THE HORSE", "СЛЕДИТЬ ЗА КОНЁМ"); break;
            default: objective = L("TROY'S LAST STAND", "ПОСЛЕДНЯЯ ОБОРОНА ТРОИ"); break;
        }

        GameObject objectiveCard = MakePanel(parent, "CurrentObjective", new Vector2(250f, -300f), new Vector2(330f, 74f),
            new Color(.13f, .040f, .012f, .93f), new Color(.91f, .42f, .09f, .68f));
        AddMapLabel(objectiveCard.transform, L("CURRENT OBJECTIVE", "ТЕКУЩАЯ ЦЕЛЬ"), new Vector2(0f, 16f), 10,
            new Color(1f, .55f, .13f, 1f), 300f);
        AddMapLabel(objectiveCard.transform, objective, new Vector2(0f, -12f), 12,
            new Color(1f, .84f, .60f, 1f), 300f);
    }

    GameObject MakePanel(Transform parent, string name, Vector2 pos, Vector2 size, Color fill, Color border)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = fill;
        image.raycastTarget = false;

        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = border;
        outline.effectDistance = new Vector2(3f, -3f);

        Shadow shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color(.025f, .006f, .002f, .72f);
        shadow.effectDistance = new Vector2(0f, -8f);
        return go;
    }

    GameObject AddRect(Transform parent, string name, Vector2 pos, Vector2 size, Color color, float rotation = 0f)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;

        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        rt.localRotation = Quaternion.Euler(0f, 0f, rotation);
        return go;
    }

    Text AddMapLabel(Transform parent, string value, Vector2 pos, int size, Color color, float width)
    {
        Text text = MakeText(parent, value, pos, size, color);
        text.rectTransform.sizeDelta = new Vector2(width, 30f);
        return text;
    }

    Text MakeText(Transform parent, string value, Vector2 pos, int size, Color color)
    {
        GameObject go = new GameObject("MapText");
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = size;
        text.fontStyle = FontStyle.Bold;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;

        RectTransform rt = text.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(200f, 36f);
        return text;
    }

    string ChapterName(int chapter)
    {
        switch (chapter)
        {
            case 1: return L("THE LANDING", "ВЫСАДКА");
            case 2: return L("ROAD TO TROY", "ДОРОГА К ТРОЕ");
            case 3: return L("THE GATES", "ВРАТА");
            case 4: return L("HEROES OF GREECE", "ГЕРОИ ГРЕЦИИ");
            case 5: return L("THE GREAT ASSAULT", "ВЕЛИКИЙ ШТУРМ");
            case 6: return L("THE HORSE", "КОНЬ");
            case 7: return L("TROY BURNS", "ТРОЯ ГОРИТ");
            default: return string.Empty;
        }
    }

    string ToRoman(int chapter)
    {
        switch (chapter)
        {
            case 1: return "I";
            case 2: return "II";
            case 3: return "III";
            case 4: return "IV";
            case 5: return "V";
            case 6: return "VI";
            case 7: return "VII";
            default: return chapter.ToString();
        }
    }
}

public sealed class CampaignMapPulse : MonoBehaviour
{
    RectTransform rect;
    Vector3 baseScale;

    void Awake()
    {
        rect = transform as RectTransform;
        baseScale = rect != null ? rect.localScale : Vector3.one;
        if (baseScale == Vector3.zero) baseScale = Vector3.one;
    }

    void Update()
    {
        if (rect == null) return;
        float pulse = 1f + Mathf.Sin(Time.unscaledTime * 3.4f) * .07f;
        rect.localScale = baseScale * pulse;
    }
}
