using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MenuFlowStylePresentation : MonoBehaviour
{
    const string ApprovedMenuResource = "Menu/ApprovedMainMenu";

    Canvas boundCanvas;
    GameObject styledLevelSelect;
    GameObject styledPauseMenu;
    GameObject styledEndMenu;
    Texture2D approvedTexture;
    Sprite approvedSprite;

    enum ButtonVisual
    {
        Primary,
        Stone,
        Ghost
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoStart()
    {
        if (FindFirstObjectByType<MenuFlowStylePresentation>() == null)
            new GameObject("MenuFlowStylePresenter").AddComponent<MenuFlowStylePresentation>();
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        LoadApprovedSprite();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        boundCanvas = null;
        styledLevelSelect = null;
        styledPauseMenu = null;
        styledEndMenu = null;
    }

    void LateUpdate()
    {
        BindCanvas();
        if (boundCanvas == null) return;

        Transform levelSelect = boundCanvas.transform.Find("LevelSelect");
        if (levelSelect != null && levelSelect.gameObject != styledLevelSelect)
        {
            StyleLevelSelect(levelSelect);
            styledLevelSelect = levelSelect.gameObject;
        }

        Transform pauseMenu = boundCanvas.transform.Find("PauseMenu");
        if (pauseMenu != null && pauseMenu.gameObject != styledPauseMenu)
        {
            StylePauseMenu(pauseMenu);
            styledPauseMenu = pauseMenu.gameObject;
        }

        Transform endMenu = boundCanvas.transform.Find("EndMenu");
        if (endMenu != null && endMenu.gameObject != styledEndMenu)
        {
            StyleEndMenu(endMenu);
            styledEndMenu = endMenu.gameObject;
        }
    }

    void BindCanvas()
    {
        GameObject menuCanvas = GameObject.Find("MenuCanvas");
        Canvas current = menuCanvas != null ? menuCanvas.GetComponent<Canvas>() : null;
        if (current == boundCanvas) return;

        boundCanvas = current;
        styledLevelSelect = null;
        styledPauseMenu = null;
        styledEndMenu = null;
    }

    void LoadApprovedSprite()
    {
        approvedTexture = Resources.Load<Texture2D>(ApprovedMenuResource);
        if (approvedTexture == null) return;

        approvedSprite = Sprite.Create(
            approvedTexture,
            new Rect(0f, 0f, approvedTexture.width, approvedTexture.height),
            new Vector2(.5f, .5f),
            100f);
    }

    void StyleLevelSelect(Transform root)
    {
        StyleScreenBackground(root, new Color(.38f, .17f, .055f, .92f));

        Transform card = root.Find("LevelCard");
        if (card != null)
        {
            StyleCard(card, new Color(.085f, .036f, .014f, .98f), new Color(.92f, .49f, .12f, .72f));
            AddAccentBars(card, 560f);
            StyleTexts(card, "CHAPTER SELECT", "ВЫБОР ГЛАВЫ");

            Button[] buttons = card.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < buttons.Length; i++)
            {
                Text label = buttons[i].GetComponentInChildren<Text>(true);
                string value = label != null ? label.text : string.Empty;
                if (ContainsAny(value, "THE LANDING", "ВЫСАДКА"))
                    StyleButton(buttons[i], ButtonVisual.Primary);
                else if (ContainsAny(value, "BACK", "НАЗАД"))
                    StyleButton(buttons[i], ButtonVisual.Ghost);
                else
                    StyleButton(buttons[i], ButtonVisual.Stone);
            }
        }

        Transform mapLayer = root.Find("CampaignMapLayer");
        if (mapLayer != null)
        {
            Image layerImage = mapLayer.GetComponent<Image>();
            if (layerImage != null)
                layerImage.color = new Color(.055f, .020f, .008f, .44f);

            Transform mapField = mapLayer.Find("MapField");
            if (mapField != null)
            {
                StyleCard(mapField, new Color(.095f, .043f, .017f, .88f), new Color(.90f, .47f, .12f, .48f));
                AddAccentBars(mapField, 900f);
            }
        }

        RuntimeFileLogger.Event("MENU", "Styled Chapter Select to approved menu language");
    }

    void StylePauseMenu(Transform root)
    {
        StyleScreenBackground(root, new Color(.24f, .085f, .025f, .86f));

        Transform card = root.Find("PauseCard");
        if (card == null) return;

        RectTransform cardRect = card as RectTransform;
        if (cardRect != null)
            cardRect.sizeDelta = new Vector2(980f, 720f);

        StyleCard(card, new Color(.075f, .030f, .012f, .985f), new Color(.95f, .52f, .14f, .80f));
        AddAccentBars(card, 940f);
        StyleTexts(card, "PAUSED", "ПАУЗА");

        Button[] buttons = card.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
        {
            Text label = buttons[i].GetComponentInChildren<Text>(true);
            string value = label != null ? label.text : string.Empty;

            if (ContainsAny(value, "RESUME", "ВЕРНУТЬСЯ В ИГРУ"))
                StyleButton(buttons[i], ButtonVisual.Primary);
            else if (ContainsAny(value, "MAIN MENU", "ГЛАВНОЕ МЕНЮ", "EXIT", "ВЫХОД"))
                StyleButton(buttons[i], ButtonVisual.Ghost);
            else
                StyleButton(buttons[i], ButtonVisual.Stone);
        }

        RuntimeFileLogger.Event("MENU", "Styled Pause menu to approved menu language");
    }

    void StyleEndMenu(Transform root)
    {
        StyleScreenBackground(root, new Color(.25f, .085f, .020f, .90f));

        Transform card = root.Find("ResultCard");
        if (card == null) return;

        RectTransform cardRect = card as RectTransform;
        if (cardRect != null)
            cardRect.sizeDelta = new Vector2(1160f, 800f);

        StyleCard(card, new Color(.080f, .031f, .012f, .988f), new Color(.98f, .53f, .14f, .82f));
        AddAccentBars(card, 1110f);
        StyleTexts(card, "RESULT", "РЕЗУЛЬТАТ");

        Button[] buttons = card.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
        {
            Text label = buttons[i].GetComponentInChildren<Text>(true);
            string value = label != null ? label.text : string.Empty;

            if (ContainsAny(value, "RETRY", "ПОВТОРИТЬ"))
                StyleButton(buttons[i], ButtonVisual.Primary);
            else if (ContainsAny(value, "MAIN MENU", "ГЛАВНОЕ МЕНЮ"))
                StyleButton(buttons[i], ButtonVisual.Ghost);
            else
                StyleButton(buttons[i], ButtonVisual.Stone);
        }

        RuntimeFileLogger.Event("MENU", "Styled Result screen to approved menu language");
    }

    void StyleScreenBackground(Transform root, Color tint)
    {
        Image image = root.GetComponent<Image>();
        if (image == null) return;

        if (approvedSprite != null)
        {
            image.sprite = approvedSprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = false;
        }
        image.color = tint;
    }

    void StyleCard(Transform card, Color fill, Color border)
    {
        Image image = card.GetComponent<Image>();
        if (image != null)
            image.color = fill;

        Outline outline = card.GetComponent<Outline>();
        if (outline == null) outline = card.gameObject.AddComponent<Outline>();
        outline.effectColor = border;
        outline.effectDistance = new Vector2(3f, -3f);

        Shadow shadow = card.GetComponent<Shadow>();
        if (shadow == null) shadow = card.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(.02f, .006f, .002f, .80f);
        shadow.effectDistance = new Vector2(0f, -10f);
    }

    void StyleTexts(Transform root, string englishTitle, string russianTitle)
    {
        Text[] texts = root.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            Text text = texts[i];
            if (text == null) continue;

            string value = text.text ?? string.Empty;
            if (ContainsAny(value, englishTitle, russianTitle))
            {
                text.color = new Color(1f, .69f, .18f, 1f);
                text.fontStyle = FontStyle.Bold;
                AddTextShadow(text, new Color(.10f, .025f, .004f, .95f), new Vector2(3f, -3f));
            }
            else if (text.GetComponentInParent<Button>() == null)
            {
                if (text.fontSize >= 18)
                    text.color = new Color(.96f, .83f, .64f, 1f);
                else
                    text.color = new Color(.80f, .69f, .57f, .94f);
            }
        }
    }

    void StyleButton(Button button, ButtonVisual visual)
    {
        if (button == null) return;

        Image image = button.GetComponent<Image>();
        if (image == null) return;

        Color face;
        Color border;
        Color textColor;
        Color hover;
        Color press;

        switch (visual)
        {
            case ButtonVisual.Primary:
                face = new Color(.82f, .20f, .045f, 1f);
                border = new Color(1f, .63f, .14f, 1f);
                textColor = new Color(1f, .88f, .58f, 1f);
                hover = new Color(1f, .87f, .68f, 1f);
                press = new Color(.72f, .55f, .42f, 1f);
                break;

            case ButtonVisual.Stone:
                face = new Color(.47f, .27f, .13f, .98f);
                border = new Color(.91f, .52f, .17f, .90f);
                textColor = new Color(1f, .86f, .63f, 1f);
                hover = new Color(1f, .92f, .77f, 1f);
                press = new Color(.72f, .62f, .53f, 1f);
                break;

            default:
                face = new Color(.17f, .060f, .022f, .97f);
                border = new Color(.63f, .31f, .09f, .78f);
                textColor = new Color(.93f, .76f, .49f, 1f);
                hover = new Color(1f, .88f, .70f, 1f);
                press = new Color(.68f, .57f, .48f, 1f);
                break;
        }

        image.color = face;

        Outline outline = button.GetComponent<Outline>();
        if (outline == null) outline = button.gameObject.AddComponent<Outline>();
        outline.effectColor = border;
        outline.effectDistance = new Vector2(4f, -4f);

        Shadow shadow = button.GetComponent<Shadow>();
        if (shadow == null) shadow = button.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(.035f, .008f, .002f, .85f);
        shadow.effectDistance = new Vector2(0f, -7f);

        button.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = hover;
        colors.selectedColor = hover;
        colors.pressedColor = press;
        colors.disabledColor = new Color(.45f, .42f, .38f, .62f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = .06f;
        button.colors = colors;

        Text label = button.GetComponentInChildren<Text>(true);
        if (label != null)
        {
            label.color = textColor;
            label.fontStyle = FontStyle.Bold;
            AddTextShadow(label, new Color(.07f, .015f, .003f, .90f), new Vector2(2f, -2f));
        }

        if (button.GetComponent<MenuButtonFeedback>() == null)
            button.gameObject.AddComponent<MenuButtonFeedback>();
    }

    void AddAccentBars(Transform card, float width)
    {
        Transform old = card.Find("ApprovedStyleAccents");
        if (old != null) return;

        GameObject accents = new GameObject("ApprovedStyleAccents");
        accents.transform.SetParent(card, false);
        RectTransform root = accents.AddComponent<RectTransform>();
        root.anchorMin = root.anchorMax = root.pivot = new Vector2(.5f, .5f);
        root.anchoredPosition = Vector2.zero;
        root.sizeDelta = Vector2.zero;
        root.SetAsFirstSibling();

        AddBar(accents.transform, "TopGold", new Vector2(0f, 0f), width, 5f, new Vector2(.5f, 1f), new Color(1f, .56f, .13f, .78f));
        AddBar(accents.transform, "TopDark", new Vector2(0f, -7f), width - 20f, 2f, new Vector2(.5f, 1f), new Color(.32f, .10f, .025f, .85f));
        AddBar(accents.transform, "BottomGold", new Vector2(0f, 0f), width, 4f, new Vector2(.5f, 0f), new Color(.83f, .38f, .09f, .62f));
    }

    void AddBar(Transform parent, string name, Vector2 offset, float width, float height, Vector2 anchor, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;

        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = anchor;
        rect.anchoredPosition = offset;
        rect.sizeDelta = new Vector2(width, height);
    }

    void AddTextShadow(Text text, Color color, Vector2 distance)
    {
        Shadow shadow = text.GetComponent<Shadow>();
        if (shadow == null) shadow = text.gameObject.AddComponent<Shadow>();
        shadow.effectColor = color;
        shadow.effectDistance = distance;
    }

    static bool ContainsAny(string value, params string[] tokens)
    {
        if (string.IsNullOrEmpty(value) || tokens == null) return false;
        for (int i = 0; i < tokens.Length; i++)
        {
            if (!string.IsNullOrEmpty(tokens[i]) && value.Contains(tokens[i]))
                return true;
        }
        return false;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (approvedSprite != null) Destroy(approvedSprite);
    }
}
