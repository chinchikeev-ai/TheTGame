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

    Text goldText, gateText, aliveText, waveText, threatText, wavePreviewText;
    Text speedText, magicText, giftText;
    Text selectedTitle, selectedStats, selectedPriority, selectedUpgradePreview;
    Text firstWavePrepTitle, firstWavePrepObjective, firstWavePrepComposition;
    Button upgradeButton, sellButton, priorityButton, startWaveButton, magicButton, giftButton;
    Button firstWavePrepStartButton;
    Button defenseToggleButton;
    Text defenseToggleText;
    Text buildSelectionText;

    GameObject waveBar;
    GameObject firstWavePrep;
    GameObject buildDock;
    GameObject buildTooltip;
    GameObject selectedCard;
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
        BuildFirstWavePreparation(root.transform);
        BuildDock(root.transform);
        BuildDefenseToggle(root.transform);
        BuildBuildTooltip(root.transform);
        BuildSelectedCard(root.transform);
    }

    void BuildTopBar(Transform parent)
    {
        GameObject bar = Panel(parent, "TopResources", new Vector2(24, -24), new Vector2(450, 72), new Color(.035f, .022f, .016f, .92f), new Vector2(0, 1), new Vector2(0, 1));
        Image coin = Icon(bar.transform, "CoinIcon", new Vector2(-184, 0), new Vector2(32, 32), CoinSprite());
        coin.color = Color.white;
        goldText = Text(bar.transform, "0", new Vector2(-142, 0), new Vector2(82, 54), 22, new Color(1f, .73f, .24f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        gateText = Text(bar.transform, "GATE", new Vector2(-25, 0), new Vector2(154, 54), 16, new Color(.94f, .84f, .67f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        aliveText = Text(bar.transform, "ALIVE", new Vector2(136, 0), new Vector2(150, 54), 16, new Color(.94f, .84f, .67f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
    }

    void BuildWaveBar(Transform parent)
    {
        waveBar = Panel(parent, "WaveStatus", new Vector2(0, -24), new Vector2(720, 118), new Color(.035f, .022f, .016f, .95f), new Vector2(.5f, 1), new Vector2(.5f, 1));
        waveText = Text(waveBar.transform, "WAVE", new Vector2(-68, 38), new Vector2(470, 28), 19, new Color(1f, .75f, .32f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        threatText = Text(waveBar.transform, "THREAT", new Vector2(-68, 10), new Vector2(470, 24), 12, new Color(.86f, .76f, .64f, 1f), TextAnchor.MiddleCenter, FontStyle.Normal);
        wavePreviewText = Text(waveBar.transform, "", new Vector2(-68, -28), new Vector2(470, 24), 11, new Color(.93f, .82f, .67f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        wavePreviewText.enabled = false;
        startWaveButton = Button(waveBar.transform, L("START WAVE", "НАЧАТЬ ВОЛНУ"), new Vector2(276, 0), new Vector2(142, 70), StartWave, true);
    }

    void BuildActionPanel(Transform parent)
    {
        GameObject panel = Panel(parent, "CombatActions", new Vector2(-24, -24), new Vector2(330, 124), new Color(.035f, .022f, .016f, .92f), new Vector2(1, 1), new Vector2(1, 1));
        Button(panel.transform, "-", new Vector2(-132, 23), new Vector2(42, 42), DecreaseSpeed, false);
        speedText = Text(panel.transform, "1x", new Vector2(-86, 23), new Vector2(48, 42), 15, new Color(1f, .78f, .34f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        Button(panel.transform, "+", new Vector2(-40, 23), new Vector2(42, 42), IncreaseSpeed, false);
        magicButton = Button(panel.transform, "", new Vector2(74, 23), new Vector2(158, 42), () => GameManager.Instance?.UseMagic(), true);
        magicText = magicButton.GetComponentInChildren<Text>();
        giftButton = Button(panel.transform, "", new Vector2(0, -32), new Vector2(282, 36), () => GameManager.Instance?.UseGift(), false);
        giftText = giftButton.GetComponentInChildren<Text>();
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
        buildDock = Panel(parent, "BuildDock", new Vector2(0, 24), new Vector2(1040, 148), new Color(.035f, .022f, .016f, .95f), new Vector2(.5f, 0), new Vector2(.5f, 0));
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
        if (defenseToggleText != null) defenseToggleText.text = defenseDockOpen ? "−" : "+";
    }

    void BuildBuildTooltip(Transform parent)
    {
        buildTooltip = Panel(parent, "BuildHoverTooltip", new Vector2(0, 210), new Vector2(500, 166), new Color(.028f, .018f, .014f, .985f), new Vector2(.5f, 0), new Vector2(.5f, 0));
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
        selectedCard = Panel(parent, "SelectedTowerCard", new Vector2(-400, 24), new Vector2(340, 274), new Color(.045f, .027f, .018f, .97f), new Vector2(1, 0), new Vector2(1, 0));
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
        gateText.text = $"{L("GATE", "ВОРОТА")} {gm.BaseHealth}/{gm.MaxBaseHealth}";
        aliveText.text = $"{L("ENEMIES", "ВРАГИ")} {EnemyRegistry.AliveCount}";
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
        firstWavePrepStartButton.interactable = !gm.GameEnded;
    }

    void UpdateActions()
    {
        GameManager gm = GameManager.Instance;
        float speed = CombatControlsUI.CurrentSpeed;
        speedText.text = $"{speed:0}x";

        float cooldown = gm != null ? gm.MagicCooldownRemaining : 0f;
        magicText.text = cooldown > 0f
            ? L("POWER ", "СИЛА ") + Mathf.CeilToInt(cooldown) + L("s", "с")
            : L("POWER READY", "СИЛА ГОТОВА");
        magicButton.interactable = gm != null && cooldown <= 0f && !gm.GameEnded;

        giftText.text = gm != null && gm.GiftAvailable
            ? L("DIVINE GIFT +100 / +2", "ДАР БОГА +100 / +2")
            : L("DIVINE GIFT USED", "ДАР БОГА ИСПОЛЬЗОВАН");
        giftButton.interactable = gm != null && gm.GiftAvailable && !gm.GameEnded;
    }

    void UpdateWave()
    {
        GameManager gm = GameManager.Instance;
        int sec = spawner != null ? Mathf.RoundToInt(spawner.CurrentWaveElapsed) : 0;
        waveText.text = $"{L("CHAPTER I", "ГЛАВА I")} • {L("WAVE", "ВОЛНА")} {gm.CurrentWave}/{gm.MaxWaves} • {sec / 60:00}:{sec % 60:00}";

        if (spawner == null)
        {
            threatText.text = "";
            wavePreviewText.text = "";
            startWaveButton.gameObject.SetActive(false);
            return;
        }

        string threat = spawner.NextWaveHasBoss
            ? L("BOSS APPROACHING: MENELAUS", "ПРИБЛИЖАЕТСЯ БОСС: МЕНЕЛАЙ")
            : spawner.NextWaveHasHeavy
                ? L("HEAVY FORMATION EXPECTED", "ОЖИДАЕТСЯ ТЯЖЁЛАЯ ФОРМАЦИЯ")
                : L("STANDARD ENEMY FORMATION", "ОБЫЧНАЯ ВРАЖЕСКАЯ ФОРМАЦИЯ");

        if (spawner.WaveActive)
            threatText.text = $"{L("WAVE ACTIVE", "ВОЛНА ИДЁТ")} • {EnemyRegistry.AliveCount} {L("alive", "в строю")} • {threat}";
        else if (spawner.InterWaveCountdown > 0)
            threatText.text = $"{L("NEXT WAVE IN", "СЛЕДУЮЩАЯ ВОЛНА ЧЕРЕЗ")} {Mathf.CeilToInt(spawner.InterWaveCountdown)}{L("s", "с")} • {spawner.NextWaveEnemyCount} {L("enemies", "врагов")}";
        else
            threatText.text = $"{L("READY", "ГОТОВО")} • {spawner.NextWaveEnemyCount} {L("enemies", "врагов")} • {threat}";

        wavePreviewText.text = CombatHudWaveFormatter.BuildPreview(spawner);
        bool canStart = spawner.WaitingForManualStart && !spawner.WaveActive && !gm.GameEnded && gm.CurrentWave > 0;
        startWaveButton.gameObject.SetActive(canStart);
        startWaveButton.interactable = canStart;
    }

    void StartWave()
    {
        if (firstWavePrep != null) firstWavePrep.SetActive(false);
        if (waveBar != null) waveBar.SetActive(true);
        if (spawner != null) spawner.StartWaveNow();
    }

    void UpdateBuildDock()
    {
        CombatHudTowerPanelPresenter.RefreshBuildDock(placement, spawner, buildTypes, buildButtons, buildSelectionText);
    }

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
        bool visible = selected != null && !defenseDockOpen;
        if (selectedCard != null) selectedCard.SetActive(visible);
        if (!visible) return;

        CombatHudTowerPanelPresenter.RefreshSelected(
            placement,
            selectedTitle,
            selectedStats,
            selectedPriority,
            selectedUpgradePreview,
            upgradeButton,
            sellButton,
            priorityButton);
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

    void SelectBuild(TowerType type) => placement?.SelectBuildType(type);

    void IncreaseSpeed() => CombatControlsUI.IncreaseSpeed();
    void DecreaseSpeed() => CombatControlsUI.DecreaseSpeed();
}
