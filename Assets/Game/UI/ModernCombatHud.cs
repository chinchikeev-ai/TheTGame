using UnityEngine;
using UnityEngine.UI;
using static CombatHudTowerCatalog;
using static CombatHudUiFactory;

public sealed class ModernCombatHud : MonoBehaviour
{
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

    Text goldText, gateText, waveText, threatText, wavePreviewText, waveProgressText;
    Text speedText, giftText, magicToggleText, magicActionText;
    Text selectedTitle, selectedStats, selectedPriority, selectedUpgradePreview;
    Text firstWavePrepTitle, firstWavePrepObjective, firstWavePrepComposition;
    Button upgradeButton, sellButton, priorityButton, startWaveButton, giftButton;
    Button magicToggle, magicAction;
    Button firstWavePrepStartButton;
    Button defenseToggleButton;
    Text defenseToggleText;
    Text buildSelectionText;

    GameObject waveBar;
    GameObject firstWavePrep;
    GameObject buildDock;
    GameObject buildTooltip;
    GameObject selectedCard;
    RectTransform selectedCardRect;
    GameObject giftChoiceOverlay;
    GameObject magicFlyout;
    Image gateHealthFill;
    Image waveProgressFill;
    Image tooltipAccent;
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

    void Start()
    {
        placement = FindFirstObjectByType<TowerPlacement>();
        spawner = FindFirstObjectByType<EnemySpawner>();
        gameplayCamera = placement != null && placement.gameCamera != null ? placement.gameCamera : Camera.main;
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
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;

        root.AddComponent<GraphicRaycaster>();
        group = root.AddComponent<CanvasGroup>();

        BuildTopBar(root.transform);
        BuildWaveBar(root.transform);
        BuildActionPanel(root.transform);
        BuildGiftChoice(root.transform);
        BuildFirstWavePreparation(root.transform);
        BuildDock(root.transform);
        BuildDefenseToggle(root.transform);
        BuildBuildTooltip(root.transform);
        BuildSelectedCard(root.transform);
        BuildMagicCornerControl(root.transform);
    }

