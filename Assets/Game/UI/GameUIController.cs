using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

public class GameUIController : MonoBehaviour
{
    Text statsText, waveText, nextWaveText, selectedText, selectedBuildText, endText;
    EnemySpawner spawner;
    TowerPlacement placement;
    RectTransform selectedPanel, towerListPanel;
    bool towerListOpen;
    string L(string en, string ru) => GameLanguage.T(en, ru);

    void Start()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();
        placement = FindFirstObjectByType<TowerPlacement>();
        EnsureEventSystem();
        BuildUI();
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();
        if (placement == null) placement = FindFirstObjectByType<TowerPlacement>();
        statsText.text = $"{L("GOLD","ЗОЛОТО")}  {GameManager.Instance.Money}    {L("GATE","ВОРОТА")}  {GameManager.Instance.BaseHealth}    {L("ALIVE","ВРАГОВ")}  {EnemyRegistry.AliveCount}";
        int waveSeconds = spawner != null ? Mathf.RoundToInt(spawner.CurrentWaveElapsed) : 0;
        waveText.text = $"{L("MAP","КАРТА")} {GameManager.Instance.MapNumber}   •   {L("WAVE","ВОЛНА")} {GameManager.Instance.CurrentWave}/{GameManager.Instance.MaxWaves}   •   {L("TIME","ВРЕМЯ")} {waveSeconds/60:00}:{waveSeconds%60:00}";

        if (spawner != null)
        {
            string threat = spawner.NextWaveHasBoss ? "  •  MENELAUS" : spawner.NextWaveHasHeavy ? L("  •  HEAVY HOPLITES","  •  ТЯЖЁЛЫЕ ГОПЛИТЫ") : "";
            string target = $"  •  {L("TARGET","ЦЕЛЬ")} {Mathf.RoundToInt(spawner.TargetWaveDuration)}{L("s","с")}";
            if (spawner.WaveActive) nextWaveText.text = $"{L("WAVE ACTIVE","ВОЛНА ИДЁТ")}  •  {L("ALIVE","ВРАГОВ")} {EnemyRegistry.AliveCount}{target}{threat}";
            else if (spawner.InterWaveCountdown > 0f) nextWaveText.text = $"{L("START IN","СТАРТ ЧЕРЕЗ")} {Mathf.CeilToInt(spawner.InterWaveCountdown)}{L("s","с")}  •  {spawner.NextWaveEnemyCount} {L("enemies","врагов")}{target}{threat}";
            else nextWaveText.text = $"{L("READY","ГОТОВО")}  •  {spawner.NextWaveEnemyCount} {L("enemies","врагов")}{target}{threat}";
        }

        if (towerListPanel != null) towerListPanel.gameObject.SetActive(towerListOpen);
        if (selectedBuildText != null && placement != null)
        {
            int cost = TowerFactory.GetCost(placement.SelectedBuildType);
            selectedBuildText.text = $"{L("BUILD","СТРОЙКА")}: {TowerLabel(placement.SelectedBuildType)}\n{cost} {L("GOLD","ЗОЛОТА")}";
        }

