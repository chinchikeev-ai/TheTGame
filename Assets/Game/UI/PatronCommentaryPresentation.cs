using UnityEngine;
using UnityEngine.UI;

public sealed class PatronCommentaryPresentation : MonoBehaviour
{
    Canvas canvas;
    CanvasGroup group;
    EnemySpawner spawner;
    Image portrait;
    Text nameText;
    Text commentText;
    GameObject card;
    Canvas menuCanvas;

    int lastEncounter = -1;
    int lastGateHealth = -1;
    bool bossCommentShown;
    bool victoryCommentShown;
    float commentUntil;
    string currentComment;
    int lastScreenWidth = -1;
    int lastScreenHeight = -1;

    string L(string en, string ru) => GameLanguage.T(en, ru);

    void Start()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();
        GameObject menu = GameObject.Find("MenuCanvas");
        menuCanvas = menu != null ? menu.GetComponent<Canvas>() : null;
        Build();
        ApplyResponsiveLayout(true);
        RefreshPatron();
    }

    void Build()
    {
        GameObject root = new GameObject("PatronCommentaryUI");
        root.transform.SetParent(transform, false);
        canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 84;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;

        group = root.AddComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;

        card = new GameObject("PatronCommentaryCard");
        card.transform.SetParent(root.transform, false);
        Image panel = card.AddComponent<Image>();
        panel.sprite = TroyHudArt.Panel();
        panel.type = Image.Type.Sliced;
        panel.color = Color.white;
        panel.raycastTarget = false;
        RectTransform panelRt = panel.rectTransform;
        panelRt.anchorMin = panelRt.anchorMax = panelRt.pivot = new Vector2(1f, 1f);

        portrait = AddImage(card.transform, "PatronPortrait", Vector2.zero, Vector2.zero);
        portrait.preserveAspect = true;

        nameText = AddText(card.transform, "", Vector2.zero, Vector2.zero, 14, new Color(1f, .72f, .25f, 1f), TextAnchor.UpperLeft, FontStyle.Bold);
        commentText = AddText(card.transform, "", Vector2.zero, Vector2.zero, 14, new Color(.96f, .88f, .76f, 1f), TextAnchor.UpperLeft, FontStyle.Normal);
    }

    void Update()
    {
        ApplyResponsiveLayout(false);

        GameManager gm = GameManager.Instance;
        if (gm == null || gm.MapNumber != 1 || !gm.GiftSelected)
        {
            if (group != null) group.alpha = 0f;
            return;
        }

        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();
        if (menuCanvas == null)
        {
            GameObject menu = GameObject.Find("MenuCanvas");
            menuCanvas = menu != null ? menu.GetComponent<Canvas>() : null;
        }

        group.alpha = gm.GameEnded || IsMenuBlockingCombat() ? 0f : 1f;
        if (group.alpha <= 0f) return;

        RefreshPatron();
        int encounter = EncounterRuntime.CurrentEncounter(spawner);

        if (lastEncounter < 0)
        {
            lastEncounter = encounter;
            lastGateHealth = gm.BaseHealth;
            Speak(IntroLine(gm.SelectedGift), 8f);
        }

        if (encounter != lastEncounter)
        {
            int previous = lastEncounter;
            lastEncounter = encounter;
            if (encounter > previous)
                Speak(EncounterLine(gm.SelectedGift, encounter), 7f);
        }

        if (lastGateHealth >= 0 && gm.BaseHealth < lastGateHealth)
            Speak(GateHitLine(gm.SelectedGift), 6f);
        lastGateHealth = gm.BaseHealth;

        bool boss = HasActiveBoss();
        if (boss && !bossCommentShown)
        {
            bossCommentShown = true;
            Speak(BossLine(gm.SelectedGift), 8f);
        }

        if (gm.BossDefeated && !victoryCommentShown)
        {
            victoryCommentShown = true;
            Speak(VictoryLine(gm.SelectedGift), 10f);
        }

        if (Time.unscaledTime > commentUntil && !gm.BossDefeated)
            commentText.text = AmbientLine(gm.SelectedGift, encounter);
        else
            commentText.text = currentComment;
    }

    void ApplyResponsiveLayout(bool force)
    {
        if (!force && Screen.width == lastScreenWidth && Screen.height == lastScreenHeight) return;
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        if (card == null || portrait == null || nameText == null || commentText == null) return;

        bool compact = Screen.width <= 1450 || Screen.height <= 800;
        RectTransform cardRt = card.transform as RectTransform;
        RectTransform portraitRt = portrait.rectTransform;
        RectTransform nameRt = nameText.rectTransform;
        RectTransform commentRt = commentText.rectTransform;

        if (compact)
        {
            cardRt.anchoredPosition = new Vector2(-16f, -96f);
            cardRt.sizeDelta = new Vector2(324f, 136f);
            portraitRt.anchoredPosition = new Vector2(-256f, -68f);
            portraitRt.sizeDelta = new Vector2(96f, 96f);
            nameRt.anchoredPosition = new Vector2(-184f, -21f);
            nameRt.sizeDelta = new Vector2(168f, 24f);
            nameText.fontSize = 12;
            commentRt.anchoredPosition = new Vector2(-184f, -49f);
            commentRt.sizeDelta = new Vector2(168f, 70f);
            commentText.fontSize = 12;
        }
        else
        {
            cardRt.anchoredPosition = new Vector2(-24f, -112f);
            cardRt.sizeDelta = new Vector2(370f, 154f);
            portraitRt.anchoredPosition = new Vector2(-292f, -77f);
            portraitRt.sizeDelta = new Vector2(116f, 116f);
            nameRt.anchoredPosition = new Vector2(-214f, -26f);
            nameRt.sizeDelta = new Vector2(190f, 26f);
            nameText.fontSize = 14;
            commentRt.anchoredPosition = new Vector2(-214f, -56f);
            commentRt.sizeDelta = new Vector2(190f, 82f);
            commentText.fontSize = 14;
        }
    }

    void RefreshPatron()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || !gm.GiftSelected) return;
        nameText.text = PatronName(gm.SelectedGift);
        portrait.sprite = PatronPortrait(gm.SelectedGift);
    }

    void Speak(string text, float seconds)
    {
        currentComment = text;
        commentText.text = text;
        commentUntil = Time.unscaledTime + seconds;
        RuntimeFileLogger.Event("PATRON_COMMENT", text);
    }

    string PatronName(DivineGiftType patron)
    {
        switch (patron)
        {
            case DivineGiftType.Ares: return L("ARES • PATRON", "АРЕС • ПОКРОВИТЕЛЬ");
            case DivineGiftType.Athena: return L("ATHENA • PATRON", "АФИНА • ПОКРОВИТЕЛЬ");
            case DivineGiftType.Apollo: return L("APOLLO • PATRON", "АПОЛЛОН • ПОКРОВИТЕЛЬ");
            case DivineGiftType.Poseidon: return L("POSEIDON • PATRON", "ПОСЕЙДОН • ПОКРОВИТЕЛЬ");
            default: return L("PATRON GOD", "БОГ-ПОКРОВИТЕЛЬ");
        }
    }

    string IntroLine(DivineGiftType patron)
    {
        switch (patron)
        {
            case DivineGiftType.Ares: return L("Good. Let them come. Build fast — then break them.", "Хорошо. Пусть идут. Строй быстро — потом ломай их.");
            case DivineGiftType.Athena: return L("Thirty seconds. Study the roads before the Greeks reach them.", "Тридцать секунд. Изучи дороги, прежде чем греки выйдут на них.");
            case DivineGiftType.Apollo: return L("The shore is quiet for now. Spend your gold with purpose.", "Берег пока тих. Трать золото с умом.");
            case DivineGiftType.Poseidon: return L("I can slow their march. You still have to stop it.", "Я замедлю их марш. Остановить его всё равно придётся тебе.");
            default: return L("Prepare Troy.", "Готовь Трою.");
        }
    }

    string EncounterLine(DivineGiftType patron, int encounter)
    {
        if (encounter <= 1)
        {
            switch (patron)
            {
                case DivineGiftType.Ares: return L("First blood. Hold nothing back.", "Первая кровь. Не сдерживайся.");
                case DivineGiftType.Athena: return L("First assault. Watch which road bends first.", "Первый штурм. Смотри, какая дорога дрогнет первой.");
                case DivineGiftType.Apollo: return L("The Greeks have landed. Let the first line earn its place.", "Греки высадились. Пусть первая линия докажет свою ценность.");
                case DivineGiftType.Poseidon: return L("They leave my sea and enter your kill zone.", "Они покинули моё море и вошли в твою зону боя.");
            }
        }

        return L($"Encounter {encounter}. They adapt — so must you.", $"Бой {encounter}. Они меняются — меняйся и ты.");
    }

    string GateHitLine(DivineGiftType patron)
    {
        switch (patron)
        {
            case DivineGiftType.Ares: return L("They touched the gate. Make them regret it.", "Они добрались до ворот. Заставь их пожалеть.");
            case DivineGiftType.Athena: return L("The gate took damage. Reinforce the weakest route now.", "Ворота повреждены. Усиль слабейшую дорогу сейчас.");
            case DivineGiftType.Apollo: return L("Too close. Do not trade stone for time again.", "Слишком близко. Не меняй камень на время ещё раз.");
            case DivineGiftType.Poseidon: return L("Even slowed, they reached Troy. Tighten the defense.", "Даже замедленные, они дошли до Трои. Уплотни оборону.");
            default: return L("Protect the gate.", "Защити ворота.");
        }
    }

    string BossLine(DivineGiftType patron)
    {
        switch (patron)
        {
            case DivineGiftType.Ares: return L("Menelaus. Finally, someone worth striking.", "Менелай. Наконец-то тот, кого стоит ударить.");
            case DivineGiftType.Athena: return L("Menelaus strengthens his escort. Separate the commander from his army.", "Менелай усиливает свиту. Отдели командира от его армии.");
            case DivineGiftType.Apollo: return L("The king has entered the field. Burn down his advantage first.", "Царь вышел на поле. Сначала сожги его преимущество.");
            case DivineGiftType.Poseidon: return L("Menelaus survived the sea. See that he does not survive Troy.", "Менелай пережил море. Сделай так, чтобы он не пережил Трою.");
            default: return L("Menelaus approaches.", "Менелай приближается.");
        }
    }

    string VictoryLine(DivineGiftType patron)
    {
        switch (patron)
        {
            case DivineGiftType.Ares: return L("That was a battle. Remember the sound of it.", "Вот это была битва. Запомни её звук.");
            case DivineGiftType.Athena: return L("Menelaus is down. Troy survives because the defense held.", "Менелай повержен. Троя выстояла, потому что оборона выдержала.");
            case DivineGiftType.Apollo: return L("The field grows quiet. For today, Troy still shines.", "Поле стихает. Сегодня Троя всё ещё сияет.");
            case DivineGiftType.Poseidon: return L("The Greeks came from the sea. Send them back to it.", "Греки пришли с моря. Отправь их обратно.");
            default: return L("Troy stands.", "Троя стоит.");
        }
    }

    string AmbientLine(DivineGiftType patron, int encounter)
    {
        switch (patron)
        {
            case DivineGiftType.Ares: return L("Keep pressure on them.", "Дави на них.");
            case DivineGiftType.Athena: return L("Read the field, not the noise.", "Смотри на поле, а не на шум.");
            case DivineGiftType.Apollo: return L("A clean defense wastes nothing.", "Хорошая оборона ничего не тратит зря.");
            case DivineGiftType.Poseidon: return encounter == 0 ? L("The tide is turning toward Troy.", "Прилив идёт к Трое.") : L("Make every slowed step costly.", "Пусть каждый замедленный шаг дорого им стоит.");
            default: return L("Hold Troy.", "Держи Трою.");
        }
    }

    bool HasActiveBoss()
    {
        foreach (Enemy enemy in EnemyRegistry.All)
            if (enemy != null && enemy.Archetype == EnemyArchetype.Boss && enemy.Health > 0f) return true;
        return false;
    }

    bool IsMenuBlockingCombat()
    {
        if (menuCanvas == null) return false;
        string[] names = { "MainMenu", "LevelSelect", "Settings", "PauseMenu", "EndMenu", "PreMapPatronSelection" };
        for (int i = 0; i < names.Length; i++)
        {
            Transform t = menuCanvas.transform.Find(names[i]);
            if (t != null && t.gameObject.activeInHierarchy) return true;
        }
        return false;
    }

    static readonly System.Collections.Generic.Dictionary<DivineGiftType, Sprite> PortraitCache = new System.Collections.Generic.Dictionary<DivineGiftType, Sprite>();

    Sprite PatronPortrait(DivineGiftType patron)
    {
        if (PortraitCache.TryGetValue(patron, out Sprite cached)) return cached;

        Texture2D texture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        Color bg;
        Color accent;
        switch (patron)
        {
            case DivineGiftType.Ares: bg = new Color(.16f, .025f, .018f, 1f); accent = new Color(.88f, .18f, .06f, 1f); break;
            case DivineGiftType.Athena: bg = new Color(.055f, .075f, .11f, 1f); accent = new Color(.76f, .69f, .43f, 1f); break;
            case DivineGiftType.Apollo: bg = new Color(.14f, .075f, .015f, 1f); accent = new Color(1f, .68f, .12f, 1f); break;
            case DivineGiftType.Poseidon: bg = new Color(.02f, .085f, .12f, 1f); accent = new Color(.16f, .67f, .83f, 1f); break;
            default: bg = new Color(.08f, .05f, .025f, 1f); accent = new Color(.8f, .55f, .2f, 1f); break;
        }

        for (int y = 0; y < 128; y++)
        for (int x = 0; x < 128; x++)
        {
            float dx = x - 64f;
            float dy = y - 64f;
            float d = Mathf.Sqrt(dx * dx + dy * dy) / 90f;
            texture.SetPixel(x, y, Color.Lerp(bg * 1.25f, bg * .55f, Mathf.Clamp01(d)));
        }

        Circle(texture, 64, 59, 25, new Color(.72f, .46f, .30f, 1f));
        Rect(texture, 43, 79, 42, 35, accent * .72f);
        Rect(texture, 49, 55, 30, 7, accent * .45f);
        Rect(texture, 52, 57, 6, 4, Color.black * .7f);
        Rect(texture, 70, 57, 6, 4, Color.black * .7f);

        if (patron == DivineGiftType.Ares)
        {
            Rect(texture, 40, 30, 48, 13, accent);
            Rect(texture, 59, 14, 10, 24, accent);
            Rect(texture, 30, 91, 68, 8, accent * .8f);
        }
        else if (patron == DivineGiftType.Athena)
        {
            Rect(texture, 40, 31, 48, 11, accent);
            Rect(texture, 44, 22, 8, 19, accent);
            Rect(texture, 76, 22, 8, 19, accent);
            Rect(texture, 29, 87, 70, 6, accent * .8f);
        }
        else if (patron == DivineGiftType.Apollo)
        {
            Circle(texture, 64, 26, 15, accent);
            for (int i = 0; i < 8; i++)
            {
                float a = i * Mathf.PI / 4f;
                int x0 = Mathf.RoundToInt(64 + Mathf.Cos(a) * 20f);
                int y0 = Mathf.RoundToInt(26 + Mathf.Sin(a) * 20f);
                Circle(texture, x0, y0, 4, accent);
            }
            Rect(texture, 29, 91, 70, 5, accent * .8f);
        }
        else if (patron == DivineGiftType.Poseidon)
        {
            Rect(texture, 61, 12, 6, 33, accent);
            Rect(texture, 47, 15, 6, 19, accent);
            Rect(texture, 75, 15, 6, 19, accent);
            Rect(texture, 47, 15, 34, 5, accent);
            Rect(texture, 31, 91, 66, 6, accent * .8f);
        }

        Ring(texture, 64, 64, 61, 57, accent);
        texture.Apply();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(.5f, .5f));
        PortraitCache[patron] = sprite;
        return sprite;
    }

    void Circle(Texture2D t, int cx, int cy, int radius, Color color)
    {
        int r2 = radius * radius;
        for (int y = cy - radius; y <= cy + radius; y++)
        for (int x = cx - radius; x <= cx + radius; x++)
            if (x >= 0 && y >= 0 && x < t.width && y < t.height && (x - cx) * (x - cx) + (y - cy) * (y - cy) <= r2)
                t.SetPixel(x, y, color);
    }

    void Ring(Texture2D t, int cx, int cy, int outer, int inner, Color color)
    {
        int o2 = outer * outer;
        int i2 = inner * inner;
        for (int y = cy - outer; y <= cy + outer; y++)
        for (int x = cx - outer; x <= cx + outer; x++)
        {
            if (x < 0 || y < 0 || x >= t.width || y >= t.height) continue;
            int d2 = (x - cx) * (x - cx) + (y - cy) * (y - cy);
            if (d2 <= o2 && d2 >= i2) t.SetPixel(x, y, color);
        }
    }

    void Rect(Texture2D t, int x, int y, int w, int h, Color color)
    {
        for (int yy = Mathf.Max(0, y); yy < Mathf.Min(t.height, y + h); yy++)
        for (int xx = Mathf.Max(0, x); xx < Mathf.Min(t.width, x + w); xx++)
            t.SetPixel(xx, yy, color);
    }

    Image AddImage(Transform parent, string name, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.raycastTarget = false;
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1f, 1f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return image;
    }

    Text AddText(Transform parent, string value, Vector2 pos, Vector2 size, int fontSize, Color color, TextAnchor alignment, FontStyle style)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.fontStyle = style;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.raycastTarget = false;
        RectTransform rt = text.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1f, 1f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return text;
    }
}
