using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Only presentation: GameMenuController owns pause state and all button callbacks.
public sealed class PauseMenuArtwork : MonoBehaviour
{
    readonly List<Sprite> ownedSprites = new List<Sprite>();
    RectTransform card;

    public void Apply(RectTransform panel, bool russian)
    {
        card = panel;
        var buttons = card.GetComponentsInChildren<Button>(true);
        foreach (Transform child in card)
            if (child.GetComponent<Button>() == null) child.gameObject.SetActive(false);
        card.sizeDelta = new Vector2(800, 960);
        card.GetComponent<Image>().enabled = false;
        var outline = card.GetComponent<Outline>();
        if (outline != null) outline.enabled = false;
        var backdrop = GetComponent<Image>();
        var background = Resources.Load<Texture2D>("PauseMenu/GameMenuBackground");
        if (background != null)
        {
            var sprite = Sprite.Create(background, new Rect(0, 0, background.width, background.height), new Vector2(.5f, .5f));
            ownedSprites.Add(sprite);
            backdrop.sprite = sprite;
            backdrop.color = Color.white;
            backdrop.type = Image.Type.Simple;
        }
        else
        {
            backdrop.color = new Color(.012f, .019f, .025f, .72f);
        }
        backdrop.raycastTarget = true;

        var body = Picture("PauseBody", card, new Vector2(0, -105), new Vector2(660, 700), Resources.Load<Sprite>("HectorHud/Panel"));
        body.transform.SetAsFirstSibling();
        var header = Picture("PauseHeader", card, new Vector2(0, 275), new Vector2(800, 450), Load("Header", new Rect(0, .12f, 1, .76f)));
        if (!russian)
        {
            header.sprite = Resources.Load<Sprite>("HectorHud/Panel");
            header.rectTransform.sizeDelta = new Vector2(600, 155);
            Caption(header.transform, "PAUSED", Vector2.zero, new Vector2(500, 100), 48);
        }
        string[] names = { "Resume", "Settings", "Restart", "MainMenu", "Exit" };
        string[] labels = { "RESUME", "SETTINGS", "RESTART CHAPTER", "MAIN MENU", "EXIT" };
        for (int i = 0; i < buttons.Length && i < names.Length; i++)
        {
            var button = buttons[i];
            button.name = "Pause_" + names[i];
            var image = button.GetComponent<Image>();
            image.sprite = Load(names[i], new Rect(.025f, .18f, .95f, .64f));
            image.color = Color.white;
            image.type = Image.Type.Simple;
            var rect = image.rectTransform;
            rect.anchoredPosition = new Vector2(0, 76 - 101 * i);
            rect.sizeDelta = new Vector2(570, 98);
            foreach (var text in button.GetComponentsInChildren<Text>(true)) text.gameObject.SetActive(false);
            if (!russian)
            {
                image.sprite = Resources.Load<Sprite>("HectorHud/Panel");
                Caption(button.transform, labels[i], Vector2.zero, new Vector2(490, 60), 25);
            }
            button.transform.SetAsLastSibling();
        }
        Fit();
    }

    void LateUpdate() => Fit();

    void Fit()
    {
        if (card == null) return;
        var size = ((RectTransform)transform).rect.size;
        card.localScale = Vector3.one * Mathf.Min(1f, Mathf.Min(size.x / 840f, size.y / 1040f));
    }

    Sprite Load(string name, Rect uv)
    {
        var texture = Resources.Load<Texture2D>("PauseMenu/" + name);
        if (texture == null) { Debug.LogError("Missing PauseMenu/" + name); return null; }
        var sprite = Sprite.Create(texture, new Rect(uv.x * texture.width, uv.y * texture.height, uv.width * texture.width, uv.height * texture.height), new Vector2(.5f, .5f));
        ownedSprites.Add(sprite);
        return sprite;
    }

    static Image Picture(string name, Transform parent, Vector2 pos, Vector2 size, Sprite sprite)
    {
        var image = new GameObject(name, typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        image.transform.SetParent(parent, false);
        image.rectTransform.anchoredPosition = pos;
        image.rectTransform.sizeDelta = size;
        image.sprite = sprite;
        image.raycastTarget = false;
        return image;
    }

    static void Caption(Transform parent, string value, Vector2 pos, Vector2 size, int fontSize)
    {
        var plate = Picture("EnglishCaption", parent, pos, size, null);
        plate.color = Color.clear;
        var text = new GameObject("Caption", typeof(RectTransform), typeof(Text)).GetComponent<Text>();
        text.transform.SetParent(plate.transform, false);
        text.rectTransform.sizeDelta = size;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(1f, .91f, .64f);
        text.text = value;
        text.raycastTarget = false;
    }

    void OnDestroy()
    {
        foreach (var sprite in ownedSprites)
            if (Application.isPlaying) Destroy(sprite); else DestroyImmediate(sprite);
    }
}
