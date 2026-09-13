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
            StretchToParent(backgroundImage.rectTransform);

            AspectRatioFitter fitter = backgroundObject.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fitter.aspectRatio = (float)background.width / background.height;
        }

        GameObject shade = new GameObject("CinematicShade");
        shade.transform.SetParent(go.transform, false);
        Image shadeImage = shade.AddComponent<Image>();
        shadeImage.color = new Color(.018f, .008f, .004f, .29f);
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
        image.color = ButtonColor(style);

        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.transition = Selectable.Transition.ColorTint;

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, .95f, .86f, 1f);
        colors.pressedColor = new Color(.78f, .58f, .42f, 1f);
        colors.selectedColor = new Color(1f, .91f, .76f, 1f);
        colors.disabledColor = new Color(.45f, .45f, .45f, .65f);
        colors.fadeDuration = .12f;
        button.colors = colors;
        button.onClick.AddListener(action);

        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = customSize ?? new Vector2(430, 64);

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = style == MenuButtonStyle.Highlight
            ? new Color(1f, .52f, .15f, .75f)
            : new Color(.62f, .38f, .18f, .35f);
        outline.effectDistance = new Vector2(1f, -1f);

        Text text = AddTitle(go.transform, label, Vector2.zero, 21, MenuTextStyle.Button);
        text.color = style == MenuButtonStyle.Highlight
            ? new Color(1f, .90f, .55f, 1f)
            : style == MenuButtonStyle.Ghost
                ? new Color(.93f, .78f, .60f, 1f)
                : new Color(.17f, .07f, .03f, 1f);

        RectTransform textRect = text.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = textRect.offsetMax = Vector2.zero;
        textRect.pivot = new Vector2(.5f, .5f);
        return button;
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
            case MenuTextStyle.Logo: return new Color(1f, .62f, .18f, 1f);
            case MenuTextStyle.Subtitle: return new Color(1f, .84f, .56f, 1f);
            case MenuTextStyle.Muted: return new Color(.82f, .72f, .62f, .92f);
            default: return new Color(.97f, .92f, .84f, 1f);
        }
    }

    static Color ButtonColor(MenuButtonStyle style)
    {
        switch (style)
        {
            case MenuButtonStyle.Highlight: return new Color(.58f, .105f, .055f, .98f);
            case MenuButtonStyle.Stone: return new Color(.72f, .56f, .39f, .98f);
            case MenuButtonStyle.Ghost: return new Color(.10f, .055f, .03f, .82f);
            default: return new Color(.32f, .18f, .09f, .98f);
        }
    }
}
