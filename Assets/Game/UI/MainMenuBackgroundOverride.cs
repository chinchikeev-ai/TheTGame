using UnityEngine;
using UnityEngine.UI;

public sealed class MainMenuBackgroundOverride : MonoBehaviour
{
    const string HighResolutionBackgroundResource = "Menu/Main_screen";
    const float BackgroundZoom = 1.45f;

    Texture2D highResolutionTexture;
    Sprite highResolutionSprite;
    GameObject appliedMenu;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoStart()
    {
        if (FindFirstObjectByType<MainMenuBackgroundOverride>() == null)
            new GameObject("MainMenuVisualRefresh").AddComponent<MainMenuBackgroundOverride>();
    }

    void Awake()
    {
        highResolutionTexture = Resources.Load<Texture2D>(HighResolutionBackgroundResource);
        if (highResolutionTexture != null)
        {
            highResolutionSprite = Sprite.Create(
                highResolutionTexture,
                new Rect(0f, 0f, highResolutionTexture.width, highResolutionTexture.height),
                new Vector2(.5f, .5f),
                100f);
        }
    }

    void LateUpdate()
    {
        GameObject canvasObject = GameObject.Find("MenuCanvas");
        if (canvasObject == null) return;

        Transform mainMenu = canvasObject.transform.Find("MainMenu");
        if (mainMenu == null) return;

        if (appliedMenu == mainMenu.gameObject) return;
        ApplyPresentation(mainMenu);
        appliedMenu = mainMenu.gameObject;
    }

    void ApplyPresentation(Transform mainMenu)
    {
        ApplyHighResolutionBackground(mainMenu);
        ApplyReadabilityWash(mainMenu);

        Transform panel = mainMenu.Find("MainPanel");
        if (panel != null)
            RestyleMainPanel(panel);

        RuntimeFileLogger.Event("MENU", "Applied refreshed high-resolution main-menu presentation");
    }

    void ApplyHighResolutionBackground(Transform mainMenu)
    {
        Transform background = mainMenu.Find("Background");
        if (background != null)
        {
            Image image = background.GetComponent<Image>();
            if (image != null && highResolutionSprite != null)
            {
                image.sprite = highResolutionSprite;
                image.type = Image.Type.Simple;
                image.preserveAspect = false;
                image.color = Color.white;
                image.raycastTarget = false;
            }

            RectTransform backgroundRect = background as RectTransform;
            if (backgroundRect != null)
                backgroundRect.localScale = new Vector3(BackgroundZoom, BackgroundZoom, 1f);

            AspectRatioFitter fitter = background.GetComponent<AspectRatioFitter>();
            if (fitter != null && highResolutionTexture != null)
                fitter.aspectRatio = (float)highResolutionTexture.width / highResolutionTexture.height;
        }

        Transform shade = mainMenu.Find("CinematicShade");
        if (shade != null)
        {
            Image shadeImage = shade.GetComponent<Image>();
            if (shadeImage != null)
                shadeImage.color = new Color(.018f, .008f, .004f, .12f);
        }
    }

    void ApplyReadabilityWash(Transform mainMenu)
    {
        Transform panel = mainMenu.Find("MainPanel");
        int insertIndex = panel != null ? panel.GetSiblingIndex() : mainMenu.childCount;

        CreateWashBand(mainMenu, "ReadabilityWashSoft", .48f, 1f, .08f, insertIndex++);
        CreateWashBand(mainMenu, "ReadabilityWashMid", .58f, 1f, .12f, insertIndex++);
        CreateWashBand(mainMenu, "ReadabilityWashStrong", .69f, 1f, .18f, insertIndex++);
    }

