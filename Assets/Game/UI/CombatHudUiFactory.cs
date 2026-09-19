using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class CombatHudUiFactory
{
    static Font runtimeFont;

    public static GameObject Panel(Transform parent, string name, Vector2 pos, Vector2 size, Color color, Vector2 anchor, Vector2 pivot)
    {
        GameObject panel = Panel(parent, name, pos, size, anchor, pivot);
        panel.GetComponent<Image>().color = color;
        return panel;
    }

    public static GameObject Panel(Transform parent, string name, Vector2 pos, Vector2 size, Vector2 anchor, Vector2 pivot)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.sprite = TroyHudArt.Panel();
        image.type = Image.Type.Sliced;
        image.color = Color.white;
        image.raycastTarget = false;
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

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
        image.sprite = TroyHudArt.Panel();
        image.type = Image.Type.Sliced;
        image.color = primary ? new Color(.78f, .28f, .08f, 1f) : new Color(.66f, .42f, .18f, 1f);

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
        text.raycastTarget = false;

        RectTransform rt = text.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return text;
    }

    static Font RuntimeFont()
    {
        if (runtimeFont == null) runtimeFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return runtimeFont;
    }
}
