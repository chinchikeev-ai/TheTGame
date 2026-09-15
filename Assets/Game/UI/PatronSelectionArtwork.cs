using System;
using UnityEngine;
using UnityEngine.UI;

// Presentation only: the existing pre-map controller owns gift application and map start.
public sealed class PatronSelectionArtwork : MonoBehaviour
{
    RectTransform content;
    readonly Outline[] highlights = new Outline[4];
    readonly GameObject[] translations = new GameObject[4];
    readonly Text[] selectionLabels = new Text[4];
    readonly DivineGiftType[] gifts = { DivineGiftType.Athena, DivineGiftType.Ares, DivineGiftType.Apollo, DivineGiftType.Poseidon };
    Button confirm;
    Text heading, backLabel, confirmLabel;
    int selected = -1;
    Action<DivineGiftType> onConfirm;
    public Text DifficultyLabel { get; private set; }

    public void Build(Action back, Action<DivineGiftType> choose)
    {
        onConfirm = choose;
        var background = Node("GodsBackground", transform).gameObject.AddComponent<RawImage>();
        background.texture = Resources.Load<Texture2D>("PatronSelection/gods_background");
        Stretch(background.rectTransform);
        var fit = background.gameObject.AddComponent<AspectRatioFitter>();
        fit.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        fit.aspectRatio = 16f / 9f;
        content = Node("GodsLayout", transform);
        content.sizeDelta = new Vector2(1600, 900);
        heading = Label("Heading", content, new Vector2(0, 405), new Vector2(1250, 65), 38);
        DifficultyLabel = Label("Difficulty", content, new Vector2(0, 360), new Vector2(900, 30), 18);
        string[] files = { "Gods_Afina", "Gods_Ares", "Gods_Apollo", "Gods_Poseidon" };
        string[] names = { "ATHENA", "ARES", "APOLLO", "POSEIDON" };
        string[] english = { "GATE +2 MAX HP\nIMMEDIATE", "HECTOR & TOWERS\n+10% DAMAGE / WHOLE MAP", "+50 STARTING GOLD\nONE TIME", "ENEMIES -10% SPEED\nWHOLE MAP" };
        float[] nameY = { -84, -67, -64, -67 };
        float[] nameWidth = { 190, 205, 233, 247 };
        float[] infoY = { -130, -115, -119, -123 };
        for (int i = 0; i < gifts.Length; i++)
        {
            int index = i;
            var rect = Node("Patron_" + gifts[i], content);
            rect.sizeDelta = new Vector2(490, 367.5f);
            rect.anchoredPosition = new Vector2(i % 2 == 0 ? -255 : 255, i < 2 ? 164 : -210);
            var art = rect.gameObject.AddComponent<RawImage>();
            art.texture = Resources.Load<Texture2D>("PatronSelection/" + files[i]);
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = art;
            var colors = button.colors;
            colors.highlightedColor = new Color(1f, .94f, .75f);
            colors.pressedColor = new Color(.8f, .7f, .5f);
            button.colors = colors;
            button.onClick.AddListener(() => Select(index));
            highlights[i] = rect.gameObject.AddComponent<Outline>();
            highlights[i].effectColor = new Color(1f, .8f, .15f);
            highlights[i].effectDistance = new Vector2(4, -4);
            highlights[i].enabled = false;
            var translation = Node("EnglishCaption", rect);
            var namePlate = Node("NamePlate", translation);
            namePlate.anchoredPosition = new Vector2(0, nameY[i]);
            namePlate.sizeDelta = new Vector2(nameWidth[i], 26);
            var parchment = namePlate.gameObject.AddComponent<Image>();
            parchment.color = new Color(.86f, .64f, .36f);
            parchment.raycastTarget = false;
            var nameLabel = Label("Name", namePlate, Vector2.zero, namePlate.sizeDelta, 22);
            nameLabel.text = names[i];
            nameLabel.color = new Color(.12f, .055f, .015f);
            nameLabel.GetComponent<Shadow>().enabled = false;
            var infoPlate = Node("BonusPlate", translation);
            infoPlate.anchoredPosition = new Vector2(50, infoY[i]);
            infoPlate.sizeDelta = new Vector2(288, 54);
            var plate = infoPlate.gameObject.AddComponent<Image>();
            plate.color = new Color(.12f, .085f, .06f);
            plate.raycastTarget = false;
            Label("Caption", infoPlate, Vector2.zero, new Vector2(280, 50), 16).text = english[i];
            translations[i] = translation.gameObject;
            selectionLabels[i] = Label("Selected", rect, new Vector2(0, 142), new Vector2(240, 30), 20);
            selectionLabels[i].gameObject.SetActive(false);
        }
        var backButton = Command("Back", new Vector2(-635, -403), new Vector2(260, 62), back);
        backLabel = backButton.GetComponentInChildren<Text>();
        confirm = Command("ConfirmPatron", new Vector2(620, -403), new Vector2(310, 62), Confirm);
        confirmLabel = confirm.GetComponentInChildren<Text>();
        ResetSelection();
        RefreshLayout();
    }