    static void CreateWashBand(Transform parent, string name, float minX, float maxX, float alpha, int siblingIndex)
    {
        Transform existing = parent.Find(name);
        GameObject go = existing != null ? existing.gameObject : new GameObject(name);
        if (existing == null) go.transform.SetParent(parent, false);

        Image image = go.GetComponent<Image>();
        if (image == null) image = go.AddComponent<Image>();
        image.color = new Color(.025f, .010f, .004f, alpha);
        image.raycastTarget = false;

        RectTransform rect = image.rectTransform;
        rect.anchorMin = new Vector2(minX, 0f);
        rect.anchorMax = new Vector2(maxX, 1f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        go.transform.SetSiblingIndex(Mathf.Clamp(siblingIndex, 0, parent.childCount - 1));
    }

    void RestyleMainPanel(Transform panel)
    {
        RectTransform panelRect = panel as RectTransform;
        if (panelRect != null)
        {
            panelRect.anchorMin = panelRect.anchorMax = panelRect.pivot = new Vector2(.79f, .5f);
            panelRect.sizeDelta = new Vector2(720f, 940f);
        }

        Image panelImage = panel.GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.color = new Color(0f, 0f, 0f, 0f);
            panelImage.raycastTarget = false;
        }

        Outline panelOutline = panel.GetComponent<Outline>();
        if (panelOutline != null) panelOutline.enabled = false;

        CreateBrandPlaque(panel);
        CreateButtonRail(panel);
        RestyleMainTexts(panel);
        RestyleMainButtons(panel);
    }

    static void CreateBrandPlaque(Transform panel)
    {
        Transform existing = panel.Find("BrandPlaque");
        GameObject plaque = existing != null ? existing.gameObject : new GameObject("BrandPlaque");
        if (existing == null) plaque.transform.SetParent(panel, false);

        Image image = plaque.GetComponent<Image>();
        if (image == null) image = plaque.AddComponent<Image>();
        image.color = new Color(.11f, .038f, .012f, .94f);
        image.raycastTarget = false;

        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = new Vector2(0f, 300f);
        rect.sizeDelta = new Vector2(650f, 220f);

        Outline outline = plaque.GetComponent<Outline>();
        if (outline == null) outline = plaque.AddComponent<Outline>();
        outline.effectColor = new Color(.95f, .56f, .16f, .86f);
        outline.effectDistance = new Vector2(3f, -3f);

        Shadow shadow = FindPlainShadow(plaque);
        if (shadow == null) shadow = plaque.AddComponent<Shadow>();
        shadow.effectColor = new Color(.02f, .006f, .002f, .78f);
        shadow.effectDistance = new Vector2(0f, -8f);

        Transform inner = plaque.transform.Find("InnerPlate");
        GameObject innerGo = inner != null ? inner.gameObject : new GameObject("InnerPlate");
        if (inner == null) innerGo.transform.SetParent(plaque.transform, false);
        Image innerImage = innerGo.GetComponent<Image>();
        if (innerImage == null) innerImage = innerGo.AddComponent<Image>();
        innerImage.color = new Color(.30f, .085f, .025f, .82f);
        innerImage.raycastTarget = false;
        RectTransform innerRect = innerImage.rectTransform;
        innerRect.anchorMin = Vector2.zero;
        innerRect.anchorMax = Vector2.one;
        innerRect.offsetMin = new Vector2(10f, 10f);
        innerRect.offsetMax = new Vector2(-10f, -10f);

        Transform top = plaque.transform.Find("GoldTop");
        GameObject topGo = top != null ? top.gameObject : new GameObject("GoldTop");
        if (top == null) topGo.transform.SetParent(plaque.transform, false);
        Image topImage = topGo.GetComponent<Image>();
        if (topImage == null) topImage = topGo.AddComponent<Image>();
        topImage.color = new Color(1f, .63f, .16f, .95f);
        topImage.raycastTarget = false;
        RectTransform topRect = topImage.rectTransform;
        topRect.anchorMin = new Vector2(.08f, 1f);
        topRect.anchorMax = new Vector2(.92f, 1f);
        topRect.pivot = new Vector2(.5f, 1f);
        topRect.anchoredPosition = new Vector2(0f, -7f);
        topRect.sizeDelta = new Vector2(0f, 6f);

        plaque.transform.SetSiblingIndex(0);
    }

