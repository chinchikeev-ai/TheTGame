using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MainMenuBackgroundOverride : MonoBehaviour
{
    const string BackgroundResource = "Menu/Main_screen";

    GameObject appliedMainMenu;
    GameMenuController controller;
    GameObject armyOverlay;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoStart()
    {
        if (FindFirstObjectByType<MainMenuBackgroundOverride>() == null)
            new GameObject("MainMenuPresenter").AddComponent<MainMenuBackgroundOverride>();
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        appliedMainMenu = null;
        controller = null;
        armyOverlay = null;
        RuntimeFileLogger.Event("MENU", $"Main-menu presenter rebound after scene load: {scene.name}");
    }

    void Update()
    {
        if (armyOverlay != null && armyOverlay.activeSelf && GameInput.PausePressed())
            armyOverlay.SetActive(false);
    }

    void LateUpdate()
    {
        if (appliedMainMenu != null) return;

        controller = FindFirstObjectByType<GameMenuController>();
        if (controller == null) return;

        GameObject canvasObject = GameObject.Find("MenuCanvas");
        if (canvasObject == null) return;

        Transform mainMenu = canvasObject.transform.Find("MainMenu");
        if (mainMenu == null) return;

        BuildProductionMenu(mainMenu);
        appliedMainMenu = mainMenu.gameObject;
        RuntimeFileLogger.Event("MENU", "Applied clean production main-menu composition");
    }

    void BuildProductionMenu(Transform mainMenu)
    {
        for (int i = 0; i < mainMenu.childCount; i++)
            mainMenu.GetChild(i).gameObject.SetActive(false);

        GameObject root = new GameObject("ProductionMainMenu");
        root.transform.SetParent(mainMenu, false);
        Stretch(root.AddComponent<RectTransform>());

        BuildBackground(root.transform);
        BuildRightReadabilityVeil(root.transform);

        CreateMenuButton(root.transform, "PLAY", GameLanguage.T("PLAY", "ИГРАТЬ"), new Vector2(470f, 118f), new Vector2(540f, 132f), true, () => InvokeController("ShowLevels"));
        CreateMenuButton(root.transform, "ARMY", GameLanguage.T("ARMY", "АРМИЯ"), new Vector2(470f, -30f), new Vector2(470f, 92f), false, ShowArmy);
        CreateMenuButton(root.transform, "SETTINGS", GameLanguage.T("SETTINGS", "НАСТРОЙКИ"), new Vector2(470f, -146f), new Vector2(470f, 92f), false, () => InvokeController("ShowSettingsFromMain"));
        CreateMenuButton(root.transform, "EXIT", GameLanguage.T("EXIT", "ВЫХОД"), new Vector2(470f, -262f), new Vector2(470f, 92f), false, () => InvokeController("QuitGame"));

        BuildArmyOverlay(root.transform);
    }

    void BuildBackground(Transform parent)
    {
        GameObject backgroundObject = new GameObject("CleanIllustratedBackground");
        backgroundObject.transform.SetParent(parent, false);
        Image background = backgroundObject.AddComponent<Image>();
        background.raycastTarget = false;
        Stretch(background.rectTransform);

        Texture2D texture = Resources.Load<Texture2D>(BackgroundResource);
        if (texture == null)
        {
            background.color = new Color(.10f, .06f, .03f, 1f);
            RuntimeFileLogger.Event("MENU", "Main menu background is missing: " + BackgroundResource);
            return;
        }

        background.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(.5f, .5f), 100f);
        background.type = Image.Type.Simple;
        background.preserveAspect = false;
        background.color = Color.white;
    }

    static void BuildRightReadabilityVeil(Transform parent)
    {
        GameObject veilObject = new GameObject("MenuReadabilityVeil");
        veilObject.transform.SetParent(parent, false);
        Image veil = veilObject.AddComponent<Image>();
        veil.color = new Color(.025f, .012f, .008f, .24f);
        veil.raycastTarget = false;

        RectTransform rect = veil.rectTransform;
        rect.anchorMin = new Vector2(.56f, 0f);
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    void CreateMenuButton(Transform parent, string objectName, string label, Vector2 position, Vector2 size, bool primary, UnityAction action)
    {
        GameObject root = new GameObject(objectName + "_Button");
        root.transform.SetParent(parent, false);

        Image image = root.AddComponent<Image>();
        image.color = primary ? new Color(.69f, .12f, .035f, .97f) : new Color(.22f, .085f, .025f, .95f);

        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Outline outline = root.AddComponent<Outline>();
        outline.effectColor = primary ? new Color(1f, .66f, .16f, .95f) : new Color(.78f, .45f, .16f, .88f);
        outline.effectDistance = new Vector2(3f, -3f);

        Shadow shadow = root.AddComponent<Shadow>();
        shadow.effectColor = new Color(.02f, .006f, .002f, .82f);
        shadow.effectDistance = new Vector2(0f, -7f);

        Button button = root.AddComponent<Button>();
        button.targetGraphic = image;
        button.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.08f, 1.04f, .91f, 1f);
        colors.pressedColor = new Color(.78f, .70f, .58f, 1f);
        colors.selectedColor = new Color(1.03f, .98f, .84f, 1f);
        colors.fadeDuration = .08f;
        button.colors = colors;
        button.onClick.AddListener(action);

        AddButtonText(root.transform, label, new Vector2(-6f, 0f), primary ? 36 : 28, true);
        AddButtonText(root.transform, "›", new Vector2(size.x * .40f, 1f), primary ? 46 : 38, true, new Vector2(48f, size.y));

        GameObject accent = new GameObject("LeftAccent");
        accent.transform.SetParent(root.transform, false);
        Image accentImage = accent.AddComponent<Image>();
        accentImage.color = primary ? new Color(1f, .67f, .18f, .92f) : new Color(.86f, .52f, .18f, .80f);
        accentImage.raycastTarget = false;
        RectTransform accentRect = accentImage.rectTransform;
        accentRect.anchorMin = accentRect.anchorMax = accentRect.pivot = new Vector2(.5f, .5f);
        accentRect.anchoredPosition = new Vector2(-size.x * .42f, 0f);
        accentRect.sizeDelta = new Vector2(6f, size.y * .62f);
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
        panelRect.sizeDelta = new Vector2(760f, 360f);

        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(.93f, .52f, .14f, .90f);
        outline.effectDistance = new Vector2(3f, -3f);

        AddOverlayText(panel.transform, GameLanguage.T("ARMY", "АРМИЯ"), new Vector2(0f, 90f), 42, FontStyle.Bold, new Color(1f, .68f, .18f, 1f));
        AddOverlayText(panel.transform, GameLanguage.T("Army management is prepared for the next production pass.", "Управление армией будет подключено на следующем этапе разработки."), new Vector2(0f, 18f), 19, FontStyle.Normal, new Color(.91f, .82f, .70f, 1f), new Vector2(620f, 92f));

        GameObject back = new GameObject("BackButton");
        back.transform.SetParent(panel.transform, false);
        Image backImage = back.AddComponent<Image>();
        backImage.color = new Color(.62f, .15f, .04f, 1f);
        RectTransform backRect = backImage.rectTransform;
        backRect.anchorMin = backRect.anchorMax = backRect.pivot = new Vector2(.5f, .5f);
        backRect.anchoredPosition = new Vector2(0f, -105f);
        backRect.sizeDelta = new Vector2(280f, 62f);
        Button button = back.AddComponent<Button>();
        button.targetGraphic = backImage;
        button.onClick.AddListener(() => armyOverlay.SetActive(false));
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
        armyOverlay.SetActive(true);
        RuntimeFileLogger.Event("MENU", "Opened Army placeholder from production main menu");
    }

    void InvokeController(string methodName)
    {
        if (controller == null) controller = FindFirstObjectByType<GameMenuController>();
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

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
