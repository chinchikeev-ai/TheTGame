using System;
using System.Collections;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class MainMenuBackgroundOverride : MonoBehaviour
{
    const string ApprovedMenuResourceRoot = "Menu/ApprovedMainMenu/part";
    const int ApprovedMenuPartCount = 10;

    readonly Vector2 playPosition = new Vector2(437f, 286f);
    readonly Vector2 playSize = new Vector2(548f, 203f);
    readonly Vector2 heroesPosition = new Vector2(454f, 127f);
    readonly Vector2 secondarySize = new Vector2(457f, 116f);
    readonly Vector2 towersPosition = new Vector2(454f, 8f);
    readonly Vector2 upgradesPosition = new Vector2(454f, -106f);
    readonly Vector2 shopPosition = new Vector2(454f, -217f);
    readonly Vector2 settingsPosition = new Vector2(861f, 473f);
    readonly Vector2 settingsSize = new Vector2(104f, 100f);
    readonly Vector2 exitPosition = new Vector2(791f, -469f);
    readonly Vector2 exitSize = new Vector2(208f, 118f);

    Texture2D approvedTexture;
    Sprite approvedSprite;
    GameObject appliedMainMenu;
    GameMenuController controller;
    GameObject toastPanel;
    Text toastText;
    CanvasGroup toastCanvasGroup;
    Coroutine toastRoutine;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoStart()
    {
        if (FindFirstObjectByType<MainMenuBackgroundOverride>() == null)
            new GameObject("ApprovedMainMenuPresenter").AddComponent<MainMenuBackgroundOverride>();
    }

    void Awake()
    {
        approvedSprite = LoadApprovedMenuSprite();
    }

    void LateUpdate()
    {
        if (approvedSprite == null) return;
        if (appliedMainMenu != null) return;

        controller = FindFirstObjectByType<GameMenuController>();
        if (controller == null) return;

        GameObject canvasObject = GameObject.Find("MenuCanvas");
        if (canvasObject == null) return;

        Transform mainMenu = canvasObject.transform.Find("MainMenu");
        if (mainMenu == null) return;

        ApplyApprovedMenu(mainMenu);
        appliedMainMenu = mainMenu.gameObject;
    }

    Sprite LoadApprovedMenuSprite()
    {
        StringBuilder encoded = new StringBuilder(560000);
        for (int i = 0; i < ApprovedMenuPartCount; i++)
        {
            TextAsset part = Resources.Load<TextAsset>($"{ApprovedMenuResourceRoot}{i:00}");
            if (part == null)
            {
                RuntimeFileLogger.Event("MENU", $"Approved menu reference part missing: {i:00}");
                return null;
            }

            encoded.Append(part.text.Trim());
        }

        try
        {
            byte[] bytes = Convert.FromBase64String(encoded.ToString());
            approvedTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false)
            {
                name = "ApprovedMainMenuReference"
            };

            if (!approvedTexture.LoadImage(bytes, false))
            {
                RuntimeFileLogger.Event("MENU", "Approved menu reference could not be decoded as an image");
                Destroy(approvedTexture);
                approvedTexture = null;
                return null;
            }

            approvedTexture.wrapMode = TextureWrapMode.Clamp;
            approvedTexture.filterMode = FilterMode.Bilinear;
            return Sprite.Create(
                approvedTexture,
                new Rect(0f, 0f, approvedTexture.width, approvedTexture.height),
                new Vector2(.5f, .5f),
                100f);
        }
        catch (Exception exception)
        {
            RuntimeFileLogger.Event("MENU", $"Approved menu reference decode failed: {exception.GetType().Name}");
            return null;
        }
    }

    void ApplyApprovedMenu(Transform mainMenu)
    {
        for (int i = 0; i < mainMenu.childCount; i++)
            mainMenu.GetChild(i).gameObject.SetActive(false);

        GameObject approvedRoot = new GameObject("ApprovedMainMenu");
        approvedRoot.transform.SetParent(mainMenu, false);
        RectTransform approvedRect = approvedRoot.AddComponent<RectTransform>();
        Stretch(approvedRect);

        GameObject artObject = new GameObject("ApprovedReferenceArt");
        artObject.transform.SetParent(approvedRoot.transform, false);
        Image art = artObject.AddComponent<Image>();
        art.sprite = approvedSprite;
        art.type = Image.Type.Simple;
        art.preserveAspect = false;
        art.raycastTarget = false;
        Stretch(art.rectTransform);

        CreateHotspot(approvedRoot.transform, "PLAY", playPosition, playSize, OpenChapterSelect, .18f);
        CreateHotspot(approvedRoot.transform, "HEROES", heroesPosition, secondarySize, () => ShowComingSoon("HEROES"), .12f);
        CreateHotspot(approvedRoot.transform, "TOWERS", towersPosition, new Vector2(457f, 112f), () => ShowComingSoon("TOWERS"), .12f);
        CreateHotspot(approvedRoot.transform, "UPGRADES", upgradesPosition, new Vector2(457f, 110f), () => ShowComingSoon("UPGRADES"), .12f);
        CreateHotspot(approvedRoot.transform, "SHOP", shopPosition, new Vector2(457f, 106f), () => ShowComingSoon("SHOP"), .12f);
        CreateHotspot(approvedRoot.transform, "SETTINGS", settingsPosition, settingsSize, OpenSettings, .16f);
        CreateHotspot(approvedRoot.transform, "EXIT", exitPosition, exitSize, ExitGame, .14f);

        BuildToast(approvedRoot.transform);
        RuntimeFileLogger.Event("MENU", "Applied user-approved main-menu reference and interactive hotspots");
    }

    void CreateHotspot(
        Transform parent,
        string name,
        Vector2 position,
        Vector2 size,
        UnityEngine.Events.UnityAction action,
        float hoverAlpha)
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

        GameObject glowObject = new GameObject("Glow");
        glowObject.transform.SetParent(go.transform, false);
        Image glow = glowObject.AddComponent<Image>();
        glow.color = new Color(1f, .72f, .22f, 0f);
        glow.raycastTarget = false;
        RectTransform glowRect = glow.rectTransform;
        Stretch(glowRect);
        glowRect.offsetMin = new Vector2(-5f, -5f);
        glowRect.offsetMax = new Vector2(5f, 5f);

        Outline outline = glowObject.AddComponent<Outline>();
        outline.effectColor = new Color(1f, .76f, .30f, .0f);
        outline.effectDistance = new Vector2(3f, -3f);

        MainMenuHotspotFeedback feedback = go.AddComponent<MainMenuHotspotFeedback>();
        feedback.Configure(glow, outline, hoverAlpha);
    }

    void BuildToast(Transform parent)
    {
        toastPanel = new GameObject("ComingSoonToast");
        toastPanel.transform.SetParent(parent, false);
        Image panelImage = toastPanel.AddComponent<Image>();
        panelImage.color = new Color(.12f, .045f, .015f, .94f);
        panelImage.raycastTarget = false;

        RectTransform panelRect = panelImage.rectTransform;
        panelRect.anchorMin = panelRect.anchorMax = panelRect.pivot = new Vector2(.5f, .5f);
        panelRect.anchoredPosition = new Vector2(320f, -390f);
        panelRect.sizeDelta = new Vector2(460f, 64f);

        Outline outline = toastPanel.AddComponent<Outline>();
        outline.effectColor = new Color(1f, .60f, .18f, .80f);
        outline.effectDistance = new Vector2(2f, -2f);

        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(toastPanel.transform, false);
        toastText = textObject.AddComponent<Text>();
        toastText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        toastText.fontSize = 20;
        toastText.fontStyle = FontStyle.Bold;
        toastText.alignment = TextAnchor.MiddleCenter;
        toastText.color = new Color(1f, .88f, .58f, 1f);
        toastText.raycastTarget = false;
        Stretch(toastText.rectTransform);

        toastCanvasGroup = toastPanel.AddComponent<CanvasGroup>();
        toastCanvasGroup.alpha = 0f;
        toastPanel.SetActive(false);
    }

    void OpenChapterSelect()
    {
        InvokeController("ShowLevels");
    }

    void OpenSettings()
    {
        InvokeController("ShowSettingsFromMain");
    }

    void ExitGame()
    {
        InvokeController("QuitGame");
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
            RuntimeFileLogger.Event("MENU", $"Approved menu action was not found: {methodName}");
            return;
        }

        method.Invoke(controller, null);
    }

    void ShowComingSoon(string feature)
    {
        if (toastPanel == null || toastText == null) return;

        toastText.text = GameLanguage.T(
            $"{feature}  -  IN DEVELOPMENT",
            $"{feature}  -  В РАЗРАБОТКЕ");

        if (toastRoutine != null) StopCoroutine(toastRoutine);
        toastRoutine = StartCoroutine(AnimateToast());
    }

    IEnumerator AnimateToast()
    {
        toastPanel.SetActive(true);
        float time = 0f;
        while (time < .14f)
        {
            time += Time.unscaledDeltaTime;
            toastCanvasGroup.alpha = Mathf.Clamp01(time / .14f);
            yield return null;
        }

        toastCanvasGroup.alpha = 1f;
        float hold = 0f;
        while (hold < 1.35f)
        {
            hold += Time.unscaledDeltaTime;
            yield return null;
        }

        time = 0f;
        while (time < .22f)
        {
            time += Time.unscaledDeltaTime;
            toastCanvasGroup.alpha = 1f - Mathf.Clamp01(time / .22f);
            yield return null;
        }

        toastCanvasGroup.alpha = 0f;
        toastPanel.SetActive(false);
        toastRoutine = null;
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
        if (approvedSprite != null) Destroy(approvedSprite);
        if (approvedTexture != null) Destroy(approvedTexture);
    }
}

