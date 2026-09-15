using UnityEngine;
using UnityEngine.UI;

public sealed class ChapterOneEncounterPresentation : MonoBehaviour
{
    static readonly string[] BlockingMenuNames = { "MainMenu", "LevelSelect", "Settings", "PauseMenu", "EndMenu", "ConfirmationModal" };

    CanvasGroup group;
    Canvas menuCanvas;
    Image portrait;
    Text kicker;
    Text title;
    Text subtitle;
    EnemySpawner spawner;
    int shownEncounter;
    int lastActiveEncounter;
    bool wasEncounterActive;
    bool firstAssaultRepelledShown;
    bool secondFormationShown;
    bool bossWarningShown;
    bool bossArrivalShown;
    float secondFormationAt;
    float hideAt;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<ChapterOneEncounterPresentation>() == null)
            new GameObject("ChapterOneEncounterPresentation").AddComponent<ChapterOneEncounterPresentation>();
    }

    void Start()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();
        Build();
    }

    void Build()
    {
        GameObject canvasObj = new GameObject("ChapterOneEncounterPresentationCanvas");
        canvasObj.transform.SetParent(transform, false);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 87;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;

        GameObject card = new GameObject("EncounterIntroCard");
        card.transform.SetParent(canvasObj.transform, false);
        Image bg = card.AddComponent<Image>();
        bg.sprite = TroyHudArt.Panel();
        bg.type = Image.Type.Sliced;
        bg.color = Color.white;
        RectTransform cr = bg.rectTransform;
        cr.anchorMin = cr.anchorMax = cr.pivot = new Vector2(.5f, .5f);
        cr.anchoredPosition = new Vector2(0, 120);
        cr.sizeDelta = new Vector2(660, 136);

        group = card.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;

        portrait = AddImage(card.transform, "Portrait", new Vector2(-274, 0), new Vector2(94, 94), TroyHudArt.Enemy("infantry"));
        kicker = AddText(card.transform, "", new Vector2(-202, 34), new Vector2(420, 22), 11, new Color(1f, .61f, .18f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        title = AddText(card.transform, "", new Vector2(-202, 4), new Vector2(420, 34), 23, new Color(1f, .88f, .68f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        subtitle = AddText(card.transform, "", new Vector2(-202, -31), new Vector2(420, 38), 12, new Color(.80f, .71f, .61f, 1f), TextAnchor.MiddleLeft, FontStyle.Normal);
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || gm.MapNumber != 1 || gm.GameEnded || IsMenuBlockingCombat())
        {
            if (group != null) group.alpha = 0f;
            return;
        }
        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();
        if (spawner == null || group == null) return;

        bool active = EncounterRuntime.EncounterActive(spawner);
        int encounter = EncounterRuntime.CurrentEncounter(spawner);

        if (wasEncounterActive && !active && lastActiveEncounter == 1 && !firstAssaultRepelledShown)
        {
            firstAssaultRepelledShown = true;
            ShowFirstAssaultRepelled();
        }

        if (firstAssaultRepelledShown && !secondFormationShown && !active && encounter == 1 && Time.unscaledTime >= secondFormationAt)
        {
            secondFormationShown = true;
            ShowSecondFormation();
        }

        if (!bossWarningShown && EncounterRuntime.NextEncounterHasBoss(spawner) && !active)
        {
            bossWarningShown = true;
            ShowBossWarning();
        }

        if (active && encounter > shownEncounter)
        {
            shownEncounter = encounter;
            lastActiveEncounter = encounter;
            if (encounter == EncounterRuntime.MaxEncounters) ShowFinalEncounter();
            else ShowEncounter(encounter, EncounterRuntime.MaxEncounters);
        }

        if (!bossArrivalShown && HasActiveBoss())
        {
            bossArrivalShown = true;
            ShowBossArrival();
        }

        wasEncounterActive = active;
        if (active) lastActiveEncounter = encounter;

        float target = Time.unscaledTime < hideAt ? 1f : 0f;
        group.alpha = Mathf.MoveTowards(group.alpha, target, Time.unscaledDeltaTime * 3.8f);
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

    void ShowEncounter(int encounter, int max)
    {
        portrait.sprite = CurrentEncounterPortrait(encounter);
        kicker.text = GameLanguage.T($"ENCOUNTER {encounter} OF {max}", $"БОЙ {encounter} ИЗ {max}");
        title.text = EncounterTitle(encounter);
        subtitle.text = EncounterSubtitle();
        hideAt = Time.unscaledTime + (encounter <= 2 ? 2.9f : 2.35f);
        RuntimeFileLogger.Event("PRESENTATION", $"Encounter intro shown encounter={encounter}");
    }

    void ShowFirstAssaultRepelled()
    {
        portrait.sprite = TroyHudArt.Icon("shield");
        kicker.text = GameLanguage.T("SHORELINE HOLDS", "БЕРЕГ УДЕРЖАН");
        title.text = GameLanguage.T("FIRST ASSAULT REPELLED", "ПЕРВЫЙ ШТУРМ ОТБИТ");
        subtitle.text = GameLanguage.T("Reinforce the weak route • the Greek beachhead is regrouping", "Усильте слабый маршрут • греческий плацдарм перегруппировывается");
        hideAt = Time.unscaledTime + 3.0f;
        secondFormationAt = Time.unscaledTime + 3.15f;
        RuntimeFileLogger.Event("PRESENTATION", "Opening flow first-assault lull shown");
    }

    void ShowSecondFormation()
    {
        portrait.sprite = EncounterPortrait();
        kicker.text = GameLanguage.T("BEACHHEAD REINFORCED", "ПЛАЦДАРМ УСИЛЕН");
        title.text = GameLanguage.T("SECOND FORMATION READY", "ВТОРОЙ СТРОЙ ГОТОВ");
        subtitle.text = GameLanguage.T("Inspect the next enemy group • redeploy before contact", "Изучите следующую группу врагов • перестройте оборону до контакта");
        hideAt = Time.unscaledTime + 3.25f;
        RuntimeFileLogger.Event("PRESENTATION", "Opening flow Encounter 2 transition shown");
    }

    void ShowBossWarning()
    {
        portrait.sprite = TroyHudArt.Portrait("menelaus");
        kicker.text = GameLanguage.T("FINAL ASSAULT INCOMING", "ПРИБЛИЖАЕТСЯ ФИНАЛЬНЫЙ ШТУРМ");
        title.text = GameLanguage.T("MENELAUS APPROACHES", "МЕНЕЛАЙ ПРИБЛИЖАЕТСЯ");
        subtitle.text = GameLanguage.T("Reinforce the gate • prepare anti-heavy defenses", "Укрепите ворота • подготовьте оборону против тяжёлых целей");
        hideAt = Time.unscaledTime + 3.8f;
    }

    void ShowFinalEncounter()
    {
        portrait.sprite = TroyHudArt.Portrait("menelaus");
        kicker.text = GameLanguage.T("FINAL ENCOUNTER", "ФИНАЛЬНЫЙ БОЙ");
        title.text = GameLanguage.T("THE KING OF SPARTA LANDS", "ЦАРЬ СПАРТЫ ВЫСАДИЛСЯ");
        subtitle.text = GameLanguage.T("Break his escort before he reaches the Trojan gate", "Разбейте сопровождение до того, как он достигнет ворот Трои");
        hideAt = Time.unscaledTime + 3.2f;
    }

    void ShowBossArrival()
    {
        portrait.sprite = TroyHudArt.Portrait("menelaus");
        kicker.text = GameLanguage.T("BOSS ARRIVAL", "ПОЯВЛЕНИЕ БОССА");
        title.text = GameLanguage.T("MENELAUS", "МЕНЕЛАЙ");
        subtitle.text = GameLanguage.T("Commander Aura active • reinforcements will follow", "Аура командира активна • последуют подкрепления");
        hideAt = Time.unscaledTime + 3.4f;
        RuntimeEffects.Instance?.PlayHeroPulse(new Vector3(-13f, .2f, 0f), new Color(.82f, .12f, .04f), 7f, .65f);
    }

    Sprite CurrentEncounterPortrait(int encounter)
    {
        if (encounter == 1) return TroyHudArt.Enemy("infantry");
        return EncounterPortrait();
    }

    Sprite EncounterPortrait()
    {
        if (EncounterRuntime.NextEncounterHeavyCount(spawner) > 0 || EncounterRuntime.NextEncounterShieldCount(spawner) > 0) return TroyHudArt.Enemy("heavy");
        if (EncounterRuntime.NextEncounterArcherCount(spawner) > 0) return TroyHudArt.Enemy("archer");
        if (EncounterRuntime.NextEncounterRunnerCount(spawner) > 0) return TroyHudArt.Enemy("runner");
        return TroyHudArt.Enemy("infantry");
    }

    string EncounterTitle(int encounter)
    {
        if (encounter == 1) return GameLanguage.T("THE FIRST BOATS HIT THE SHORE", "ПЕРВЫЕ ЛОДКИ У БЕРЕГА");
        if (encounter == 2) return GameLanguage.T("THE BEACHHEAD ADVANCES", "ПЛАЦДАРМ ПЕРЕХОДИТ В НАСТУПЛЕНИЕ");
        if (encounter == 3) return GameLanguage.T("HEAVY TROOPS ADVANCE", "ТЯЖЁЛЫЕ ВОЙСКА НАСТУПАЮТ");
        if (encounter == 4) return GameLanguage.T("THE GREEKS PRESS FORWARD", "ГРЕКИ УСИЛИВАЮТ НАТИСК");
        return GameLanguage.T("HOLD THE LINE", "УДЕРЖИВАЙТЕ ЛИНИЮ");
    }

    string EncounterSubtitle()
    {
        if (EncounterRuntime.NextEncounterHeavyCount(spawner) > 0) return GameLanguage.T("Heavy armor detected • Spears and Ballista recommended", "Обнаружена тяжёлая броня • рекомендуются копья и баллисты");
        if (EncounterRuntime.NextEncounterRunnerCount(spawner) > 0) return GameLanguage.T("Fast units detected • slow and area control recommended", "Обнаружены быстрые враги • рекомендуется замедление и контроль зоны");
        if (EncounterRuntime.NextEncounterArcherCount(spawner) > 0) return GameLanguage.T("Ranged support detected • protect exposed lanes", "Обнаружена дальняя поддержка • прикройте открытые линии");
        return GameLanguage.T("Greek infantry is advancing from the beach", "Греческая пехота наступает с берега");
    }

    bool HasActiveBoss()
    {
        foreach (Enemy enemy in EnemyRegistry.All)
            if (enemy != null && enemy.Archetype == EnemyArchetype.Boss && enemy.Health > 0f) return true;
        return false;
    }

    Image AddImage(Transform parent, string name, Vector2 pos, Vector2 size, Sprite sprite)
    {
        GameObject go = new GameObject(name); go.transform.SetParent(parent, false); Image image = go.AddComponent<Image>(); image.sprite = sprite; image.raycastTarget = false;
        RectTransform rt = image.rectTransform; rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f); rt.anchoredPosition = pos; rt.sizeDelta = size; return image;
    }

    Text AddText(Transform parent, string value, Vector2 pos, Vector2 size, int fontSize, Color color, TextAnchor alignment, FontStyle style)
    {
        GameObject go = new GameObject("Text"); go.transform.SetParent(parent, false); Text t = go.AddComponent<Text>(); t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); t.text = value; t.fontSize = fontSize; t.fontStyle = style; t.color = color; t.alignment = alignment; t.raycastTarget = false; t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Truncate;
        RectTransform rt = t.rectTransform; rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f); rt.anchoredPosition = pos; rt.sizeDelta = size; return t;
    }
}
