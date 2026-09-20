using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public sealed class MainMenuBackgroundOverride : MonoBehaviour
{
    GameObject appliedMainMenu;
    GameMenuController controller;
    GameObject armyOverlay;
    public void Initialize(GameMenuController owner)
    {
        controller = owner;
        GameObject main = owner != null ? owner.MainMenuRoot : null;
        if (main == null || appliedMainMenu == main) return;

        BuildProductionMenu(main.transform);
        appliedMainMenu = main;
        RuntimeFileLogger.Event("MENU", "Applied clean production main-menu composition");
    }

    void Update()
    {
        if (armyOverlay != null && armyOverlay.activeSelf && GameInput.PausePressed())
            HideArmy();
    }

    void BuildProductionMenu(Transform mainMenu)
    {
        for (int i = 0; i < mainMenu.childCount; i++)
            mainMenu.GetChild(i).gameObject.SetActive(false);

        GameObject root = new GameObject("ProductionMainMenu");
        root.transform.SetParent(mainMenu, false);
        Stretch(root.AddComponent<RectTransform>());

        var artwork = root.AddComponent<MainMenuArtwork>();
        artwork.Build(() => InvokeController("ShowLevels"), ShowArmy,
            () => InvokeController("ShowSettingsFromMain"), () => InvokeController("QuitGame"));
        BuildArmyOverlay(artwork.Content);
    }
    static Text AddButtonText(Transform parent, string value, Vector2 position, int fontSize, bool bold, Vector2? size = null)
    {
        GameObject go = new GameObject("Label");
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(1f, .91f, .72f, 1f);
        text.raycastTarget = false;

        RectTransform rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size ?? new Vector2(360f, 70f);
        return text;
    }

    void BuildArmyOverlay(Transform parent)
    {
        armyOverlay = new GameObject("ArmyOverlay");
        armyOverlay.transform.SetParent(parent, false);

        Image blocker = armyOverlay.AddComponent<Image>();
        blocker.color = new Color(.018f, .008f, .004f, .90f);
        blocker.raycastTarget = true;
        Stretch(blocker.rectTransform);

        GameObject panel = new GameObject("ArmyCard");
        panel.transform.SetParent(armyOverlay.transform, false);
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(.10f, .045f, .018f, .98f);
        RectTransform panelRect = panelImage.rectTransform;
        panelRect.anchorMin = panelRect.anchorMax = panelRect.pivot = new Vector2(.5f, .5f);
        panelRect.sizeDelta = new Vector2(760f, 500f);

        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(.93f, .52f, .14f, .90f);
        outline.effectDistance = new Vector2(3f, -3f);

        AddOverlayText(panel.transform, GameLanguage.T("HECTOR - PRINCE OF TROY", "ГЕКТОР - ПРИНЦ ТРОИ"), new Vector2(0f, 170f), 32, FontStyle.Bold, new Color(1f, .68f, .18f, 1f));
        AddOverlayText(panel.transform, GameLanguage.T(
            "Defender of Troy. Leads the front line and attacks nearby enemies.\n\nWAR CRY  /  SHIELD WALL\nSPEAR THROW  /  ULTIMATE",
            "Защитник Трои. Сражается на передовой и атакует ближайших врагов.\n\nБОЕВОЙ КЛИЧ  /  СТЕНА ЩИТОВ\nБРОСОК КОПЬЯ  /  ГЛАВНАЯ СПОСОБНОСТЬ"),
            new Vector2(0f, 5f), 24, FontStyle.Normal, new Color(.91f, .82f, .70f, 1f), new Vector2(620f, 220f));

        GameObject back = new GameObject("BackButton");
        back.transform.SetParent(panel.transform, false);
        Image backImage = back.AddComponent<Image>();
        backImage.color = new Color(.62f, .15f, .04f, 1f);
        RectTransform backRect = backImage.rectTransform;
        backRect.anchorMin = backRect.anchorMax = backRect.pivot = new Vector2(.5f, .5f);
        backRect.anchoredPosition = new Vector2(0f, -175f);
        backRect.sizeDelta = new Vector2(280f, 62f);
        Button button = back.AddComponent<Button>();
        button.targetGraphic = backImage;
        button.onClick.AddListener(HideArmy);
        AddButtonText(back.transform, GameLanguage.T("BACK", "НАЗАД"), Vector2.zero, 23, true, new Vector2(250f, 58f));

        armyOverlay.SetActive(false);
    }

    static Text AddOverlayText(Transform parent, string value, Vector2 position, int fontSize, FontStyle style, Color color, Vector2? size = null)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        RectTransform rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size ?? new Vector2(620f, 70f);
        return text;
    }

    void ShowArmy()
    {
        if (armyOverlay == null) return;
        SetMainActionsInteractable(false);
        armyOverlay.SetActive(true);
        armyOverlay.GetComponentInChildren<Button>().Select();
        RuntimeFileLogger.Event("MENU", "Opened Hector information from illustrated main menu");
    }

    void HideArmy()
    {
        if (armyOverlay == null) return;
        armyOverlay.SetActive(false);
        SetMainActionsInteractable(true);
        armyOverlay.transform.parent.Find("HEROES_Button").GetComponent<Button>().Select();
    }

    void SetMainActionsInteractable(bool interactable)
    {
        foreach (Transform child in armyOverlay.transform.parent)
        {
            var button = child.GetComponent<Button>();
            if (button != null) button.interactable = interactable;
        }
    }

    void InvokeController(string methodName)
    {
        if (controller == null) controller = GameMenuController.Instance;
        if (controller == null) return;

        MethodInfo method = typeof(GameMenuController).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (method == null)
        {
            RuntimeFileLogger.Event("MENU", "Menu action not found: " + methodName);
            return;
        }
        method.Invoke(controller, null);
    }

    static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
