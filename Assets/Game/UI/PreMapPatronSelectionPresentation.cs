using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public sealed class PreMapPatronSelectionPresentation : MonoBehaviour
{
    GameMenuController menu;
    Canvas menuCanvas;
    GameObject difficultyOverlay;
    GameObject patronOverlay;
    Button chapterOneButton;
    bool bound;

    IEnumerator Start()
    {
        while (!bound)
        {
            bound = TryBind();
            if (!bound) yield return null;
        }
    }

    void Update()
    {
        if (!bound || menu == null || menuCanvas == null)
        {
            bound = TryBind();
            return;
        }

        GameManager gm = GameManager.Instance;
        if (GameMenuController.QuitRequested)
        {
            HideAll();
            return;
        }
        if (gm == null || gm.GameEnded || gm.GiftSelected) return;

        // Safety net: any legacy/restart flow attempting to enter combat without a patron
        // is routed back through the full pre-map preparation flow.
        if (Time.timeScale > 0f)
            ShowDifficulty();
    }

    bool TryBind()
    {
        menu = FindFirstObjectByType<GameMenuController>();
        GameObject canvasObject = GameObject.Find("MenuCanvas");
        menuCanvas = canvasObject != null ? canvasObject.GetComponent<Canvas>() : null;
        if (menu == null || menuCanvas == null) return false;

        Transform levelSelect = menuCanvas.transform.Find("LevelSelect");
        if (levelSelect == null) return false;

        if (difficultyOverlay == null) BuildDifficultyOverlay();
        if (patronOverlay == null) BuildPatronOverlay();

        if (chapterOneButton == null)
        {
            Button[] buttons = levelSelect.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < buttons.Length; i++)
            {
                Text label = buttons[i].GetComponentInChildren<Text>(true);
                if (label == null) continue;
                if (!label.text.Contains("THE LANDING") && !label.text.Contains("ВЫСАДКА")) continue;
                chapterOneButton = buttons[i];
                chapterOneButton.onClick.RemoveAllListeners();
                chapterOneButton.onClick.AddListener(ShowDifficulty);
                break;
            }
        }

        return difficultyOverlay != null && patronOverlay != null && chapterOneButton != null;
    }

    void BuildDifficultyOverlay()
    {
        difficultyOverlay = MakeOverlay("PreMapDifficultySelection");
        GameObject panel = MakePanel(difficultyOverlay.transform, "DifficultyCard", new Vector2(980f, 720f));

        MakeText(panel.transform,
            GameLanguage.T("CHOOSE DIFFICULTY", "ВЫБЕРИТЕ СЛОЖНОСТЬ"),
            new Vector2(0f, 280f), new Vector2(820f, 62f), 38, true, new Color(1f, .72f, .26f, 1f));
        MakeText(panel.transform,
            GameLanguage.T("Choose the battle rules before selecting a patron god.", "Сначала выберите правила боя, затем бога-покровителя."),
            new Vector2(0f, 225f), new Vector2(790f, 46f), 17, false, new Color(.88f, .78f, .66f, 1f));

        MakeDifficultyButton(panel.transform, new Vector2(0f, 105f),
            GameLanguage.T("STORY", "ИСТОРИЯ"),
            GameLanguage.T("More forgiving defense • 190 starting gold", "Более мягкая оборона • 190 стартового золота"),
            CampaignDifficulty.Story);
        MakeDifficultyButton(panel.transform, new Vector2(0f, 0f),
            GameLanguage.T("STRATEGOS", "СТРАТЕГ"),
            GameLanguage.T("Standard campaign pressure • 150 starting gold", "Стандартное давление кампании • 150 стартового золота"),
            CampaignDifficulty.Strategos);
        MakeDifficultyButton(panel.transform, new Vector2(0f, -105f),
            GameLanguage.T("LEGENDARY", "ЛЕГЕНДА"),
            GameLanguage.T("Hardest pressure and economy • 120 starting gold", "Самое высокое давление и жёсткая экономика • 120 стартового золота"),
            CampaignDifficulty.Legendary);

        MakeButton(panel.transform, GameLanguage.T("BACK", "НАЗАД"), new Vector2(0f, -275f), new Vector2(240f, 52f), HideDifficulty);
        difficultyOverlay.SetActive(false);
    }

    void BuildPatronOverlay()
    {
        patronOverlay = MakeOverlay("PreMapPatronSelection");
        GameObject panel = MakePanel(patronOverlay.transform, "PatronCard", new Vector2(980f, 720f));

        MakeText(panel.transform,
            GameLanguage.T("CHOOSE A PATRON GOD", "ВЫБЕРИТЕ БОГА-ПОКРОВИТЕЛЯ"),
            new Vector2(0, 286), new Vector2(820, 62), 36, true, new Color(1f, .72f, .26f, 1f));
        MakeText(panel.transform,
            GameLanguage.T("One patron stays with Troy for the whole battle.", "Один покровитель помогает Трое всю битву."),
            new Vector2(0, 232), new Vector2(800, 46), 17, false, new Color(.88f, .78f, .66f, 1f));
        MakeText(panel.transform,
            GameLanguage.T("Difficulty: ", "Сложность: ") + DifficultyRules.Label(CampaignController.Instance != null ? CampaignController.Instance.Difficulty : CampaignDifficulty.Story),
            new Vector2(0, 194), new Vector2(800, 38), 15, true, new Color(.75f, .62f, .48f, 1f));

        MakeGodButton(panel.transform, new Vector2(-225, 88),
            GameLanguage.T("ARES", "АРЕС"),
            GameLanguage.T("HECTOR & TOWERS +10% DAMAGE\nWHOLE MAP", "ГЕКТОР И БАШНИ +10% УРОНА\nВСЮ КАРТУ"),
            DivineGiftType.Ares);
        MakeGodButton(panel.transform, new Vector2(225, 88),
            GameLanguage.T("ATHENA", "АФИНА"),
            GameLanguage.T("GATE +2 MAX HP\nIMMEDIATE", "ВОРОТА +2 МАКС. HP\nСРАЗУ"),
            DivineGiftType.Athena);
        MakeGodButton(panel.transform, new Vector2(-225, -86),
            GameLanguage.T("APOLLO", "АПОЛЛОН"),
            GameLanguage.T("+50 STARTING GOLD\nONE TIME", "+50 СТАРТОВОГО ЗОЛОТА\nОДИН РАЗ"),
            DivineGiftType.Apollo);
        MakeGodButton(panel.transform, new Vector2(225, -86),
            GameLanguage.T("POSEIDON", "ПОСЕЙДОН"),
            GameLanguage.T("ENEMIES -10% SPEED\nWHOLE MAP", "ВРАГИ -10% СКОРОСТИ\nВСЮ КАРТУ"),
            DivineGiftType.Poseidon);

        MakeButton(panel.transform, GameLanguage.T("BACK", "НАЗАД"), new Vector2(0, -270), new Vector2(240, 52), BackToDifficulty);
        patronOverlay.SetActive(false);
    }

    GameObject MakeOverlay(string name)
    {
        GameObject overlay = new GameObject(name);
        overlay.transform.SetParent(menuCanvas.transform, false);
        Image backdrop = overlay.AddComponent<Image>();
        backdrop.color = new Color(.015f, .009f, .006f, .985f);
        RectTransform rect = backdrop.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        return overlay;
    }

    void ShowDifficulty()
    {
        if (GameMenuController.QuitRequested) return;
        GameManager gm = GameManager.Instance;
        if (gm != null && gm.GiftSelected)
        {
            StartMap();
            return;
        }

        Time.timeScale = 0f;
        if (patronOverlay != null) patronOverlay.SetActive(false);
        if (difficultyOverlay != null)
        {
            difficultyOverlay.transform.SetAsLastSibling();
            difficultyOverlay.SetActive(true);
        }
        RuntimeFileLogger.Event("MENU", "Pre-map difficulty selection opened");
    }

    void HideDifficulty()
    {
        if (difficultyOverlay != null) difficultyOverlay.SetActive(false);
    }

    void SelectDifficulty(CampaignDifficulty difficulty)
    {
        CampaignController.Instance?.SetDifficulty(difficulty);
        RuntimeFileLogger.Event("MENU", $"Pre-map difficulty selected: {difficulty}");
        HideDifficulty();
        ShowPatron();
    }

    void ShowPatron()
    {
        if (GameMenuController.QuitRequested) return;
        GameManager gm = GameManager.Instance;
        if (gm != null && gm.GiftSelected)
        {
            StartMap();
            return;
        }

        Time.timeScale = 0f;
        if (difficultyOverlay != null) difficultyOverlay.SetActive(false);
        if (patronOverlay != null)
        {
            patronOverlay.transform.SetAsLastSibling();
            patronOverlay.SetActive(true);
        }
        RuntimeFileLogger.Event("PATRON", "Pre-map patron selection opened");
    }

    void BackToDifficulty()
    {
        if (patronOverlay != null) patronOverlay.SetActive(false);
        ShowDifficulty();
    }

    void Choose(DivineGiftType gift)
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || !gm.UseGift(gift)) return;
        RuntimeFileLogger.Event("PATRON", $"Pre-map patron confirmed: {gift}");
        HideAll();
        StartMap();
    }

    void HideAll()
    {
        if (difficultyOverlay != null) difficultyOverlay.SetActive(false);
        if (patronOverlay != null) patronOverlay.SetActive(false);
    }

    void StartMap()
    {
        if (menu == null) return;
        MethodInfo startLevel = typeof(GameMenuController).GetMethod("StartLevel", BindingFlags.Instance | BindingFlags.NonPublic);
        if (startLevel == null)
        {
            RuntimeFileLogger.Event("PATRON", "Could not locate GameMenuController.StartLevel; map start aborted.");
            ShowDifficulty();
            return;
        }
        startLevel.Invoke(menu, null);
    }

    void MakeDifficultyButton(Transform parent, Vector2 pos, string title, string description, CampaignDifficulty difficulty)
    {
        Button button = MakeButton(parent, title + "\n" + description, pos, new Vector2(620f, 84f), () => SelectDifficulty(difficulty));
        Text label = button.GetComponentInChildren<Text>(true);
        if (label != null)
        {
            label.fontSize = 17;
            label.lineSpacing = 1.15f;
        }
    }

    void MakeGodButton(Transform parent, Vector2 pos, string title, string description, DivineGiftType gift)
    {
        Button button = MakeButton(parent, title + "\n" + description, pos, new Vector2(410, 140), () => Choose(gift));
        Text label = button.GetComponentInChildren<Text>(true);
        if (label != null)
        {
            label.fontSize = 16;
            label.lineSpacing = 1.15f;
        }
    }

    GameObject MakePanel(Transform parent, string name, Vector2 size)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = new Color(.07f, .04f, .022f, .99f);
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = size;
        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(.78f, .40f, .12f, .7f);
        outline.effectDistance = new Vector2(2f, -2f);
        return go;
    }

    Button MakeButton(Transform parent, string label, Vector2 pos, Vector2 size, UnityEngine.Events.UnityAction action)
    {
        GameObject go = new GameObject("PreMapButton");
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = new Color(.28f, .13f, .055f, .98f);
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        go.AddComponent<MenuButtonFeedback>();
        MakeText(go.transform, label, Vector2.zero, size - new Vector2(20, 12), 16, true, new Color(1f, .86f, .65f, 1f));
        return button;
    }

    Text MakeText(Transform parent, string value, Vector2 pos, Vector2 size, int fontSize, bool bold, Color color)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
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