public sealed class MainMenuHotspotFeedback : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    ISelectHandler,
    IDeselectHandler
{
    const float HoverScale = 1.025f;
    const float PressScale = .965f;
    const float Speed = 20f;

    RectTransform rect;
    Image glow;
    Outline outline;
    float hoverAlpha;
    float targetAlpha;
    Vector3 targetScale = Vector3.one;
    bool highlighted;

    public void Configure(Image glowImage, Outline glowOutline, float alpha)
    {
        glow = glowImage;
        outline = glowOutline;
        hoverAlpha = alpha;
    }

    void Awake()
    {
        rect = transform as RectTransform;
    }

    void OnEnable()
    {
        highlighted = false;
        targetAlpha = 0f;
        targetScale = Vector3.one;
        if (rect != null) rect.localScale = Vector3.one;
        ApplyVisual(0f);
    }

    void Update()
    {
        if (rect == null) return;
        float t = 1f - Mathf.Exp(-Speed * Time.unscaledDeltaTime);
        rect.localScale = Vector3.Lerp(rect.localScale, targetScale, t);

        float current = glow != null ? glow.color.a : 0f;
        float next = Mathf.Lerp(current, targetAlpha, t);
        ApplyVisual(next);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHighlighted(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHighlighted(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        targetScale = Vector3.one * PressScale;
        targetAlpha = hoverAlpha * 1.25f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        targetScale = Vector3.one * (highlighted ? HoverScale : 1f);
        targetAlpha = highlighted ? hoverAlpha : 0f;
    }

    public void OnSelect(BaseEventData eventData)
    {
        SetHighlighted(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetHighlighted(false);
    }

    void SetHighlighted(bool value)
    {
        highlighted = value && IsInteractable();
        targetScale = Vector3.one * (highlighted ? HoverScale : 1f);
        targetAlpha = highlighted ? hoverAlpha : 0f;
    }

    void ApplyVisual(float alpha)
    {
        if (glow != null)
            glow.color = new Color(1f, .72f, .22f, alpha);
        if (outline != null)
            outline.effectColor = new Color(1f, .80f, .34f, Mathf.Clamp01(alpha * 2.1f));
    }

    bool IsInteractable()
    {
        Button button = GetComponent<Button>();
        return button == null || button.interactable;
    }
}
