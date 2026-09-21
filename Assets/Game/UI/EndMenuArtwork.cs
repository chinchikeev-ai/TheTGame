using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Artwork only; the menu controller supplies live results and owns all actions.
public sealed class EndMenuArtwork : MonoBehaviour
{
    readonly List<Sprite> ownedSprites = new List<Sprite>();
    Sprite victoryBackground, defeatBackground;
    Image background;
    RectTransform layout;
    Text quote;

    public void Apply(RectTransform panel, Text title, Text chapter, Text summary)
    {
        layout = panel;
        layout.sizeDelta = new Vector2(1000, 940);
        layout.anchoredPosition = Vector2.zero;
        layout.GetComponent<Image>().enabled = false;
        foreach (var outline in layout.GetComponents<Outline>()) outline.enabled = false;
        foreach (Transform child in layout)
            if (child.GetComponent<Text>() == null && child.GetComponent<Button>() == null)
                child.gameObject.SetActive(false);

        var blocker = GetComponent<Image>();
        blocker.sprite = null;
        blocker.color = Color.black;
        blocker.raycastTarget = true;
        victoryBackground = Load("Victory");
        defeatBackground = Load("Defeat");
        background = Picture("ResultBackground", transform, Vector2.zero, Vector2.one, victoryBackground);
        background.transform.SetAsFirstSibling();
        var banner = Picture("ResultBanner", layout, new Vector2(0, 80), new Vector2(720, 730), Load("Banner"));
        banner.transform.SetAsFirstSibling();

        ConfigureText(title, new Vector2(0, 270), new Vector2(580, 88), 60);
        title.color = new Color(1f, .85f, .4f);
        ConfigureText(chapter, new Vector2(0, 208), new Vector2(570, 34), 21);
        ConfigureText(summary, new Vector2(0, 23), new Vector2(590, 310), 20);
        summary.lineSpacing = 1.12f;
        quote = CombatHudUiFactory.Text(layout, "", new Vector2(0, -155), new Vector2(560, 48),
            20, new Color(1f, .83f, .56f), TextAnchor.MiddleCenter, FontStyle.Italic);

        Sprite buttonSprite = Load("Button");
        var buttons = panel.GetComponentsInChildren<Button>(true);
        string[] names = { "ResultRetry", "ResultChapters", "ResultMainMenu" };
        for (int i = 0; i < buttons.Length; i++)
        {
            var button = buttons[i];
            button.name = names[i];
            var image = button.GetComponent<Image>();
            image.sprite = buttonSprite;
            image.type = Image.Type.Simple;
            image.color = Color.white;
            image.raycastTarget = true;
            var rect = image.rectTransform;
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = new Vector2(-320 + i * 320, -375);
            rect.sizeDelta = new Vector2(310, 100);
            foreach (var outline in button.GetComponents<Outline>()) outline.enabled = false;
            var feedback = button.GetComponent<MenuButtonFeedback>();
            if (feedback != null) feedback.enabled = false;
            button.colors = ColorBlock.defaultColorBlock;
            var label = button.GetComponentInChildren<Text>(true);
            ConfigureText(label, Vector2.zero, new Vector2(238, 48), 24);
        }
        SetResult(true);
        Fit();
    }

    public void SetResult(bool victory)
    {
        if (background == null) return;
        background.sprite = victory ? victoryBackground : defeatBackground;
        quote.text = victory
            ? GameLanguage.T("Troy stands. Its defenders stand together.", "Троя стоит. Её защитники едины.")
            : GameLanguage.T("Even heroes fall. Their story continues.", "Даже герои падают. Но их история продолжается.");
    }

    void LateUpdate() => Fit();

    public void Fit()
    {
        if (layout == null || background == null || background.sprite == null) return;
        Vector2 size = ((RectTransform)transform).rect.size;
        layout.localScale = Vector3.one * Mathf.Min(size.x / 1060f, size.y / 960f);
        Vector2 art = background.sprite.rect.size;
        background.rectTransform.sizeDelta = art * Mathf.Max(size.x / art.x, size.y / art.y);
    }

    static void ConfigureText(Text text, Vector2 pos, Vector2 size, int fontSize)
    {
        text.rectTransform.anchorMin = text.rectTransform.anchorMax = text.rectTransform.pivot = new Vector2(.5f, .5f);
        text.rectTransform.anchoredPosition = pos;
        text.rectTransform.sizeDelta = size;
        text.fontSize = fontSize;
        text.resizeTextForBestFit = true;
        text.resizeTextMaxSize = fontSize;
        text.resizeTextMinSize = fontSize - 3;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(1f, .94f, .76f);
        text.raycastTarget = false;
    }

    Sprite Load(string name)
    {
        var texture = Resources.Load<Texture2D>("EndMenu/" + name);
        if (texture == null) { Debug.LogError("Missing EndMenu/" + name); return null; }
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
        ownedSprites.Add(sprite);
        return sprite;
    }

    static Image Picture(string name, Transform parent, Vector2 pos, Vector2 size, Sprite sprite)
    {
        return CombatHudUiFactory.Icon(parent, name, pos, size, sprite);
    }

    void OnDestroy()
    {
        foreach (var sprite in ownedSprites)
            if (sprite != null)
            {
                if (Application.isPlaying) Destroy(sprite);
                else DestroyImmediate(sprite);
            }
    }
}
