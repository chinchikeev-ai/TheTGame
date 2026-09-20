using UnityEngine;
using UnityEngine.UI;
using static CombatHudTowerCatalog;
using static CombatHudUiFactory;

public sealed class ModernCombatHud : MonoBehaviour
{
    public static ModernCombatHud Instance { get; private set; }
    public Transform HudRoot { get; private set; }

    static readonly string[] BlockingMenuNames =
    {
        "MainMenu", "LevelSelect", "Settings", "PauseMenu", "EndMenu", "ConfirmationModal"
    };

    Canvas canvas;
    CanvasGroup group;
    TowerPlacement placement;
    EnemySpawner spawner;
    Canvas legacyCanvas;
    Canvas menuCanvas;
    Camera gameplayCamera;

    Text goldText, gateText, encounterText, threatText, encounterPreviewText, encounterProgressText;
    Text speedText, magicActionText;
    Text selectedTitle, selectedStats, selectedPriority, selectedUpgradePreview;
    Text firstEncounterPrepTitle, firstEncounterPrepObjective, firstEncounterPrepComposition;
    Button upgradeButton, sellButton, priorityButton, startEncounterButton;
    Button magicAction;
    Button firstEncounterPrepStartButton;
    Button defenseToggleButton;
    Button settingsButton;
    Text defenseToggleText;
    Text buildSelectionText;