    void BuildTopBar(Transform parent)
    {
        GameObject bar = Panel(parent, "TopResources", new Vector2(24, -24), new Vector2(330, 132), new Color(.035f, .022f, .016f, .94f), new Vector2(0, 1), new Vector2(0, 1));
        Image coin = Icon(bar.transform, "CoinIcon", new Vector2(-128, 34), new Vector2(34, 34), CoinSprite());
        coin.color = Color.white;
        goldText = Text(bar.transform, "0", new Vector2(-96, 34), new Vector2(190, 46), 25, new Color(1f, .73f, .24f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        gateText = Text(bar.transform, L("GATE", "ВОРОТА"), new Vector2(0, -4), new Vector2(278, 30), 14, new Color(.94f, .84f, .67f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        gateHealthFill = ProgressBar(bar.transform, "GateHealthProgress", new Vector2(0, -42), new Vector2(278, 16), new Color(.18f, .08f, .045f, 1f), new Color(.79f, .22f, .08f, 1f));
    }

    void BuildWaveBar(Transform parent)
    {
        waveBar = Panel(parent, "WaveStatus", new Vector2(0, -24), new Vector2(760, 152), new Color(.035f, .022f, .016f, .95f), new Vector2(.5f, 1), new Vector2(.5f, 1));
        waveText = Text(waveBar.transform, "WAVE", new Vector2(0, 54), new Vector2(500, 30), 20, new Color(1f, .75f, .32f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        threatText = Text(waveBar.transform, "THREAT", new Vector2(0, 27), new Vector2(520, 24), 12, new Color(.86f, .76f, .64f, 1f), TextAnchor.MiddleCenter, FontStyle.Normal);
        wavePreviewText = Text(waveBar.transform, "", new Vector2(0, 2), new Vector2(500, 22), 11, new Color(.93f, .82f, .67f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        wavePreviewText.enabled = false;
        waveProgressFill = ProgressBar(waveBar.transform, "WaveProgress", new Vector2(0, -8), new Vector2(500, 14), new Color(.16f, .09f, .055f, 1f), new Color(1f, .58f, .12f, 1f));
        waveProgressText = Text(waveBar.transform, "0%", new Vector2(0, -27), new Vector2(220, 20), 11, new Color(.95f, .84f, .67f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);

        Button speedDown = Button(waveBar.transform, "<", new Vector2(-62, -55), new Vector2(46, 38), DecreaseSpeed, false);
        speedDown.gameObject.name = "SpeedPrevious";
        speedText = Text(waveBar.transform, "1x", new Vector2(0, -55), new Vector2(68, 38), 16, new Color(1f, .78f, .34f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        speedText.gameObject.name = "SpeedValue";
        Button speedUp = Button(waveBar.transform, ">", new Vector2(62, -55), new Vector2(46, 38), IncreaseSpeed, false);
        speedUp.gameObject.name = "SpeedNext";
        startWaveButton = Button(waveBar.transform, L("START", "СТАРТ"), new Vector2(310, -2), new Vector2(116, 76), StartWave, true);
    }

    void BuildActionPanel(Transform parent)
    {
        GameObject panel = Panel(parent, "CombatActions", new Vector2(-24, -24), new Vector2(330, 66), new Color(.035f, .022f, .016f, .92f), new Vector2(1, 1), new Vector2(1, 1));
        giftButton = Button(panel.transform, "", Vector2.zero, new Vector2(282, 40), ToggleGiftChoice, false);
        giftText = giftButton.GetComponentInChildren<Text>();
    }

    void BuildMagicCornerControl(Transform parent)
    {
        GameObject container = new GameObject("MagicCornerControls", typeof(RectTransform));
        container.transform.SetParent(parent, false);
        RectTransform containerRect = container.GetComponent<RectTransform>();
        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;

        magicToggle = Button(container.transform, L("MAGIC", "МАГИЯ"), Vector2.zero, new Vector2(64, 64), ToggleMagicFlyout, true);
        magicToggle.gameObject.name = "MagicToggle";
        RectTransform toggleRect = magicToggle.transform as RectTransform;
        toggleRect.anchorMin = toggleRect.anchorMax = toggleRect.pivot = new Vector2(1f, 0f);
        toggleRect.anchoredPosition = new Vector2(-24f, 104f);
        magicToggleText = magicToggle.GetComponentInChildren<Text>();
        if (magicToggleText != null)
        {
            magicToggleText.fontSize = 10;
            magicToggleText.resizeTextForBestFit = true;
            magicToggleText.resizeTextMinSize = 8;
            magicToggleText.resizeTextMaxSize = 11;
        }

        magicFlyout = Panel(container.transform, "MagicFlyout", new Vector2(-24f, 184f), new Vector2(300f, 84f), new Color(.035f, .022f, .016f, .97f), new Vector2(1f, 0f), new Vector2(1f, 0f));
        magicAction = Button(magicFlyout.transform, "", Vector2.zero, new Vector2(268f, 54f), CastMagic, true);
        magicAction.gameObject.name = "Magic_Primary";
        magicActionText = magicAction.GetComponentInChildren<Text>();
        if (magicActionText != null) magicActionText.fontSize = 13;
        magicFlyout.SetActive(false);
    }

    void ToggleMagicFlyout()
    {
        if (magicFlyout == null) return;
        bool show = !magicFlyout.activeSelf;
        CloseGiftChoice();
        magicFlyout.SetActive(show);
        if (show)
        {
            HideBuildTooltip();
            if (selectedCard != null) selectedCard.SetActive(false);
        }
    }

    void CloseMagicFlyout()
    {
        if (magicFlyout != null) magicFlyout.SetActive(false);
    }

    void CastMagic()
    {
        GameManager gm = GameManager.Instance;
        if (gm != null && gm.UseMagic()) CloseMagicFlyout();
    }

    void BuildGiftChoice(Transform parent)
    {
        giftChoiceOverlay = new GameObject("DivineGiftChoiceOverlay");
        giftChoiceOverlay.transform.SetParent(parent, false);
        Image backdrop = giftChoiceOverlay.AddComponent<Image>();
        backdrop.color = new Color(.012f, .008f, .006f, .72f);
        RectTransform backdropRect = backdrop.rectTransform;
        backdropRect.anchorMin = Vector2.zero;
        backdropRect.anchorMax = Vector2.one;
        backdropRect.offsetMin = Vector2.zero;
        backdropRect.offsetMax = Vector2.zero;

        GameObject panel = Panel(giftChoiceOverlay.transform, "DivineGiftChoicePanel", Vector2.zero, new Vector2(780, 350), new Color(.045f, .026f, .016f, .995f), new Vector2(.5f, .5f), new Vector2(.5f, .5f));
        Text(panel.transform, L("CHOOSE A DIVINE GIFT", "ВЫБЕРИТЕ ДАР БОГА"), new Vector2(0, 132), new Vector2(650, 44), 27, new Color(1f, .70f, .24f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        Text(panel.transform, L("Choose once. The blessing lasts for this map.", "Выберите один раз. Благословение действует всю карту."), new Vector2(0, 96), new Vector2(650, 30), 13, new Color(.78f, .69f, .60f, 1f), TextAnchor.MiddleCenter, FontStyle.Normal);
        Button ares = Button(panel.transform, L("ARES\n+10% PLAYER DAMAGE", "АРЕС\n+10% УРОНА ИГРОКА"), new Vector2(-190, 31), new Vector2(330, 82), () => ChooseGift(DivineGiftType.Ares), true);
        ares.gameObject.name = "Gift_Ares";
        Button athena = Button(panel.transform, L("ATHENA\nGATE +2 HP", "АФИНА\nВОРОТА +2 HP"), new Vector2(190, 31), new Vector2(330, 82), () => ChooseGift(DivineGiftType.Athena), false);
        athena.gameObject.name = "Gift_Athena";
        Button apollo = Button(panel.transform, L("APOLLO\n+100 GOLD", "АПОЛЛОН\n+100 ЗОЛОТА"), new Vector2(-190, -68), new Vector2(330, 82), () => ChooseGift(DivineGiftType.Apollo), false);
        apollo.gameObject.name = "Gift_Apollo";
        Button poseidon = Button(panel.transform, L("POSEIDON\nENEMIES -10% SPEED", "ПОСЕЙДОН\nВРАГИ -10% СКОРОСТИ"), new Vector2(190, -68), new Vector2(330, 82), () => ChooseGift(DivineGiftType.Poseidon), false);
        poseidon.gameObject.name = "Gift_Poseidon";
        Button close = Button(panel.transform, L("CANCEL", "ОТМЕНА"), new Vector2(0, -142), new Vector2(200, 40), CloseGiftChoice, false);
        close.gameObject.name = "GiftChoiceCancel";
        giftChoiceOverlay.SetActive(false);
    }

    void BuildFirstWavePreparation(Transform parent)
    {
        firstWavePrep = Panel(parent, "FirstWavePreparation", new Vector2(0, -176), new Vector2(760, 330), new Color(.035f, .022f, .016f, .975f), new Vector2(.5f, 1f), new Vector2(.5f, 1f));
        firstWavePrepTitle = Text(firstWavePrep.transform, "", new Vector2(0, 112), new Vector2(680, 62), 34, new Color(1f, .68f, .22f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        Text(firstWavePrep.transform, L("CHAPTER I • THE LANDING", "ГЛАВА I • ВЫСАДКА"), new Vector2(0, 66), new Vector2(620, 30), 13, new Color(.76f, .67f, .58f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        firstWavePrepObjective = Text(firstWavePrep.transform, "", new Vector2(0, 24), new Vector2(640, 42), 17, new Color(.96f, .88f, .75f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        firstWavePrepComposition = Text(firstWavePrep.transform, "", new Vector2(0, -30), new Vector2(660, 52), 14, new Color(.90f, .79f, .64f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        firstWavePrepStartButton = Button(firstWavePrep.transform, L("START WAVE", "НАЧАТЬ ВОЛНУ"), new Vector2(0, -105), new Vector2(300, 64), StartWave, true);
        firstWavePrep.SetActive(false);
    }

    void BuildDock(Transform parent)
    {
        buildDock = Panel(parent, "BuildDock", new Vector2(-104f, 24f), new Vector2(1040, 148), new Color(.035f, .022f, .016f, .95f), new Vector2(1f, 0f), new Vector2(1f, 0f));
        Text(buildDock.transform, L("TROJAN DEFENSES", "ОБОРОНА ТРОИ"), new Vector2(-438, 52), new Vector2(240, 26), 13, new Color(.74f, .65f, .55f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        buildSelectionText = Text(buildDock.transform, "", new Vector2(310, 52), new Vector2(560, 26), 12, new Color(1f, .72f, .28f, 1f), TextAnchor.MiddleRight, FontStyle.Bold);
        for (int i = 0; i < buildTypes.Length; i++)
        {
            TowerType type = buildTypes[i];
            float x = -397 + i * 158;
            buildButtons[i] = Button(buildDock.transform, BuildLabel(type, buildHotkeys[i]), new Vector2(x, -17), new Vector2(144, 78), () => SelectBuild(type), false);
            BuildButtonHoverRelay relay = buildButtons[i].gameObject.AddComponent<BuildButtonHoverRelay>();
            relay.Initialize(type, () => ShowBuildTooltip(type), HideBuildTooltip);
        }
        defenseDockOpen = false;
        buildDock.SetActive(false);
    }

    void BuildDefenseToggle(Transform parent)
    {
        defenseToggleButton = Button(parent, "+", new Vector2(-24, 24), new Vector2(64, 64), ToggleDefenseDock, true);
        RectTransform rt = defenseToggleButton.transform as RectTransform;
        if (rt != null)
        {
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1f, 0f);
            rt.anchoredPosition = new Vector2(-24f, 24f);
        }
        defenseToggleText = defenseToggleButton.GetComponentInChildren<Text>();
        if (defenseToggleText != null)
        {
            defenseToggleText.fontSize = 28;
            defenseToggleText.fontStyle = FontStyle.Bold;
        }
    }

    void ToggleDefenseDock()
    {
        defenseDockOpen = !defenseDockOpen;
        if (buildDock != null) buildDock.SetActive(defenseDockOpen);
        if (!defenseDockOpen) HideBuildTooltip();
        if (defenseDockOpen) CloseMagicFlyout();
        if (defenseToggleText != null) defenseToggleText.text = defenseDockOpen ? "−" : "+";
    }

    void BuildBuildTooltip(Transform parent)
    {
        buildTooltip = Panel(parent, "BuildHoverTooltip", new Vector2(-104f, 190f), new Vector2(500, 166), new Color(.028f, .018f, .014f, .985f), new Vector2(1f, 0f), new Vector2(1f, 0f));
        buildTooltip.GetComponent<Image>().raycastTarget = false;
        GameObject accentObject = new GameObject("Accent");
        accentObject.transform.SetParent(buildTooltip.transform, false);
        tooltipAccent = accentObject.AddComponent<Image>();
        tooltipAccent.raycastTarget = false;
        RectTransform accentRt = tooltipAccent.rectTransform;
        accentRt.anchorMin = accentRt.anchorMax = accentRt.pivot = new Vector2(0f, .5f);
        accentRt.anchoredPosition = new Vector2(8f, 0f);
        accentRt.sizeDelta = new Vector2(8f, 146f);
        tooltipTitle = Text(buildTooltip.transform, "", new Vector2(-108, 52), new Vector2(286, 28), 18, new Color(1f, .76f, .31f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        tooltipRole = Text(buildTooltip.transform, "", new Vector2(-108, 24), new Vector2(286, 26), 12, new Color(.78f, .70f, .61f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        tooltipStats = Text(buildTooltip.transform, "", new Vector2(-108, -10), new Vector2(286, 36), 12, new Color(.93f, .87f, .79f, 1f), TextAnchor.MiddleLeft, FontStyle.Normal);
        tooltipMatchup = Text(buildTooltip.transform, "", new Vector2(118, -2), new Vector2(208, 106), 12, new Color(.78f, .91f, .70f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        buildTooltip.SetActive(false);
    }

    void BuildSelectedCard(Transform parent)
    {
        selectedCard = Panel(parent, "SelectedTowerCard", Vector2.zero, new Vector2(340, 274), new Color(.045f, .027f, .018f, .97f), new Vector2(.5f, .5f), new Vector2(.5f, .5f));
        selectedCardRect = selectedCard.transform as RectTransform;
        selectedTitle = Text(selectedCard.transform, "", new Vector2(0, 103), new Vector2(304, 34), 18, new Color(1f, .70f, .28f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        selectedStats = Text(selectedCard.transform, "", new Vector2(0, 45), new Vector2(304, 72), 13, new Color(.94f, .87f, .77f, 1f), TextAnchor.UpperLeft, FontStyle.Normal);
        selectedUpgradePreview = Text(selectedCard.transform, "", new Vector2(0, -17), new Vector2(304, 44), 11, new Color(1f, .73f, .31f, 1f), TextAnchor.UpperLeft, FontStyle.Bold);
        selectedPriority = Text(selectedCard.transform, "", new Vector2(0, -54), new Vector2(304, 24), 11, new Color(.82f, .72f, .62f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        upgradeButton = Button(selectedCard.transform, L("UPGRADE", "УЛУЧШИТЬ"), new Vector2(-78, -91), new Vector2(144, 42), () => placement?.UpgradeSelected(), true);
        sellButton = Button(selectedCard.transform, L("SELL", "ПРОДАТЬ"), new Vector2(78, -91), new Vector2(144, 42), () => placement?.SellSelected(), false);
        priorityButton = Button(selectedCard.transform, L("TARGET PRIORITY", "ПРИОРИТЕТ ЦЕЛИ"), new Vector2(0, -122), new Vector2(304, 30), () => placement?.CycleSelectedPriority(), false);
        selectedCard.SetActive(false);
    }

    void Update()
    {
        if (placement == null) placement = FindFirstObjectByType<TowerPlacement>();
        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();
        if (gameplayCamera == null) gameplayCamera = placement != null && placement.gameCamera != null ? placement.gameCamera : Camera.main;
        if (menuCanvas == null) FindCanvases();

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
            CloseGiftChoice();
            CloseMagicFlyout();
            if (selectedCard != null) selectedCard.SetActive(false);
            if (firstWavePrep != null) firstWavePrep.SetActive(false);
            return;
        }

        UpdateResources();
        UpdateFirstWavePreparation();
        UpdateWave();
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

    void UpdateFirstWavePreparation()
    {
        GameManager gm = GameManager.Instance;
        bool visible = spawner != null && gm != null && gm.CurrentWave == 0 && !spawner.WaveActive && spawner.WaitingForManualStart && !gm.GameEnded;
        if (firstWavePrep != null) firstWavePrep.SetActive(visible);
        if (waveBar != null) waveBar.SetActive(!visible);
        if (!visible) return;
        int seconds = Mathf.Max(0, Mathf.CeilToInt(spawner.InterWaveCountdown));
        firstWavePrepTitle.text = L($"PREPARE FOR ATTACK — {seconds}", $"ПОДГОТОВКА К АТАКЕ — {seconds}");
        firstWavePrepObjective.text = L("OBJECTIVE • DEFEND THE GATE", "ЦЕЛЬ • ЗАЩИТИТЕ ВОРОТА");
        firstWavePrepComposition.text = CombatHudWaveFormatter.BuildPreview(spawner);
        firstWavePrepStartButton.interactable = false;
    }

    void UpdateActions()
    {
        GameManager gm = GameManager.Instance;
        speedText.text = $"{CombatControlsUI.CurrentSpeed:0}x";
        float cooldown = gm != null ? gm.MagicCooldownRemaining : 0f;
        bool magicReady = gm != null && cooldown <= .01f && !gm.GameEnded && EnemyRegistry.AliveCount > 0;
        if (magicToggleText != null) magicToggleText.text = cooldown > .01f ? Mathf.CeilToInt(cooldown) + L("s", "с") : L("MAGIC", "МАГИЯ");
        if (magicActionText != null) magicActionText.text = cooldown > .01f ? L("DIVINE POWER   ", "БОЖЕСТВЕННАЯ СИЛА   ") + Mathf.CeilToInt(cooldown) + L("s", "с") : L("DIVINE POWER   READY", "БОЖЕСТВЕННАЯ СИЛА   ГОТОВА");
        if (magicAction != null) magicAction.interactable = magicReady;
        if (magicToggle != null) magicToggle.interactable = gm != null && !gm.GameEnded;
        if (gm == null || gm.GameEnded) CloseMagicFlyout();

        giftText.text = gm != null && gm.GiftAvailable ? L("CHOOSE DIVINE GIFT", "ВЫБРАТЬ ДАР БОГА") : gm != null && gm.GiftSelected ? L("DIVINE GIFT SELECTED", "ДАР БОГА ВЫБРАН") : L("DIVINE GIFT", "ДАР БОГА");
        giftButton.interactable = gm != null && gm.GiftAvailable && !gm.GameEnded;
        if (gm == null || !gm.GiftAvailable) CloseGiftChoice();
    }

    void UpdateWave()
    {
        GameManager gm = GameManager.Instance;
        int sec = spawner != null ? Mathf.RoundToInt(spawner.CurrentWaveElapsed) : 0;
        waveText.text = $"{L("WAVE", "ВОЛНА")} {gm.CurrentWave}/{gm.MaxWaves}   •   {sec / 60:00}:{sec % 60:00}";
        if (spawner == null)
        {
            threatText.text = "";
            wavePreviewText.text = "";
            if (waveProgressFill != null) waveProgressFill.fillAmount = 0f;
            if (waveProgressText != null) waveProgressText.text = "0%";
            startWaveButton.gameObject.SetActive(false);
            return;
        }

        string threat = spawner.NextWaveHasBoss ? L("BOSS APPROACHING: MENELAUS", "ПРИБЛИЖАЕТСЯ БОСС: МЕНЕЛАЙ") : spawner.NextWaveHasHeavy ? L("HEAVY FORMATION EXPECTED", "ОЖИДАЕТСЯ ТЯЖЁЛАЯ ФОРМАЦИЯ") : L("STANDARD ENEMY FORMATION", "ОБЫЧНАЯ ВРАЖЕСКАЯ ФОРМАЦИЯ");
        if (spawner.WaveActive) threatText.text = $"{EnemyRegistry.AliveCount} {L("ENEMIES REMAIN", "ВРАГОВ В СТРОЮ")} • {threat}";
        else if (spawner.InterWaveCountdown > 0) threatText.text = $"{L("NEXT WAVE IN", "СЛЕДУЮЩАЯ ВОЛНА ЧЕРЕЗ")} {Mathf.CeilToInt(spawner.InterWaveCountdown)}{L("s", "с")} • {spawner.NextWaveEnemyCount} {L("enemies", "врагов")}";
        else threatText.text = $"{L("READY", "ГОТОВО")} • {spawner.NextWaveEnemyCount} {L("enemies", "врагов")} • {threat}";

        float progress = spawner.CurrentWaveProgress;
        if (waveProgressFill != null) waveProgressFill.fillAmount = progress;
        if (waveProgressText != null) waveProgressText.text = spawner.WaveActive ? $"{spawner.CurrentWaveResolvedEnemies} / {Mathf.Max(1, spawner.CurrentWaveTotalEnemies)}   •   {Mathf.RoundToInt(progress * 100f)}%" : gm.CurrentWave > 0 ? "100%" : "0%";
        wavePreviewText.text = CombatHudWaveFormatter.BuildPreview(spawner);
        bool canStart = spawner.WaitingForManualStart && !spawner.WaveActive && !gm.GameEnded && gm.CurrentWave > 0;
        startWaveButton.gameObject.SetActive(canStart);
        startWaveButton.interactable = canStart;
    }

    void ToggleGiftChoice()
    {
        GameManager gm = GameManager.Instance;
        if (giftChoiceOverlay == null || gm == null || !gm.GiftAvailable) return;
        bool show = !giftChoiceOverlay.activeSelf;
        CloseMagicFlyout();
        giftChoiceOverlay.SetActive(show);
        if (show)
        {
            HideBuildTooltip();
            if (selectedCard != null) selectedCard.SetActive(false);
        }
    }

    void CloseGiftChoice() { if (giftChoiceOverlay != null) giftChoiceOverlay.SetActive(false); }
    void ChooseGift(DivineGiftType gift) { GameManager gm = GameManager.Instance; if (gm != null && gm.UseGift(gift)) CloseGiftChoice(); }

    void StartWave()
    {
        if (firstWavePrep != null) firstWavePrep.SetActive(false);
        if (waveBar != null) waveBar.SetActive(true);
        if (spawner != null) spawner.StartWaveNow();
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
        bool visible = selected != null && !defenseDockOpen && (giftChoiceOverlay == null || !giftChoiceOverlay.activeSelf) && (magicFlyout == null || !magicFlyout.activeSelf);
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
        localPoint += new Vector2(direction * 205f, 55f);

        float halfWidth = selectedCardRect.rect.width * .5f;
        float halfHeight = selectedCardRect.rect.height * .5f;
        const float margin = 18f;
        localPoint.x = Mathf.Clamp(localPoint.x, canvasRect.rect.xMin + halfWidth + margin, canvasRect.rect.xMax - halfWidth - margin);
        localPoint.y = Mathf.Clamp(localPoint.y, canvasRect.rect.yMin + halfHeight + margin, canvasRect.rect.yMax - halfHeight - margin);
        selectedCardRect.anchoredPosition = localPoint;
    }

    void HandlePcHotkeys()
    {
        if (placement == null || (giftChoiceOverlay != null && giftChoiceOverlay.activeSelf) || (magicFlyout != null && magicFlyout.activeSelf)) return;
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
