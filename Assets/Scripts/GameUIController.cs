using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

public class GameUIController : MonoBehaviour
{
    Text statsText, waveText, nextWaveText, selectedText, endText;
    EnemySpawner spawner;
    TowerPlacement placement;
    RectTransform selectedPanel;
    string L(string en, string ru) => GameLanguage.T(en, ru);

    void Start(){ spawner=FindFirstObjectByType<EnemySpawner>(); placement=FindFirstObjectByType<TowerPlacement>(); EnsureEventSystem(); BuildUI(); }

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

    void EnsureEventSystem(){ if(EventSystem.current!=null)return; GameObject es=new GameObject("EventSystem"); es.AddComponent<EventSystem>(); #if ENABLE_INPUT_SYSTEM
        es.AddComponent<InputSystemUIInputModule>();
#else
        es.AddComponent<StandaloneInputModule>();
#endif
    }

    void BuildUI()
    {
        GameObject canvasObj=new GameObject("GameCanvas"); Canvas canvas=canvasObj.AddComponent<Canvas>(); canvas.renderMode=RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler=canvasObj.AddComponent<CanvasScaler>(); scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution=new Vector2(1920f,1080f); scaler.matchWidthOrHeight=.5f; canvasObj.AddComponent<GraphicRaycaster>();
        statsText=CreateText(canvas.transform,"Stats",new Vector2(24,-22),new Vector2(760,48),28,TextAnchor.UpperLeft);
        waveText=CreateText(canvas.transform,"Wave",new Vector2(24,-68),new Vector2(850,42),22,TextAnchor.UpperLeft);
        nextWaveText=CreateText(canvas.transform,"NextWave",new Vector2(0,-22),new Vector2(1100,48),20,TextAnchor.UpperCenter); Anchor(nextWaveText.rectTransform,new Vector2(.5f,1f),new Vector2(.5f,1f),new Vector2(.5f,1f));
        CreateBuildBar(canvas.transform); CreateSelectedPanel(canvas.transform);
        Text help=CreateText(canvas.transform,"Help",new Vector2(-24,24),new Vector2(1000,58),18,TextAnchor.LowerRight); Anchor(help.rectTransform,new Vector2(1f,0f),new Vector2(1f,0f),new Vector2(1f,0f));
        help.text=L("2D GRID • FREE CELLS BUILDABLE • HECTOR: LMB/RMB, Q WAR CRY, E SHIELD WALL, R SPEAR THROW","2D СЕТКА • СТРОЙТЕ НА СВОБОДНЫХ КЛЕТКАХ • ГЕКТОР: ЛКМ/ПКМ, Q КЛИЧ, E ЩИТОВАЯ СТЕНА, R КОПЬЁ");
        endText=CreateText(canvas.transform,"End",Vector2.zero,new Vector2(900,180),64,TextAnchor.MiddleCenter); Anchor(endText.rectTransform,new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(.5f,.5f)); endText.gameObject.SetActive(false);
    }

    void CreateBuildBar(Transform parent)
    {
        GameObject panel=CreatePanel(parent,"BuildBar",new Vector2(0,20),new Vector2(760,105)); Anchor(panel.GetComponent<RectTransform>(),new Vector2(.5f,0f),new Vector2(.5f,0f),new Vector2(.5f,0f));
        CreateButton(panel.transform,L("ARCHER TOWER\n100 GOLD","БАШНЯ ЛУЧНИКОВ\n100 ЗОЛОТА"),new Vector2(-245,0),new Vector2(220,72),()=>placement?.SelectBuildType(TowerType.MachineGun));
        CreateButton(panel.transform,L("BALLISTA\n220 GOLD","БАЛЛИСТА\n220 ЗОЛОТА"),Vector2.zero,new Vector2(220,72),()=>placement?.SelectBuildType(TowerType.Cannon));
        CreateButton(panel.transform,L("PRIESTS OF APOLLO\n160 GOLD","ЖРЕЦЫ АПОЛЛОНА\n160 ЗОЛОТА"),new Vector2(245,0),new Vector2(220,72),()=>placement?.SelectBuildType(TowerType.Slow));
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

    GameObject CreatePanel(Transform parent,string name,Vector2 pos,Vector2 size){GameObject go=new GameObject(name);go.transform.SetParent(parent,false);Image image=go.AddComponent<Image>();image.color=new Color(.06f,.08f,.11f,.88f);RectTransform rt=image.rectTransform;rt.anchoredPosition=pos;rt.sizeDelta=size;return go;}
    void CreateButton(Transform parent,string label,Vector2 pos,Vector2 size,UnityEngine.Events.UnityAction action){GameObject go=new GameObject(label.Replace("\n","_"));go.transform.SetParent(parent,false);Image image=go.AddComponent<Image>();image.color=new Color(.18f,.25f,.34f,.96f);Button button=go.AddComponent<Button>();button.targetGraphic=image;button.onClick.AddListener(action);RectTransform rt=image.rectTransform;rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f);rt.anchoredPosition=pos;rt.sizeDelta=size;Text text=CreateText(go.transform,"Label",Vector2.zero,size,17,TextAnchor.MiddleCenter);Anchor(text.rectTransform,Vector2.zero,Vector2.one,new Vector2(.5f,.5f));text.rectTransform.offsetMin=Vector2.zero;text.rectTransform.offsetMax=Vector2.zero;}
    Text CreateText(Transform parent,string name,Vector2 pos,Vector2 size,int fontSize,TextAnchor alignment){GameObject go=new GameObject(name);go.transform.SetParent(parent,false);Text t=go.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontSize=fontSize;t.fontStyle=FontStyle.Bold;t.color=Color.white;t.alignment=alignment;t.text=name=="Label"?parent.name.Replace("_","\n"):"";t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Overflow;RectTransform rt=t.rectTransform;rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(0,1);rt.anchoredPosition=pos;rt.sizeDelta=size;return t;}
    void Anchor(RectTransform rt,Vector2 min,Vector2 max,Vector2 pivot){rt.anchorMin=min;rt.anchorMax=max;rt.pivot=pivot;}
}
