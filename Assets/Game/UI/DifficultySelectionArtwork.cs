using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Illustrated pre-map difficulty screen. Gameplay difficulty application remains owned by
// PreMapPatronSelectionPresentation / CampaignController.
public sealed class DifficultySelectionArtwork : MonoBehaviour
{
    const float DesignWidth = 1600f;
    const float DesignHeight = 900f;

    readonly CampaignDifficulty[] difficulties =
    {
        CampaignDifficulty.Story,
        CampaignDifficulty.Strategos,
        CampaignDifficulty.Legendary
    };

    readonly string[] portraitResources =
    {
        "DifficultySelection/Story",
        "DifficultySelection/Strategos",
        "DifficultySelection/Legendary"
    };

    readonly Color[] headerColors =
    {
        new Color(.025f, .30f, .58f, 1f),
        new Color(.018f, .33f, .23f, 1f),
        new Color(.63f, .045f, .035f, 1f)
    };

    readonly Button[] cards = new Button[3];
    readonly RectTransform[] cardRects = new RectTransform[3];
    readonly Image[] cardSurfaces = new Image[3];
    readonly Outline[] selections = new Outline[3];
    readonly Text[] selectedLabels = new Text[3];
    readonly List<Sprite> runtimeSprites = new List<Sprite>();

    RectTransform content;
    Button next;
    Text heading;
    Text subtitle;
    Text backLabel;
    Text nextLabel;
    Text leftSignTop;
    Text leftSignBottom;
    Text rightBanner;
    Text rightStoneQuote;
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
        content.sizeDelta = new Vector2(DesignWidth, DesignHeight);

        BuildHeader();
        BuildSceneDecor();

        float[] x = { -430f, 0f, 430f };
        for (int i = 0; i < difficulties.Length; i++)
            BuildCard(i, new Vector2(x[i], -18f));

