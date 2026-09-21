using System;
using UnityEngine;
using UnityEngine.UI;

// Illustrated pre-map difficulty screen. Gameplay difficulty application remains owned by
// PreMapPatronSelectionPresentation / CampaignController.
public sealed class DifficultySelectionArtwork : MonoBehaviour
{
    readonly CampaignDifficulty[] difficulties =
    {
        CampaignDifficulty.Story,
        CampaignDifficulty.Strategos,
        CampaignDifficulty.Legendary
    };

    readonly string[] portraitResources =
    {
        "PatronSelection/Gods_Apollo",
        "HectorHud/Portrait",
        "PatronSelection/Gods_Ares"
    };

    readonly Color[] headerColors =
    {
        new Color(.035f, .31f, .58f, 1f),
        new Color(.025f, .34f, .25f, 1f),
        new Color(.58f, .055f, .045f, 1f)
    };

    readonly Button[] cards = new Button[3];
    readonly Outline[] selections = new Outline[3];
    readonly Text[] selectedLabels = new Text[3];

    RectTransform content;
    Button next;
    Text heading;
    Text subtitle;
    Text backLabel;
    Text nextLabel;
    CampaignDifficulty selected;
    bool hasSelection;
    Action onBack;
    Action<CampaignDifficulty> onConfirm;

    public CampaignDifficulty SelectedDifficulty => selected;
    public Button NextButton => next;

    public void Build(Action back, Action<CampaignDifficulty> confirm, CampaignDifficulty initial)
    {
        onBack = back;
        onConfirm = confirm;

        BuildBackground();

        content = Node("DifficultyLayout", transform);
        content.sizeDelta = new Vector2(1600f, 900f);

        BuildHeader();

        float[] x = { -445f, 0f, 445f };
        for (int i = 0; i < difficulties.Length; i++)
            BuildCard(i, new Vector2(x[i], -28f));

        Button backButton = Command(
            "Back",
            new Vector2(-655f, -400f),
            new Vector2(300f, 74f),
            () => onBack?.Invoke(),
            new Color(.20f, .19f, .17f, .98f));
        backLabel = backButton.GetComponentInChildren<Text>(true);

        next = Command(
            "Next",
            new Vector2(635f, -400f),
            new Vector2(390f, 82f),
            Confirm,
            new Color(.58f, .055f, .04f, 1f));
        nextLabel = next.GetComponentInChildren<Text>(true);

        SetSelection(initial);
        RefreshLanguage();
        RefreshLayout();
    }

    public void SetSelection(CampaignDifficulty difficulty)
    {
        selected = difficulty;
        hasSelection = true;
        for (int i = 0; i < difficulties.Length; i++)
        {
            bool active = difficulties[i] == selected;
            if (selections[i] != null) selections[i].enabled = active;
            if (selectedLabels[i] != null) selectedLabels[i].gameObject.SetActive(active);
        }
        if (next != null) next.interactable = true;
    }

    void BuildBackground()
    {
        Texture2D texture = Resources.Load<Texture2D>("ChapterSelect/Background");
        GameObject go = new GameObject("DifficultyBackground", typeof(RectTransform), typeof(RawImage));
        go.transform.SetParent(transform, false);
        RawImage image = go.GetComponent<RawImage>();
        image.texture = texture;
        image.color = texture != null ? Color.white : new Color(.18f, .34f, .42f, 1f);
        image.raycastTarget = false;
        Stretch(image.rectTransform);

        AspectRatioFitter fitter = go.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        fitter.aspectRatio = 4f / 3f;

        GameObject wash = new GameObject("DifficultyWarmWash", typeof(RectTransform), typeof(Image));
        wash.transform.SetParent(transform, false);
        Image washImage = wash.GetComponent<Image>();
        washImage.color = new Color(.95f, .77f, .42f, .08f);
        washImage.raycastTarget = false;
        Stretch(washImage.rectTransform);
    }

