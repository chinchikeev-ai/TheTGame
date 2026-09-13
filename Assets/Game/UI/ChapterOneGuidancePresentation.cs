using UnityEngine;
using UnityEngine.UI;

public sealed class ChapterOneGuidancePresentation : MonoBehaviour
{
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
        spawner = FindFirstObjectByType<EnemySpawner>();
        GameObject menu = GameObject.Find("MenuCanvas");
        menuCanvas = menu != null ? menu.GetComponent<Canvas>() : null;
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

        objectiveCard = Panel(root.transform, "ChapterObjective", new Vector2(0f, -178f), new Vector2(650f, 98f), new Vector2(.5f,1f), new Vector2(.5f,1f));
        objectiveIcon = AddIcon(objectiveCard.transform,"ObjectiveIcon",new Vector2(-292,-48),46,TroyHudArt.Icon("gate"));
        chapterText = AddText(objectiveCard.transform, "", new Vector2(-248f, -18f), new Vector2(480f, 24f), 12, new Color(1f, .69f, .23f, 1f), TextAnchor.UpperLeft, FontStyle.Bold, new Vector2(0f, 1f));
        objectiveText = AddText(objectiveCard.transform, "", new Vector2(-248f, -42f), new Vector2(480f, 30f), 18, new Color(.96f, .88f, .75f, 1f), TextAnchor.UpperLeft, FontStyle.Bold, new Vector2(0f, 1f));
        progressText = AddText(objectiveCard.transform, "", new Vector2(-248f, -72f), new Vector2(560f, 22f), 12, new Color(.77f, .69f, .59f, 1f), TextAnchor.UpperLeft, FontStyle.Normal, new Vector2(0f, 1f));

        tutorialCard = Panel(root.transform, "ContextTutorial", new Vector2(24f, 84f), new Vector2(500f, 126f), new Vector2(0f,0f), new Vector2(0f,0f));
        tutorialIcon = AddIcon(tutorialCard.transform,"TutorialIcon",new Vector2(42,63),52,TroyHudArt.Tower(TowerType.MachineGun));
        tutorialTitle = AddText(tutorialCard.transform, "", new Vector2(78f, 88f), new Vector2(396f, 24f), 13, new Color(1f, .70f, .24f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold, new Vector2(0f, 0f));
        tutorialText = AddText(tutorialCard.transform, "", new Vector2(78f, 24f), new Vector2(396f, 58f), 14, new Color(.93f, .86f, .76f, 1f), TextAnchor.UpperLeft, FontStyle.Normal, new Vector2(0f, 0f));
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

        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();
        if (menuCanvas == null)
        {
            GameObject menu = GameObject.Find("MenuCanvas");
            menuCanvas = menu != null ? menu.GetComponent<Canvas>() : null;
        }

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

        if (gm.BossDefeated)
        {
            objectiveIcon.sprite = TroyHudArt.Icon("gate");
            objectiveText.text = L("OBJECTIVE COMPLETE", "ЦЕЛЬ ВЫПОЛНЕНА");
            progressText.text = L("Menelaus defeated • clear the battlefield", "Менелай повержен • зачистите поле боя");
            return;
        }

        if (spawner != null && (spawner.NextWaveHasBoss || gm.CurrentWave >= gm.MaxWaves))
        {
            objectiveIcon.sprite = TroyHudArt.Portrait("menelaus");
            objectiveText.text = L("FINAL OBJECTIVE • DEFEAT MENELAUS", "ФИНАЛЬНАЯ ЦЕЛЬ • ПОБЕДИТЕ МЕНЕЛАЯ");
            progressText.text = L("Commander Aura • periodic reinforcements", "Аура командира • периодические подкрепления");
            return;
        }

        objectiveIcon.sprite = TroyHudArt.Icon("gate");
        objectiveText.text = L("DEFEND THE GATE", "ЗАЩИТИТЕ ВОРОТА");
        int wave = Mathf.Clamp(gm.CurrentWave, 0, gm.MaxWaves);
        progressText.text = $"{L("WAVES", "ВОЛНЫ")} {wave}/{gm.MaxWaves}   •   {L("GATE", "ВОРОТА")} {gm.BaseHealth}/{gm.MaxBaseHealth}";
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
        if (gm.TowersBuilt == 0) return 0;
        if (gm.CurrentWave == 0 && spawner != null && !spawner.WaveActive) return 1;
        if (gm.CurrentWave == 1 && spawner != null && spawner.WaveActive) return 2;
        if (gm.CurrentWave >= 2 && gm.CurrentWave < gm.MaxWaves && gm.TowersBuilt > 0) return 3;
        if (spawner != null && spawner.NextWaveHasBoss) return 4;
        return -1;
    }

