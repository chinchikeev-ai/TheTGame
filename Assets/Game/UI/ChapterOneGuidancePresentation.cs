using UnityEngine;
using UnityEngine.UI;

public sealed class ChapterOneGuidancePresentation : MonoBehaviour
{
    public Transform UiRoot => canvas != null ? canvas.transform : null;

    Canvas canvas;
    CanvasGroup group;
    EnemySpawner spawner;
    Text chapterText;
    Text objectiveText;
    Text progressText;
    Text tutorialTitle;
    Text tutorialText;
    Image objectiveIcon;
    Image tutorialIcon;
    GameObject objectiveCard;
    GameObject tutorialCard;
    Canvas menuCanvas;
    int lastTutorialStage = -1;
    float tutorialShownAt;

    string L(string en, string ru) => GameLanguage.T(en, ru);

    void Start()
    {
        spawner = EnemySpawner.Instance;
        menuCanvas = GameMenuController.Instance != null ? GameMenuController.Instance.MenuCanvas : null;
        Build();
    }

    void Build()
    {
        GameObject root = new GameObject("ChapterOneGuidanceUI");
        canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 82;
        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;
        group = root.AddComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;

        objectiveCard = Panel(root.transform, "ChapterObjective", new Vector2(24f, -176f), new Vector2(450f, 104f), new Vector2(0f, 1f), new Vector2(0f, 1f));
        objectiveIcon = AddIcon(objectiveCard.transform, "ObjectiveIcon", new Vector2(-194f, 0f), 44, TroyHudArt.Icon("gate"));
        chapterText = AddText(objectiveCard.transform, "", new Vector2(74f, -14f), new Vector2(352f, 22f), 11, new Color(1f, .69f, .23f, 1f), TextAnchor.UpperLeft, FontStyle.Bold, new Vector2(0f, 1f));
        objectiveText = AddText(objectiveCard.transform, "", new Vector2(74f, -38f), new Vector2(352f, 28f), 16, new Color(.96f, .88f, .75f, 1f), TextAnchor.UpperLeft, FontStyle.Bold, new Vector2(0f, 1f));
        progressText = AddText(objectiveCard.transform, "", new Vector2(74f, -72f), new Vector2(352f, 22f), 11, new Color(.77f, .69f, .59f, 1f), TextAnchor.UpperLeft, FontStyle.Normal, new Vector2(0f, 1f));

        tutorialCard = Panel(root.transform, "ContextTutorial", new Vector2(24f, -298f), new Vector2(450f, 122f), new Vector2(0f, 1f), new Vector2(0f, 1f));
        Image tutorialPanel = tutorialCard.GetComponent<Image>();
        Sprite parchment = Resources.Load<Sprite>("HectorHud/Parchment");
        if (parchment != null)
        {
            tutorialPanel.sprite = parchment;
            tutorialPanel.type = Image.Type.Simple;
            tutorialPanel.color = Color.white;
        }
        tutorialIcon = AddIcon(tutorialCard.transform, "TutorialIcon", new Vector2(-190f, 0f), 52, TroyHudArt.Tower(TowerType.MachineGun));
        tutorialTitle = AddText(tutorialCard.transform, "", new Vector2(72f, -16f), new Vector2(354f, 24f), 13, new Color(.42f, .16f, .06f, 1f), TextAnchor.UpperLeft, FontStyle.Bold, new Vector2(0f, 1f));
        tutorialText = AddText(tutorialCard.transform, "", new Vector2(72f, -45f), new Vector2(354f, 62f), 13, new Color(.22f, .11f, .055f, 1f), TextAnchor.UpperLeft, FontStyle.Bold, new Vector2(0f, 1f));
        tutorialCard.SetActive(false);
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || gm.MapNumber != 1)
        {
            if (group != null) group.alpha = 0f;
            return;
        }
        if (spawner == null) spawner = EnemySpawner.Instance;
        if (menuCanvas == null && GameMenuController.Instance != null)
            menuCanvas = GameMenuController.Instance.MenuCanvas;
        bool hidden = gm.GameEnded || IsMenuBlockingCombat();
        group.alpha = hidden ? 0f : 1f;
        if (hidden) return;
        bool bossActive = HasActiveBoss();
        objectiveCard.SetActive(!bossActive);
        UpdateObjective(gm);
        UpdateTutorial(gm);
    }

    void UpdateObjective(GameManager gm)
    {
        chapterText.text = L("CHAPTER I • THE LANDING", "ГЛАВА I • ВЫСАДКА");
        int currentEncounter = EncounterRuntime.CurrentEncounter(spawner);
        int maxEncounters = Mathf.Max(1, EncounterRuntime.MaxEncounters);
        bool active = EncounterRuntime.EncounterActive(spawner);
        bool between = EncounterRuntime.BetweenEncounters(spawner);
        bool finalEncounter = currentEncounter >= maxEncounters || EncounterRuntime.NextEncounterHasBoss(spawner);

        if (gm.BossDefeated)
        {
            objectiveIcon.sprite = TroyHudArt.Icon("gate");
            objectiveText.text = L("OBJECTIVE COMPLETE", "ЦЕЛЬ ВЫПОЛНЕНА");
            progressText.text = L("MENELAUS DOWN • CLEAR THE FIELD", "МЕНЕЛАЙ ПОВЕРЖЕН • ЗАЧИСТИТЕ ПОЛЕ");
            return;
        }
        if (finalEncounter)
        {
            objectiveIcon.sprite = TroyHudArt.Portrait("menelaus");
            objectiveText.text = L("FINAL OBJECTIVE • DEFEAT MENELAUS", "ФИНАЛЬНАЯ ЦЕЛЬ • ПОБЕДИТЕ МЕНЕЛАЯ");
            progressText.text = L("AURA + REINFORCEMENTS • PROTECT THE GATE", "АУРА + ПОДКРЕПЛЕНИЯ • ЗАЩИТИТЕ ВОРОТА");
            return;
        }
        objectiveIcon.sprite = TroyHudArt.Icon("gate");
        if (currentEncounter == 0 && !active)
        {
            objectiveText.text = L("FORTIFY THE LANDING", "УКРЕПИТЕ БЕРЕГ");
            progressText.text = L("COVER BOTH ROUTES • BUILD BEFORE CONTACT", "ПЕРЕКРОЙТЕ ОБА МАРШРУТА • ПОСТРОЙТЕСЬ ДО АТАКИ");
            return;
        }
        if (active)
        {
            objectiveText.text = currentEncounter == 1 ? L("HOLD THE LANDING", "УДЕРЖИТЕ БЕРЕГ") : L("DEFEND THE GATE", "ЗАЩИТИТЕ ВОРОТА");
            progressText.text = L("HOLD THE LINE • ADAPT TO THE ENEMY MIX", "ДЕРЖИТЕ СТРОЙ • АДАПТИРУЙТЕСЬ К СОСТАВУ ВРАГА");
            return;
        }
        if (between)
        {
            objectiveText.text = currentEncounter == 1 ? L("FIRST ASSAULT REPELLED", "ПЕРВЫЙ ШТУРМ ОТБИТ") : L("REGROUP AND REINFORCE", "ПЕРЕГРУППИРУЙТЕСЬ");
            progressText.text = L("REPAIR • UPGRADE • REDEPLOY", "РЕМОНТ • УЛУЧШЕНИЕ • ПЕРЕСТРОЕНИЕ");
            return;
        }
        objectiveText.text = L("DEFEND THE GATE", "ЗАЩИТИТЕ ВОРОТА");
        progressText.text = L("KEEP THE GATE STANDING", "НЕ ДАЙТЕ ВОРОТАМ ПАСТЬ");
    }

    void UpdateTutorial(GameManager gm)
    {
        int stage = TutorialStage(gm);
        if (stage != lastTutorialStage)
        {
            lastTutorialStage = stage;
            tutorialShownAt = Time.unscaledTime;
            ApplyTutorial(stage);
        }
        bool persistent = stage == 0 || stage == 4;
        bool visible = stage >= 0 && (persistent || Time.unscaledTime - tutorialShownAt < 10f);
        tutorialCard.SetActive(visible);
    }

    int TutorialStage(GameManager gm)
    {
        if (gm.BossDefeated) return -1;
        int currentEncounter = EncounterRuntime.CurrentEncounter(spawner);
        int maxEncounters = Mathf.Max(1, EncounterRuntime.MaxEncounters);
        bool active = EncounterRuntime.EncounterActive(spawner);
        if (EncounterRuntime.NextEncounterHasBoss(spawner) || currentEncounter >= maxEncounters) return 4;
        if (gm.TowersBuilt == 0) return 0;
        if (currentEncounter == 0 && spawner != null && !active) return 1;
        if (currentEncounter == 1 && active) return 2;
        if (currentEncounter == 1 && EncounterRuntime.BetweenEncounters(spawner)) return 5;
        if (currentEncounter >= 2 && currentEncounter < maxEncounters && gm.TowersBuilt > 0) return 3;
        return -1;
    }

    void ApplyTutorial(int stage)
    {
        switch (stage)
        {
            case 0: tutorialIcon.sprite = TroyHudArt.Tower(TowerType.MachineGun); tutorialTitle.text = L("FIRST DEFENSE", "ПЕРВАЯ ОБОРОНА"); tutorialText.text = L("Choose 1–6, inspect counters, then place your first defense on a build point.", "Выберите 1–6, изучите контрмеры и поставьте первую оборону на точке строительства."); break;
            case 1: tutorialIcon.sprite = TroyHudArt.Icon("enemy"); tutorialTitle.text = L("PREPARE THE SHORE", "ПОДГОТОВЬТЕ БЕРЕГ"); tutorialText.text = L("Read the enemy roster and deploy counters. The first assault begins when preparation ends.", "Изучите состав врага и расставьте контрмеры. Первый штурм начнётся после подготовки."); break;
            case 2: tutorialIcon.sprite = TroyHudArt.Portrait("hector"); tutorialTitle.text = L("COMMAND HECTOR", "УПРАВЛЕНИЕ ГЕКТОРОМ"); tutorialText.text = L("LMB Hector to select • RMB road to move • Q/E/R/F or use the HUD abilities.", "ЛКМ по Гектору — выбрать • ПКМ по дороге — идти • Q/E/R/F или способности в HUD."); break;
            case 3: tutorialIcon.sprite = TroyHudArt.Tower(TowerType.Cannon); tutorialTitle.text = L("ADAPT THE DEFENSE", "АДАПТИРУЙТЕ ОБОРОНУ"); tutorialText.text = L("Upgrade, sell and change target priority as the Greek assault changes.", "Улучшайте, продавайте и меняйте приоритет целей по мере изменения греческого штурма."); break;
            case 4: tutorialIcon.sprite = TroyHudArt.Portrait("menelaus"); tutorialTitle.text = L("BOSS • MENELAUS", "БОСС • МЕНЕЛАЙ"); tutorialText.text = L("His aura strengthens nearby Greeks. Control the escort and focus your strongest anti-heavy defenses.", "Его аура усиливает греков рядом. Сдерживайте сопровождение и сфокусируйте сильнейшую тяжёлую оборону."); break;
            case 5: tutorialIcon.sprite = TroyHudArt.Icon("gate"); tutorialTitle.text = L("THE FIRST LINE HOLDS", "ПЕРВАЯ АТАКА ОТБИТА"); tutorialText.text = L("Use the lull to reinforce weak routes and inspect the next enemy group before battle two.", "Используйте передышку: усилите слабые дороги и изучите следующую группу врагов перед вторым боем."); break;
            default: tutorialCard.SetActive(false); break;
        }
    }

    bool HasActiveBoss(){foreach(Enemy enemy in EnemyRegistry.All) if(enemy!=null&&enemy.Archetype==EnemyArchetype.Boss&&enemy.Health>0f)return true;return false;}
    bool IsMenuBlockingCombat(){if(menuCanvas==null)return false;string[] names={"MainMenu","LevelSelect","Settings","PauseMenu","EndMenu","ConfirmationModal"};for(int i=0;i<names.Length;i++){Transform t=menuCanvas.transform.Find(names[i]);if(t!=null&&t.gameObject.activeInHierarchy)return true;}return false;}

    GameObject Panel(Transform parent,string name,Vector2 pos,Vector2 size,Vector2 anchor,Vector2 pivot){GameObject go=new GameObject(name);go.transform.SetParent(parent,false);Image image=go.AddComponent<Image>();image.sprite=TroyHudArt.Panel();image.type=Image.Type.Sliced;image.color=Color.white;image.raycastTarget=false;RectTransform rt=image.rectTransform;rt.anchorMin=rt.anchorMax=anchor;rt.pivot=pivot;rt.anchoredPosition=pos;rt.sizeDelta=size;return go;}
    Image AddIcon(Transform parent,string name,Vector2 pos,float size,Sprite sprite){GameObject go=new GameObject(name);go.transform.SetParent(parent,false);Image image=go.AddComponent<Image>();image.sprite=sprite;image.raycastTarget=false;RectTransform rt=image.rectTransform;rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f);rt.anchoredPosition=pos;rt.sizeDelta=new Vector2(size,size);return image;}
    Text AddText(Transform parent,string value,Vector2 pos,Vector2 size,int fontSize,Color color,TextAnchor alignment,FontStyle style,Vector2 anchor){GameObject go=new GameObject("Text");go.transform.SetParent(parent,false);Text text=go.AddComponent<Text>();text.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");text.text=value;text.fontSize=fontSize;text.color=color;text.alignment=alignment;text.fontStyle=style;text.horizontalOverflow=HorizontalWrapMode.Wrap;text.verticalOverflow=VerticalWrapMode.Truncate;text.raycastTarget=false;RectTransform rt=text.rectTransform;rt.anchorMin=rt.anchorMax=rt.pivot=anchor;rt.anchoredPosition=pos;rt.sizeDelta=size;return text;}
}