        Tower selected = placement != null ? placement.SelectedTower : null;
        selectedPanel.gameObject.SetActive(selected != null);
        if (selected != null)
        {
            string upgrade = selected.Level >= 3 ? L("MAX LEVEL","МАКС. УРОВЕНЬ") : $"{L("UPGRADE","УЛУЧШИТЬ")} {selected.UpgradeCost} {L("GOLD","ЗОЛОТА")}";
            string priority = selected.Type == TowerType.TrojanGuard ? L("BLOCKING SQUAD","БЛОКИРУЮЩИЙ ОТРЯД") : $"{L("PRIORITY","ПРИОРИТЕТ")}: {selected.Priority}";
            selectedText.text = $"{selected.DisplayName}  •  {L("LVL","УР.")} {selected.Level}/3\n{L("DMG","УРОН")} {selected.damage:0}   {L("RANGE","ДАЛЬНОСТЬ")} {selected.range:0.0}   {L("RATE","СКОРОСТЬ")} {selected.fireRate:0.0}/s\n{priority}\n{upgrade}   •   {L("SELL","ПРОДАТЬ")} {selected.SellValue} {L("GOLD","ЗОЛОТА")}";
        }
        endText.gameObject.SetActive(GameManager.Instance.GameEnded);
        if (GameManager.Instance.GameEnded) endText.text = GameManager.Instance.EndMessage == "VICTORY" ? L("VICTORY","ПОБЕДА") : L("GAME OVER","ПОРАЖЕНИЕ");
    }

    void EnsureEventSystem()
    {
        if (EventSystem.current != null) return;
        GameObject es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
        es.AddComponent<InputSystemUIInputModule>();
#else
        es.AddComponent<StandaloneInputModule>();
#endif
    }

    void BuildUI()
    {
        GameObject canvasObj = new GameObject("GameCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f,1080f);
        scaler.matchWidthOrHeight = .5f;
        canvasObj.AddComponent<GraphicRaycaster>();

        statsText = CreateText(canvas.transform,"Stats",new Vector2(24,-22),new Vector2(760,48),28,TextAnchor.UpperLeft);
        waveText = CreateText(canvas.transform,"Wave",new Vector2(24,-68),new Vector2(850,42),22,TextAnchor.UpperLeft);
        nextWaveText = CreateText(canvas.transform,"NextWave",new Vector2(0,-22),new Vector2(1100,48),20,TextAnchor.UpperCenter);
        Anchor(nextWaveText.rectTransform,new Vector2(.5f,1f),new Vector2(.5f,1f),new Vector2(.5f,1f));

        CreateBuildPicker(canvas.transform);
        CreateSelectedPanel(canvas.transform);

        Text help = CreateText(canvas.transform,"Help",new Vector2(-24,24),new Vector2(1200,58),18,TextAnchor.LowerRight);
        Anchor(help.rectTransform,new Vector2(1f,0f),new Vector2(1f,0f),new Vector2(1f,0f));
        help.text = L("BUILD ON MARKED GROUND • HECTOR: LMB SELECT, RMB MOVE, Q WAR CRY, E SHIELD WALL, R SPEAR THROW, F FOR TROY!","СТРОЙТЕ НА ОТМЕЧЕННЫХ ПОЗИЦИЯХ • ГЕКТОР: ЛКМ ВЫБОР, ПКМ ДВИЖЕНИЕ, Q КЛИЧ, E СТЕНА ЩИТОВ, R КОПЬЁ, F ЗА ТРОЮ!");

        endText = CreateText(canvas.transform,"End",Vector2.zero,new Vector2(900,180),64,TextAnchor.MiddleCenter);
        Anchor(endText.rectTransform,new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(.5f,.5f));
        endText.gameObject.SetActive(false);
    }

    void CreateBuildPicker(Transform parent)
    {
        Button plusButton = CreateButton(parent,"+",new Vector2(-34,-250),new Vector2(64,64),ToggleTowerList,26);
        Anchor(plusButton.GetComponent<RectTransform>(),new Vector2(1f,1f),new Vector2(1f,1f),new Vector2(1f,1f));

        selectedBuildText = CreateText(parent,"SelectedBuild",new Vector2(-112,-326),new Vector2(220,56),16,TextAnchor.UpperRight);
        Anchor(selectedBuildText.rectTransform,new Vector2(1f,1f),new Vector2(1f,1f),new Vector2(1f,1f));
        selectedBuildText.color = new Color(1f,.84f,.48f,1f);

        GameObject panel=CreatePanel(parent,"TowerList",new Vector2(-34,-455),new Vector2(285,390));
        towerListPanel=panel.GetComponent<RectTransform>();
        Anchor(towerListPanel,new Vector2(1f,1f),new Vector2(1f,1f),new Vector2(1f,1f));
        CreateTowerButton(panel.transform,L("ARCHER TOWER","БАШНЯ ЛУЧНИКОВ"),TowerType.MachineGun,new Vector2(0,145));
        CreateTowerButton(panel.transform,L("BALLISTA","БАЛЛИСТА"),TowerType.Cannon,new Vector2(0,85));
        CreateTowerButton(panel.transform,L("PRIESTS OF APOLLO","ЖРЕЦЫ АПОЛЛОНА"),TowerType.Slow,new Vector2(0,25));
        CreateTowerButton(panel.transform,L("SPEAR THROWERS","МЕТАТЕЛИ КОПИЙ"),TowerType.SpearThrower,new Vector2(0,-35));
        CreateTowerButton(panel.transform,L("FIRE TOWER","ОГНЕННАЯ БАШНЯ"),TowerType.FireTower,new Vector2(0,-95));
        CreateTowerButton(panel.transform,L("TROJAN GUARD","ТРОЯНСКАЯ СТРАЖА"),TowerType.TrojanGuard,new Vector2(0,-155));
        panel.SetActive(false);
    }

    void ToggleTowerList()
    {
        towerListOpen = !towerListOpen;
    }

    void CreateTowerButton(Transform parent, string label, TowerType type, Vector2 pos)
    {
        int cost = TowerFactory.GetCost(type);
        CreateButton(parent,$"{label}\n{cost} {L("GOLD","ЗОЛОТА")}",pos,new Vector2(245,52),() =>
        {
            placement?.SelectBuildType(type);
            towerListOpen = false;
        },15);
    }

    string TowerLabel(TowerType type)
    {
        switch (type)
        {
            case TowerType.Cannon: return L("Ballista","Баллиста");
            case TowerType.Slow: return L("Apollo Priests","Жрецы Аполлона");
            case TowerType.SpearThrower: return L("Spear Throwers","Метатели копий");
            case TowerType.FireTower: return L("Fire Tower","Огненная башня");
            case TowerType.TrojanGuard: return L("Trojan Guard","Троянская стража");
            default: return L("Archer Tower","Башня лучников");
        }
    }

    void CreateSelectedPanel(Transform parent)
    {
        GameObject panel=CreatePanel(parent,"SelectedTowerPanel",new Vector2(-24,-120),new Vector2(460,285)); RectTransform rt=panel.GetComponent<RectTransform>(); Anchor(rt,new Vector2(1,1),new Vector2(1,1),new Vector2(1,1)); selectedPanel=rt;
        selectedText=CreateText(panel.transform,"SelectedInfo",new Vector2(20,-18),new Vector2(420,145),18,TextAnchor.UpperLeft);
        CreateButton(panel.transform,L("UPGRADE","УЛУЧШИТЬ"),new Vector2(-110,-90),new Vector2(185,50),()=>placement?.UpgradeSelected());
        CreateButton(panel.transform,L("SELL","ПРОДАТЬ"),new Vector2(110,-90),new Vector2(185,50),()=>placement?.SellSelected());
        CreateButton(panel.transform,L("TARGET PRIORITY","ПРИОРИТЕТ ЦЕЛИ"),new Vector2(0,-150),new Vector2(390,48),()=>placement?.CycleSelectedPriority());
        selectedPanel.gameObject.SetActive(false);
    }

    GameObject CreatePanel(Transform parent,string name,Vector2 pos,Vector2 size)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent,false);
        Image image = go.AddComponent<Image>();
        image.color = new Color(.06f,.08f,.11f,.88f);
        RectTransform rt = image.rectTransform;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return go;
    }

    Button CreateButton(Transform parent,string label,Vector2 pos,Vector2 size,UnityEngine.Events.UnityAction action,int fontSize=17)
    {
        GameObject go = new GameObject(label.Replace("\n","_"));
        go.transform.SetParent(parent,false);
        Image image = go.AddComponent<Image>();
        image.color = new Color(.18f,.25f,.34f,.96f);
        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f,.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        Text text = CreateText(go.transform,"Label",Vector2.zero,size,fontSize,TextAnchor.MiddleCenter);
        Anchor(text.rectTransform,Vector2.zero,Vector2.one,new Vector2(.5f,.5f));
        text.rectTransform.offsetMin = Vector2.zero;
        text.rectTransform.offsetMax = Vector2.zero;
        return button;
    }

    Text CreateText(Transform parent,string name,Vector2 pos,Vector2 size,int fontSize,TextAnchor alignment)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent,false);
        Text t = go.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = fontSize;
        t.fontStyle = FontStyle.Bold;
        t.color = Color.white;
        t.alignment = alignment;
        t.text = name == "Label" ? parent.name.Replace("_","\n") : "";
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform rt = t.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0,1);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return t;
    }

    void Anchor(RectTransform rt,Vector2 min,Vector2 max,Vector2 pivot)
    {
        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.pivot = pivot;
    }
}