    void BuildHeader()
    {
        Sprite parchment = LoadSprite("ChapterSelect/Parchment");

        GameObject titlePlate = new GameObject("DifficultyTitlePlate", typeof(RectTransform), typeof(Image));
        titlePlate.transform.SetParent(content, false);
        Image titleImage = titlePlate.GetComponent<Image>();
        titleImage.sprite = parchment;
        titleImage.type = parchment != null ? Image.Type.Sliced : Image.Type.Simple;
        titleImage.color = parchment != null ? Color.white : new Color(.84f, .68f, .42f, .98f);
        titleImage.raycastTarget = false;
        RectTransform titleRect = titleImage.rectTransform;
        titleRect.anchorMin = titleRect.anchorMax = titleRect.pivot = new Vector2(.5f, .5f);
        titleRect.anchoredPosition = new Vector2(0f, 382f);
        titleRect.sizeDelta = new Vector2(1040f, 116f);

        heading = Label("Heading", titlePlate.transform, new Vector2(0f, 10f), new Vector2(940f, 70f), 52);
        heading.color = new Color(.28f, .025f, .018f, 1f);

        GameObject subtitlePlate = new GameObject("DifficultySubtitlePlate", typeof(RectTransform), typeof(Image));
        subtitlePlate.transform.SetParent(content, false);
        Image subImage = subtitlePlate.GetComponent<Image>();
        subImage.sprite = parchment;
        subImage.type = parchment != null ? Image.Type.Sliced : Image.Type.Simple;
        subImage.color = parchment != null ? new Color(.96f, .92f, .80f, 1f) : new Color(.78f, .65f, .44f, .98f);
        subImage.raycastTarget = false;
        RectTransform subRect = subImage.rectTransform;
        subRect.anchorMin = subRect.anchorMax = subRect.pivot = new Vector2(.5f, .5f);
        subRect.anchoredPosition = new Vector2(0f, 317f);
        subRect.sizeDelta = new Vector2(850f, 58f);

        subtitle = Label("Subtitle", subtitlePlate.transform, Vector2.zero, new Vector2(780f, 42f), 22);
        subtitle.color = new Color(.16f, .085f, .035f, 1f);
    }

    void BuildCard(int index, Vector2 position)
    {
        CampaignDifficulty difficulty = difficulties[index];

        GameObject card = new GameObject(
            "Difficulty_" + difficulty,
            typeof(RectTransform),
            typeof(Image),
            typeof(Button),
            typeof(Outline));
        card.transform.SetParent(content, false);

        RectTransform rect = card.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(405f, 575f);

        Image surface = card.GetComponent<Image>();
        surface.sprite = LoadSprite("ChapterSelect/Parchment");
        surface.type = surface.sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        surface.color = surface.sprite != null ? Color.white : new Color(.88f, .76f, .55f, 1f);

        Outline border = card.GetComponent<Outline>();
        border.effectColor = new Color(.23f, .11f, .035f, 1f);
        border.effectDistance = new Vector2(4f, -4f);

        Button button = card.GetComponent<Button>();
        button.targetGraphic = surface;
        button.onClick.AddListener(() => SetSelection(difficulty));
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.04f, 1.01f, .91f, 1f);
        colors.pressedColor = new Color(.84f, .78f, .66f, 1f);
        colors.selectedColor = Color.white;
        button.colors = colors;
        card.AddComponent<MenuButtonFeedback>();
        cards[index] = button;

        selections[index] = card.AddComponent<Outline>();
        selections[index].effectColor = new Color(1f, .78f, .12f, 1f);
        selections[index].effectDistance = new Vector2(8f, -8f);
        selections[index].enabled = false;

        GameObject header = new GameObject("Header", typeof(RectTransform), typeof(Image));
        header.transform.SetParent(card.transform, false);
        Image headerImage = header.GetComponent<Image>();
        headerImage.color = headerColors[index];
        headerImage.raycastTarget = false;
        RectTransform headerRect = headerImage.rectTransform;
        headerRect.anchorMin = headerRect.anchorMax = headerRect.pivot = new Vector2(.5f, 1f);
        headerRect.anchoredPosition = new Vector2(0f, -9f);
        headerRect.sizeDelta = new Vector2(378f, 82f);

        Text title = Label("Title", header.transform, Vector2.zero, new Vector2(350f, 64f), 35);
        title.text = DifficultyTitle(difficulty);
        title.color = Color.white;

        GameObject portraitFrame = new GameObject("PortraitFrame", typeof(RectTransform), typeof(Image), typeof(RectMask2D));
        portraitFrame.transform.SetParent(card.transform, false);
        Image frame = portraitFrame.GetComponent<Image>();
        frame.color = new Color(.20f, .11f, .045f, 1f);
        frame.raycastTarget = false;
        RectTransform frameRect = frame.rectTransform;
        frameRect.anchorMin = frameRect.anchorMax = frameRect.pivot = new Vector2(.5f, 1f);
        frameRect.anchoredPosition = new Vector2(0f, -96f);
        frameRect.sizeDelta = new Vector2(350f, 245f);

