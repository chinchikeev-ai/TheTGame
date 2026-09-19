using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Presentation and selection only; campaign progress and starting a run remain external.
public sealed class ChapterSelectionArtwork : MonoBehaviour
{
    readonly List<Sprite> sprites = new List<Sprite>();
    readonly List<Image> nodes = new List<Image>();
    RectTransform composition;
    Text heading, description, status, startLabel;
    Button start;
    Action begin;
    Func<int, bool> unlocked;
    int selected = 1;
    bool russian;
    Sprite parchment;
    Sprite lockedNode, availableNode;
    public int SelectedChapter => selected;

    public void Build(Action onStart, Action onBack, Func<int, bool> isUnlocked, bool ru)
    {
        begin = onStart;
        unlocked = isUnlocked;
        russian = ru;
        parchment = Load("Parchment");
        var background = Picture(transform, "MapBackground", Load("Background"), new Rect(0, 0, 1448, 1086));
        background.rectTransform.anchorMin = Vector2.zero;
        background.rectTransform.anchorMax = Vector2.one;
        background.rectTransform.offsetMin = background.rectTransform.offsetMax = Vector2.zero;
        var fit = background.gameObject.AddComponent<AspectRatioFitter>();
        fit.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        fit.aspectRatio = 1448f / 1086f;
        background.enabled = false;
        GetComponent<Image>().color = new Color(.025f, .065f, .075f);
        composition = new GameObject("ChapterComposition", typeof(RectTransform)).GetComponent<RectTransform>();
        composition.SetParent(transform, false);
        composition.sizeDelta = new Vector2(1448, 1086);
        Picture(composition, "AlignedMap", background.sprite, new Rect(0, 0, 1448, 1086));
        lockedNode = Slice(new Rect(672, 279, 79, 79));
        availableNode = Slice(new Rect(557, 721, 86, 87));

        if (ru) Picture(composition, "CampaignHeading", Slice(new Rect(30, 5, 555, 177)), new Rect(30, 5, 555, 177));
        else
        {
            Picture(composition, "CampaignHeading", parchment, new Rect(35, 15, 540, 150));
            Label("CAMPAIGN", new Rect(60, 28, 490, 70), 48, TextAnchor.MiddleCenter);
            Label("ROAD TO TROY", new Rect(60, 100, 490, 45), 28, TextAnchor.MiddleCenter);
        }

        Vector2[] positions = { new Vector2(487, 586), new Vector2(600, 764), new Vector2(779, 625), new Vector2(773, 441), new Vector2(710, 319) };
        string[] ruNames = { "1. ВЫСАДКА", "2. У КРЕПОСТИ", "3. РАВНИНЫ", "4. СТЕНЫ", "5. ТРОЯ" };
        string[] enNames = { "1. LANDING", "2. FORTRESS", "3. PLAINS", "4. WALLS", "5. TROY" };
        for (int i = 0; i < positions.Length; i++)
        {
            int chapter = i + 1;
            var p = positions[i];
            Sprite icon = i == 0 ? Slice(new Rect(437, 536, 102, 101)) : lockedNode;
            var button = Command("Chapter" + chapter, icon, new Rect(p.x - 49, p.y - 49, 98, 98), () => SelectChapter(chapter));
            nodes.Add(button.GetComponent<Image>());
            var mask = new GameObject("NodeClip" + chapter, typeof(RectTransform), typeof(CanvasRenderer), typeof(ChapterNodeMask), typeof(Mask));
            mask.transform.SetParent(composition, false);
            Place(mask.GetComponent<RectTransform>(), new Rect(p.x - 49, p.y - 49, 98, 98));
            mask.GetComponent<Mask>().showMaskGraphic = false;
            mask.GetComponent<ChapterNodeMask>().raycastTarget = false;
            button.transform.SetParent(mask.transform, false);
            Place(button.GetComponent<RectTransform>(), new Rect(0, 0, 98, 98));
            Vector2 labelPos = i >= 3 ? new Vector2(p.x + 43, p.y - 37) : new Vector2(p.x - 105, p.y + 46);
            float labelWidth = i >= 3 ? 116 : 205;
            Picture(composition, "ChapterPlaque" + chapter, parchment, new Rect(labelPos.x, labelPos.y, labelWidth, 58));
            Label(ru ? ruNames[i] : enNames[i], new Rect(labelPos.x + 7, labelPos.y + 5, labelWidth - 14, 46), 24, TextAnchor.MiddleCenter);
        }
        Picture(composition, "ChapterDetails", parchment, new Rect(938, 203, 502, 829));
        heading = Label("", new Rect(975, 236, 427, 68), 38, TextAnchor.MiddleCenter);
        heading.color = new Color(.43f, .025f, .025f);
        heading.fontStyle = FontStyle.Bold;
        Picture(composition, "LandingIllustration", Slice(new Rect(978, 311, 426, 260)), new Rect(978, 311, 426, 260));
        description = Label("", new Rect(985, 580, 407, 112), 27, TextAnchor.UpperLeft);
        Picture(composition, "DurationIcon", Slice(new Rect(990, 678, 50, 53)), new Rect(990, 678, 50, 53));
        Picture(composition, "HeroIcon", Slice(new Rect(989, 733, 64, 62)), new Rect(989, 733, 64, 62));
        Label(ru ? "Герой: Гектор" : "Hero: Hector", new Rect(1065, 738, 327, 45), 28, TextAnchor.MiddleLeft);
        status = Label("", new Rect(1065, 693, 327, 42), 23, TextAnchor.MiddleLeft);
        start = Command("StartChapter", ru ? Slice(new Rect(979, 800, 427, 92)) : parchment, new Rect(979, 800, 427, 92), () => { if (selected == 1) begin?.Invoke(); });
        startLabel = Label("", new Rect(998, 817, 389, 58), 30, TextAnchor.MiddleCenter);
        startLabel.color = new Color(.35f, .03f, .015f);
        Label(ru ? "«Первый шаг врага — последний,\nесли мы едины!»\n— Гектор" : "\"Their first step is their last\nif we stand together.\"\n— Hector", new Rect(986, 904, 407, 95), 25, TextAnchor.MiddleCenter);
        Command("Back", ru ? Slice(new Rect(16, 949, 300, 116)) : parchment, new Rect(16, 949, 300, 116), onBack);
        if (!ru) Label("BACK", new Rect(50, 974, 230, 55), 32, TextAnchor.MiddleCenter);
        SelectChapter(1);
        Fit();
    }

