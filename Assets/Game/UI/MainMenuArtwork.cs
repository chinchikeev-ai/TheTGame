using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class MainMenuArtwork : MonoBehaviour
{
    RectTransform layout;
    readonly GameObject[] translations = new GameObject[3];
    Text settingsTip;
    bool? lastRussian;

    public Transform Content => layout;

    public void Build(UnityAction play, UnityAction heroes, UnityAction settings, UnityAction exit)
    {
        var background = Art("Background", transform, "Menu/Main_screen", Vector2.zero, Vector2.zero);
        background.rectTransform.anchorMin = Vector2.zero;
        background.rectTransform.anchorMax = Vector2.one;
        background.rectTransform.offsetMin = background.rectTransform.offsetMax = Vector2.zero;
        var fit = background.gameObject.AddComponent<AspectRatioFitter>();
        fit.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        fit.aspectRatio = 16f / 9f;

        layout = Node("MainScreenLayout", transform, Vector2.zero, new Vector2(1600, 900));
        Art("Logo", layout, "MainScreen/Name_main2", new Vector2(-285, 312), new Vector2(750, 250));
        var playButton = Command("PLAY_Button", "Start", new Vector2(390, 240), new Vector2(640, 213.333f), play);
        var heroesButton = Command("HEROES_Button", "Heroes", new Vector2(390, 68), new Vector2(460, 153.333f), heroes);
        var settingsButton = Command("SETTINGS_Button", "Settings", new Vector2(732, 390), new Vector2(96, 96), settings);
        settingsButton.GetComponent<RawImage>().uvRect = new Rect(.14f, .15f, .72f, .72f);
        var exitButton = Command("EXIT_Button", "exit2", new Vector2(650, -384), new Vector2(260, 86.667f), exit);

        translations[0] = Translation(playButton, "ИГРАТЬ", new Vector2(55, 1), new Vector2(266, 89), 48, new Rect(.23f, .48f, .015f, .04f));
        translations[1] = Translation(heroesButton, "ГЕРОИ", new Vector2(40, 0), new Vector2(191, 60), 36, new Rect(.84f, .38f, .035f, .24f));
        translations[2] = Translation(exitButton, "ВЫХОД", new Vector2(26, 0), new Vector2(91, 36), 24, new Rect(.80f, .38f, .025f, .24f));

        settingsTip = Label("SettingsTooltip", layout, new Vector2(675, 316), new Vector2(225, 40), 20);
        settingsTip.color = new Color(1f, .94f, .79f);
        var shadow = settingsTip.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(.08f, .035f, .01f, 1f);
        shadow.effectDistance = new Vector2(2, -2);
        settingsTip.gameObject.SetActive(false);
        var trigger = settingsButton.gameObject.AddComponent<EventTrigger>();
        TooltipEvent(trigger, EventTriggerType.PointerEnter, true);
        TooltipEvent(trigger, EventTriggerType.PointerExit, false);
        TooltipEvent(trigger, EventTriggerType.Select, true);
        TooltipEvent(trigger, EventTriggerType.Deselect, false);
        RefreshLayout();
    }

    void TooltipEvent(EventTrigger trigger, EventTriggerType type, bool visible)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(_ => settingsTip.gameObject.SetActive(visible));
        trigger.triggers.Add(entry);
    }

    void LateUpdate() => RefreshLayout();

    void RefreshLayout()
    {
        if (layout == null) return;
        var available = ((RectTransform)transform).rect.size;
        layout.localScale = Vector3.one * Mathf.Min(available.x / 1600f, available.y / 900f);
        if (lastRussian != GameLanguage.Russian) ApplyLanguage(GameLanguage.Russian);
    }

    void ApplyLanguage(bool russian)
    {
        lastRussian = russian;
        foreach (var translation in translations) translation.SetActive(russian);
        settingsTip.text = russian ? "НАСТРОЙКИ" : "SETTINGS";
    }

    Button Command(string name, string file, Vector2 position, Vector2 size, UnityAction action)
    {
        var art = Art(name, layout, "MainScreen/" + file, position, size);
        art.raycastTarget = true;
        var button = art.gameObject.AddComponent<Button>();
        button.targetGraphic = art;
        button.transition = Selectable.Transition.None;
        button.onClick.AddListener(action);
        art.gameObject.AddComponent<MenuButtonFeedback>();
        return button;
    }

    static GameObject Translation(Button button, string value, Vector2 position, Vector2 size, int fontSize, Rect sample)
    {
        // Sample empty parchment from the same supplied texture to cover baked English lettering.
        var plate = Node("RussianCaption", button.transform, position, size).gameObject.AddComponent<RawImage>();
        plate.texture = button.GetComponent<RawImage>().texture;
        plate.uvRect = sample;
        plate.raycastTarget = false;
        var label = Label("Label", plate.transform, Vector2.zero, size, fontSize);
        label.text = value;
        return plate.gameObject;
    }

    static RawImage Art(string name, Transform parent, string resource, Vector2 position, Vector2 size)
    {
        var image = Node(name, parent, position, size).gameObject.AddComponent<RawImage>();
        image.texture = Resources.Load<Texture2D>(resource);
        image.raycastTarget = false;
        if (image.texture == null) Debug.LogError("Missing main-screen artwork: " + resource);
        return image;
    }

    static Text Label(string name, Transform parent, Vector2 position, Vector2 size, int fontSize)
    {
        var text = Node(name, parent, position, size).gameObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(.17f, .075f, .022f);
        text.raycastTarget = false;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = fontSize - 6;
        text.resizeTextMaxSize = fontSize;
        return text;
    }

    static RectTransform Node(string name, Transform parent, Vector2 position, Vector2 size)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return rect;
    }
}