    static void CreateButtonRail(Transform panel)
    {
        Transform existing = panel.Find("ButtonRail");
        GameObject rail = existing != null ? existing.gameObject : new GameObject("ButtonRail");
        if (existing == null) rail.transform.SetParent(panel, false);

        Image image = rail.GetComponent<Image>();
        if (image == null) image = rail.AddComponent<Image>();
        image.color = new Color(.035f, .012f, .004f, .28f);
        image.raycastTarget = false;

        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = new Vector2(0f, -67f);
        rect.sizeDelta = new Vector2(636f, 525f);

        Outline outline = rail.GetComponent<Outline>();
        if (outline == null) outline = rail.AddComponent<Outline>();
        outline.effectColor = new Color(.88f, .49f, .13f, .20f);
        outline.effectDistance = new Vector2(2f, -2f);

        rail.transform.SetSiblingIndex(Mathf.Min(1, panel.childCount - 1));
    }

    static void RestyleMainTexts(Transform panel)
    {
        foreach (Text text in panel.GetComponentsInChildren<Text>(true))
        {
            string value = text.text ?? string.Empty;
            RectTransform rect = text.rectTransform;

            if (value == "THE TROY GAME")
            {
                text.fontSize = 58;
                text.fontStyle = FontStyle.Bold;
                text.color = new Color(1f, .67f, .17f, 1f);
                rect.anchoredPosition = new Vector2(0f, 350f);
                rect.sizeDelta = new Vector2(620f, 80f);
                EnsureTextShadow(text, new Color(.12f, .035f, .005f, .95f), new Vector2(3f, -4f));
            }
            else if (value == "GODS DEFENSE")
            {
                text.fontSize = 25;
                text.fontStyle = FontStyle.Bold;
                text.color = new Color(1f, .88f, .55f, 1f);
                rect.anchoredPosition = new Vector2(0f, 292f);
                rect.sizeDelta = new Vector2(560f, 42f);
                EnsureTextShadow(text, new Color(.06f, .02f, .005f, .85f), new Vector2(2f, -2f));
            }
            else if (value.Contains("DEFEND TROY") || value.Contains("ЗАЩИТИ ТРОЮ"))
            {
                text.fontSize = 16;
                text.color = new Color(.94f, .82f, .64f, .96f);
                rect.anchoredPosition = new Vector2(0f, 245f);
                rect.sizeDelta = new Vector2(570f, 36f);
            }
            else if (value.Contains("Progress saves automatically") || value.Contains("Прогресс сохраняется автоматически"))
            {
                text.fontSize = 13;
                text.color = new Color(.86f, .72f, .55f, .90f);
                rect.anchoredPosition = new Vector2(0f, -355f);
            }
            else if (value.Contains("PRE-ALPHA"))
            {
                text.fontSize = 12;
                text.color = new Color(.70f, .56f, .42f, .86f);
                rect.anchoredPosition = new Vector2(0f, -405f);
            }
        }
    }

