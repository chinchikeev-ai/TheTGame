using UnityEngine;
using UnityEngine.UI;

public sealed class ModernCombatHud : MonoBehaviour
{
    Canvas canvas;
    CanvasGroup group;
    TowerPlacement placement;
    EnemySpawner spawner;
    Canvas legacyCanvas;
    Canvas menuCanvas;

    Text goldText, gateText, aliveText, waveText, threatText, selectedTitle, selectedStats, selectedPriority;
    Button upgradeButton, sellButton, priorityButton;
    Text buildSelectionText;

    readonly TowerType[] buildTypes =
    {
        TowerType.SpearThrower,
        TowerType.MachineGun,
        TowerType.Cannon,
        TowerType.Slow,
        TowerType.FireTower,
        TowerType.TrojanGuard
    };

    readonly string[] buildHotkeys = { "1", "2", "3", "4", "5", "6" };
    readonly Button[] buildButtons = new Button[6];

    string L(string en, string ru) => GameLanguage.T(en, ru);

    void Start()
    {
        placement = FindFirstObjectByType<TowerPlacement>();
        spawner = FindFirstObjectByType<EnemySpawner>();
        FindCanvases();
        Build();
    }

    void FindCanvases()
    {
        GameObject legacyObject = GameObject.Find("GameCanvas");
        legacyCanvas = legacyObject != null ? legacyObject.GetComponent<Canvas>() : null;
        GameObject menuObject = GameObject.Find("MenuCanvas");
        menuCanvas = menuObject != null ? menuObject.GetComponent<Canvas>() : null;
        if (legacyCanvas != null) legacyCanvas.enabled = false;
    }

    void Build()
    {
        GameObject root = new GameObject("ModernCombatHUD");
        canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 80;
        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920,1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;
        root.AddComponent<GraphicRaycaster>();
        group = root.AddComponent<CanvasGroup>();

        BuildTopBar(root.transform);
        BuildWaveBar(root.transform);
        BuildDock(root.transform);
        BuildSelectedCard(root.transform);
        BuildPcHints(root.transform);
    }

    void BuildTopBar(Transform parent)
    {
        GameObject bar = Panel(parent,"TopResources",new Vector2(24,-24),new Vector2(660,74),new Color(.035f,.022f,.016f,.92f),new Vector2(0,1),new Vector2(0,1));
        goldText = Text(bar.transform,"GOLD",new Vector2(22,0),new Vector2(190,60),22,new Color(1f,.73f,.24f,1f),TextAnchor.MiddleLeft,FontStyle.Bold);
        gateText = Text(bar.transform,"GATE",new Vector2(220,0),new Vector2(190,60),20,new Color(.94f,.84f,.67f,1f),TextAnchor.MiddleLeft,FontStyle.Bold);
        aliveText = Text(bar.transform,"ALIVE",new Vector2(425,0),new Vector2(200,60),20,new Color(.94f,.84f,.67f,1f),TextAnchor.MiddleLeft,FontStyle.Bold);
    }

    void BuildWaveBar(Transform parent)
    {
        GameObject bar = Panel(parent,"WaveStatus",new Vector2(0,-24),new Vector2(760,92),new Color(.035f,.022f,.016f,.94f),new Vector2(.5f,1),new Vector2(.5f,1));
        waveText = Text(bar.transform,"WAVE",new Vector2(0,18),new Vector2(720,34),22,new Color(1f,.75f,.32f,1f),TextAnchor.MiddleCenter,FontStyle.Bold);
        threatText = Text(bar.transform,"THREAT",new Vector2(0,-20),new Vector2(720,32),16,new Color(.86f,.76f,.64f,1f),TextAnchor.MiddleCenter,FontStyle.Normal);
    }

