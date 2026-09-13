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
        BuildActionPanel(root.transform);
        BuildWaveBar(root.transform);
        BuildDock(root.transform);
        BuildBuildTooltip(root.transform);
        BuildSelectedCard(root.transform);
        BuildPcHints(root.transform);
    }

    void BuildTopBar(Transform parent)
    {
        GameObject bar = Panel(parent, "TopResources", new Vector2(24, -24), new Vector2(660, 74), new Color(.035f, .022f, .016f, .92f), new Vector2(0, 1), new Vector2(0, 1));
        Image coin = Icon(bar.transform, "CoinIcon", new Vector2(34, 0), new Vector2(38, 38), CoinSprite());
        coin.color = Color.white;
        goldText = Text(bar.transform, "0", new Vector2(82, 0), new Vector2(110, 60), 24, new Color(1f, .73f, .24f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        gateText = Text(bar.transform, "GATE", new Vector2(220, 0), new Vector2(190, 60), 20, new Color(.94f, .84f, .67f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        aliveText = Text(bar.transform, "ALIVE", new Vector2(425, 0), new Vector2(200, 60), 20, new Color(.94f, .84f, .67f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
    }

    void BuildActionPanel(Transform parent)
    {
        GameObject panel = Panel(parent, "CombatActions", new Vector2(-24, -24), new Vector2(430, 190), new Color(.035f, .022f, .016f, .92f), new Vector2(1, 1), new Vector2(1, 1));
        Button(panel.transform, "-", new Vector2(-178, -32), new Vector2(54, 54), DecreaseSpeed, false);
        Button(panel.transform, "+", new Vector2(-58, -32), new Vector2(54, 54), IncreaseSpeed, false);
        speedText = Text(panel.transform, "1x", new Vector2(-118, -32), new Vector2(82, 54), 18, new Color(1f, .78f, .34f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        magicButton = Button(panel.transform, "", new Vector2(110, -32), new Vector2(180, 54), () => GameManager.Instance?.UseMagic(), true);
        magicText = magicButton.GetComponentInChildren<Text>();
        giftButton = Button(panel.transform, "", new Vector2(110, -100), new Vector2(300, 50), () => GameManager.Instance?.UseGift(), false);
        giftText = giftButton.GetComponentInChildren<Text>();
    }

    void BuildWaveBar(Transform parent)
    {
        GameObject bar = Panel(parent, "WaveStatus", new Vector2(0, -24), new Vector2(860, 142), new Color(.035f, .022f, .016f, .95f), new Vector2(.5f, 1), new Vector2(.5f, 1));
        waveText = Text(bar.transform, "WAVE", new Vector2(-90, 48), new Vector2(620, 34), 22, new Color(1f, .75f, .32f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        threatText = Text(bar.transform, "THREAT", new Vector2(-90, 12), new Vector2(620, 30), 15, new Color(.86f, .76f, .64f, 1f), TextAnchor.MiddleCenter, FontStyle.Normal);
        wavePreviewText = Text(bar.transform, "", new Vector2(-90, -28), new Vector2(620, 34), 14, new Color(.93f, .82f, .67f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        startWaveButton = Button(bar.transform, L("START WAVE", "НАЧАТЬ ВОЛНУ"), new Vector2(330, 2), new Vector2(170, 82), StartWave, true);
    }

    void BuildDock(Transform parent)
    {
        GameObject dock = Panel(parent, "BuildDock", new Vector2(0, 24), new Vector2(1120, 154), new Color(.035f, .022f, .016f, .95f), new Vector2(.5f, 0), new Vector2(.5f, 0));
        Text(dock.transform, L("TROJAN DEFENSES", "ОБОРОНА ТРОИ"), new Vector2(-480, 54), new Vector2(260, 28), 14, new Color(.74f, .65f, .55f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        buildSelectionText = Text(dock.transform, "", new Vector2(390, 54), new Vector2(620, 28), 14, new Color(1f, .72f, .28f, 1f), TextAnchor.MiddleRight, FontStyle.Bold);

        for (int i = 0; i < buildTypes.Length; i++)
        {
            TowerType type = buildTypes[i];
            float x = -430 + i * 174;
            buildButtons[i] = Button(dock.transform, BuildLabel(type, buildHotkeys[i]), new Vector2(x, -18), new Vector2(158, 82), () => SelectBuild(type), false);
            BuildButtonHoverRelay relay = buildButtons[i].gameObject.AddComponent<BuildButtonHoverRelay>();
            relay.Initialize(type, () => ShowBuildTooltip(type), HideBuildTooltip);
        }
    }

    void BuildBuildTooltip(Transform parent)
    {
        buildTooltip = Panel(parent, "BuildHoverTooltip", new Vector2(0, 344), new Vector2(520, 174), new Color(.028f, .018f, .014f, .985f), new Vector2(.5f, 0), new Vector2(.5f, 0));
        buildTooltip.GetComponent<Image>().raycastTarget = false;

        GameObject accentObject = new GameObject("Accent");
        accentObject.transform.SetParent(buildTooltip.transform, false);
        tooltipAccent = accentObject.AddComponent<Image>();
        tooltipAccent.raycastTarget = false;
        RectTransform accentRt = tooltipAccent.rectTransform;
        accentRt.anchorMin = accentRt.anchorMax = accentRt.pivot = new Vector2(0f, .5f);
        accentRt.anchoredPosition = new Vector2(8f, 0f);
        accentRt.sizeDelta = new Vector2(8f, 154f);

        tooltipTitle = Text(buildTooltip.transform, "", new Vector2(-116, 55), new Vector2(300, 30), 20, new Color(1f, .76f, .31f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        tooltipRole = Text(buildTooltip.transform, "", new Vector2(-116, 26), new Vector2(300, 28), 13, new Color(.78f, .70f, .61f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        tooltipStats = Text(buildTooltip.transform, "", new Vector2(-116, -10), new Vector2(300, 38), 13, new Color(.93f, .87f, .79f, 1f), TextAnchor.MiddleLeft, FontStyle.Normal);
        tooltipMatchup = Text(buildTooltip.transform, "", new Vector2(126, -2), new Vector2(220, 112), 13, new Color(.78f, .91f, .70f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        buildTooltip.SetActive(false);
    }

    void BuildSelectedCard(Transform parent)
    {
        GameObject card = Panel(parent, "SelectedTowerCard", new Vector2(-24, -230), new Vector2(450, 430), new Color(.045f, .027f, .018f, .97f), new Vector2(1, 1), new Vector2(1, 1));
        selectedTitle = Text(card.transform, L("SELECT A DEFENSE", "ВЫБЕРИТЕ ОБОРОНУ"), new Vector2(24, -22), new Vector2(400, 46), 24, new Color(1f, .70f, .28f, 1f), TextAnchor.UpperLeft, FontStyle.Bold);
        selectedStats = Text(card.transform, "", new Vector2(24, -82), new Vector2(400, 138), 17, new Color(.94f, .87f, .77f, 1f), TextAnchor.UpperLeft, FontStyle.Normal);
        selectedUpgradePreview = Text(card.transform, "", new Vector2(24, -214), new Vector2(400, 68), 15, new Color(1f, .73f, .31f, 1f), TextAnchor.UpperLeft, FontStyle.Bold);
        selectedPriority = Text(card.transform, "", new Vector2(24, -278), new Vector2(400, 34), 15, new Color(.82f, .72f, .62f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        upgradeButton = Button(card.transform, L("UPGRADE", "УЛУЧШИТЬ"), new Vector2(-110, -342), new Vector2(190, 58), () => placement?.UpgradeSelected(), true);
        sellButton = Button(card.transform, L("SELL", "ПРОДАТЬ"), new Vector2(110, -342), new Vector2(190, 58), () => placement?.SellSelected(), false);
        priorityButton = Button(card.transform, L("TARGET PRIORITY", "ПРИОРИТЕТ ЦЕЛИ"), new Vector2(0, -397), new Vector2(410, 44), () => placement?.CycleSelectedPriority(), false);
    }

    void BuildPcHints(Transform parent)
    {
        GameObject hint = Panel(parent, "PcHints", new Vector2(24, 24), new Vector2(650, 48), new Color(.03f, .02f, .015f, .82f), new Vector2(0, 0), new Vector2(0, 0));
        Text(hint.transform, L("LMB SELECT/BUILD   •   RMB HECTOR MOVE   •   Q E R F ABILITIES   •   ESC PAUSE", "ЛКМ ВЫБОР/СТРОЙКА   •   ПКМ ДВИЖЕНИЕ ГЕКТОРА   •   Q E R F СПОСОБНОСТИ   •   ESC ПАУЗА"), new Vector2(14, 0), new Vector2(620, 42), 13, new Color(.77f, .69f, .60f, 1f), TextAnchor.MiddleLeft, FontStyle.Normal);
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
        gateText.text = $"{L("GATE", "ВОРОТА")}   {gm.BaseHealth}/{gm.MaxBaseHealth}";
        aliveText.text = $"{L("ENEMIES", "ВРАГИ")}   {EnemyRegistry.AliveCount}";
    }

    void UpdateActions()
    {
        GameManager gm = GameManager.Instance;
        float speed = CombatControlsUI.CurrentSpeed;
        speedText.text = $"{speed:0}x";

        float cooldown = gm != null ? gm.MagicCooldownRemaining : 0f;
        magicText.text = cooldown > 0f
            ? L("MAGIC ", "МАГИЯ ") + Mathf.CeilToInt(cooldown) + L("s", "с")
            : L("MAGIC READY", "МАГИЯ ГОТОВА");
        magicButton.interactable = gm != null && cooldown <= 0f && !gm.GameEnded;

        giftText.text = gm != null && gm.GiftAvailable
            ? L("GIFT +100 / +2", "ДАР +100 / +2")
            : L("GIFT USED", "ДАР ИСПОЛЬЗОВАН");
        giftButton.interactable = gm != null && gm.GiftAvailable && !gm.GameEnded;
    }

    void UpdateWave()
    {
        GameManager gm = GameManager.Instance;
        int sec = spawner != null ? Mathf.RoundToInt(spawner.CurrentWaveElapsed) : 0;
        waveText.text = $"{L("CHAPTER I", "ГЛАВА I")}   •   {L("WAVE", "ВОЛНА")} {gm.CurrentWave}/{gm.MaxWaves}   •   {sec / 60:00}:{sec % 60:00}";

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
            threatText.text = $"{L("WAVE ACTIVE", "ВОЛНА ИДЁТ")}   •   {EnemyRegistry.AliveCount} {L("alive", "в строю")}   •   {threat}";
        else if (spawner.InterWaveCountdown > 0)
            threatText.text = $"{L("NEXT WAVE IN", "СЛЕДУЮЩАЯ ВОЛНА ЧЕРЕЗ")} {Mathf.CeilToInt(spawner.InterWaveCountdown)}{L("s", "с")}   •   {spawner.NextWaveEnemyCount} {L("enemies", "врагов")}";
        else
            threatText.text = $"{L("READY", "ГОТОВО")}   •   {spawner.NextWaveEnemyCount} {L("enemies", "врагов")}   •   {threat}";

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
        if (placement == null) return;
        int cost = TowerFactory.GetCost(placement.SelectedBuildType);
        TowerType recommended = RecommendedDefense();
        buildSelectionText.text = $"{L("SELECTED", "ВЫБРАНО")}: {TowerName(placement.SelectedBuildType)}   •   {cost} {L("GOLD", "ЗОЛОТА")}   •   {L("RECOMMENDED", "РЕКОМЕНДАЦИЯ")}: {TowerName(recommended)}";

        for (int i = 0; i < buildButtons.Length; i++)
        {
            if (buildButtons[i] == null) continue;
            Image image = buildButtons[i].GetComponent<Image>();
            bool selected = buildTypes[i] == placement.SelectedBuildType;
            bool recommendedButton = buildTypes[i] == recommended;
            bool affordable = GameManager.Instance != null && GameManager.Instance.Money >= TowerFactory.GetCost(buildTypes[i]);

            if (selected) image.color = new Color(.58f, .11f, .045f, .98f);
            else if (recommendedButton && affordable) image.color = new Color(.48f, .30f, .07f, .98f);
            else if (recommendedButton) image.color = new Color(.29f, .20f, .08f, .92f);
            else image.color = affordable ? new Color(.24f, .14f, .08f, .96f) : new Color(.11f, .085f, .07f, .88f);
        }
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
        tooltipTitle.text = TowerDisplayName(hoveredBuildType) + (recommended ? "   •   " + L("RECOMMENDED", "РЕКОМЕНДУЕТСЯ") : "");
        tooltipRole.text = TowerRole(hoveredBuildType);
        float dps = data.damage * Mathf.Max(.01f, data.attacksPerSecond);
        tooltipStats.text = $"{L("DMG", "УРОН")} {data.damage:0.#}   •   DPS {dps:0.#}   •   {L("RNG", "ДАЛЬН")} {data.range:0.0}   •   {data.cost} {L("GOLD", "ЗОЛОТА")}";
        tooltipMatchup.text = $"+ {L("STRONG", "СИЛЁН")}\n{StrongAgainst(hoveredBuildType)}\n\n− {L("WEAK", "СЛАБ")}\n{WeakAgainst(hoveredBuildType)}";
    }

    void UpdateSelected()
    {
        Tower tower = placement != null ? placement.SelectedTower : null;
        bool has = tower != null;
        upgradeButton.gameObject.SetActive(has);
        sellButton.gameObject.SetActive(has);
        priorityButton.gameObject.SetActive(has && tower.Type != TowerType.TrojanGuard);

        if (!has)
        {
            selectedTitle.text = L("SELECT A TOWER-UNIT", "ВЫБЕРИТЕ ОБОРОНУ");
            selectedStats.text = L("Click a deployed Trojan defense to inspect stats, upgrade it, sell it, or change targeting priority.", "Нажмите на установленную оборону Трои, чтобы увидеть характеристики, улучшить, продать или изменить приоритет цели.");
            selectedUpgradePreview.text = "";
            selectedPriority.text = "";
            return;
        }

        selectedTitle.text = $"{tower.DisplayName}   LV {tower.Level}/3";
        selectedStats.text = $"{L("DAMAGE", "УРОН")}     {tower.damage:0}\n{L("RANGE", "ДАЛЬНОСТЬ")}       {tower.range:0.0}\n{L("ATTACK RATE", "СКОРОСТЬ АТАКИ")}   {tower.fireRate:0.00}/s\n{L("SELL VALUE", "ЦЕНА ПРОДАЖИ")}      {tower.SellValue}";

        if (tower.Level >= 3)
        {
            selectedUpgradePreview.text = L("MAXIMUM LEVEL REACHED", "ДОСТИГНУТ МАКСИМАЛЬНЫЙ УРОВЕНЬ");
        }
        else
        {
            float nextDamage = tower.damage * 1.32f;
            float nextRange = tower.range * 1.10f;
            float nextRate = tower.fireRate * 1.12f;
            selectedUpgradePreview.text = $"{L("NEXT UPGRADE", "СЛЕДУЮЩЕЕ УЛУЧШЕНИЕ")}   {tower.UpgradeCost} {L("GOLD", "ЗОЛОТА")}\n{tower.damage:0} → {nextDamage:0} DMG   •   {tower.range:0.0} → {nextRange:0.0} RNG   •   {tower.fireRate:0.00} → {nextRate:0.00}/s";
        }

        selectedPriority.text = tower.Type == TowerType.TrojanGuard
            ? L("ROLE: BLOCKING SQUAD", "РОЛЬ: БЛОКИРУЮЩИЙ ОТРЯД")
            : $"{L("TARGETING", "ПРИОРИТЕТ")}: {tower.Priority}";

        upgradeButton.interactable = tower.Level < 3 && GameManager.Instance.Money >= tower.UpgradeCost;
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