    static void RestyleMainButtons(Transform panel)
    {
        Button[] buttons = panel.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length && i < 5; i++)
        {
            Vector2 position;
            Vector2 size;
            switch (i)
            {
                case 0:
                    position = new Vector2(0f, 120f);
                    size = new Vector2(590f, 92f);
                    break;
                case 1:
                    position = new Vector2(0f, 12f);
                    size = new Vector2(550f, 74f);
                    break;
                case 2:
                    position = new Vector2(0f, -78f);
                    size = new Vector2(550f, 74f);
                    break;
                case 3:
                    position = new Vector2(0f, -168f);
                    size = new Vector2(520f, 70f);
                    break;
                default:
                    position = new Vector2(0f, -258f);
                    size = new Vector2(430f, 66f);
                    break;
            }

            StyleButton(buttons[i], i, position, size);
        }
    }

    static void StyleButton(Button button, int index, Vector2 position, Vector2 size)
    {
        Image frame = button.GetComponent<Image>();
        if (frame == null) frame = button.gameObject.AddComponent<Image>();
        frame.sprite = null;
        frame.type = Image.Type.Simple;
        frame.color = FrameColor(index);
        frame.raycastTarget = true;

        RectTransform rect = frame.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        button.targetGraphic = frame;
        button.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, .93f, .72f, 1f);
        colors.pressedColor = new Color(.70f, .55f, .40f, 1f);
        colors.selectedColor = new Color(1f, .88f, .62f, 1f);
        colors.disabledColor = new Color(.42f, .42f, .42f, .62f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = .07f;
        button.colors = colors;

        Outline outline = button.GetComponent<Outline>();
        if (outline == null) outline = button.gameObject.AddComponent<Outline>();
        outline.effectColor = index == 0
            ? new Color(1f, .67f, .18f, .92f)
            : new Color(.36f, .16f, .055f, .96f);
        outline.effectDistance = new Vector2(3f, -3f);

        Shadow shadow = FindPlainShadow(button.gameObject);
        if (shadow == null) shadow = button.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(.02f, .006f, .002f, .84f);
        shadow.effectDistance = new Vector2(0f, -7f);

        GameObject face = EnsureImageChild(button.transform, "RefreshFace", FaceColor(index));
        RectTransform faceRect = face.GetComponent<RectTransform>();
        faceRect.anchorMin = Vector2.zero;
        faceRect.anchorMax = Vector2.one;
        faceRect.offsetMin = new Vector2(8f, 9f);
        faceRect.offsetMax = new Vector2(-8f, -8f);
        face.transform.SetSiblingIndex(0);

        GameObject topHighlight = EnsureImageChild(face.transform, "TopHighlight", TopHighlightColor(index));
        RectTransform topRect = topHighlight.GetComponent<RectTransform>();
        topRect.anchorMin = new Vector2(.03f, 1f);
        topRect.anchorMax = new Vector2(.97f, 1f);
        topRect.pivot = new Vector2(.5f, 1f);
        topRect.anchoredPosition = new Vector2(0f, -5f);
        topRect.sizeDelta = new Vector2(0f, 4f);

        GameObject accent = EnsureImageChild(face.transform, "LeftAccent", AccentColor(index));
        RectTransform accentRect = accent.GetComponent<RectTransform>();
        accentRect.anchorMin = new Vector2(0f, .10f);
        accentRect.anchorMax = new Vector2(0f, .90f);
        accentRect.pivot = new Vector2(0f, .5f);
        accentRect.anchoredPosition = new Vector2(10f, 0f);
        accentRect.sizeDelta = new Vector2(11f, 0f);

        CreateBadge(button.transform, index, size);

        Text label = button.GetComponentInChildren<Text>(true);
        if (label != null)
        {
            label.fontSize = index == 0 ? 26 : 21;
            label.fontStyle = FontStyle.Bold;
            label.color = LabelColor(index);
            label.alignment = TextAnchor.MiddleCenter;
            label.raycastTarget = false;
            RectTransform labelRect = label.rectTransform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(78f, 0f);
            labelRect.offsetMax = new Vector2(-50f, 0f);
            labelRect.pivot = new Vector2(.5f, .5f);
            EnsureTextShadow(label, new Color(.08f, .025f, .008f, .78f), new Vector2(1f, -2f));
        }

        CreateChevron(button.transform, index);
    }

    static void CreateBadge(Transform parent, int index, Vector2 size)
    {
        GameObject badge = EnsureImageChild(parent, "MenuBadge", BadgeColor(index));
        RectTransform badgeRect = badge.GetComponent<RectTransform>();
        badgeRect.anchorMin = badgeRect.anchorMax = badgeRect.pivot = new Vector2(0f, .5f);
        badgeRect.anchoredPosition = new Vector2(36f, 0f);
        badgeRect.sizeDelta = new Vector2(index == 0 ? 58f : 50f, index == 0 ? 58f : 50f);
        badge.transform.SetSiblingIndex(1);

        Text text = badge.GetComponentInChildren<Text>(true);
        if (text == null)
        {
            GameObject textGo = new GameObject("BadgeText");
            textGo.transform.SetParent(badge.transform, false);
            text = textGo.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.alignment = TextAnchor.MiddleCenter;
            text.fontStyle = FontStyle.Bold;
            text.raycastTarget = false;
            RectTransform textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = textRect.offsetMax = Vector2.zero;
        }

        text.text = index == 0 ? ">" : index == 1 ? "+" : index == 2 ? "III" : index == 3 ? "#" : "X";
        text.fontSize = index == 2 ? 17 : 24;
        text.color = BadgeTextColor(index);
    }

    static void CreateChevron(Transform parent, int index)
    {
        Transform existing = parent.Find("MenuChevron");
        Text text;
        if (existing == null)
        {
            GameObject go = new GameObject("MenuChevron");
            go.transform.SetParent(parent, false);
            text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.alignment = TextAnchor.MiddleCenter;
            text.fontStyle = FontStyle.Bold;
            text.raycastTarget = false;
        }
        else
        {
            text = existing.GetComponent<Text>();
            if (text == null) text = existing.gameObject.AddComponent<Text>();
        }

        text.text = ">";
        text.fontSize = 25;
        text.color = LabelColor(index);
        RectTransform rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(1f, .5f);
        rect.anchoredPosition = new Vector2(-28f, 0f);
        rect.sizeDelta = new Vector2(32f, 52f);
    }

    static GameObject EnsureImageChild(Transform parent, string name, Color color)
    {
        Transform existing = parent.Find(name);
        GameObject go = existing != null ? existing.gameObject : new GameObject(name);
        if (existing == null) go.transform.SetParent(parent, false);

        Image image = go.GetComponent<Image>();
        if (image == null) image = go.AddComponent<Image>();
        image.sprite = null;
        image.type = Image.Type.Simple;
        image.color = color;
        image.raycastTarget = false;
        return go;
    }

    static Shadow FindPlainShadow(GameObject go)
    {
        Shadow[] shadows = go.GetComponents<Shadow>();
        foreach (Shadow shadow in shadows)
        {
            if (shadow.GetType() == typeof(Shadow)) return shadow;
        }
        return null;
    }

    static void EnsureTextShadow(Text text, Color color, Vector2 distance)
    {
        Shadow shadow = FindPlainShadow(text.gameObject);
        if (shadow == null) shadow = text.gameObject.AddComponent<Shadow>();
        shadow.effectColor = color;
        shadow.effectDistance = distance;
    }

    static Color FrameColor(int index)
    {
        if (index == 0) return new Color(.97f, .60f, .16f, 1f);
        if (index == 4) return new Color(.55f, .16f, .08f, 1f);
        return new Color(.39f, .20f, .075f, 1f);
    }

    static Color FaceColor(int index)
    {
        if (index == 0) return new Color(.58f, .095f, .04f, 1f);
        if (index == 1 || index == 2) return new Color(.73f, .53f, .30f, 1f);
        if (index == 4) return new Color(.24f, .045f, .025f, 1f);
        return new Color(.17f, .075f, .032f, 1f);
    }

    static Color TopHighlightColor(int index)
    {
        if (index == 0) return new Color(1f, .76f, .33f, .78f);
        if (index == 1 || index == 2) return new Color(1f, .82f, .50f, .48f);
        return new Color(.94f, .58f, .20f, .32f);
    }

    static Color AccentColor(int index)
    {
        if (index == 4) return new Color(.82f, .18f, .08f, 1f);
        return new Color(1f, .59f, .13f, 1f);
    }

    static Color BadgeColor(int index)
    {
        if (index == 0) return new Color(.18f, .055f, .012f, 1f);
        if (index == 1 || index == 2) return new Color(.34f, .16f, .05f, 1f);
        if (index == 4) return new Color(.10f, .025f, .015f, 1f);
        return new Color(.08f, .035f, .018f, 1f);
    }

    static Color BadgeTextColor(int index)
    {
        if (index == 1 || index == 2) return new Color(1f, .75f, .34f, 1f);
        return new Color(1f, .84f, .48f, 1f);
    }

    static Color LabelColor(int index)
    {
        if (index == 1 || index == 2) return new Color(.20f, .07f, .018f, 1f);
        return new Color(1f, .86f, .54f, 1f);
    }
}