    void BuildDock(Transform parent)
    {
        GameObject dock = Panel(parent,"BuildDock",new Vector2(0,24),new Vector2(1120,154),new Color(.035f,.022f,.016f,.95f),new Vector2(.5f,0),new Vector2(.5f,0));
        Text(dock.transform,L("TROJAN DEFENSES","ОБОРОНА ТРОИ"),new Vector2(-480,54),new Vector2(260,28),14,new Color(.74f,.65f,.55f,1f),TextAnchor.MiddleLeft,FontStyle.Bold);
        buildSelectionText = Text(dock.transform,"",new Vector2(390,54),new Vector2(620,28),14,new Color(1f,.72f,.28f,1f),TextAnchor.MiddleRight,FontStyle.Bold);

        for (int i=0;i<buildTypes.Length;i++)
        {
            TowerType type = buildTypes[i];
            float x = -430 + i*174;
            buildButtons[i] = Button(dock.transform,BuildLabel(type,buildHotkeys[i]),new Vector2(x,-18),new Vector2(158,82),()=>SelectBuild(type),false);
        }
    }

    void BuildSelectedCard(Transform parent)
    {
        GameObject card = Panel(parent,"SelectedTowerCard",new Vector2(-24,-132),new Vector2(430,350),new Color(.045f,.027f,.018f,.96f),new Vector2(1,1),new Vector2(1,1));
        selectedTitle = Text(card.transform,L("SELECT A DEFENSE","ВЫБЕРИТЕ ОБОРОНУ"),new Vector2(24,-22),new Vector2(380,46),24,new Color(1f,.70f,.28f,1f),TextAnchor.UpperLeft,FontStyle.Bold);
        selectedStats = Text(card.transform,"",new Vector2(24,-80),new Vector2(380,145),17,new Color(.94f,.87f,.77f,1f),TextAnchor.UpperLeft,FontStyle.Normal);
        selectedPriority = Text(card.transform,"",new Vector2(24,-205),new Vector2(380,34),15,new Color(.82f,.72f,.62f,1f),TextAnchor.MiddleLeft,FontStyle.Bold);
        upgradeButton = Button(card.transform,L("UPGRADE","УЛУЧШИТЬ"),new Vector2(-105,-272),new Vector2(180,58),()=>placement?.UpgradeSelected(),true);
        sellButton = Button(card.transform,L("SELL","ПРОДАТЬ"),new Vector2(105,-272),new Vector2(180,58),()=>placement?.SellSelected(),false);
        priorityButton = Button(card.transform,L("TARGET PRIORITY","ПРИОРИТЕТ ЦЕЛИ"),new Vector2(0,-326),new Vector2(390,44),()=>placement?.CycleSelectedPriority(),false);
    }

    void BuildPcHints(Transform parent)
    {
        GameObject hint = Panel(parent,"PcHints",new Vector2(24,24),new Vector2(650,48),new Color(.03f,.02f,.015f,.82f),new Vector2(0,0),new Vector2(0,0));
        Text(hint.transform,L("LMB SELECT/BUILD   •   RMB HECTOR MOVE   •   Q E R F ABILITIES   •   ESC PAUSE","ЛКМ ВЫБОР/СТРОЙКА   •   ПКМ ДВИЖЕНИЕ ГЕКТОРА   •   Q E R F СПОСОБНОСТИ   •   ESC ПАУЗА"),new Vector2(14,0),new Vector2(620,42),13,new Color(.77f,.69f,.60f,1f),TextAnchor.MiddleLeft,FontStyle.Normal);
    }

    void Update()
    {
        if (placement == null) placement = FindFirstObjectByType<TowerPlacement>();
        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();
        if (menuCanvas == null) FindCanvases();

        bool blocked = IsMenuBlockingCombat();
        if (group != null)
        {
            group.alpha = blocked ? 0f : 1f;
            group.interactable = !blocked;
            group.blocksRaycasts = !blocked;
        }
        if (blocked || GameManager.Instance == null) return;

        UpdateResources();
        UpdateWave();
        UpdateBuildDock();
        UpdateSelected();
        HandlePcHotkeys();
    }

    bool IsMenuBlockingCombat()
    {
        if (menuCanvas == null) return false;
        string[] names={"MainMenu","LevelSelect","Settings","PauseMenu","EndMenu","ConfirmationModal"};
        foreach(string n in names){ Transform t=menuCanvas.transform.Find(n); if(t!=null&&t.gameObject.activeInHierarchy) return true; }
        return false;
    }

