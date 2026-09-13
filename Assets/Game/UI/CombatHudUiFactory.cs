using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class CombatHudUiFactory
{
    static Sprite coinSprite;
    static Font runtimeFont;

    public static GameObject Panel(Transform parent, string name, Vector2 pos, Vector2 size, Color color, Vector2 anchor, Vector2 pivot)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = color;
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(.67f, .36f, .13f, .42f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);
        return go;
    }

    public static Image Icon(Transform parent, string name, Vector2 pos, Vector2 size, Sprite sprite)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.sprite = sprite;
        image.raycastTarget = false;
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return image;
    }

    public static Button Button(Transform parent, string label, Vector2 pos, Vector2 size, UnityAction action, bool primary)
    {
        GameObject go = new GameObject(label);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = primary ? new Color(.55f, .11f, .045f, .98f) : new Color(.24f, .14f, .08f, .96f);

        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        go.AddComponent<MenuButtonFeedback>();
        go.AddComponent<MenuUiAudioFeedback>();
        Text(go.transform, label, Vector2.zero, size, 15,
            primary ? new Color(1f, .88f, .50f, 1f) : new Color(.94f, .84f, .70f, 1f),
            TextAnchor.MiddleCenter, FontStyle.Bold);
        return button;
    }

    public static Text Text(Transform parent, string value, Vector2 pos, Vector2 size, int fontSize, Color color, TextAnchor alignment, FontStyle style)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = RuntimeFont();
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = color;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;

        RectTransform rt = text.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return text;
    }

    public static Sprite CoinSprite()
    {
        if (coinSprite != null) return coinSprite;

        const int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            name = "RuntimeCoinIcon"
        };
        Color clear = new Color(0f, 0f, 0f, 0f);
        Color edge = new Color(.74f, .38f, .06f, 1f);
        Color gold = new Color(1f, .72f, .18f, 1f);
        Color shine = new Color(1f, .92f, .48f, 1f);
        Vector2 center = new Vector2((size - 1) * .5f, (size - 1) * .5f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                Color color = clear;
                if (distance < 29f) color = distance > 24f ? edge : gold;
                if (distance < 17f && x < 30 && y > 33) color = shine;
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        coinSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(.5f, .5f));
        coinSprite.name = "RuntimeCoinSprite";
        return coinSprite;
    }

    static Font RuntimeFont()
    {
        if (runtimeFont == null) runtimeFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return runtimeFont;
    }
}