        GameObject portraitObject = new GameObject("Portrait", typeof(RectTransform), typeof(RawImage));
        portraitObject.transform.SetParent(portraitFrame.transform, false);
        RawImage portrait = portraitObject.GetComponent<RawImage>();
        portrait.texture = Resources.Load<Texture2D>(portraitResources[index]);
        portrait.color = portrait.texture != null ? Color.white : headerColors[index];
        portrait.raycastTarget = false;
        Stretch(portrait.rectTransform);

        Text description = Label(
            "Description",
            card.transform,
            new Vector2(0f, -98f),
            new Vector2(330f, 90f),
            21);
        description.text = DifficultyDescription(difficulty);
        description.color = new Color(.15f, .075f, .028f, 1f);

        Text goldCaption = Label(
            "GoldCaption",
            card.transform,
            new Vector2(-30f, -162f),
            new Vector2(205f, 30f),
            18);
        goldCaption.text = GameLanguage.T("STARTING GOLD", "СТАРТОВОЕ ЗОЛОТО");
        goldCaption.color = new Color(.16f, .075f, .025f, 1f);

        GameObject coin = new GameObject("Coin", typeof(RectTransform), typeof(Image));
        coin.transform.SetParent(card.transform, false);
        Image coinImage = coin.GetComponent<Image>();
        coinImage.color = new Color(1f, .65f, .05f, 1f);
        coinImage.raycastTarget = false;
        RectTransform coinRect = coinImage.rectTransform;
        coinRect.anchorMin = coinRect.anchorMax = coinRect.pivot = new Vector2(.5f, .5f);
        coinRect.anchoredPosition = new Vector2(-118f, -205f);
        coinRect.sizeDelta = new Vector2(58f, 58f);
        Outline coinEdge = coin.AddComponent<Outline>();
        coinEdge.effectColor = new Color(.38f, .16f, .015f, 1f);
        coinEdge.effectDistance = new Vector2(3f, -3f);

        Text gold = Label(
            "StartingGold",
            card.transform,
            new Vector2(45f, -205f),
            new Vector2(210f, 68f),
            44);
        gold.text = DifficultyRules.StartingGold(difficulty).ToString();
        gold.color = new Color(.30f, .025f, .018f, 1f);

        GameObject mottoPlate = new GameObject("MottoPlate", typeof(RectTransform), typeof(Image));
        mottoPlate.transform.SetParent(card.transform, false);
        Image mottoImage = mottoPlate.GetComponent<Image>();
        mottoImage.color = new Color(.78f, .65f, .42f, .32f);
        mottoImage.raycastTarget = false;
        RectTransform mottoRect = mottoImage.rectTransform;
        mottoRect.anchorMin = mottoRect.anchorMax = mottoRect.pivot = new Vector2(.5f, .5f);
        mottoRect.anchoredPosition = new Vector2(0f, -252f);
        mottoRect.sizeDelta = new Vector2(340f, 54f);

        Text motto = Label("Motto", mottoPlate.transform, Vector2.zero, new Vector2(316f, 46f), 18);
        motto.text = DifficultyMotto(difficulty);
        motto.color = new Color(.20f, .10f, .035f, 1f);

