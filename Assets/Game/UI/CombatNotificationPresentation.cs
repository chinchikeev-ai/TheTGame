using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class CombatNotificationPresentation : MonoBehaviour
{
    sealed class PendingComment
    {
        public PatronCommentaryEvent eventType;
        public Sprite icon;
        public int value;
    }

    readonly Queue<PendingComment> pendingComments = new Queue<PendingComment>();
    CanvasGroup group;
    Image portrait;
    Image eventIcon;
    Text patronName;
    Text speech;
    EnemySpawner spawner;
    Canvas menuCanvas;

    bool initialized;
    bool bossWasActive;
    bool lastBossDefeated;
    bool lastWaveActive;
    bool lastHectorDowned;
    int lastWave;
    int lastTowersBuilt;
    int lastTowersSold;
    int lastGate;
    int lastLeaks;
    int lastKillMilestone;
    int lastClosingWave = -1;
    float lastMagicCooldown;
    string lastLanguage;
    DivineGiftType displayedPatron;
    PatronCommentaryEvent lastEvent;
    int lastEventValue;
    float nextSpeechAt;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<CombatNotificationPresentation>() == null)
            new GameObject("CombatNotificationPresentation").AddComponent<CombatNotificationPresentation>();
    }

    void Start()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();
        BindMenuCanvas();
        Build();
    }

    void Build()
    {
        GameObject root = new GameObject("DivinePatronCommentary");
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 84;
        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;
        group = root.AddComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;

        GameObject panelObject = new GameObject("PatronObserverPanel");
        panelObject.transform.SetParent(root.transform, false);
        Image panel = panelObject.AddComponent<Image>();
        panel.sprite = TroyHudArt.Panel();
        panel.type = Image.Type.Sliced;
        panel.color = new Color(1f, 1f, 1f, .94f);
        panel.raycastTarget = false;
        RectTransform panelRect = panel.rectTransform;
        panelRect.anchorMin = panelRect.anchorMax = panelRect.pivot = new Vector2(1f, 1f);
        panelRect.anchoredPosition = new Vector2(-24f, -166f);
        panelRect.sizeDelta = new Vector2(620f, 204f);

        GameObject bubbleObject = new GameObject("PatronSpeechBubble");
        bubbleObject.transform.SetParent(panelObject.transform, false);
        Image bubble = bubbleObject.AddComponent<Image>();
        bubble.color = new Color(.035f, .022f, .016f, .96f);
        bubble.raycastTarget = false;
        RectTransform bubbleRect = bubble.rectTransform;
        bubbleRect.anchorMin = bubbleRect.anchorMax = bubbleRect.pivot = new Vector2(.5f, .5f);
        bubbleRect.anchoredPosition = new Vector2(-102f, -10f);
        bubbleRect.sizeDelta = new Vector2(370f, 142f);

        eventIcon = AddImage(bubbleObject.transform, "EventIcon", new Vector2(-153f, 42f), new Vector2(34f, 34f), TroyHudArt.Icon("gift"));
        patronName = AddText(bubbleObject.transform, "PatronName", "", new Vector2(25f, 43f), new Vector2(300f, 30f), 17, new Color(1f, .72f, .28f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        speech = AddText(bubbleObject.transform, "PatronSpeech", "", new Vector2(0f, -15f), new Vector2(326f, 78f), 14, new Color(.96f, .88f, .76f, 1f), TextAnchor.UpperLeft, FontStyle.Italic);

        portrait = AddImage(panelObject.transform, "SelectedPatronPortrait", new Vector2(202f, 3f), new Vector2(194f, 194f), null);
        portrait.preserveAspect = true;
        group.alpha = 0f;
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || group == null)
        {
            if (group != null) group.alpha = 0f;
            return;
        }

        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();
        if (menuCanvas == null) BindMenuCanvas();
        bool visible = gm.GiftSelected && !IsMenuBlocking();
        group.alpha = visible ? 1f : 0f;
        if (!visible) return;

        if (!initialized || displayedPatron != gm.SelectedGift)
            InitializePatron(gm);

        if (lastLanguage != GameLanguage.Code)
        {
            lastLanguage = GameLanguage.Code;
            patronName.text = DivinePatronCommentaryCatalog.PatronName(displayedPatron);
            speech.text = DivinePatronCommentaryCatalog.Line(displayedPatron, lastEvent, lastEventValue);
        }

        HectorController hector = HectorController.Instance;
        bool hectorDowned = hector != null && hector.IsDowned;
        bool bossActive = HasBoss();
        bool waveActive = spawner != null && spawner.WaveActive;

        if (gm.BossDefeated && !lastBossDefeated)
            QueueComment(PatronCommentaryEvent.BossDefeated, TroyHudArt.Icon("hector"));
        if (bossActive && !bossWasActive)
            QueueComment(PatronCommentaryEvent.BossEntered, TroyHudArt.Icon("boss"));
        if (gm.BaseHealth < lastGate)
            QueueComment(PatronCommentaryEvent.GateDamaged, TroyHudArt.Icon("gate"), lastGate - gm.BaseHealth);
        if (gm.Leaks > lastLeaks)
            QueueComment(PatronCommentaryEvent.EnemyLeaked, TroyHudArt.Icon("enemy"), gm.Leaks - lastLeaks);
        if (hectorDowned != lastHectorDowned)
            QueueComment(hectorDowned ? PatronCommentaryEvent.HectorDowned : PatronCommentaryEvent.HectorRevived, TroyHudArt.Icon("hector"));
        if (gm.CurrentWave > lastWave)
            QueueComment(PatronCommentaryEvent.WaveStarted, TroyHudArt.Icon("enemy"), gm.CurrentWave);
        if (lastWaveActive && !waveActive && gm.CurrentWave > 0)
            QueueComment(PatronCommentaryEvent.WaveCompleted, TroyHudArt.Icon("shield"), gm.CurrentWave);
        if (gm.TowersBuilt > lastTowersBuilt)
            QueueComment(PatronCommentaryEvent.DefenseBuilt, TroyHudArt.Icon("shield"));
        if (gm.TowersSold > lastTowersSold)
            QueueComment(PatronCommentaryEvent.DefenseSold, TroyHudArt.Icon("gold"));
        if (gm.MagicCooldownRemaining > 0f && lastMagicCooldown <= 0f)
            QueueComment(PatronCommentaryEvent.DivinePowerUsed, TroyHudArt.Icon("magic"));
        if (gm.Kills / 10 > lastKillMilestone)
            QueueComment(PatronCommentaryEvent.KillMilestone, TroyHudArt.Icon("sword"), gm.Kills);
        if (waveActive && spawner != null && spawner.CurrentWaveTotalEnemies > 0 && EnemyRegistry.AliveCount > 0 && EnemyRegistry.AliveCount <= Mathf.Max(1, Mathf.CeilToInt(spawner.CurrentWaveTotalEnemies * .25f)))
            QueueOncePerWave(PatronCommentaryEvent.WaveClosing, TroyHudArt.Icon("enemy"), gm.CurrentWave);

        bossWasActive = bossActive;
        lastBossDefeated = gm.BossDefeated;
        lastWaveActive = waveActive;
        lastHectorDowned = hectorDowned;
        lastWave = gm.CurrentWave;
        lastTowersBuilt = gm.TowersBuilt;
        lastTowersSold = gm.TowersSold;
        lastGate = gm.BaseHealth;
        lastLeaks = gm.Leaks;
        lastKillMilestone = gm.Kills / 10;
        lastMagicCooldown = gm.MagicCooldownRemaining;
        PresentNextComment();
    }

    void QueueOncePerWave(PatronCommentaryEvent eventType, Sprite icon, int wave)
    {
        if (lastClosingWave == wave) return;
        lastClosingWave = wave;
        QueueComment(eventType, icon, wave);
    }

    void QueueComment(PatronCommentaryEvent eventType, Sprite icon, int value = 0)
    {
        pendingComments.Enqueue(new PendingComment { eventType = eventType, icon = icon, value = value });
    }

    void PresentNextComment()
    {
        if (pendingComments.Count == 0 || Time.unscaledTime < nextSpeechAt) return;
        PendingComment next = pendingComments.Dequeue();
        Say(next.eventType, next.icon, next.value);
        nextSpeechAt = Time.unscaledTime + 3.2f;
    }

    void InitializePatron(GameManager gm)
    {
        initialized = true;
        displayedPatron = gm.SelectedGift;
        portrait.sprite = LoadPortrait(displayedPatron);
        portrait.color = portrait.sprite != null ? Color.white : new Color(1f, 1f, 1f, 0f);
        patronName.text = DivinePatronCommentaryCatalog.PatronName(displayedPatron);
        lastLanguage = GameLanguage.Code;
        lastWave = gm.CurrentWave;
        lastTowersBuilt = gm.TowersBuilt;
        lastTowersSold = gm.TowersSold;
        lastGate = gm.BaseHealth;
        lastLeaks = gm.Leaks;
        lastKillMilestone = gm.Kills / 10;
        lastMagicCooldown = gm.MagicCooldownRemaining;
        lastWaveActive = spawner != null && spawner.WaveActive;
        lastHectorDowned = HectorController.Instance != null && HectorController.Instance.IsDowned;
        bossWasActive = HasBoss();
        lastBossDefeated = gm.BossDefeated;
        pendingComments.Clear();
        Say(PatronCommentaryEvent.PatronSelected, TroyHudArt.Icon("gift"));
        nextSpeechAt = Time.unscaledTime + 3.2f;
    }

    void Say(PatronCommentaryEvent eventType, Sprite icon, int value = 0)
    {
        lastEvent = eventType;
        lastEventValue = value;
        speech.text = DivinePatronCommentaryCatalog.Line(displayedPatron, eventType, value);
        eventIcon.sprite = icon;
        RuntimeFileLogger.Event("PATRON_COMMENT", $"god={displayedPatron}, event={eventType}, value={value}");
    }

    static Sprite LoadPortrait(DivineGiftType patron)
    {
        Texture2D texture = Resources.Load<Texture2D>("UI/Patrons/" + patron);
        return texture != null
            ? Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(.5f, .5f), 100f)
            : null;
    }

    bool HasBoss()
    {
        foreach (Enemy enemy in EnemyRegistry.All)
            if (enemy != null && enemy.Archetype == EnemyArchetype.Boss && enemy.Health > 0f) return true;
        return false;
    }

    void BindMenuCanvas()
    {
        GameObject menu = GameObject.Find("MenuCanvas");
        menuCanvas = menu != null ? menu.GetComponent<Canvas>() : null;
    }

    bool IsMenuBlocking()
    {
        if (menuCanvas == null) return false;
        string[] names = { "MainMenu", "LevelSelect", "Settings", "PauseMenu", "EndMenu", "ConfirmationModal" };
        for (int i = 0; i < names.Length; i++)
        {
            Transform screen = menuCanvas.transform.Find(names[i]);
            if (screen != null && screen.gameObject.activeInHierarchy) return true;
        }
        return false;
    }

    static Image AddImage(Transform parent, string name, Vector2 position, Vector2 size, Sprite sprite)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.sprite = sprite;
        image.raycastTarget = false;
        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return image;
    }

    static Text AddText(Transform parent, string name, string value, Vector2 position, Vector2 size, int fontSize, Color color, TextAnchor alignment, FontStyle style)
    {
        GameObject go = new GameObject(name);
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
        RectTransform rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return text;
    }
}