    GameObject encounterBar;
    GameObject firstEncounterPrep;
    GameObject buildDock;
    GameObject buildTooltip;
    GameObject selectedCard;
    RectTransform selectedCardRect;
    Image gateHealthFill;
    Image encounterProgressFill;
    Image tooltipAccent;
    Image tooltipPortrait;
    Text tooltipTitle, tooltipRole, tooltipStats, tooltipMatchup;
    TowerType hoveredBuildType;
    bool buildTooltipVisible;
    bool defenseDockOpen;

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
    TowerType RecommendedDefense() => CombatHudRecommendationPolicy.Recommend(spawner, buildTypes);

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        placement = FindFirstObjectByType<TowerPlacement>();
        spawner = EnemySpawner.Instance;
        gameplayCamera = placement != null && placement.gameCamera != null ? placement.gameCamera : Camera.main;
        FindCanvases();
        Build();
    }

    void FindCanvases()
    {
        GameObject legacyObject = GameObject.Find("GameCanvas");
        legacyCanvas = legacyObject != null ? legacyObject.GetComponent<Canvas>() : null;
        GameObject menuObject = GameObject.Find("MenuCanvas");
        menuCanvas = GameMenuController.Instance != null ? GameMenuController.Instance.MenuCanvas : (menuObject != null ? menuObject.GetComponent<Canvas>() : null);
        if (legacyCanvas != null) legacyCanvas.enabled = false;
    }

    void Build()
    {
        GameObject root = new GameObject("ModernCombatHUD");
        HudRoot = root.transform;
        canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 80;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;

        root.AddComponent<GraphicRaycaster>();
        group = root.AddComponent<CanvasGroup>();

        BuildTopBar(root.transform);
        BuildEncounterBar(root.transform);
        BuildFirstEncounterPreparation(root.transform);
        BuildDock(root.transform);
        BuildDefenseToggle(root.transform);
        BuildBuildTooltip(root.transform);
        BuildSelectedCard(root.transform);
        BuildMagicCornerControl(root.transform);
    }

    void BuildTopBar(Transform parent)
    {
        GameObject bar = new GameObject("TopResources", typeof(RectTransform));
        bar.transform.SetParent(parent, false);
        RectTransform barRect = bar.transform as RectTransform;
        barRect.anchorMin = barRect.anchorMax = barRect.pivot = new Vector2(0f, 1f);
        barRect.anchoredPosition = new Vector2(20f, -20f);
        barRect.sizeDelta = new Vector2(530f, 154f);

        GameObject goldPanel = Panel(bar.transform, "GoldResourcePanel", new Vector2(-150f, 48f), new Vector2(212f, 54f), new Color(.42f, .22f, .08f, 1f), new Vector2(.5f, .5f), new Vector2(.5f, .5f));
        Image coin = Icon(bar.transform, "CoinIcon", new Vector2(-242f, 48f), new Vector2(56f, 56f), TroyHudArt.Icon("gold"));
        coin.color = Color.white;
        goldText = Text(bar.transform, "0", new Vector2(-137f, 48f), new Vector2(128f, 38f), 27, new Color(1f, .88f, .48f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);

        Panel(bar.transform, "SpeedControlPanel", new Vector2(78f, 48f), new Vector2(174f, 54f), new Color(.12f, .065f, .035f, .98f), new Vector2(.5f, .5f), new Vector2(.5f, .5f));
        Button speedDown = Button(bar.transform, "<", new Vector2(28f, 48f), new Vector2(42f, 42f), DecreaseSpeed, false);
        speedDown.gameObject.name = "SpeedPrevious";
        speedText = Text(bar.transform, "1x", new Vector2(78f, 48f), new Vector2(58f, 38f), 17, new Color(1f, .82f, .40f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        speedText.gameObject.name = "SpeedValue";
        Button speedUp = Button(bar.transform, ">", new Vector2(128f, 48f), new Vector2(42f, 42f), IncreaseSpeed, false);
        speedUp.gameObject.name = "SpeedNext";

        settingsButton = Button(bar.transform, "", new Vector2(218f, 48f), new Vector2(54f, 54f), OpenSettings, false);
        settingsButton.gameObject.name = "CombatSettingsButton";
        Text settingsLabel = settingsButton.GetComponentInChildren<Text>();
        if (settingsLabel != null)
        {
            settingsLabel.text = L("SET", "НАСТР");
            settingsLabel.fontSize = 10;
            settingsLabel.resizeTextForBestFit = true;
            settingsLabel.resizeTextMinSize = 8;
            settingsLabel.resizeTextMaxSize = 10;
        }

        GameObject gatePanel = Panel(bar.transform, "GateResourcePanel", new Vector2(-35f, -38f), new Vector2(450f, 76f), Color.white, new Vector2(.5f, .5f), new Vector2(.5f, .5f));
        Image gateIcon = Icon(bar.transform, "GateIcon", new Vector2(-216f, -38f), new Vector2(68f, 68f), null);
        gateText = Text(bar.transform, L("GATE", "ВОРОТА"), new Vector2(18f, -22f), new Vector2(310f, 28f), 22, new Color(1f, .94f, .72f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        gateText.gameObject.name = "GateHealthLabel";
        gateHealthFill = ProgressBar(bar.transform, "GateHealthProgress", new Vector2(18f, -54f), new Vector2(310f, 20f), new Color(.11f, .025f, .02f, 1f), new Color(.92f, .035f, .07f, 1f));
        gatePanel.AddComponent<GateHudArtwork>().Apply(gatePanel.GetComponent<Image>(), gateIcon, gateHealthFill);
    }

    void BuildEncounterBar(Transform parent)
    {
        encounterBar = Panel(parent, "WaveStatus", new Vector2(0, -18), new Vector2(820, 138), new Color(.050f, .027f, .018f, .96f), new Vector2(.5f, 1), new Vector2(.5f, 1));
        Panel(encounterBar.transform, "WaveBannerCloth", new Vector2(0, 30), new Vector2(680, 64), new Color(.48f, .050f, .025f, .97f), new Vector2(.5f, .5f), new Vector2(.5f, .5f));
        Icon(encounterBar.transform, "WaveLeftLaurel", new Vector2(-310, 31), new Vector2(48, 48), TroyHudArt.Icon("sword"));
        Icon(encounterBar.transform, "WaveRightLaurel", new Vector2(310, 31), new Vector2(48, 48), TroyHudArt.Icon("sword"));

        encounterText = Text(encounterBar.transform, "ENCOUNTER", new Vector2(0, 42), new Vector2(430, 38), 26, new Color(1f, .86f, .48f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        threatText = Text(encounterBar.transform, "THREAT", new Vector2(0, 9), new Vector2(600, 22), 12, new Color(1f, .91f, .72f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        encounterPreviewText = Text(encounterBar.transform, "", new Vector2(0, -14), new Vector2(500, 20), 11, new Color(.93f, .82f, .67f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        encounterPreviewText.enabled = false;
        encounterProgressFill = ProgressBar(encounterBar.transform, "WaveProgress", new Vector2(0, -24), new Vector2(560, 16), new Color(.16f, .09f, .055f, 1f), new Color(1f, .58f, .12f, 1f));
        encounterProgressText = Text(encounterBar.transform, "0%", new Vector2(0, -45), new Vector2(280, 20), 11, new Color(.95f, .84f, .67f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        startEncounterButton = Button(encounterBar.transform, L("START", "СТАРТ"), new Vector2(350, -25), new Vector2(104, 58), StartEncounter, true);
    }

    void BuildMagicCornerControl(Transform parent)
    {
        GameObject actions = new GameObject("DivinePowerActions", typeof(RectTransform));
        actions.transform.SetParent(parent, false);
        RectTransform actionsRect = actions.transform as RectTransform;
        actionsRect.anchorMin = actionsRect.anchorMax = actionsRect.pivot = new Vector2(1f, 0f);
        actionsRect.anchoredPosition = new Vector2(-24f, 24f);
        actionsRect.sizeDelta = new Vector2(340f, 116f);

        magicAction = Button(actions.transform, "", new Vector2(-106f, 0f), new Vector2(164f, 112f), CastMagic, true);
        magicAction.gameObject.name = "Magic_Primary";
        Icon(magicAction.transform, "MagicIcon", new Vector2(0f, 27f), new Vector2(54f, 54f), TroyHudArt.Ability("magic"));
        magicActionText = magicAction.GetComponentInChildren<Text>();
        if (magicActionText != null)
        {
            magicActionText.fontSize = 11;
            magicActionText.lineSpacing = .9f;
            magicActionText.resizeTextForBestFit = true;
            magicActionText.resizeTextMinSize = 8;
            magicActionText.resizeTextMaxSize = 12;
            RectTransform textRect = magicActionText.rectTransform;
            textRect.anchoredPosition = new Vector2(0f, -28f);
            textRect.sizeDelta = new Vector2(146f, 46f);
        }
    }

    void CastMagic()
    {
        GameManager gm = GameManager.Instance;
        if (gm != null) gm.UseMagic();
    }

    void OpenSettings()
    {
        GameMenuController menu = FindFirstObjectByType<GameMenuController>();
        if (menu != null) menu.OpenCombatSettings();
    }

    void BuildFirstEncounterPreparation(Transform parent)
    {
        firstEncounterPrep = Panel(parent, "FirstWavePreparation", new Vector2(0, -176), new Vector2(760, 330), new Color(.035f, .022f, .016f, .975f), new Vector2(.5f, 1f), new Vector2(.5f, 1f));
        firstEncounterPrepTitle = Text(firstEncounterPrep.transform, "", new Vector2(0, 112), new Vector2(680, 62), 34, new Color(1f, .68f, .22f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        Text(firstEncounterPrep.transform, L("CHAPTER I • THE LANDING", "ГЛАВА I • ВЫСАДКА"), new Vector2(0, 66), new Vector2(620, 30), 13, new Color(.76f, .67f, .58f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        firstEncounterPrepObjective = Text(firstEncounterPrep.transform, "", new Vector2(0, 24), new Vector2(640, 42), 17, new Color(.96f, .88f, .75f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        firstEncounterPrepComposition = Text(firstEncounterPrep.transform, "", new Vector2(0, -30), new Vector2(660, 52), 14, new Color(.90f, .79f, .64f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        firstEncounterPrepStartButton = Button(firstEncounterPrep.transform, L("START ENCOUNTER", "НАЧАТЬ БОЙ"), new Vector2(0, -105), new Vector2(300, 64), StartEncounter, true);
        firstEncounterPrep.SetActive(false);
    }

    void BuildDock(Transform parent)
    {
        buildDock = Panel(parent, "BuildDock", new Vector2(0f, 22f), new Vector2(940f, 190f), new Color(.040f, .024f, .016f, .96f), new Vector2(.5f, 0f), new Vector2(.5f, 0f));
        Panel(buildDock.transform, "BuildDockHeader", new Vector2(0, 78), new Vector2(850, 40), new Color(.44f, .050f, .025f, .97f), new Vector2(.5f, .5f), new Vector2(.5f, .5f));
        Text(buildDock.transform, L("TROJAN DEFENDERS", "ЗАЩИТНИКИ ТРОИ"), new Vector2(-250, 78), new Vector2(330, 30), 20, new Color(1f, .86f, .50f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        buildSelectionText = Text(buildDock.transform, "", new Vector2(198, 78), new Vector2(480, 24), 11, new Color(1f, .78f, .34f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        for (int i = 0; i < buildTypes.Length; i++)
        {
            TowerType type = buildTypes[i];
            float x = -390 + i * 156;
            buildButtons[i] = Button(buildDock.transform, "", new Vector2(x, -18), new Vector2(140, 134), () => SelectBuild(type), false);
            buildButtons[i].gameObject.name = "BuildCard_" + type;
            Icon(buildButtons[i].transform, "TowerIcon", new Vector2(0, 30), new Vector2(62, 62), TroyHudArt.Tower(type));
            Text(buildButtons[i].transform, buildHotkeys[i], new Vector2(-56, 49), new Vector2(24, 22), 13, new Color(1f, .82f, .36f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
            Text towerName = Text(buildButtons[i].transform, TowerName(type).ToUpperInvariant(), new Vector2(0, -24), new Vector2(132, 28), 12, new Color(1f, .90f, .68f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
            towerName.resizeTextForBestFit = true;
            towerName.resizeTextMinSize = 9;
            towerName.resizeTextMaxSize = 12;
            Icon(buildButtons[i].transform, "CostCoin", new Vector2(-26, -50), new Vector2(24, 24), TroyHudArt.Icon("gold"));
            Text(buildButtons[i].transform, TowerFactory.GetCost(type).ToString(), new Vector2(20, -50), new Vector2(64, 22), 12, new Color(1f, .82f, .36f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
            BuildButtonHoverRelay relay = buildButtons[i].gameObject.AddComponent<BuildButtonHoverRelay>();
            relay.Initialize(type, () => ShowBuildTooltip(type), HideBuildTooltip);
        }
        defenseDockOpen = false;
        buildDock.SetActive(false);
    }

    void BuildDefenseToggle(Transform parent)
    {
        defenseToggleButton = Button(parent, L("DEFENDERS", "ЗАЩИТА"), new Vector2(-24f, 24f), new Vector2(164f, 112f), ToggleDefenseDock, true);
        defenseToggleButton.gameObject.name = "DefendersToggle";
        RectTransform rt = defenseToggleButton.transform as RectTransform;
        if (rt != null)
        {
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1f, 0f);
            rt.anchoredPosition = new Vector2(-24f, 24f);
        }
        Icon(defenseToggleButton.transform, "DefendersIcon", new Vector2(0f, 27f), new Vector2(54f, 54f), TroyHudArt.Tower(TowerType.TrojanGuard));
        defenseToggleText = defenseToggleButton.GetComponentInChildren<Text>();
        if (defenseToggleText != null)
        {
            defenseToggleText.rectTransform.anchoredPosition = new Vector2(0f, -28f);
            defenseToggleText.rectTransform.sizeDelta = new Vector2(146f, 46f);
            defenseToggleText.fontSize = 11;
            defenseToggleText.fontStyle = FontStyle.Bold;
            defenseToggleText.lineSpacing = .9f;
            defenseToggleText.resizeTextForBestFit = true;
            defenseToggleText.resizeTextMinSize = 8;
            defenseToggleText.resizeTextMaxSize = 12;
        }
    }

    void ToggleDefenseDock()
    {
        defenseDockOpen = !defenseDockOpen;
        if (buildDock != null) buildDock.SetActive(defenseDockOpen);
        if (!defenseDockOpen) HideBuildTooltip();
        if (defenseToggleText != null) defenseToggleText.text = defenseDockOpen ? L("DEFENDERS\nCLOSE", "ЗАЩИТА\nЗАКРЫТЬ") : L("DEFENDERS", "ЗАЩИТА");
    }

    void BuildBuildTooltip(Transform parent)
    {
        buildTooltip = Panel(parent, "BuildHoverTooltip", new Vector2(0f, 220f), new Vector2(520, 190), new Color(.028f, .018f, .014f, .985f), new Vector2(.5f, 0f), new Vector2(.5f, 0f));
        buildTooltip.GetComponent<Image>().raycastTarget = false;
        GameObject accentObject = new GameObject("Accent");
        accentObject.transform.SetParent(buildTooltip.transform, false);
        tooltipAccent = accentObject.AddComponent<Image>();
        tooltipAccent.raycastTarget = false;
        RectTransform accentRt = tooltipAccent.rectTransform;
        accentRt.anchorMin = accentRt.anchorMax = accentRt.pivot = new Vector2(0f, .5f);
        accentRt.anchoredPosition = new Vector2(8f, 0f);
        accentRt.sizeDelta = new Vector2(8f, 168f);
        tooltipPortrait = Icon(buildTooltip.transform, "TooltipTowerPortrait", new Vector2(-200f, 34f), new Vector2(84f, 84f), TroyHudArt.Tower(TowerType.TrojanGuard));
        tooltipTitle = Text(buildTooltip.transform, "", new Vector2(-112, 62), new Vector2(300, 32), 18, new Color(1f, .76f, .31f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        tooltipRole = Text(buildTooltip.transform, "", new Vector2(-112, 30), new Vector2(300, 28), 12, new Color(.78f, .70f, .61f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        tooltipStats = Text(buildTooltip.transform, "", new Vector2(-112, -12), new Vector2(300, 42), 12, new Color(.93f, .87f, .79f, 1f), TextAnchor.MiddleLeft, FontStyle.Normal);
        tooltipMatchup = Text(buildTooltip.transform, "", new Vector2(120, -18), new Vector2(210, 126), 12, new Color(.78f, .91f, .70f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        buildTooltip.SetActive(false);
    }

    void BuildSelectedCard(Transform parent)
    {
        selectedCard = Panel(parent, "SelectedTowerCard", Vector2.zero, new Vector2(390, 300), new Color(.045f, .027f, .018f, .97f), new Vector2(.5f, .5f), new Vector2(.5f, .5f));
        selectedCardRect = selectedCard.transform as RectTransform;
        Panel(selectedCard.transform, "SelectedTowerHeader", new Vector2(0, 101), new Vector2(350, 82), new Color(.42f, .050f, .025f, .97f), new Vector2(.5f, .5f), new Vector2(.5f, .5f));
        Icon(selectedCard.transform, "SelectedTowerCrest", new Vector2(-136, 101), new Vector2(82, 82), TroyHudArt.Tower(TowerType.TrojanGuard));
        selectedTitle = Text(selectedCard.transform, "", new Vector2(43, 108), new Vector2(245, 40), 18, new Color(1f, .86f, .50f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        Text(selectedCard.transform, L("TROJAN DEFENSE", "ОБОРОНА ТРОИ"), new Vector2(43, 80), new Vector2(245, 22), 10, new Color(.74f, .65f, .55f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);

        Panel(selectedCard.transform, "SelectedStatsPanel", new Vector2(0, 26), new Vector2(350, 92), new Color(.075f, .044f, .028f, .94f), new Vector2(.5f, .5f), new Vector2(.5f, .5f));
        selectedStats = Text(selectedCard.transform, "", new Vector2(0, 26), new Vector2(316, 74), 13, new Color(.96f, .89f, .76f, 1f), TextAnchor.UpperLeft, FontStyle.Bold);

        Panel(selectedCard.transform, "SelectedUpgradePanel", new Vector2(0, -58), new Vector2(350, 62), new Color(.10f, .056f, .030f, .94f), new Vector2(.5f, .5f), new Vector2(.5f, .5f));
        selectedUpgradePreview = Text(selectedCard.transform, "", new Vector2(0, -58), new Vector2(316, 46), 11, new Color(1f, .78f, .34f, 1f), TextAnchor.UpperLeft, FontStyle.Bold);
        selectedPriority = Text(selectedCard.transform, "", new Vector2(0, -99), new Vector2(330, 22), 11, new Color(.86f, .76f, .64f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);

        upgradeButton = Button(selectedCard.transform, L("UPGRADE", "УЛУЧШИТЬ"), new Vector2(-112, -128), new Vector2(118, 40), () => placement?.UpgradeSelected(), true);
        sellButton = Button(selectedCard.transform, L("SELL", "ПРОДАТЬ"), new Vector2(12, -128), new Vector2(104, 40), () => placement?.SellSelected(), false);
        priorityButton = Button(selectedCard.transform, L("PRIORITY", "ПРИОРИТЕТ"), new Vector2(124, -128), new Vector2(112, 40), () => placement?.CycleSelectedPriority(), false);
        selectedCard.SetActive(false);
    }

    void Update()
    {
        if (placement == null) placement = FindFirstObjectByType<TowerPlacement>();
        if (spawner == null) spawner = EnemySpawner.Instance;
        if (gameplayCamera == null) gameplayCamera = placement != null && placement.gameCamera != null ? placement.gameCamera : Camera.main;
        if (menuCanvas == null && GameMenuController.Instance != null) menuCanvas = GameMenuController.Instance.MenuCanvas;

        bool blocked = IsMenuBlockingCombat();
        if (group != null)
        {
            group.alpha = blocked ? 0f : 1f;
            group.interactable = !blocked;
            group.blocksRaycasts = !blocked;
        }
        if (blocked || GameManager.Instance == null)
        {
            HideBuildTooltip();
            if (selectedCard != null) selectedCard.SetActive(false);
            if (firstEncounterPrep != null) firstEncounterPrep.SetActive(false);
            return;
        }

        UpdateResources();
        UpdateFirstEncounterPreparation();
        UpdateEncounter();
        UpdateActions();
        UpdateBuildDock();
        UpdateSelected();
        if (buildTooltipVisible) RefreshBuildTooltip();
        HandlePcHotkeys();
    }

    bool IsMenuBlockingCombat()
    {
        if (menuCanvas == null) return false;
        for (int i = 0; i < BlockingMenuNames.Length; i++)
        {
            Transform menu = menuCanvas.transform.Find(BlockingMenuNames[i]);
            if (menu != null && menu.gameObject.activeInHierarchy) return true;
        }
        return false;
    }

    void UpdateResources()
    {
        GameManager gm = GameManager.Instance;
        goldText.text = gm.Money.ToString();
        gateText.text = $"{L("GATE", "ВОРОТА")}   {gm.BaseHealth} / {gm.MaxBaseHealth}";
        if (gateHealthFill != null) gateHealthFill.fillAmount = gm.MaxBaseHealth > 0 ? Mathf.Clamp01(gm.BaseHealth / (float)gm.MaxBaseHealth) : 0f;
    }

    void UpdateFirstEncounterPreparation()
    {
        GameManager gm = GameManager.Instance;
        bool visible = spawner != null && gm != null && EncounterRuntime.CurrentEncounter(spawner) == 0 && !EncounterRuntime.EncounterActive(spawner) && EncounterRuntime.WaitingForEncounterStart(spawner) && !gm.GameEnded;
        if (firstEncounterPrep != null) firstEncounterPrep.SetActive(visible);
        if (encounterBar != null) encounterBar.SetActive(!visible);
        if (!visible) return;
        int seconds = Mathf.Max(0, Mathf.CeilToInt(EncounterRuntime.InterEncounterCountdown(spawner)));
        firstEncounterPrepTitle.text = L($"PREPARE FOR ATTACK — {seconds}", $"ПОДГОТОВКА К АТАКЕ — {seconds}");
        firstEncounterPrepObjective.text = L("OBJECTIVE • DEFEND THE GATE", "ЦЕЛЬ • ЗАЩИТИТЕ ВОРОТА");
        firstEncounterPrepComposition.text = CombatHudEncounterFormatter.BuildPreview(spawner);
        firstEncounterPrepStartButton.interactable = false;
    }

    void UpdateActions()
    {
        GameManager gm = GameManager.Instance;
        speedText.text = $"{CombatControlsUI.CurrentSpeed:0}x";
        float cooldown = gm != null ? gm.MagicCooldownRemaining : 0f;
        bool magicReady = gm != null && cooldown <= .01f && !gm.GameEnded && EnemyRegistry.AliveCount > 0;
        string state = gm != null && gm.GameEnded
            ? L("ENDED", "ЗАВЕРШЕНО")
            : cooldown > .01f
                ? L($"{Mathf.CeilToInt(cooldown)}s", $"{Mathf.CeilToInt(cooldown)}с")
                : EnemyRegistry.AliveCount <= 0
                    ? L("WAIT", "ЖДЁМ")
                    : L("READY", "ГОТОВО");
        if (magicActionText != null)
            magicActionText.text = L(
                $"MAGIC\n120 DMG • SLOW 5s\n{state}",
                $"МАГИЯ\n120 УРОНА • SLOW 5с\n{state}");
        if (magicAction != null) magicAction.interactable = magicReady;
    }

    void UpdateEncounter()
    {
        GameManager gm = GameManager.Instance;
        int currentEncounter = EncounterRuntime.CurrentEncounter(spawner);
        int maxEncounters = EncounterRuntime.MaxEncounters;
        int sec = Mathf.RoundToInt(EncounterRuntime.CurrentEncounterElapsed(spawner));
        encounterText.text = $"{L("ENCOUNTER", "БОЙ")} {currentEncounter}/{maxEncounters}   •   {sec / 60:00}:{sec % 60:00}";
        if (spawner == null)
        {
            threatText.text = "";
            encounterPreviewText.text = "";
            if (encounterProgressFill != null) encounterProgressFill.fillAmount = 0f;
            if (encounterProgressText != null) encounterProgressText.text = "0%";
            startEncounterButton.gameObject.SetActive(false);
            return;
        }

        string threat = EncounterRuntime.NextEncounterHasBoss(spawner) ? L("BOSS APPROACHING: MENELAUS", "ПРИБЛИЖАЕТСЯ БОСС: МЕНЕЛАЙ") : EncounterRuntime.NextEncounterHasHeavy(spawner) ? L("HEAVY FORMATION EXPECTED", "ОЖИДАЕТСЯ ТЯЖЁЛАЯ ФОРМАЦИЯ") : L("STANDARD ENEMY FORMATION", "ОБЫЧНАЯ ВРАЖЕСКАЯ ФОРМАЦИЯ");
        if (EncounterRuntime.EncounterActive(spawner)) threatText.text = $"{EnemyRegistry.AliveCount} {L("ENEMIES REMAIN", "ВРАГОВ В СТРОЮ")} • {threat}";
        else if (EncounterRuntime.InterEncounterCountdown(spawner) > 0f) threatText.text = $"{L("NEXT ENCOUNTER IN", "СЛЕДУЮЩИЙ БОЙ ЧЕРЕЗ")} {Mathf.CeilToInt(EncounterRuntime.InterEncounterCountdown(spawner))}{L("s", "с")} • {EncounterRuntime.NextEncounterEnemyCount(spawner)} {L("enemies", "врагов")}";
        else threatText.text = $"{L("READY", "ГОТОВО")} • {EncounterRuntime.NextEncounterEnemyCount(spawner)} {L("enemies", "врагов")} • {threat}";

        float progress = EncounterRuntime.CurrentEncounterProgress(spawner);
        if (encounterProgressFill != null) encounterProgressFill.fillAmount = progress;
        if (encounterProgressText != null) encounterProgressText.text = EncounterRuntime.EncounterActive(spawner) ? $"{EncounterRuntime.CurrentEncounterResolvedEnemies(spawner)} / {Mathf.Max(1, EncounterRuntime.CurrentEncounterTotalEnemies(spawner))}   •   {Mathf.RoundToInt(progress * 100f)}%" : currentEncounter > 0 ? "100%" : "0%";
        encounterPreviewText.text = CombatHudEncounterFormatter.BuildPreview(spawner);
        bool canStart = EncounterRuntime.WaitingForEncounterStart(spawner) && !EncounterRuntime.EncounterActive(spawner) && !gm.GameEnded && currentEncounter > 0;
        startEncounterButton.gameObject.SetActive(canStart);
        startEncounterButton.interactable = canStart;
    }

    void StartEncounter()
    {
        if (firstEncounterPrep != null) firstEncounterPrep.SetActive(false);
        if (encounterBar != null) encounterBar.SetActive(true);
        EncounterRuntime.StartEncounterNow(spawner);
    }

    void UpdateBuildDock() => CombatHudTowerPanelPresenter.RefreshBuildDock(placement, spawner, buildTypes, buildButtons, buildSelectionText);

    void ShowBuildTooltip(TowerType type)
    {
        if (!defenseDockOpen) return;
        hoveredBuildType = type;
        buildTooltipVisible = true;
        if (buildTooltip != null) buildTooltip.SetActive(true);
        RefreshBuildTooltip();
    }

    void HideBuildTooltip()
    {
        buildTooltipVisible = false;
        if (buildTooltip != null) buildTooltip.SetActive(false);
    }

    void RefreshBuildTooltip()
    {
        if (!defenseDockOpen || !buildTooltipVisible || buildTooltip == null) return;
        TowerData data = BalanceCatalog.GetTower(hoveredBuildType);
        if (data == null) return;
        bool recommended = hoveredBuildType == RecommendedDefense();
        bool affordable = GameManager.Instance != null && GameManager.Instance.Money >= data.cost;
        if (tooltipPortrait != null) tooltipPortrait.sprite = TroyHudArt.Tower(hoveredBuildType);
        tooltipAccent.color = recommended ? new Color(1f, .68f, .16f, 1f) : affordable ? new Color(.30f, .82f, .36f, 1f) : new Color(.82f, .18f, .08f, 1f);
        tooltipTitle.text = TowerDisplayName(hoveredBuildType) + (recommended ? " • " + L("RECOMMENDED", "РЕКОМЕНДУЕТСЯ") : "");
        tooltipRole.text = TowerRole(hoveredBuildType);
        float dps = data.damage * Mathf.Max(.01f, data.attacksPerSecond);
        tooltipStats.text = $"{L("DMG", "УРОН")} {data.damage:0.#} • DPS {dps:0.#} • {L("RNG", "ДАЛЬН")} {data.range:0.0} • {data.cost} {L("GOLD", "ЗОЛОТА")}";
        tooltipMatchup.text = $"+ {L("STRONG", "СИЛЁН")}\n{StrongAgainst(hoveredBuildType)}\n\n− {L("WEAK", "СЛАБ")}\n{WeakAgainst(hoveredBuildType)}";
    }

    void UpdateSelected()
    {
        Tower selected = placement != null ? placement.SelectedTower : null;
        bool visible = selected != null && !defenseDockOpen;
        if (selectedCard != null) selectedCard.SetActive(visible);
        if (!visible) return;
        PositionSelectedCard(selected);
        CombatHudTowerPanelPresenter.RefreshSelected(placement, selectedTitle, selectedStats, selectedPriority, selectedUpgradePreview, upgradeButton, sellButton, priorityButton);
    }

    void PositionSelectedCard(Tower selected)
    {
        if (selected == null || selectedCardRect == null || canvas == null || gameplayCamera == null) return;
        RectTransform canvasRect = canvas.transform as RectTransform;
        if (canvasRect == null) return;

        Vector3 screenPoint = gameplayCamera.WorldToScreenPoint(selected.transform.position + Vector3.up * 1.2f);
        if (screenPoint.z <= 0f) return;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out Vector2 localPoint)) return;

        float direction = screenPoint.x < Screen.width * .5f ? 1f : -1f;
        localPoint += new Vector2(direction * 215f, 58f);

        float halfWidth = selectedCardRect.rect.width * .5f;
        float halfHeight = selectedCardRect.rect.height * .5f;
        const float margin = 18f;
        localPoint.x = Mathf.Clamp(localPoint.x, canvasRect.rect.xMin + halfWidth + margin, canvasRect.rect.xMax - halfWidth - margin);
        localPoint.y = Mathf.Clamp(localPoint.y, canvasRect.rect.yMin + halfHeight + margin, canvasRect.rect.yMax - halfHeight - margin);
        selectedCardRect.anchoredPosition = localPoint;
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

    Image ProgressBar(Transform parent, string name, Vector2 pos, Vector2 size, Color trackColor, Color fillColor)
    {
        GameObject track = new GameObject(name);
        track.transform.SetParent(parent, false);
        Image trackImage = track.AddComponent<Image>();
        trackImage.color = trackColor;
        trackImage.raycastTarget = false;
        RectTransform trackRect = trackImage.rectTransform;
        trackRect.anchorMin = trackRect.anchorMax = trackRect.pivot = new Vector2(.5f, .5f);
        trackRect.anchoredPosition = pos;
        trackRect.sizeDelta = size;
        GameObject fillObject = new GameObject("Fill");
        fillObject.transform.SetParent(track.transform, false);
        Image fill = fillObject.AddComponent<Image>();
        fill.color = fillColor;
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = 0;
        fill.fillAmount = 1f;
        fill.raycastTarget = false;
        RectTransform fillRect = fill.rectTransform;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(2f, 2f);
        fillRect.offsetMax = new Vector2(-2f, -2f);
        return fill;
    }

    void SelectBuild(TowerType type) => placement?.SelectBuildType(type);
    void IncreaseSpeed() => CombatControlsUI.IncreaseSpeed();
    void DecreaseSpeed() => CombatControlsUI.DecreaseSpeed();
}