        selectedLabels[index] = Label(
            "Selected",
            card.transform,
            new Vector2(0f, 267f),
            new Vector2(210f, 28f),
            17);
        selectedLabels[index].text = GameLanguage.T("SELECTED", "ВЫБРАНО");
        selectedLabels[index].color = new Color(1f, .84f, .22f, 1f);
        selectedLabels[index].gameObject.SetActive(false);
    }

    void Confirm()
    {
        if (!hasSelection || next == null || !next.interactable) return;
        next.interactable = false;
        onConfirm?.Invoke(selected);
        if (gameObject.activeInHierarchy) next.interactable = true;
    }

    void LateUpdate()
    {
        RefreshLanguage();
        RefreshLayout();
    }

    void RefreshLanguage()
    {
        if (heading == null) return;
        heading.text = GameLanguage.T("CHOOSE DIFFICULTY", "ВЫБЕРИТЕ СЛОЖНОСТЬ");
        subtitle.text = GameLanguage.T(
            "ONE TROY. THREE TRIALS. WHAT LEGEND WILL BE YOURS?",
            "ОДНА ТРОЯ. ТРИ ИСПЫТАНИЯ. КАКАЯ ЛЕГЕНДА БУДЕТ ТВОЕЙ?");
        if (backLabel != null) backLabel.text = GameLanguage.T("BACK", "НАЗАД");
        if (nextLabel != null) nextLabel.text = GameLanguage.T("NEXT  >", "ДАЛЕЕ  >");

        for (int i = 0; i < difficulties.Length; i++)
        {
            Transform card = content != null ? content.Find("Difficulty_" + difficulties[i]) : null;
            if (card == null) continue;
            Text title = card.Find("Header/Title")?.GetComponent<Text>();
            Text description = card.Find("Description")?.GetComponent<Text>();
            Text goldCaption = card.Find("GoldCaption")?.GetComponent<Text>();
            Text motto = card.Find("MottoPlate/Motto")?.GetComponent<Text>();
            if (title != null) title.text = DifficultyTitle(difficulties[i]);
            if (description != null) description.text = DifficultyDescription(difficulties[i]);
            if (goldCaption != null) goldCaption.text = GameLanguage.T("STARTING GOLD", "СТАРТОВОЕ ЗОЛОТО");
            if (motto != null) motto.text = DifficultyMotto(difficulties[i]);
            if (selectedLabels[i] != null)
                selectedLabels[i].text = GameLanguage.T("SELECTED", "ВЫБРАНО");
        }
    }

    string DifficultyTitle(CampaignDifficulty difficulty)
    {
        switch (difficulty)
        {
            case CampaignDifficulty.Story: return GameLanguage.T("STORY", "ИСТОРИЯ");
            case CampaignDifficulty.Legendary: return GameLanguage.T("LEGEND", "ЛЕГЕНДА");
            default: return GameLanguage.T("STRATEGOS", "СТРАТЕГ");
        }
    }

    string DifficultyDescription(CampaignDifficulty difficulty)
    {
        switch (difficulty)
        {
            case CampaignDifficulty.Story:
                return GameLanguage.T(
                    "For players who want to enjoy the battle and Troy's story.",
                    "Для тех, кто хочет насладиться игрой и узнать историю Трои.");
            case CampaignDifficulty.Legendary:
                return GameLanguage.T(
                    "For those worthy of entering the legends.",
                    "Только для тех, кто достоин войти в легенды.");
            default:
                return GameLanguage.T(
                    "A true challenge for experienced defenders.",
                    "Настоящий вызов для опытных защитников.");
        }
    }

    string DifficultyMotto(CampaignDifficulty difficulty)
    {
        switch (difficulty)
        {
            case CampaignDifficulty.Story:
                return GameLanguage.T("EVEN THE GODS SMILE HERE!", "ЗДЕСЬ ДАЖЕ БОГИ УЛЫБАЮТСЯ ТЕБЕ!");
            case CampaignDifficulty.Legendary:
                return GameLanguage.T("PAIN. BATTLES. IMMORTALITY!", "БОЛЬ. БИТВЫ. БЕССМЕРТИЕ!");
            default:
                return GameLanguage.T("THINK. PLAN. WIN!", "ДУМАЙ. ПЛАНИРУЙ. ПОБЕЖДАЙ!");
        }
    }

    void RefreshLayout()
    {
        if (content == null) return;
        Vector2 available = ((RectTransform)transform).rect.size;
        float scale = Mathf.Min(available.x / 1600f, available.y / 900f);
        content.localScale = Vector3.one * scale;
    }

    Button Command(string name, Vector2 position, Vector2 size, Action action, Color color)
    {
        RectTransform rect = Node(name, content);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image plate = rect.gameObject.AddComponent<Image>();
        plate.color = color;

        Outline edge = rect.gameObject.AddComponent<Outline>();
        edge.effectColor = new Color(.90f, .63f, .24f, 1f);
        edge.effectDistance = new Vector2(4f, -4f);

        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = plate;
        button.onClick.AddListener(() => action?.Invoke());
        rect.gameObject.AddComponent<MenuButtonFeedback>();

        Text label = Label("Label", rect, Vector2.zero, size - new Vector2(24f, 12f), 31);
        label.color = new Color(1f, .91f, .70f, 1f);
        return button;
    }

    static Text Label(string name, Transform parent, Vector2 position, Vector2 size, int fontSize)
    {
        RectTransform rect = Node(name, parent);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Text text = rect.gameObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = Mathf.Max(12, fontSize - 10);
        text.resizeTextMaxSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.color = new Color(.16f, .075f, .025f, 1f);
        text.raycastTarget = false;

        Shadow shadow = rect.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, .28f);
        shadow.effectDistance = new Vector2(1.5f, -1.5f);
        return text;
    }

    static RectTransform Node(string name, Transform parent)
    {
        RectTransform rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        return rect;
    }

    static Sprite LoadSprite(string resource)
    {
        Texture2D texture = Resources.Load<Texture2D>(resource);
        if (texture == null) return null;
        return Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(.5f, .5f),
            100f);
    }

    static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }
}
