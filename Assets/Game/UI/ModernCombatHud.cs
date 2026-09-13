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
    Button upgradeButton, sellButton, priorityButton, startWaveButton, magicButton, giftButton;
    Text buildSelectionText;

    GameObject buildTooltip;
    GameObject selectedCard;
    Image tooltipAccent;
    Text tooltipTitle, tooltipRole, tooltipStats, tooltipMatchup;
    TowerType hoveredBuildType;
    bool buildTooltipVisible;

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
        BuildDock(root.transform);
        BuildBuildTooltip(root.transform);
        BuildSelectedCard(root.transform);
    }

    void BuildTopBar(Transform parent)
    {
        GameObject bar = Panel(parent, "TopResources", new Vector2(24, -24), new Vector2(470, 74), new Color(.035f, .022f, .016f, .92f), new Vector2(0, 1), new Vector2(0, 1));
        Image coin = Icon(bar.transform, "CoinIcon", new Vector2(-194, 0), new Vector2(34, 34), CoinSprite());
        coin.color = Color.white;
        goldText = Text(bar.transform, "0", new Vector2(-148, 0), new Vector2(86, 58), 23, new Color(1f, .73f, .24f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        gateText = Text(bar.transform, "GATE", new Vector2(-28, 0), new Vector2(160, 58), 17, new Color(.94f, .84f, .67f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        aliveText = Text(bar.transform, "ALIVE", new Vector2(142, 0), new Vector2(160, 58), 17, new Color(.94f, .84f, .67f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
    }

    void BuildWaveBar(Transform parent)
    {
        GameObject bar = Panel(parent, "WaveStatus", new Vector2(0, -24), new Vector2(760, 124), new Color(.035f, .022f, .016f, .95f), new Vector2(.5f, 1), new Vector2(.5f, 1));
        waveText = Text(bar.transform, "WAVE", new Vector2(-72, 40), new Vector2(500, 30), 20, new Color(1f, .75f, .32f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        threatText = Text(bar.transform, "THREAT", new Vector2(-72, 8), new Vector2(500, 28), 13, new Color(.86f, .76f, .64f, 1f), TextAnchor.MiddleCenter, FontStyle.Normal);
        wavePreviewText = Text(bar.transform, "", new Vector2(-72, -24), new Vector2(500, 28), 12, new Color(.93f, .82f, .67f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        startWaveButton = Button(bar.transform, L("START WAVE", "НАЧАТЬ ВОЛНУ"), new Vector2(292, 0), new Vector2(150, 74), StartWave, true);
    }

    void BuildActionPanel(Transform parent)
    {
        GameObject panel = Panel(parent, "CombatActions", new Vector2(-24, -24), new Vector2(350, 132), new Color(.035f, .022f, .016f, .92f), new Vector2(1, 1), new Vector2(1, 1));
        Button(panel.transform, "-", new Vector2(-142, 24), new Vector2(44, 44), DecreaseSpeed, false);
        speedText = Text(panel.transform, "1x", new Vector2(-94, 24), new Vector2(52, 44), 16, new Color(1f, .78f, .34f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        Button(panel.transform, "+", new Vector2(-46, 24), new Vector2(44, 44), IncreaseSpeed, false);
        magicButton = Button(panel.transform, "", new Vector2(78, 24), new Vector2(170, 44), () => GameManager.Instance?.UseMagic(), true);
        magicText = magicButton.GetComponentInChildren<Text>();
        giftButton = Button(panel.transform, "", new Vector2(0, -34), new Vector2(300, 38), () => GameManager.Instance?.UseGift(), false);
        giftText = giftButton.GetComponentInChildren<Text>();
    }

    void BuildDock(Transform parent)
    {
        GameObject dock = Panel(parent, "BuildDock", new Vector2(0, 24), new Vector2(1040, 148), new Color(.035f, .022f, .016f, .95f), new Vector2(.5f, 0), new Vector2(.5f, 0));
        Text(dock.transform, L("TROJAN DEFENSES", "ОБОРОНА ТРОИ"), new Vector2(-438, 52), new Vector2(240, 26), 13, new Color(.74f, .65f, .55f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        buildSelectionText = Text(dock.transform, "", new Vector2(310, 52), new Vector2(560, 26), 12, new Color(1f, .72f, .28f, 1f), TextAnchor.MiddleRight, FontStyle.Bold);

        for (int i = 0; i < buildTypes.Length; i++)
        {
            TowerType type = buildTypes[i];
            float x = -397 + i * 158;
            buildButtons[i] = Button(dock.transform, BuildLabel(type, buildHotkeys[i]), new Vector2(x, -17), new Vector2(144, 78), () => SelectBuild(type), false);
            BuildButtonHoverRelay relay = buildButtons[i].gameObject.AddComponent<BuildButtonHoverRelay>();
            relay.Initialize(type, () => ShowBuildTooltip(type), HideBuildTooltip);
        }
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
        selectedCard = Panel(parent, "SelectedTowerCard", new Vector2(-24, 200), new Vector2(390, 390), new Color(.045f, .027f, .018f, .97f), new Vector2(1, 0), new Vector2(1, 0));
        selectedTitle = Text(selectedCard.transform, "", new Vector2(0, 145), new Vector2(350, 42), 21, new Color(1f, .70f, .28f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        selectedStats = Text(selectedCard.transform, "", new Vector2(0, 68), new Vector2(350, 110), 15, new Color(.94f, .87f, .77f, 1f), TextAnchor.UpperLeft, FontStyle.Normal);
        selectedUpgradePreview = Text(selectedCard.transform, "", new Vector2(0, -22), new Vector2(350, 62), 13, new Color(1f, .73f, .31f, 1f), TextAnchor.UpperLeft, FontStyle.Bold);
        selectedPriority = Text(selectedCard.transform, "", new Vector2(0, -72), new Vector2(350, 30), 13, new Color(.82f, .72f, .62f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        upgradeButton = Button(selectedCard.transform, L("UPGRADE", "УЛУЧШИТЬ"), new Vector2(-92, -126), new Vector2(166, 52), () => placement?.UpgradeSelected(), true);
        sellButton = Button(selectedCard.transform, L("SELL", "ПРОДАТЬ"), new Vector2(92, -126), new Vector2(166, 52), () => placement?.SellSelected(), false);
        priorityButton = Button(selectedCard.transform, L("TARGET PRIORITY", "ПРИОРИТЕТ ЦЕЛИ"), new Vector2(0, -170), new Vector2(350, 38), () => placement?.CycleSelectedPriority(), false);
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
            return;
        }

        UpdateResources();
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
        bool canStart = spawner.WaitingForManualStart && !spawner.WaveActive && !gm.GameEnded;
        startWaveButton.gameObject.SetActive(canStart);
        startWaveButton.interactable = canStart;
    }

    void StartWave()
    {
        if (spawner != null) spawner.StartWaveNow();
    }

    void UpdateBuildDock()
    {
        CombatHudTowerPanelPresenter.RefreshBuildDock(placement, spawner, buildTypes, buildButtons, buildSelectionText);
    }

    void ShowBuildTooltip(TowerType type)
    {
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
        if (!buildTooltipVisible || buildTooltip == null) return;
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
        bool visible = selected != null;
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