    void UpdateResources()
    {
        GameManager gm=GameManager.Instance;
        goldText.text=$"{L("GOLD","ЗОЛОТО")}   {gm.Money}";
        gateText.text=$"{L("GATE","ВОРОТА")}   {gm.BaseHealth}/{gm.MaxBaseHealth}";
        aliveText.text=$"{L("ENEMIES","ВРАГИ")}   {EnemyRegistry.AliveCount}";
    }

    void UpdateWave()
    {
        GameManager gm=GameManager.Instance;
        int sec=spawner!=null?Mathf.RoundToInt(spawner.CurrentWaveElapsed):0;
        waveText.text=$"{L("CHAPTER I","ГЛАВА I")}   •   {L("WAVE","ВОЛНА")} {gm.CurrentWave}/{gm.MaxWaves}   •   {sec/60:00}:{sec%60:00}";
        if(spawner==null){threatText.text="";return;}
        string threat=spawner.NextWaveHasBoss?L("BOSS APPROACHING: MENELAUS","ПРИБЛИЖАЕТСЯ БОСС: МЕНЕЛАЙ"):spawner.NextWaveHasHeavy?L("HEAVY HOPLITES EXPECTED","ОЖИДАЮТСЯ ТЯЖЁЛЫЕ ГОПЛИТЫ"):L("STANDARD ENEMY FORMATION","ОБЫЧНАЯ ВРАЖЕСКАЯ ФОРМАЦИЯ");
        if(spawner.WaveActive) threatText.text=$"{L("WAVE ACTIVE","ВОЛНА ИДЁТ")}   •   {EnemyRegistry.AliveCount} {L("alive","в строю")}   •   {threat}";
        else if(spawner.InterWaveCountdown>0) threatText.text=$"{L("NEXT WAVE IN","СЛЕДУЮЩАЯ ВОЛНА ЧЕРЕЗ")} {Mathf.CeilToInt(spawner.InterWaveCountdown)}{L("s","с")}   •   {spawner.NextWaveEnemyCount} {L("enemies","врагов")}   •   {threat}";
        else threatText.text=$"{L("READY","ГОТОВО")}   •   {spawner.NextWaveEnemyCount} {L("enemies","врагов")}   •   {threat}";
    }

    void UpdateBuildDock()
    {
        if(placement==null)return;
        int cost=TowerFactory.GetCost(placement.SelectedBuildType);
        buildSelectionText.text=$"{L("SELECTED","ВЫБРАНО")}: {TowerName(placement.SelectedBuildType)}   •   {cost} {L("GOLD","ЗОЛОТА")}";
        for(int i=0;i<buildButtons.Length;i++)
        {
            if(buildButtons[i]==null)continue;
            Image image=buildButtons[i].GetComponent<Image>();
            bool selected=buildTypes[i]==placement.SelectedBuildType;
            bool affordable=GameManager.Instance!=null&&GameManager.Instance.Money>=TowerFactory.GetCost(buildTypes[i]);
            image.color=selected?new Color(.58f,.11f,.045f,.98f):affordable?new Color(.24f,.14f,.08f,.96f):new Color(.11f,.085f,.07f,.88f);
        }
    }

    void UpdateSelected()
    {
        Tower tower=placement!=null?placement.SelectedTower:null;
        bool has=tower!=null;
        upgradeButton.gameObject.SetActive(has);
        sellButton.gameObject.SetActive(has);
        priorityButton.gameObject.SetActive(has&&tower.Type!=TowerType.TrojanGuard);
        if(!has)
        {
            selectedTitle.text=L("SELECT A TOWER-UNIT","ВЫБЕРИТЕ ОБОРОНУ");
            selectedStats.text=L("Click a deployed Trojan defense to inspect stats, upgrade it, sell it, or change targeting priority.","Нажмите на установленную оборону Трои, чтобы увидеть характеристики, улучшить, продать или изменить приоритет цели.");
            selectedPriority.text="";
            return;
        }
        selectedTitle.text=$"{tower.DisplayName}   LV {tower.Level}/3";
        selectedStats.text=$"{L("DAMAGE","УРОН")}     {tower.damage:0}\n{L("RANGE","ДАЛЬНОСТЬ")}       {tower.range:0.0}\n{L("ATTACK RATE","СКОРОСТЬ АТАКИ")}   {tower.fireRate:0.0}/s\n\n{(tower.Level>=3?L("MAXIMUM LEVEL","МАКСИМАЛЬНЫЙ УРОВЕНЬ"):L("UPGRADE COST","ЦЕНА УЛУЧШЕНИЯ")+"   "+tower.UpgradeCost)}\n{L("SELL VALUE","ЦЕНА ПРОДАЖИ")}      {tower.SellValue}";
        selectedPriority.text=tower.Type==TowerType.TrojanGuard?L("ROLE: BLOCKING SQUAD","РОЛЬ: БЛОКИРУЮЩИЙ ОТРЯД"):$"{L("TARGETING","ПРИОРИТЕТ")}: {tower.Priority}";
        upgradeButton.interactable=tower.Level<3&&GameManager.Instance.Money>=tower.UpgradeCost;
    }