    public void SelectChapter(int chapter)
    {
        selected = Mathf.Clamp(chapter, 1, 5);
        bool available = selected == 1;
        bool open = available || (unlocked != null && unlocked(selected));
        string[] names = russian ? new[] { "ВЫСАДКА", "У КРЕПОСТИ", "РАВНИНЫ", "СТЕНЫ", "ТРОЯ" } : new[] { "LANDING", "FORTRESS", "PLAINS", "WALLS", "TROY" };
        heading.text = (russian ? "ГЛАВА " : "CHAPTER ") + new[] { "I", "II", "III", "IV", "V" }[selected - 1] + " — " + names[selected - 1];
        description.text = available
            ? (russian ? "Греки высадились на берегах Трои. Остановите их, пока они не закрепились на пляже!" : "The Greeks have landed on Troy's shores. Stop them before they secure the beach!")
            : (open ? (russian ? "Глава открыта. Содержимое ещё в разработке." : "Chapter unlocked. Content is still in development.") : (russian ? "Продолжайте кампанию, чтобы открыть эту главу." : "Continue the campaign to unlock this chapter."));
        status.text = available ? (russian ? "Длительность: ~12 минут" : "Duration: ~12 minutes") : (open ? (russian ? "В разработке" : "In development") : (russian ? "Закрыто" : "Locked"));
        start.interactable = available;
        start.GetComponent<Image>().sprite = available && russian ? GetStartSprite() : parchment;
        startLabel.text = available ? (russian ? "" : "START CHAPTER") : (open ? (russian ? "СКОРО" : "COMING SOON") : (russian ? "ЗАКРЫТО" : "LOCKED"));
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].color = i + 1 == selected ? Color.white : new Color(.78f, .78f, .78f);
            if (i == 1) nodes[i].sprite = unlocked != null && unlocked(2) ? availableNode : lockedNode;
        }
    }

    Sprite startSprite;
    Sprite GetStartSprite() => startSprite != null ? startSprite : (startSprite = Slice(new Rect(979, 800, 427, 92)));
    void OnEnable() { if (heading != null) SelectChapter(selected); }
    void LateUpdate() => Fit();
    void Fit()
    {
        if (composition == null) return;
        var size = ((RectTransform)transform).rect.size;
        composition.localScale = Vector3.one * Mathf.Min(size.x / 1448f, size.y / 1086f);
    }
    Sprite Load(string name)
    {
        var texture = Resources.Load<Texture2D>("ChapterSelect/" + name);
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
        sprites.Add(sprite);
        return sprite;
    }
    Sprite Slice(Rect topLeftPixels)
    {
        var texture = Resources.Load<Texture2D>("ChapterSelect/ReferenceAtlas");
        float x = texture.width / 1448f, y = texture.height / 1086f;
        var rect = new Rect(topLeftPixels.x * x, (1086 - topLeftPixels.yMax) * y, topLeftPixels.width * x, topLeftPixels.height * y);
        var sprite = Sprite.Create(texture, rect, new Vector2(.5f, .5f));
        sprites.Add(sprite);
        return sprite;
    }
    static void Place(RectTransform rt, Rect r)
    {
        rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(r.x, -r.y);
        rt.sizeDelta = r.size;
    }
    static Image Picture(Transform parent, string name, Sprite sprite, Rect r)
    {
        var image = new GameObject(name, typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        image.transform.SetParent(parent, false);
        Place(image.rectTransform, r);
        image.sprite = sprite;
        image.raycastTarget = false;
        return image;
    }
    Button Command(string name, Sprite sprite, Rect r, Action action)
    {
        var image = Picture(composition, name, sprite, r);
        image.raycastTarget = true;
        var button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => action?.Invoke());
        return button;
    }
    Text Label(string value, Rect r, int size, TextAnchor alignment)
    {
        var text = new GameObject("Label", typeof(RectTransform), typeof(Text)).GetComponent<Text>();
        text.transform.SetParent(composition, false);
        Place(text.rectTransform, r);
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = size;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 18;
        text.resizeTextMaxSize = size;
        text.alignment = alignment;
        text.color = new Color(.12f, .065f, .025f);
        text.text = value;
        text.raycastTarget = false;
        return text;
    }
    void OnDestroy()
    {
        foreach (var sprite in sprites)
            if (Application.isPlaying) Destroy(sprite); else DestroyImmediate(sprite);
    }
}