    public void ResetSelection()
    {
        selected = -1;
        foreach (var highlight in highlights) if (highlight != null) highlight.enabled = false;
        foreach (var label in selectionLabels) if (label != null) label.gameObject.SetActive(false);
        if (confirm != null) confirm.interactable = false;
    }

    void Select(int index)
    {
        selected = index;
        for (int i = 0; i < highlights.Length; i++)
        {
            highlights[i].enabled = i == selected;
            selectionLabels[i].gameObject.SetActive(i == selected);
        }
        confirm.interactable = true;
    }

    void Confirm()
    {
        if (selected < 0 || !confirm.interactable) return;
        confirm.interactable = false;
        onConfirm?.Invoke(gifts[selected]);
        if (gameObject.activeInHierarchy) confirm.interactable = true;
    }

    void LateUpdate() => RefreshLayout();

    void RefreshLayout()
    {
        if (content == null) return;
        var available = ((RectTransform)transform).rect.size;
        content.localScale = Vector3.one * Mathf.Min(available.x / 1600f, available.y / 900f);
        ApplyLanguage(GameLanguage.Russian);
    }

    void ApplyLanguage(bool russian)
    {
        heading.text = russian ? "ВЫБОР БОГА-ПОКРОВИТЕЛЯ" : "CHOOSE A PATRON GOD";
        backLabel.text = russian ? "НАЗАД" : "BACK";
        confirmLabel.text = russian ? "ПОДТВЕРДИТЬ ВЫБОР" : "CONFIRM CHOICE";
        foreach (var label in selectionLabels) label.text = russian ? "ВЫБРАНО" : "SELECTED";
        foreach (var translation in translations) translation.SetActive(!russian);
    }

    Button Command(string name, Vector2 position, Vector2 size, Action action)
    {
        var rect = Node(name, content);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        var plate = rect.gameObject.AddComponent<Image>();
        plate.color = new Color(.34f, .055f, .035f);
        var edge = rect.gameObject.AddComponent<Outline>();
        edge.effectColor = new Color(.95f, .7f, .3f);
        edge.effectDistance = new Vector2(3, -3);
        var button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = plate;
        button.onClick.AddListener(() => action());
        Label("Label", rect, Vector2.zero, size - new Vector2(20, 8), 22);
        return button;
    }

    static Text Label(string name, Transform parent, Vector2 position, Vector2 size, int fontSize)
    {
        var rect = Node(name, parent);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        var text = rect.gameObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(1f, .88f, .58f);
        text.raycastTarget = false;
        var shadow = rect.gameObject.AddComponent<Shadow>();
        shadow.effectDistance = new Vector2(2, -2);
        return text;
    }

    static RectTransform Node(string name, Transform parent)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        return rect;
    }

    static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }
}
