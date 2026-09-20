using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class ModernSettingsPresentation : MonoBehaviour
{
    enum Tab { Audio, Video, Gameplay, Controls }

    Canvas canvas;
    GameMenuController menu;
    GameObject settingsRoot;
    GameObject legacyPanel;
    GameObject modernPanel;
    GameObject contentRoot;
    Text descriptionText;
    readonly Button[] tabs = new Button[4];
    Tab activeTab = Tab.Audio;
    bool built;

    static readonly Vector2[] Resolutions =
    {
        new Vector2(1366,768), new Vector2(1600,900), new Vector2(1920,1080),
        new Vector2(2560,1440), new Vector2(3840,2160)
    };
    static readonly int[] FpsOptions = { 30, 60, 120, 144, -1 };

    int resolutionIndex;
    int fpsIndex;

    string L(string en, string ru) => GameLanguage.T(en, ru);
    CampaignDifficulty CurrentDifficulty => CampaignController.Instance != null ? CampaignController.Instance.Difficulty : CampaignDifficulty.Story;

    IEnumerator Start()
    {
        yield return null;
        Bind();
    }

    void Update()
    {
        if (canvas == null || settingsRoot == null)
        {
            Bind();
            return;
        }

        if (settingsRoot.activeInHierarchy && !built)
            Build();
    }

    void Bind()
    {
        canvas = FindMenuCanvas();
        menu = FindFirstObjectByType<GameMenuController>();
        if (canvas == null || menu == null) return;
        Transform settings = canvas.transform.Find("Settings");
        if (settings == null) return;
        settingsRoot = settings.gameObject;
        Transform legacy = settings.Find("SettingsPanel");
        legacyPanel = legacy != null ? legacy.gameObject : null;
        built = false;
        if (settingsRoot.activeInHierarchy) Build();
    }

    Canvas FindMenuCanvas()
    {
        GameObject menuCanvas = GameObject.Find("MenuCanvas");
        return menuCanvas != null ? menuCanvas.GetComponent<Canvas>() : null;
    }

    void Build()
    {
        if (settingsRoot == null) return;
        Transform old = settingsRoot.transform.Find("ModernSettingsPanel");
        if (old != null) Destroy(old.gameObject);
        if (legacyPanel != null) legacyPanel.SetActive(false);

        FindResolution();
        FindFps();

        modernPanel = MakeRect(settingsRoot.transform, "ModernSettingsPanel", Vector2.zero, new Vector2(1448, 1086), Color.white);
        tabs[(int)Tab.Audio] = MakeButton(modernPanel.transform, L("AUDIO", "ЗВУК"), new Vector2(-327, 292), () => ShowTab(Tab.Audio), new Vector2(220,68), true);
        tabs[(int)Tab.Video] = MakeButton(modernPanel.transform, L("VIDEO", "ВИДЕО"), new Vector2(-81, 292), () => ShowTab(Tab.Video), new Vector2(220,68), false);
        tabs[(int)Tab.Gameplay] = MakeButton(modernPanel.transform, L("GAMEPLAY", "ИГРА"), new Vector2(153, 292), () => ShowTab(Tab.Gameplay), new Vector2(220,68), false);
        tabs[(int)Tab.Controls] = MakeButton(modernPanel.transform, L("CONTROLS", "УПРАВЛЕНИЕ"), new Vector2(406, 292), () => ShowTab(Tab.Controls), new Vector2(228,68), false);
        GameObject contentPanel = MakeRect(modernPanel.transform, "ContentPanel", new Vector2(63, 30), new Vector2(850, 420), Color.clear);
        contentPanel.GetComponent<Image>().raycastTarget = false;
        contentRoot = new GameObject("ContentRoot");
        contentRoot.transform.SetParent(contentPanel.transform, false);
        RectTransform cr = contentRoot.AddComponent<RectTransform>();
        cr.anchorMin = Vector2.zero;
        cr.anchorMax = Vector2.one;
        cr.offsetMin = Vector2.zero;
        cr.offsetMax = Vector2.zero;

        MakeButton(modernPanel.transform, L("BACK", "НАЗАД"), new Vector2(173,-420), Back, new Vector2(246,86), false);
        MakeButton(modernPanel.transform, L("APPLY", "ПРИМЕНИТЬ"), new Vector2(485,-420), ApplyAndBack, new Vector2(280,86), true);
        modernPanel.AddComponent<SettingsArtworkLayout>().Initialize();

        ShowTab(activeTab);
        built = true;
    }

    void ShowTab(Tab tab)
    {
        activeTab = tab;
        if (contentRoot == null) return;
        for (int i = contentRoot.transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = contentRoot.transform.GetChild(i).gameObject;
            child.SetActive(false);
            Destroy(child);
        }

        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i] == null) continue;
            Image image = tabs[i].GetComponent<Image>();
            if (image != null) image.color = i == (int)tab ? new Color(.65f,.08f,.025f,.8f) : Color.clear;
            tabs[i].GetComponentInChildren<Text>().color = i == (int)tab ? new Color(1f,.93f,.72f) : new Color(.12f,.09f,.045f);
        }

        switch (tab)
        {
            case Tab.Audio: BuildAudio(); break;
            case Tab.Video: BuildVideo(); break;
            case Tab.Gameplay: BuildGameplay(); break;
            case Tab.Controls: BuildControls(); break;
        }
        foreach (Text text in contentRoot.GetComponentsInChildren<Text>()) text.color = new Color(.10f, .075f, .04f);
        SelectFirstContentControl();
    }

    void BuildAudio()
    {
        MakeAudioSliderCard(
            L("MASTER VOLUME", "ОБЩАЯ ГРОМКОСТЬ"),
            L("All game sounds and effects", "Все звуки игры и эффекты"),
            110f,
            GameUserSettings.MasterVolume,
            value => GameUserSettings.MasterVolume = value);
        MakeAudioSliderCard(
            L("MUSIC", "МУЗЫКА"),
            L("Background music only", "Только фоновая музыка"),
            -10f,
            GameUserSettings.MusicVolume,
            value => GameUserSettings.MusicVolume = value);
    }

    void MakeAudioSliderCard(string label, string description, float y, float initial, Action<float> onChanged)
    {
        GameObject card = MakeRect(contentRoot.transform, label + " Row", new Vector2(0f, y), new Vector2(800f, 86f), Color.clear);
        card.GetComponent<Image>().raycastTarget = false;
        AddText(card.transform, label, new Vector2(-240f,0f), new Vector2(290f,65f), 23, Color.black, TextAnchor.MiddleLeft, FontStyle.Normal);

        GameObject sliderObject = new GameObject(label + " Slider", typeof(RectTransform));
        sliderObject.transform.SetParent(card.transform, false);
        Slider slider = sliderObject.AddComponent<Slider>();
        RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
        sliderRect.anchorMin = sliderRect.anchorMax = sliderRect.pivot = new Vector2(.5f,.5f);
        sliderRect.anchoredPosition = new Vector2(115f,0f);
        sliderRect.sizeDelta = new Vector2(330f,40f);

        GameObject bg = MakeRect(sliderObject.transform, "Background", Vector2.zero, new Vector2(470f,12f), new Color(.18f,.11f,.075f,1f));
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0f,.5f);
        bgRect.anchorMax = new Vector2(1f,.5f);
        bgRect.pivot = new Vector2(.5f,.5f);
        bgRect.offsetMin = new Vector2(0f,-6f);
        bgRect.offsetMax = new Vector2(0f,6f);
        bg.GetComponent<Image>().raycastTarget = false;

        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderObject.transform, false);
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0f,.5f);
        fillAreaRect.anchorMax = new Vector2(1f,.5f);
        fillAreaRect.pivot = new Vector2(.5f,.5f);
        fillAreaRect.offsetMin = new Vector2(10f,-6f);
        fillAreaRect.offsetMax = new Vector2(-10f,6f);

        GameObject fill = MakeRect(fillArea.transform, "Fill", Vector2.zero, Vector2.zero, new Color(.015f,.52f,.93f,1f));
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        fill.GetComponent<Image>().raycastTarget = false;

        GameObject handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleArea.transform.SetParent(sliderObject.transform, false);
        RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
        handleAreaRect.anchorMin = new Vector2(0f,.5f);
        handleAreaRect.anchorMax = new Vector2(1f,.5f);
        handleAreaRect.pivot = new Vector2(.5f,.5f);
        handleAreaRect.offsetMin = new Vector2(11f,-18f);
        handleAreaRect.offsetMax = new Vector2(-11f,18f);

        GameObject handle = MakeRect(handleArea.transform, "Handle", Vector2.zero, new Vector2(22f,28f), new Color(1f,.72f,.25f,1f));
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.anchorMin = handleRect.anchorMax = handleRect.pivot = new Vector2(.5f,.5f);
        handleRect.sizeDelta = new Vector2(42f, 4f);
        handle.GetComponent<Image>().sprite = modernPanel.GetComponent<SettingsArtworkLayout>().KnobSprite;
        handle.GetComponent<Image>().color = Color.white;

        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handle.GetComponent<Image>();
        slider.direction = Slider.Direction.LeftToRight;
        slider.wholeNumbers = false;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = initial;

        Text valueText = AddText(card.transform, Mathf.RoundToInt(initial * 100f) + "%", new Vector2(350f,0f), new Vector2(90f,40f), 24, Color.black, TextAnchor.MiddleRight, FontStyle.Normal);
        slider.onValueChanged.AddListener(value =>
        {
            valueText.text = Mathf.RoundToInt(value * 100f) + "%";
            onChanged(value);
        });
    }

    void BuildVideo()
    {
        SetHeader(L("VIDEO", "ВИДЕО"), L("Display, frame pacing and graphics quality.", "Экран, частота кадров и качество графики."));
        MakeSelectorRow(L("WINDOW MODE", "РЕЖИМ ЭКРАНА"), 115, () => GameUserSettings.Fullscreen ? L("FULLSCREEN", "ПОЛНЫЙ ЭКРАН") : L("WINDOWED", "ОКОННЫЙ"), () => { GameUserSettings.Fullscreen = !GameUserSettings.Fullscreen; ShowTab(Tab.Video); });
        MakeSelectorRow(L("RESOLUTION", "РАЗРЕШЕНИЕ"), 35, ResolutionLabel, CycleResolution);
        MakeSelectorRow("VSync", -45, () => GameUserSettings.VSync ? L("ON", "ВКЛ") : L("OFF", "ВЫКЛ"), () => { GameUserSettings.VSync = !GameUserSettings.VSync; ShowTab(Tab.Video); });
        MakeSelectorRow(L("FPS LIMIT", "ЛИМИТ FPS"), -125, () => GameUserSettings.FpsLimit <= 0 ? L("UNLIMITED", "БЕЗ ЛИМИТА") : GameUserSettings.FpsLimit.ToString(), CycleFps);
        MakeSelectorRow(L("GRAPHICS QUALITY", "КАЧЕСТВО ГРАФИКИ"), -205, QualityLabel, CycleQuality);
    }

    void BuildGameplay()
    {
        SetHeader(L("GAMEPLAY", "ИГРА"), L("Campaign preferences and language.", "Параметры кампании и язык."));
        MakeSelectorRow(L("LANGUAGE", "ЯЗЫК"), 105, () => GameLanguage.Russian ? "Русский" : "English", ToggleLanguage);
        MakeSelectorRow(L("DIFFICULTY", "СЛОЖНОСТЬ"), -15, () => DifficultyRules.Label(CurrentDifficulty), CycleDifficulty);
        MakeButton(contentRoot.transform, L("RESET DEFAULTS", "СБРОСИТЬ НАСТРОЙКИ"), new Vector2(160,-100), ResetDefaults, new Vector2(300,52), false);
        AddHint(L("Difficulty changes campaign combat rules. Language change rebuilds the menu.", "Сложность меняет правила боя. Смена языка перестраивает меню."), new Vector2(0,-135));
    }

    void BuildControls()
    {
        SetHeader(L("CONTROLS", "УПРАВЛЕНИЕ"), L("PC keyboard and mouse controls.", "Управление на ПК: клавиатура и мышь."));
        MakeControlRow(L("CAMERA MOVE", "КАМЕРА"), L("WASD / ARROWS", "WASD / СТРЕЛКИ"), 130);
        MakeControlRow(L("SELECT / BUILD", "ВЫБОР / СТРОИТЕЛЬСТВО"), L("LEFT MOUSE BUTTON", "ЛЕВАЯ КНОПКА МЫШИ"), 70);
        MakeControlRow(L("HECTOR MOVE", "ДВИЖЕНИЕ ГЕКТОРА"), L("RIGHT MOUSE BUTTON", "ПРАВАЯ КНОПКА МЫШИ"), 10);
        MakeControlRow(L("ZOOM", "МАСШТАБ"), L("MOUSE WHEEL", "КОЛЕСО МЫШИ"), -50);
        MakeControlRow(L("SCREENSHOT", "СНИМОК ЭКРАНА"), "F12", -110);
        MakeControlRow(L("PAUSE / BACK", "ПАУЗА / НАЗАД"), "ESC", -170);
        AddHint(L("Tower hotkeys: 1–6. Hector: click to select, RMB to move, Q / E / R / F abilities. Screenshots are saved to the game data Screenshots folder.", "Оборона: 1–6. Гектор: ЛКМ выбрать, ПКМ двигаться, способности Q / E / R / F. Снимки сохраняются в папку Screenshots данных игры."), new Vector2(0,-240));
    }

    void SetHeader(string title, string description)
    {
        if (descriptionText != null) descriptionText.text = description;
    }

    void MakeSelectorRow(string label, float y, Func<string> value, UnityAction action)
    {
        y = 90f + y * .75f;
        AddText(contentRoot.transform,label,new Vector2(-230,y),new Vector2(300,46),16,Color.white,TextAnchor.MiddleLeft,FontStyle.Bold);
        MakeButton(contentRoot.transform,value(),new Vector2(160,y),action,new Vector2(300,52),false);
    }

    void MakeControlRow(string label, string value, float y)
    {
        y = 65f + y * .75f;
        MakeRect(contentRoot.transform,"ControlRow",new Vector2(0,y),new Vector2(690,58),Color.clear).GetComponent<Image>().raycastTarget = false;
        AddText(contentRoot.transform,label,new Vector2(-205,y),new Vector2(300,44),15,new Color(.94f,.88f,.80f,1f),TextAnchor.MiddleLeft,FontStyle.Bold);
        AddText(contentRoot.transform,value,new Vector2(205,y),new Vector2(300,44),15,new Color(1f,.72f,.30f,1f),TextAnchor.MiddleRight,FontStyle.Bold);
    }

    void AddHint(string text, Vector2 pos) { }

    void CycleResolution() { resolutionIndex=(resolutionIndex+1)%Resolutions.Length; Vector2 r=Resolutions[resolutionIndex]; GameUserSettings.SetResolution((int)r.x,(int)r.y); ShowTab(Tab.Video); }
    void CycleFps() { fpsIndex=(fpsIndex+1)%FpsOptions.Length; GameUserSettings.FpsLimit=FpsOptions[fpsIndex]; ShowTab(Tab.Video); }
    void CycleQuality() { if(QualitySettings.names.Length==0)return; QualitySettings.SetQualityLevel((QualitySettings.GetQualityLevel()+1)%QualitySettings.names.Length,true); ShowTab(Tab.Video); }
    void CycleDifficulty() { if(CampaignController.Instance!=null) CampaignController.Instance.CycleDifficulty(); ShowTab(Tab.Gameplay); }
    void ToggleLanguage() { if(menu!=null) menu.SendMessage("ToggleLanguage",SendMessageOptions.DontRequireReceiver); built=false; StartCoroutine(RebindAfterLanguage()); }
    IEnumerator RebindAfterLanguage(){ yield return null; yield return null; Bind(); }
    string ResolutionLabel(){ Vector2 r=Resolutions[Mathf.Clamp(resolutionIndex,0,Resolutions.Length-1)]; return $"{(int)r.x} × {(int)r.y}"; }
    string QualityLabel(){ string[] n=QualitySettings.names; int index=Mathf.Clamp(QualitySettings.GetQualityLevel(),0,Mathf.Max(0,n.Length-1)); if(n.Length==0) return L("Default","ПО УМОЛЧАНИЮ"); return GameLanguage.Russian ? $"УРОВЕНЬ {index+1}" : n[index]; }

    void FindResolution(){ float best=float.MaxValue; for(int i=0;i<Resolutions.Length;i++){ float d=Mathf.Abs(Resolutions[i].x-GameUserSettings.ResolutionWidth)+Mathf.Abs(Resolutions[i].y-GameUserSettings.ResolutionHeight); if(d<best){best=d;resolutionIndex=i;}} }
    void FindFps(){ fpsIndex=1; for(int i=0;i<FpsOptions.Length;i++) if(FpsOptions[i]==GameUserSettings.FpsLimit){fpsIndex=i;break;} }

    void ApplyAndBack(){ GameUserSettings.Save(); Back(); }
    void ResetDefaults(){ GameUserSettings.ResetToDefaults(); FindResolution(); FindFps(); ShowTab(activeTab); }
    void Back(){ if(menu!=null) menu.SendMessage("BackFromSettings",SendMessageOptions.DontRequireReceiver); }

    void SelectFirstContentControl()
    {
        if(EventSystem.current==null||contentRoot==null)return;
        Button button=contentRoot.GetComponentInChildren<Button>(true);
        if(button!=null) EventSystem.current.SetSelectedGameObject(button.gameObject);
        else
        {
            Slider slider=contentRoot.GetComponentInChildren<Slider>(true);
            if(slider!=null) EventSystem.current.SetSelectedGameObject(slider.gameObject);
        }
    }

    GameObject MakePanel(Transform parent,string name,Vector2 pos,Vector2 size,Color color){ GameObject go=MakeRect(parent,name,pos,size,color); Outline o=go.AddComponent<Outline>(); o.effectColor=new Color(.68f,.37f,.14f,.42f); o.effectDistance=new Vector2(1.5f,-1.5f); return go; }
    GameObject MakeRect(Transform parent,string name,Vector2 pos,Vector2 size,Color color){ GameObject go=new GameObject(name); go.transform.SetParent(parent,false); Image im=go.AddComponent<Image>(); im.color=color; RectTransform rt=im.rectTransform; rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f); rt.anchoredPosition=pos; rt.sizeDelta=size; return go; }
    Button MakeButton(Transform parent,string label,Vector2 pos,UnityAction action,Vector2 size,bool primary)
    {
        bool artworkButton = parent == modernPanel.transform;
        GameObject go = MakeRect(parent, label, pos, size, artworkButton ? Color.clear : new Color(.83f,.65f,.38f,.8f));
        Button button = go.AddComponent<Button>();
        button.targetGraphic = go.GetComponent<Image>();
        button.onClick.AddListener(action);
        go.AddComponent<MenuUiAudioFeedback>();
        AddText(go.transform, label, Vector2.zero, size, artworkButton ? 26 : 17,
            artworkButton && primary ? new Color(1f,.94f,.73f) : new Color(.10f,.075f,.04f), TextAnchor.MiddleCenter, FontStyle.Bold);
        return button;
    }
    Text AddText(Transform parent,string value,Vector2 pos,Vector2 size,int fontSize,Color color,TextAnchor align,FontStyle style){ GameObject go=new GameObject("Text"); go.transform.SetParent(parent,false); Text t=go.AddComponent<Text>(); t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); t.text=value; t.fontSize=fontSize; t.color=color; t.alignment=align; t.fontStyle=style; t.horizontalOverflow=HorizontalWrapMode.Wrap; RectTransform rt=t.rectTransform; rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f); rt.anchoredPosition=pos; rt.sizeDelta=size; return t; }
}