    void ApplyTutorial(int stage)
    {
        switch (stage)
        {
            case 0:
                tutorialIcon.sprite=TroyHudArt.Tower(TowerType.MachineGun);
                tutorialTitle.text=L("FIRST DEFENSE","ПЕРВАЯ ОБОРОНА");
                tutorialText.text=L("Choose 1–6, hover for counters, then click a build point.","Выберите 1–6, наведите для контрмер и нажмите точку строительства.");
                break;
            case 1:
                tutorialIcon.sprite=TroyHudArt.Icon("enemy");
                tutorialTitle.text=L("PREPARE THE LANDING","ПОДГОТОВЬТЕСЬ К ВЫСАДКЕ");
                tutorialText.text=L("Read the next wave, build counters, then START WAVE.","Изучите следующую волну, постройте контрмеры и запустите её.");
                break;
            case 2:
                tutorialIcon.sprite=TroyHudArt.Portrait("hector");
                tutorialTitle.text=L("HECTOR","ГЕКТОР");
                tutorialText.text=L("RMB moves Hector • Q/E/R/F abilities reinforce weak lanes.","ПКМ двигает Гектора • Q/E/R/F усиливают слабые линии.");
                break;
            case 3:
                tutorialIcon.sprite=TroyHudArt.Tower(TowerType.Cannon);
                tutorialTitle.text=L("ADAPT THE DEFENSE","АДАПТИРУЙТЕ ОБОРОНУ");
                tutorialText.text=L("Select deployed units to upgrade, sell or change target priority.","Выбирайте оборону: улучшайте, продавайте и меняйте приоритет целей.");
                break;
            case 4:
                tutorialIcon.sprite=TroyHudArt.Portrait("menelaus");
                tutorialTitle.text=L("BOSS • MENELAUS","БОСС • МЕНЕЛАЙ");
                tutorialText.text=L("Aura buffs nearby Greeks. Focus Ballista/Spears and control his escort.","Аура усиливает греков рядом. Фокусируйте баллисты/копья и контролируйте сопровождение.");
                break;
            default:
                tutorialCard.SetActive(false);
                break;
        }
    }

    bool HasActiveBoss()
    {
        foreach (Enemy enemy in EnemyRegistry.All)
            if (enemy != null && enemy.Archetype == EnemyArchetype.Boss && enemy.Health > 0f) return true;
        return false;
    }

    bool IsMenuBlockingCombat()
    {
        if (menuCanvas == null) return false;
        string[] names = { "MainMenu", "LevelSelect", "Settings", "PauseMenu", "EndMenu", "ConfirmationModal" };
        for (int i = 0; i < names.Length; i++)
        {
            Transform t = menuCanvas.transform.Find(names[i]);
            if (t != null && t.gameObject.activeInHierarchy) return true;
        }
        return false;
    }

    GameObject Panel(Transform parent, string name, Vector2 pos, Vector2 size, Vector2 anchor, Vector2 pivot)
    {
        GameObject go = new GameObject(name); go.transform.SetParent(parent,false);
        Image image = go.AddComponent<Image>(); image.sprite=TroyHudArt.Panel(); image.type=Image.Type.Sliced; image.color=Color.white; image.raycastTarget=false;
        RectTransform rt=image.rectTransform; rt.anchorMin=rt.anchorMax=anchor; rt.pivot=pivot; rt.anchoredPosition=pos; rt.sizeDelta=size; return go;
    }

    Image AddIcon(Transform parent,string name,Vector2 pos,float size,Sprite sprite)
    {
        GameObject go=new GameObject(name); go.transform.SetParent(parent,false); Image image=go.AddComponent<Image>(); image.sprite=sprite; image.raycastTarget=false; RectTransform rt=image.rectTransform; rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f); rt.anchoredPosition=pos; rt.sizeDelta=new Vector2(size,size); return image;
    }

    Text AddText(Transform parent, string value, Vector2 pos, Vector2 size, int fontSize, Color color, TextAnchor alignment, FontStyle style, Vector2 anchor)
    {
        GameObject go = new GameObject("Text"); go.transform.SetParent(parent,false);
        Text text=go.AddComponent<Text>(); text.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); text.text=value; text.fontSize=fontSize; text.color=color; text.alignment=alignment; text.fontStyle=style; text.horizontalOverflow=HorizontalWrapMode.Wrap; text.verticalOverflow=VerticalWrapMode.Truncate; text.raycastTarget=false;
        RectTransform rt=text.rectTransform; rt.anchorMin=rt.anchorMax=rt.pivot=anchor; rt.anchoredPosition=pos; rt.sizeDelta=size; return text;
    }
}
