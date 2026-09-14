using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public sealed class PreMapPatronSelectionPresentation : MonoBehaviour
{
    GameMenuController menu;
    Canvas menuCanvas;
    GameObject overlay;
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
        if (gm == null || gm.GameEnded || gm.GiftSelected) return;

        // Safety net for restart/legacy flows that try to enter combat without a patron.
        // The choice still happens before the run can actually begin because GameManager rejects BeginRun without it.
        if (Time.timeScale > 0f)
            ShowChoice();
    }

    bool TryBind()
    {
        menu = FindFirstObjectByType<GameMenuController>();
        GameObject canvasObject = GameObject.Find("MenuCanvas");
        menuCanvas = canvasObject != null ? canvasObject.GetComponent<Canvas>() : null;
        if (menu == null || menuCanvas == null) return false;

        Transform levelSelect = menuCanvas.transform.Find("LevelSelect");
        if (levelSelect == null) return false;

        if (overlay == null) BuildOverlay();
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
                chapterOneButton.onClick.AddListener(ShowChoice);
                break;
            }
        }

        return overlay != null && chapterOneButton != null;
    }

    void BuildOverlay()
    {
        overlay = new GameObject("PreMapPatronSelection");
        overlay.transform.SetParent(menuCanvas.transform, false);
        Image backdrop = overlay.AddComponent<Image>();
        backdrop.color = new Color(.015f, .009f, .006f, .985f);
        RectTransform br = backdrop.rectTransform;
        br.anchorMin = Vector2.zero;
        br.anchorMax = Vector2.one;
        br.offsetMin = br.offsetMax = Vector2.zero;

        GameObject panel = MakePanel(overlay.transform, new Vector2(980f, 720f));
        MakeText(panel.transform, GameLanguage.T("CHOOSE A PATRON GOD", "ВЫБЕРИТЕ БОГА-ПОКРОВИТЕЛЯ"), new Vector2(0, 286), new Vector2(820, 62), 36, true, new Color(1f, .72f, .26f, 1f));
        MakeText(panel.transform,
            GameLanguage.T("Choose before the map begins. One patron stays with Troy for the whole battle.", "Выбор делается ДО начала карты. Один покровитель помогает Трое всю битву."),
            new Vector2(0, 224), new Vector2(800, 54), 17, false, new Color(.88f, .78f, .66f, 1f));

        MakeGodButton(panel.transform, new Vector2(-225, 102),
            GameLanguage.T("ARES", "АРЕС"),
            GameLanguage.T("HECTOR & TOWERS +10% DAMAGE\nWHOLE MAP", "ГЕКТОР И БАШНИ +10% УРОНА\nВСЮ КАРТУ"),
            DivineGiftType.Ares);
        MakeGodButton(panel.transform, new Vector2(225, 102),
            GameLanguage.T("ATHENA", "АФИНА"),
            GameLanguage.T("GATE +2 MAX HP\nIMMEDIATE", "ВОРОТА +2 МАКС. HP\nСРАЗУ"),
            DivineGiftType.Athena);
        MakeGodButton(panel.transform, new Vector2(-225, -72),
            GameLanguage.T("APOLLO", "АПОЛЛОН"),
            GameLanguage.T("+50 STARTING GOLD\nONE TIME", "+50 СТАРТОВОГО ЗОЛОТА\nОДИН РАЗ"),
            DivineGiftType.Apollo);
        MakeGodButton(panel.transform, new Vector2(225, -72),
            GameLanguage.T("POSEIDON", "ПОСЕЙДОН"),
            GameLanguage.T("ENEMIES -10% SPEED\nWHOLE MAP", "ВРАГИ -10% СКОРОСТИ\nВСЮ КАРТУ"),
            DivineGiftType.Poseidon);

        MakeButton(panel.transform, GameLanguage.T("BACK", "НАЗАД"), new Vector2(0, -270), new Vector2(240, 52), HideChoice);
        overlay.SetActive(false);
    }

    void ShowChoice()
    {
        GameManager gm = GameManager.Instance;
        if (gm != null && gm.GiftSelected)
        {
            StartMap();
            return;
        }
        Time.timeScale = 0f;
        if (overlay != null)
        {
            overlay.transform.SetAsLastSibling();
            overlay.SetActive(true);
        }
        RuntimeFileLogger.Event("PATRON", "Pre-map patron selection opened.");
    }

    void HideChoice()
    {
        if (overlay != null) overlay.SetActive(false);
    }

    void Choose(DivineGiftType gift)
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || !gm.UseGift(gift)) return;
        RuntimeFileLogger.Event("PATRON", $"Pre-map patron confirmed: {gift}");
        HideChoice();
        StartMap();
    }

    void StartMap()
    {
        if (menu == null) return;
        MethodInfo startLevel = typeof(GameMenuController).GetMethod("StartLevel", BindingFlags.Instance | BindingFlags.NonPublic);
        if (startLevel == null)
        {
            RuntimeFileLogger.Event("PATRON", "Could not locate GameMenuController.StartLevel; map start aborted.");
            ShowChoice();
            return;
        }
        startLevel.Invoke(menu, null);
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

    GameObject MakePanel(Transform parent, Vector2 size)
    {
        GameObject go = new GameObject("PatronCard");
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
        GameObject go = new GameObject("PatronButton");
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
