using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MainMenuBackgroundOverride : MonoBehaviour
{
    const string ApprovedMenuResource = "Menu/ApprovedMainMenu";
    const float ReferenceWidth = 1920f;
    const float ReferenceHeight = 1080f;

    readonly List<Sprite> runtimeButtonSprites = new List<Sprite>();

    GameObject appliedMainMenu;
    GameMenuController controller;
    Texture2D approvedTexture;

    GameObject featureOverlay;
    Text featureTitle;
    Text featureBody;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoStart()
    {
        if (FindFirstObjectByType<MainMenuBackgroundOverride>() == null)
            new GameObject("ApprovedMainMenuPresenter").AddComponent<MainMenuBackgroundOverride>();
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ClearRuntimeSprites();
        appliedMainMenu = null;
        controller = null;
        featureOverlay = null;
        featureTitle = null;
        featureBody = null;
        approvedTexture = null;
        RuntimeFileLogger.Event("MENU", $"Approved main-menu presenter rebound after scene load: {scene.name}");
    }

    void Update()
    {
        if (featureOverlay != null && featureOverlay.activeSelf && GameInput.PausePressed())
            HideFeature();
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
        RuntimeFileLogger.Event("MENU", "Applied approved main-menu art with stable navigation lifecycle");
    }

    void BuildApprovedMenu(Transform mainMenu)
    {
        for (int i = 0; i < mainMenu.childCount; i++)
            mainMenu.GetChild(i).gameObject.SetActive(false);

        GameObject root = new GameObject("ApprovedMainMenu");
        root.transform.SetParent(mainMenu, false);
        Stretch(root.AddComponent<RectTransform>());

        BuildApprovedArt(root.transform);

        CreateAnimatedButton(root.transform, "PLAY", new Vector2(437f, 286f), new Vector2(548f, 203f), () => InvokeController("ShowLevels"), .24f);
        CreateAnimatedButton(root.transform, "HEROES", new Vector2(454f, 127f), new Vector2(457f, 116f), () => ShowFeature("HEROES"), .16f);
        CreateAnimatedButton(root.transform, "TOWERS", new Vector2(454f, 8f), new Vector2(457f, 112f), () => ShowFeature("TOWERS"), .16f);
        CreateAnimatedButton(root.transform, "UPGRADES", new Vector2(454f, -106f), new Vector2(457f, 110f), () => ShowFeature("UPGRADES"), .16f);
        CreateAnimatedButton(root.transform, "SHOP", new Vector2(454f, -217f), new Vector2(457f, 106f), () => ShowFeature("SHOP"), .16f);
        CreateAnimatedButton(root.transform, "SETTINGS", new Vector2(861f, 473f), new Vector2(104f, 100f), () => InvokeController("ShowSettingsFromMain"), .20f);
        CreateAnimatedButton(root.transform, "EXIT", new Vector2(791f, -469f), new Vector2(208f, 118f), () => InvokeController("QuitGame"), .18f);

        BuildFeatureOverlay(root.transform);
    }

    void BuildApprovedArt(Transform parent)
    {
        GameObject backgroundObject = new GameObject("ApprovedReferenceArt");
        backgroundObject.transform.SetParent(parent, false);
        Image background = backgroundObject.AddComponent<Image>();
        background.raycastTarget = false;
        Stretch(background.rectTransform);

        approvedTexture = Resources.Load<Texture2D>(ApprovedMenuResource);
        if (approvedTexture == null)
        {
            RuntimeFileLogger.Event("MENU", "Approved main-menu reference art is missing");
            background.color = new Color(.10f, .06f, .03f, 1f);
            return;
        }

        Sprite backgroundSprite = Sprite.Create(
            approvedTexture,
            new Rect(0f, 0f, approvedTexture.width, approvedTexture.height),
            new Vector2(.5f, .5f),
            100f);
        runtimeButtonSprites.Add(backgroundSprite);

        background.sprite = backgroundSprite;
        background.type = Image.Type.Simple;
        background.preserveAspect = false;
        background.color = Color.white;
    }

    void CreateAnimatedButton(
        Transform parent,
        string name,
        Vector2 position,
        Vector2 size,
        UnityAction action,
        float glowAlpha)
    {
        GameObject root = new GameObject(name + "_Button");
        root.transform.SetParent(parent, false);

        Image input = root.AddComponent<Image>();
        input.color = new Color(1f, 1f, 1f, .001f);
        input.raycastTarget = true;

        RectTransform rootRect = input.rectTransform;
        rootRect.anchorMin = rootRect.anchorMax = rootRect.pivot = new Vector2(.5f, .5f);
        rootRect.anchoredPosition = position;
        rootRect.sizeDelta = size;

        Button button = root.AddComponent<Button>();
        button.targetGraphic = input;
        button.transition = Selectable.Transition.None;
        button.onClick.AddListener(action);

        Sprite buttonSprite = CreateButtonSprite(position, size);
        if (buttonSprite == null) return;

        GameObject baseObject = new GameObject("PressedBase");
        baseObject.transform.SetParent(root.transform, false);
        Image pressedBase = baseObject.AddComponent<Image>();
        pressedBase.sprite = buttonSprite;
        pressedBase.type = Image.Type.Simple;
        pressedBase.preserveAspect = false;
        pressedBase.color = new Color(.48f, .40f, .31f, 1f);
        pressedBase.raycastTarget = false;
        Stretch(pressedBase.rectTransform);

        GameObject faceObject = new GameObject("AnimatedFace");
        faceObject.transform.SetParent(root.transform, false);
        Image face = faceObject.AddComponent<Image>();
        face.sprite = buttonSprite;
        face.type = Image.Type.Simple;
        face.preserveAspect = false;
        face.color = Color.white;
        face.raycastTarget = false;
        Stretch(face.rectTransform);

        Outline glow = faceObject.AddComponent<Outline>();
        glow.effectColor = new Color(1f, .72f, .18f, 0f);
        glow.effectDistance = new Vector2(3f, -3f);

        Shadow shadow = faceObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(.05f, .012f, .002f, .72f);
        shadow.effectDistance = new Vector2(0f, -6f);

        ApprovedMenuButtonFeedback feedback = root.AddComponent<ApprovedMenuButtonFeedback>();
        feedback.Configure(face.rectTransform, face, glow, glowAlpha);
    }

    Sprite CreateButtonSprite(Vector2 position, Vector2 size)
    {
        if (approvedTexture == null) return null;

        float scaleX = approvedTexture.width / ReferenceWidth;
        float scaleY = approvedTexture.height / ReferenceHeight;

        float x = (ReferenceWidth * .5f + position.x - size.x * .5f) * scaleX;
        float y = (ReferenceHeight * .5f + position.y - size.y * .5f) * scaleY;
        float width = size.x * scaleX;
        float height = size.y * scaleY;

        x = Mathf.Clamp(x, 0f, approvedTexture.width - 1f);
        y = Mathf.Clamp(y, 0f, approvedTexture.height - 1f);
        width = Mathf.Clamp(width, 1f, approvedTexture.width - x);
        height = Mathf.Clamp(height, 1f, approvedTexture.height - y);

        Sprite sprite = Sprite.Create(
            approvedTexture,
            new Rect(x, y, width, height),
            new Vector2(.5f, .5f),
            100f);
        runtimeButtonSprites.Add(sprite);
        return sprite;
    }

    void BuildFeatureOverlay(Transform parent)
    {
        featureOverlay = new GameObject("FeatureOverlay");
        featureOverlay.transform.SetParent(parent, false);

        Image blocker = featureOverlay.AddComponent<Image>();
        blocker.color = new Color(.025f, .012f, .006f, .96f);
        blocker.raycastTarget = true;
        Stretch(blocker.rectTransform);

        GameObject panel = new GameObject("FeatureCard");
        panel.transform.SetParent(featureOverlay.transform, false);
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(.10f, .045f, .018f, .98f);
        panelImage.raycastTarget = false;

        RectTransform panelRect = panelImage.rectTransform;
        panelRect.anchorMin = panelRect.anchorMax = panelRect.pivot = new Vector2(.5f, .5f);
        panelRect.sizeDelta = new Vector2(760f, 430f);

        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(.95f, .54f, .14f, .85f);
        outline.effectDistance = new Vector2(3f, -3f);

        featureTitle = AddOverlayText(panel.transform, "", new Vector2(0f, 105f), new Vector2(620f, 80f), 44, FontStyle.Bold, new Color(1f, .68f, .18f, 1f));
        featureBody = AddOverlayText(panel.transform, "", new Vector2(0f, 20f), new Vector2(620f, 110f), 20, FontStyle.Normal, new Color(.90f, .80f, .66f, 1f));

        CreateOverlayButton(panel.transform, GameLanguage.T("BACK", "НАЗАД"), new Vector2(0f, -125f), HideFeature);
        featureOverlay.SetActive(false);
    }

    Text AddOverlayText(Transform parent, string value, Vector2 position, Vector2 size, int fontSize, FontStyle style, Color color)
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

        RectTransform rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return text;
    }

    void CreateOverlayButton(Transform parent, string label, Vector2 position, UnityAction action)
    {
        GameObject go = new GameObject("BackButton");
        go.transform.SetParent(parent, false);

        Image image = go.AddComponent<Image>();
        image.color = new Color(.65f, .16f, .045f, 1f);

        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(280f, 66f);

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(1f, .62f, .16f, .95f);
        outline.effectDistance = new Vector2(3f, -3f);

        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        go.AddComponent<MenuButtonFeedback>();

        Text labelText = AddOverlayText(go.transform, label, Vector2.zero, new Vector2(260f, 58f), 24, FontStyle.Bold, Color.white);
        labelText.raycastTarget = false;
    }

    void ShowFeature(string feature)
    {
        if (featureOverlay == null || featureTitle == null || featureBody == null) return;

        featureTitle.text = feature;
        featureBody.text = GameLanguage.T(
            "This section is prepared for a later production pass.",
            "Этот раздел будет подключён на следующем этапе разработки.");
        featureOverlay.SetActive(true);
        RuntimeFileLogger.Event("MENU", $"Opened main-menu section: {feature}");
    }

    void HideFeature()
    {
        if (featureOverlay == null) return;
        featureOverlay.SetActive(false);
        RuntimeFileLogger.Event("MENU", "Returned from main-menu section to approved main menu");
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

    void ClearRuntimeSprites()
    {
        for (int i = 0; i < runtimeButtonSprites.Count; i++)
        {
            if (runtimeButtonSprites[i] != null)
                Destroy(runtimeButtonSprites[i]);
        }
        runtimeButtonSprites.Clear();
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
        ClearRuntimeSprites();
    }
}

