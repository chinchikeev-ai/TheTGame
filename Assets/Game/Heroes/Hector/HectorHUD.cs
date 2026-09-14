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
    readonly Text[] cooldownTexts = new Text[4];
    Texture2D portraitMaskTexture;
    Sprite portraitMaskSprite;
    Sprite healthTrackSprite;
    Sprite solidSprite;
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
        canvasObj.AddComponent<GraphicRaycaster>();

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;

        root = new GameObject("HectorPanel");
        root.transform.SetParent(canvasObj.transform, false);
        Image panel = root.AddComponent<Image>();
        panel.sprite = Art("Panel");
        panel.type = Image.Type.Simple;
        panel.color = Color.white;
        RectTransform pr = panel.rectTransform;
        pr.anchorMin = pr.anchorMax = pr.pivot = new Vector2(0f, 0f);
        pr.anchoredPosition = new Vector2(18f, 18f);
        pr.sizeDelta = new Vector2(440f, 292f);

        AddImage(root.transform, "HectorPortraitFrame", new Vector2(-137, 76), new Vector2(156, 156), Art("PortraitFrame"));
        Image mask = AddImage(root.transform, "PortraitMask", new Vector2(-133, 76), new Vector2(116, 116), CreatePortraitMask());
        mask.gameObject.AddComponent<Mask>().showMaskGraphic = false;
        Image portrait = AddImage(mask.transform, "HectorPortrait", Vector2.zero, new Vector2(116, 116), Art("Portrait"));
        portrait.raycastTarget = true;
        Button portraitButton = portrait.gameObject.AddComponent<Button>();
        portraitButton.targetGraphic = portrait;
        portraitButton.onClick.AddListener(SelectHectorFromHud);

        AddImage(root.transform, "HectorNameBanner", new Vector2(76, 106), new Vector2(240, 46), Art("Nameplate"));
        nameText = AddText(root.transform, "HECTOR", new Vector2(76, 106), new Vector2(182, 28), 22, new Color(1f, .93f, .72f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        nameText.gameObject.name = "HectorName";
        hpText = AddText(root.transform, "", new Vector2(76, 70), new Vector2(234, 22), 16, new Color(.94f, .86f, .72f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        hpText.gameObject.name = "HectorHealthText";

        Sprite healthSource = Art("HealthTrack");
        // The authored bar occupies the middle strip of its source canvas.
        healthTrackSprite = Sprite.Create(healthSource.texture, new Rect(0, healthSource.texture.height * .33f, healthSource.texture.width, healthSource.texture.height * .47f), new Vector2(.5f, .5f));
        AddImage(root.transform, "HectorHealthFrame", new Vector2(76, 46), new Vector2(234, 24), healthTrackSprite);
        GameObject track = new GameObject("HealthTrack");
        track.transform.SetParent(root.transform, false);
        Image trackImage = track.AddComponent<Image>();
        trackImage.color = new Color(.15f, .06f, .035f, .95f);
        RectTransform tr = trackImage.rectTransform;
        tr.anchorMin = tr.anchorMax = tr.pivot = new Vector2(.5f, .5f);
        tr.anchoredPosition = new Vector2(76, 46);
        tr.sizeDelta = new Vector2(215, 12);
        trackImage.raycastTarget = false;

        GameObject fillObj = new GameObject("HealthFill");
        fillObj.transform.SetParent(track.transform, false);
        hpFill = fillObj.AddComponent<Image>();
        solidSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height), new Vector2(.5f, .5f));
        hpFill.sprite = solidSprite;
        hpFill.color = new Color(.88f, .055f, .035f, 1f);
        hpFill.raycastTarget = false;
        hpFill.type = Image.Type.Filled;
        hpFill.fillMethod = Image.FillMethod.Horizontal;
        RectTransform fr = hpFill.rectTransform;
        fr.anchorMin = Vector2.zero;
        fr.anchorMax = Vector2.one;
        fr.offsetMin = new Vector2(2, 2);
        fr.offsetMax = new Vector2(-2, -2);

        commandText = AddText(root.transform, "", new Vector2(76, 22), new Vector2(234, 20), 12, new Color(1f, .76f, .28f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
        commandText.gameObject.name = "HectorStatus";
        AddImage(root.transform, "HectorQuoteParchment", new Vector2(0, -17), new Vector2(390, 46), Art("Parchment"));
        commentaryText = AddText(root.transform, "", new Vector2(0, -17), new Vector2(352, 36), 14, new Color(.20f, .10f, .045f, 1f), TextAnchor.MiddleCenter, FontStyle.Italic);
        commentaryText.gameObject.name = "HectorCommentary";

        for (int i = 0; i < 4; i++)
        {
            float x = -144 + i * 96;
            GameObject slot = new GameObject("Ability_" + abilityKeys[i]);
            slot.transform.SetParent(root.transform, false);
            Image bg = slot.AddComponent<Image>();
            bg.sprite = Art("AbilityButton");
            bg.color = Color.white;
            RectTransform sr = bg.rectTransform;
            sr.anchorMin = sr.anchorMax = sr.pivot = new Vector2(.5f, .5f);
            sr.anchoredPosition = new Vector2(x, -91);
            sr.sizeDelta = new Vector2(82, 82);

            int abilityIndex = i;
            Button button = slot.AddComponent<Button>();
            button.targetGraphic = bg;
            button.onClick.AddListener(() => UseAbilityFromHud(abilityIndex));
            abilityButtons[i] = button;

            AddImage(slot.transform, "Icon", new Vector2(0, 6), new Vector2(52, 52), Art(abilityArt[i]));
            Text keyLabel = AddText(slot.transform, abilityKeys[i], new Vector2(-29, 28), new Vector2(18, 18), 12, new Color(1f, .95f, .72f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);
            keyLabel.gameObject.AddComponent<Outline>().effectDistance = new Vector2(1, -1);
            abilityTexts[i] = AddText(slot.transform, "", new Vector2(0, -22), new Vector2(68, 16), 10, new Color(.96f, .86f, .70f, 1f), TextAnchor.MiddleCenter, FontStyle.Bold);

            GameObject cd = new GameObject("Cooldown");
            cd.transform.SetParent(slot.transform, false);
            cooldownFills[i] = cd.AddComponent<Image>();
            cooldownFills[i].sprite = solidSprite;
            cooldownFills[i].color = new Color(.02f, .015f, .01f, .72f);
            cooldownFills[i].type = Image.Type.Filled;
            cooldownFills[i].fillMethod = Image.FillMethod.Radial360;
            cooldownFills[i].fillOrigin = 2;
            cooldownFills[i].fillClockwise = false;
            cooldownFills[i].raycastTarget = false;
            RectTransform cr = cooldownFills[i].rectTransform;
            cr.anchorMin = Vector2.zero;
            cr.anchorMax = Vector2.one;
            cr.offsetMin = new Vector2(14, 21);
            cr.offsetMax = new Vector2(-14, -9);
            cooldownTexts[i] = AddText(slot.transform, "", new Vector2(0, 6), new Vector2(50, 28), 22, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);
            cooldownTexts[i].gameObject.name = "CooldownSeconds";
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
            nameText.text = GameLanguage.T("HECTOR", "ГЕКТОР");
            hpText.text = GameLanguage.T("REVIVE ", "ВОЗВРАЩЕНИЕ ") + Mathf.CeilToInt(h.DownedRemaining) + GameLanguage.T("s", "с");
            hpFill.fillAmount = 0f;
            commandText.text = GameLanguage.T("DOWNED", "ПОВЕРЖЕН");
            commandText.color = new Color(.74f, .48f, .34f, 1f);
            return;
        }

        nameText.text = GameLanguage.T("HECTOR", "ГЕКТОР");
        hpText.text = $"HP {Mathf.CeilToInt(h.Health)} / {Mathf.CeilToInt(h.maxHealth)}";
        hpFill.fillAmount = h.maxHealth > 0f ? Mathf.Clamp01(h.Health / h.maxHealth) : 0f;
        commandText.text = h.Selected
            ? GameLanguage.T("SELECTED", "ВЫБРАН")
            : GameLanguage.T("PRINCE OF TROY", "ПРИНЦ ТРОИ");
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
            cooldownTexts[i].text = remain[i] > .01f ? Mathf.CeilToInt(remain[i]).ToString() : "";
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
        commentaryText.text = line;
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

    static Sprite Art(string name)
    {
        Sprite sprite = Resources.Load<Sprite>("HectorHud/" + name);
        if (sprite == null) throw new System.InvalidOperationException("Missing Hector HUD sprite: " + name);
        return sprite;
    }

    Sprite CreatePortraitMask()
    {
        portraitMaskTexture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
        var pixels = new Color[128 * 128];
        for (int y = 0; y < 128; y++)
            for (int x = 0; x < 128; x++)
                pixels[y * 128 + x] = new Color(1, 1, 1, Mathf.Clamp01(63.5f - Vector2.Distance(new Vector2(x, y), new Vector2(63.5f, 63.5f))));
        portraitMaskTexture.SetPixels(pixels);
        portraitMaskTexture.Apply();
        portraitMaskSprite = Sprite.Create(portraitMaskTexture, new Rect(0, 0, 128, 128), new Vector2(.5f, .5f));
        return portraitMaskSprite;
    }

    void OnDestroy()
    {
        Destroy(portraitMaskSprite);
        Destroy(portraitMaskTexture);
        Destroy(healthTrackSprite);
        Destroy(solidSprite);
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