        BuildBottomButtons();

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
            if (cardSurfaces[i] != null)
                cardSurfaces[i].color = active ? Color.white : new Color(.94f, .91f, .83f, 1f);
        }

        if (next != null) next.interactable = true;
    }

    void BuildBackground()
    {
        Texture2D texture = Resources.Load<Texture2D>("ChapterSelect/Background");
        GameObject backgroundObject = new GameObject("DifficultyBackground", typeof(RectTransform), typeof(RawImage));
        backgroundObject.transform.SetParent(transform, false);

        RawImage background = backgroundObject.GetComponent<RawImage>();
        background.texture = texture;
        background.color = texture != null ? Color.white : new Color(.18f, .34f, .42f, 1f);
        background.raycastTarget = false;
        Stretch(background.rectTransform);

        AspectRatioFitter fitter = backgroundObject.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        fitter.aspectRatio = 1448f / 1086f;

        AddStretchWash("SkyWarmth", new Color(1f, .84f, .56f, .05f));
        AddBottomShade();
    }

    void AddStretchWash(string name, Color color)
    {
        GameObject wash = new GameObject(name, typeof(RectTransform), typeof(Image));
        wash.transform.SetParent(transform, false);
        Image image = wash.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        Stretch(image.rectTransform);
    }

    void AddBottomShade()
    {
        GameObject shade = new GameObject("GroundShade", typeof(RectTransform), typeof(Image));
        shade.transform.SetParent(transform, false);
        Image image = shade.GetComponent<Image>();
        image.color = new Color(.11f, .055f, .018f, .18f);
        image.raycastTarget = false;

        RectTransform rect = image.rectTransform;
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(.5f, 0f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(0f, 170f);
    }

    void BuildHeader()
    {
        Sprite parchment = LoadSprite("ChapterSelect/Parchment");

        GameObject titlePlate = Plate(
            content,
            "DifficultyTitlePlate",
            new Vector2(0f, 382f),
            new Vector2(1110f, 124f),
            parchment,
            new Color(1f, .97f, .87f, 1f),
            new Color(.30f, .13f, .035f, .95f),
            5f);

        heading = Label("Heading", titlePlate.transform, new Vector2(0f, 8f), new Vector2(1010f, 76f), 56);
        heading.color = new Color(.29f, .018f, .012f, 1f);

        AddRivet(titlePlate.transform, new Vector2(-525f, 37f));
        AddRivet(titlePlate.transform, new Vector2(525f, 37f));
        AddRivet(titlePlate.transform, new Vector2(-525f, -37f));
        AddRivet(titlePlate.transform, new Vector2(525f, -37f));

        GameObject subtitlePlate = Plate(
            content,
            "DifficultySubtitlePlate",
            new Vector2(0f, 314f),
            new Vector2(870f, 58f),
            parchment,
            new Color(.98f, .94f, .83f, 1f),
            new Color(.31f, .15f, .05f, .8f),
            3f);

        subtitle = Label("Subtitle", subtitlePlate.transform, Vector2.zero, new Vector2(808f, 42f), 22);
        subtitle.color = new Color(.13f, .06f, .022f, 1f);
    }

    void BuildSceneDecor()
    {
        Sprite parchment = LoadSprite("ChapterSelect/Parchment");

        GameObject post = Plate(
            content,
            "LeftSignPost",
            new Vector2(-742f, 74f),
            new Vector2(34f, 515f),
            null,
            new Color(.30f, .16f, .065f, 1f),
            new Color(.12f, .055f, .018f, .9f),
            3f);
        post.transform.localRotation = Quaternion.Euler(0f, 0f, -1.5f);

        GameObject signTop = Plate(
            content,
            "LeftSignGods",
            new Vector2(-684f, 230f),
            new Vector2(230f, 72f),
            parchment,
            new Color(.88f, .72f, .47f, 1f),
            new Color(.22f, .10f, .025f, .95f),
            4f);
        signTop.transform.localRotation = Quaternion.Euler(0f, 0f, 4f);
        leftSignTop = Label("Label", signTop.transform, Vector2.zero, new Vector2(200f, 58f), 22);
        leftSignTop.color = new Color(.16f, .07f, .02f, 1f);

        GameObject signBottom = Plate(
            content,
            "LeftSignHeroes",
            new Vector2(-690f, 142f),
            new Vector2(244f, 70f),
            parchment,
            new Color(.85f, .68f, .43f, 1f),
            new Color(.22f, .10f, .025f, .95f),
            4f);
        signBottom.transform.localRotation = Quaternion.Euler(0f, 0f, -2.5f);
        leftSignBottom = Label("Label", signBottom.transform, Vector2.zero, new Vector2(214f, 56f), 22);
        leftSignBottom.color = new Color(.16f, .07f, .02f, 1f);

        GameObject banner = Plate(
            content,
            "RightTroyBanner",
            new Vector2(703f, 222f),
            new Vector2(205f, 210f),
            parchment,
            new Color(.95f, .89f, .73f, 1f),
            new Color(.45f, .08f, .04f, .8f),
            4f);
        banner.transform.localRotation = Quaternion.Euler(0f, 0f, -3f);
        rightBanner = Label("Label", banner.transform, Vector2.zero, new Vector2(174f, 175f), 27);
        rightBanner.color = new Color(.47f, .04f, .025f, 1f);

        GameObject stone = Plate(
            content,
            "RightChoiceStone",
            new Vector2(714f, -150f),
            new Vector2(220f, 215f),
            null,
            new Color(.58f, .56f, .50f, .96f),
            new Color(.25f, .22f, .18f, .9f),
            4f);
        stone.transform.localRotation = Quaternion.Euler(0f, 0f, 7f);
        rightStoneQuote = Label("Label", stone.transform, Vector2.zero, new Vector2(184f, 176f), 20);
        rightStoneQuote.color = new Color(.10f, .085f, .06f, 1f);

        BuildBottomHelmetAccent();
    }

    void BuildBottomHelmetAccent()
    {
        GameObject crest = new GameObject("BottomHelmetCrest", typeof(RectTransform), typeof(Image));
        crest.transform.SetParent(content, false);
        Image crestImage = crest.GetComponent<Image>();
        crestImage.color = new Color(.64f, .045f, .035f, .96f);
        crestImage.raycastTarget = false;
        RectTransform crestRect = crestImage.rectTransform;
        crestRect.anchorMin = crestRect.anchorMax = crestRect.pivot = new Vector2(.5f, .5f);
        crestRect.anchoredPosition = new Vector2(-100f, -411f);
        crestRect.sizeDelta = new Vector2(126f, 38f);
        crestRect.localRotation = Quaternion.Euler(0f, 0f, 10f);

        GameObject helmet = new GameObject("BottomHelmet", typeof(RectTransform), typeof(Image));
        helmet.transform.SetParent(content, false);
        Image helmetImage = helmet.GetComponent<Image>();
        helmetImage.color = new Color(.73f, .44f, .10f, 1f);
        helmetImage.raycastTarget = false;
        RectTransform helmetRect = helmetImage.rectTransform;
        helmetRect.anchorMin = helmetRect.anchorMax = helmetRect.pivot = new Vector2(.5f, .5f);
        helmetRect.anchoredPosition = new Vector2(-92f, -424f);
        helmetRect.sizeDelta = new Vector2(122f, 72f);
        Outline edge = helmet.AddComponent<Outline>();
        edge.effectColor = new Color(.18f, .08f, .02f, 1f);
        edge.effectDistance = new Vector2(3f, -3f);
    }

    void BuildCard(int index, Vector2 position)
    {
        CampaignDifficulty difficulty = difficulties[index];

        GameObject card = new GameObject(
            "Difficulty_" + difficulty,
            typeof(RectTransform),
            typeof(Image),
            typeof(Button),
            typeof(Shadow));
        card.transform.SetParent(content, false);

        RectTransform rect = card.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(418f, 594f);
        cardRects[index] = rect;

        Image surface = card.GetComponent<Image>();
        surface.sprite = LoadSprite("ChapterSelect/Parchment");
        surface.type = Image.Type.Simple;
        surface.color = new Color(.94f, .91f, .83f, 1f);
        cardSurfaces[index] = surface;

        Shadow border = card.GetComponent<Shadow>();
        border.effectColor = new Color(.19f, .08f, .025f, .98f);
        border.effectDistance = new Vector2(6f, -7f);

        Outline darkFrame = card.AddComponent<Outline>();
        darkFrame.effectColor = new Color(.25f, .105f, .025f, 1f);
        darkFrame.effectDistance = new Vector2(3f, -3f);

        Button button = card.GetComponent<Button>();
        button.targetGraphic = surface;
        button.onClick.AddListener(() => SetSelection(difficulty));
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.04f, 1.01f, .91f, 1f);
        colors.pressedColor = new Color(.82f, .75f, .62f, 1f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(.55f, .52f, .48f, .7f);
        colors.fadeDuration = .08f;
        button.colors = colors;
        card.AddComponent<MenuButtonFeedback>();
        cards[index] = button;

        selections[index] = card.AddComponent<Outline>();
        selections[index].effectColor = new Color(1f, .72f, .08f, 1f);
        selections[index].effectDistance = new Vector2(8f, -8f);
        selections[index].enabled = false;

        BuildCardRivets(card.transform);

        GameObject header = Plate(
            card.transform,
            "Header",
            new Vector2(0f, 252f),
            new Vector2(388f, 82f),
            null,
            headerColors[index],
            new Color(.22f, .075f, .02f, .9f),
            3f);
        Text title = Label("Title", header.transform, Vector2.zero, new Vector2(354f, 62f), 37);
        title.text = DifficultyTitle(difficulty);
        title.color = Color.white;

        GameObject portraitFrame = Plate(
            card.transform,
            "PortraitFrame",
            new Vector2(0f, 92f),
            new Vector2(366f, 250f),
            null,
            new Color(.18f, .085f, .028f, 1f),
            new Color(.10f, .04f, .012f, 1f),
            4f);
        portraitFrame.AddComponent<RectMask2D>();

        GameObject portraitObject = new GameObject("Portrait", typeof(RectTransform), typeof(RawImage));
        portraitObject.transform.SetParent(portraitFrame.transform, false);
        RawImage portrait = portraitObject.GetComponent<RawImage>();
        portrait.texture = Resources.Load<Texture2D>(portraitResources[index]);
        portrait.color = portrait.texture != null ? Color.white : headerColors[index];
        portrait.raycastTarget = false;
        RectTransform portraitRect = portrait.rectTransform;
        Stretch(portraitRect);
        portraitRect.offsetMin = new Vector2(-20f, -12f);
        portraitRect.offsetMax = new Vector2(20f, 15f);
        if (portrait.texture != null)
        {
            AspectRatioFitter portraitFit = portraitObject.AddComponent<AspectRatioFitter>();
            portraitFit.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            portraitFit.aspectRatio = (float)portrait.texture.width / portrait.texture.height;
        }

        Text description = Label(
            "Description",
            card.transform,
            new Vector2(0f, -83f),
            new Vector2(342f, 86f),
            21);
        description.text = DifficultyDescription(difficulty);
        description.color = new Color(.13f, .055f, .018f, 1f);

        Text goldCaption = Label(
            "GoldCaption",
            card.transform,
            new Vector2(-31f, -151f),
            new Vector2(222f, 28f),
            17);
        goldCaption.text = GameLanguage.T("STARTING GOLD", "СТАРТОВОЕ ЗОЛОТО");
        goldCaption.color = new Color(.14f, .06f, .018f, 1f);

        GameObject coin = new GameObject("Coin", typeof(RectTransform), typeof(Image));
        coin.transform.SetParent(card.transform, false);
        Image coinImage = coin.GetComponent<Image>();
        coinImage.color = new Color(1f, .67f, .05f, 1f);
        coinImage.raycastTarget = false;
        RectTransform coinRect = coinImage.rectTransform;
        coinRect.anchorMin = coinRect.anchorMax = coinRect.pivot = new Vector2(.5f, .5f);
        coinRect.anchoredPosition = new Vector2(-123f, -197f);
        coinRect.sizeDelta = new Vector2(62f, 62f);
        Outline coinEdge = coin.AddComponent<Outline>();
        coinEdge.effectColor = new Color(.39f, .15f, .008f, 1f);
        coinEdge.effectDistance = new Vector2(4f, -4f);
        Text coinMark = Label("CoinMark", coin.transform, Vector2.zero, new Vector2(48f, 48f), 28);
        coinMark.text = "I";
        coinMark.color = new Color(.60f, .27f, .015f, 1f);

        Text gold = Label(
            "StartingGold",
            card.transform,
            new Vector2(54f, -196f),
            new Vector2(215f, 70f),
            48);
        gold.text = DifficultyRules.StartingGold(difficulty).ToString();
        gold.color = new Color(.31f, .02f, .012f, 1f);

        GameObject mottoPlate = Plate(
            card.transform,
            "MottoPlate",
            new Vector2(0f, -257f),
            new Vector2(350f, 54f),
            null,
            new Color(.76f, .64f, .43f, .30f),
            new Color(.32f, .15f, .045f, .35f),
            2f);
        Text motto = Label("Motto", mottoPlate.transform, Vector2.zero, new Vector2(324f, 45f), 18);
        motto.text = DifficultyMotto(difficulty);
        motto.color = new Color(.18f, .08f, .025f, 1f);

        selectedLabels[index] = Label(
            "Selected",
            card.transform,
            new Vector2(0f, 292f),
            new Vector2(220f, 28f),
            17);
        selectedLabels[index].text = GameLanguage.T("SELECTED", "ВЫБРАНО");
        selectedLabels[index].color = new Color(1f, .84f, .20f, 1f);
        selectedLabels[index].gameObject.SetActive(false);
    }

    void BuildCardRivets(Transform card)
    {
        AddRivet(card, new Vector2(-192f, 274f));
        AddRivet(card, new Vector2(192f, 274f));
        AddRivet(card, new Vector2(-192f, -274f));
        AddRivet(card, new Vector2(192f, -274f));
    }

    void BuildBottomButtons()
    {
        Sprite backSprite = LoadAtlasSlice(new Rect(16f, 949f, 300f, 116f));
        Sprite nextSprite = LoadAtlasSlice(new Rect(979f, 800f, 427f, 92f));

        Button backButton = ArtCommand(
            "Back",
            new Vector2(-628f, -405f),
            new Vector2(320f, 88f),
            backSprite,
            new Color(.78f, .75f, .68f, 1f),
            () => onBack?.Invoke());
        backLabel = backButton.GetComponentInChildren<Text>(true);

        next = ArtCommand(
            "Next",
            new Vector2(615f, -405f),
            new Vector2(430f, 94f),
            nextSprite,
            new Color(.78f, .055f, .035f, 1f),
            Confirm);
        nextLabel = next.GetComponentInChildren<Text>(true);
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

        if (backLabel != null) backLabel.text = GameLanguage.T("<  BACK", "<  НАЗАД");
        if (nextLabel != null) nextLabel.text = GameLanguage.T("NEXT  >", "ДАЛЕЕ  >");

        if (leftSignTop != null)
            leftSignTop.text = GameLanguage.T("THE GODS\nARE WATCHING", "БОГИ\nНАБЛЮДАЮТ");
        if (leftSignBottom != null)
            leftSignBottom.text = GameLanguage.T("HEROES\nCHOOSE", "ГЕРОИ\nВЫБИРАЮТ");
        if (rightBanner != null)
            rightBanner.text = GameLanguage.T("TROY\nSTANDS\nFOREVER!", "ТРОЯ\nВСЕГДА\nСТОИТ!");
        if (rightStoneQuote != null)
            rightStoneQuote.text = GameLanguage.T(
                "GREAT DEEDS\nBEGIN WITH\nYOUR CHOICE",
                "ВЕЛИКИЕ ДЕЛА\nНАЧИНАЮТСЯ\nС ТВОЕГО ВЫБОРА");

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
                    "For those who want to enjoy the game and discover Troy's story.",
                    "Для тех, кто хочет насладиться игрой и узнать историю Трои.");
            case CampaignDifficulty.Legendary:
                return GameLanguage.T(
                    "Only for those worthy of entering the legends.",
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
        float scale = Mathf.Min(available.x / DesignWidth, available.y / DesignHeight);
        if (scale <= 0f) return;
        content.localScale = Vector3.one * scale;
    }

    GameObject Plate(
        Transform parent,
        string name,
        Vector2 position,
        Vector2 size,
        Sprite sprite,
        Color color,
        Color edgeColor,
        float edgeDistance)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        Image image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.type = Image.Type.Simple;
        image.color = color;
        image.raycastTarget = false;

        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = edgeColor;
        outline.effectDistance = new Vector2(edgeDistance, -edgeDistance);
        return go;
    }

    Button ArtCommand(
        string name,
        Vector2 position,
        Vector2 size,
        Sprite sprite,
        Color fallbackColor,
        Action action)
    {
        RectTransform rect = Node(name, content);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image plate = rect.gameObject.AddComponent<Image>();
        plate.sprite = sprite;
        plate.type = Image.Type.Simple;
        plate.color = sprite != null ? Color.white : fallbackColor;

        Outline edge = rect.gameObject.AddComponent<Outline>();
        edge.effectColor = new Color(.24f, .09f, .018f, .95f);
        edge.effectDistance = new Vector2(4f, -4f);

        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = plate;
        button.onClick.AddListener(() => action?.Invoke());

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.08f, 1.04f, .90f, 1f);
        colors.pressedColor = new Color(.82f, .75f, .63f, 1f);
        colors.disabledColor = new Color(.48f, .45f, .41f, .70f);
        colors.fadeDuration = .08f;
        button.colors = colors;

        rect.gameObject.AddComponent<MenuButtonFeedback>();

        Text label = Label("Label", rect, Vector2.zero, size - new Vector2(30f, 14f), 34);
        label.color = name == "Next"
            ? new Color(.98f, .88f, .63f, 1f)
            : new Color(.92f, .88f, .78f, 1f);
        return button;
    }

    void AddRivet(Transform parent, Vector2 position)
    {
        GameObject go = new GameObject("Rivet", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        Image image = go.GetComponent<Image>();
        image.color = new Color(.83f, .53f, .16f, 1f);
        image.raycastTarget = false;

        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(12f, 12f);

        Outline edge = go.AddComponent<Outline>();
        edge.effectColor = new Color(.23f, .09f, .012f, 1f);
        edge.effectDistance = new Vector2(2f, -2f);
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
        text.resizeTextMinSize = Mathf.Max(12, fontSize - 11);
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

    Sprite LoadSprite(string resource)
    {
        Texture2D texture = Resources.Load<Texture2D>(resource);
        if (texture == null) return null;
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(.5f, .5f),
            100f);
        runtimeSprites.Add(sprite);
        return sprite;
    }

    Sprite LoadAtlasSlice(Rect topLeftPixels)
    {
        Texture2D texture = Resources.Load<Texture2D>("ChapterSelect/ReferenceAtlas");
        if (texture == null) return null;

        float x = texture.width / 1448f;
        float y = texture.height / 1086f;
        Rect rect = new Rect(
            topLeftPixels.x * x,
            (1086f - topLeftPixels.yMax) * y,
            topLeftPixels.width * x,
            topLeftPixels.height * y);

        Sprite sprite = Sprite.Create(texture, rect, new Vector2(.5f, .5f), 100f);
        runtimeSprites.Add(sprite);
        return sprite;
    }

    static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }

    void OnDestroy()
    {
        foreach (Sprite sprite in runtimeSprites)
        {
            if (sprite == null) continue;
            if (Application.isPlaying) Destroy(sprite);
            else DestroyImmediate(sprite);
        }
        runtimeSprites.Clear();
    }
}
