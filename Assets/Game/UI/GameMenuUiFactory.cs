using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum MenuTextStyle
{
    Normal,
    Logo,
    Subtitle,
    Button,
    Muted
}

public enum MenuButtonStyle
{
    Default,
    Stone,
    Highlight,
    Ghost
}

public static class GameMenuUiFactory
{
    const string IllustratedButtonResource = "Menu/Buttons/ButtonPrimary";
    static Sprite illustratedButtonSprite;

    public static GameObject MakeScreen(Canvas canvas, string name, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(canvas.transform, false);
        Image image = go.AddComponent<Image>();
        image.color = color;
        StretchToParent(image.rectTransform);
        return go;
    }

    public static GameObject MakeMainMenuScreen(Canvas canvas, string backgroundResource)
    {
        GameObject go = new GameObject("MainMenu");
        go.transform.SetParent(canvas.transform, false);
        RectTransform root = go.AddComponent<RectTransform>();
        StretchToParent(root);

        Texture2D background = Resources.Load<Texture2D>(backgroundResource);
        if (background != null)
        {
            GameObject backgroundObject = new GameObject("Background");
            backgroundObject.transform.SetParent(go.transform, false);
            Image backgroundImage = backgroundObject.AddComponent<Image>();
            backgroundImage.sprite = Sprite.Create(background, new Rect(0f, 0f, background.width, background.height), new Vector2(.5f, .5f));
            backgroundImage.type = Image.Type.Simple;
            backgroundImage.raycastTarget = false;
            StretchToParent(backgroundImage.rectTransform);

            AspectRatioFitter fitter = backgroundObject.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fitter.aspectRatio = (float)background.width / background.height;
        }

        GameObject shade = new GameObject("CinematicShade");
        shade.transform.SetParent(go.transform, false);
        Image shadeImage = shade.AddComponent<Image>();
        shadeImage.color = new Color(.018f, .008f, .004f, .35f);
        shadeImage.raycastTarget = false;
        StretchToParent(shadeImage.rectTransform);
        return go;
    }

    public static GameObject MakePanel(Transform parent, string name, Vector2 anchor, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = color;

        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = anchor;
        rect.sizeDelta = size;

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(.73f, .43f, .16f, .45f);
        outline.effectDistance = new Vector2(2f, -2f);
        return go;
    }

    public static Text AddTitle(Transform parent, string value, Vector2 position, int size, MenuTextStyle style = MenuTextStyle.Normal, Vector2? customSize = null)
    {
        GameObject go = new GameObject(string.IsNullOrEmpty(value) ? "Text" : value);
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = size;
        text.fontStyle = style == MenuTextStyle.Muted ? FontStyle.Normal : FontStyle.Bold;
        text.color = TextColor(style);
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.raycastTarget = false;

        RectTransform rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = customSize ?? new Vector2(1000, 80);
        return text;
    }

    public static void AddDivider(Transform parent, Vector2 position, float width)
    {
        GameObject go = new GameObject("Divider");
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = new Color(.82f, .49f, .19f, .55f);
        image.raycastTarget = false;

        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(width, 2f);
    }

    public static Button AddButton(Transform parent, string label, Vector2 position, UnityAction action, Vector2? customSize = null, MenuButtonStyle style = MenuButtonStyle.Default)
    {
        GameObject go = new GameObject(string.IsNullOrEmpty(label) ? "Button" : label);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();

        Sprite art = GetIllustratedButtonSprite();
        if (art != null)
        {
            image.sprite = art;
            image.type = Image.Type.Simple;
            image.preserveAspect = false;
            image.color = ButtonTint(style);
        }
        else
        {
            image.color = ButtonColor(style);
        }

        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.transition = Selectable.Transition.ColorTint;

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.08f, 1.05f, .94f, 1f);
        colors.pressedColor = new Color(.78f, .72f, .62f, 1f);
        colors.selectedColor = new Color(1.04f, 1f, .88f, 1f);
        colors.disabledColor = new Color(.45f, .45f, .45f, .65f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = .08f;
        button.colors = colors;
        button.onClick.AddListener(action);

        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = customSize ?? new Vector2(430, 64);

        Shadow shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color(.04f, .015f, .006f, .72f);
        shadow.effectDistance = new Vector2(0f, -5f);

        Text text = AddTitle(go.transform, label, Vector2.zero, style == MenuButtonStyle.Highlight ? 25 : 21, MenuTextStyle.Button);
        text.color = style == MenuButtonStyle.Highlight
            ? new Color(.20f, .075f, .015f, 1f)
            : style == MenuButtonStyle.Ghost
                ? new Color(.22f, .08f, .025f, 1f)
                : new Color(.18f, .065f, .02f, 1f);

        RectTransform textRect = text.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = textRect.offsetMax = Vector2.zero;
        textRect.pivot = new Vector2(.5f, .5f);
        return button;
    }

    static Sprite GetIllustratedButtonSprite()
    {
        if (illustratedButtonSprite != null) return illustratedButtonSprite;
        Texture2D texture = Resources.Load<Texture2D>(IllustratedButtonResource);
        if (texture == null) return null;
        illustratedButtonSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(.5f, .5f), 100f);
        return illustratedButtonSprite;
    }

    static void StretchToParent(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }

    static Color TextColor(MenuTextStyle style)
    {
        switch (style)
        {
            case MenuTextStyle.Logo: return new Color(1f, .68f, .15f, 1f);
            case MenuTextStyle.Subtitle: return new Color(1f, .88f, .58f, 1f);
            case MenuTextStyle.Muted: return new Color(.90f, .81f, .70f, .95f);
            default: return new Color(.98f, .94f, .86f, 1f);
        }
    }

    static Color ButtonTint(MenuButtonStyle style)
    {
        switch (style)
        {
            case MenuButtonStyle.Highlight: return new Color(1f, .82f, .33f, 1f);
            case MenuButtonStyle.Stone: return new Color(.88f, .72f, .49f, 1f);
            case MenuButtonStyle.Ghost: return new Color(.70f, .50f, .32f, 1f);
            default: return new Color(.82f, .62f, .36f, 1f);
        }
    }

    static Color ButtonColor(MenuButtonStyle style)
    {
        switch (style)
        {
            case MenuButtonStyle.Highlight: return new Color(.72f, .28f, .08f, .98f);
            case MenuButtonStyle.Stone: return new Color(.72f, .56f, .39f, .98f);
            case MenuButtonStyle.Ghost: return new Color(.18f, .08f, .035f, .92f);
            default: return new Color(.32f, .18f, .09f, .98f);
        }
    }
}
