using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class MainMenuBackgroundOverride : MonoBehaviour
{
    const string ApprovedMenuResource = "Menu/ApprovedMainMenu";

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

        BuildApprovedMenu(mainMenu);
        appliedMainMenu = mainMenu.gameObject;
        RuntimeFileLogger.Event("MENU", "Applied approved main-menu reference art and live hotspots");
    }

    void BuildApprovedMenu(Transform mainMenu)
    {
        for (int i = 0; i < mainMenu.childCount; i++)
            mainMenu.GetChild(i).gameObject.SetActive(false);

        GameObject root = new GameObject("ApprovedMainMenu");
        root.transform.SetParent(mainMenu, false);
        Stretch(root.AddComponent<RectTransform>());

        BuildApprovedArt(root.transform);

        CreateHotspot(root.transform, "PLAY", new Vector2(437f, 286f), new Vector2(548f, 203f), () => InvokeController("ShowLevels"));
        CreateHotspot(root.transform, "HEROES", new Vector2(454f, 127f), new Vector2(457f, 116f), () => ShowComingSoon("HEROES"));
        CreateHotspot(root.transform, "TOWERS", new Vector2(454f, 8f), new Vector2(457f, 112f), () => ShowComingSoon("TOWERS"));
        CreateHotspot(root.transform, "UPGRADES", new Vector2(454f, -106f), new Vector2(457f, 110f), () => ShowComingSoon("UPGRADES"));
        CreateHotspot(root.transform, "SHOP", new Vector2(454f, -217f), new Vector2(457f, 106f), () => ShowComingSoon("SHOP"));
        CreateHotspot(root.transform, "SETTINGS", new Vector2(861f, 473f), new Vector2(104f, 100f), () => InvokeController("ShowSettingsFromMain"));
        CreateHotspot(root.transform, "EXIT", new Vector2(791f, -469f), new Vector2(208f, 118f), () => InvokeController("QuitGame"));

        BuildToast(root.transform);
    }

    void BuildApprovedArt(Transform parent)
    {
        GameObject backgroundObject = new GameObject("ApprovedReferenceArt");
        backgroundObject.transform.SetParent(parent, false);
        Image background = backgroundObject.AddComponent<Image>();
        background.raycastTarget = false;
        Stretch(background.rectTransform);

        Texture2D texture = Resources.Load<Texture2D>(ApprovedMenuResource);
        if (texture == null)
        {
            RuntimeFileLogger.Event("MENU", "Approved main-menu reference art is missing");
            background.color = new Color(.10f, .06f, .03f, 1f);
            return;
        }

        background.sprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(.5f, .5f),
            100f);
        background.type = Image.Type.Simple;
        background.preserveAspect = false;
        background.color = Color.white;
    }

    void CreateHotspot(Transform parent, string name, Vector2 position, Vector2 size, UnityAction action)
    {
        GameObject go = new GameObject(name + "_Hotspot");
        go.transform.SetParent(parent, false);

        Image input = go.AddComponent<Image>();
        input.color = new Color(1f, 1f, 1f, .001f);
        input.raycastTarget = true;

        RectTransform rect = input.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Button button = go.AddComponent<Button>();
        button.targetGraphic = input;
        button.transition = Selectable.Transition.None;
        button.onClick.AddListener(action);
    }

    void BuildToast(Transform parent)
    {
        toast = new GameObject("ComingSoonToast");
        toast.transform.SetParent(parent, false);
        Image image = toast.AddComponent<Image>();
        image.color = new Color(.11f, .04f, .012f, .95f);
        image.raycastTarget = false;

        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = new Vector2(270f, -405f);
        rect.sizeDelta = new Vector2(500f, 64f);

        Outline outline = toast.AddComponent<Outline>();
        outline.effectColor = new Color(1f, .58f, .15f, .9f);
        outline.effectDistance = new Vector2(2f, -2f);

        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(toast.transform, false);
        toastText = textObject.AddComponent<Text>();
        toastText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        toastText.fontSize = 19;
        toastText.fontStyle = FontStyle.Bold;
        toastText.alignment = TextAnchor.MiddleCenter;
        toastText.color = new Color(1f, .88f, .56f, 1f);
        toastText.raycastTarget = false;
        Stretch(toastText.rectTransform);
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

    static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
