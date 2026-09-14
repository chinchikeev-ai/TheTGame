using UnityEngine;
using UnityEngine.UI;

public class HectorHUD : MonoBehaviour
{
    static readonly string[] BlockingMenuNames =
    {
        "MainMenu", "LevelSelect", "Settings", "PauseMenu", "EndMenu"
    };

    readonly string[] abilityKeys = { "Q", "E", "R", "F" };
    readonly string[] abilityArt = { "warcry", "shieldwall", "spear", "ultimate" };

    GameObject root;
    Text nameText;
    Text hpText;
    Text commandText;
    Text commentaryText;
    Image hpFill;
    readonly Text[] abilityTexts = new Text[4];
    readonly Image[] cooldownFills = new Image[4];
    readonly Button[] abilityButtons = new Button[4];
    Canvas menuCanvas;
    EnemySpawner spawner;

    bool stateInitialized;
    bool lastDowned;
    bool lastWaveActive;
    bool lastBossDefeated;
    int lastGateHealth;
    int lastWave;
    int lastBossWarningWave = -1;
    int lastClosingWave = -1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<HectorHUD>() == null)
            new GameObject("HectorHUD").AddComponent<HectorHUD>();
    }

    void Start()
    {
        GameObject canvasObj = new GameObject("HectorHUDCanvas");
        canvasObj.transform.SetParent(transform, false);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 83;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;

        root = new GameObject("HectorPanel");
        root.transform.SetParent(canvasObj.transform, false);
        Image panel = root.AddComponent<Image>();
        panel.sprite = TroyHudArt.Panel();
        panel.type = Image.Type.Sliced;
        panel.color = Color.white;
        RectTransform pr = panel.rectTransform;
        pr.anchorMin = pr.anchorMax = pr.pivot = new Vector2(0f, 0f);
        pr.anchoredPosition = new Vector2(18f, 18f);
        pr.sizeDelta = new Vector2(488f, 266f);

        AddPanel(root.transform, "HectorPortraitFrame", new Vector2(-190, 70), new Vector2(128, 128), new Color(.22f, .12f, .055f, 1f));
        Image portrait = AddImage(root.transform, "HectorPortrait", new Vector2(-190, 70), new Vector2(104, 104), TroyHudArt.Portrait("hector"));
        portrait.raycastTarget = true;
        Button portraitButton = portrait.gameObject.AddComponent<Button>();
        portraitButton.targetGraphic = portrait;
        portraitButton.onClick.AddListener(SelectHectorFromHud);

        AddPanel(root.transform, "HectorNameBanner", new Vector2(42, 94), new Vector2(304, 48), new Color(.33f, .070f, .035f, 1f));
        nameText = AddText(root.transform, "HECTOR", new Vector2(42, 103), new Vector2(278, 28), 21, new Color(1f, .86f, .50f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        hpText = AddText(root.transform, "", new Vector2(42, 78), new Vector2(278, 22), 13, new Color(.94f, .86f, .72f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);

        AddPanel(root.transform, "HectorHealthFrame", new Vector2(42, 48), new Vector2(314, 28), new Color(.10f, .045f, .025f, 1f));
        GameObject track = new GameObject("HealthTrack");
        track.transform.SetParent(root.transform, false);
        Image trackImage = track.AddComponent<Image>();
        trackImage.color = new Color(.15f, .06f, .035f, .95f);
        RectTransform tr = trackImage.rectTransform;
        tr.anchorMin = tr.anchorMax = tr.pivot = new Vector2(.5f, .5f);
        tr.anchoredPosition = new Vector2(42, 48);
        tr.sizeDelta = new Vector2(286, 14);

        GameObject fillObj = new GameObject("HealthFill");
        fillObj.transform.SetParent(track.transform, false);
        hpFill = fillObj.AddComponent<Image>();
        hpFill.color = new Color(.72f, .16f, .06f, 1f);
        hpFill.type = Image.Type.Filled;
        hpFill.fillMethod = Image.FillMethod.Horizontal;
        RectTransform fr = hpFill.rectTransform;
        fr.anchorMin = Vector2.zero;
        fr.anchorMax = Vector2.one;
        fr.offsetMin = new Vector2(2, 2);
        fr.offsetMax = new Vector2(-2, -2);

        commandText = AddText(root.transform, "", new Vector2(42, 25), new Vector2(310, 20), 10, new Color(1f, .76f, .28f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        AddPanel(root.transform, "HectorQuoteParchment", new Vector2(0, -22), new Vector2(436, 48), new Color(.20f, .13f, .075f, .96f));
        commentaryText = AddText(root.transform, "", new Vector2(0, -22), new Vector2(402, 34), 12, new Color(.98f, .90f, .76f, 1f), TextAnchor.MiddleLeft, FontStyle.Italic);
        commentaryText.gameObject.name = "HectorCommentary";

        AddPanel(root.transform, "HectorAbilityRail", new Vector2(0, -91), new Vector2(436, 78), new Color(.085f, .050f, .030f, .98f));
        for (int i = 0; i < 4; i++)
        {
            float x = -153 + i * 102;
            GameObject slot = new GameObject("Ability_" + abilityKeys[i]);
            slot.transform.SetParent(root.transform, false);
            Image bg = slot.AddComponent<Image>();
            bg.sprite = TroyHudArt.Panel();
            bg.type = Image.Type.Sliced;
            bg.color = new Color(.36f, .18f, .070f, 1f);
            RectTransform sr = bg.rectTransform;
            sr.anchorMin = sr.anchorMax = sr.pivot = new Vector2(.5f, .5f);
            sr.anchoredPosition = new Vector2(x, -91);
            sr.sizeDelta = new Vector2(88, 72);

            int abilityIndex = i;
            Button button = slot.AddComponent<Button>();
            button.targetGraphic = bg;
            button.onClick.AddListener(() => UseAbilityFromHud(abilityIndex));
            abilityButtons[i] = button;

            AddText(slot.transform, abilityKeys[i], new Vector2(-30, 24), new Vector2(20, 18), 10, new Color(1f, .82f, .36f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
            AddImage(slot.transform, "Icon", new Vector2(0, 10), new Vector2(38, 38), TroyHudArt.Ability(abilityArt[i]));
            abilityTexts[i] = AddText(slot.transform, "", new Vector2(0, -25), new Vector2(82, 22), 8, new Color(.96f, .86f, .70f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);

            GameObject cd = new GameObject("Cooldown");
            cd.transform.SetParent(slot.transform, false);
            cooldownFills[i] = cd.AddComponent<Image>();
            cooldownFills[i].color = new Color(.02f, .015f, .01f, .72f);
            cooldownFills[i].type = Image.Type.Filled;
            cooldownFills[i].fillMethod = Image.FillMethod.Radial360;
            cooldownFills[i].fillOrigin = 2;
            cooldownFills[i].fillClockwise = false;
            cooldownFills[i].raycastTarget = false;
            RectTransform cr = cooldownFills[i].rectTransform;
            cr.anchorMin = Vector2.zero;
            cr.anchorMax = Vector2.one;
            cr.offsetMin = Vector2.zero;
            cr.offsetMax = Vector2.zero;
            cooldownFills[i].transform.SetAsFirstSibling();
        }

        root.SetActive(false);
    }

    void Update()
    {
        HectorController h = HectorController.Instance;
        GameManager gm = GameManager.Instance;
        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();

        bool hidden = h == null || gm == null || gm.GameEnded || IsMenuBlockingCombat();
        if (hidden || root == null)
        {
            if (root != null) root.SetActive(false);
            stateInitialized = false;
            return;
        }

        root.SetActive(true);
        UpdateStatus(h);
        UpdateAbilities(h);
        UpdateCommentary(h, gm);
    }

    void UpdateStatus(HectorController h)
    {
        if (h.IsDowned)
        {
            nameText.text = GameLanguage.T("HECTOR DOWNED", "ГЕКТОР ПОВЕРЖЕН");
            hpText.text = GameLanguage.T("REVIVE ", "ВОЗВРАЩЕНИЕ ") + Mathf.CeilToInt(h.DownedRemaining) + GameLanguage.T("s", "с");
            hpFill.fillAmount = 0f;
            commandText.text = GameLanguage.T("UNAVAILABLE • REVIVING", "НЕДОСТУПЕН • ВОЗВРАЩАЕТСЯ");
            commandText.color = new Color(.74f, .48f, .34f, 1f);
            return;
        }

        nameText.text = GameLanguage.T("HECTOR • PRINCE OF TROY", "ГЕКТОР • ПРИНЦ ТРОИ");
        hpText.text = $"HP {Mathf.CeilToInt(h.Health)} / {Mathf.CeilToInt(h.maxHealth)}";
        hpFill.fillAmount = h.maxHealth > 0f ? Mathf.Clamp01(h.Health / h.maxHealth) : 0f;
        commandText.text = h.Selected
            ? GameLanguage.T("SELECTED • RMB MOVE • Q/E/R/F", "ВЫБРАН • ПКМ ДВИЖЕНИЕ • Q/E/R/F")
            : GameLanguage.T("CLICK PORTRAIT TO COMMAND", "НАЖМИТЕ ПОРТРЕТ ДЛЯ УПРАВЛЕНИЯ");
        commandText.color = h.Selected ? new Color(1f, .84f, .36f, 1f) : new Color(.74f, .67f, .58f, 1f);
    }

    void UpdateAbilities(HectorController h)
    {
        string[] names =
        {
            GameLanguage.T("WAR CRY", "КЛИЧ"),
            GameLanguage.T("SHIELD", "ЩИТЫ"),
            GameLanguage.T("SPEAR", "КОПЬЁ"),
            GameLanguage.T("FOR TROY!", "ЗА ТРОЮ!")
        };
        float[] remain = { h.WarCryCooldownRemaining, h.ShieldWallCooldownRemaining, h.SpearThrowCooldownRemaining, h.UltimateCooldownRemaining };
        float[] total = { h.warCryCooldown, h.shieldWallCooldown, h.spearThrowCooldown, h.ultimateCooldown };

        for (int i = 0; i < 4; i++)
        {
            float ratio = total[i] > 0f ? Mathf.Clamp01(remain[i] / total[i]) : 0f;
            cooldownFills[i].fillAmount = ratio;
            abilityTexts[i].text = names[i];
            bool ready = !h.IsDowned && remain[i] <= .01f && h.CanAcceptCombatCommand;
            abilityTexts[i].color = ready ? new Color(1f, .86f, .42f, 1f) : new Color(.66f, .60f, .52f, 1f);
            if (abilityButtons[i] != null) abilityButtons[i].interactable = !h.IsDowned && remain[i] <= .01f;
        }
    }

    void UpdateCommentary(HectorController h, GameManager gm)
    {
        if (!stateInitialized)
        {
            stateInitialized = true;
            lastDowned = h.IsDowned;
            lastWaveActive = spawner != null && spawner.WaveActive;
            lastBossDefeated = gm.BossDefeated;
            lastGateHealth = gm.BaseHealth;
            lastWave = gm.CurrentWave;
            Say(
                GameLanguage.T(
                    "Troy stands. Tell me where the line is weakest.",
                    "Троя стоит. Покажи мне, где строй слабее всего."));
            return;
        }

        if (h.IsDowned != lastDowned)
        {
            Say(h.IsDowned
                ? GameLanguage.T("Hold the gate... I am not finished yet.", "Держите ворота... я ещё не закончил.")
                : GameLanguage.T("I am back. Form the line around me.", "Я снова в строю. Сомкнуть строй вокруг меня."));
        }
        else if (gm.BossDefeated && !lastBossDefeated)
        {
            Say(GameLanguage.T("Menelaus is down. Troy still stands!", "Менелай пал. Троя всё ещё стоит!"));
        }
        else if (gm.BaseHealth < lastGateHealth)
        {
            float ratio = gm.MaxBaseHealth > 0 ? gm.BaseHealth / (float)gm.MaxBaseHealth : 0f;
            Say(ratio <= .5f
                ? GameLanguage.T("The gate is failing! Reinforce it now!", "Ворота не выдержат! Усильте оборону немедленно!")
                : GameLanguage.T("They reached the gate. Push them back!", "Они добрались до ворот. Отбросить их!"));
        }
        else if (spawner != null && !spawner.WaveActive && spawner.WaitingForManualStart && spawner.NextWaveHasBoss)
        {
            int warningWave = Mathf.Clamp(gm.CurrentWave + 1, 1, gm.MaxWaves);
            if (lastBossWarningWave != warningWave)
            {
                lastBossWarningWave = warningWave;
                Say(GameLanguage.T("Menelaus is coming. Save your strongest defense.", "Идёт Менелай. Сберегите сильнейшую оборону."));
            }
        }
        else if (spawner != null && spawner.WaveActive && !lastWaveActive)
        {
            Say(GameLanguage.T($"Wave {gm.CurrentWave}. Hold the formation!", $"Волна {gm.CurrentWave}. Держать строй!"));
        }
        else if (spawner != null && !spawner.WaveActive && lastWaveActive && gm.CurrentWave > 0)
        {
            Say(GameLanguage.T("The wave is broken. Repair and prepare.", "Волна разбита. Чиним оборону и готовимся."));
        }
        else if (spawner != null && spawner.WaveActive && spawner.CurrentWaveTotalEnemies > 0 && lastClosingWave != gm.CurrentWave)
        {
            int remaining = EnemyRegistry.AliveCount;
            if (remaining > 0 && remaining <= Mathf.Max(1, Mathf.CeilToInt(spawner.CurrentWaveTotalEnemies * .25f)))
            {
                lastClosingWave = gm.CurrentWave;
                Say(GameLanguage.T("They are wavering. Finish them!", "Они дрогнули. Добиваем!"));
            }
        }

        lastDowned = h.IsDowned;
        lastWaveActive = spawner != null && spawner.WaveActive;
        lastBossDefeated = gm.BossDefeated;
        lastGateHealth = gm.BaseHealth;
        lastWave = gm.CurrentWave;
    }

    void Say(string line)
    {
        if (commentaryText == null) return;
        commentaryText.text = GameLanguage.T("HECTOR: ", "ГЕКТОР: ") + line;
        RuntimeFileLogger.Event("HECTOR_COMMENT", line);
    }

    void SelectHectorFromHud()
    {
        HectorController h = HectorController.Instance;
        if (h == null || h.IsDowned) return;
        h.SetSelected(true);
        RuntimeFileLogger.Event("HECTOR_INPUT", "Selected from HUD portrait.");
    }

    void UseAbilityFromHud(int index)
    {
        HectorController h = HectorController.Instance;
        if (h == null || h.IsDowned || !h.CanAcceptCombatCommand) return;
        if (!h.Selected) h.SetSelected(true);

        switch (index)
        {
            case 0: h.UseWarCry(); break;
            case 1: h.UseShieldWall(); break;
            case 2: h.UseSpearThrow(); break;
            case 3: h.UseUltimate(); break;
        }
    }

    bool IsMenuBlockingCombat()
    {
        if (menuCanvas == null)
        {
            GameObject menu = GameObject.Find("MenuCanvas");
            menuCanvas = menu != null ? menu.GetComponent<Canvas>() : null;
        }
        if (menuCanvas == null) return false;

        for (int i = 0; i < BlockingMenuNames.Length; i++)
        {
            Transform screen = menuCanvas.transform.Find(BlockingMenuNames[i]);
            if (screen != null && screen.gameObject.activeInHierarchy) return true;
        }
        return false;
    }

    Image AddImage(Transform parent, string name, Vector2 pos, Vector2 size, Sprite sprite)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.sprite = sprite;
        image.raycastTarget = false;
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return image;
    }

    Image AddPanel(Transform parent, string name, Vector2 pos, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.sprite = TroyHudArt.Panel();
        image.type = Image.Type.Sliced;
        image.color = color;
        image.raycastTarget = false;
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return image;
    }

    Text AddText(Transform parent, string name, Vector2 pos, Vector2 size, int fontSize, Color color, TextAnchor alignment, FontStyle style)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Text t = go.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.text = name;
        t.fontSize = fontSize;
        t.fontStyle = style;
        t.color = color;
        t.alignment = alignment;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Truncate;
        t.raycastTarget = false;
        RectTransform rt = t.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return t;
    }
}
