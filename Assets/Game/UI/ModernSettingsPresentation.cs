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
    Text titleText;
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

        modernPanel = MakePanel(settingsRoot.transform, "ModernSettingsPanel", Vector2.zero, new Vector2(1320, 800), new Color(.055f,.03f,.018f,.985f));
        AddText(modernPanel.transform, L("SETTINGS", "НАСТРОЙКИ"), new Vector2(-455, 335), new Vector2(360,56), 38, new Color(1f,.62f,.18f,1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        AddText(modernPanel.transform, L("Customize your experience", "Настройте игру под себя"), new Vector2(-455, 296), new Vector2(430,32), 16, new Color(.78f,.69f,.60f,1f), TextAnchor.MiddleLeft, FontStyle.Normal);

        GameObject nav = MakePanel(modernPanel.transform, "Navigation", new Vector2(-475, -5), new Vector2(260, 550), new Color(.075f,.042f,.026f,.92f));
        tabs[(int)Tab.Audio] = MakeButton(nav.transform, L("AUDIO", "ЗВУК"), new Vector2(0, 190), () => ShowTab(Tab.Audio), new Vector2(215,56), true);
        tabs[(int)Tab.Video] = MakeButton(nav.transform, L("VIDEO", "ВИДЕО"), new Vector2(0, 120), () => ShowTab(Tab.Video), new Vector2(215,56), false);
        tabs[(int)Tab.Gameplay] = MakeButton(nav.transform, L("GAMEPLAY", "ИГРА"), new Vector2(0, 50), () => ShowTab(Tab.Gameplay), new Vector2(215,56), false);
        tabs[(int)Tab.Controls] = MakeButton(nav.transform, L("CONTROLS", "УПРАВЛЕНИЕ"), new Vector2(0, -20), () => ShowTab(Tab.Controls), new Vector2(215,56), false);
        descriptionText = AddText(nav.transform, "", new Vector2(0,-175), new Vector2(215,130), 14, new Color(.72f,.64f,.56f,.92f), TextAnchor.UpperLeft, FontStyle.Normal);

        GameObject contentPanel = MakePanel(modernPanel.transform, "ContentPanel", new Vector2(170, -5), new Vector2(800, 550), new Color(.035f,.022f,.016f,.92f));
        contentRoot = new GameObject("ContentRoot");
        contentRoot.transform.SetParent(contentPanel.transform, false);
        RectTransform cr = contentRoot.AddComponent<RectTransform>();
        cr.anchorMin = Vector2.zero; cr.anchorMax = Vector2.one; cr.offsetMin = cr.offsetMax = Vector2.zero;

        MakeButton(modernPanel.transform, L("RESET DEFAULTS", "СБРОСИТЬ"), new Vector2(-250,-350), ResetDefaults, new Vector2(270,54), false);
        MakeButton(modernPanel.transform, L("BACK", "НАЗАД"), new Vector2(80,-350), Back, new Vector2(240,54), false);
        MakeButton(modernPanel.transform, L("APPLY", "ПРИМЕНИТЬ"), new Vector2(370,-350), ApplyAndBack, new Vector2(250,54), true);

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
            if (image != null) image.color = i == (int)tab ? new Color(.55f,.11f,.045f,.98f) : new Color(.22f,.13f,.08f,.96f);
        }

        switch (tab)
        {
            case Tab.Audio: BuildAudio(); break;
            case Tab.Video: BuildVideo(); break;
            case Tab.Gameplay: BuildGameplay(); break;
            case Tab.Controls: BuildControls(); break;
        }
        SelectFirstContentControl();
    }

    void BuildAudio()
    {
        SetHeader(L("AUDIO", "ЗВУК"), L("Balance music and overall game volume.", "Настройте музыку и общую громкость игры."));
        MakeSliderRow(L("MASTER VOLUME", "ОБЩАЯ ГРОМКОСТЬ"), 105, GameUserSettings.MasterVolume, value => GameUserSettings.MasterVolume = value);
        MakeSliderRow(L("MUSIC", "МУЗЫКА"), -35, GameUserSettings.MusicVolume, value => GameUserSettings.MusicVolume = value);
        AddHint(L("Changes are previewed immediately.", "Изменения слышны сразу."), new Vector2(0,-150));
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
        AddHint(L("Difficulty changes campaign combat rules. Language change rebuilds the menu.", "Сложность меняет правила боя. Смена языка перестраивает меню."), new Vector2(0,-135));
    }

    void BuildControls()
    {
        SetHeader(L("CONTROLS", "УПРАВЛЕНИЕ"), L("PC keyboard and mouse controls.", "Управление на ПК: клавиатура и мышь."));
        MakeControlRow(L("CAMERA MOVE", "КАМЕРА"), "WASD / ARROWS", 110);
        MakeControlRow(L("SELECT / BUILD", "ВЫБОР / СТРОИТЕЛЬСТВО"), L("LEFT MOUSE BUTTON", "ЛЕВАЯ КНОПКА МЫШИ"), 40);
        MakeControlRow(L("HECTOR MOVE", "ДВИЖЕНИЕ ГЕКТОРА"), L("RIGHT MOUSE BUTTON", "ПРАВАЯ КНОПКА МЫШИ"), -30);
        MakeControlRow(L("ZOOM", "МАСШТАБ"), L("MOUSE WHEEL", "КОЛЕСО МЫШИ"), -100);
        MakeControlRow(L("PAUSE / BACK", "ПАУЗА / НАЗАД"), "ESC", -170);
        AddHint(L("Tower hotkeys: 1–6. Hector: click to select, RMB to move, Q / E / R / F abilities.", "Оборона: 1–6. Гектор: ЛКМ выбрать, ПКМ двигаться, способности Q / E / R / F."), new Vector2(0,-240));
    }

    void SetHeader(string title, string description)
    {
        titleText = AddText(contentRoot.transform, title, new Vector2(-245,226), new Vector2(580,48), 28, new Color(1f,.72f,.30f,1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        AddText(contentRoot.transform, description, new Vector2(-245,178), new Vector2(620,38), 15, new Color(.76f,.68f,.59f,1f), TextAnchor.MiddleLeft, FontStyle.Normal);
        if (descriptionText != null) descriptionText.text = description;
    }

    void MakeSliderRow(string label, float y, float initial, Action<float> onChanged)
    {
        AddText(contentRoot.transform, label, new Vector2(-230,y+26), new Vector2(330,34), 16, Color.white, TextAnchor.MiddleLeft, FontStyle.Bold);
        GameObject go = new GameObject(label + " Slider"); go.transform.SetParent(contentRoot.transform,false);
        Slider slider = go.AddComponent<Slider>(); RectTransform rt = go.GetComponent<RectTransform>(); rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f); rt.anchoredPosition=new Vector2(-35,y-16); rt.sizeDelta=new Vector2(470,28);
        GameObject bg = MakeRect(go.transform,"Background",Vector2.zero,new Vector2(470,12),new Color(.18f,.11f,.075f,1f));
        GameObject fill = MakeRect(go.transform,"Fill",Vector2.zero,new Vector2(470,12),new Color(.72f,.20f,.06f,1f));
        slider.fillRect = fill.GetComponent<RectTransform>();
        GameObject handle = MakeRect(go.transform,"Handle",Vector2.zero,new Vector2(22,34),new Color(1f,.72f,.25f,1f));
        slider.handleRect = handle.GetComponent<RectTransform>(); slider.targetGraphic=handle.GetComponent<Image>(); slider.minValue=0; slider.maxValue=1; slider.value=initial;
        Text value = AddText(contentRoot.transform, Mathf.RoundToInt(initial*100)+"%", new Vector2(255,y-16), new Vector2(90,34), 16, new Color(1f,.82f,.52f,1f), TextAnchor.MiddleRight, FontStyle.Bold);
        slider.onValueChanged.AddListener(v => { value.text=Mathf.RoundToInt(v*100)+"%"; onChanged(v); });
    }

    void MakeSelectorRow(string label, float y, Func<string> value, UnityAction action)
    {
        AddText(contentRoot.transform,label,new Vector2(-230,y),new Vector2(300,46),16,Color.white,TextAnchor.MiddleLeft,FontStyle.Bold);
        MakeButton(contentRoot.transform,value(),new Vector2(160,y),action,new Vector2(300,52),false);
    }

    void MakeControlRow(string label, string value, float y)
    {
        MakeRect(contentRoot.transform,"ControlRow",new Vector2(0,y),new Vector2(690,58),new Color(.075f,.047f,.032f,.95f));
        AddText(contentRoot.transform,label,new Vector2(-205,y),new Vector2(300,44),15,new Color(.94f,.88f,.80f,1f),TextAnchor.MiddleLeft,FontStyle.Bold);
        AddText(contentRoot.transform,value,new Vector2(205,y),new Vector2(300,44),15,new Color(1f,.72f,.30f,1f),TextAnchor.MiddleRight,FontStyle.Bold);
    }

    void AddHint(string text, Vector2 pos) => AddText(contentRoot.transform,text,pos,new Vector2(650,54),13,new Color(.68f,.61f,.54f,.9f),TextAnchor.UpperLeft,FontStyle.Normal);

    void CycleResolution() { resolutionIndex=(resolutionIndex+1)%Resolutions.Length; Vector2 r=Resolutions[resolutionIndex]; GameUserSettings.SetResolution((int)r.x,(int)r.y); ShowTab(Tab.Video); }
    void CycleFps() { fpsIndex=(fpsIndex+1)%FpsOptions.Length; GameUserSettings.FpsLimit=FpsOptions[fpsIndex]; ShowTab(Tab.Video); }
    void CycleQuality() { if(QualitySettings.names.Length==0)return; QualitySettings.SetQualityLevel((QualitySettings.GetQualityLevel()+1)%QualitySettings.names.Length,true); ShowTab(Tab.Video); }
    void CycleDifficulty() { if(CampaignController.Instance!=null) CampaignController.Instance.CycleDifficulty(); ShowTab(Tab.Gameplay); }
    void ToggleLanguage() { if(menu!=null) menu.SendMessage("ToggleLanguage",SendMessageOptions.DontRequireReceiver); built=false; StartCoroutine(RebindAfterLanguage()); }
    IEnumerator RebindAfterLanguage(){ yield return null; yield return null; Bind(); }
    string ResolutionLabel(){ Vector2 r=Resolutions[Mathf.Clamp(resolutionIndex,0,Resolutions.Length-1)]; return $"{(int)r.x} × {(int)r.y}"; }
    string QualityLabel(){ string[] n=QualitySettings.names; return n.Length>0?n[Mathf.Clamp(QualitySettings.GetQualityLevel(),0,n.Length-1)]:"Default"; }

    void FindResolution(){ float best=float.MaxValue; for(int i=0;i<Resolutions.Length;i++){ float d=Mathf.Abs(Resolutions[i].x-GameUserSettings.ResolutionWidth)+Mathf.Abs(Resolutions[i].y-GameUserSettings.ResolutionHeight); if(d<best){best=d;resolutionIndex=i;}} }
    void FindFps(){ fpsIndex=1; for(int i=0;i<FpsOptions.Length;i++) if(FpsOptions[i]==GameUserSettings.FpsLimit){fpsIndex=i;break;} }

    void ApplyAndBack(){ GameUserSettings.Save(); Back(); }
    void ResetDefaults(){ GameUserSettings.ResetToDefaults(); FindResolution(); FindFps(); ShowTab(activeTab); }
    void Back(){ if(menu!=null) menu.SendMessage("BackFromSettings",SendMessageOptions.DontRequireReceiver); }

    void SelectFirstContentControl(){ if(EventSystem.current==null||contentRoot==null)return; Button b=contentRoot.GetComponentInChildren<Button>(true); if(b!=null) EventSystem.current.SetSelectedGameObject(b.gameObject); }

    GameObject MakePanel(Transform parent,string name,Vector2 pos,Vector2 size,Color color){ GameObject go=MakeRect(parent,name,pos,size,color); Outline o=go.AddComponent<Outline>(); o.effectColor=new Color(.68f,.37f,.14f,.42f); o.effectDistance=new Vector2(1.5f,-1.5f); return go; }
    GameObject MakeRect(Transform parent,string name,Vector2 pos,Vector2 size,Color color){ GameObject go=new GameObject(name); go.transform.SetParent(parent,false); Image im=go.AddComponent<Image>(); im.color=color; RectTransform rt=im.rectTransform; rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f); rt.anchoredPosition=pos; rt.sizeDelta=size; return go; }
    Button MakeButton(Transform parent,string label,Vector2 pos,UnityAction action,Vector2 size,bool primary){ GameObject go=MakeRect(parent,label,pos,size,primary?new Color(.55f,.11f,.045f,.98f):new Color(.22f,.13f,.08f,.96f)); Button b=go.AddComponent<Button>(); b.targetGraphic=go.GetComponent<Image>(); b.onClick.AddListener(action); go.AddComponent<MenuButtonFeedback>(); go.AddComponent<MenuUiAudioFeedback>(); Navigation nav=b.navigation; nav.mode=Navigation.Mode.Automatic; b.navigation=nav; AddText(go.transform,label,Vector2.zero,size,17,primary?new Color(1f,.88f,.50f,1f):new Color(.92f,.82f,.69f,1f),TextAnchor.MiddleCenter,FontStyle.Bold); return b; }
    Text AddText(Transform parent,string value,Vector2 pos,Vector2 size,int fontSize,Color color,TextAnchor align,FontStyle style){ GameObject go=new GameObject("Text"); go.transform.SetParent(parent,false); Text t=go.AddComponent<Text>(); t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); t.text=value; t.fontSize=fontSize; t.color=color; t.alignment=align; t.fontStyle=style; t.horizontalOverflow=HorizontalWrapMode.Wrap; RectTransform rt=t.rectTransform; rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f); rt.anchoredPosition=pos; rt.sizeDelta=size; return t; }
}
