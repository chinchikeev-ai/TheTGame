using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public sealed class AudioSettingsLayoutPolisher : MonoBehaviour
{
    static AudioSettingsLayoutPolisher instance;

    Transform settingsRoot;
    Transform contentRoot;
    int polishedAudioRootId;
    float nextProbeAt;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        if (instance != null) return;
        GameObject host = new GameObject("AudioSettingsLayoutPolisher");
        DontDestroyOnLoad(host);
        instance = host.AddComponent<AudioSettingsLayoutPolisher>();
    }

    IEnumerator Start()
    {
        while (settingsRoot == null)
        {
            GameObject menuCanvas = GameObject.Find("MenuCanvas");
            if (menuCanvas != null) settingsRoot = menuCanvas.transform.Find("Settings");
            if (settingsRoot == null) yield return null;
        }
    }

    void LateUpdate()
    {
        if (settingsRoot == null || !settingsRoot.gameObject.activeInHierarchy) return;
        if (Time.unscaledTime < nextProbeAt) return;
        nextProbeAt = Time.unscaledTime + .20f;

        Transform modern = settingsRoot.Find("ModernSettingsPanel");
        Transform content = modern != null ? modern.Find("ContentPanel/ContentRoot") : null;
        if (content == null) return;

        Slider[] sliders = content.GetComponentsInChildren<Slider>(true);
        if (sliders.Length != 2) return;

        int rootId = content.GetInstanceID() ^ sliders[0].GetInstanceID() ^ sliders[1].GetInstanceID();
        if (contentRoot == content && polishedAudioRootId == rootId && content.Find("AudioMasterCard") != null) return;

        contentRoot = content;
        PolishAudio(sliders);
        polishedAudioRootId = rootId;
    }

    void PolishAudio(Slider[] sliders)
    {
        Text[] texts = contentRoot.GetComponentsInChildren<Text>(true);
        Text masterLabel = texts.FirstOrDefault(t => IsText(t, "MASTER VOLUME", "ОБЩАЯ ГРОМКОСТЬ"));
        Text musicLabel = texts.FirstOrDefault(t => IsText(t, "MUSIC", "МУЗЫКА"));
        Text[] percentages = texts.Where(t => t != null && t.text != null && t.text.EndsWith("%"))
            .OrderByDescending(t => t.rectTransform.anchoredPosition.y)
            .ToArray();
        Text hint = texts.FirstOrDefault(t => t != null && (t.text.Contains("previewed immediately") || t.text.Contains("слышны сразу")));

        Slider masterSlider = sliders.OrderByDescending(s => s.transform.GetSiblingIndex()).FirstOrDefault(s => s.name.Contains("MASTER") || s.name.Contains("ОБЩАЯ"));
        if (masterSlider == null) masterSlider = sliders.OrderByDescending(s => s.transform.GetSiblingIndex()).First();
        Slider musicSlider = sliders.First(s => s != masterSlider);

        CreateCard("AudioMasterCard", new Vector2(0f, 72f));
        CreateCard("AudioMusicCard", new Vector2(0f, -68f));

        Place(masterLabel, new Vector2(-210f, 92f), new Vector2(330f, 32f), TextAnchor.MiddleLeft);
        Place(musicLabel, new Vector2(-210f, -48f), new Vector2(330f, 32f), TextAnchor.MiddleLeft);
        PlaceSlider(masterSlider, new Vector2(20f, 48f));
        PlaceSlider(musicSlider, new Vector2(20f, -92f));

        if (percentages.Length > 0) Place(percentages[0], new Vector2(265f, 92f), new Vector2(90f, 32f), TextAnchor.MiddleRight);
        if (percentages.Length > 1) Place(percentages[1], new Vector2(265f, -48f), new Vector2(90f, 32f), TextAnchor.MiddleRight);
        if (hint != null) Place(hint, new Vector2(0f, -170f), new Vector2(650f, 44f), TextAnchor.UpperLeft);

        EnsureDescription("AudioMasterDescription",
            GameLanguage.T("All game sounds and effects", "Все звуки игры и эффекты"),
            new Vector2(-210f, 67f));
        EnsureDescription("AudioMusicDescription",
            GameLanguage.T("Background music only", "Только фоновая музыка"),
            new Vector2(-210f, -73f));
    }

    void CreateCard(string name, Vector2 position)
    {
        Transform existing = contentRoot.Find(name);
        if (existing != null) Destroy(existing.gameObject);

        GameObject card = new GameObject(name);
        card.transform.SetParent(contentRoot, false);
        Image image = card.AddComponent<Image>();
        image.color = new Color(.075f, .047f, .032f, .94f);
        image.raycastTarget = false;
        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(690f, 118f);
        Outline outline = card.AddComponent<Outline>();
        outline.effectColor = new Color(.53f, .30f, .12f, .32f);
        outline.effectDistance = new Vector2(1f, -1f);
        card.transform.SetAsFirstSibling();
    }

    void EnsureDescription(string name, string value, Vector2 position)
    {
        Transform existing = contentRoot.Find(name);
        Text text;
        if (existing == null)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(contentRoot, false);
            text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 12;
            text.color = new Color(.72f, .64f, .56f, .95f);
        }
        else text = existing.GetComponent<Text>();

        text.text = value;
        Place(text, position, new Vector2(330f, 24f), TextAnchor.MiddleLeft);
    }

    static bool IsText(Text text, string en, string ru)
    {
        return text != null && (text.text == en || text.text == ru);
    }

    static void Place(Text text, Vector2 position, Vector2 size, TextAnchor alignment)
    {
        if (text == null) return;
        RectTransform rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        text.alignment = alignment;
    }

    static void PlaceSlider(Slider slider, Vector2 position)
    {
        if (slider == null) return;
        RectTransform rect = slider.transform as RectTransform;
        if (rect == null) return;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(470f, 36f);
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}
