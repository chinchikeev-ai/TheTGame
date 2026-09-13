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

        GameObject objective = Panel(root.transform, "ChapterObjective", new Vector2(24f, -116f), new Vector2(460f, 164f), new Color(.032f, .021f, .016f, .94f), new Vector2(0f, 1f), new Vector2(0f, 1f));
        chapterText = AddText(objective.transform, "", new Vector2(22f, -18f), new Vector2(410f, 28f), 13, new Color(1f, .69f, .23f, 1f), TextAnchor.UpperLeft, FontStyle.Bold, new Vector2(0f, 1f));
        objectiveText = AddText(objective.transform, "", new Vector2(22f, -52f), new Vector2(410f, 52f), 20, new Color(.96f, .88f, .75f, 1f), TextAnchor.UpperLeft, FontStyle.Bold, new Vector2(0f, 1f));
        progressText = AddText(objective.transform, "", new Vector2(22f, -108f), new Vector2(410f, 38f), 14, new Color(.77f, .69f, .59f, 1f), TextAnchor.UpperLeft, FontStyle.Normal, new Vector2(0f, 1f));

        tutorialCard = Panel(root.transform, "ContextTutorial", new Vector2(24f, 196f), new Vector2(520f, 118f), new Color(.025f, .017f, .013f, .96f), new Vector2(0f, 0f), new Vector2(0f, 0f));
        tutorialTitle = AddText(tutorialCard.transform, "", new Vector2(20f, 14f), new Vector2(470f, 28f), 14, new Color(1f, .70f, .24f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold, new Vector2(0f, .5f));
        tutorialText = AddText(tutorialCard.transform, "", new Vector2(20f, -24f), new Vector2(470f, 62f), 15, new Color(.93f, .86f, .76f, 1f), TextAnchor.UpperLeft, FontStyle.Normal, new Vector2(0f, .5f));
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

        UpdateObjective(gm);
        UpdateTutorial(gm);
    }

    void UpdateObjective(GameManager gm)
    {
        chapterText.text = L("CHAPTER I • THE LANDING", "ГЛАВА I • ВЫСАДКА");

        if (gm.BossDefeated)
        {
            objectiveText.text = L("OBJECTIVE COMPLETE", "ЦЕЛЬ ВЫПОЛНЕНА");
            progressText.text = L("Menelaus is defeated. Hold Troy until the battlefield is clear.", "Менелай повержен. Удерживайте Трою до полной зачистки поля.");
            return;
        }

        if (spawner != null && (spawner.NextWaveHasBoss || gm.CurrentWave >= gm.MaxWaves))
        {
            objectiveText.text = L("DEFEAT MENELAUS", "ПОБЕДИТЕ МЕНЕЛАЯ");
            progressText.text = L("Commander Aura empowers nearby Greeks • reinforcements arrive periodically", "Командирская аура усиливает греков рядом • периодически прибывают подкрепления");
            return;
        }

        objectiveText.text = L("DEFEND THE GATE", "ЗАЩИТИТЕ ВОРОТА");
        int wave = Mathf.Clamp(gm.CurrentWave, 0, gm.MaxWaves);
        progressText.text = $"{L("WAVES", "ВОЛНЫ")}  {wave}/{gm.MaxWaves}   •   {L("GATE", "ВОРОТА")}  {gm.BaseHealth}/{gm.MaxBaseHealth}";
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
        bool visible = stage >= 0 && (persistent || Time.unscaledTime - tutorialShownAt < 12f);
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
                tutorialTitle.text = L("FIRST DEFENSE", "ПЕРВАЯ ОБОРОНА");
                tutorialText.text = L("Choose a Tower-Unit with 1–6, then click a build point. Hover the build buttons to compare roles and counters.", "Выберите Tower-Unit клавишами 1–6, затем нажмите на точку строительства. Наведите на кнопки, чтобы сравнить роли и контрмеры.");
                break;
            case 1:
                tutorialTitle.text = L("PREPARE THE LANDING", "ПОДГОТОВЬТЕСЬ К ВЫСАДКЕ");
                tutorialText.text = L("Read the next-wave composition. Build counters, then press START WAVE when your defense is ready.", "Изучите состав следующей волны. Постройте контрмеры и нажмите НАЧАТЬ ВОЛНУ, когда оборона готова.");
                break;
            case 2:
                tutorialTitle.text = L("HECTOR", "ГЕКТОР");
                tutorialText.text = L("RMB moves Hector. Use Q / E / R / F abilities to reinforce a weak lane or stop a breakthrough.", "ПКМ перемещает Гектора. Используйте Q / E / R / F, чтобы усилить слабую линию или остановить прорыв.");
                break;
            case 3:
                tutorialTitle.text = L("ADAPT THE DEFENSE", "АДАПТИРУЙТЕ ОБОРОНУ");
                tutorialText.text = L("Select deployed Tower-Units to inspect range, upgrade them, sell them, or change targeting priority.", "Выбирайте установленные Tower-Unit: проверяйте дальность, улучшайте, продавайте и меняйте приоритет цели.");
                break;
            case 4:
                tutorialTitle.text = L("BOSS: MENELAUS", "БОСС: МЕНЕЛАЙ");
                tutorialText.text = L("His Commander Aura buffs nearby Greeks and he calls reinforcements. Focus Ballista/Spears on Menelaus and control the escort.", "Его Командирская аура усиливает греков рядом, а сам он вызывает подкрепления. Сфокусируйте баллисты/копья на Менелае и контролируйте сопровождение.");
                break;
            default:
                tutorialCard.SetActive(false);
                break;
        }
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

    GameObject Panel(Transform parent, string name, Vector2 pos, Vector2 size, Color color, Vector2 anchor, Vector2 pivot)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(.67f, .36f, .13f, .45f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);
        return go;
    }

    Text AddText(Transform parent, string value, Vector2 pos, Vector2 size, int fontSize, Color color, TextAnchor alignment, FontStyle style, Vector2 anchor)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.fontStyle = style;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.raycastTarget = false;
        RectTransform rt = text.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return text;
    }
}
