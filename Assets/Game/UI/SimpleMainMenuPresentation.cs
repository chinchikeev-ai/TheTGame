using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public sealed class SimpleMainMenuPresentation : MonoBehaviour
{
    readonly Button[] armyTabs = new Button[3];

    GameObject appliedRoot;
    GameObject armyOverlay;
    Text armyTitle;
    Text armyBody;
    GameMenuController controller;
    int activeArmyTab;
    public void Initialize(GameMenuController owner)
    {
        controller = owner;
        GameObject main = owner != null ? owner.MainMenuRoot : null;
        if (main == null) return;

        Transform approved = main.transform.Find("ApprovedMainMenu");
        if (approved == null || appliedRoot == approved.gameObject) return;

        Build(approved);
        appliedRoot = approved.gameObject;
        RuntimeFileLogger.Event("MENU", "Simplified main menu polished: Play / Army / Settings / Exit");
    }

    void Update()
    {
        if (armyOverlay != null && armyOverlay.activeSelf && GameInput.PausePressed())
            HideArmy();
    }

    void Build(Transform approved)
    {
        Transform old = approved.Find("SimpleMenuLayer");
        if (old != null) Destroy(old.gameObject);

        Button[] legacyButtons = approved.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < legacyButtons.Length; i++)
            legacyButtons[i].interactable = false;

        GameObject layer = new GameObject("SimpleMenuLayer");
        layer.transform.SetParent(approved, false);
        RectTransform layerRect = layer.AddComponent<RectTransform>();
        Stretch(layerRect);

        // The approved image contains the historical five-section navigation.
        // Keep the art, but visually and interactively replace that whole area with one clean hierarchy.
        MakeMask(layer.transform, new Vector2(525f, 0f), new Vector2(900f, 1080f), new Color(.025f, .011f, .005f, .95f));

        GameObject panel = MakePanel(
            layer.transform,
            "MainActions",
            new Vector2(465f, 0f),
            new Vector2(600f, 620f),
            new Color(.045f, .020f, .009f, .97f));

        // Product title is intentionally language-neutral branding.
        AddText(panel.transform, "THE TROY GAME", new Vector2(0f, 235f), new Vector2(540f, 72f), 44, FontStyle.Bold, new Color(1f, .66f, .16f, 1f));
        AddText(panel.transform, L("GODS DEFENSE", "ЗАЩИТА БОГОВ"), new Vector2(0f, 190f), new Vector2(500f, 38f), 18, FontStyle.Bold, new Color(.86f, .70f, .48f, 1f));
        MakeLine(panel.transform, new Vector2(0f, 153f), 455f);

        MakeButton(panel.transform, L("PLAY", "ИГРАТЬ"), new Vector2(0f, 80f), new Vector2(500f, 92f), () => InvokeController("ShowLevels"), true, 29);
        MakeButton(panel.transform, L("ARMY", "АРМИЯ"), new Vector2(0f, -30f), new Vector2(500f, 72f), ShowArmy, false, 23);

        MakeButton(panel.transform, L("SETTINGS", "НАСТРОЙКИ"), new Vector2(-130f, -142f), new Vector2(235f, 56f), () => InvokeController("ShowSettingsFromMain"), false, 17);
        MakeButton(panel.transform, L("EXIT", "ВЫХОД"), new Vector2(130f, -142f), new Vector2(235f, 56f), () => InvokeController("QuitGame"), false, 17);

        AddText(
            panel.transform,
            L("Defend Troy. Hold the road. Break the landing.", "Защити Трою. Удержи дорогу. Сорви высадку."),
            new Vector2(0f, -225f),
            new Vector2(510f, 52f),
            14,
            FontStyle.Normal,
            new Color(.70f, .60f, .49f, .88f));

        BuildArmyOverlay(layer.transform);
    }

    void BuildArmyOverlay(Transform parent)
    {
        armyOverlay = new GameObject("ArmyOverlay");
        armyOverlay.transform.SetParent(parent, false);
        Image backdrop = armyOverlay.AddComponent<Image>();
        backdrop.color = new Color(.018f, .009f, .004f, .985f);
        Stretch(backdrop.rectTransform);

        GameObject panel = MakePanel(
            armyOverlay.transform,
            "ArmyCard",
            Vector2.zero,
            new Vector2(1320f, 780f),
            new Color(.060f, .027f, .012f, .99f));

        AddText(panel.transform, L("ARMY", "АРМИЯ"), new Vector2(-525f, 315f), new Vector2(900f, 62f), 42, FontStyle.Bold, new Color(1f, .66f, .16f, 1f), TextAnchor.MiddleLeft);
        AddText(
            panel.transform,
            L("Troy's hero, defenders and known enemies.", "Герой Трои, защитники и известные враги."),
            new Vector2(-525f, 272f),
            new Vector2(900f, 36f),
            16,
            FontStyle.Normal,
            new Color(.83f, .72f, .60f, 1f),
            TextAnchor.MiddleLeft);
        MakeLine(panel.transform, new Vector2(0f, 238f), 1130f);

        GameObject navigation = MakePanel(
            panel.transform,
            "ArmyNavigation",
            new Vector2(-455f, -25f),
            new Vector2(300f, 485f),
            new Color(.040f, .021f, .012f, .96f));

        armyTabs[0] = MakeButton(navigation.transform, L("HECTOR", "ГЕКТОР"), new Vector2(0f, 150f), new Vector2(250f, 62f), () => ShowArmyTab(0), true, 19);
        armyTabs[1] = MakeButton(navigation.transform, L("DEFENDERS", "ЗАЩИТНИКИ"), new Vector2(0f, 72f), new Vector2(250f, 62f), () => ShowArmyTab(1), false, 18);
        armyTabs[2] = MakeButton(navigation.transform, L("ENEMIES", "ВРАГИ"), new Vector2(0f, -6f), new Vector2(250f, 62f), () => ShowArmyTab(2), false, 19);

        AddText(
            navigation.transform,
            L("Select a section", "Выберите раздел"),
            new Vector2(0f, -100f),
            new Vector2(240f, 34f),
            13,
            FontStyle.Normal,
            new Color(.64f, .55f, .47f, .86f));

        MakeButton(navigation.transform, L("BACK", "НАЗАД"), new Vector2(0f, -185f), new Vector2(210f, 52f), HideArmy, false, 16);

        GameObject content = MakePanel(
            panel.transform,
            "ArmyContent",
            new Vector2(165f, -25f),
            new Vector2(825f, 485f),
            new Color(.033f, .018f, .010f, .97f));

        armyTitle = AddText(content.transform, "", new Vector2(-350f, 172f), new Vector2(700f, 52f), 28, FontStyle.Bold, new Color(1f, .75f, .28f, 1f), TextAnchor.MiddleLeft);
        MakeLine(content.transform, new Vector2(0f, 135f), 700f);
        armyBody = AddText(content.transform, "", new Vector2(-350f, 86f), new Vector2(700f, 330f), 18, FontStyle.Normal, new Color(.90f, .82f, .72f, 1f), TextAnchor.UpperLeft);

        ShowArmyTab(0);
        armyOverlay.SetActive(false);
    }

    void ShowArmy()
    {
        if (armyOverlay == null) return;
        ShowArmyTab(activeArmyTab);
        armyOverlay.transform.SetAsLastSibling();
        armyOverlay.SetActive(true);
        RuntimeFileLogger.Event("MENU", "Opened Army screen");
    }

    void HideArmy()
    {
        if (armyOverlay == null) return;
        armyOverlay.SetActive(false);
        RuntimeFileLogger.Event("MENU", "Returned from Army screen");
    }

    void ShowArmyTab(int tab)
    {
        if (armyTitle == null || armyBody == null) return;
        activeArmyTab = Mathf.Clamp(tab, 0, 2);
        RefreshArmyTabVisuals();

        switch (activeArmyTab)
        {
            case 0:
                armyTitle.text = L("HECTOR • PRINCE OF TROY", "ГЕКТОР • ПРИНЦ ТРОИ");
                armyBody.text = L(
                    "FRONT-LINE HERO • MOBILE COMMANDER\n\nQ  WAR CRY\nE  SHIELD WALL\nR  SPEAR THROW\nF  ULTIMATE\n\nMove Hector with RMB along battlefield roads. He attacks nearby enemies automatically and returns to battle after being downed.",
                    "ГЕРОЙ ПЕРЕДОВОЙ • МОБИЛЬНЫЙ КОМАНДИР\n\nQ  БОЕВОЙ КЛИЧ\nE  СТЕНА ЩИТОВ\nR  БРОСОК КОПЬЯ\nF  ГЛАВНАЯ СПОСОБНОСТЬ\n\nПеремещайте Гектора ПКМ по дорогам поля боя. Он автоматически атакует ближайших врагов и возвращается в бой после падения.");
                break;

            case 1:
                armyTitle.text = L("DEFENDERS OF TROY", "ЗАЩИТНИКИ ТРОИ");
                armyBody.text = L(
                    "SHIELD GUARD      Blocker • Frontline\nTROJAN ARCHERS    Ranged damage • Anti-light\nSPEAR THROWERS    Armor pierce • Anti-heavy\nBALLISTA CREW     Heavy target • Anti-siege\nPRIESTS OF APOLLO Support • Control\nFIRE CREW         Area damage • Burn\n\nUse different defenders together: hold the road, control groups and focus heavy targets.",
                    "ЩИТОВАЯ ГВАРДИЯ   Блокировка • Передовая\nТРОЯНСКИЕ ЛУЧНИКИ Дальний бой • Против лёгких\nМЕТАТЕЛИ КОПИЙ    Бронебойный • Против тяжёлых\nРАСЧЁТ БАЛЛИСТЫ   Тяжёлые цели • Против осады\nЖРЕЦЫ АПОЛЛОНА    Поддержка • Контроль\nОГНЕННЫЙ РАСЧЁТ   Урон по площади • Горение\n\nСочетайте защитников: удерживайте дорогу, контролируйте группы и уничтожайте тяжёлые цели.");
                break;

            default:
                armyTitle.text = L("KNOWN ENEMIES", "ИЗВЕСТНЫЕ ВРАГИ");
                armyBody.text = L(
                    "GREEK INFANTRY\nRUNNERS\nHEAVY HOPLITES\nSHIELD BEARERS\nARCHERS\nBATTERING RAM\nMENELAUS\n\nClick an enemy during battle to inspect its current HP, speed, armor, arrow resistance, gate damage and reward.",
                    "ГРЕЧЕСКАЯ ПЕХОТА\nБЕГУНЫ\nТЯЖЁЛЫЕ ГОПЛИТЫ\nЩИТОНОСЦЫ\nЛУЧНИКИ\nТАРАН\nМЕНЕЛАЙ\n\nНажмите на врага во время боя, чтобы увидеть его здоровье, скорость, броню, защиту от стрел, урон воротам и награду.");
                break;
        }
    }

    void RefreshArmyTabVisuals()
    {
        for (int i = 0; i < armyTabs.Length; i++)
        {
            Button button = armyTabs[i];
            if (button == null) continue;
            Image image = button.GetComponent<Image>();
            if (image != null)
                image.color = i == activeArmyTab
                    ? new Color(.62f, .15f, .045f, .99f)
                    : new Color(.20f, .105f, .050f, .98f);

            Outline outline = button.GetComponent<Outline>();
            if (outline != null)
                outline.effectColor = i == activeArmyTab
                    ? new Color(1f, .66f, .16f, .90f)
                    : new Color(.58f, .32f, .13f, .70f);
        }
    }

    void InvokeController(string methodName)
    {
        if (controller == null) controller = GameMenuController.Instance;
        if (controller == null) return;

        MethodInfo method = typeof(GameMenuController).GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (method == null)
        {
            RuntimeFileLogger.Event("MENU", "Menu action not found: " + methodName);
            return;
        }
        method.Invoke(controller, null);
    }

    string L(string en, string ru) => GameLanguage.T(en, ru);

    GameObject MakeMask(Transform parent, Vector2 position, Vector2 size, Color color)
    {
        GameObject go = new GameObject("LegacyMenuMask");
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = true;
        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return go;
    }

    GameObject MakePanel(Transform parent, string name, Vector2 position, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = color;
        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(.90f, .46f, .11f, .65f);
        outline.effectDistance = new Vector2(2f, -2f);
        return go;
    }

    Button MakeButton(
        Transform parent,
        string label,
        Vector2 position,
        Vector2 size,
        UnityEngine.Events.UnityAction action,
        bool primary,
        int fontSize)
    {
        GameObject go = new GameObject(label + " Button");
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = primary ? new Color(.62f, .15f, .045f, .99f) : new Color(.20f, .105f, .050f, .98f);
        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = primary ? new Color(1f, .66f, .16f, .90f) : new Color(.58f, .32f, .13f, .70f);
        outline.effectDistance = new Vector2(2f, -2f);
        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        go.AddComponent<MenuButtonFeedback>();
        AddText(go.transform, label, Vector2.zero, size - new Vector2(18f, 10f), fontSize, FontStyle.Bold, new Color(1f, .88f, .69f, 1f));
        return button;
    }

    Text AddText(
        Transform parent,
        string value,
        Vector2 position,
        Vector2 size,
        int fontSize,
        FontStyle style,
        Color color,
        TextAnchor anchor = TextAnchor.MiddleCenter)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = anchor;
        text.color = color;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.raycastTarget = false;
        RectTransform rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return text;
    }

    void MakeLine(Transform parent, Vector2 position, float width)
    {
        GameObject line = new GameObject("Divider");
        line.transform.SetParent(parent, false);
        Image image = line.AddComponent<Image>();
        image.color = new Color(.95f, .48f, .11f, .42f);
        image.raycastTarget = false;
        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(width, 2f);
    }

    static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