public sealed class ApprovedMenuButtonFeedback : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    ISelectHandler,
    IDeselectHandler
{
    const float HoverScale = 1.055f;
    const float PressScale = .92f;
    const float ReleaseKickScale = 1.08f;
    const float Speed = 18f;

    RectTransform animatedFace;
    Image faceImage;
    Outline glow;
    float hoverGlowAlpha;
    Vector3 wantedScale = Vector3.one;
    float wantedGlow;
    bool highlighted;

    public void Configure(RectTransform face, Image image, Outline outline, float glowAlpha)
    {
        animatedFace = face;
        faceImage = image;
        glow = outline;
        hoverGlowAlpha = glowAlpha;
        wantedScale = Vector3.one;
        wantedGlow = 0f;
        ApplyVisuals(0f);
    }

    void Update()
    {
        if (animatedFace == null) return;

        float t = 1f - Mathf.Exp(-Speed * Time.unscaledDeltaTime);
        animatedFace.localScale = Vector3.Lerp(animatedFace.localScale, wantedScale, t);

        float currentGlow = glow != null ? glow.effectColor.a : 0f;
        float nextGlow = Mathf.Lerp(currentGlow, wantedGlow, t);
        ApplyVisuals(nextGlow);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        highlighted = true;
        wantedScale = Vector3.one * HoverScale;
        wantedGlow = hoverGlowAlpha;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlighted = false;
        wantedScale = Vector3.one;
        wantedGlow = 0f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        wantedScale = Vector3.one * PressScale;
        wantedGlow = hoverGlowAlpha * 1.35f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        if (animatedFace != null)
            animatedFace.localScale = Vector3.one * ReleaseKickScale;
        wantedScale = Vector3.one * (highlighted ? HoverScale : 1f);
        wantedGlow = highlighted ? hoverGlowAlpha : 0f;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!IsInteractable()) return;
        highlighted = true;
        wantedScale = Vector3.one * HoverScale;
        wantedGlow = hoverGlowAlpha;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        highlighted = false;
        wantedScale = Vector3.one;
        wantedGlow = 0f;
    }

    void ApplyVisuals(float glowAlpha)
    {
        if (faceImage != null)
        {
            float warmth = highlighted ? .94f : 1f;
            faceImage.color = new Color(1f, warmth, highlighted ? .84f : 1f, 1f);
        }

        if (glow != null)
            glow.effectColor = new Color(1f, .72f, .18f, Mathf.Clamp01(glowAlpha));
    }

    bool IsInteractable()
    {
        Button button = GetComponent<Button>();
        return button == null || button.interactable;
    }
}