    void HandlePcHotkeys()
    {
        if (placement == null) return;
        for (int i = 0; i < buildTypes.Length; i++)
        {
            if (!GameInput.BuildSlotPressed(i + 1)) continue;
            SelectBuild(buildTypes[i]);
            break;
        }
    }

    void SelectBuild(TowerType type){ placement?.SelectBuildType(type); }

    string BuildLabel(TowerType type,string key)=>$"[{key}]  {TowerName(type)}\n{TowerFactory.GetCost(type)} {L("GOLD","ЗОЛОТА")}";
    string TowerName(TowerType type){ switch(type){case TowerType.SpearThrower:return L("SPEAR","КОПЬЯ");case TowerType.MachineGun:return L("ARCHERS","ЛУЧНИКИ");case TowerType.Cannon:return L("BALLISTA","БАЛЛИСТА");case TowerType.Slow:return L("APOLLO","АПОЛЛОН");case TowerType.FireTower:return L("FIRE","ОГОНЬ");case TowerType.TrojanGuard:return L("GUARD","СТРАЖА");default:return type.ToString();} }

    GameObject Panel(Transform parent,string name,Vector2 pos,Vector2 size,Color color,Vector2 anchor,Vector2 pivot){GameObject go=new GameObject(name);go.transform.SetParent(parent,false);Image image=go.AddComponent<Image>();image.color=color;RectTransform rt=image.rectTransform;rt.anchorMin=rt.anchorMax=anchor;rt.pivot=pivot;rt.anchoredPosition=pos;rt.sizeDelta=size;Outline outline=go.AddComponent<Outline>();outline.effectColor=new Color(.67f,.36f,.13f,.42f);outline.effectDistance=new Vector2(1.5f,-1.5f);return go;}
    Button Button(Transform parent,string label,Vector2 pos,Vector2 size,UnityEngine.Events.UnityAction action,bool primary){GameObject go=new GameObject(label);go.transform.SetParent(parent,false);Image image=go.AddComponent<Image>();image.color=primary?new Color(.55f,.11f,.045f,.98f):new Color(.24f,.14f,.08f,.96f);Button button=go.AddComponent<Button>();button.targetGraphic=image;button.onClick.AddListener(action);RectTransform rt=image.rectTransform;rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f);rt.anchoredPosition=pos;rt.sizeDelta=size;go.AddComponent<MenuButtonFeedback>();go.AddComponent<MenuUiAudioFeedback>();Text(go.transform,label,Vector2.zero,size,15,primary?new Color(1f,.88f,.50f,1f):new Color(.94f,.84f,.70f,1f),TextAnchor.MiddleCenter,FontStyle.Bold);return button;}
    Text Text(Transform parent,string value,Vector2 pos,Vector2 size,int fontSize,Color color,TextAnchor alignment,FontStyle style){GameObject go=new GameObject("Text");go.transform.SetParent(parent,false);Text t=go.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=value;t.fontSize=fontSize;t.fontStyle=style;t.color=color;t.alignment=alignment;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;RectTransform rt=t.rectTransform;rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f);rt.anchoredPosition=pos;rt.sizeDelta=size;return t;}
}
