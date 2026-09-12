using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
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
        canvas = FindFirstObjectByType<Canvas>();
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

    void Build()
    {
        if (settingsRoot == null) return;
        Transform old = settingsRoot.transform.Find("ModernSettingsPanel");
        if (old != null) Destroy(old.gameObject);
        if (legacyPanel != null) legacyPanel.SetActive(false);

        FindResolution();
        FindFps();

        modernPanel = MakePanel(settingsRoot.transform, "ModernSettingsPanel", Vector2.zero, new Vector2(1480, 900), new Color(.055f,.03f,.018f,.985f));
        AddText(modernPanel.transform, L("SETTINGS", "НАСТРОЙКИ"), new Vector2(-520, 385), new Vector2(360,60), 42, new Color(1f,.62f,.18f,1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        AddText(modernPanel.transform, L("Customize your experience", "Настройте игру под себя"), new Vector2(-520, 342), new Vector2(430,36), 17, new Color(.78f,.69f,.60f,1f), TextAnchor.MiddleLeft, FontStyle.Normal);

        GameObject nav = MakePanel(modernPanel.transform, "Navigation", new Vector2(-545, -20), new Vector2(300, 650), new Color(.075f,.042f,.026f,.92f));
        tabs[(int)Tab.Audio] = MakeButton(nav.transform, L("AUDIO", "ЗВУК"), new Vector2(0, 230), () => ShowTab(Tab.Audio), new Vector2(245,62), true);
        tabs[(int)Tab.Video] = MakeButton(nav.transform, L("VIDEO", "ВИДЕО"), new Vector2(0, 150), () => ShowTab(Tab.Video), new Vector2(245,62), false);
        tabs[(int)Tab.Gameplay] = MakeButton(nav.transform, L("GAMEPLAY", "ИГРА"), new Vector2(0, 70), () => ShowTab(Tab.Gameplay), new Vector2(245,62), false);
        tabs[(int)Tab.Controls] = MakeButton(nav.transform, L("CONTROLS", "УПРАВЛЕНИЕ"), new Vector2(0, -10), () => ShowTab(Tab.Controls), new Vector2(245,62), false);

        descriptionText = AddText(nav.transform, "", new Vector2(0,-190), new Vector2(245,150), 15, new Color(.72f,.64f,.56f,.92f), TextAnchor.UpperLeft, FontStyle.Normal);

        GameObject contentPanel = MakePanel(modernPanel.transform, "ContentPanel", new Vector2(230, -20), new Vector2(900, 650), new Color(.035f,.022f,.016f,.92f));
        contentRoot = new GameObject("ContentRoot");
        contentRoot.transform.SetParent(contentPanel.transform, false);
        RectTransform cr = contentRoot.AddComponent<RectTransform>();
        cr.anchorMin = Vector2.zero; cr.anchorMax = Vector2.one; cr.offsetMin = cr.offsetMax = Vector2.zero;

        MakeButton(modernPanel.transform, L("APPLY", "ПРИМЕНИТЬ"), new Vector2(445,-400), ApplyAndBack, new Vector2(260,58), true);
        MakeButton(modernPanel.transform, L("BACK", "НАЗАД"), new Vector2(145,-400), Back, new Vector2(260,58), false);
        MakeButton(modernPanel.transform, L("RESET DEFAULTS", "СБРОСИТЬ"), new Vector2(-205,-400), ResetDefaults, new Vector2(310,58), false);

        ShowTab(activeTab);
        built = true;
    }

    void ShowTab(Tab tab)
    {
        activeTab = tab;
        if (contentRoot == null) return;
        for (int i = contentRoot.transform.childCount - 1; i >= 0; i--) Destroy(contentRoot.transform.GetChild(i).gameObject);

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
        MakeSliderRow(L("MASTER VOLUME", "ОБЩАЯ ГРОМКОСТЬ"), 190, GameUserSettings.MasterVolume, value => GameUserSettings.MasterVolume = value);
        MakeSliderRow(L("MUSIC", "МУЗЫКА"), 70, GameUserSettings.MusicVolume, value => GameUserSettings.MusicVolume = value);
        AddHint(L("Changes are previewed immediately.", "Изменения слышны сразу."), new Vector2(0,-125));
    }

    void BuildVideo()
    {
        SetHeader(L("VIDEO", "ВИДЕО"), L("Display, frame pacing and graphics quality.", "Экран, частота кадров и качество графики."));
        MakeSelectorRow(L("WINDOW MODE", "РЕЖИМ ЭКРАНА"), 210, () => GameUserSettings.Fullscreen ? L("FULLSCREEN", "ПОЛНЫЙ ЭКРАН") : L("WINDOWED", "ОКОННЫЙ"), () => { GameUserSettings.Fullscreen = !GameUserSettings.Fullscreen; ShowTab(Tab.Video); });
        MakeSelectorRow(L("RESOLUTION", "РАЗРЕШЕНИЕ"), 110, ResolutionLabel, CycleResolution);
        MakeSelectorRow("VSync", 10, () => GameUserSettings.VSync ? L("ON", "ВКЛ") : L("OFF", "ВЫКЛ"), () => { GameUserSettings.VSync = !GameUserSettings.VSync; ShowTab(Tab.Video); });
        MakeSelectorRow(L("FPS LIMIT", "ЛИМИТ FPS"), -90, () => GameUserSettings.FpsLimit <= 0 ? L("UNLIMITED", "БЕЗ ЛИМИТА") : GameUserSettings.FpsLimit.ToString(), CycleFps);
        MakeSelectorRow(L("GRAPHICS QUALITY", "КАЧЕСТВО ГРАФИКИ"), -190, QualityLabel, CycleQuality);
    }

    void BuildGameplay()
    {
        SetHeader(L("GAMEPLAY", "ИГРА"), L("Campaign preferences and language.", "Параметры кампании и язык."));
        MakeSelectorRow(L("LANGUAGE", "ЯЗЫК"), 170, () => GameLanguage.Russian ? "Русский" : "English", ToggleLanguage);
        MakeSelectorRow(L("DIFFICULTY", "СЛОЖНОСТЬ"), 50, () => DifficultyRules.Label(CampaignSave.Difficulty), CycleDifficulty);
        AddHint(L("Difficulty changes campaign combat rules. Language change rebuilds the menu.", "Сложность меняет правила боя. Смена языка перестраивает меню."), new Vector2(0,-100));
    }

    void BuildControls()
    {
        SetHeader(L("CONTROLS", "УПРАВЛЕНИЕ"), L("Current control scheme. Runtime rebinding is not implemented yet.", "Текущая схема управления. Переназначение клавиш пока не реализовано."));
        MakeControlRow(L("CAMERA MOVE", "КАМЕРА"), "WASD / ARROWS", 190);
        MakeControlRow(L("SELECT / BUILD", "ВЫБОР / СТРОИТЕЛЬСТВО"), L("MOUSE / CONFIRM", "МЫШЬ / ПОДТВЕРДИТЬ"), 95);
        MakeControlRow(L("ZOOM", "МАСШТАБ"), L("MOUSE WHEEL", "КОЛЕСО МЫШИ"), 0);
        MakeControlRow(L("PAUSE / BACK", "ПАУЗА / НАЗАД"), "ESC / START", -95);
        AddHint(L("Controller UI navigation is enabled. Full remapping should be implemented as a separate input-system pass.", "Навигация интерфейса с геймпада включена. Полное переназначение нужно реализовать отдельным этапом Input System."), new Vector2(0,-220));
    }

    void SetHeader(string title, string description)
    {
        titleText = AddText(contentRoot.transform, title, new Vector2(-300,255), new Vector2(650,55), 30, new Color(1f,.72f,.30f,1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        AddText(contentRoot.transform, description, new Vector2(-300,215), new Vector2(720,45), 16, new Color(.76f,.68f,.59f,1f), TextAnchor.MiddleLeft, FontStyle.Normal);
        if (descriptionText != null) descriptionText.text = description;
    }

    void MakeSliderRow(string label, float y, float initial, Action<float> onChanged)
    {
        AddText(contentRoot.transform, label, new Vector2(-265,y+28), new Vector2(420,36), 17, Color.white, TextAnchor.MiddleLeft, FontStyle.Bold);
        GameObject go = new GameObject(label + " Slider"); go.transform.SetParent(contentRoot.transform,false);
        Slider slider = go.AddComponent<Slider>(); RectTransform rt = go.GetComponent<RectTransform>(); rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f); rt.anchoredPosition=new Vector2(-70,y-18); rt.sizeDelta=new Vector2(540,30);
        GameObject bg = MakeRect(go.transform,"Background",Vector2.zero,new Vector2(540,12),new Color(.18f,.11f,.075f,1f));
        GameObject fill = MakeRect(go.transform,"Fill",Vector2.zero,new Vector2(540,12),new Color(.72f,.20f,.06f,1f));
        slider.fillRect = fill.GetComponent<RectTransform>();
        GameObject handle = MakeRect(go.transform,"Handle",Vector2.zero,new Vector2(22,36),new Color(1f,.72f,.25f,1f));
        slider.handleRect = handle.GetComponent<RectTransform>(); slider.targetGraphic=handle.GetComponent<Image>(); slider.minValue=0; slider.maxValue=1; slider.value=initial;
        Text value = AddText(contentRoot.transform, Mathf.RoundToInt(initial*100)+"%", new Vector2(280,y-18), new Vector2(100,36), 17, new Color(1f,.82f,.52f,1f), TextAnchor.MiddleRight, FontStyle.Bold);
        slider.onValueChanged.AddListener(v => { value.text=Mathf.RoundToInt(v*100)+"%"; onChanged(v); });
    }

    void MakeSelectorRow(string label, float y, Func<string> value, Action action)
    {
        AddText(contentRoot.transform,label,new Vector2(-270,y),new Vector2(350,50),17,Color.white,TextAnchor.MiddleLeft,FontStyle.Bold);
        MakeButton(contentRoot.transform,value(),new Vector2(200,y),action,new Vector2(360,56),false);
    }

    void MakeControlRow(string label, string value, float y)
    {
        MakeRect(contentRoot.transform,"ControlRow",new Vector2(0,y),new Vector2(760,66),new Color(.075f,.047f,.032f,.95f));
        AddText(contentRoot.transform,label,new Vector2(-230,y),new Vector2(330,50),16,new Color(.94f,.88f,.80f,1f),TextAnchor.MiddleLeft,FontStyle.Bold);
        AddText(contentRoot.transform,value,new Vector2(220,y),new Vector2(330,50),16,new Color(1f,.72f,.30f,1f),TextAnchor.MiddleRight,FontStyle.Bold);
    }

    void AddHint(string text, Vector2 pos) => AddText(contentRoot.transform,text,pos,new Vector2(720,90),14,new Color(.68f,.61f,.54f,.9f),TextAnchor.UpperLeft,FontStyle.Normal);

    void CycleResolution() { resolutionIndex=(resolutionIndex+1)%Resolutions.Length; Vector2 r=Resolutions[resolutionIndex]; GameUserSettings.SetResolution((int)r.x,(int)r.y); ShowTab(Tab.Video); }
    void CycleFps() { fpsIndex=(fpsIndex+1)%FpsOptions.Length; GameUserSettings.FpsLimit=FpsOptions[fpsIndex]; ShowTab(Tab.Video); }
    void CycleQuality() { if(QualitySettings.names.Length==0)return; QualitySettings.SetQualityLevel((QualitySettings.GetQualityLevel()+1)%QualitySettings.names.Length,true); ShowTab(Tab.Video); }
    void CycleDifficulty() { if(CampaignController.Instance!=null) CampaignController.Instance.CycleDifficulty(); else CampaignSave.CycleDifficulty(); ShowTab(Tab.Gameplay); }
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
    Button MakeButton(Transform parent,string label,Vector2 pos,UnityEngine.Events.UnityAction action,Vector2 size,bool primary){ GameObject go=MakeRect(parent,label,pos,size,primary?new Color(.55f,.11f,.045f,.98f):new Color(.22f,.13f,.08f,.96f)); Button b=go.AddComponent<Button>(); b.targetGraphic=go.GetComponent<Image>(); b.onClick.AddListener(action); go.AddComponent<MenuButtonFeedback>(); go.AddComponent<MenuUiAudioFeedback>(); Navigation nav=b.navigation; nav.mode=Navigation.Mode.Automatic; b.navigation=nav; AddText(go.transform,label,Vector2.zero,size,17,primary?new Color(1f,.88f,.50f,1f):new Color(.92f,.82f,.69f,1f),TextAnchor.MiddleCenter,FontStyle.Bold); return b; }
    Text AddText(Transform parent,string value,Vector2 pos,Vector2 size,int fontSize,Color color,TextAnchor align,FontStyle style){ GameObject go=new GameObject("Text"); go.transform.SetParent(parent,false); Text t=go.AddComponent<Text>(); t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); t.text=value; t.fontSize=fontSize; t.color=color; t.alignment=align; t.fontStyle=style; t.horizontalOverflow=HorizontalWrapMode.Wrap; RectTransform rt=t.rectTransform; rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f); rt.anchoredPosition=pos; rt.sizeDelta=size; return t; }
}
