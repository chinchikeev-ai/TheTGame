using UnityEngine;
using UnityEngine.UI;

public sealed class ModernCombatHud : MonoBehaviour
{
    Canvas canvas;
    CanvasGroup group;
    TowerPlacement placement;
    EnemySpawner spawner;
    Canvas legacyCanvas;
    Canvas menuCanvas;

    Text goldText, gateText, aliveText, waveText, threatText, wavePreviewText;
    Text selectedTitle, selectedStats, selectedPriority, selectedUpgradePreview;
    Button upgradeButton, sellButton, priorityButton, startWaveButton;
    Text buildSelectionText;

    GameObject buildTooltip;
    Image tooltipAccent;
    Text tooltipTitle, tooltipRole, tooltipStats, tooltipTags, tooltipMatchup;
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
        BuildDock(root.transform);
        BuildBuildTooltip(root.transform);
        BuildSelectedCard(root.transform);
        BuildPcHints(root.transform);
    }

    void BuildTopBar(Transform parent)
    {
        GameObject bar = Panel(parent, "TopResources", new Vector2(24, -24), new Vector2(660, 74), new Color(.035f, .022f, .016f, .92f), new Vector2(0, 1), new Vector2(0, 1));
        goldText = Text(bar.transform, "GOLD", new Vector2(22, 0), new Vector2(190, 60), 22, new Color(1f, .73f, .24f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        gateText = Text(bar.transform, "GATE", new Vector2(220, 0), new Vector2(190, 60), 20, new Color(.94f, .84f, .67f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        aliveText = Text(bar.transform, "ALIVE", new Vector2(425, 0), new Vector2(200, 60), 20, new Color(.94f, .84f, .67f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
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
            relay.Initialize(() => ShowBuildTooltip(type), HideBuildTooltip);
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

        BuildTooltipSilhouette(buildTooltip.transform);
        tooltipTitle = Text(buildTooltip.transform, "", new Vector2(-116, 55), new Vector2(300, 30), 20, new Color(1f, .76f, .31f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        tooltipRole = Text(buildTooltip.transform, "", new Vector2(-116, 26), new Vector2(300, 28), 13, new Color(.78f, .70f, .61f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        tooltipStats = Text(buildTooltip.transform, "", new Vector2(-116, -10), new Vector2(300, 38), 13, new Color(.93f, .87f, .79f, 1f), TextAnchor.MiddleLeft, FontStyle.Normal);
        tooltipTags = Text(buildTooltip.transform, "", new Vector2(-116, -47), new Vector2(300, 28), 13, new Color(.96f, .63f, .24f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        tooltipMatchup = Text(buildTooltip.transform, "", new Vector2(126, -2), new Vector2(220, 112), 13, new Color(.78f, .91f, .70f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        buildTooltip.SetActive(false);
    }

    void BuildTooltipSilhouette(Transform parent)
    {
        GameObject head = new GameObject("SilhouetteHead");
        head.transform.SetParent(parent, false);
        Image headImage = head.AddComponent<Image>();
        headImage.color = new Color(.74f, .42f, .14f, .92f);
        headImage.raycastTarget = false;
        RectTransform headRt = headImage.rectTransform;
        headRt.anchorMin = headRt.anchorMax = headRt.pivot = new Vector2(.5f, .5f);
        headRt.anchoredPosition = new Vector2(-214, 36);
        headRt.sizeDelta = new Vector2(34, 34);

        GameObject body = new GameObject("SilhouetteBody");
        body.transform.SetParent(parent, false);
        Image bodyImage = body.AddComponent<Image>();
        bodyImage.color = new Color(.56f, .25f, .09f, .92f);
        bodyImage.raycastTarget = false;
        RectTransform bodyRt = bodyImage.rectTransform;
        bodyRt.anchorMin = bodyRt.anchorMax = bodyRt.pivot = new Vector2(.5f, .5f);
        bodyRt.anchoredPosition = new Vector2(-214, -20);
        bodyRt.sizeDelta = new Vector2(54, 74);

        GameObject weapon = new GameObject("SilhouetteWeapon");
        weapon.transform.SetParent(parent, false);
        Image weaponImage = weapon.AddComponent<Image>();
        weaponImage.color = new Color(.91f, .68f, .25f, .92f);
        weaponImage.raycastTarget = false;
        RectTransform weaponRt = weaponImage.rectTransform;
        weaponRt.anchorMin = weaponRt.anchorMax = weaponRt.pivot = new Vector2(.5f, .5f);
        weaponRt.anchoredPosition = new Vector2(-182, -2);
        weaponRt.sizeDelta = new Vector2(10, 104);
        weaponRt.localRotation = Quaternion.Euler(0f, 0f, -18f);
    }

    void BuildSelectedCard(Transform parent)
    {
        GameObject card = Panel(parent, "SelectedTowerCard", new Vector2(-24, -182), new Vector2(450, 430), new Color(.045f, .027f, .018f, .97f), new Vector2(1, 1), new Vector2(1, 1));
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
        UpdateBuildDock();
        UpdateSelected();
        if (buildTooltipVisible) RefreshBuildTooltip();
        HandlePcHotkeys();
    }

    bool IsMenuBlockingCombat()
    {
        if (menuCanvas == null) return false;
        string[] names = { "MainMenu", "LevelSelect", "Settings", "PauseMenu", "EndMenu", "ConfirmationModal" };
        foreach (string n in names)
        {
            Transform t = menuCanvas.transform.Find(n);
            if (t != null && t.gameObject.activeInHierarchy) return true;
        }
        return false;
    }

    void UpdateResources()
    {
        GameManager gm = GameManager.Instance;
        goldText.text = $"{L("GOLD", "ЗОЛОТО")}   {gm.Money}";
        gateText.text = $"{L("GATE", "ВОРОТА")}   {gm.BaseHealth}/{gm.MaxBaseHealth}";
        aliveText.text = $"{L("ENEMIES", "ВРАГИ")}   {EnemyRegistry.AliveCount}";
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

        wavePreviewText.text = BuildWavePreview();
        bool canStart = spawner.WaitingForManualStart && !spawner.WaveActive && !gm.GameEnded;
        startWaveButton.gameObject.SetActive(canStart);
        startWaveButton.interactable = canStart;
    }

    string BuildWavePreview()
    {
        if (spawner == null) return "";
        string preview = L("NEXT: ", "ДАЛЕЕ: ");
        bool any = false;
        any |= AppendWavePart(ref preview, L("INF", "ПЕХ"), spawner.NextWaveInfantryCount, any);
        any |= AppendWavePart(ref preview, L("RUN", "БЕГ"), spawner.NextWaveRunnerCount, any);
        any |= AppendWavePart(ref preview, L("HEAVY", "ТЯЖ"), spawner.NextWaveHeavyCount, any);
        any |= AppendWavePart(ref preview, L("SHIELD", "ЩИТ"), spawner.NextWaveShieldCount, any);
        any |= AppendWavePart(ref preview, L("ARCHER", "ЛУК"), spawner.NextWaveArcherCount, any);
        any |= AppendWavePart(ref preview, L("MENELAUS", "МЕНЕЛАЙ"), spawner.NextWaveBossCount, any);
        return any ? preview : L("NEXT WAVE DATA PREPARING", "ПОДГОТОВКА ДАННЫХ ВОЛНЫ");
    }

    bool AppendWavePart(ref string text, string label, int count, bool alreadyHas)
    {
        if (count <= 0) return false;
        if (alreadyHas) text += "   •   ";
        text += $"{label} ×{count}";
        return true;
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

    TowerType RecommendedDefense()
    {
        if (spawner == null) return TowerType.MachineGun;

        float best = float.MinValue;
        TowerType winner = TowerType.MachineGun;
        for (int i = 0; i < buildTypes.Length; i++)
        {
            TowerType type = buildTypes[i];
            float score = RecommendationScore(type);
            if (score > best)
            {
                best = score;
                winner = type;
            }
        }
        return winner;
    }

    float RecommendationScore(TowerType type)
    {
        if (spawner == null) return 0f;
        float inf = spawner.NextWaveInfantryCount;
        float run = spawner.NextWaveRunnerCount;
        float heavy = spawner.NextWaveHeavyCount;
        float shield = spawner.NextWaveShieldCount;
        float arch = spawner.NextWaveArcherCount;
        float boss = spawner.NextWaveBossCount;
        float total = Mathf.Max(1f, inf + run + heavy + shield + arch + boss);

        switch (type)
        {
            case TowerType.MachineGun: return inf * 1.35f + run * 2.1f + arch * 1.1f;
            case TowerType.SpearThrower: return heavy * 2.35f + shield * 2.0f + boss * 1.25f;
            case TowerType.Cannon: return boss * 4.2f + heavy * 1.65f + shield * 1.25f;
            case TowerType.Slow: return run * 1.75f + total * .28f;
            case TowerType.FireTower: return inf * 1.55f + run * 1.25f + arch * 1.0f + total * .22f;
            case TowerType.TrojanGuard: return heavy * .9f + shield * .7f + total * .18f;
            default: return 0f;
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
        tooltipTags.text = TowerTags(hoveredBuildType);
        tooltipMatchup.text = $"+ {L("STRONG", "СИЛЁН")}\n{StrongAgainst(hoveredBuildType)}\n\n− {L("WEAK", "СЛАБ")}\n{WeakAgainst(hoveredBuildType)}";
    }

    string TowerDisplayName(TowerType type)
    {
        switch (type)
        {
            case TowerType.MachineGun: return L("TROJAN ARCHERS", "ТРОЯНСКИЕ ЛУЧНИКИ");
            case TowerType.SpearThrower: return L("SPEAR THROWERS", "МЕТАТЕЛИ КОПИЙ");
            case TowerType.Cannon: return L("BALLISTA CREW", "РАСЧЁТ БАЛЛИСТЫ");
            case TowerType.Slow: return L("PRIESTS OF APOLLO", "ЖРЕЦЫ АПОЛЛОНА");
            case TowerType.FireTower: return L("FIRE CREW", "ОГНЕННЫЙ РАСЧЁТ");
            case TowerType.TrojanGuard: return L("SHIELD GUARD", "ЩИТОВАЯ ГВАРДИЯ");
            default: return type.ToString().ToUpperInvariant();
        }
    }

    string TowerRole(TowerType type)
    {
        switch (type)
        {
            case TowerType.MachineGun: return L("RANGED DPS • ANTI-LIGHT", "ДАЛЬНИЙ БОЙ • ПРОТИВ ЛЁГКИХ");
            case TowerType.SpearThrower: return L("ARMOR PIERCE • ANTI-HEAVY", "БРОНЕБОЙНЫЙ • ПРОТИВ ТЯЖЁЛЫХ");
            case TowerType.Cannon: return L("HEAVY SINGLE TARGET • ANTI-SIEGE", "ТЯЖЁЛЫЙ УРОН • ПРОТИВ ОСАДЫ");
            case TowerType.Slow: return L("SUPPORT • CONTROL", "ПОДДЕРЖКА • КОНТРОЛЬ");
            case TowerType.FireTower: return L("AOE • BURN • AREA DENIAL", "AOE • ГОРЕНИЕ • КОНТРОЛЬ ЗОНЫ");
            case TowerType.TrojanGuard: return L("BLOCKER • FRONTLINE", "БЛОКИРОВКА • ПЕРЕДОВАЯ");
            default: return "";
        }
    }

    string TowerTags(TowerType type)
    {
        switch (type)
        {
            case TowerType.MachineGun: return "[RANGED]   [LIGHT]";
            case TowerType.SpearThrower: return "[ARMOR]   [HEAVY]";
            case TowerType.Cannon: return "[SIEGE]   [BOSS]   [PIERCE]";
            case TowerType.Slow: return "[SLOW]   [SUPPORT]";
            case TowerType.FireTower: return "[AOE]   [BURN]   [ZONE]";
            case TowerType.TrojanGuard: return "[BLOCK]   [FRONTLINE]";
            default: return "";
        }
    }

    string StrongAgainst(TowerType type)
    {
        switch (type)
        {
            case TowerType.MachineGun: return L("Runners • Infantry", "Бегуны • Пехота");
            case TowerType.SpearThrower: return L("Heavy • Shields", "Тяжёлые • Щитоносцы");
            case TowerType.Cannon: return L("Boss • Siege • Heavy", "Босс • Осада • Тяжёлые");
            case TowerType.Slow: return L("Fast groups", "Быстрые группы");
            case TowerType.FireTower: return L("Dense groups", "Плотные группы");
            case TowerType.TrojanGuard: return L("Holding lanes", "Удержание линии");
            default: return "—";
        }
    }

    string WeakAgainst(TowerType type)
    {
        switch (type)
        {
            case TowerType.MachineGun: return L("Heavy armor", "Тяжёлая броня");
            case TowerType.SpearThrower: return L("Light swarms", "Толпы лёгких");
            case TowerType.Cannon: return L("Fast swarms", "Быстрые толпы");
            case TowerType.Slow: return L("Damage races", "Чистый урон");
            case TowerType.FireTower: return L("Single heavy", "Одиночные тяжёлые");
            case TowerType.TrojanGuard: return L("Ranged pressure", "Дальний обстрел");
            default: return "—";
        }
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

    string BuildLabel(TowerType type, string key) => $"[{key}]  {TowerName(type)}\n{TowerFactory.GetCost(type)} {L("GOLD", "ЗОЛОТА")}";

    string TowerName(TowerType type)
    {
        switch (type)
        {
            case TowerType.SpearThrower: return L("SPEAR", "КОПЬЯ");
            case TowerType.MachineGun: return L("ARCHERS", "ЛУЧНИКИ");
            case TowerType.Cannon: return L("BALLISTA", "БАЛЛИСТА");
            case TowerType.Slow: return L("APOLLO", "АПОЛЛОН");
            case TowerType.FireTower: return L("FIRE", "ОГОНЬ");
            case TowerType.TrojanGuard: return L("GUARD", "СТРАЖА");
            default: return type.ToString();
        }
    }

    GameObject Panel(Transform parent, string name, Vector2 pos, Vector2 size, Color color, Vector2 anchor, Vector2 pivot)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = color;
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(.67f, .36f, .13f, .42f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);
        return go;
    }

    Button Button(Transform parent, string label, Vector2 pos, Vector2 size, UnityEngine.Events.UnityAction action, bool primary)
    {
        GameObject go = new GameObject(label);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = primary ? new Color(.55f, .11f, .045f, .98f) : new Color(.24f, .14f, .08f, .96f);
        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        go.AddComponent<MenuButtonFeedback>();
        go.AddComponent<MenuUiAudioFeedback>();
        Text(go.transform, label, Vector2.zero, size, 15, primary ? new Color(1f, .88f, .50f, 1f) : new Color(.94f, .84f, .70f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        return button;
    }

    Text Text(Transform parent, string value, Vector2 pos, Vector2 size, int fontSize, Color color, TextAnchor alignment, FontStyle style)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent, false);
        Text t = go.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.text = value;
        t.fontSize = fontSize;
        t.fontStyle = style;
        t.color = color;
        t.alignment = alignment;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Truncate;
        RectTransform rt = t.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return t;
    }
}
