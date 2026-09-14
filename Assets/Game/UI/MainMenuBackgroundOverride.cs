using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class MainMenuBackgroundOverride : MonoBehaviour
{
    const string BackgroundResource = "Menu/Main_screen";

    GameObject appliedMainMenu;
    GameMenuController controller;
    GameObject toast;
    Text toastText;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoStart()
    {
        if (FindFirstObjectByType<MainMenuBackgroundOverride>() == null)
            new GameObject("ApprovedMainMenuPresenter").AddComponent<MainMenuBackgroundOverride>();
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

        BuildApprovedLayout(mainMenu);
        appliedMainMenu = mainMenu.gameObject;
        RuntimeFileLogger.Event("MENU", "Applied approved main-menu layout stage");
    }

    void BuildApprovedLayout(Transform mainMenu)
    {
        for (int i = 0; i < mainMenu.childCount; i++)
            mainMenu.GetChild(i).gameObject.SetActive(false);

        GameObject root = new GameObject("ApprovedMainMenu");
        root.transform.SetParent(mainMenu, false);
        Stretch(root.AddComponent<RectTransform>());

        BuildBackground(root.transform);
        BuildLogo(root.transform);
        BuildMenu(root.transform);
        BuildToast(root.transform);
    }

    void BuildBackground(Transform parent)
    {
        GameObject backgroundObject = new GameObject("Background");
        backgroundObject.transform.SetParent(parent, false);
        Image background = backgroundObject.AddComponent<Image>();
        background.raycastTarget = false;
        Stretch(background.rectTransform);

        Texture2D texture = Resources.Load<Texture2D>(BackgroundResource);
        if (texture != null)
        {
            background.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(.5f, .5f), 100f);
            background.type = Image.Type.Simple;
            background.preserveAspect = false;
            background.color = Color.white;
        }
        else
        {
            background.color = new Color(.12f, .08f, .04f, 1f);
        }

        GameObject shadeObject = new GameObject("RightShade");
        shadeObject.transform.SetParent(parent, false);
        Image shade = shadeObject.AddComponent<Image>();
        shade.color = new Color(.025f, .008f, .003f, .63f);
        shade.raycastTarget = false;
        RectTransform shadeRect = shade.rectTransform;
        shadeRect.anchorMin = new Vector2(.60f, 0f);
        shadeRect.anchorMax = Vector2.one;
        shadeRect.offsetMin = Vector2.zero;
        shadeRect.offsetMax = Vector2.zero;
    }

    void BuildLogo(Transform parent)
    {
        GameObject plaque = CreatePanel(parent, "LogoPlaque", new Vector2(-570f, 330f), new Vector2(650f, 245f),
            new Color(.15f, .05f, .015f, .95f), new Color(.96f, .55f, .13f, .95f));

        AddText(plaque.transform, "THE TROY GAME", new Vector2(0f, 38f), 58, new Color(1f, .70f, .18f, 1f));
        AddText(plaque.transform, "GODS DEFENSE", new Vector2(0f, -38f), 30, new Color(1f, .90f, .58f, 1f));
    }

    void BuildMenu(Transform parent)
    {
        Button play = CreateMenuButton(parent, "PLAY", new Vector2(485f, 255f), new Vector2(540f, 170f),
            new Color(.93f, .30f, .05f, 1f), new Color(1f, .67f, .16f, 1f), 56);
        play.onClick.AddListener(() => InvokeController("ShowLevels"));

        CreateSecondary(parent, "HEROES", new Vector2(485f, 72f));
        CreateSecondary(parent, "TOWERS", new Vector2(485f, -58f));
        CreateSecondary(parent, "UPGRADES", new Vector2(485f, -188f));
        CreateSecondary(parent, "SHOP", new Vector2(485f, -318f));

        Button settings = CreateMenuButton(parent, "SETTINGS", new Vector2(825f, 465f), new Vector2(175f, 76f),
            new Color(.18f, .07f, .025f, .98f), new Color(.95f, .55f, .13f, 1f), 19);
        settings.onClick.AddListener(() => InvokeController("ShowSettingsFromMain"));

        Button exit = CreateMenuButton(parent, "EXIT", new Vector2(825f, -470f), new Vector2(190f, 78f),
            new Color(.24f, .05f, .025f, .98f), new Color(.85f, .19f, .08f, 1f), 24);
        exit.onClick.AddListener(() => InvokeController("QuitGame"));
    }

    void CreateSecondary(Transform parent, string label, Vector2 position)
    {
        Button button = CreateMenuButton(parent, label, position, new Vector2(455f, 100f),
            new Color(.78f, .60f, .36f, 1f), new Color(.36f, .16f, .05f, 1f), 28);
        button.onClick.AddListener(() => ShowComingSoon(label));
    }

    Button CreateMenuButton(Transform parent, string label, Vector2 position, Vector2 size, Color face, Color border, int fontSize)
    {
        GameObject go = new GameObject(label);
        go.transform.SetParent(parent, false);

        Image image = go.AddComponent<Image>();
        image.color = face;

        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = border;
        outline.effectDistance = new Vector2(5f, -5f);

        Shadow shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color(.03f, .01f, .005f, .82f);
        shadow.effectDistance = new Vector2(0f, -8f);

        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, .94f, .78f, 1f);
        colors.pressedColor = new Color(.70f, .58f, .46f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.fadeDuration = .07f;
        button.colors = colors;

        go.AddComponent<MenuButtonFeedback>();

        Text labelText = AddText(go.transform, label, Vector2.zero, fontSize, LabelColor(face));
        RectTransform labelRect = labelText.rectTransform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        return button;
    }

    GameObject CreatePanel(Transform parent, string name, Vector2 position, Vector2 size, Color fill, Color border)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);

        Image image = go.AddComponent<Image>();
        image.color = fill;
        image.raycastTarget = false;

        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = border;
        outline.effectDistance = new Vector2(4f, -4f);
        return go;
    }

    Text AddText(Transform parent, string value, Vector2 position, int fontSize, Color color)
    {
        GameObject go = new GameObject(value + "_Text");
        go.transform.SetParent(parent, false);

        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;
        text.raycastTarget = false;

        RectTransform rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(600f, 90f);

        Shadow shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color(.05f, .015f, .005f, .88f);
        shadow.effectDistance = new Vector2(2f, -3f);
        return text;
    }

    void BuildToast(Transform parent)
    {
        toast = CreatePanel(parent, "ComingSoonToast", new Vector2(280f, -415f), new Vector2(520f, 64f),
            new Color(.10f, .035f, .012f, .96f), new Color(.94f, .53f, .13f, .85f));
        toastText = AddText(toast.transform, string.Empty, Vector2.zero, 19, new Color(1f, .88f, .56f, 1f));
        toast.SetActive(false);
    }

    void ShowComingSoon(string feature)
    {
        if (toast == null || toastText == null) return;
        toastText.text = GameLanguage.T(feature + " - IN DEVELOPMENT", feature + " - В РАЗРАБОТКЕ");
        toast.SetActive(true);
        CancelInvoke(nameof(HideToast));
        Invoke(nameof(HideToast), 1.5f);
    }

    void HideToast()
    {
        if (toast != null) toast.SetActive(false);
    }

    void InvokeController(string methodName)
    {
        if (controller == null) controller = FindFirstObjectByType<GameMenuController>();
        if (controller == null) return;

        MethodInfo method = typeof(GameMenuController).GetMethod(methodName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (method == null)
        {
            RuntimeFileLogger.Event("MENU", "Menu action not found: " + methodName);
            return;
        }
        method.Invoke(controller, null);
    }

    static Color LabelColor(Color background)
    {
        float luminance = background.r * .299f + background.g * .587f + background.b * .114f;
        return luminance > .55f ? new Color(.20f, .07f, .018f, 1f) : new Color(1f, .88f, .56f, 1f);
    }

    static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
